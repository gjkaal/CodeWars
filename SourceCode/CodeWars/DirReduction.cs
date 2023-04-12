using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeWars
{
    public class DirReductionTests
    {
        [SetUp]
        public void Setup()
        {
            // Reduce the map directions to the shortest path
        }

        [Test]
        public void Test1()
        {
            string[] a = new string[] { "NORTH", "SOUTH", "SOUTH", "EAST", "WEST", "NORTH", "WEST" };
            string[] b = new string[] { "WEST" };
            Assert.AreEqual(b, DirReduction.DirReduc(a));
        }

        [Test]
        public void Test2()
        {
            string[] a = new string[] { "NORTH", "WEST", "SOUTH", "EAST" };
            string[] b = new string[] { "NORTH", "WEST", "SOUTH", "EAST" };
            Assert.AreEqual(b, DirReduction.DirReduc(a));
        }

        [TestCase(new[] { 1, 2, 2, 2 }, ExpectedResult = 1)]
        [TestCase(new[] { -2, 2, 2, 2 }, ExpectedResult = -2)]
        [TestCase(new[] { 11, 11, 14, 11, 11 }, ExpectedResult = 14)]
        public int ValidateGetUnique(IEnumerable<int> numbers)
        {
            return FindUnique.GetUnique(numbers);
        }
    }

    public static class DirReduction
    {
        public static string[] DirReduc(String[] arr)
        {
            int checkLength;
            do
            {
                checkLength = arr.Length;
                arr = Reduce(arr);
            } while (checkLength != arr.Length);
            return arr;
        }

        private static string[] RemoveEmptyElements(string[] arr)
        {
            string[] result = new string[CountElements(arr)];
            int p = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null) continue;
                result[p++] = arr[i];
            }
            return result;
        }

        private static int CountElements(string[] arr)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null) continue;
                count++;
            }
            return count;
        }

        private static string[] Reduce(string[] arr)
        {
            int position = 0;
            while (position < arr.Length - 1)
            {
                Console.WriteLine($"{arr[position]} - {arr[position + 1]}");
                if (arr[position] == "NORTH" && arr[position + 1] == "SOUTH" ||
                  arr[position] == "SOUTH" && arr[position + 1] == "NORTH" ||
                  arr[position] == "WEST" && arr[position + 1] == "EAST" ||
                  arr[position] == "EAST" && arr[position + 1] == "WEST")
                {
                    Console.WriteLine($"Reducing {arr[position]}, {arr[position + 1]}");
                    arr[position] = null;
                    arr[position + 1] = null;
                }
                position++;
            }
            return RemoveEmptyElements(arr);
        }
    }
}