using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMarkConsole;
//[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60)]
//[SimpleJob(RuntimeMoniker.Net70)]
[RPlotExporter]
public class NullableBenchmark
{
    private static readonly int? x = (new Random()).Next();
    private static readonly int? y = null;

    // 0.60 ns (netcore 6)
    [Benchmark]
    public bool CheckObject()
    {
        return x == null;
    }

    // 0.10 ns (netcore 6)
    [Benchmark]
    public bool CheckNullable()
    {
        return x.HasValue;
    }

    // 0.30ns (netcore 6)
    [Benchmark]
    public bool CheckObjectInverted()
    {
        return y == null;
    }

    // 0.18 ns (netcore 6)
    [Benchmark]
    public bool CheckNullableInverted()
    {
        return y.HasValue;
    }
}
