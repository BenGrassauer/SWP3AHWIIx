public class Einkaufsliste
{
    private readonly string[] artikel = new string[10];
    private int anzahl;

    public int Anzahl => anzahl;

    public bool VersucheHinzufuegen(string artikelName, out string meldung)
    {
        if (anzahl >= artikel.Length)
        {
            meldung = "Fehler: Die Einkaufsliste ist voll.";
            return false;
        }

        artikel[anzahl] = artikelName;
        anzahl++;
        meldung = $"'{artikelName}' wurde hinzugefuegt.";
        return true;
    }

    public bool Enthaelt(string gesuchterArtikel)
    {
        bool gefunden = false;

        for (int i = 0; i < anzahl; i++)
        {
            if (artikel[i] == gesuchterArtikel)
            {
                gefunden = true;
                break;
            }
        }

        return gefunden;
    }

    public void GibKurzeNamenAus(int minLaenge)
    {
        for (int i = 0; i < anzahl; i++)
        {
            if (artikel[i].Length >= minLaenge)
            {
                continue;
            }

            Console.WriteLine(artikel[i]);
        }
    }
}

public class Program
{
    public static void Main()
    {
        Einkaufsliste liste = new Einkaufsliste();
        liste.VersucheHinzufuegen("Milch", out _);
    }
}
