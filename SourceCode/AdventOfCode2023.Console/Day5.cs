using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AdventOfCode2023.Console;
public class Day5Calculator
{
    private readonly Action<string> StdOut;
    private Almanak _almanak;

    public List<long> Seeds => _almanak.Seeds;
    public List<(long RangeStart, long RangeEnd)> SeedRanges => _almanak.SeedRanges;
    public List<Mapping> Mappings => _almanak.Mappings;

    public Day5Calculator(Action<string> _)
    {
        StdOut = System.Console.WriteLine;
        _almanak = new Almanak();
    }

    public long FindLowestSeedRangeLocation(int timeoutInSeconds)
    {
        StdOut.Invoke($"Start FindLowestSeedRangeLocation");
        var t = new Stopwatch();
        t.Start();
        var location = 0L;
        var set = _almanak.Mappings[_almanak.Mappings.Count - 1];
        var totalSet = set.Maps.Select(m => m.DestinationEnd - m.DestinationStart).Max();
        while(location < long.MaxValue) {
            ResetLog();
            if(IsSolution(location))
            {
                StdOut.Invoke($"Found solution {location}");
                return location;
            }
            location++;
            if (StatusUpdate(t, timeoutInSeconds, totalSet, location))
            {
                return -1;
            }
        }
        throw new Exception("No solution found");
    }

    private void ResetLog()
    {
        Logs.Clear();
    }
    private readonly List<string> Logs = new();

    private bool IsSolution(long location)
    {
        var seed = FindSeed(location, _almanak.Mappings.Count - 1);
        return seed >= 0
            && _almanak.SeedRanges.Any(r => r.RangeStart <= seed && r.RangeEnd > seed);
    }

    private long FindSeed(long location, int i)
    {
        var value=0L;
        try
        {
            if (i < 0)
            {
                Logs.Add($"Seed value: {location}");
                return location;
            }
            var mapping = _almanak.Mappings[i];
            Logs.Add($"Mapping: {mapping.Name}");
            value = mapping.MapValueReverse(location);
            Logs.Add($"Mapping: {location} =>  {value}");
            if (value >= 0)
            {
                return FindSeed(value, i - 1);
            }
            else
            {
                Logs.Add("No mapping found");
                return -1;
            }                
        }
        catch(Exception e) { 
            StdOut.Invoke($"Error {e.Message}");
            StdOut.Invoke($"Location: {location}, i: {i}, value: {value}");
            StdOut.Invoke($"Logs: \r\n{string.Join("\r\n", Logs)}");
            throw;
        }
    }

    private long FindLowestSeedRangeLocationTopToBottom(int timeoutInSeconds)
    {
        StdOut.Invoke($"Start FindLowestSeedRangeLocation");
        var t = new Stopwatch();
        long location = long.MaxValue;
        t.Start();
        var totalSet = _almanak.SeedRanges.Select(m => m.RangeEnd - m.RangeStart).Max();
        var countUp = 0;
        foreach (var (rangeStart, rangeEnd) in _almanak.SeedRanges)
        {
            StdOut.Invoke($"Checking range {rangeStart} - {rangeEnd}");
            for (var seed = rangeStart; seed < rangeEnd; seed++)
            {
                countUp++;
                var seedLocation = FindSeedLocation(seed, 0);
                if (seedLocation < location) location = seedLocation;
                if (StatusUpdate(t, timeoutInSeconds, totalSet, countUp))
                {
                    return -1;
                }
            }
        };
        return location;
    }

    private bool StatusUpdate(Stopwatch t, int timeoutInSeconds, long totalSet, long current)
    {
        if (t.Elapsed.TotalSeconds > timeoutInSeconds)
        {
            StdOut.Invoke($"Timeout after {timeoutInSeconds} seconds. {totalSet - current} seeds left to check.");
            StdOut.Invoke($"Currently at {Math.Round((double)(current / totalSet) * 100)} percent completed {current} out of {totalSet}");
            StdOut.Invoke($"Estimate {(totalSet / current) * timeoutInSeconds} seconds to complete");
            return true;
        }
        if (t.Elapsed.Seconds > 60)
        {
            t.Restart();
            var pct = Math.Round((double)(current / totalSet) * 100);           
            StdOut.Invoke($"Currently at {pct} percent completed {current} out of {totalSet}");           
        }
        return false;
    }

    public long FindLowestSeedLocation()
    {
        long location = long.MaxValue;
        foreach (var seed in _almanak.Seeds)
        {
            var seedLocation = FindSeedLocation(seed, 0);
            if (seedLocation < location)
            {
                location = seedLocation;
            }
        }
        return location;
    }

    public long FindSeedLocation(long seed, int index)
    {
        if (index >= _almanak.Mappings.Count) return seed;
        var mapping = _almanak.Mappings[index];
        if (mapping.MapResults.ContainsKey(seed)) return mapping.MapResults[seed];

        var result = mapping.MapValue(seed);
        result = FindSeedLocation(result, index + 1);
        mapping.MapResults.Add(seed, result);
        return result;
    }

    private const string seedMarker = "seeds:";

    public void ReadAlmanakFile(Stream fileData)
    {
        using var reader = new StreamReader(fileData, leaveOpen: true);
        var result = new Almanak();

        // First not empty line are the seeds
        var currentLine = reader.ReadLine();
        while (currentLine is not null && !currentLine.StartsWith(seedMarker, StringComparison.OrdinalIgnoreCase))
        {
            currentLine = reader.ReadLine();
        }
        if (currentLine is null)
        {
            throw new Exception("No seeds found");
        }

        ReadSeedValues(result, currentLine);

        currentLine = reader.ReadLine();

        while (currentLine is not null)
        {
            currentLine = reader.ReadLine();
            if (!string.IsNullOrEmpty(currentLine))
            {
                // Read mapping
                var mapping = new Mapping { Name = currentLine[..^1] };
                currentLine = reader.ReadLine();
                while (currentLine is not null && currentLine != string.Empty)
                {
                    mapping.Maps.Add(currentLine.ParseDestinationMap());
                    currentLine = reader.ReadLine();
                }
                result.Mappings.Add(mapping);
            }
        }

        _almanak = result;
    }

    private static void ReadSeedValues(Almanak result, string currentLine)
    {
        var seeds = currentLine.Substring(seedMarker.Length).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var seed in seeds)
        {
            result.Seeds.Add(Convert.ToInt64(seed));
        }
        for (var i = 0; i < result.Seeds.Count; i += 2)
        {
            var seed = result.Seeds[i];
            var rangeLength = result.Seeds[i + 1];
            result.SeedRanges.Add((seed, seed + rangeLength));
        }
        // Verify that there are not overlapping ranges
        foreach (var range in result.SeedRanges)
        {
            foreach (var otherRange in result.SeedRanges)
            {
                if (range == otherRange) continue;
                if (range.RangeStart >= otherRange.RangeStart && range.RangeStart < otherRange.RangeEnd)
                {
                    throw new Exception("Overlapping ranges");
                }
            }
        }
    }

    private class Almanak
    {
        public List<long> Seeds { get; set; } = new List<long>();
        public List<(long RangeStart, long RangeEnd)> SeedRanges { get; set; } = new List<(long RangeStart, long RangeEnd)>();
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();
    }
}

public static class Day5CalculatorExtensions {

    public static long MapValue(this Mapping map, long source)
    {
        var maps = map.Maps.SingleOrDefault(m => source >= m.SourceStart && source < m.SourceEnd);
        if (maps is not null)
        {
            var offset = source - maps.SourceStart;
            return maps.DestinationStart + offset;
        }
        return source;
    }

    public static long MapValueReverse(this Mapping map, long source)
    {
        var maps = map.Maps.SingleOrDefault(m => source >= m.DestinationStart && source < m.DestinationEnd);
        if (maps is not null)
        {
            var offset = source - maps.DestinationStart;
            return maps.SourceStart + offset;
        }
        return source;
    }

    public static DestinationMap ParseDestinationMap(this string line)
    {
        var parts = line.Split(" ");
        var sourceStart = Convert.ToInt64(parts[1]);
        var destinationStart = Convert.ToInt64(parts[0]);
        var length = Convert.ToInt64(parts[2]);
        return new DestinationMap
        {
            SourceStart = sourceStart,
            SourceEnd = sourceStart + length,
            DestinationStart = destinationStart,
            DestinationEnd = destinationStart + length
        };
    }
}

public class DestinationMap
{
    public long SourceStart { get; set; }
    public long SourceEnd { get; set; }
    public long DestinationStart { get; set; }
    public long DestinationEnd { get; set; }
}

public class Mapping
{
    public string Name { get; set; } = string.Empty;
    public List<DestinationMap> Maps { get; } = new List<DestinationMap>();
    public Dictionary<long, long> MapResults { get; } = new();
}
