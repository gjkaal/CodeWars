using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

[TestClass]
public class Day2
{
    private readonly string[] input = File.ReadAllLines("InputFiles/Day2.txt");

    public Dictionary<DiceColor, int> gameConfiguration = new Dictionary<DiceColor, int> { 
        { DiceColor.Red, 12 }, 
        { DiceColor.Green, 13 }, 
        { DiceColor.Blue, 14 } 
    };

    [TestMethod]
    public void TestGameValidTest()
    {
        var lines = input;
        var validGames = 0;
        foreach (var line in lines.Where(a => a.Length > 0))
        {
            var (isValid, gameId) = IsGameValid(line, gameConfiguration);
            Assert.IsTrue(gameId>0);
            if (isValid)
            {
                validGames += gameId;
            }
        }
        Assert.AreEqual(2256, validGames);
    }

    [TestMethod]
    public void FindGamePowerTotal()
    {
        var lines = input;
        var totalPower = 0;
        foreach (var line in lines.Where(a => a.Length > 0))
        {
            var m = GetDiceMatrix(line).Matrix;
            var power = m[DiceColor.Red] * m[DiceColor.Green] * m[DiceColor.Blue];
            totalPower += power;
        }
        Assert.AreEqual(74229, totalPower);
    }

    [DataTestMethod]
    [DataRow("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", 4, 2, 6)]
    [DataRow("Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", 1, 3, 4)]
    [DataRow("Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", 20, 13, 6)]
    public void TestGameMinimumSet(string game, int expectedRed, int expectedGreen, int expectedBlue) { 
        var m = GetDiceMatrix(game).Matrix;
        Assert.AreEqual(expectedBlue, m[DiceColor.Blue]);
        Assert.AreEqual(expectedRed, m[DiceColor.Red]);
        Assert.AreEqual(expectedGreen, m[DiceColor.Green]);
    }

    private (int GameId,  Dictionary<DiceColor, int> Matrix) GetDiceMatrix(string game)
    {
        var gameSplit = game.Split(':');
        var gameId = Convert.ToInt32(gameSplit[0].Split(' ')[1]);
        var gameParts = gameSplit[1].Split(';');
        var dice = new Dictionary<DiceColor, int> {
            {DiceColor.Red, 0 },
            {DiceColor.Green, 0 },
            {DiceColor.Blue, 0 }
        };
        foreach (var gamePart in gameParts)
        {
            var diceParts = gamePart.Split(',');
            foreach (var dicePart in diceParts)
            {
                var dicePartParts = dicePart.Trim().Split(' ');
                var diceColor = Enum.Parse<DiceColor>(dicePartParts[1], true);
                var diceCount = Convert.ToInt32(dicePartParts[0]);
                if (dice[diceColor] < diceCount)
                {
                    dice[diceColor] = diceCount;
                }
            }
        }
        return (gameId, dice);
    }

    [DataTestMethod]
    [DataRow("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", true)]
    [DataRow("Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", false)]
    public void TestGameValid(string game, bool expected) => Assert.AreEqual(expected, IsGameValid(game, gameConfiguration).isValid);

    public (bool isValid, int gameId) IsGameValid(string game, Dictionary<DiceColor, int> gameConfig)
    {
        var gameSplit = game.Split(':');
        var gameId = Convert.ToInt32(gameSplit[0].Split(' ')[1]);
        var gameParts = gameSplit[1].Split(';');
        var dice = new Dictionary<DiceColor, int>(gameConfig);
        foreach (var gamePart in gameParts)
        {
            var diceParts = gamePart.Split(',');
            foreach (var dicePart in diceParts)
            {
                var dicePartParts = dicePart.Trim().Split(' ');
                var diceColor = Enum.Parse<DiceColor>(dicePartParts[1], true);
                var diceCount = Convert.ToInt32(dicePartParts[0]);
                if (dice[diceColor] < diceCount)
                {
                    return (false, gameId);
                }
            }
        }
        return (true, gameId);
    }

    public enum DiceColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        White,
        Black
    }
}


