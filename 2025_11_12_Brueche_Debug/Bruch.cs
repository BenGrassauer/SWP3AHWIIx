using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

class Bruch
{
    private int neuereNenner;
    private int neuerZaehler;
    private int zaehler;
    private int nenner;

    public Bruch(string bruchtext)
    {
        string[] ganzzahlteil = bruchtext.Split(' ');
        if (ganzzahlteil.Length == 1)
        { string[] teile = ganzzahlteil[0].Split('/');
            this.nenner = int.Parse(teile[1]);
            this.zaehler = int.Parse(teile[0]) + this.nenner * int.Parse(ganzzahlteil[0]);
        }
        else
        {
            string[] teile = ganzzahlteil[1].Split('/');
            this.nenner = int.Parse(teile[1]);
            this.zaehler = int.Parse(teile[0]);
        }
    }

    public Bruch addiere(Bruch b)
    {
        this.neuereNenner = kgv(this.nenner, b.nenner);
        this.neuerZaehler =
            (this.neuereNenner / this.nenner) * this.zaehler
            + (this.neuereNenner / b.nenner) * b.zaehler;
        return new Bruch(this.neuerZaehler + "/" + this.neuereNenner);
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

    public static string ToString(Bruch b)
    {
        int ganzzahligerAnteil = b.zaehler / b.nenner;
        b.zaehler = b.zaehler % b.nenner;
        return ganzzahligerAnteil + " " + b.zaehler + "/" + b.nenner;
    }
}
