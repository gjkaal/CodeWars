using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMarkConsole;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
public class SwitchOrTryparseOrIfBenchmark
{
    public enum BenchmarkEnum
    {
        None = 0,
        E = 1,
        B = 2,
        D = 3,
    }

    private static readonly string[] Values =
    {
        "X",
        "E",
        "B",
        "D"
    };

    // 450ns 
    public void UseTryparseBenchmark()
    {
        foreach(var s in Values)
        {
            _ = UseTryParse(s);
        }
    }

    [Benchmark]
    // 70ns
    public void UseSwitchBenchmark()
    {
        foreach (var s in Values)
        {
            _ = UseSwitch(s);
        }
    }

    [Benchmark]
    // 15 ns
    public void UseSwitchOptimizedBenchmark()
    {
        foreach (var s in Values)
        {
            _ = UseSwitchOptimized(s);
        }
    }

    // 80ns
    public void UseIfBenchmark()
    {
        foreach (var s in Values)
        {
            _ = UseIf(s);
        }
    }

    private BenchmarkEnum UseTryParse(string value)
    {
        return (Enum.TryParse<BenchmarkEnum>(value, true, out var result))
            ? result
            : BenchmarkEnum.None;
    }

    private BenchmarkEnum UseSwitch(string value)
    {
        switch(value.ToUpperInvariant()) {
            case "E": return BenchmarkEnum.E;
            case "B": return BenchmarkEnum.B;
            case "D": return BenchmarkEnum.D;
            default: return BenchmarkEnum.None;
        }
    }

    private BenchmarkEnum UseSwitchOptimized(string value)
    {
        switch (value[0])
        {
            case 'E':
            case 'e':
                return BenchmarkEnum.E;
            case 'B':
            case 'b':
                return BenchmarkEnum.B;
            case 'D':
            case 'd':
                return BenchmarkEnum.D;
            default: return BenchmarkEnum.None;
        }
    }

    private BenchmarkEnum UseIf(string value)
    {
        var valueToTest = value.ToUpperInvariant();
        if (valueToTest == "E") return BenchmarkEnum.E;
        if (valueToTest == "B") return BenchmarkEnum.B;
        if (valueToTest == "D") return BenchmarkEnum.D;
        return BenchmarkEnum.None;
    }
}