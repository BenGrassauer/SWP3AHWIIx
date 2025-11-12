// See https://aka.ms/new-console-template for more information
class Program
{
    static void Main(string[] args)
    {
        try
        {
#if DEBUG
            args = new string[] { "3/4", "2/5" }; // Zum Testen
#endif
            if (args.Length != 2)
            {
                throw new Exception("Bitte genau zwei Brüche als Argumente angeben.");
            }
            if (args[0].IndexOf('/') == -1 || args[1].IndexOf('/') == -1)
            {
                throw new Exception(
                    "Die Brüche müssen im Format Zähler/Nenner angegeben werden. z.B. 3/4"
                );
            }
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            Bruch bruch1 = new Bruch(args[0]);
            Bruch bruch2 = new Bruch(args[1]);
            var ergebnis = bruch1.addiere(bruch2);
            Console.WriteLine("Ergebnis: " + ergebnis);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
