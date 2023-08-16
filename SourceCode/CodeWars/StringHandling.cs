using NUnit.Framework;
using System;
using System.Text;

namespace CodeWars
{
    public class StringHandling
    {
        public static string RemoveCharsFromStringWithBuilder(string value, char toRemove)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.IndexOf(toRemove) >= 0)
            {
                StringBuilder builder = new(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] != toRemove)
                    {
                        builder.Append((value[i]));
                    }
                }

                return builder.ToString();
            }
            return value;
        }

        public static string RemoveCharFromStringFast(string value, char toRemove)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var target = new char[value.Length];
            var targetCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c.CompareTo(toRemove) == 0) continue;
                target[targetCount] = c;
                targetCount++;
            }
            return new string(target, 0, targetCount);
        }

        public static string RemoveCharsWithStringReplace(string value, string[] toRemove)
        {
            foreach (var c in toRemove)
            {
                value = value.Replace(c, "");
            }
            return value;
        }

        public static string RemoveCharsFromStringFast(string value, char[] toRemove)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var target = new char[value.Length];
            var targetCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                var found = false;
                for (int j = 0; j < toRemove.Length; j++)
                {
                    var c2 = toRemove[j];
                    if (c.CompareTo(c2) == 0)
                    {
                        found = true;
                        break;
                    }
                }
                if (found) continue;
                target[targetCount] = c;
                targetCount++;
            }
            return new string(target, 0, targetCount);
        }

        public static string RemoveCharsFromStringCorrected(string value, char toRemove)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(toRemove, StringComparison.OrdinalIgnoreCase))
            {
                value = value.Replace(toRemove.ToString(), string.Empty);
            }
            return value;
        }
    }

    [TestFixture]
    public class StringHandlingTests
    { 
        [TestCase('A', null, 'o', "")]
        [TestCase('B', "", 'o', "")]
        [TestCase('C', "The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringFastTest(char testName, string value, char toRemove, string expected)
        {
            Console.WriteLine($"Test {testName}");
            Assert.AreEqual(expected, StringHandling.RemoveCharsFromStringFast(value, new[] { toRemove }));            
        }

        [TestCase('A', null, 'o', "")]
        [TestCase('B', "", 'o', "")]
        [TestCase('C', "The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringWithBuilderTest(char testName, string value, char toRemove, string expected)
        {
            Console.WriteLine($"Test {testName}");
            Assert.AreEqual(expected, StringHandling.RemoveCharsFromStringWithBuilder(value, toRemove));
        }

        [TestCase(null, 'o', "")]
        [TestCase("", 'o', "")]
        [TestCase("The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringCorrectedTest(string value, char toRemove, string expected)
        {
            Assert.AreEqual(expected, StringHandling.RemoveCharsFromStringCorrected(value, toRemove));
        }
    }
}
