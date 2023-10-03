using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

[TestClass]
public class Day16
{
    private static readonly string[] Input = File.ReadAllLines("InputFiles/Day16.txt");
    private static readonly Action<string> StdOut = System.Console.WriteLine;
    private const string SampleData = @".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....";

    [TestMethod]
    public void ReadMapTest()
    {
        var mirrorMap = new MirrorMap(SampleData.Split(Environment.NewLine));
        var mirror = mirrorMap.GetMirror(0, 0);
        Assert.AreEqual(MirrorType.None, mirror);
        mirror = mirrorMap.GetMirror(1, 0);
        Assert.AreEqual(MirrorType.VerticalSplit, mirror);
        mirror = mirrorMap.GetMirror(5, 0);
        Assert.AreEqual(MirrorType.MirrorLeftDown, mirror);
    }

    [TestMethod]
    public void Part1Sample()
    {
        var mirrorMap = new MirrorMap(SampleData.Split(Environment.NewLine));
        _ = EnergizeMap(mirrorMap, 0, 0, LightDirections.Left, 0);
        DrawMap(mirrorMap);
        var result = GetEnergizedTiles(mirrorMap);
        Assert.AreEqual(46, result);
    }

    [TestMethod]
    public void Part1()
    {
        var mirrorMap = new MirrorMap(Input);
        _ = EnergizeMap(mirrorMap, 0, 0, LightDirections.Left, 0);
        DrawMap(mirrorMap);
        var result = GetEnergizedTiles(mirrorMap);
        Assert.AreEqual(8323, result);
    }

    [TestMethod]
    public void FindMaximumPathTestSample()
    {
        var mirrorMap = new MirrorMap(SampleData.Split(Environment.NewLine));
        var (x, y, direction, energized) = FindMaximumPath(mirrorMap);
        Assert.AreEqual(3, x);
        Assert.AreEqual(0, y);
        Assert.AreEqual(LightDirections.Down, direction);
        Assert.AreEqual(51, energized);        
    }


    [TestMethod]
    public void FindMaximumPathTestSamples()
    {
        var mirrorMap = new MirrorMap(".. ./".Split(' '));
        var (_, _, _, energized) = FindMaximumPath(mirrorMap);
        Assert.AreEqual(3, energized);

        mirrorMap = new MirrorMap("/. \\.".Split(' '));
        (_, _, _, energized) = FindMaximumPath(mirrorMap);
        Assert.AreEqual(4, energized);

        mirrorMap = new MirrorMap("/. /.".Split(' '));
        (_, _, _, energized) = FindMaximumPath(mirrorMap);
        Assert.AreEqual(3, energized);
    }

    [TestMethod]
    public void FindMaximumPathTest()
    {
        var mirrorMap = new MirrorMap(Input);
        var (x, y, direction, energized) = FindMaximumPath(mirrorMap);
        Assert.AreEqual(6, x);
        Assert.AreEqual(110, y);
        Assert.AreEqual(LightDirections.Up, direction);

        mirrorMap.Reset();
        EnergizeMap(mirrorMap, x, y, direction, 0);
        DrawMap(mirrorMap);

        Assert.IsTrue(energized > 8484);
        Assert.AreEqual(8491, energized);
    }

    private (int X, int Y, LightDirections Direction, int Energized) FindMaximumPath(MirrorMap mirrorMap) { 
        var maxPath = (X: 0, Y: 0, Direction: LightDirections.None, Energized: 0);
        int energized;
        for(var i = 0; i < mirrorMap.Height; i++)
        {
            mirrorMap.Reset();
            EnergizeMap(mirrorMap, 0, i, LightDirections.Left, 0);
            energized = GetEnergizedTiles(mirrorMap);
            if (energized > maxPath.Energized)
            {
                maxPath = (X: 0, Y: i, Direction: LightDirections.Left, Energized: energized);
            }

            mirrorMap.Reset();
            EnergizeMap(mirrorMap, mirrorMap.Width-1, i, LightDirections.Right, 0);
            energized = GetEnergizedTiles(mirrorMap);
            if (energized > maxPath.Energized)
            {
                maxPath = (X: mirrorMap.Width, Y: i, Direction: LightDirections.Right, Energized: energized);
            }
        }
        for (var j = 0; j < mirrorMap.Width; j++)
        {
            mirrorMap.Reset();
            EnergizeMap(mirrorMap, j, 0, LightDirections.Down, 0);
            energized = GetEnergizedTiles(mirrorMap);
            if (energized > maxPath.Energized)
            {
                maxPath = (X: j, Y: 0, Direction: LightDirections.Down, Energized: energized);
            }

            mirrorMap.Reset();
            EnergizeMap(mirrorMap, j, mirrorMap.Height-1, LightDirections.Up, 0);
            energized = GetEnergizedTiles(mirrorMap);
            if (energized > maxPath.Energized)
            {
                maxPath = (X: j, Y: mirrorMap.Height, Direction: LightDirections.Up, Energized: energized);
            }
        }

        return maxPath;
    }

    private void DrawMap(MirrorMap mirrorMap)
    {
        var sb = new StringBuilder();
        for (var y = 0; y < mirrorMap.Width; y++)
        {
            sb.Clear();
            for (var x = 0; x < mirrorMap.Height; x++)
            {
                var mirror = mirrorMap.GetMirror(x, y);
                if (mirror != MirrorType.None)
                {
                    if (mirror == MirrorType.VerticalSplit)
                    {
                        sb.Append('|');
                    }
                    else if (mirror == MirrorType.HorizontalSplit)
                    {
                        sb.Append('-');
                    }
                    else if (mirror == MirrorType.MirrorLeftUp)
                    {
                        sb.Append('/');
                    }
                    else if (mirror == MirrorType.MirrorLeftDown)
                    {
                        sb.Append('\\');
                    }
                }
                else
                {

                    var light = mirrorMap.GetLight(x, y);
                    if (light == LightDirections.None)
                    {
                        sb.Append('.');
                    }
                    else if (light == LightDirections.Up)
                    {
                        sb.Append('^');
                    }
                    else if (light == LightDirections.Down)
                    {
                        sb.Append('v');
                    }
                    else if (light == LightDirections.Left)
                    {
                        sb.Append('>');
                    }
                    else if (light == LightDirections.Right)
                    {
                        sb.Append('<');
                    }
                    else
                    {
                        sb.Append('2');
                    }
                }               
            }
            StdOut.Invoke(sb.ToString());
            Assert.AreEqual(mirrorMap.Width, sb.Length);
        }
    }

    private int GetEnergizedTiles(MirrorMap mirrorMap) { 
        var energized = 0;
        for(var i = 0; i < mirrorMap.Height; i++)
        {
            for (var j = 0; j < mirrorMap.Width; j++)
            {
                energized += mirrorMap.GetEnergized(i, j);
            }
        }

        StdOut.Invoke($"Energized: {energized}");
        return energized;
    }

    private static int EnergizeMap(MirrorMap map, int xPos, int yPos, LightDirections light, int pathLength) 
    { 
        if (pathLength > 10000)
        {
            throw new Exception("Path too long");
        }
        if (xPos < 0 || xPos >= map.Width || yPos < 0 || yPos >= map.Height)
        {
            return pathLength;
        }

        var current = map.GetLight(xPos, yPos);
        if ((current & light) == light)
        {
            return pathLength;
        }

        map.SetLight(xPos, yPos, light);
        var mapItem = map.GetMirror(xPos, yPos);

        if (mapItem == MirrorType.None)
        {
            if (light == LightDirections.Up)
            {
                return EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength+1);
            }
            if (light == LightDirections.Down)
            {
                return EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
            }
            if (light == LightDirections.Left)
            {
                return EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
            }
            if (light == LightDirections.Right)
            {
                return EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
            }
        }
        else if (mapItem == MirrorType.MirrorLeftDown)
        {
            if (light == LightDirections.Up)
            {
                return EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
            }
            if (light == LightDirections.Down)
            {
                return EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
            }
            if (light == LightDirections.Left)
            {
                return EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
            }
            if (light == LightDirections.Right)
            {
                return EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength + 1);
            }
        }
        else if (mapItem == MirrorType.MirrorLeftUp)
        {
            if (light == LightDirections.Up)
            {
                return EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
            }
            if (light == LightDirections.Down)
            {
                return EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
            }
            if (light == LightDirections.Left)
            {
                return EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength + 1);
            }
            if (light == LightDirections.Right)
            {
                return EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
            }
        }
        else if (mapItem == MirrorType.HorizontalSplit)
        {
            if (light == LightDirections.Up)
            {
                var p1 = EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
                var p2 = EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
                return Math.Max(p1, p2);
            }
            if (light == LightDirections.Down)
            {
                var p1 = EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
                var p2 = EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
                return Math.Max(p1, p2);
            }
            if (light == LightDirections.Left)
            {
                return EnergizeMap(map, xPos + 1, yPos, LightDirections.Left, pathLength + 1);
            }
            if (light == LightDirections.Right)
            {
                return EnergizeMap(map, xPos - 1, yPos, LightDirections.Right, pathLength + 1);
            }
        }
        else if (mapItem == MirrorType.VerticalSplit)
        {
            if (light == LightDirections.Up)
            {
                return EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength + 1);
            }
            if (light == LightDirections.Down)
            {
                return EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
            }
            if (light == LightDirections.Left)
            {
                var p1 = EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength + 1);
                var p2 = EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
                return Math.Max(p1, p2);
            }
            if (light == LightDirections.Right)
            {
                var p1 = EnergizeMap(map, xPos, yPos - 1, LightDirections.Up, pathLength + 1);
                var p2 = EnergizeMap(map, xPos, yPos + 1, LightDirections.Down, pathLength + 1);
                return Math.Max(p1, p2);
            }
        }
        throw new Exception("Invalid mirror type");
    }
}

[Flags]
public enum LightDirections
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}

public enum MirrorType
{
    None = 0,
    VerticalSplit = 1,
    HorizontalSplit = 2,
    MirrorLeftUp = 4,
    MirrorLeftDown = 8
}

public class MirrorMap
{
    public int Height => _light.Length;
    public int Width => _light[0].Length;

    private readonly LightDirections[][] _light;
    private readonly MirrorType[][] _map;
    public MirrorMap(string[] mapData)
    {
        _light = new LightDirections[mapData.Length][];
        _map = new MirrorType[mapData.Length][];
        var width = mapData[0].Length;
        for (var i = 0; i < mapData.Length; i++)
        {
            var lightRow = new LightDirections[width];
            var mapRow = new MirrorType[width];
            var item = mapData[i].ToCharArray();
            for (var x = 0; x < item.Length; x++)
            {
                mapRow[x] = item[x] switch
                {
                    '.' => MirrorType.None,
                    '|' => MirrorType.VerticalSplit,
                    '-' => MirrorType.HorizontalSplit,
                    '/' => MirrorType.MirrorLeftUp,
                    '\\' => MirrorType.MirrorLeftDown,
                    _ => throw new Exception("Invalid character")
                };
            }
            _light[i] = lightRow;
            _map[i] = mapRow;
        }
    }

    internal int GetEnergized(int xPos, int yPos) { 
        if (xPos < 0 || yPos >= Height || yPos < 0 || xPos >= Width)
        {
            return 0;
        }
        return _light[yPos][xPos] == LightDirections.None ? 0 : 1;
    }

    internal LightDirections GetLight(int xPos, int yPos) {
        if (xPos < 0 || yPos >= Height || yPos < 0 || xPos >= Width)
        {
            return 0;
        }
        return _light[yPos][xPos];
    }

    internal void SetLight(int xPos, int yPos, LightDirections lightDirection)
    {
        if (xPos < 0 || yPos >= Height || yPos < 0 || xPos >= Width)
        {
            return;
        }
        var current = _light[yPos][xPos];
        _light[yPos][xPos] = (current | lightDirection);
    }

    internal MirrorType GetMirror(int xPos, int yPos)
    {
        if (xPos < 0 || yPos >= Height || yPos < 0 || xPos >= Width)
        {
            return 0;
        }
        return _map[yPos][xPos];
    }

    internal void Reset() {
        for (int y = 0; y < Height; y++)
        {
            for(int x = 0; x<Width; x++)
            {
                _light[y][x] = LightDirections.None;
            }
        }
    }
}
