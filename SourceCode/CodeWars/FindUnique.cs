using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeWars
{   
    public class FindUniqueTests
    {
        [SetUp]
        public void Setup()
        {
            // Find the unique elements is a list
        }     

        [TestCase(new[] { 1, 2, 2, 2 }, ExpectedResult = 1)]
        [TestCase(new[] { -2, 2, 2, 2 }, ExpectedResult = -2)]
        [TestCase(new[] { 11, 11, 14, 11, 11 }, ExpectedResult = 14)]
        public int ValidateGetUnique(IEnumerable<int> numbers)
        {
            return FindUnique.GetUnique(numbers);
        }
    }

    public static class FindUnique
    {
        public static int GetUnique(IEnumerable<int> numbers)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            var enumerator = numbers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int val = enumerator.Current;
                if (result.ContainsKey(val)) result[val] += 1;
                else result.Add(val, 1);
            }
            foreach (int val in result.Keys)
            {
                if (result[val] == 1) return val;
            }
            throw new InvalidOperationException("No unique values");
        }
    }
}