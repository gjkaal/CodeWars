using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ChipSoft
{
    /// <summary>
    ///
    /// </summary>
    public class Keypad
    {
        private const string InvalidKeyForKeypad = "The provided key is not available on this keypad";

        /// <summary>
        /// Number of keys in X direction.
        /// </summary>
        public int KeypadSizeX { get; set; }

        /// <summary>
        /// Number of keys in Y direction.
        /// </summary>
        public int KeypadSizeY { get; set; }

        /// <summary>
        /// Distance between center point of keys on the keypad.
        /// </summary>
        public float GridSize { get; set; }

        private readonly Dictionary<int, Vector2> KeyPositions = new Dictionary<int, Vector2>();

        public Keypad(int xSize, int ySize, float gridSize)
        {
            KeypadSizeX = xSize;
            KeypadSizeY = ySize;
            GridSize = gridSize;
            InitKeypad(KeyPositions, KeypadSizeX, KeypadSizeY, GridSize);
        }

        private void InitKeypad(Dictionary<int, Vector2> keyPositions, int keypadSizeX, int keypadSizeY, float gridSize)
        {
            var key = 0;
            keyPositions.Clear();
            for (int y = 0; y < keypadSizeY; y++)
                for (int x = 0; x < keypadSizeX; x++)
                {
                    keyPositions.Add(++key, new Vector2(x * gridSize, y * gridSize));
                }
        }

        public float CalculateTrackLength(int[] keys)
        {
            if (keys.Length <= 1) return 0.0f;
            var length = 0.0f;
            var currentKey = GetVector(keys[0]);
            for (int i = 1; i < keys.Length; i++)
            {
                var nextKey = GetVector(keys[i]);
                length += (currentKey - nextKey).Length();
                currentKey = nextKey;
            }
            return length;
        }

        private Vector2 GetVector(int keyNumber)
        {
            if (KeyPositions.ContainsKey(keyNumber)) return KeyPositions[keyNumber];
            throw new ArgumentOutOfRangeException(InvalidKeyForKeypad);
        }
    }

    /// <summary>
    /// Test the tracklength
    /// </summary>
    public class TrackLengthTests
    {
        private Keypad sut;

        [SetUp]
        public void Setup()
        {
            sut = new Keypad(3, 3, 1.0f);
        }

        [TestCase(new int[] { 1, 2, 3, 6, 9 }, 4.0f)]
        [TestCase(new int[] { 1, 2 }, 1.0f)]
        [TestCase(new int[] { 1, 3 }, 2.0f)]
        [TestCase(new int[] { 1, 5 }, 1.14142f)]
        [TestCase(new int[] { 8, 6 }, 1.14142f)]
        [TestCase(new int[] { 5, 8, 7, 4, 2, 6, 9, 5, 1 }, 9.657f)]
        [TestCase(new int[] { 5, 8, 7, 4, 2, 6, 9, 1 }, 9.657f)]
        public void ValidatTrackLength(IEnumerable<int> keys, float expectedLength)
        {
            var actual = sut.CalculateTrackLength(keys.ToArray());
            Assert.AreEqual(expectedLength, actual, 0.001);
        }
    }
}
