using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace CodeWars
{
    public class HumanTimeFormat
    {
        const int Second = 1;
        const int Minute = 60;
        const int Hour = 60 * 60;
        const int Day = Hour * 24;
        const int Year = Day * 365;
        const int Now = 5;

        public static string FormatDuration(int seconds)
        {
            // names and pruralization available for translation
            var names = new string[] { "year|years", "day|days", "hour|hours", "minute|minutes", "second|seconds", "now" };
            var sizes = new int[] { Year, Day, Hour, Minute, Second };
            var sb = new List<string>();
            for (int x = 0; x < sizes.Length; x++)
            {
                var n = seconds / sizes[x];
                seconds %= sizes[x];
                if (n == 0) continue;
                var name = names[x].Split('|');
                sb.Add($"{n} {name[(n > 1 ? 1 : 0)]}");
            }

            return BuildOutput(names, sb);
        }

        private static string BuildOutput(string[] names, List<string> sb)
        {
            if (sb.Count == 0) return names[Now];
            var output = new StringBuilder();
            for (int x = 0; x < sb.Count; x++)
            {
                if (x > 0) output.Append((x == sb.Count - 1) ? " and " : ", ");
                output.Append(sb[x]);
            }
            return output.ToString();
        }
    }

    public class HumanTimeFormatTests
    {
        [Test]
        public void BasicTests()
        {
            Assert.AreEqual("now", HumanTimeFormat.FormatDuration(0));
            Assert.AreEqual("1 second", HumanTimeFormat.FormatDuration(1));
            Assert.AreEqual("1 minute and 2 seconds", HumanTimeFormat.FormatDuration(62));
            Assert.AreEqual("2 minutes", HumanTimeFormat.FormatDuration(120));
            Assert.AreEqual("1 hour, 1 minute and 2 seconds", HumanTimeFormat.FormatDuration(3662));
            Assert.AreEqual("182 days, 1 hour, 44 minutes and 40 seconds", HumanTimeFormat.FormatDuration(15731080));
            Assert.AreEqual("4 years, 68 days, 3 hours and 4 minutes", HumanTimeFormat.FormatDuration(132030240));
            Assert.AreEqual("6 years, 192 days, 13 hours, 3 minutes and 54 seconds", HumanTimeFormat.FormatDuration(205851834));
            Assert.AreEqual("8 years, 12 days, 13 hours, 41 minutes and 1 second", HumanTimeFormat.FormatDuration(253374061));
            Assert.AreEqual("7 years, 246 days, 15 hours, 32 minutes and 54 seconds", HumanTimeFormat.FormatDuration(242062374));
            Assert.AreEqual("3 years, 85 days, 1 hour, 9 minutes and 26 seconds", HumanTimeFormat.FormatDuration(101956166));
            Assert.AreEqual("1 year, 19 days, 18 hours, 19 minutes and 46 seconds", HumanTimeFormat.FormatDuration(33243586));
        }
    }
}
