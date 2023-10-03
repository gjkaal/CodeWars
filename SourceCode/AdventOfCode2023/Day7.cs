using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023;

[TestClass]
public class Day7
{
    // see https://adventofcode.com/2023/day/7
    private static readonly Action<string> StdOut = System.Console.WriteLine;
    private readonly string[] input = File.ReadAllLines("InputFiles/Day7.txt");
    private readonly string[] sample = new string[]
    {
        "32T3K 765",
        "T55J5 684",
        "KK677 28",
        "KTJJT 220",
        "QQQJA 483"
    };

    [DataTestMethod]
    [DataRow("32T3K", HandType.OnePair)]
    [DataRow("T55J5", HandType.ThreeOfAKind)]
    [DataRow("KK677", HandType.TwoPair)]
    [DataRow("KTJJT", HandType.TwoPair)]
    [DataRow("23232", HandType.FullHouse)]
    [DataRow("QQQJA", HandType.ThreeOfAKind)]
    public void TestGetHandType(string input, HandType expected) => Assert.AreEqual(expected, GetHandType(input));
    
    [DataTestMethod]
    [DataRow("22222", 0x00011111 )]
    [DataRow("23232", 0x00012121 )]
    [DataRow("KTJJT", 0x000DA00A )]
    public void TestGetScore(string input, int expected) => Assert.AreEqual(expected, GetScore(input));

    [DataTestMethod]
    [DataRow("22222", 0x00711111)]
    [DataRow("23232", 0x00512121)]
    [DataRow("KTJJT", 0x006DA00A)]
    public void TestHandvalue(string input, int expected) => Assert.AreEqual(expected, GetHandValue(input));

    [TestMethod]
    public void TestSample() => Assert.AreEqual(5905, TotalScore(sample));

    [TestMethod]
    public void TestInput() => Assert.AreEqual(251421071, TotalScore(input));
    
    private int TotalScore(string[] values)
    {
        var cardsScored = values.Select(s => (
            hand: GetHandValue(s[..5]),
            bid: int.Parse(s[6..])))
         .OrderBy(m => m.hand)
         .Select(m => m.bid)
         .ToArray();
        var total = 0;
        for (var i = 0; i < cardsScored.Length; i++)
        {
            total += (i + 1) * cardsScored[i];
        }
        return total;
    }

    private int GetHandValue(string input) { 
        var handType = GetOptimalHandType(input);
        var score = GetScore(input);
        return ((int)handType * 0x00100000) + score;
    }

    private HandType GetOptimalHandType(string input)
    {
        var handType = GetHandType(input);
        if (input.Any(c => c == 'J'))
        {
            foreach(var card in input.Where(c => c != 'J'))
            {
                var newHand = input.Replace('J', card);
                var newHandType = GetHandType(newHand);
                if (newHandType > handType)
                {
                    handType = newHandType;
                }
            }
            return handType;
        }
        else
        {
            return handType;
        }
    }

        private HandType GetHandType(string input) { 

        var cards = new Dictionary<char, int>();
        foreach (var card in input)
        {
            if (cards.ContainsKey(card))
            {
                cards[card]++;
            }
            else
            {
                cards.Add(card, 1);
            }
        }

        switch(cards.Count)
        {
            case 1:
                return HandType.FiveOfAKind;
            case 2:
                return cards.Values.Any(v => v == 4) ? HandType.FourOfAKind : HandType.FullHouse;
            case 3:
                return cards.Values.Any(v => v == 3) ? HandType.ThreeOfAKind : HandType.TwoPair;
            case 4:
                return HandType.OnePair;
            default:
                return HandType.HighCard;
        }        
    }

    private int GetScore (string hand)
    {
        var value = 0;
        var cards = "J23456789-T-QKA";
        for (var i = 0; i < 5; i++)
        {
            var index = cards.IndexOf(hand[i]);
            if (index < 0) throw new Exception($"Invalid card: '{hand[i]}' not supported");
            value = (value << 4) + index;
        }
        return value;
    }

    public enum HandType
    {
        None,
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind,
    }
}
