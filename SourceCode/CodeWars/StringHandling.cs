using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeWars
{
    [TestFixture]
    public class StringHandling
    {
        public static string RemoveCharsFromStringWithBuilder(string value, char toRemove)
        {
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

        public static string RemoveCharsFromStringFast(string value, char toRemove)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var target = new char[value.Length];
            var targetCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c.CompareTo(toRemove) == 0) continue;
                target[targetCount] = c;
                targetCount++;
            }
            return new string(target,0, targetCount);
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

        public static string RemoveCharsFromStringLevel(string value, char toRemove)
        {
            if (value.Contains(toRemove))
            {
                value = value.Replace(toRemove, string.Empty.First());
            }
            return value;
        }

        [TestCase('A', null, 'o', "")]
        [TestCase('B', "", 'o', "")]
        [TestCase('C', "The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringFastTest(char testName, string value, char toRemove, string expected)
        {
            Console.WriteLine($"Test {testName}");
            Assert.AreEqual(expected, RemoveCharsFromStringFast(value, toRemove));            
        }

        [TestCase('A', null, 'o', "")]
        [TestCase('B', "", 'o', "")]
        [TestCase('C', "The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringWithBuilderTest(char testName, string value, char toRemove, string expected)
        {
            Console.WriteLine($"Test {testName}");
            Assert.AreEqual(expected, RemoveCharsFromStringWithBuilder(value, toRemove));
        }

        [TestCase(null, 'o', "")]
        [TestCase("", 'o', "")]
        [TestCase("The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringLevelTest(string value, char toRemove, string expected)
        {
            Assert.AreEqual(expected, RemoveCharsFromStringLevel(value, toRemove));
        }

        [TestCase(null, 'o', "")]
        [TestCase("", 'o', "")]
        [TestCase("The Quick Brown Dog", 'o', "The Quick Brwn Dg")]
        public void RemoveCharsFromStringCorrectedTest(string value, char toRemove, string expected)
        {
            Assert.AreEqual(expected, RemoveCharsFromStringCorrected(value, toRemove));
        }
    }
}
