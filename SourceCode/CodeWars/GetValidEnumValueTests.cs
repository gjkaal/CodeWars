using NUnit.Framework;
using System;

namespace CodeWars
{
    public class GetValidEnumValueTests
    {
        public static T GetValidEnumValue<T>(int value) where T : System.Enum
        {
            return (Enum.IsDefined(typeof(T), value))
                ? (T)Enum.ToObject(typeof(T), value)
                : throw new ArgumentOutOfRangeException(nameof(value), $"Invalid value : {value}");
        }

        [TestCase(1, TestEnumSet.start)]
        [TestCase(2, TestEnumSet.second)]
        [TestCase(4, TestEnumSet.last)]
        public void TestValidConversion(int value, TestEnumSet expected)
        {
            Assert.AreEqual(expected, GetValidEnumValue<TestEnumSet>(value));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(5)]
        public void TestInValidConversion(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = GetValidEnumValue<TestEnumSet>(value));
        }

        public enum TestEnumSet
        {
            start = 1,
            second = 2,
            last = 4
        }
    }
}