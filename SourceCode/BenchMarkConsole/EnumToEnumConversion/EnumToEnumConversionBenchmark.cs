using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMarkConsole.EnumToEnumConversion;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
public class EnumToEnumConversionBenchmark
{
    public static CodeGebouwdOngebouwd? GetGebouwdOngebouwdcodeUsingIf(GebouwdOngebouwdCode value)
    {
        if (value.Equals(GebouwdOngebouwdCode.G) == true)
        {
            return CodeGebouwdOngebouwd.G;
        }

        if (value.Equals(GebouwdOngebouwdCode.O) == true)
        {
            return CodeGebouwdOngebouwd.O;
        }

        if (value.Equals(GebouwdOngebouwdCode.B) == true)
        {
            return CodeGebouwdOngebouwd.B;
        }

        return null;
    }

    public static CodeGebouwdOngebouwd? GetGebouwdOngebouwdcodeUsingSwitch(GebouwdOngebouwdCode value)
    {
        return value switch
        {
            GebouwdOngebouwdCode.B => (CodeGebouwdOngebouwd?)CodeGebouwdOngebouwd.B,
            GebouwdOngebouwdCode.O => (CodeGebouwdOngebouwd?)CodeGebouwdOngebouwd.O,
            GebouwdOngebouwdCode.G => (CodeGebouwdOngebouwd?)CodeGebouwdOngebouwd.G,
            _ => null,
        };
    }

    private static readonly GebouwdOngebouwdCode[] Values =
{
        GebouwdOngebouwdCode.B,
        GebouwdOngebouwdCode.O,
        GebouwdOngebouwdCode.G,
        GebouwdOngebouwdCode.B,
        GebouwdOngebouwdCode.Z,
        GebouwdOngebouwdCode.Z,
        GebouwdOngebouwdCode.G,
        GebouwdOngebouwdCode.G,
        GebouwdOngebouwdCode.B,
        GebouwdOngebouwdCode.O,
    };

    [Benchmark]
    public void UseStatementsBenchmarkIf()
    {
        foreach (var s in Values)
        {
            _ = GetGebouwdOngebouwdcodeUsingIf(s);
        }
    }

    [Benchmark]
    public void UseStatementsBenchmarkSwitch()
    {
        foreach (var s in Values)
        {
            _ = GetGebouwdOngebouwdcodeUsingSwitch(s);
        }
    }
}
