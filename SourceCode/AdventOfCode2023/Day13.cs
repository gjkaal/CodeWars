using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day13
{
    private static Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.";

    private const string example1a = @"..##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.";

    private const string example2 = @"#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#";

    private const string example2a = @"#...##..#
#...##..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#";

    private const string example3 = @"..##..##..#.#
..##..##..#.#
##...####...#
#.......#..##
.#..##.#.#.#.
#.##.......#.
..#.#...##...
#.#..#.##..#.
#.#..#.##..#.
..#.#...##...
#.##.......#.
.#..##.#.#.#.
#.......#..##
##...####..##
..##..##..#.#";

    [TestMethod]
    public void FindAllMirrorsTest()
    {
        var fi = new FileInfo("InputFiles/Day13test.txt");
        StdOut = (s) => { };
        var summarizeMirrors = 0;
        var mapCount = 0;
        foreach (string[] map in fi.ReadMapData())
        {
            mapCount++;
            var m = FindMirrors(map, -1, -1).ToArray();
            foreach (var mirror in m)
            {
                if (mirror.MirrorType == Mirrortype.None) continue;
                summarizeMirrors += mirror.Position * (mirror.MirrorType == Mirrortype.Horizontal ? 100 : 1);

                StdOut = System.Console.WriteLine;
                DrawMap($"Test map {mapCount}", map, mirror.Position, mirror.MirrorType, -1, -1);
                StdOut = (s) => { };
            }
        }
        Assert.AreEqual(505, summarizeMirrors);
    }

    [TestMethod]
    public void FindAllDirtyMirrorsTest()
    {
        var fi = new FileInfo("InputFiles/Day13test.txt");
        StdOut = (s) => { };
        var summarizeMirrors = 0;
        var mapCount = 0;
        foreach (string[] map in fi.ReadMapData())
        {
            mapCount++;
            var m = FindDirtyMirrors(map).ToArray();
            foreach (var mirror in m)
            {
                if (mirror.MirrorType == Mirrortype.None) continue;
                summarizeMirrors += mirror.Position * (mirror.MirrorType == Mirrortype.Horizontal ? 100 : 1);

                StdOut = System.Console.WriteLine;
                StdOut.Invoke("");
                StdOut.Invoke($"Total to line: {summarizeMirrors}");
                DrawMap($"Test map {mapCount}", map, mirror.Position, mirror.MirrorType, -1, -1);
                StdOut = (s) => { };
            }
        }
        Assert.AreEqual(1200, summarizeMirrors);
    }

    [TestMethod]
    public void FindAllMirrorsDay13()
    {
        var fi = new FileInfo("InputFiles/Day13.txt");
        StdOut = (s) => { };
        var summarizeMirrors = 0;
        var mapCount = 0;
        foreach (string[] map in fi.ReadMapData())
        {
            mapCount++;
            var m = FindMirrors(map, -1, -1).ToArray();
            foreach (var mirror in m)
            {
                if (mirror.MirrorType == Mirrortype.None) continue;
                summarizeMirrors += mirror.Position * (mirror.MirrorType == Mirrortype.Horizontal ? 100 : 1);

                StdOut = System.Console.WriteLine;
                StdOut.Invoke("");
                StdOut.Invoke($"Total to line: {summarizeMirrors}");
                DrawMap($"Day 13 map {mapCount}", map, mirror.Position, mirror.MirrorType, -1, -1);
                StdOut = (s) => { };
            }
        }

        Assert.AreEqual(29130, summarizeMirrors);
    }

    [TestMethod]
    public void FindAllDirtyMirrorsDay13()
    {
        var fi = new FileInfo("InputFiles/Day13.txt");
        var summarizeMirrors = 0;
        var mapCount = 0;
        StdOut = (s) => { };
        foreach (string[] map in fi.ReadMapData())
        {
            mapCount++;
            var m = FindDirtyMirrors(map).ToArray();
            if (m.Length > 1)
            {
                StdOut($"Found {m.Length} mirrors in map {mapCount}");
                Assert.Fail();
            }
            foreach (var mirror in m)
            {
                if (mirror.MirrorType == Mirrortype.None) continue;
                summarizeMirrors += mirror.Position * (mirror.MirrorType == Mirrortype.Horizontal ? 100 : 1);

                StdOut = System.Console.WriteLine;
                StdOut.Invoke("");
                StdOut.Invoke($"Total to line: {summarizeMirrors}");
                DrawMap($"Test map {mapCount}", map, mirror.Position, mirror.MirrorType, -1, -1);
                StdOut = (s) => { };
            }
        }

        // 34203 is too high
        Assert.IsTrue(summarizeMirrors > 22799);
        Assert.AreEqual(33438, summarizeMirrors);
    }

    [TestMethod]
    public void FindMirrorTest1()
    {
        Mirror[] mirrors = FindMirrors(example1.Split(Environment.NewLine), -1, -1).ToArray();
        foreach(var m in mirrors)
        {
            StdOut($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(1, mirrors.Length);

        var vertical = mirrors.Single(m => m.MirrorType == Mirrortype.Vertical);
        Assert.AreEqual(5, vertical.Position);

        var horizontal = mirrors.Any(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.IsFalse(horizontal);
    }

    [TestMethod]
    public void FindMirrorTest1a()
    {
        Mirror[] mirrors = FindMirrors(example1a.Split(Environment.NewLine), -1, -1).ToArray();
        foreach (var m in mirrors)
        {
            StdOut($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(2, mirrors.Length);

        var vertical = mirrors.Single(m => m.MirrorType == Mirrortype.Vertical);
        Assert.AreEqual(5, vertical.Position);

        var horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(3, horizontal.Position);
    }

    [TestMethod]
    public void FindMirrorTest2()
    {
        Mirror[] mirrors = FindMirrors(example2.Split(Environment.NewLine), -1, -1).ToArray();
        foreach (var m in mirrors)
        {
            StdOut($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(1, mirrors.Length);

        var horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(4, horizontal.Position);

        var vertical = mirrors.Any(m => m.MirrorType == Mirrortype.Vertical);
        Assert.IsFalse(vertical);
    }

    [TestMethod]
    public void FindMirrorTest2a()
    {
        Mirror[] mirrors = FindMirrors(example2a.Split(Environment.NewLine), -1, -1).ToArray();
        foreach (var m in mirrors)
        {
            StdOut($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(1, mirrors.Length);

        var vertical = mirrors.Any(m => m.MirrorType == Mirrortype.Vertical);
        Assert.IsFalse(vertical);

        var horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(1, horizontal.Position);
    }

    [TestMethod]
    public void FindMirrorAlternativesTest() {
        Mirror[] mirrors = FindMirrorAlternatives(example2.Split(Environment.NewLine)).ToArray();

        StdOut.Invoke("");
        StdOut.Invoke($"Found {mirrors.Length} solutions");

        foreach (var m in mirrors)
        {
            StdOut.Invoke($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(2, mirrors.Length);
    }

    [TestMethod]
    public void FindMirrorDifferenceTest()
    {
        Mirror[] mirrors = FindDirtyMirrors(example2.Split(Environment.NewLine)).ToArray();
        StdOut.Invoke("");
        StdOut.Invoke($"Found {mirrors.Length} solutions");

        foreach (var m in mirrors)
        {
            StdOut.Invoke($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(1, mirrors.Length);
        var horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(1, horizontal.Position);


        mirrors = FindDirtyMirrors(example1.Split(Environment.NewLine)).ToArray();
        StdOut.Invoke("");
        StdOut.Invoke($"Found {mirrors.Length} solutions");

        foreach (var m in mirrors)
        {
            StdOut.Invoke($"Mirror result at {m.Position} of type {m.MirrorType}");
        }
        Assert.AreEqual(1, mirrors.Length);
        horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(3, horizontal.Position);
    }

    private IEnumerable<Mirror> FindDirtyMirrors(string[] map) 
    {
        var mirrors = new List<string>();        
        var mirrorsOrg = FindMirrors(map, -1, -1).ToArray();

        foreach (var m in mirrorsOrg)
        {
            var mirrorKey = $"{m.MirrorType}_{m.Position}";
            mirrors.Add(mirrorKey);
        }

        var mirrorsAlt = FindMirrorAlternatives(map).ToArray();
        foreach (var m in mirrorsAlt)
        {
            var mirrorKey = $"{m.MirrorType}_{m.Position}";
            if (mirrors.Contains(mirrorKey)) continue;
            yield return m;
        }
    }

    private static IEnumerable<Mirror> FindMirrorAlternatives(string[] map) { 
        var xSize = map[0].Length;
        var mirrors = new List<string>();

        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < xSize; x++)
            {
                foreach(var m in FindMirrors(map, x, y))
                {
                    var mirrorKey = $"{m.MirrorType}_{m.Position}";
                    if (mirrors.Contains(mirrorKey)) continue;
                    mirrors.Add(mirrorKey);
                    yield return m;
                }
            }
        }
    }

    [TestMethod]
    public void FindMirrorTest3()
    {
        Mirror[] mirrors = FindDirtyMirrors(example3.Split(Environment.NewLine)).ToArray();

        foreach (var m in mirrors)
        {
            StdOut($"Mirror result at {m.Position} of type {m.MirrorType}");
        }        

        Assert.AreEqual(1, mirrors.Length);
        var horizontal = mirrors.Single(m => m.MirrorType == Mirrortype.Horizontal);
        Assert.AreEqual(8, horizontal.Position);

        var vertical = mirrors.Any(m => m.MirrorType == Mirrortype.Vertical);
        Assert.IsFalse(vertical);
    }

    private static IEnumerable<Mirror> FindMirrors(string[] map, int flipX, int flipY) {
        // find horizontal mirror
        foreach(var mirrorPosition in FindMirrorLines(map, flipX, flipY))
        {
            yield return new Mirror(mirrorPosition, Mirrortype.Horizontal);
        }

        // rotate map and find vertical mirror
        var mapWidth = map[0].Length;
        string[] rotatedMap = new string[mapWidth];
        for (int x = 0; x < mapWidth; x++)
        {
            var sb = new StringBuilder();
            for (int y = 0; y < map.Length; y++)
            {
                sb.Append(map[y][x]);
            }
            rotatedMap[x] = sb.ToString();
        }

        // find mirror in rotated map
        foreach (var mirrorPosition in FindMirrorLines(rotatedMap, flipY, flipX))
        if (mirrorPosition >= 0)
        {
            yield return new Mirror(mirrorPosition, Mirrortype.Vertical);
        }
    }

    private static void DrawMap(string mapLabel, string[] map, int mirrorPos, Mirrortype mirrortype, int flipX, int flipY)
    { 
        StdOut.Invoke($"{mapLabel}: ({map[0].Length} x {map.Length})");
        StdOut.Invoke($"Mirror: ({mirrortype} at {mirrorPos})");
        try
        {
            for (int y = 0; y < map.Length; y++)
            {
                string line = map[y];
                if (y == flipY)
                {
                    char c = (line[flipX] == '#') ? '.' : '#';
                    line = line.Remove(flipX, 1).Insert(flipX, c.ToString());
                }
                if (mirrortype == Mirrortype.Horizontal)
                {
                    if (y == mirrorPos)
                    {
                        StdOut.Invoke(new string('-', map[0].Length));
                    }
                }
                if (mirrortype == Mirrortype.Vertical)
                {
                    if (mirrorPos > 0 && mirrorPos < line.Length - 2)
                    {
                        line = line.Insert(mirrorPos, "|");
                    }
                }
                StdOut.Invoke(line);
            }
        }
        catch (Exception ex)
        {
            StdOut.Invoke($"Error: {ex.Message}");            
        }
    }

    private static IEnumerable<int> FindMirrorLines(string[] map, int flipX, int flipY) {
        for (int y = 0; y < map.Length-1; y++)
        {
            // found mirror?
            var found = true;
            for (var z = 0; z <= y; z++)
            {
                var p2 = y + 1;
                if (y - z < 0) break;
                if (p2 + z >= map.Length) break;
                if (CheckMirrorLine(map, y, z, p2, flipX, flipY))
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                yield return y+1;
            }
        }
    }

    private static bool CheckMirrorLine(string[] map, int y, int z, int p2, int flipX, int flipY)
    {
        var y1 = y - z;
        var y2 = p2 + z;

        var line1 = map[y1];
        if (y1 == flipY)
        {
            char c = (line1[flipX] == '#') ? '.' : '#';
            line1 = line1.Remove(flipX, 1).Insert(flipX, c.ToString());
        }

        var line2 = map[y2];
        if (y2 == flipY)
        {
            char c = (line2[flipX] == '#') ? '.' : '#';
            line2 = line2.Remove(flipX, 1).Insert(flipX, c.ToString());
        }

        return line1 != line2;
    }
}

public readonly struct Mirror
{
    public Mirror(int position, Mirrortype mirrorType)
    {
        Position = position;
        MirrorType = mirrorType;
    }
    public int Position { get; }
    public Mirrortype MirrorType { get;  }
}

public enum Mirrortype
{
    None,
    Vertical,
    Horizontal
}