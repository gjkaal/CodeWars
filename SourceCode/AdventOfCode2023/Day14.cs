using System.Text;

namespace AdventOfCode2023;
[TestClass]
public class Day14
{

    private static readonly Action<string> StdOut = System.Console.WriteLine;

    const string input1 = @"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....";

    const string input2 = @"OOOO.#.O..
OO..#....#
OO..O##..O
O..#.OO...
........#.
..#....#.#
..O..#.O.O
..O.......
#....###..
#....#....";

    const string input3 = @"OOOO.#.O..
OO..#....#
OO..O##..O
O..#.OO...
........#.
..#....#.#
..O..#.O.O
..O.......
#....###..
#....#O...";

    private static readonly string[] cycles = new string[]
    {
        @".....#....
....#...O#
...OO##...
.OO#......
.....OOO#.
.O#...O#.#
....O#....
......OOOO
#...O###..
#..OO#....",
        @".....#....
....#...O#
.....##...
..O#......
.....OOO#.
.O#...O#.#
....O#...O
.......OOO
#..OO###..
#.OOO#...O",
        @".....#....
....#...O#
.....##...
..O#......
.....OOO#.
.O#...O#.#
....O#...O
.......OOO
#...O###.O
#.OOO#...O"
    };

    [TestMethod]
    public void TestRollNorth() 
    {
        var map = new BoulderMap(input1.Split(Environment.NewLine));
        var expect = new BoulderMap(input2.Split(Environment.NewLine));
        map = map.RollNorth();
        map.PlotMap(StdOut);
        Assert.IsTrue(expect.IsEqualTo( map.Roll(Direction.North)));
    }

    [TestMethod]
    public void TestCycle()
    {
        var map = new BoulderMap(input1.Split(Environment.NewLine));

        map.Cycle(1, StdOut, false);
        map.PlotMap(StdOut);
        var expect = new BoulderMap(cycles[0].Split(Environment.NewLine));
        Assert.IsTrue(expect.IsEqualTo(map));

        map.Cycle(1, StdOut, false);
        map.PlotMap(StdOut);
        expect = new BoulderMap(cycles[1].Split(Environment.NewLine));
        Assert.IsTrue(expect.IsEqualTo(map));

        map.Cycle(1, StdOut, false);
        map.PlotMap(StdOut);
        expect = new BoulderMap(cycles[2].Split(Environment.NewLine));
        Assert.IsTrue(expect.IsEqualTo(map));
    }

    [TestMethod]
    public void TestCycleReduce()
    {
        var map1 = new BoulderMap(input1.Split(Environment.NewLine));
        var reduced = 0;
        map1.Cycle(1000, (s) => { reduced += 1; StdOut.Invoke(s); }, false);
        var hash1 = map1.CalculateHash();
        Assert.AreEqual(0, reduced);

        var map2 = new BoulderMap(input1.Split(Environment.NewLine));
        map2.Cycle(1000, (s) => { reduced += 1; StdOut.Invoke(s); }, true);
        var hash2 = map2.CalculateHash();

        Assert.IsTrue(reduced>0);
        StdOut($"Reduced loops: {reduced}");

        map1.PlotMap(StdOut);
        map2.PlotMap(StdOut);
    }

    [TestMethod]
    public void TestCalculateLoadNorth()
    {
        var map = new BoulderMap(input2.Split(Environment.NewLine));
        Assert.AreEqual(136, map.CalculateLoad(Direction.North));
    }

    [TestMethod]
    public void TestCalculateLoadNorth2()
    {
        var map = new BoulderMap(input3.Split(Environment.NewLine));
        Assert.AreEqual(137, map.CalculateLoad(Direction.North));
    }

    [TestMethod]
    public void TestCalculateLoadNorthDay14()
    {
        var fi = new FileInfo("InputFiles/Day14.txt");
        var mapData = fi.ReadMapData().First();
        var map = new BoulderMap(mapData);

        map = map.RollNorth();
        var maxLoad = map.CalculateLoad(Direction.North);

        map.PlotMap(StdOut);
        StdOut($"Max Load North: {maxLoad}");
        Assert.IsTrue(maxLoad > 102689);
        Assert.AreEqual(109665, maxLoad);
    }

    [TestMethod]
    public void TestCalculateLoadNorthDay14Cycled()
    {
        var fi = new FileInfo("InputFiles/Day14.txt");
        var mapData = fi.ReadMapData().First();
        var map = new BoulderMap(mapData);

        map = map.Cycle(1000000000L, StdOut, true);
        var maxLoad = map.CalculateLoad(Direction.North);

        StdOut($"Max Load North: {maxLoad}");
        Assert.AreEqual(96061, maxLoad);
    }

    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }
}

public class BoulderMap
{
    public char[,] Map { get; private set; }
    public int XSize => Map.GetLength(0);
    public int YSize => Map.GetLength(0);
    public BoulderMap()
    {
        Map = new char[10, 10];
        for(int i = 0; i < XSize; i++)
        {
            for(int j = 0; j < YSize; j++)
            {
                Map[i, j] = '.';
            }
        }
    }

    public BoulderMap(string[] map)
    {
        Map = new char[map[0].Length, map.Length];
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                Map[i, j] = map[j][i];
            }
        }
    }

    public BoulderMap(BoulderMap map)
    {
        Map = new char[map.XSize, map.YSize];
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                Map[i, j] = map.Map[i, j];
            }
        }
    }
}

public static class BoulderMapExtensions
{
    public static bool IsEqualTo(this BoulderMap map, BoulderMap other)
    {
        if (map.YSize != other.YSize || map.XSize != other.XSize) return false;
        for (int j = 0; j < map.YSize; j++)
        {
            for (int i = 0; i < map.XSize; i++)
            {
                if (map.Map[i,j] != other.Map[i,j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static void PlotMap(this BoulderMap map, Action<string> stdOut)
    {
        stdOut.Invoke($"Map {map.XSize}x{map.YSize}");
        for (int j = 0; j < map.YSize; j++)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < map.XSize; i++)
            {
                sb.Append(map.Map[i, j]);
            }
            stdOut.Invoke(sb.ToString());
        }
    }

    private readonly static Dictionary<ulong, long> _mapCycles = new Dictionary<ulong, long>();
    public static BoulderMap Cycle(this BoulderMap map, long cycleCount, Action<string> logger, bool withReducer)
    {
        var loopCount = 0L;
        while(cycleCount > 0)
        {
            map = map.RollNorth().RollWest().RollSouth().RollEast();
            if (withReducer)
            {
                var hash = map.CalculateHash();
                if (_mapCycles.ContainsKey(hash))
                {
                    var cycle = loopCount - _mapCycles[hash];
                    logger.Invoke($"Cycle detected: {cycleCount} at {loopCount}");
                    cycleCount = cycleCount % cycle;
                    logger.Invoke($"Cycle count reduced to {cycleCount}");
                }
                else
                {
                    _mapCycles.Add(hash, loopCount);
                }
            }
            loopCount++;
            cycleCount--;
        }
        return map;
    }

    
    public static ulong CalculateHash(this BoulderMap map)
    {
        ulong hashedValue = 3074457345618258791ul;

        for (var y = 1; y < map.YSize; y++)
        {
            for (var x = 0; x < map.XSize; x++)
            {
                ulong mapValue =  (map.Map[x, y]) switch
                {
                    '.' => 0L,
                    '#' => 1L,
                    'O' => 2L,
                    _ => throw new Exception("Invalid map value")
                };
                hashedValue += mapValue;
                hashedValue *= 3074457345618258799ul;
            }
        }
        return hashedValue;
    }
    

    public static BoulderMap RollNorth(this BoulderMap map) {
        var movement = true;
        while (movement)
        {
            movement = false;
            for (var y = 1; y < map.YSize; y++)
            {
                for (var x = 0; x < map.XSize; x++)
                {
                    if (map.Map[x, y] == 'O' && map.Map[x, y - 1] == '.')
                    {
                        map.Map[x, y-1] = 'O';
                        map.Map[x, y] = '.';
                        movement = true;
                    }
                }
            }
        }
        return map;
    }

    public static BoulderMap RollWest(this BoulderMap map)
    {
        var movement = true;
        while (movement)
        {
            movement = false;
            for (var y = 0; y < map.YSize; y++)
            {
                for (var x = 1; x < map.XSize; x++)
                {
                    if (map.Map[x, y] == 'O' && map.Map[x - 1, y] == '.')
                    {
                        map.Map[x - 1, y] = 'O';
                        map.Map[x, y] = '.';
                        movement = true;
                    }
                }
            }
        }
        return map;
    }

    public static BoulderMap RollSouth(this BoulderMap map)
    {
        var movement = true;
        while (movement)
        {
            movement = false;
            for (var y = map.YSize-2; y >= 0; y--)
            {
                for (var x = 0; x < map.XSize; x++)
                {
                    if (map.Map[x, y] == 'O' && map.Map[x, y + 1] == '.')
                    {
                        map.Map[x, y + 1] = 'O';
                        map.Map[x, y] = '.';
                        movement = true;
                    }
                }
            }
        }
        return map;
    }

    public static BoulderMap RollEast(this BoulderMap map)
    {
        var movement = true;
        while (movement)
        {
            movement = false;
            for (var y = 0; y < map.YSize; y++)
            {
                for (var x = map.XSize-2; x >=0; x--)
                {
                    if (map.Map[x, y] == 'O' && map.Map[x + 1, y] == '.')
                    {
                        map.Map[x + 1, y] = 'O';
                        map.Map[x, y] = '.';
                        movement = true;
                    }
                }
            }
        }
        return map;
    }


    public static BoulderMap Roll(this BoulderMap map, Day14.Direction direction)
    {
        var newMap = new BoulderMap(map);
        switch (direction)
        {
            case Day14.Direction.North:
                newMap.RollNorth();
                break;
            case Day14.Direction.East:
                newMap.RollEast();
                break;
            case Day14.Direction.South:
                newMap.RollSouth();
                break;
            case Day14.Direction.West:
                newMap.RollWest();
                break;
            default:
                break;
        }
        return newMap;
    }

    public static long CalculateLoadNorth(this BoulderMap map)
    {
        var load = 0L;
        var offset = map.YSize;
        for (var y = 0; y < map.YSize; y++)
        {
            for (var x = 0; x < map.XSize; x++)
            {
                if (map.Map[x, y] == 'O')
                {
                    load = load + (offset - y);
                }
            }
        }
        return load;
    }

    public static long CalculateLoad(this BoulderMap map, Day14.Direction direction)
    {
        switch (direction)
        {
            case Day14.Direction.North:
                return map.CalculateLoadNorth();
            case Day14.Direction.East:
                return 0;
            case Day14.Direction.South:
                return 0;
            case Day14.Direction.West:
                return 0;
            default:
                break;
        }
        return 0;
    }
}