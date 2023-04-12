using System;
using NUnit.Framework;

namespace CodeWars
{
    public class ConwayLife
    {
        public static int[,] GetGeneration(int[,] cells, int generation)
        {
            Console.WriteLine($"Generation : {generation}");
            Console.WriteLine(GetMap(cells));
            int[,] nextGen = NewWorld(cells);
            while (generation > 0)
            {
                nextGen = NextGen(nextGen);
                generation -= 1;
            }
            return Crop(nextGen);
        }

        private static string GetMap(int[,] cells)
        {
            if (cells == null) return "empty";
            var sb = new System.Text.StringBuilder();
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    sb.Append((cells[x, y] > 0) ? '*' : ' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static int[,] NextGen(int[,] cells)
        {
            var next = NewWorld(cells);
            for (var x = 0; x < next.GetLength(0); x++)
                for (var y = 0; y < next.GetLength(1); y++)
                    next[x, y] = ApplyRulesOfLife(next, x, y);
            for (var x = 0; x < next.GetLength(0); x++)
                for (var y = 0; y < next.GetLength(1); y++)
                    next[x, y] = ((next[x, y] & 2) ==2 ? 1 : 0);
            return next;
        }

        private static int ApplyRulesOfLife(int[,] cells, int x, int y)
        {
            var width = cells.GetLength(0);
            var height = cells.GetLength(1);
            var current = cells[x, y];
            var alive = (cells[x, y] & 1) == 1;
            var neighbors = 0;
            for(var n = -1; n<=1; n++)
            for(var m = -1; m <= 1; m++)
                {
                    if ((n == 0 && m == 0) ||
                       (x+n < 0 || x+n >= width) ||
                       (y + m < 0 || y + m >= height)) continue;
                    if ((cells[x + n, y + m] & 1)==1) neighbors++;
                }                  
            if (alive && neighbors < 2) return 1;
            if (alive && neighbors > 3) return 1;
            if (alive && (neighbors == 2 || neighbors == 3)) return 3;
            if (!alive && neighbors == 3) return 2;
            return current;
        }

        // Create a new world with at least a single empty border
        public static int[,] NewWorld(int[,] cells)
        {
            var width = cells.GetLength(0)-1;
            var height = cells.GetLength(1)-1;
            bool addRow0 = false, addRow1 = false,addColumn0 = false, addColumn1 = false;
            for (var x = 0; x<=width; x++)
            {
                if (cells[x, 0] == 1) addRow0 = true;
                if (cells[x, height] == 1) addRow1 = true;
            }
            for (var y = 0; y <= height; y++)
            {
                if (cells[0, y] == 1) addColumn0 = true;
                if (cells[width, y] == 1) addColumn1 = true;
            }
            // outside rows and colums are empty
            if (!(addRow0 || addRow1 || addColumn0 || addColumn1)) return cells;
            var newWidth = width + (addColumn0 ? 1 : 0) + (addColumn1 ? 1 : 0) + 1;
            var newHeight = height + (addRow0 ? 1 : 0) + (addRow1 ? 1 : 0) + 1;
            var result = new int[newWidth, newHeight];
            int offsetX = (addColumn0 ? 1 : 0), offsetY = (addRow0 ? 1 : 0);
            for (var x = 0; x <= width; x++)
                for (var y = 0; y <= height; y++)
                    result[offsetX+x, offsetY + y] = cells[ x, y];
            return result;
        }

        public static int[,] Crop(int[,] cells)
        {
            var maxArraySize = (int)Math.Sqrt(int.MaxValue);
            int minX= maxArraySize, maxX= -maxArraySize, minY= maxArraySize, maxY= -maxArraySize;
            for(var x=0;x<cells.GetLength(0);x++)
            for(var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x,y]==0) continue;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            if (minX == maxArraySize) return new int[0, 0];
            var xSize = maxX - minX + 1;
            var ySize = maxY - minY + 1;
            var result = new int[xSize, ySize];
            for (var x = 0; x < xSize; x++)
                for (var y = 0; y < ySize; y++)
                    result[x, y] = cells[minX + x, minY + y];
            return result;
        }
    }

    [TestFixture]
    public class ConwayLifeTest
    {
        [Test]
        public void TestNewWorld()
        {
            int[,] cells = new int[,] { { 1, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestNewWorld2()
        {
            int[,] cells = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestNewWorld3()
        {
            int[,] cells = new int[,] { { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestNewWorld4()
        {
            int[,] cells = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestNewWorld5()
        {
            int[,] cells = new int[,] { { 0, 0, 0, 0 }, {  1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 1, 1, 0, 0 }, { 0, 0, 0, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestNewWorld6()
        {
            int[,] cells = new int[,] { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 1 }, { 0, 1, 1, 0 } };
            int[,] expected = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } };
            var result = ConwayLife.NewWorld(cells);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestCropCells()
        {
            int[,] cells = new int[,] { { 1, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
            var result = ConwayLife.Crop(cells);
            CollectionAssert.AreEqual(cells, result);
        }

        [Test]
        public void TestCropCells2()
        {
            int[,] cells = new int[,] { { 0, 1, 0, 0 }, { 0, 0, 1, 1 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } };
            int[,] expect = new int[,] { { 1, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
            var result = ConwayLife.Crop(cells);
            CollectionAssert.AreEqual(expect, result);
        }

        [Test]
        public void TestCropOne()
        {
            int[,] cells = new int[,] { { 1, 0, 0 }, { 0, 0, 0 }, { 0,0,0 } };
            var result = ConwayLife.Crop(cells);
            CollectionAssert.AreEqual(new int[,] { { 1 } }, result);
        }

        [Test]
        public void TestCropEmpty()
        {
            int[,] cells = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            var result = ConwayLife.Crop(cells);
            CollectionAssert.AreEqual(new int[,] { { } }, result);
        }

        [Test]
        public void TestGlider()
        {
            int[][,] gliders =
            {
                new int[,] {{1,0,0},{0,1,1},{1,1,0}},
                new int[,] {{0,1,0},{0,0,1},{1,1,1}}
            };
            Console.WriteLine("Glider");
            int[,] res = ConwayLife.GetGeneration(gliders[0], 1);
            CollectionAssert.AreEqual(gliders[1], res, "Output doesn't match expected");
        }
    }
}
