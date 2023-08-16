using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CoreElements;

namespace BenchMarkConsole;

[SimpleJob(RuntimeMoniker.Net70)]
[RPlotExporter]
public class ListsBenchmark
{
    private readonly static Random random = new Random();
    private readonly static List<int> systemList;
    private readonly static ElementList<int> elementList;
    private readonly static int[] items = new int[56];

    static ListsBenchmark()
    {
        for(int i = 0; i < items.GetLength(0); i++) { 
            items[i] = random.Next(250); 
        }

        systemList = new List<int>(items);
        elementList = new ElementList<int>(items);
    }

    public int CreateList()
    { 
        var item = new List<int>(items);
        return item.Count;
    }

    public int CreateElementList()
    {
        var item = new List<int>(items);
        return item.Count;
    }

    
    public int[] ListToArray() { 
        return systemList.ToArray();
    }

    
    public int[] ElementListToArray()
    {
        return elementList.ToArray();
    }

    [Benchmark]
    public int ArrayToList()
    {
        var item = items.ToList();
        return item.Count;
    }

    [Benchmark]
    public int ArrayToElementList()
    {
        var item = items.ToElementList();
        return item.Count;
    }
}

//[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60)]
//[SimpleJob(RuntimeMoniker.Net70)]
[RPlotExporter]
public class FindExtensionBenchmark
{
    public static ReadOnlySpan<char> GetExtensionWithSpan(string value)
    {
        return value.AsSpan(value.LastIndexOf('.') + 1);
    }

    public static string GetExtensionWithSubstring(string value)
    {
        return value.Split('.').Last();
    }

    [Benchmark]
    public bool CheckWithSpan()
    {
        var ext = GetExtensionWithSpan("filenaam.with.several.items.txt");
        return ext.CompareTo("txt".AsSpan(), StringComparison.Ordinal) == 0;
    }

    [Benchmark]
    public bool CheckWithSubstring()
    {
        var ext = GetExtensionWithSubstring("filenaam.with.several.items.txt");
        return ext.CompareTo("txt") == 0;
    }
}
