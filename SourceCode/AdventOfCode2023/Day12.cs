namespace AdventOfCode2023;

[TestClass]
public class Day12
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1";

    [DataTestMethod]
    [DataRow(0, 3, 0)]
    [DataRow(1, 3, 1)]
    [DataRow(2, 3, 2)]
    [DataRow(3, 3, 10)]
    [DataRow(4, 3, 11)]
    [DataRow(5, 3, 12)]
    [DataRow(6, 3, 20)]
    [DataRow(10, 10, 10)]
    public void TestToBase(int value, int targetBase, int expected) => Assert.AreEqual(expected, value.ToBase(targetBase));

    [TestMethod]
    public void MapEnumeratorTest()
    {
        var enumerator = new MapEnumerator("??..?.##.");
        Assert.AreEqual("......##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual("#.....##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual(".#....##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual("##....##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual("....#.##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual("#...#.##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual(".#..#.##.", enumerator.Current);
        Assert.IsTrue(enumerator.Next());
        Assert.AreEqual("##..#.##.", enumerator.Current);
        StdOut.Invoke($"{enumerator.Counter} of {enumerator.CounterMax}");
        Assert.IsFalse(enumerator.Next());
    }

    [DataTestMethod]
    [DataRow("???.### 1,1,3", 1)]
    [DataRow(".??..??...?##. 1,1,3", 4)]
    [DataRow("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
    [DataRow("????.#...#... 4,1,1", 1)]
    [DataRow("????.######..#####. 1,6,5", 4)]
    [DataRow("?###???????? 3,2,1", 10)]
    public void TestFindArrangements(string value, int expected)
    {
        var conditionRecords = ParseRow(value);
        var result = FindArrangementsBruteForce(conditionRecords, 1);
        Assert.AreEqual(expected, result.Length);

        var resultRecursive = CountArrangements(conditionRecords);
        Assert.AreEqual(expected, resultRecursive);
    }

    [DataTestMethod]
    [DataRow("???.### 1,1,3", 1)]
    [DataRow(".??..??...?##. 1,1,3", 16384)]
    [DataRow("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
    [DataRow("????.#...#... 4,1,1", 16)]
    [DataRow("????.######..#####. 1,6,5", 2500)]
    [DataRow("?###???????? 3,2,1", 506250)]
    public void TestFindArrangementsFolded(string value, int expected)
    {
        var conditionRecords = ParseRow(Unfold(value, 5));
        var result = CountArrangements(conditionRecords);

        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("???.### 1,1,3", "???.###", 1, 1, 3)]
    [DataRow(".??..??...?##. 1,1,3", ".??..??...?##.", 1, 1, 3)]
    [DataRow("?#?#?#?#?#?#?#? 1,3,1,6", "?#?#?#?#?#?#?#?", 1, 3, 1, 6)]
    public void TestParseRow(string value, string expectCondition, params int[] expected)
    {
        var conditionRecords = ParseRow(value);
        var condition = conditionRecords.Hints;
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], condition[i]);
        }
        Assert.AreEqual(expectCondition, conditionRecords.Condition);
    }

    [DataTestMethod]
    [DataRow("#.#.###", 1,1,3)]
    [DataRow(".#...#....###.", 1,1,3)]
    [DataRow(".#.###.#.######", 1, 3, 1, 6)]
    public void TestGetHints(string value, params int[] expected)
    {
        var condition = GetHints(value);
        Assert.AreEqual(expected.Length, condition.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], condition[i]);
        }
    }

    [DataTestMethod]
    [DataRow(".# 1", ".#?.#?.#?.#?.# 1,1,1,1,1")]
    [DataRow("???.### 1,1,3", "???.###????.###????.###????.###????.### 1,1,3,1,1,3,1,1,3,1,1,3,1,1,3")]
    [DataRow("????.#...#... 4,1,1", "????.#...#...?????.#...#...?????.#...#...?????.#...#...?????.#...#... 4,1,1,4,1,1,4,1,1,4,1,1,4,1,1")]
    public void TestUnfold(string value, string expected)
    {
        var result = Unfold(value, 5);
        Assert.AreEqual(expected, result);
    }

    private string Unfold(string value, int foldCount) { 
        var values = value.Split(' ');
        var map = string.Join("?", Enumerable.Repeat(values[0], foldCount));
        var hints = string.Join(",", Enumerable.Repeat(values[1], foldCount));
        return $"{map} {hints}";
    }

    [TestMethod]
    public void TestFindAllArrangements()
    {
        var conditionRecords = example1.Split(Environment.NewLine).Select(x => ParseRow(x)).ToArray();
        var result = CountAllArrangements(conditionRecords);
        Assert.AreEqual(21, result);
    }

    [TestMethod]
    public void TestFindAllArrangementsFromPuzzle()
    {
        var fi = new FileInfo("InputFiles/Day12.txt");
        var map = fi.ReadMapData().First();
        var conditionRecords = map.Select(x => ParseRow(x)).ToArray();
        var result = CountAllArrangements(conditionRecords);
        Assert.AreEqual(7407, result);
    }

    [TestMethod]
    public void TestFindAllArrangementsUnfolded()
    {
        var conditionRecords = example1.Split(Environment.NewLine).Select(x => ParseRow(Unfold(x, 5))).ToArray();
        var result = CountAllArrangements(conditionRecords);
        Assert.AreEqual(525152, result);
    }

    [TestMethod]
    public void TestFindAllArragnementsFromPuzzleUnfolded()
    {
        var fi = new FileInfo("InputFiles/Day12.txt");
        var map = fi.ReadMapData().First();
        var conditionRecords = map.Select(x => ParseRow(Unfold(x, 5))).ToArray();
        Assert.AreEqual(1000, conditionRecords.Length);
        var total = 0L;
        foreach (var conditionRecord in map)
        {
            var unFolded = Unfold(conditionRecord, 5);
            var condition = ParseRow(unFolded);
            var subResult = CountArrangements(condition);
            total += subResult;
        }
       // var result = CountAllArrangements(conditionRecords);

        // 14245284920 is too low
        // 14588454526 is too low
        Assert.IsTrue(total > 14245284920);
        Assert.IsTrue(total > 14588454526);
        Assert.AreEqual(30568243604962, total);
    }

    private long CountAllArrangements(ConditionRecords[] conditionRecords) {
        var count = 0L;
        foreach (var conditionRecord in conditionRecords)
        {
            count += CountArrangements(conditionRecord);
        }
        return count;
    }

    private bool VerifyHintsEqual(int[] hints, string value) {
        var inSet = false;
        var x = 0;
        var result = new List<int>();
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '#')
            {
                if (!inSet)
                {
                    x = i;
                    inSet = true;
                }
            }
            else
            {
                if (inSet)
                {
                    var hintValue = i - x;
                    inSet = false;
                    result.Add(hintValue);
                }
            }
        }
        if (inSet)
        {
            var hintValue = value.Length - x;
            result.Add(hintValue);
        }

        return hints.SequenceEqual(result);
    }

    private int[] GetHints(string value) {
        var inSet = false;
        var x = 0;
        var result = new List<int>();
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '#')
            {
                if (!inSet)
                {
                    x = i;
                    inSet = true;
                }
            }
            else
            {
                if (inSet)
                {
                    result.Add(i - x);
                    inSet = false;
                }
            }
        }
        if (inSet)
        {
            result.Add(value.Length - x);
        }
        return result.ToArray();
    }

    private string[] FindArrangementsBruteForce(ConditionRecords conditionRecords, int foldCount)
    {
        if (foldCount==0) return Array.Empty<string>();

        var hints = conditionRecords.Hints;
        var map = conditionRecords.Condition;
        var solutions = new List<string>();

        var enumerator = new MapEnumerator(map);
        do
        {
            var current = enumerator.Current;
            var currentHints = GetHints(current);
            if (hints.SequenceEqual(currentHints))
            {
                solutions.Add(current);
            }
        } while (enumerator.Next());

        return solutions.ToArray();
    }
   
    private long CountArrangements(ConditionRecords conditionRecord)
    {
        _memoize.Clear();
       return CountWays(conditionRecord.Condition, conditionRecord.Hints);
    }

    private readonly Dictionary<int, long> _memoize = new();
    private long CountWays(string line, int[] runs)
    {
        // A dictionary is used to memoize the results
        // to prevent duplicate calculations
        var hash = $"{line}:{string.Join(',', runs)}".GetHashCode();
        if (_memoize.TryGetValue(hash, out var result))
        {
            return result;
        }
        result = CountWaysInternal(line, runs);
        _memoize.Add(hash, result);
        return result;
    }

    private long CountWaysInternal(string line, int[] runs) 
    {
        if (line.Length == 0)
        {
            // If the line length is 0 and
            // there are no runs left, this
            // is a valid solution
            return (runs.Length == 0)
                ? 1L
                : 0L;
        }
        if (runs.Length == 0)
        {
            // No more runs to check, so this
            // is only a valid solution if there
            // are no blocks left
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    return 0L;
                }
            }
            return 1L;
        }

        // The line is not long enough, so
        // this is not a valid solution
        if (line.Length < runs.Sum() + runs.Length - 1)
        {
            return 0L;
        }

        // If the first character is an empty space,
        // then we can skip it and try again
        if (line[0] == '.')
        {
            return CountWays(line[1..], runs);
        }

        // If the first character is a block, then check if
        // it is the start of a run.
        if (line[0] == '#')
        {
            // Split the runs into the first run and the rest
            var run = runs[0];
            var leftoverRuns = runs[1..];
            for (var i = 0; i < run; i++)
            {
                if (line[i] == '.')
                {
                    return 0L;
                }
            }

            // If the number of elements left is less than the
            // expected run length, then this is not a valid solution
            if (run < line.Length && line[run] == '#')
            {
                return 0L;
            }

            // Check if there are more possible solutions in this line
            return (run + 1) < line.Length
                ? CountWays(line[(run + 1)..], leftoverRuns)
                : CountWays(string.Empty, leftoverRuns);
        }

        // The first character is a placeholder (?)
        // So retry with a branch for each possibility
        return (
          CountWays("#" + line[1..], runs) + CountWays("." + line[1..], runs)
        );
    }

    private int CountArrangementsBruteForce(ConditionRecords conditionRecords)
    {
        var checkHint = conditionRecords.Hints.ToArray();
        var map = conditionRecords.Condition;
        var solution = 0;
        var enumerator = new MapEnumerator(map);
        do
        {
            var current = enumerator.Current;
            if (VerifyHintsEqual(checkHint, current))
            {
                solution++;
            }
        } while (enumerator.Next());

        return solution;
    }


    private ConditionRecords ParseRow(string value)
    {
        try
        {
            var parts = value.Split(' ');
            var condition = parts[0];
            var hints = parts[1].Split(',').Select(x => int.Parse(x)).ToArray();
            return new ConditionRecords { Condition = condition, Hints = hints };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid row: {value}", nameof(value), ex);
        }
    }
}

public class MapEnumerator
{
    private readonly char[] _map;
    private readonly int[] _modMap;
    private int _counter;
    private int _nMax;

    public MapEnumerator(string map)
    {
        _map = map.ToCharArray();
        _modMap = new int[_map.Length];
        if (string.IsNullOrEmpty(map))
        {
            throw new ArgumentException("Invalid map", nameof(map));
        }
        Reset();
    }

    public void Reset()
    {
        _nMax = 1;
        for (int i = 0; i < _map.Length; i++)
        {
            if (_map[i] == '?')
            {
                _modMap[i] = 0;
                _nMax = _nMax * 2;
            }
            else
            {
                _modMap[i] = -1;
            }
        }
        _nMax -= 1;
        _counter = 0;
    }
    
    public int Counter => _counter;
    public int CounterMax => _nMax;

    public string Current { 
        get 
        {
            var f = 1;
            var result = new char[_map.Length];
            for (var i = 0; i < _map.Length; i++)
            {
                if (_modMap[i]<0)
                {
                    result[i] = _map[i];
                }
                else
                {
                    var pf = (_counter / f) % 2;
                    switch (pf)
                    {
                        case 0:
                            result[i] = '.';
                            break;
                        case 1:
                            result[i] = '#';
                            break;
                    }
                    f = f << 1;
                }
            }
            return new string(result);
        } 
    }
    public bool Next() {
        if (_counter < _nMax)
        {
            _counter++;
            return true;
        }
        return false;
    }
}

public static class NumberExtensions
{
    public static int ToBase(this int value, int targetBase)
    {
        List<byte> result = new();
        do
        {
            result.Add((byte)(value % targetBase));
            value = value / targetBase;
        }
        while (value > 0);

        var newBase = 0;
        for (int i = result.Count - 1; i >= 0; i--)
        {
            newBase = newBase * 10 + result[i];
        }
        return newBase;
    }
}

public struct ConditionRecords
{
    public string Condition { get; set; }
    public int[] Hints { get; set; }
}
