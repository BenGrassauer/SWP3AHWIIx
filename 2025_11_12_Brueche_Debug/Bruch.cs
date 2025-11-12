using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

class Bruch
{
    private int neuereNenner;
    private int neuerZaehler;
    private int zaehler;
    private int nenner;
    private int ganzzahligerAnteil;

    public Bruch(string bruchtext)
    {
        string[] teile = bruchtext.Split('/');
        this.zaehler = int.Parse(teile[0]);
        this.nenner = int.Parse(teile[1]);
    }

    public string addiere(Bruch b)
    {
        if (this.nenner == b.nenner)
        {
            neuerZaehler = this.zaehler + b.zaehler;
            neuereNenner = this.nenner;
        }
        else
        {
            neuereNenner = kgv(this.nenner, b.nenner);
            neuerZaehler =
                neuereNenner / this.nenner * this.zaehler + neuereNenner / b.nenner * b.zaehler;
            Console.WriteLine(kgv(this.nenner, b.nenner));
        }
        if (neuerZaehler > neuereNenner)
        {
            ganzzahligerAnteil = neuerZaehler / neuereNenner;
            neuerZaehler = neuerZaehler % neuereNenner;
        }
        if (ganzzahligerAnteil == 0)
        {
            return neuerZaehler + "/" + neuereNenner;
        }
        else
        {
            return ganzzahligerAnteil.ToString()
                + " "
                + neuerZaehler.ToString()
                + "/"
                + neuereNenner.ToString();
        }
    }

    private int kgv(int a, int b)
    {
        int max = Math.Max(a, b);
        int min = Math.Min(a, b);
        int kgv = max;
        while (kgv % min != 0)
        {
            kgv += max;
        }
        return kgv;
    }

    public static string toString(Bruch b)
    {
        return b.zaehler + "/" + b.nenner;
    }
}
