using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day17
{
    private readonly  Action<string> StdOut = System.Console.WriteLine;

    private const string SampleData = @"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533";

    [TestMethod]
    public void LoadMepTest()
    {
        var map = new HeatlossMap(SampleData.Split(Environment.NewLine));
        Assert.AreEqual(13, map.Width);
        Assert.AreEqual(13, map.Height);
        Assert.AreEqual(4, map.ValueAt(1,0));
        Assert.AreEqual(3, map.ValueAt(2,5));
    }

    [TestMethod]
    public void FindMinimumPathSample()
    {
        var map = new HeatlossMap(SampleData.Split(Environment.NewLine));
        var start = new Position(0, 0);
        var finish = new Position(map.Width-1, map.Height-1);
        var (path, heatLoss) = map.FindPath(start, finish);
        StdOut($"heatLoss: {heatLoss}");
        DrawMap(map, path);
        Assert.AreEqual(26, path.Length);
    }

    private void DrawMap(HeatlossMap map, Position[] path)
    {
        for (int y = 0; y < map.Height; y++)
        {
            var line = new StringBuilder();
            for (int x = 0; x < map.Width; x++)
            {
                var pos = new Position(x, y);
                if (path.Contains(pos))
                {
                    line.Append("X");
                }
                else
                {
                    line.Append(map.ValueAt(x, y));
                }
            }
            StdOut(line.ToString());
        }
    }
}

public class HeatlossMap
{
    private readonly int[,] _map;

    public int Width => _map.GetLength(0);
    public int Height => _map.GetLength(1);

    public HeatlossMap(string[] mapData)
    {
        _map = new int[mapData[0].Length, mapData.Length];
        for (int y = 0; y < mapData.Length; y++)
        {
            for (int x = 0; x < mapData[y].Length; x++)
            {
                _map[x, y] = int.Parse(mapData[y][x].ToString());
            }
        }
    }

    public int ValueAt(int xPos, int yPos) 
    { 
        return _map[xPos, yPos]; 
    }



    public (Position[] Path, int Heatloss) FindPath(Position start, Position finish) 
    { 
        // Use Dijkstra's algorithm to find the shortest path
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        var unvisited = new List<Position>();
        var visited = new List<Position>();
        var distance = new Dictionary<Position, int>();
        var previous = new Dictionary<Position, Position?>();
        foreach (var pos in AllPositions())
        {
            unvisited.Add(pos);
            distance[pos] = int.MaxValue;
            previous[pos] = null;
        }
        distance[start] = 0;
        var track = new Position[] { start, start };
        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(p => distance[p]).First();
            //if (current.Equals(finish))
            //{
            //    break;
            //}
            unvisited.Remove(current);
            visited.Add(current);

            foreach (var pos in Neighbours(current))
            {
                if (visited.Contains(pos))
                {
                    continue;
                }
                var alt = distance[current] + ValueAt(pos.X, pos.Y);
                if (distance.ContainsKey(pos) && alt < distance[pos])
                {
                    distance[pos] = alt;
                    previous[pos] = current;
                }
            }
        }

        // Reconstruct the optimal path from the finish
        // back to the start with the shortest distance
        var path = new List<Position>();
        Position? currentPos = finish;
        var heatLoss = 0;
        while (currentPos is not null )
        {
            path.Add((Position)currentPos);
            heatLoss += ValueAt(((Position)currentPos).X, ((Position)currentPos).Y);
            currentPos = previous[(Position)currentPos];
        }

        return (path.ToArray(), heatLoss);
    }

    private readonly Position[] Matrix = new Position[] 
    {
        new Position( 0, -1 ), // Up
        new Position( 0, 1 ), // Down
        new Position( -1, 0 ), // Left
        new Position( 1, 0 ), // Right
    };

    private IEnumerable<Position> Neighbours(Position current) 
    {
        foreach(var step in Matrix)
        {
            var pos = new Position(current.X + step.X, current.Y + step.Y, new Directions[2]);
            if (pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height)
            {
                continue;
            }
            Directions dir = Directions.None;
            if (current.X == pos.X && current.Y < pos.Y) dir = Directions.Down;
            if (current.X == pos.X && current.Y > pos.Y) dir = Directions.Up;
            if (current.X < pos.X && current.Y == pos.Y) dir = Directions.Right;
            if (current.X > pos.X && current.Y == pos.Y) dir = Directions.Left;
            if (current.Track[0] == dir)
            {
                continue;
            }
            pos.Track[0] = dir;
            yield return pos;
        }        
    }

    private IEnumerable<Position> AllPositions() 
    { 
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return new Position(x, y, new Directions[] { Directions.None, Directions.None});
            }
        }
    }
}
