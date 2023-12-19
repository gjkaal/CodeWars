using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode2023.Day14;

namespace AdventOfCode2023;
[TestClass]
public class Day18
{
    private static Action<string> StdOut => System.Console.WriteLine;

    private readonly string[] Puzzle = File.ReadLines("InputFiles\\Day18.txt").ToArray();

    private const string Example = @"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)";

    [TestMethod]
    public void ParserTest()
    {
        var input = Example.Split(Environment.NewLine);
        foreach (var line in input)
        {
            Command command = ParseCommand(line);
            StdOut.Invoke($"{command.Direction} {command.Distance} {command.Color}");
        }
        Assert.IsTrue(true, "Parser completed");
    }

    [DataTestMethod]
    [DataRow("R 6 (#70c710)", Directions.Right, 461937)]
    [DataRow("D 5 (#0dc571)", Directions.Down, 56407)]
    [DataRow("L 2 (#5713f0)", Directions.Right, 356671)]
    [DataRow("L 5 (#8ceee2)", Directions.Left, 577262)]
    public void TestSampleCommand(string value, Directions direction, int distance)
    {
        var command = ParseCommand2(value);
        Assert.AreEqual(direction, command.Direction);
        Assert.AreEqual(distance, command.Distance);
    }

    [TestMethod]
    public void TestSampleCommandlistSimple()
    {
        DPosition[] path = GetPath(Example.Split(Environment.NewLine), ParseCommand);
        double area = 0;
        for (var i = 0; i < path.Length-1; i++)
        {
            var p = path[i];
            var p2 = path[i + 1];
            // (x2 - x1) * (y2 + y1) / 2
            area += ((p2.X - p.X) * (p2.Y + p.Y)/2) ;
        }
        
        StdOut.Invoke($"Area: {area}");

        Assert.AreEqual(62, area);
    }

    [TestMethod]
    public void TestSampleCommandlist()
    {
        var map = ParseCommandset(Example.Split(Environment.NewLine), ParseCommand2);
        DrawMap(map);

        Assert.AreEqual(952408144115, CountMap(map, '#'));
    }


    [TestMethod]
    public void TestCommandlistSimple()
    {
        var map = ParseCommandset(Puzzle, ParseCommand);
        DrawMap(map);

        Assert.AreEqual(67891, CountMap(map, '#'));
    }

    private DPosition[] GetPath(string[] input, Func<string, Command> commandParser)
    {
        var pos = new DPosition(1,1);
        var path = new List<DPosition> { pos };
        foreach (var line in input)
        {
            Command command = commandParser(line);
            switch (command.Direction)
            {
                case Directions.Right:
                    pos = new DPosition(pos.X + command.Distance + 0.5, pos.Y);
                    break;
                case Directions.Left:
                    pos = new DPosition(pos.X - command.Distance - 0.5, pos.Y);
                    break;
                case Directions.Up:
                    pos = new DPosition(pos.X, pos.Y - command.Distance - 0.5);
                    break;
                case Directions.Down:
                    pos = new DPosition(pos.X, pos.Y + command.Distance + 0.5);
                    break;
                default:
                    break;
            }
            path.Add(pos);
        }
        path.Add(new DPosition(1, 1));
        return path.ToArray();
    }

    private char[][] ParseCommandset(IEnumerable<string> input, Func<string, Command> commandParser)
    {
        var xSize = 0;
        var ySize = 0;
        var minX = 0;
        var minY = 0;
        var pos = new Position(0, 0);
        foreach (var line in input)
        {
            Command command = commandParser(line);
            pos = UpdateMapPosition(pos, null, command, '.');
            if (pos.X > xSize) xSize = pos.X;
            if (pos.Y > ySize) ySize = pos.Y;
            if (pos.X < minX) minX = pos.X;
            if (pos.Y < minY) minY = pos.Y;
        }
        xSize += 1;
        ySize += 1;

        var map = new char[ySize-minY][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new char[xSize-minX];
            for (int j = 0; j < map[i].Length; j++)
            {
                map[i][j] = '.';
            }
        }

        pos = new Position(-minX, -minY);
        foreach (var line in input)
        {
            Command command = commandParser(line);
            pos = UpdateMapPosition(pos, map, command, '#');
        }

        pos = FindInnerCell(map);
        while(pos.X >= 0 && pos.Y >= 0)
        {
            FloodFill(map, pos, '.', '#');
            pos = FindInnerCell(map);
        }
        return map;
    }

    private Position FindInnerCell(char[][] map) { 
        for (int y = 0; y < map.Length; y++)
        {
            var count = 0;
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] != '.' &&
                    (x == 0 || map[y][x-1] == '.') &&
                    (x == map[y].Length-1 || map[y][x+1] == '.')
                    ) {
                    count += 1;
                }
                if (map[y][x] == '.' && ((count % 2) == 1))
                {
                    return new Position(x,y);
                }
            }
        }
        return new Position(-1, -1);
    }

    private long CountMap(char[][] map, char v){
        var count = 0;
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == v) count++;
            }
        }
        return count;
    }

    private void FloodFill(char[][] map, Position viewPos, char oldChar, char newChar)
    {
        var stack = new Stack<Position>();
        stack.Push(viewPos);
        while (stack.Count>0)
        {
            // Pop off end of stack.
            viewPos = stack.Pop();
            if (viewPos.Y < 0 || viewPos.Y >= map.Length) continue;
            if (viewPos.X < 0 || viewPos.X >= map[viewPos.Y].Length) continue;
            if (map[viewPos.Y][viewPos.X] != oldChar) continue;
            map[viewPos.Y][viewPos.X] = newChar;
            stack.Push(new Position(viewPos.X, viewPos.Y - 1));
            stack.Push(new Position(viewPos.X, viewPos.Y + 1));
            stack.Push(new Position(viewPos.X - 1, viewPos.Y));
            stack.Push(new Position(viewPos.X + 1, viewPos.Y));
        }
    }

    private Position UpdateMapPosition(Position pos, char[][]? map, Command command, char line)
    {
        if (map != null)
        {
            if (pos.Y >= map.Length)
            {
                throw new IndexOutOfRangeException($"Index out of range {pos.X}, {pos.Y}");
            }
            if (pos.X >= map[pos.Y].Length)
            {
                throw new IndexOutOfRangeException($"Index out of range {pos.X}, {pos.Y}");
            }
            map[pos.Y][pos.X] = line;
        }

        for (var n = 0; n < command.Distance; n++)
        {
            switch(command.Direction)
            {
                case Directions.Right:
                    pos = new Position(pos.X + 1, pos.Y);
                    break;
                case Directions.Left:
                    pos = new Position(pos.X - 1, pos.Y);
                    break;
                case Directions.Up:
                    pos = new Position(pos.X, pos.Y-1);
                    break;
                case Directions.Down:
                    pos = new Position(pos.X, pos.Y + 1);
                    break;
                default:
                    break;
            }

            if (map != null)
            {
                if (pos.Y >= map.Length) { 
                    throw new IndexOutOfRangeException($"Index out of range {pos.X}, {pos.Y}");
                }
                if(pos.X >= map[pos.Y].Length)
                {
                    throw new IndexOutOfRangeException($"Index out of range {pos.X}, {pos.Y}");
                }
                map[pos.Y][pos.X] = line;
            }
        }
        return pos;
    }

    private static Command ParseCommand2(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var hexDistance = '0'+ parts[2][2..7].Trim().ToUpperInvariant();
        var dir = (parts[2][7]) switch
        {
            '0' => Directions.Right,
            '2' => Directions.Left,
            '3' => Directions.Up,
            '1' => Directions.Down,
            _ => throw new NotImplementedException($"Cannot handle value: {parts[2][8]}")
        };
        var distance = Convert.FromHexString(hexDistance);
        var length = 0;
        for (int i = 0; i < distance.Length; i++)
        {
            length = length * 256 + distance[i];
        }
        return new Command { Direction = dir, Distance = length };
    }

    private static Command ParseCommand(string line){ 
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dir = (parts[0][0]) switch
        {
            'R' => Directions.Right,
            'L' => Directions.Left,
            'U' => Directions.Up,
            'D' => Directions.Down,
            _ => throw new NotImplementedException()
        };
        var length = int.Parse(parts[1]);
        var htmlColor = parts[2][1..8].Trim().ToUpperInvariant();
        var color = ColorTranslator.FromHtml(htmlColor);
        return new Command { Direction = dir, Distance = length, Color = color };
    }

    private void DrawMap(char[][] map) 
    { 
        for (int y = 0; y < map.Length; y++)
        {
            StdOut.Invoke(new string(map[y]));
        }
    }
}

public readonly struct DPosition
{
    public DPosition(double x, double y) : this()
    {
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }
}

    public class Command
{
    public Directions Direction { get; set; }
    public int Distance { get; set; }
    public Color Color { get; set; }
}