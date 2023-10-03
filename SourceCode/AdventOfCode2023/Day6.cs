
namespace AdventOfCode2023;
[TestClass]
public class Day6
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;
    private readonly Dictionary<int, int> TestData = new Dictionary<int, int>()
    {
        { 57, 291 },
        { 72, 1172 },
        { 69, 1176 },
        { 92, 2026 },
    };

    private readonly Dictionary<int, int> SampleData = new Dictionary<int, int>()
    {
        { 7,9 },
        { 15,40 },
        { 30,200 },
    };

    [DataTestMethod]
    [DataRow(1, 6)]
    [DataRow(2, 10)]
    [DataRow(3, 12)]
    [DataRow(4, 12)]
    [DataRow(5, 10)]
    [DataRow(6, 6)]
    [DataRow(7, 0)]
    [DataRow(1, 6)]
    public void TestDistance(int miliseconds, int expectDistance) => Assert.AreEqual(expectDistance, CalculateDistance(miliseconds, 7));

    [DataTestMethod]
    [DataRow(7, 9, 1, EdgeType.None)]
    [DataRow(7, 9, 2, EdgeType.Lower)]
    [DataRow(7, 9, 3, EdgeType.None)]
    [DataRow(7, 9, 4, EdgeType.None)]
    [DataRow(7, 9, 5, EdgeType.Upper)]
    [DataRow(7, 9, 6, EdgeType.None)]
    [DataRow(30, 200, 10, EdgeType.None)]
    [DataRow(30, 200, 11, EdgeType.Lower)]
    [DataRow(30, 200, 12, EdgeType.None)]
    [DataRow(30, 200, 18, EdgeType.None)]
    [DataRow(30, 200, 19, EdgeType.Upper)]
    [DataRow(30, 200, 20, EdgeType.None)]
    [DataRow(15, 40, 3, EdgeType.None)]
    [DataRow(15, 40, 4, EdgeType.Lower)]
    [DataRow(15, 40, 5, EdgeType.None)]
    [DataRow(15, 40, 10, EdgeType.None)]
    [DataRow(15, 40, 11, EdgeType.Upper)]
    [DataRow(15, 40, 12, EdgeType.None)]
    public void TestFindEdgeType(int totalRaceTime, int currentRecord, int loadTime, EdgeType expectValue)
    {
        Assert.AreEqual(expectValue, FindEdgeType(totalRaceTime, currentRecord, loadTime));
    }

    [DataTestMethod]
    [DataRow(7, 9, 4)]
    [DataRow(15, 40, 8)]
    [DataRow(30, 200, 9)]
    public void TestWinCounts(long raceTimeTotal, long currentRecord, long expectValue) => Assert.AreEqual(expectValue, CountWins(raceTimeTotal, currentRecord));

    [DataTestMethod]
    [DataRow(7, 9, 4)]
    [DataRow(15, 40, 8)]
    [DataRow(30, 200, 9)]
    [DataRow(71530, 940200, 71503)]
    public void TestWinCountsSimple(long raceTimeTotal, long currentRecord, long expectValue) => Assert.AreEqual(expectValue, CountWinsSimple(raceTimeTotal, currentRecord));

    [TestMethod]
    public void VerifySampleData()
    {
        var wins = 1L;
        foreach (var (raceTimeTotal, currentRecord) in SampleData)
        {
            wins = wins * CountWins(raceTimeTotal, currentRecord);
        }
        Assert.AreEqual(288, wins);
    }

    [TestMethod]
    public void VerifyTestData()
    {
        var wins = 1L;
        foreach (var (raceTimeTotal, currentRecord) in TestData)
        {
            wins = wins * CountWins(raceTimeTotal, currentRecord);
        }
        Assert.AreEqual(160816, wins);
    }

    [TestMethod]
    public void VerifyScoringToWin()
    {
        var wins = CountWins(57726992, 291117211762026);
        Assert.AreEqual(46561107, wins);
    }

    private static long CalculateDistance(long loadTime, long raceTimeTotal) { 
        var timeToRace = raceTimeTotal - loadTime;
        return timeToRace * loadTime;
    }

    private long CountWinsSimple(long raceTimeTotal, long currentRecord)
    {
        var start = 0L;
        var distance = 0L;
        while (distance <= currentRecord)
        {
            distance = CalculateDistance(start, raceTimeTotal);
            start++;
        }

        var end = raceTimeTotal;
        distance = 0L;
        while (distance <= currentRecord)
        {
            distance = CalculateDistance(end, raceTimeTotal);
            end--;
        }
        StdOut.Invoke($"Start {start-1} End {end + 1}");
        return end + 1 - (start - 1) + 1;
    }

    [Flags]
    public enum EdgeType
    {
        None = 0,
        Lower = 1,
        Upper = 2,
        Both = 3
    }
    
    private static long CountWins(long raceTimeTotal, long currentRecord)
    {
        var lowerEdge = 0L;
        var upperEdge = 0L;

        var start = raceTimeTotal / 2;
        var step = start / 2;
        var searchDown = true;
        var minimumStepsize = 0;
        var findLowerEdge = true;

        var distance = CalculateDistance(start, raceTimeTotal);
        if (distance < currentRecord)
        {
            throw new Exception("No wins possible");
        }

        while ((lowerEdge == 0 || upperEdge == 0) && minimumStepsize < 3)
        {            
            var edge = FindEdgeType(raceTimeTotal, currentRecord, start);
            if (findLowerEdge && edge.HasFlag(EdgeType.Lower))
            {
                lowerEdge = start;
                start = raceTimeTotal / 2;
                step = start / 2;
                searchDown = false;
                edge = FindEdgeType(raceTimeTotal, currentRecord, start);
                minimumStepsize = 0;
                findLowerEdge = false;
            }
            if (edge.HasFlag(EdgeType.Upper))
            {
                upperEdge = start;
                break;
            }
            if (searchDown)
            {
                start -= step;
            }
            else
            {
                start += step;
            }

            var newDistance = CalculateDistance(start, raceTimeTotal);
            if (newDistance < currentRecord)
            {
                if (findLowerEdge)
                {
                    if (searchDown)
                    {
                        searchDown = false;
                    }
                    step = step / 2;
                    if (step < 1)
                    {
                        step = 1;
                        minimumStepsize++;
                    }
                }
                else
                {
                    if (!searchDown)
                    {
                        searchDown = true;
                    }
                    step = step / 2;
                    if (step < 1)
                    {
                        step = 1;
                        minimumStepsize++;
                    }
                }
            }
            else
            {
                if (findLowerEdge)
                {
                    if (!searchDown)
                    {
                        searchDown = true;
                    }
                    step = step / 2;
                    if (step < 1)
                    {
                        step = 1;
                        minimumStepsize++;
                    }
                }
                else
                {
                    if (searchDown)
                    {
                        searchDown = false;
                    }
                    step = step / 2;
                    if (step < 1)
                    {
                        step = 1;
                        minimumStepsize++;
                    }
                }
            }
        }
        return (upperEdge + 1) - (lowerEdge - 1) - 1;
    }

    private static EdgeType FindEdgeType(long raceTimeTotal, long currentRecord, long loadTime)
    {
        EdgeType edgeType = EdgeType.None;
        var distance = CalculateDistance(loadTime, raceTimeTotal);
        if (distance <= currentRecord) return EdgeType.None;

        var testTime = loadTime - 1;
        var testDistance = CalculateDistance(testTime, raceTimeTotal);
        if (testDistance <= currentRecord)
        {
            edgeType |= EdgeType.Lower;
        }
        testTime = loadTime + 1;
        testDistance = CalculateDistance(testTime, raceTimeTotal);
        if (testDistance <= currentRecord)
        {
            edgeType |= EdgeType.Upper;
        }
        return edgeType;
    }
}
