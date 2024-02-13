namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;
using System;

using matthiasffm.Common.Collections;

/// <summary>
/// Theme: Cosmic Expansion
/// </summary>
[TestFixture]
public class Day11
{
    private record struct Coord(long Row, long Col);

    private static HashSet<Coord> ParseData(IEnumerable<string> lines)
    {
        return lines.SelectMany((l, row) => l.Select((c, col) => (row, col, c))
                                             .Where(tpl => tpl.c == '#')
                                             .Select(tpl => new Coord(tpl.row, tpl.col)))
                    .ToHashSet();
    }

    [Test]
    public void TestSamples()
    {
        var map = new [] {
            "...#......",
            ".......#..",
            "#.........",
            "..........",
            "......#...",
            ".#........",
            ".........#",
            "..........",
            ".......#..",
            "#...#.....",
        };
        var galaxies = ParseData(map);

        Puzzle(galaxies,   2L).Should().Be(374L);
        Puzzle(galaxies,  10L).Should().Be(1030L);
        Puzzle(galaxies, 100L).Should().Be(8410L);
    }

    [Test]
    public void TestAocInput()
    {
        var map = FileUtils.ReadAllLines(this);
        var galaxies = ParseData(map);

        Puzzle(galaxies,       2L).Should().Be(9965032L);
        Puzzle(galaxies, 1000000L).Should().Be(550358864332L);
    }

    // The cosmic researcher has collected a bunch of data and compiled the data into a single giant image (your puzzle input). The image includes empty
    // space (.) and galaxies (#). The researcher is trying to figure out the sum of the lengths of the shortest path between every pair of galaxies.
    // However, there's a catch: the universe expanded in the time it took the light from those galaxies to reach the observatory. Due to something involving
    // gravitational effects, only some space expands. In fact, the result is that any rows or columns that contain no galaxies should all actually be twice
    // as big or more. Equipped with this expanded universe, the shortest path between every pair of galaxies can be found. For each pair, find any shortest
    // path between the two galaxies using only steps that move up, down, left, or right exactly one . or # at a time.
    //
    // Puzzle == Expand the universe, then find the length of the shortest path between every pair of galaxies. What is the sum of these lengths?
    private static long Puzzle(IEnumerable<Coord> galaxies, long expansion)
    {
        // expand galaxies

        var missingRows = Enumerable.Range(0, (int)galaxies.Max(g => g.Row))
                                    .Except(galaxies.Select(g => (int)g.Row))
                                    .ToArray();

        var missingCols = Enumerable.Range(0, (int)galaxies.Max(g => g.Col))
                                    .Except(galaxies.Select(g => (int)g.Col))
                                    .ToArray();

        var expandedUniverse = galaxies.Select(g => new Coord(g.Row + missingRows.Count(r => r < g.Row) * (expansion - 1),
                                                              g.Col + missingCols.Count(c => c < g.Col) * (expansion - 1)));

        // sum manhattan distances of all variations (includes a-b, b-a so divide by 2 in the end)

        return expandedUniverse.Variations()
                               .Select(tpl => Manhattan(tpl.Item1, tpl.Item2))
                               .Sum() / 2;
    }

    private static long Manhattan(Coord g1, Coord g2)
        => Math.Abs(g1.Row - g2.Row) + Math.Abs(g1.Col - g2.Col);
}
