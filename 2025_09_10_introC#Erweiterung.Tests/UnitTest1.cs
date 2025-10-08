namespace _2025_09_10_introCs_Erweiterung.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var frac1 = new Fraction(1, 2);
        var frac2 = new Fraction(1, 3);
        var result = frac1 + frac2;
        Assert.Equal("5/6", result.ToString());
    }
}
