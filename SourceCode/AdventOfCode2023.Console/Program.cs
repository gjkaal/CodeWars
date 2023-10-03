namespace AdventOfCode2023.Console;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Usage: AdventOfCode2023.Console <timeout>");
            return;
        }
        var timeout = Convert.ToInt16(args[0]);
        System.Console.WriteLine("Start calculating");
        var calculator = new Day5Calculator((s) => System.Console.WriteLine(s));
        calculator.ReadAlmanakFile(File.OpenRead("InputFiles/Day5.txt"));
        calculator.FindLowestSeedRangeLocation(timeout);
        System.Console.WriteLine("Done calculating");
        System.Console.WriteLine("Press [ENTER] to stop");
        System.Console.ReadLine();
    }
}
