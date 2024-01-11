namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

/// <summary>
/// Theme: Calculate winning points from scratchcards
/// </summary>
[TestFixture]
public class Day04
{
    record Card(IEnumerable<int> Winning, IEnumerable<int> Draw)
    {
        public Card(string[] splits) :
            this(splits[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray(),
                 splits[2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray())
        { }
    }

    private static IEnumerable<Card> ParseData(string[] lines) =>
        lines.Select(l => new Card(l.Split(':', '|'))).ToArray();

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
            "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
            "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
            "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
            "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
            "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11",
        };

        var cards = ParseData(lines);

        Puzzle1(cards).Should().Be(8 + 2 + 2 + 1 + 0 + 0);
        Puzzle2(cards).Should().Be(30);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);
        var cards = ParseData(lines);

        Puzzle1(cards).Should().Be(21821);
        Puzzle2(cards).Should().Be(5539496);
    }

    // Each scratchcard has two lists of numbers separated by a vertical bar (|): a list of winning numbers and then a list of
    // numbers you have. You organize the information into a table (your puzzle input). As far as the Elf has been able to figure
    // out, you have to figure out which of the numbers you have appear in the list of winning numbers. The first match makes the
    // card worth one point and each match after the first doubles the point value of that card.
    //
    // Puzzle == How many points are they worth in total?
    private static int Puzzle1(IEnumerable<Card> cards) =>
        cards.Sum(c => DrawValue(c.Winning.Intersect(c.Draw).Count()));

    private static int DrawValue(int nbrOfWinningCards) =>
        nbrOfWinningCards > 0 ? MathExtensions.Pow2(nbrOfWinningCards - 1) : 0;

    // The rules have actually been printed on the back of every card this whole time. There's no such thing as "points". Instead,
    // scratchcards only cause you to win more scratchcards equal to the number of winning numbers you have.
    // Specifically, you win copies of the scratchcards below the winning card equal to the number of matches. So, if card 10 were
    // to have 5 matching numbers, you would win one copy each of cards 11, 12, 13, 14, and 15.
    // Copies of scratchcards are scored like normal scratchcards and have the same card number as the card they copied. So, if you
    // win a copy of card 10 and it has 5 matching numbers, it would then win a copy of the same cards that the original card 10
    // won: cards 11, 12, 13, 14, and 15. This process repeats until none of the copies cause you to win any more cards. (Cards
    // will never make you copy a card past the end of the table.)
    // Process all of the original and copied scratchcards until no more scratchcards are won. Including the original set of scratchcards.
    //
    // Puzzle == How many total scratchcards do you end up with?
    private static int Puzzle2(IEnumerable<Card> cards)
    {
        var wins  = cards.Select(c => c.Winning.Intersect(c.Draw).Count()).ToArray();
        var stack = Enumerable.Repeat(1, wins.Length).ToArray();

        for(int i = 1; i < stack.Length; i++)
        {
            for(int win = 0; win < wins[i - 1]; win++)
            {
                stack[i + win] += stack[i - 1];
            }
        }

        return stack.Sum();
    }
}
