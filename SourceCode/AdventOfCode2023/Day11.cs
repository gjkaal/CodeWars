using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

[TestClass]
public class Day11
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....";

    [TestMethod]
    public void ExpandGalaxiesTest()
    {
        var map = example1.Split(Environment.NewLine);
        Assert.AreEqual(10, map.GetLength(0));
        char[][] result = ExpandGalaxies(map);
        DrawMap(result);
        Assert.AreEqual(12, result.Length);
        Assert.AreEqual(13, result[0].Length);
    }

    [TestMethod]
    public void GetGalaxiesTest()
    {
        var map = example1.Split(Environment.NewLine);
        char[][] result = ExpandGalaxies(map);
        var galaxies = GetGalaxies(result);
        Assert.AreEqual(9, galaxies.Length);
        Assert.AreEqual(4, galaxies[0].X);
        Assert.AreEqual(0, galaxies[0].Y);
        Assert.AreEqual(5, galaxies[8].X);
        Assert.AreEqual(11, galaxies[8].Y);
    }

    [DataTestMethod]
    [DataRow(0, 6, 15)]
    [DataRow(2, 5, 17)]
    [DataRow(3, 6, 6)]
    [DataRow(7, 8, 5)]
    [DataRow(4, 8, 9)]
    public void GetGalaxiesDistanceTest(int n1, int n2, int expectDistance)
    {
        var map = example1.Split(Environment.NewLine);
        char[][] result = ExpandGalaxies(map);
        var galaxies = GetGalaxies(result);

        var g1 = galaxies[n1];
        var g2 = galaxies[n2];
        Assert.AreEqual(expectDistance, g1.Distance(g2));
    }

    [TestMethod]
    public void TestGetExpandingClumns()
    {
        var map = example1.Split(Environment.NewLine);
        var (cols, rows) = GetExpandingColumns(map);
        Assert.AreEqual(10, cols.Length);
        Assert.AreEqual(10, rows.Length);

        // 2, 5, 8
        Assert.AreEqual(false, cols[0]);
        Assert.AreEqual(false, cols[1]);
        Assert.AreEqual(true, cols[2]);
        Assert.AreEqual(false, cols[3]);
        Assert.AreEqual(false, cols[4]);
        Assert.AreEqual(true, cols[5]);
        Assert.AreEqual(false, cols[6]);
        Assert.AreEqual(false, cols[7]);
        Assert.AreEqual(true, cols[8]);
        Assert.AreEqual(false, cols[9]);

        Assert.AreEqual(false, rows[0]);
        Assert.AreEqual(false, rows[1]);
        Assert.AreEqual(false, rows[2]);
        Assert.AreEqual(true, rows[3]);
        Assert.AreEqual(false, rows[4]);
        Assert.AreEqual(false, rows[5]);
        Assert.AreEqual(false, rows[6]);
        Assert.AreEqual(true, rows[7]);
        Assert.AreEqual(false, rows[8]);
        Assert.AreEqual(false, rows[9]);
    }

    [TestMethod]
    public void TestGetPairs() {
        var map = example1.Split(Environment.NewLine);
        char[][] result = ExpandGalaxies(map);
        var galaxies = GetGalaxies(result);
        var pairs = GetPairs(galaxies).ToArray();
        Assert.AreEqual(36, pairs.Length);
    }

    [TestMethod]
    public void CountSumOfDistanceTest()
    {
        var map = example1.Split(Environment.NewLine);
        char[][] result = ExpandGalaxies(map);
        var galaxies = GetGalaxies(result);
        var totalDistance = 0L;
        foreach (var (g1, g2) in GetPairs(galaxies))
        {
            totalDistance += g1.Distance(g2);
        }
        Assert.AreEqual(374, totalDistance);
    }

    [DataTestMethod]
    [DataRow(2, 374)]
    [DataRow(10, 1030)]
    [DataRow(100, 8410)]
    public void CountSumOfDistanceTestExpanding(int factor, long expected)
    {
        var map = example1.Split(Environment.NewLine);
        var (cols, rows) = GetExpandingColumns(map);
        char[][] result = GalaxyMap(map);
        var galaxies = GetGalaxies(result, cols, rows, factor);
        var totalDistance = 0L;
        foreach (var (g1, g2) in GetPairs(galaxies))
        {
            totalDistance += g1.Distance(g2);
        }
        Assert.AreEqual(expected, totalDistance);
    }

    [TestMethod]
    public void CountSumOfDistances() {
        var fi = new FileInfo("InputFiles/Day11.txt");
        var map = fi.ReadMapData().First();
        char[][] result = ExpandGalaxies(map);
        var galaxies = GetGalaxies(result);
        var totalDistance = 0L;
        foreach (var (g1, g2) in GetPairs(galaxies))
        {
            totalDistance += g1.Distance(g2);
        }
        Assert.AreEqual(9556712, totalDistance);
    }

    [TestMethod]
    public void CountRealSumOfDistances()
    {
        var fi = new FileInfo("InputFiles/Day11.txt");
        var map = fi.ReadMapData().First();
        var (cols, rows) = GetExpandingColumns(map);
        char[][] result = GalaxyMap(map);
        var galaxies = GetGalaxies(result, cols, rows, 1000000);
        var totalDistance = 0L;
        foreach (var (g1, g2) in GetPairs(galaxies))
        {
            totalDistance += g1.Distance(g2);
        }
        // 678626878094 is too high
        // 678626432023 is too high
        // 678626199476
        Assert.AreEqual(678626199476, totalDistance);
    }

    private IEnumerable<(Position, Position)> GetPairs(Position[] galaxies) { 
        for(int i = 0; i < galaxies.Length; i++)
        {
            for(int j = i + 1; j < galaxies.Length; j++)
            {
                yield return (galaxies[i], galaxies[j]);
            }
        }
    }

    private Position[] GetGalaxies(char[][] result) 
    { 
        var galaxies = new List<Position>();
        for (int y = 0; y < result.GetLength(0); y++)
        {
            for(int x = 0; x < result[y].Length; x++)
            {
                if (result[y][x] == '#')
                {
                    galaxies.Add( new Position(x, y));
                }
            }
        }
        return galaxies.ToArray();
    }

    private Position[] GetGalaxies(char[][] result, bool[] cols, bool[] rows, int expand)
    {
        var galaxies = new List<Position>();
        var ySize = result.Length;
        var xSize = result[0].Length;
        if (cols.Length != xSize) throw new ArgumentException("Invalid cols size");
        if (rows.Length != ySize) throw new ArgumentException("Invalid rows size");

        int yPos = 0;
        for (int y = 0; y < ySize; y++)
        {
            int xPos = 0;
            for (int x = 0; x < xSize; x++)
            {
                if (result[y][x] == '#')
                {
                    galaxies.Add(new Position(xPos, yPos));
                }
                if (cols[x])
                {
                    xPos += expand;
                }
                else
                {
                    xPos += 1;
                }
            }
            if (rows[y])
            {
                yPos += expand;
            }
            else
            {
                yPos += 1;
            }            
        }
        return galaxies.ToArray();
    }

    private void DrawMap(char[][] result) { 
        for (int i = 0; i < result.GetLength(0); i++) { 
            StdOut.Invoke(new string(result[i]));
        }
    }

    private (bool[] cols, bool[] rows) GetExpandingColumns(string[] map) 
    {
        bool[] mapRowExpansion = new bool[map[0].Length];
        bool[] mapColumnExpansion = new bool[map.Length];
        for (var i = 0; i < mapRowExpansion.Length; i++)
        {
            mapRowExpansion[i] = true;
        }
        for (var i = 0; i < mapColumnExpansion.Length; i++)
        {
            mapColumnExpansion[i] = true;
        }
        for (var i1 = 0; i1 < map.Length; i1++)
        {
            var line = map[i1];
            if (line.Contains('#')) {
                mapRowExpansion[i1] = false;
            }
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    mapColumnExpansion[i] = false;
                }
            }
        }
        return (mapColumnExpansion, mapRowExpansion);
    }

    private char[][] GalaxyMap(string[] map)
    {
        return map.Select(s => s.ToCharArray()).ToArray();
    }

    private char[][] ExpandGalaxies(string[] map) {
        var result = new List<string>();
        bool[] mapExpansion = new bool[map[0].Length];
        for(var i = 0; i < mapExpansion.Length; i++) 
        {
            mapExpansion[i] = true;
        }
        foreach (var line in map)
        {
            for(int i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    mapExpansion[i] = false;
                }
            }
        }

        var newWidth = mapExpansion.Length + mapExpansion.Count(b => b);
        foreach(var line in map)
        {
            if (!line.Contains('#'))
            {
                result.Add(new string('.', newWidth));
                result.Add(new string('.', newWidth));
            }
            else 
            {
                var xpos = 0;
                char[] nextLine = new char[newWidth];
                for(int i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    if (mapExpansion[i])
                    {
                        nextLine[xpos] = '.';
                        xpos++;
                    }
                    nextLine[xpos] = c;
                    xpos++;
                }
                var newRow = new string(nextLine);
                result.Add(newRow);
                xpos++;
            }
        }
        return result.Select(s => s.ToCharArray()).ToArray();
    }
}

public static class PositionExtensions
{
    public static int Distance(this Position p1, Position p2)
    {
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }
}

public static class MapExtensions {

    public static IEnumerable<string[]> ReadMapData(this FileInfo fileInfo)
    {
        var map = new List<string>();
        using var stream = fileInfo.OpenRead();
        using var reader = new StreamReader(stream, leaveOpen: true);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) { 
                yield return map.ToArray();
                map.Clear();
                continue;
            }
            map.Add(line);
        }
        yield return map.ToArray();
    }
}