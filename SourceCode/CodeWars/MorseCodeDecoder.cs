
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeWars
{
    class MorseCodeDecoder
    {
        public static string DecodeBits(string bits)
        {
            var sb = new System.Text.StringBuilder();
            var lastActive = false;
            var isActive = false;
            var started = false;
            var edge = false;
            int time = 0;
            char s;

            int speed = FindTransmissionSpeed(bits);

            for (var i=0; i<=bits.Length; i++)
            {
                s = (i < bits.Length) ? bits[i] : '0';
                lastActive = isActive;
                isActive = (s == '1');
                time++;
                edge = (lastActive != isActive);
                if (edge)
                {
                    if (lastActive)
                    {
                        // changed to low
                        if (time > speed)
                        {
                            sb.Append('-');
                        }
                        else
                        {
                            sb.Append('.');
                        }                       
                    }
                    else if (started)
                    {
                        // changed to low
                        if (time > speed*3)
                        {
                            // word spacing
                            sb.Append("   ");
                        }
                        else if (time > speed)
                        {
                            // char spacing
                            sb.Append(' ');
                        }
                    }
                    started = true;
                    time = 0;
                }
            }
            return sb.ToString();
        }

        private static int FindTransmissionSpeed(string bits)
        {
            var dot = int.MaxValue;
            bits = bits.Trim('0');
            var checkHigh = bits.Split('0', StringSplitOptions.RemoveEmptyEntries);
            var checkLow = bits.Trim('0').Split('1', StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in checkHigh)
            {
                if (c.Length < dot) dot = c.Length;
            }

            foreach (var c in checkLow)
            {
                if (c.Length < dot) dot = c.Length;
            }
            return dot;
        }

        public static string DecodeMorse(string morseCode)
        {
            var sb = new System.Text.StringBuilder();
            var source = morseCode.Split(' ');
            var pauze = false;
            var started = false;
            foreach (var c in source)
            {
                if (string.IsNullOrWhiteSpace(c))
                {
                    if (!pauze) pauze = true;
                }
                else
                {
                    if (pauze && started) sb.Append(' ');
                    pauze = false;
                    started = true;
                    sb.Append(MorseCode.Get(c));
                }
            }
            return sb.ToString();
        }
    }

    [TestFixture]
    public class MorseCodeDecoderTests
    {
        [TestCase("1100110011001100000011000000111111001100111111001111110000000000000011001111110011111100111111000000110011001111110000001111110011001100000011", "HEY JUDE")]
        [TestCase("01110", "E")]
        public void TestDecodeMorse(string input, string expected)
        {
            Assert.AreEqual(expected, MorseCodeDecoder.DecodeMorse(MorseCodeDecoder.DecodeBits(input)));
        }

        [TestCase("1100110011001100000011000000111111001100111111001111110000000000000011001111110011111100111111000000110011001111110000001111110011001100000011", ".... . -.--   .--- ..- -.. .")]
        [TestCase("01110", ".")]
        public void TestDecodeBits(string input, string expected)
        {
            Assert.AreEqual(expected, MorseCodeDecoder.DecodeBits(input));
        }

        [Test]
        public void MorseCodeDecoderBasicTest()
        {
            try
            {
                string input = ".... . -.--   .--- ..- -.. .";
                string expected = "HEY JUDE";

                string actual = MorseCodeDecoder.DecodeMorse(input);

                Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
                Assert.Fail("There seems to be an error somewhere in your code. Exception message reads as follows: " + ex.Message);
            }
        }
    }

    [TestFixture]
    public class MorseCodeTests
    {
        [TestCase(".-", 'A')]
        public void TestSingleCharacter(string value, char expect) => Assert.AreEqual(expect, MorseCode.Get(value));
    }

    public static class MorseCode
    {
        public static char Get(string code)
        {
            foreach(var e in morse)
            {
                if (e.Value == code) return e.Key;
            }
            return '\0';
        }
        private static readonly Dictionary<char, string> morse = new Dictionary<char, string>()
            {
                {'A' , ".-"},
                {'B' , "-..."},
                {'C' , "-.-."},
                {'D' , "-.."},
                {'E' , "."},
                {'F' , "..-."},
                {'G' , "--."},
                {'H' , "...."},
                {'I' , ".."},
                {'J' , ".---"},
                {'K' , "-.-"},
                {'L' , ".-.."},
                {'M' , "--"},
                {'N' , "-."},
                {'O' , "---"},
                {'P' , ".--."},
                {'Q' , "--.-"},
                {'R' , ".-."},
                {'S' , "..."},
                {'T' , "-"},
                {'U' , "..-"},
                {'V' , "...-"},
                {'W' , ".--"},
                {'X' , "-..-"},
                {'Y' , "-.--"},
                {'Z' , "--.."},
                {'0' , "-----"},
                {'1' , ".----"},
                {'2' , "..---"},
                {'3' , "...--"},
                {'4' , "....-"},
                {'5' , "....."},
                {'6' , "-...."},
                {'7' , "--..."},
                {'8' , "---.."},
                {'9' , "----."},
            };
    }
}
