namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: Find perfect reflections
/// </summary>
[TestFixture]
public class Day13
{
    // encode every column and row of the mirror pattern as a bit array in a uint value (max row or column size is 15 so this works)
    private record Pattern(uint[] Rows, uint[] Columns);

    private static IEnumerable<Pattern> ParseData(string[] lines)
        => string.Join(',', lines)
                 .Split(",,")
                 .Select(p => CreatePattern(p.Split(",")));

    private static Pattern CreatePattern(string[] patternLines)
        => new(patternLines.Select(l => ToUint(l)).ToArray(),
               Enumerable.Range(0, patternLines[0].Length)
                         .Select(c => ToUint(patternLines.Select(l => l[c]))).ToArray());

    private static uint ToUint(IEnumerable<char> chars) => (uint)chars.Select((c, i) => c == '#' ? 1 << i : 0).Sum();

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "#.##..##.",
            "..#.##.#.",
            "##......#",
            "##......#",
            "..#.##.#.",
            "..##..##.",
            "#.#.##.#.",
            "",
            "#...##..#",
            "#....#..#",
            "..##..###",
            "#####.##.",
            "#####.##.",
            "..##..###",
            "#....#..#",
        };

        var patterns = ParseData(lines);

        Puzzle1(patterns).Should().Be(5 + 400);
        Puzzle2(patterns).Should().Be(100 + 300);
    }

    [Test]
    public void TestAocInput()
    {
        var lines  = FileUtils.ReadAllLines(this);

        var patterns = ParseData(lines);

        Puzzle1(patterns).Should().Be(36015);
        Puzzle2(patterns).Should().Be(35335);
    }

    // As you move through the valley of mirrors, you find that several of them have fallen from the large metal frames keeping them in place. The mirrors are extremely flat and
    // shiny, and many of the fallen mirrors have lodged into the ash at strange angles. Because the terrain is all one color, it's hard to tell where it's safe to walk or where
    // you're about to run into a mirror.
    // You note down the patterns of ash (.) and rocks (#) that you see as you walk (your puzzle input); perhaps by carefully analyzing these patterns, you can figure out where
    // the mirrors are! To find the reflection in each pattern, you need to find a perfect reflection across either a horizontal line between two rows or across a vertical line
    // between two columns.
    // To summarize your pattern notes, add up the number of columns to the left of each vertical line of reflection; to that, also add 100 multiplied by the number of rows above
    // each horizontal line of reflection.
    //
    // Puzzle == Find the line of reflection in each of the patterns in your notes. What number do you get after summarizing all of your notes?
    private static int Puzzle1(IEnumerable<Pattern> patterns)
        => patterns.Sum(p => ColumnsLeftOfVerticalReflection(p, (left, right) => left.SequenceEqual(right)) +
                             RowsAboveHorizontalReflection(p, (left, right) => left.SequenceEqual(right)) * 100);

    // Upon closer inspection, you discover that every mirror has exactly one smudge: exactly one . or # should be the opposite type.
    // In each pattern, you'll need to locate and fix the smudge that causes a different reflection line to be valid. (The old reflection line won't necessarily continue being valid
    // after the smudge is fixed.)
    // Summarize your notes as before, but instead use the new different reflection lines.
    //
    // Puzzle == In each pattern, fix the smudge and find the different line of reflection. What number do you get after summarizing the new reflection line in each pattern in your notes?
    private static int Puzzle2(IEnumerable<Pattern> patterns)
        => patterns.Sum(p => ColumnsLeftOfVerticalReflection(p, (left, right) => ExactlyOneBitDifference(left, right)) +
                             RowsAboveHorizontalReflection(p, (left, right) => ExactlyOneBitDifference(left, right)) * 100);

    // find vertical reflection line between columns, starts with biggest possible reflection size
    private static int ColumnsLeftOfVerticalReflection(Pattern pattern, Func<IEnumerable<uint>, IEnumerable<uint>, bool> compare)
    {
        for(int c = pattern.Columns.Length / 2; c > 0; c--)
        {
            if(compare(pattern.Columns.Take(c), pattern.Columns.Skip(c).Take(c).Reverse()))
            {
                return c;
            }

            if(compare(pattern.Columns.Reverse().Take(c), pattern.Columns.Reverse().Skip(c).Take(c).Reverse()))
            {
                return pattern.Columns.Length - c;
            }
        }

        return 0;
    }

    // find horizontal reflection line between rows, starts with biggest possible reflection size
    private static int RowsAboveHorizontalReflection(Pattern pattern, Func<IEnumerable<uint>, IEnumerable<uint>, bool> compare)
    {
        for(int r = pattern.Rows.Length / 2; r > 0; r--)
        {
            if(compare(pattern.Rows.Take(r), pattern.Rows.Skip(r).Take(r).Reverse()))
            {
                return r;
            }

            if(compare(pattern.Rows.Reverse().Take(r), pattern.Rows.Reverse().Skip(r).Take(r).Reverse()))
            {
                return pattern.Rows.Length - r;
            }
        }

        return 0;
    }

    // rows and cols are encoded as bits in an uint value, just a xor is enough to find the reflection with 1 (one) bit smudged
    private static bool ExactlyOneBitDifference(IEnumerable<uint> left, IEnumerable<uint> right)
        => left.Zip(right).Sum(tuple => CountSetBits(tuple.First ^ tuple.Second)) == 1;

    private static int CountSetBits(uint bits)
    {
        // System.Runtime.Intrinsics.X86.Popcnt.PopCount(bits); also works for Intel and AMD cpus

        if(bits == 0)
        {
            return 0;
        }

        bits &= bits - 1;

        // we only care about 1 (one) set bit
        // if its more its already to many
        return bits == 0 ? 1 : 2;
    }
}
