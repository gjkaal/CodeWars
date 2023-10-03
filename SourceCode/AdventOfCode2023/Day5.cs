using AdventOfCode2023.Console;
using CoreElements;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day5
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;
    private readonly Day5Calculator calculator = new Day5Calculator(StdOut);

    [TestMethod]
    public void TestParseLine()
    {
        var map = "50 98 2".ParseDestinationMap();
        Assert.AreEqual(98, map.SourceStart);
        Assert.AreEqual(100, map.SourceEnd);
        Assert.AreEqual(50, map.DestinationStart);
        Assert.AreEqual(52, map.DestinationEnd);
    }

    [DataTestMethod]
    [DataRow(0,0)]
    [DataRow(1,1)]
    [DataRow(49,49)]
    [DataRow(50,52)]
    [DataRow(51,53)]
    [DataRow(52,54)]
    [DataRow(96,98)]
    [DataRow(97,99)]
    [DataRow(98,50)]
    [DataRow(99,51)]
    public void TestMap(int source, int destination)
    {
        var mapping = new Mapping
        {            
        };
        mapping.Maps.Add("50 98 2".ParseDestinationMap());
        mapping.Maps.Add("52 50 48".ParseDestinationMap());
        var result = mapping.MapValue(source);
        Assert.AreEqual(destination, result);
    }

    [TestMethod]
    public void ReadAlmanakFileTest()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example));
        calculator.ReadAlmanakFile(stream);
        Assert.AreEqual(4, calculator.Seeds.Count);
        Assert.AreEqual(2, calculator.SeedRanges.Count);
        var (rangeStart, rangeEnd) = calculator.SeedRanges[0];
        Assert.AreEqual(79, rangeStart);
        Assert.AreEqual(93, rangeEnd);
        Assert.AreEqual(7, calculator.Mappings.Count);
        Assert.AreEqual(79, calculator.Seeds[0]);
        Assert.AreEqual("seed-to-soil map", calculator.Mappings[0].Name);
    }

    [TestMethod]
    public void TestInterlocked()
    {
        var value = new InterlockedValue<int>(10);
        Assert.IsTrue(value.UpdateValue(10, 1));
        Assert.IsTrue(value.UpdateValue(1, 5));
        Assert.IsFalse(value.UpdateValue(4, 7));
        Assert.AreEqual(5, value.Value);
    }

    [TestMethod]
    public void FindLowestLocationExample()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example));
        calculator.ReadAlmanakFile(stream);
        Assert.AreEqual(35L, calculator.FindLowestSeedLocation());
    }

    [TestMethod]
    public void FindLowestLocationRangeExample()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example));
        calculator.ReadAlmanakFile(stream);
        Assert.AreEqual(46L, calculator.FindLowestSeedRangeLocation(10));
    }

    [TestMethod]
    public void FindLowestLocationRange()
    {
        using var stream = new FileStream("InputFiles/Day5.txt", FileMode.Open);
        calculator.ReadAlmanakFile(stream);
        // should be within 10 minutes => 600 seconds
        Assert.AreEqual(63179500, calculator.FindLowestSeedRangeLocation(600));
    }

    [TestMethod()]
    public void FindLowestLocation()
    {
        using var stream = new FileStream("InputFiles/Day5.txt", FileMode.Open);
        calculator.ReadAlmanakFile(stream);
        Assert.AreEqual(265018614L, calculator.FindLowestSeedLocation());
    }

    

    private const string example = @"
seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";
}
