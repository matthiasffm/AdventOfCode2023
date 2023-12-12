namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: play Camel Cards (== simple form of Poker)
/// </summary>
[TestFixture]
public class Day07
{
    private enum Type
    {
        HighCard        = 1,
        OnePair         = 2,
        TwoPair         = 3,
        ThreeOfAKind    = 4,
        FullHouse       = 5,
        FourOfAKind     = 6,
        FiveOfAKind     = 7,
    }

    // A hand consists of five cards labeled one of A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2. The relative strength of each card follows this order, where A is the highest and
    // 2 is the lowest.
    // Every hand is exactly one type. From strongest to weakest, they are:
    // - Five of a kind, where all five cards have the same label: AAAAA
    // - Four of a kind, where four cards have the same label and one card has a different label: AA8AA
    // - Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
    // - Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
    // - Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
    // - One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
    // - High card, where all cards' labels are distinct: 23456
    // Hands are primarily ordered based on type; for example, every full house is stronger than any three of a kind.
    // If two hands have the same type, a second ordering rule takes effect. Start by comparing the first card in each hand. If these cards are different, the hand with the
    // stronger first card is considered stronger. If the first card in each hand have the same label, however, then move on to considering the second card in each hand. If
    // they differ, the hand with the higher second card wins; otherwise, continue with the third card in each hand, then the fourth, then the fifth.
    private record Hand : IComparable<Hand>
    {
        protected Type   _rank;
        protected string _cards;

        public string Cards => _cards;

        public Hand(string cards)
        {
            _cards = cards;
            _rank  = CalcRank();
        }

        protected virtual Type CalcRank() =>
            _cards.GroupBy(c => c).Select(g => g.Count()).Order().Reverse().ToArray() switch {
                [5]         => Type.FiveOfAKind,
                [4, 1]      => Type.FourOfAKind,
                [3, 2]      => Type.FullHouse,
                [3, ..]     => Type.ThreeOfAKind,
                [2, 2, 1]   => Type.TwoPair,
                [2, ..]     => Type.OnePair,
                _           => Type.HighCard,
            };

        public int CompareTo(Hand? other)
        {
            if(other == null)
            {
                return 1;
            }
            else if(_rank == other._rank)
            {
                return _cards.CompareTo(other._cards);
            }
            else
            {
                return _rank.CompareTo(other._rank);
            }
        }
    }

    // To make things a little more interesting, the Elf introduces one additional rule. Now, J cards are jokers - wildcards that can act like whatever card would make the
    // hand the strongest type possible.
    // To balance this, J cards are now the weakest individual cards, weaker even than 2. The other cards stay in the same order: A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, J.
    // Joker cards can pretend to be whatever card is best for the purpose of determining hand type; for example, QJJQ2 is now considered four of a kind. However, for the
    // purpose of breaking ties between two hands of the same type, J is always treated as J (aka the weakest card), not the card it's pretending to be.
    private record JokerHand : Hand
    {
        private const char Joker       = 'B';
        private const char MappedJoker = '1';

        public JokerHand(string cards) : base(MapJokerCards(cards))
        {
        }

        // maps a J to the weakest card (here 1)
        private static string MapJokerCards(string cards) => cards.Replace(Joker, MappedJoker);

        // new ranking with jokers
        protected override Type CalcRank() => (baseRank: base.CalcRank(), jokers: _cards.Count(c => c == MappedJoker)) switch {
            (Type.FourOfAKind, 1)   => Type.FiveOfAKind,
            (Type.FourOfAKind, 4)   => Type.FiveOfAKind,
            (Type.FullHouse, 2)     => Type.FiveOfAKind,
            (Type.FullHouse, 3)     => Type.FiveOfAKind,
            (Type.ThreeOfAKind, 1)  => Type.FourOfAKind,
            (Type.ThreeOfAKind, 3)  => Type.FourOfAKind,
            (Type.TwoPair, 2)       => Type.FourOfAKind,
            (Type.TwoPair, 1)       => Type.FullHouse,
            (Type.OnePair, 2)       => Type.ThreeOfAKind,
            (Type.OnePair, 1)       => Type.ThreeOfAKind,
            (Type.HighCard, 1)      => Type.OnePair,
            _                       => base.CalcRank(),
        };
    }

    private record CamelCard(Hand Hand, int Bid);

    private static IEnumerable<CamelCard> ParseData(string[] lines) =>
        lines.Select(l => new CamelCard(new Hand(MapCards(l[0..5])), int.Parse(l[6..])))
             .ToArray();

    private static string MapCards(string s) =>
        string.Concat(s.ToCharArray().Select(c => MapCard(c)));

    // for basic string comparison to work with poker cards they have to be mapped to characters with the correct ordering
    private static char MapCard(char c) => c switch {
        'T' => 'A',
        'J' => 'B',
        'Q' => 'C',
        'K' => 'D',
        'A' => 'E',
        _   => c,
    };

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "32T3K 765",
            "T55J5 684",
            "KK677 28",
            "KTJJT 220",
            "QQQJA 483",
        };

        var handsAndBids = ParseData(lines);
        Puzzle(handsAndBids).Should().Be(765L * 1L + 220L * 2L + 28L * 3L + 684L * 4L + 483L * 5L);

        var jokerHandsAndBids = handsAndBids.Select(h => h with { Hand = new JokerHand(h.Hand.Cards) })
                                            .ToArray();
        Puzzle(jokerHandsAndBids).Should().Be(5905L);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);

        var handsAndBids = ParseData(lines);
        Puzzle(handsAndBids).Should().Be(250347426L);

        var jokerHandsAndBids = handsAndBids.Select(h => h with { Hand = new JokerHand(h.Hand.Cards) })
                                            .ToArray();
        Puzzle(jokerHandsAndBids).Should().Be(251224870L);
    }

    // In Camel Cards, you get a list of hands, and your goal is to order them based on the strength of each hand. To play the game, you are given a list of hands
    // and their corresponding bid (your puzzle input).
    // Each hand is followed by its bid amount. Each hand wins an amount equal to its bid multiplied by its rank, where the weakest hand gets rank 1, the second-weakest
    // hand gets rank 2, and so on up to the strongest hand.
    // Now, you can determine the total winnings of this set of hands by adding up the result of multiplying each hand's bid with its rank.
    //
    // Puzzle == Find the rank of every hand in your set. What are the total winnings?
    private static long Puzzle(IEnumerable<CamelCard> handsAndBids)
        => handsAndBids.OrderBy(h => h.Hand)
                       .Select((h, i) => (i + 1L) * h.Bid)
                       .Sum();
}
