namespace AdventOfCode2023;

[TestClass]
public class Day4
{
    private const string example = 
        "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53\r\n" +
        "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19\r\n" +
        "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1\r\n" +
        "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83\r\n" +
        "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36\r\n" +
        "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";
    private readonly string[] input = File.ReadAllLines("InputFiles/Day4.txt");
    private static readonly Action<string> StdOut = System.Console.WriteLine;

    [DataTestMethod]
    [DataRow("Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53", 8)]
    [DataRow("Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19", 2)]
    [DataRow("Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36", 0)]
    public void TestCard(string card, int expected) => Assert.AreEqual(expected, GetCardScore(card));

    [DataTestMethod]
    [DataRow("Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53", 4)]
    [DataRow("Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19", 2)]
    [DataRow("Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36", 0)]
    public void TestCardMatch(string card, int expected) => Assert.AreEqual(expected, GetCardMatch(card).matchCount);

    [TestMethod]
    public void TestCardSum()
    {
        var lines = example.Split("\r\n");
        var cards = lines.Where(a => a.Length > 0).ToArray();
        var sum = 0;
        foreach (var card in cards)
        {
            sum += GetCardScore(card);
        }
        Assert.AreEqual(13, sum);
    }

    [TestMethod]
    public void PlayingCardSum()
    {
        var lines = input;
        var cards = lines.Where(a => a.Length > 0).ToArray();
        var sum = 0;
        foreach (var card in cards)
        {
            sum += GetCardScore(card);
        }
        Assert.AreEqual(25571, sum);
    }

    [TestMethod]
    public void TotalCards()
    {
        var lines = input;
        var totalCards = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var cardTotal = FindTotalWinningCards(lines, i);
            totalCards += cardTotal;
        }
        Assert.AreEqual(8805731, totalCards);
    }

    [TestMethod]
    public void TotalCardsExample()
    {
        var lines = example.Split("\r\n");
        var totalCards = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var cardTotal = FindTotalWinningCards(lines, i);
            StdOut.Invoke($"Line {i+1} has {cardTotal} winning cards");
            totalCards += cardTotal;
        }

        foreach(var card in _cards)
        {
            StdOut.Invoke($"{card.Key} has been used {card.Value} times.");
        }

        Assert.AreEqual(30, totalCards);
    }

    private int FindTotalWinningCards(string[] lines, int index) 
    {
        if (index >= lines.Length) return 0;
        var line = lines[index];
        var cards = line.Length > 0 ? 1 : 0;
        var (cardId, matchCount) = GetCardMatch(lines[index]);
        AddCard(cardId);
        if (matchCount > 0)
        {
            for(var x = 1; x<=matchCount; x++)
            {

                cards += FindTotalWinningCards(lines, index + x);
            }
        }
        return cards;
    }

    private readonly Dictionary<int, int> _cards = new Dictionary<int, int>();
    private void AddCard(int cardId)
    {
        if (_cards.ContainsKey(cardId))
        {
            _cards[cardId]++;
        }
        else
        {
            _cards.Add(cardId, 1);
        }
    }

    private readonly Dictionary<int, int> _cardWins = new Dictionary<int, int>();
    private (int cardId, int matchCount) GetCardMatch(string card)
    {
        var cardHash = card.GetHashCode();
        if (_cardWins.ContainsKey(card.GetHashCode()))
        {
            return (cardHash, _cardWins[cardHash]);
        }
        var cardSplit = card.Split(":");
        var cardNumbers = cardSplit[1].Split("|");
        var winningNumber = GetNumbers(cardNumbers[0]);
        var drawNumber = GetNumbers(cardNumbers[1]);
        var score = 0;
        foreach (var number in drawNumber)
        {
            if (winningNumber.Contains(number)) score++;
        }
        _cardWins.Add(cardHash, score);
        return (cardHash, score);
    }


    private int GetCardScore(string card) { 
        var cardSplit = card.Split(":");
        var cardNumbers = cardSplit[1].Split("|");
        var winningNumber = GetNumbers(cardNumbers[0]);
        var drawNumber = GetNumbers(cardNumbers[1]);
        var score = 0;
        foreach (var number in drawNumber)
        {
            score = winningNumber.Contains(number) 
                ? score == 0 ? 1 : score * 2 
                : score;
        }
        return score;
    }

    private int[] GetNumbers(string value) {
        var result = new List<int>();
        foreach(var item in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            result.Add(int.Parse(item.Trim()));
        }
        return result.ToArray();
    }
}
