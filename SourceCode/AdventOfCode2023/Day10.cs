
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Text;
using static AdventOfCode2023.Day14;

namespace AdventOfCode2023;

[TestClass]
public class Day10
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @".....
-L|F7
7S-7|
L|7||
-L-J|
L|-JF";

    private const string example2 = @"7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ";

    private const string example3 = @"...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........";

    private const string example4 = @".F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...";

    private const string example5 = @"FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L";

    [DataTestMethod]
    [DataRow(1, 2, false)]
    [DataRow(1, 3, true)]
    [DataRow(2, 3, false)]
    [DataRow(3, 2, false)]
    public void TestEquality(int x, int y, bool expected)
    {
        var pos1 = new Position ( x, y);
        var pos2 = new Position ( 1, 3);
        var result = pos1.Equals(pos2);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(-1, 0, '.')]
    [DataRow(1, 10, '.')]
    [DataRow(0, 1, '-')]
    [DataRow(0, 2, '7')]
    [DataRow(1, 1, 'L')]
    [DataRow(1, 2, 'S')]
    public void TestGetMapElement(int x, int y, char expected)
    {
        var map = example1.Split(Environment.NewLine) ?? Array.Empty<string>();
        var pos = new Position(x, y);
        var result = GetMapElement(map, pos);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestGetFlags()
    {
        var directions = Directions.Up;
        var result = directions.GetFlags().ToArray();
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(Directions.Up, result[0]);

        directions = Directions.Up | Directions.Left | Directions.Down;
        result = directions.GetFlags().ToArray();
        Assert.AreEqual(3, result.Count());
        Assert.AreEqual(Directions.Up, result[0]);
        Assert.AreEqual(Directions.Down, result[1]);
        Assert.AreEqual(Directions.Left, result[2]);

    }

    [TestMethod]
    public void TestFindRoute1()
    {
        var map = example1.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = FindStartPosition(map);
        Assert.IsNotNull(starpos);
        Assert.AreEqual(1, starpos.X);
        Assert.AreEqual(2, starpos.Y);

        var result = FindRoute(map, starpos);
        Assert.IsNotNull(result);
        Assert.AreEqual(true, result.Item1);
        Assert.AreEqual(4 * 2 + 1, result.Item2.Length);
    }

    [TestMethod]
    public void TestFindRoute2()
    {
        var map = example2.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = FindStartPosition(map);
        var result = FindRoute(map, starpos);
        Assert.IsNotNull(result);
        Assert.AreEqual(true, result.Item1);
        Assert.AreEqual(8 * 2 + 1, result.Item2.Length);
    }

    [TestMethod]
    public void TestFindRoute()
    {
        string[] mapData = ReadMap();
        var starpos = FindStartPosition(mapData);

        StdOut.Invoke($"Start position: {starpos.X},{starpos.Y}");

        var result = FindRoute(mapData, starpos);
        Assert.IsNotNull(result);
        Assert.AreEqual(true, result.Item1);
        Assert.AreEqual(6649 * 2 + 1, result.Item2.Length);
    }

    private static string[] ReadMap()
    {
        var map = new List<string>();
        using var stream = new FileStream("InputFiles/Day10.txt", FileMode.Open);
        using var reader = new StreamReader(stream, leaveOpen: true);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            map.Add(line);
        }
        return map.ToArray();
    }

    [TestMethod]
    public void TestEnclosedSample3()
    {
        var map = example3.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = FindStartPosition(map);
        var enclosedSpace = FindEnclosed(map, starpos);
        Assert.AreEqual(4, enclosedSpace);
    }

    [TestMethod]
    public void TestEnclosedSample4()
    {
        var map = example4.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = FindStartPosition(map);
        var enclosedSpace = FindEnclosed(map, starpos);
        Assert.AreEqual(8, enclosedSpace);
    }

    [TestMethod]
    public void TestEnclosedSample5()
    {
        var map = example5.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = FindStartPosition(map);
        var enclosedSpace = FindEnclosed(map, starpos);
        Assert.AreEqual(10, enclosedSpace);
    }

    [TestMethod]
    public void TestFindInvalidRoute()
    {
        var map = example5.Split(Environment.NewLine) ?? Array.Empty<string>();
        var starpos = new Position(0, 1);
        var result = FindRoute(map, starpos);
        Assert.IsNotNull(result);
        Assert.AreEqual(false, result.Item1);
        Assert.AreEqual(2, result.Item2.Length);
    }

    [TestMethod]
    public void TestEnclosedInMap()
    {
        string[] mapData = ReadMap();
        var starpos = FindStartPosition(mapData);
        var enclosedSpace = FindEnclosed(mapData, starpos);

        // 78 is not the correct answer
        // 20 is not correct either
        // 90 is not correct
        // 22 is not correct
        // 569 is not correct
        Assert.AreEqual(601, enclosedSpace);
    }

    [TestMethod]
    public void TestMarkDeadEndsExample()
    {
        var sourceMap = example5.Split(Environment.NewLine) ?? Array.Empty<string>();
        var findMap = MarkDeadEndsInMap(sourceMap).Item1;
        DrawMap(sourceMap, 0, new Position(-1, -1), new Position(-1, -1));
        DrawMap(findMap, 0, new Position(-1, -1), new Position(-1, -1));
        Assert.IsNotNull(findMap);
        Assert.AreEqual(sourceMap.Length, findMap.Length);
    }

    [TestMethod]
    public void TestMarkDeadEnds()
    {
        string[] sourceMap = ReadMap();
        var findMap = MarkDeadEndsInMap(sourceMap).Item1;
        DrawMap(sourceMap, 0, new Position(-1, -1), new Position(-1, -1));
        DrawMap(findMap, 0, new Position(-1, -1), new Position(-1, -1));
        Assert.IsNotNull(findMap);
        Assert.AreEqual(sourceMap.Length, findMap.Length);
    }


    private int FindEnclosed(string[] map, Position startpos)
    {
        var currentpos = startpos;
        var startDirections = GetDirections(map, currentpos);
        var directions = startDirections.GetFlags().ToArray();

        var enclosed1 = FindEnclosedPositions(map, currentpos);
        StdOut.Invoke($"Enclosed : {enclosed1}");

        return enclosed1;
    }

    private int FindEnclosedPositions(string[] sourceMap, Position currentpos)
    {
        var findMap = MarkDeadEndsInMap(sourceMap);
        DrawMap(findMap.Item1, 0, currentpos, currentpos);
        var modifiedMap = new string[findMap.Item1.Length];
        Array.Copy(findMap.Item1, modifiedMap, findMap.Item1.Length);

        var dir = GetDirections(modifiedMap, currentpos);
        var dirChar = GetDirectionsCharacter(dir);
        FillMapElement(modifiedMap, currentpos, dirChar);


        foreach (var track in findMap.Item2)
        {
            if (track.Any(m => m.Equals(currentpos))) continue;
            foreach(var pos in track)
            {
                FillMapElement(modifiedMap, pos, '.');
            }
        }
        
        var ySize = modifiedMap.Length-1;
        var xSize = modifiedMap[0].Length-1;
        for(int i = 0; i <= ySize; i++)
        {
            FloodFill(modifiedMap, new Position(0, i), '.', 'O');
            FloodFill(modifiedMap, new Position(xSize, i), '.', 'O');
        }
        for (int i = 0; i <= xSize; i++)
        {
            FloodFill(modifiedMap, new Position(i, 0), '.', 'O');
            FloodFill(modifiedMap, new Position(i, ySize), '.', 'O');
        }

        int externalCount = 0;
        for (int i = 0; i < modifiedMap.Length; i++)
        {
            var line = modifiedMap[i];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == 'O')
                {
                    externalCount++;
                }
            }
        }

        // remove loops and bends and replace with edge
        try
        {
            for (int i = 0; i < modifiedMap.Length; i++)
            {
                var line = modifiedMap[i];
                var mline = new char[line.Length];
                var c = 'O';
                var edge = -1;
                for (var x = 0; x < line.Length; x++)
                {
                    var nc = line[x];
                    mline[x] = nc;
                    if (nc == 'F')
                    {
                        if (edge != -1)
                        {
                            throw new Exception($"Invalid map at {x}, {i}");
                        }
                        else
                        {
                            edge = x;
                            c = nc;
                        }
                    }
                    if (nc == 'L')
                    {
                        if (edge != -1)
                        {
                            throw new Exception($"Invalid map at {x}, {i}");
                        }
                        else
                        {
                            edge = x;
                            c = nc;
                        }
                    }
                    if (nc == 'J')
                    {
                        if (edge == -1)
                        {
                            throw new Exception($"Invalid map at {x}, {i}");
                        }
                        else
                        {
                            mline[edge] = c == 'F' ? '|' : '-';
                            mline[x] = '-';
                            edge = -1;
                            c = 'O';
                        }
                    }
                    if (nc == '7')
                    {
                        if (edge == -1)
                        {
                            throw new Exception($"Invalid map at {x}, {i}");
                        }
                        else
                        {
                            mline[edge] = c == 'L' ? '|' : '-';
                            mline[x] = '-';
                            edge = -1;
                            c = 'O';
                        }
                    }
                }
                modifiedMap[i] = new string(mline);
            }
        }
        catch
        {
            DrawMap(modifiedMap, externalCount, new Position(-1, -1), new Position(-1, -1));
            throw;
        }

        DrawMap(modifiedMap, externalCount, currentpos, currentpos);

        int internalCount = 0;
        for (int i = 0; i < modifiedMap.Length; i++)
        {
            var line = modifiedMap[i];
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '.')
                {
                    var countElements = 0;
                    for(xSize= j; xSize < line.Length; xSize++)
                    {
                        if ("|".Contains(line[xSize])) countElements++;
                    }
                    if (countElements%2 == 1)
                    {
                        internalCount++;
                    }
                }
            }
        }

        StdOut.Invoke($"Enclosed count: {internalCount}");
        DrawMap(modifiedMap, internalCount, Position.Minus1(), Position.Minus1());
        return internalCount;
    }

    private static void FloodFill(string[] map, Position viewPos, char oldChar, char newChar)
    {
        if (viewPos.Y < 0 || viewPos.Y >= map.Length) return;
        if (viewPos.X < 0 || viewPos.X >= map[viewPos.Y].Length) return;
        if (map[viewPos.Y][viewPos.X] != oldChar) return;
        map[viewPos.Y] = string.Concat(map[viewPos.Y][..viewPos.X], newChar, map[viewPos.Y][(viewPos.X + 1)..]);
        FloodFill(map, new Position(viewPos.X, viewPos.Y - 1), oldChar, newChar);
        FloodFill(map, new Position(viewPos.X, viewPos.Y + 1), oldChar, newChar);            
        FloodFill(map, new Position(viewPos.X - 1, viewPos.Y), oldChar, newChar);
        FloodFill(map, new Position(viewPos.X + 1, viewPos.Y), oldChar, newChar);
    }

    private static void FillMapElement(string[] map, Position viewPos, char character)
    {
        if (viewPos.Y < 0 || viewPos.Y >= map.Length) return;
        var line = map[viewPos.Y];
        if (viewPos.X < 0 || viewPos.X >= line.Length) return;
        line = string.Concat(line[..viewPos.X], character, line[(viewPos.X + 1)..]);
        map[viewPos.Y] = line;
    }

    private static (string[], List<Position[]>) MarkDeadEndsInMap(string[] map)
    {
        var modifiedMap = new string[map.Length];
        Array.Copy(map, modifiedMap, map.Length);
        var validTracks = new List<Position[]>();

        for (int i = 0; i < modifiedMap.Length; i++)
        {
            var line = modifiedMap[i];
            for (int j = 0; j < line.Length; j++)
            {
                var pos = new Position(j, i);
                var alreadyChecked = false;
                foreach (var track in validTracks)
                {
                    if (track.Any(m => m.Equals(pos)))
                    {
                        alreadyChecked = true;
                        continue;
                    }
                }
                if (alreadyChecked) continue;

                var mapElement = GetMapElement(modifiedMap, pos);
                if (mapElement == 'S') continue;
                if (mapElement == '.') continue;
                var findTrack = FindRoute(modifiedMap, pos);
                if (findTrack.Item1)
                {
                    validTracks.Add(findTrack.Item2);
                }
                else
                {
                    foreach (var track in findTrack.Item2)
                    {
                        FillMapElement(modifiedMap, track, '.');
                    }
                }
            }
        }
        return (modifiedMap.ToArray(), validTracks);
    }

    private static (bool, Position[]) FindRoute(string[] map, Position startpos)
    {
        var result = 1;
        var track = new List<Position> {
            startpos
        };

        var currentpos = startpos;        
        var startDirections = GetDirections(map, currentpos);
        var directions = startDirections.GetFlags().ToArray();

        var (valid1, current1) = GetNextPosition(map, new Position(-1,-1), currentpos, directions[0]);
        var (valid2, current2) = GetNextPosition(map, new Position(-1,-1), currentpos, directions[1]);
        if (valid1)
        {
            track.Add(current1);
        }
        if (valid2)
        {
            track.Add(current2);
        }

        var prevPos1 = currentpos;
        var prevPos2 = currentpos;
        while (!current1.Equals(current2) && (valid1 || valid2))
        {
            Position newpos1 = new Position(-1, -1);
            Position newpos2 = new Position(-1, -1);
            result++;
            if (valid1)
            {
                var directions1 = GetDirections(map, current1);
                (valid1, newpos1) = GetNextPosition(map, prevPos1, current1, directions1);
            }
            if (valid2) {
                var directions2 = GetDirections(map, current2);
                (valid2, newpos2) = GetNextPosition(map, prevPos2, current2, directions2);
            }
            if (valid1)
            {
                prevPos1 = current1;
                current1 = newpos1;
                track.Add(current1);
            }

            if (current1.Equals(current2)) break;

            if (valid2)
            {
                prevPos2 = current2;
                current2 = newpos2;
                track.Add(current2);
            }
        }
        return (valid1 && valid2 || current1.Equals(current2), track.ToArray());
    }

    private static char GetMapElement(string[] map, Position pos) { 
        if (pos.Y < 0 || pos.Y >= map.Length) return '.';
        var line = map[pos.Y];
        if (pos.X < 0 || pos.X >= line.Length) return '.';
        return line[pos.X];
    }

    private void DrawDirections(string label, Directions directions) { 
        var sb = new StringBuilder();
        sb.Append(label);
        sb.Append(": ");

        foreach (var direction in directions.GetFlags())
        {
            sb.Append(direction.ToString());
            sb.Append(' ');
        }
        StdOut.Invoke(sb.ToString());
    }

    private static void DrawMap(string[] map, int result, Position current1, Position current2)
    {
        StdOut.Invoke($"Result {result}: {current1.X},{current1.Y} {current2.X},{current2.Y}");
        for (var i = 0; i < map.Length; i++)
        {
            var line = map[i];
            if (i == current1.Y)
            {
                line = string.Concat(line[..current1.X], 'x', line[(current1.X + 1)..]);
            }
            if (i == current2.Y)
            {
                line = string.Concat(line[..current2.X], 'X', line[(current2.X + 1)..]);
            }
            StdOut.Invoke(line);
        }
        StdOut.Invoke("");
    }

    private static readonly Directions[] directionsMap = new[] { Directions.Up, Directions.Down, Directions.Left, Directions.Right };

    private static (bool, Position) GetNextPosition(string[] map, Position previous, Position current, Directions directions) 
    {
        Directions verifyConnection;
        if (directions == Directions.None) return (false,current);

        foreach (var direction in directionsMap)
        {
            if ((directions & direction) == direction)
            {
                Position nextPosition;
                switch (direction)
                {
                    case Directions.Up:
                        nextPosition = new Position(current.X, current.Y - 1);
                        if (nextPosition.Equals(previous)) continue;
                        verifyConnection = GetDirections(map, nextPosition);
                        if ((verifyConnection & Directions.Down) == Directions.Down)
                        {
                            return (true, nextPosition);
                        }
                        break;
                    case Directions.Down:
                        nextPosition = new Position(current.X, current.Y + 1);
                        if (nextPosition.Equals(previous)) continue;
                        verifyConnection = GetDirections(map, nextPosition);
                        if ((verifyConnection & Directions.Up) == Directions.Up)
                        {
                            return (true, nextPosition);
                        }
                        break;
                    case Directions.Left:
                        nextPosition = new Position(current.X - 1, current.Y);
                        if (nextPosition.Equals(previous)) continue;
                        verifyConnection = GetDirections(map, nextPosition);
                        if ((verifyConnection & Directions.Right) == Directions.Right)
                        {
                            return (true, nextPosition);
                        }
                        break;
                    case Directions.Right:
                        nextPosition = new Position(current.X + 1, current.Y);
                        if (nextPosition.Equals(previous)) continue;
                        verifyConnection = GetDirections(map, nextPosition);
                        if ((verifyConnection & Directions.Left) == Directions.Left)
                        {
                            return (true, nextPosition);
                        }
                        break;
                    default:
                        throw new Exception($"Invalid direction in directionsMap {direction}");
                }

                if (!nextPosition.Equals(previous))
                {
                    return (false, nextPosition);
                }
            }
        }
        throw new Exception($"Could not find next position from {current.X},{current.Y} in direction {directions}");
    }

    private static Directions GetDirections(string[] map, Position pos)
    {
        var maxY = map.Length;
        if (pos.Y < 0 || pos.Y >= maxY) return Directions.None;
        var mapLine = map[pos.Y];
        var maxX = mapLine.Length;
        if (pos.X < 0 || pos.X >= maxX) return Directions.None;
        var mapItem = map[pos.Y][pos.X];
        var result = GetDirections(mapItem, pos);
        if(result != Directions.Start)
        {
            return result;
        }
        // find the directions using other items
        var N = pos.Y - 1 > 0 ? GetDirections(map[pos.Y - 1][pos.X], pos) : Directions.None;
        var S = pos.Y + 1 < maxY ? GetDirections(map[pos.Y + 1][pos.X], pos) : Directions.None;
        var W = pos.X - 1 > 0 ? GetDirections(map[pos.Y][pos.X - 1], pos) : Directions.None;
        var E = pos.X + 1 < maxX ? GetDirections(map[pos.Y][pos.X + 1], pos) : Directions.None;


        var directions = new List<Directions>();
        if ((N & Directions.Down) == Directions.Down)
        {
            directions.Add( Directions.Up);
        }
        if ((S & Directions.Up) == Directions.Up)
        {
            directions.Add(Directions.Down);
        }
        if ((W & Directions.Right) == Directions.Right)
        {
            directions.Add(Directions.Left);
        }
        if ((E & Directions.Left) == Directions.Left)
        {
            directions.Add(Directions.Right);
        }
        if (directions.Count != 2)
        {
            throw new Exception($"Could not find directions as start position {pos.X},{pos.Y}");
        }
        return directions[0] | directions[1];
    }

    private static char GetDirectionsCharacter(Directions directions)
    {
        return directions switch
        {
            Directions.Up | Directions.Right => 'L',
            Directions.Left | Directions.Down => '7',
            Directions.Left | Directions.Up => 'J',
            Directions.Down | Directions.Right => 'F',
            Directions.Down | Directions.Up => '|',
            Directions.Left | Directions.Right => '-',
            Directions.None => '.',
            _ => throw new Exception($"Invalid directions {directions}")
        };
    }

    private static Directions GetDirections(char mapItem, Position position)
    {
        switch (mapItem)
        {
            case 'L':
                return Directions.Up | Directions.Right;
            case '7':
                return Directions.Left | Directions.Down;
            case 'F':
                return Directions.Right | Directions.Down;
            case '-':
                return Directions.Left | Directions.Right;
            case '|':
                return Directions.Up | Directions.Down;
            case 'J':
                return Directions.Up | Directions.Left;
            case '.':
            case 'O':
            case 'I':
                return Directions.None | Directions.None;
            case 'S':
                return Directions.Start | Directions.Start;
            default:
                throw new Exception($"Unknown map item [{mapItem}] at {position.X}, {position.Y}");
        }
    }

    private Position FindStartPosition(string[] map) { 
        for (int i = 0; i < map.Length; i++)
        {
            var line = map[i];
            var xpos = line.IndexOf('S');
           if(xpos>=0)
            {
                return new Position (xpos, i);
            }
        }
        throw new Exception("No start position found");
    }
}

static class EnumExtensions
{
    public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum
    {
        var AValue = (int)(object)value;
        foreach (var key in Enum.GetValues(value.GetType()))
        {
            var keyValue = (int)key;
            if (keyValue == 0) continue; // skip the zero value
            if ((keyValue & AValue) == keyValue)
                yield return (T)key;
        }
    }
}

[Flags]
public enum Directions
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Start = 16
}

public readonly struct Position
{
    public int X { get; }
    public int Y { get; }
    public Directions[] Track { get; }

    public static Position Minus1()
    {
        return new Position(-1, -1);
    }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
        Track = Array.Empty<Directions>();
    }

    public Position(int x, int y, Directions[] travelDirection)
    {
        X = x;
        Y = y;
        Track = travelDirection;
    }

    public long LongValue()
    {
        return (long)Y * 100000 + X;
    }

    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y;
    }
}
