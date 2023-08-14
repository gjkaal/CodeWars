using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace BenchMarkConsole;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
public class StringReplaceBenchmarks
{
    private readonly string SourceString = "This is a test string where characters wil be removed";

    [Benchmark]
    public string ReplaceWithStringReplace()
    {
        return RemoveCharsWithStringReplace(SourceString, new[] { "e", "r" });
    }

    [Benchmark]
    public string ReplaceOptimized()
    {
        return RemoveCharsFromStringFast(SourceString, new[] { 'e', 'r' });
    }

    [Benchmark]
    public string ReplaceWithStringReplaceNoChanges()
    {
        return RemoveCharsWithStringReplace(SourceString, new[] { "q", "\n" });
    }

    [Benchmark]
    public string ReplaceOptimizedNoChanges()
    {
        return RemoveCharsFromStringFast(SourceString, new[] { 'q', '\n' });
    }

    public static string RemoveCharsWithStringReplace(string value, string[] toRemove)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        foreach (var c in toRemove)
        {
            value = value.Replace(c, "", StringComparison.InvariantCulture);
        }
        return value;
    }

    public static string RemoveCharsFromStringFast(string value, char[] toRemove)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var target = new char[value.Length];
        var targetCount = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            var found = false;
            for (int j = 0; j < toRemove.Length; j++)
            {
                var c2 = toRemove[j];
                if (c.CompareTo(c2) != 0)
                {
                    found = true;
                    break;
                }
            }
            if (found) continue;
            target[targetCount] = c;
            targetCount++;
        }
        return new string(target, 0, targetCount);
    }
}