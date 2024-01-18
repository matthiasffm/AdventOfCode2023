namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: predict the next or previous environment value
/// </summary>
[TestFixture]
public class Day09
{
    private static IEnumerable<IList<int>> ParseData(IEnumerable<string> lines) =>
        lines.Select(l => l.Split(' ', StringSplitOptions.TrimEntries)
                           .Select(s => int.Parse(s))
                           .ToList());

    [Test]
    public void TestSamples()
    {
        string[] lines = [
            "0 3 6 9 12 15",
            "1 3 6 10 15 21",
            "10 13 16 21 30 45",
        ];

        var histories = ParseData(lines);

        Puzzle1(histories).Should().Be(18 + 28 + 68);
        Puzzle2(histories).Should().Be(-3 + 0 + 5);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);

        var histories = ParseData(lines);

        Puzzle1(histories).Should().Be(1887980197);
        Puzzle2(histories).Should().Be(990);
    }

    // You pull out your handy Oasis And Sand Instability Sensor and analyze your surroundings. The OASIS produces a report of many values and how they are changing over
    // time (your puzzle input). Each line in the report contains the history of a single value.
    // To best protect the oasis, your environmental report should include a prediction of the next value in each history. To do this, start by making a new sequence from
    // the difference at each step of your history. If that sequence is not all zeroes, repeat this process, using the sequence you just generated as the input sequence.
    // Once all of the values in your latest sequence are zeroes, you can extrapolate what the next value of the original history should be.
    // Analyze your OASIS report and extrapolate the next value for each history. 
    //
    // Puzzle == What is the sum of these extrapolated values?
    private static int Puzzle1(IEnumerable<IList<int>> histories)
        => histories.Sum(h => ExtrapolateNextValue(h));

    private static int ExtrapolateNextValue(IList<int> history)
        => history.All(h => h == 0) ? 0 : history.Last() + ExtrapolateNextValue(Diff(history));

    // Of course, it would be nice to have even more history included in your report. Surely it's safe to just extrapolate backwards as well, right?
    // For each history, repeat the process of finding differences until the sequence of differences is entirely zero. Then, rather than adding a zero to the end and filling
    // in the next values of each previous sequence, you should instead add a zero to the beginning of your sequence of zeroes, then fill in new first values for each
    // previous sequence.
    // Analyze your OASIS report again, this time extrapolating the previous value for each history.
    //
    // Puzzle ==  What is the sum of these extrapolated values?
    private static int Puzzle2(IEnumerable<IList<int>> histories)
        => histories.Sum(h => ExtrapolatePreviousValue(h));

    private static int ExtrapolatePreviousValue(IList<int> history)
        => history.All(h => h == 0) ? 0 : history.First() - ExtrapolatePreviousValue(Diff(history));

    // calculates the differences between each pair of values in history
    private static IList<int> Diff(IList<int> history)
        => history.SkipLast(1)
                  .Zip(history.Skip(1))
                  .Select(d => d.Second - d.First)
                  .ToList();
}
