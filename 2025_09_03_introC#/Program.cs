using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Verwendung: dotnet run \"2 3/8\" \"1 5/6\"");
        }

        var first = ParseMixedFraction(args[0]);
        var second = ParseMixedFraction(args[1]);

        var result = first + second;

        Console.WriteLine(ToMixedFraction(result));
    }

    static Fraction ParseMixedFraction(string input)
    {
        string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        int whole = 0;
        Fraction frac = new Fraction(0, 1);

        if (parts.Length == 2)
        {
            whole = int.Parse(parts[0]);
            frac = ParseFraction(parts[1]);
        }
        else if (parts.Length == 1)
        {
            if (parts[0].Contains('/'))
                frac = ParseFraction(parts[0]);
            else
                whole = int.Parse(parts[0]);
        }

        return new Fraction(whole * frac.Denominator + frac.Numerator, frac.Denominator);
    }

    static Fraction ParseFraction(string part)
    {
        string[] nums = part.Split('/');
        return new Fraction(int.Parse(nums[0]), int.Parse(nums[1]));
    }

    static string ToMixedFraction(Fraction frac)
    {
        frac = frac.Simplify();

        int whole = frac.Numerator / frac.Denominator;
        int remainder = frac.Numerator % frac.Denominator;

        if (remainder == 0)
            return whole.ToString();

        if (whole == 0)
            return $"{remainder}/{frac.Denominator}";
        else
            return $"{whole} {remainder}/{frac.Denominator}";
    }
}

public struct Fraction
{
    public int Numerator { get; }
    public int Denominator { get; }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Nenner darf nicht 0 sein.");

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        Numerator = numerator;
        Denominator = denominator;
    }

    public Fraction Simplify()
    {
        int gcd = GCD(Math.Abs(Numerator), Denominator);
        return new Fraction(Numerator / gcd, Denominator / gcd);
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int numerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        int denominator = a.Denominator * b.Denominator;
        return new Fraction(numerator, denominator).Simplify();
    }

    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
}
