namespace AdventOfCode2023;

[TestClass]
public class Day3
{
    private readonly string[] input = File.ReadAllLines("InputFiles/Day3.txt");
    private const string example = "467..114..\r\n...*......\r\n..35..633.\r\n......#...\r\n617*......\r\n.....+.58.\r\n..592.....\r\n......755.\r\n...$.*....\r\n.664*598..\r\n..........";
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    [TestMethod]
    public void TestSchemaSum()
    {
        var lines = example.Split("\r\n");
        var partNumbers = FindPartnumbers(lines).ToArray();

        StdOut.Invoke(string.Join(",", partNumbers));

        Assert.AreEqual(8, partNumbers.Length);

        var sum = partNumbers.Sum(a => a);
        Assert.AreEqual(4361, sum);
    }

    [TestMethod]
    public void TestGearRatioSum()
    {
        var lines = example.Split("\r\n");
        var gearRatios = FindGearRatios(lines).ToArray();
        Assert.AreEqual(3, gearRatios.Length);
        Assert.AreEqual(16345, gearRatios[0]);
        Assert.AreEqual(451490, gearRatios[1]);
        Assert.AreEqual(397072, gearRatios[2]);
    }

    [TestMethod]
    public void EngineGearRatioSum()
    {
        var lines = input;
        var gearRatios = FindGearRatios(lines).ToArray();
        var totalSum = gearRatios.Sum();
        Assert.AreEqual(84289137, totalSum);
    }

        private int[] FindGearRatios(string[] lines)
    {
        var gearRatios = new List<int>();
        for (var i = 0; i < lines.Length; i++)
        {
            var (currentLine, prevLine, nextLine) = GetLines(lines, i);
            var gears = GearPositions(currentLine);
            StdOut.Invoke($"Line {i} has {gears.Count()} gears");
            if (gears.Count() == 0) continue;

            var prevLineNumbers = FindNumbers(prevLine);
            var curentLineNumbers = FindNumbers(currentLine);
            var nextLineNumbers = FindNumbers(nextLine);
            foreach (var gear in gears)
            {
                var locatedGears = new Dictionary<string, int>();
                var foundGears = new List<(string Position, int GearValue)>();
                var match = curentLineNumbers.Where(a => (a.StartPos + a.Value.Length) == gear || (a.StartPos == gear + 1)).ToArray();
                foreach (var m in match)
                {
                    var position = $"{m.StartPos}:{i}";
                    foundGears.Add(new(position, Convert.ToInt32(m.Value)));
                }
                for (var x = -1; x <= 1; x++)
                {
                    var xPos = gear + x;
                    foundGears.AddRange(FindGear(i - 1, prevLine, prevLineNumbers, xPos));
                    foundGears.AddRange(FindGear(i + 1, nextLine, nextLineNumbers, xPos));
                }
                foreach (var (position, gearValue) in foundGears)
                {
                    if (!locatedGears.ContainsKey(position))
                    {
                        StdOut.Invoke($"Gear found at {position} with value {gearValue}");
                        locatedGears.Add(position, Convert.ToInt32(gearValue));
                    }
                }
                if (locatedGears.Count > 1)
                {
                    StdOut.Invoke($"Gear {gear} found {locatedGears.Count} gears");
                    StdOut.Invoke($"Gears : {string.Join(", ", locatedGears.Select(m => m.Value))}");
                    Assert.AreEqual(2, locatedGears.Count);
                    var gearValues = locatedGears.Select(m => m.Value).ToArray();
                    gearRatios.Add(gearValues[0] * gearValues[1]);
                }
            }
        }
        return gearRatios.ToArray();
    }

    private IEnumerable<(string Position, int GearValue)> FindGear(int i, string line, (int StartPos, string Value)[] lineNumbers, int xPos)
    {
        if (xPos < 0 || xPos >= line.Length) yield break;
        if (IsNumber(line[xPos]))
        {
            var match = lineNumbers.Where(a => a.StartPos <= xPos && (a.StartPos + a.Value.Length) >= xPos).ToArray();
            if (match.Any())
            {
                foreach (var m in match)
                {
                    var position = $"{m.StartPos}:{i}";
                    yield return new(position, Convert.ToInt32(m.Value));
                }
            }
        }
    }

    private bool IsNumber(char c)
    {
        return c >= '0' && c <= '9';
    }

    private IEnumerable<int> GearPositions(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '*')
            {
                yield return i;
            }
        }
    }

    [TestMethod]
    public void EngineSchemaSum()
    {
        var lines = input;
        var partNumbers = FindPartnumbers(lines).ToArray();
        var sum = partNumbers.Sum(a => a);
        StdOut.Invoke(sum.ToString());
        Assert.IsTrue(sum > 522144);

        Assert.AreEqual(525181, sum);
    }

    [DataTestMethod]
    [DataRow("467..114..", new int[] { 467, 114 })]
    [DataRow("...*......", new int[] { })]
    [DataRow(".....+.58.", new int[] { 58 })]
    [DataRow(".664.598..", new int[] { 664, 598 })]
    [DataRow(".........6", new int[] { 6 })]
    public void TestFindNumbers(string line, int[] expectedNumbers)
    {
        var numbers = FindNumbers(line);
        Assert.AreEqual(expectedNumbers.Length, numbers.Length);
        for (int i = 0; i < expectedNumbers.Length; i++)
        {
            Assert.AreEqual(expectedNumbers[i], Convert.ToInt32(numbers[i].Value));
        }
    }

    [DataTestMethod]
    [DataRow("467..114..", new int[] { 0, 5 })]
    [DataRow("...*......", new int[] { })]
    [DataRow(".....+.58.", new int[] { 7 })]
    [DataRow(".664.598..", new int[] { 1, 5 })]
    public void TestFindPositions(string line, int[] expectedPositions)
    {
        var numbers = FindNumbers(line);
        Assert.AreEqual(expectedPositions.Length, numbers.Length);
        for (int i = 0; i < expectedPositions.Length; i++)
        {
            Assert.AreEqual(expectedPositions[i], numbers[i].StartPos);
        }
    }

    private (string currentLine, string prevLine, string nextLine) GetLines(string[] lines, int i)
    {
        string currentLine;
        string prevLine;
        string nextLine;
        currentLine = lines[i];
        if (i > 0)
        {
            prevLine = lines[i - 1];
        }
        else
        {
            prevLine = new string('.', currentLine.Length);
        }
        if (i < lines.Length - 1)
        {
            nextLine = lines[i + 1];
        }
        else
        {
            nextLine = new string('.', currentLine.Length);
        }
        return (currentLine, prevLine, nextLine);
    }

    private IEnumerable<int> FindPartnumbers(string[] lines)
    {
        for (var i = 0; i < lines.Length; i++)
        {
            var (currentLine, prevLine, nextLine) = GetLines(lines, i);
            var numbersInLine = FindNumbers(currentLine);
            var validPartsInLine = FindValidParts(numbersInLine, prevLine, currentLine, nextLine);
            foreach (var validPart in validPartsInLine)
            {
                yield return validPart;
            }
        }
    }

    private int[] FindValidParts((int StartPos, string Value)[] numbersInLine, string prevLine, string currentLine, string nextLine)
    {
        var result = new List<int>();
        foreach (var (startPos, value) in numbersInLine)
        {
            var valid = false;
            var length = value.Length;
            var xStart = startPos - 1;
            var xEnd = startPos + length;
            if (xStart >= 0)
            {
                valid = valid || (currentLine[xStart] != '.');
            }
            if (xEnd < currentLine.Length)
            {
                valid = valid || (currentLine[xEnd] != '.');
            }
            if (!valid)
            {
                if (xStart < 0) xStart = 0;
                if (xEnd >= currentLine.Length) xEnd = currentLine.Length - 1;
                for (int x = xStart; x <= xEnd; x++)
                {
                    valid = valid || (prevLine[x] != '.');
                    valid = valid || (nextLine[x] != '.');
                }
            }
            if (valid)
            {
                result.Add(Convert.ToInt32(value));
            }
        }
        return result.ToArray();
    }

    private static (int StartPos, string Value)[] FindNumbers(string line)
    {
        var numbers = new List<(int, string)>();
        var numberStart = -1;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] >= '0' && line[i] <= '9')
            {
                if (numberStart == -1)
                {
                    numberStart = i;
                }
            }
            else
            {
                if (numberStart != -1)
                {
                    var number = line.Substring(numberStart, i - numberStart);
                    numbers.Add(new(numberStart, number));
                    numberStart = -1;
                }
            }
        }

        if (numberStart != -1)
        {
            var number = line.Substring(numberStart);
            numbers.Add(new(numberStart, number));
        }

        return numbers.ToArray();
    }
}