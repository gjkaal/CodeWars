using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day9
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    private const string example1 = @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";

    [DataTestMethod]
    [DataRow("0 3 6", 9)]
    [DataRow("0 3 6 9 12 15", 18)]
    [DataRow("0 -3 -6 -9 -12 -15", -18)]
    [DataRow("1 3 6 10 15 21", 28)]
    [DataRow("10 13 16 21 30 45", 68)]
    public void TestCalculatePrediction(string value, int expected)
    {
        var result = CalculateRowPrediction(value, true);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("0 3 6", -3)]
    [DataRow("0 3 6 9 12 15", -3)]
    [DataRow("0 -3 -6 -9 -12 -15", 3)]
    [DataRow("1 3 6 10 15 21", 0)]
    [DataRow("10 13 16 21 30 45", 5)]
    public void TestCalculateFuture(string value, int expected)
    {
        var result = CalculateRowPrediction(value, false);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestCalculateStreamFuture()
    {
        long result;
        using var stream = new FileStream("InputFiles/Day9.txt", FileMode.Open);
        result = CalculateStreamPrediction(stream, true);
        Assert.AreEqual(1641934234, result);
    }

    [TestMethod]
    public void TestCalculateStreamHistory()
    {
        long result;
        using var stream = new FileStream("InputFiles/Day9.txt", FileMode.Open);
        result = CalculateStreamPrediction(stream, false);
        Assert.AreEqual(975, result);
    }

    [TestMethod]
    public void CalculatePrediction()
    {
        long result;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(example1));        
        result = CalculateStreamPrediction(stream, true);        
        Assert.AreEqual(114, result);
    }

    private long CalculateStreamPrediction(Stream stream, bool future) { 
        var result = 0L;
        using var reader = new StreamReader(stream);
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    result += CalculateRowPrediction(line, future);
                }
            }
        }
        return result;
    }

    private int CalculateRowPrediction(string value, bool future) { 
        var numbers = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToList();
        return GetPrediction(numbers, future);
    }

    private int GetPrediction(List<int> numbers, bool future)
    {
        var checkDiff = new List<int>();
        var isZero = true;
        for (int i = 1; i < numbers.Count; i++)
        {
            var diff = numbers[i] - numbers[i - 1];
            checkDiff.Add(diff);
            isZero &= diff == 0;
        }
        if (future) { 
            if (isZero)
            {
                return numbers.Last();
            }
            else
            {
                return numbers.Last() + GetPrediction(checkDiff, future);
            }
        }
        else
        {
            if (isZero)
            {
                return numbers.First();
            }
            else
            {
                return numbers.First() - GetPrediction(checkDiff, future);
            }

        }
    }
}
