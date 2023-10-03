using System.Text;

namespace AdventOfCode2023;

[TestClass]
public class Day1
{
    private static readonly Action<string> StdOut = System.Console.WriteLine;
    private readonly string[] input = File.ReadAllLines("InputFiles/Day1.txt");

    [TestMethod]
    public void GetTotalCalibration()
    {
        var lines = input;
        var sum = 0;
        foreach (var line in lines.Where(a => a.Length > 0))
        {
            var numbers = GetNumbers(line);
            Assert.AreEqual(2, numbers.Length);
            if(!(numbers[0] > '0' && numbers[0] <= '9'))
            {
                StdOut.Invoke(line);
                StdOut.Invoke(numbers);
                StdOut.Invoke(Convert.ToInt32(numbers).ToString());
            }
            Assert.IsTrue(numbers[0] > '0' && numbers[0] <= '9');
            Assert.IsTrue(numbers[1] > '0' && numbers[1] <= '9');
            sum += Convert.ToInt32(numbers);
        }
        StdOut($"Sum: {sum}");
        Assert.AreEqual(53539, sum);
    }

    [DataTestMethod]
    [DataRow("two1nine\r\neightwothree\r\nabcone2threexyz\r\nxtwone3four\r\n4nineeightseven2\r\nzoneight234\r\n7pqrstsixteen", 281)]
    public void TotalCalibrationTest(string value, int expected) 
    {
        var lines = value.Split(Environment.NewLine);
        var sum = 0;
        foreach (var line in lines)
        {
            sum += Convert.ToInt32(GetNumbers(line));
        }
        Assert.AreEqual(expected, sum);
    }

    [DataTestMethod]
    [DataRow("5ffour295", "5295")]
    [DataRow("2vdqng1sixzjlkjvq", "21")]
    [DataRow("five8six", "8")]
    [DataRow("92", "92")]
    [DataRow("five0six", "0")]
    public void GetSimpleNumbersTest(string value, string expected) => Assert.AreEqual(expected, GetSimpleNumbers(value));

    [DataTestMethod]
    [DataRow("5ffour295", "55")]
    [DataRow("2vdqng1sixzjlkjvq", "26")]
    [DataRow("xtwone3four", "24")]
    [DataRow("five86", "56")]
    [DataRow("63jqkh7sixnine1jmqsqtdhpg", "61")]
    [DataRow("five8six", "56")]
    [DataRow("92", "92")]
    [DataRow("9", "99")]
    [DataRow("db7", "77")]
    [DataRow("fponeight86phxr", "16")]
    [DataRow("eight", "88")]
    [DataRow("five0six", "56")]
    [DataRow("threes6", "36")]
    [DataRow("7pqrstsixteen", "76")]
    [DataRow("cccgsqgj2seveneight2", "22")]
    [DataRow("rnnkxtbnx1threekbddbpzthreerxcnbcgx", "13")]
    [DataRow("ptwonefive2threekfrtvnbmplpsevenseven","27")]
    [DataRow("79sixone","71")]
    [DataRow("79sixone4","74")]
    [DataRow("79twone","71")]
    [DataRow("3cnsj", "33")]
    [DataRow("mheightfhllpvk6rdnrznkndp","86")]
    public void GetNumbersTest(string value, string expected) => Assert.AreEqual(expected, GetNumbers(value));

    [DataTestMethod]
    [DataRow("5295", 55)]
    [DataRow("2", 22)]
    [DataRow("8", 88)]
    [DataRow("0", 0)]
    public void CalibrationTest(string value, int expected) => Assert.AreEqual(expected, Calibration(value));

    private int Calibration(string numbers)
    {
        if (numbers.Length == 0)
        {
            return 0;
        }
        return (numbers[0] - '0')* 10 + (numbers[numbers.Length-1] - '0');
    }

    private string GetSimpleNumbers(string line)
    {
        var sb = new char[line.Length];
        var i = 0;
        foreach (var c in line)
        {
            if (c >= '0' && c <= '9')
            {
                sb[i++] = c;
            }
        }
        return new string(sb, 0, i);
    }

    private static readonly Dictionary<string, char> NumberNames = new Dictionary<string, char>
    {
        {"nine", '9'},
        {"zero", '0'},
        {"two", '2'},
        {"one", '1'},
        {"three", '3'},
        {"four", '4'},
        {"five", '5'},
        {"six", '6'},
        {"seven", '7'},
        {"eight", '8'},
    };

    private string GetNumbers(string line)
    {
        char leftNumber = '\0';
        char rightNumber = '\0';
        var x = 0;
        while (leftNumber == '\0' && x < line.Length)
        {
            leftNumber = GetNumberValue(line, x++);            
        }
        x = line.Length - 1;
        while (rightNumber == '\0' && x >=0)
        {
            rightNumber = GetNumberValue(line, x--);
        }
        return new string(new[] { leftNumber, rightNumber });
    }

    private static char GetNumberValue(string line, int x)
    {
        if (line[x] >= '0' && line[x] <= '9')
        {
            return line[x];
        }
        foreach (var item in NumberNames)
        {
            if ((x + item.Key.Length) <= line.Length &&
                line.Substring(x, item.Key.Length) == item.Key)
            {
                return item.Value;
            }
        }
        return '\0';
    }  
}