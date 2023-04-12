using NUnit.Framework;
using System;

namespace CodeWars
{

    public class TenPinBowling
    {
        public static int BowlingScore(string frames)
        {
            System.Console.WriteLine(frames);
            string[] throws = frames.Split(' ');
            int length = throws.Length; // should be 10...
            int[] score = new int[length];
            int[] final = new int[length];
            Zip(score, throws, (v) => FrameScore(v));
            for(var x = length-1; x >= 0; x--)
            {
                if (score[x] > 0)
                {
                    final[x] = score[x];
                }
                if (score[x] == -2)
                {
                    if (score[x + 1] == -2)
                    {
                        if (score[x + 2] >= -1)
                        {
                            var n = FrameScore(throws[x + 2]);
                            var t = throws[x + 2].Substring(0, throws[x + 2].Length-1)+"0";
                            final[x] = 20 + (n<0 ? 10 : n);
                            final[x + 1] = 10 + FrameScore(t);
                            if (final[x] > 30) final[x] = 30;
                        }
                        else
                        {
                            final[x] = 30;
                        }                        
                    }
                    else if (score[x + 1] >= 0)
                    {
                        var n = FrameScore(throws[x + 1].Substring(0, 2));
                        final[x] = 10 + (n<0?10:n);
                        if (final[x] > 30) final[x] = 30;
                    }
                    else
                    {
                        final[x] = 20;
                    }
                }
                if (score[x] == -1)
                {
                    final[x] = 10 + EvalPosition(throws[x+1][0]);
                }
            }
            return Sum(final, (v) => v);
        }

        private static void Zip(int[] score, string[] throws, Func<string, int> func)
        {
            var size = score.Length;
            for (int x = 0; x < size; x++) score[x] = func(throws[x]);
        }

        private static int Sum(int[] score, Func<int, int> func)
        {
            var sum = 0;
            var size = score.Length;
            for (int x = 0; x < size; x++) sum += func(score[x]);
            return sum;
        }

        internal static int FrameScore(string frame)
        {
            if (frame.Length == 3)
            {
                return frame[1] == '/' ? 10 + EvalPosition(frame[2]) : 20 + EvalPosition(frame[2]);
            }
            if (frame.Length == 2 && frame[1] == '/')
            {
                // spare
                return -1;
            }
            if (frame.Length == 1)
            {
                // strike
                return -2;
            }
            return EvalPosition(frame[0]) + EvalPosition(frame[1]);
        }

        private static int EvalPosition(char frame)
        {
            return (frame == 'X' ? 10 : int.Parse(frame.ToString()));
        }
    }

    public class TenPinBowlingTests
    {
        [TestCase("11 11 11 11 11 11 11 11 11 11", 20)]
        [TestCase("5/ 4/ 3/ 2/ 1/ 0/ X 9/ 4/ 8/8", 150)]
        [TestCase("X X 9/ 80 X X 90 8/ 7/ 44", 171)]
        [TestCase("54 9/ 9/ X 00 9/ X 50 6/ XXX", 148)]
        [TestCase("32 8/ 90 20 45 X 06 62 X 1/X", 114)]
        [TestCase("7/ 52 06 17 07 51 61 X X 34", 103)]
        public void ExampleTests(string value, int expected)
        {
            Assert.AreEqual(expected, TenPinBowling.BowlingScore(value));
        }

        [Test]
        public void ExampleTests2()
        {
            Assert.AreEqual(115, TenPinBowling.BowlingScore("00 5/ 4/ 53 33 22 4/ 5/ 45 XXX"));
        }

        [Test]
        public void ExampleTests3()
        {
            Assert.AreEqual(107, TenPinBowling.BowlingScore("6/ 3/ 23 60 32 X 9/ 9/ 54 12"));
        }

        [Test]
        public void ExampleTests4()
        {
            Assert.AreEqual(150, TenPinBowling.BowlingScore("5/ 4/ 3/ 2/ 1/ 0/ X 9/ 4/ 8/8"));
        }

        [TestCase("XXX", 30)]
        [TestCase("XX0", 20)]
        [TestCase("XX2", 22)]
        [TestCase("8/8", 18)]
        [TestCase("2/0", 10)]
        [TestCase("0/X", 20)]
        public void FrameScoreTest(string value, int expected)
        {
            Assert.AreEqual(expected, TenPinBowling.FrameScore(value));
        }

        [Test]
        public void ExampleSpareTests()
        {
            Assert.AreEqual(29, TenPinBowling.BowlingScore("1/ 11 11 11 11 11 11 11 11 11"));
            Assert.AreEqual(29, TenPinBowling.BowlingScore("11 1/ 11 11 11 11 11 11 11 11"));
            Assert.AreEqual(31, TenPinBowling.BowlingScore("11 11 11 11 11 11 11 11 11 1/3"));
        }

        [Test]
        public void ExampleStrikeTests()
        {
            Assert.AreEqual(300, TenPinBowling.BowlingScore("X X X X X X X X X XXX"));
        }
    }
}
