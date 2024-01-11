namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: Find gears for the gondola
/// </summary>
[TestFixture]
public class Day03
{
    private (HashSet<(int col, int row)> symbols, HashSet<(int col, int row)> gears) ParseData(string[] lines)
    {
        HashSet<(int, int)> symbols = [];
        HashSet<(int, int)> gears = [];

        for(int row = 0; row < lines.Length; row++)
        {
            for(int col = 0; col < lines[row].Length; col++)
            {
                if(lines[row][col] != '.' && !char.IsDigit(lines[row][col]))
                {
                    symbols.Add((col, row));
                }
                if(lines[row][col] == '*')
                {
                    gears.Add((col, row));
                }
            }
        }

        return (symbols, gears);
    }

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "467..114..",
            "...*......",
            "..35..633.",
            "......#...",
            "617*......",
            ".....+.58.",
            "..592.....",
            "......755.",
            "...$.*....",
            ".664.598..",
        };

        var (symbols, gears) = ParseData(lines);

        Puzzle1(lines, symbols).Should().Be(4361);
        Puzzle2(lines, gears).Should().Be(467 * 35 + 755 * 598);
    }

    [Test]
    public void TestAocInput()
    {
        var lines            = FileUtils.ReadAllLines(this);
        var (symbols, gears) = ParseData(lines);

        Puzzle1(lines, symbols).Should().Be(535235);
        Puzzle2(lines, gears).Should().Be(79844424);
    }

    // An engine part seems to be missing from the engine, but nobody can figure out which one. If you can add up all the part numbers in the engine schematic, it should be
    // easy to work out which part is missing. The engine schematic (your puzzle input) consists of a visual representation of the engine. There are lots of numbers and symbols
    // you don't really understand, but apparently any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.
    //
    // Puzzle == What is the sum of all of the part numbers in the engine schematic?
    private static int Puzzle1(IEnumerable<string> lines, HashSet<(int col, int row)> symbols)
    {
        var sumValidNumbers = 0;

        int row = 0;
        foreach(var line in lines)
        {
            var hasAdjacentSymbol   = false;
            var currentNumber       = 0 ;

            for(int col = 0; col < line.Length; col++)
            {
                if(char.IsDigit(line[col]))
                {
                    // build the numbers digit by digit and remember all the adjacent symbols in the process

                    hasAdjacentSymbol |= TwoD.Neighbors8.Select(n => (col + n.col, row + n.row)).Intersect(symbols).Any();
                    currentNumber = currentNumber * 10 + (line[col] - '0');
                }

                if((currentNumber > 0) && (!char.IsDigit(line[col]) || col == line.Length - 1))
                {
                    // at the end of a number sum it up

                    if(hasAdjacentSymbol)
                    {
                        sumValidNumbers += currentNumber;
                    }

                    hasAdjacentSymbol   = false;
                    currentNumber       = 0 ;
                }
            }

            row++;
        }

        return sumValidNumbers;
    }

    // One of the gears in the engine is wrong. A gear is any * symbol that is adjacent to exactly two part numbers. Its gear ratio is the result of multiplying those two numbers
    // together.
    // This time, you need to find the gear ratio of every gear and add them all up so that the engineer can figure out which gear needs to be replaced.
    //
    // Puzzle == What is the sum of all of the gear ratios in your engine schematic?
    private static int Puzzle2(IEnumerable<string> lines, HashSet<(int col, int row)> gears)
    {
        var gearsWithAdjacentNumbers = new Dictionary<(int col, int row), List<int>>();

        int row = 0;
        foreach(var line in lines)
        {
            var currentNumber = 0 ;
            var adjacentGears = new HashSet<(int col, int row)>();

            for(int col = 0; col < line.Length; col++)
            {
                if(char.IsDigit(line[col]))
                {
                    // build the numbers digit by digit and remember all the adjacent gears in the process

                    foreach(var adjacentGear in TwoD.Neighbors8.Select(n => (col + n.col, row + n.row)).Intersect(gears)!)
                    {
                        adjacentGears.Add(adjacentGear);
                    }

                    currentNumber = currentNumber * 10 + (line[col] - '0');
                }

                // at the end of a number save this number for every adjacent gear in the lookup gearsWithAdjacentNumbers

                if((currentNumber > 0) && (!char.IsDigit(line[col]) || col == line.Length - 1))
                {
                    foreach(var adjacentGear in adjacentGears)
                    {
                        if(!gearsWithAdjacentNumbers.TryGetValue(adjacentGear, out List<int>? numbers))
                        {
                            numbers = [];
                            gearsWithAdjacentNumbers[adjacentGear] = numbers;
                        }
                        numbers.Add(currentNumber);
                    }

                    currentNumber = 0 ;
                    adjacentGears.Clear();
                }
            }

            row++;
        }

        // in gearsWithAdjacentNumbers are all gears and all their adjacent numbers
        // if any of them has excatly two => sum their gear ratio up as asked

        return gearsWithAdjacentNumbers.Where(s => s.Value.Count == 2)
                                       .Sum(s => s.Value.Aggregate(1, (prod, number) => prod * number));
    }
}
