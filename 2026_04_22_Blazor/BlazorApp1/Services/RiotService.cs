using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

public class RiotService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private static Dictionary<int, ChampionInfo>? _championCache;
    private static DateTime _championCacheAt = DateTime.MinValue;

    public RiotService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("riot");
        _apiKey = Environment.GetEnvironmentVariable("RIOT_API_KEY") ?? string.Empty;
    }

    private static (string accountHost, string platformHost) MapRegion(string region)
    {
        region = region?.ToUpperInvariant() ?? "EUW";
        return region switch
        {
            "EUW" => ("europe.api.riotgames.com", "euw1.api.riotgames.com"),
            "EUNE" => ("europe.api.riotgames.com", "eun1.api.riotgames.com"),
            "NA" => ("americas.api.riotgames.com", "na1.api.riotgames.com"),
            "KR" => ("asia.api.riotgames.com", "kr.api.riotgames.com"),
            _ => ("europe.api.riotgames.com", "euw1.api.riotgames.com"),
        };
    }

    public async Task<RiotAccountDto?> GetAccountByRiotIdAsync(string name, string tagLine, string region)
    {
        var (accountHost, _) = MapRegion(region);
        var url = $"https://{accountHost}/riot/account/v1/accounts/by-riot-id/{Uri.EscapeDataString(name)}/{Uri.EscapeDataString(tagLine)}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(_apiKey)) req.Headers.Add("X-Riot-Token", _apiKey);

        var resp = await _http.SendAsync(req);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        if (resp.StatusCode == (HttpStatusCode)429)
        {
            if (resp.Headers.TryGetValues("Retry-After", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var sec)) await Task.Delay(TimeSpan.FromSeconds(sec + 1));
            }
            resp = await _http.SendAsync(req);
        }

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<RiotAccountDto>();
    }

    public async Task<List<ChampionMasteryDto>> GetChampionMasteriesByPuuidAsync(string puuid, string region)
    {
        var (_, platformHost) = MapRegion(region);
        var url = $"https://{platformHost}/lol/champion-mastery/v4/champion-masteries/by-puuid/{Uri.EscapeDataString(puuid)}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(_apiKey)) req.Headers.Add("X-Riot-Token", _apiKey);

        var resp = await _http.SendAsync(req);
        if (resp.StatusCode == (HttpStatusCode)429)
        {
            if (resp.Headers.TryGetValues("Retry-After", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var sec)) await Task.Delay(TimeSpan.FromSeconds(sec + 1));
            }
            resp = await _http.SendAsync(req);
        }

        resp.EnsureSuccessStatusCode();
        var list = await resp.Content.ReadFromJsonAsync<List<ChampionMasteryDto>>();
        return list ?? new List<ChampionMasteryDto>();
    }

    public async Task<Dictionary<int, ChampionInfo>> GetChampionMappingAsync()
    {
        if (_championCache != null && (DateTime.UtcNow - _championCacheAt) < TimeSpan.FromHours(24))
            return _championCache.ToDictionary(kv => kv.Key, kv => kv.Value);

        var versionsUrl = "https://ddragon.leagueoflegends.com/api/versions.json";
        var versions = await _http.GetFromJsonAsync<string[]>(versionsUrl);
        var version = versions?.FirstOrDefault() ?? "13.1.1";

        var champJsonUrl = $"https://ddragon.leagueoflegends.com/cdn/{version}/data/en_US/champion.json";
        var champData = await _http.GetFromJsonAsync<JsonElement>(champJsonUrl);

        var map = new Dictionary<int, ChampionInfo>();
        if (champData.ValueKind == JsonValueKind.Object && champData.TryGetProperty("data", out var data))
        {
            foreach (var prop in data.EnumerateObject())
            {
                var obj = prop.Value;
                if (!obj.TryGetProperty("key", out var keyProp)) continue;
                if (!int.TryParse(keyProp.GetString(), out var intKey)) continue;

                var name = obj.GetProperty("name").GetString();
                var id = obj.GetProperty("id").GetString();
                var imageFull = obj.GetProperty("image").GetProperty("full").GetString();
                var imageUrl = $"https://ddragon.leagueoflegends.com/cdn/{version}/img/champion/{imageFull}";

                map[intKey] = new ChampionInfo
                {
                    Key = intKey,
                    Id = id,
                    Name = name,
                    ImageUrl = imageUrl
                };
            }
        }

        _championCache = map.ToDictionary(kv => kv.Key, kv => kv.Value);
        _championCacheAt = DateTime.UtcNow;
        return _championCache;
    }
}
