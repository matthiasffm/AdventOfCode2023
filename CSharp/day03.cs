namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class Day03
{
    private HashSet<(int col, int row)> ParseData(string[] lines)
    {
        HashSet<(int, int)> symbolPos = [];

        for(int row = 0; row < lines.Length; row++)
        {
            for(int col = 0; col < lines[row].Length; col++)
            {
                if(lines[row][col] != '.' && !char.IsDigit(lines[row][col]))
                {
                    symbolPos.Add((col, row));
                }
            }
        }

        return symbolPos;
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

        var symbolPos = ParseData(lines);

        Puzzle1(lines, symbolPos).Should().Be(4361);
        Puzzle2(lines, symbolPos).Should().Be(467 * 35 + 755 * 598);
    }

    [Test]
    public void TestAocInput()
    {
        var lines     = FileUtils.ReadAllLines(this);
        var symbolPos = ParseData(lines);

        Puzzle1(lines, symbolPos).Should().Be(535235);
        Puzzle2(lines, symbolPos).Should().Be(79844424);
    }

    // An engine part seems to be missing from the engine, but nobody can figure out which one. If you can add up all the part numbers in the engine schematic, it should be
    // easy to work out which part is missing. The engine schematic (your puzzle input) consists of a visual representation of the engine. There are lots of numbers and symbols
    // you don't really understand, but apparently any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.
    //
    // Puzzle == What is the sum of all of the part numbers in the engine schematic?
    private static int Puzzle1(IEnumerable<string> lines, HashSet<(int col, int row)> symbolPos)
    {
        var validNumbers = new List<int>();

        int row = 0;
        foreach(var line in lines)
        {
            var inNumber            = false;
            var hasAdjacentSymbol   = false;
            var currentNumber       = 0 ;

            for(int col = 0; col < line.Length; col++)
            {
                if(char.IsDigit(line[col]))
                {
                    // build the numbers digit by digit and remember all the adjacent symbols in the process

                    inNumber = true;
                    hasAdjacentSymbol |= Neighbors(col, row).Any(n => symbolPos.Contains(n));

                    currentNumber = currentNumber * 10 + (line[col] - '0');
                }

                if(inNumber && (!char.IsDigit(line[col]) || col == line.Length - 1))
                {
                    // at the end of a number sum it up

                    if(hasAdjacentSymbol)
                    {
                        validNumbers.Add(currentNumber);
                    }

                    inNumber            = false;
                    hasAdjacentSymbol   = false;
                    currentNumber       = 0 ;
                }
            }

            row++;
        }

        return validNumbers.Sum();
    }

    // One of the gears in the engine is wrong. A gear is any * symbol that is adjacent to exactly two part numbers. Its gear ratio is the result of multiplying those two numbers
    // together.
    // This time, you need to find the gear ratio of every gear and add them all up so that the engineer can figure out which gear needs to be replaced.
    //
    // Puzzle == What is the sum of all of the gear ratios in your engine schematic?
    private static int Puzzle2(IEnumerable<string> lines, HashSet<(int col, int row)> symbolPos)
    {
        var symbolsWithAdjacentNumbers = new Dictionary<(int col, int row), List<int>>();

        int row = 0;
        foreach(var line in lines)
        {
            var inNumber            = false;
            var currentNumber       = 0 ;
            var adjacentSymbols     = new HashSet<(int col, int row)>();

            for(int col = 0; col < line.Length; col++)
            {
                if(char.IsDigit(line[col]))
                {
                    // build the numbers digit by digit and remember all the adjacent symbols in the process

                    inNumber = true;

                    foreach(var adjacentSymbol in Neighbors(col, row).Where(n => symbolPos.Contains(n)))
                    {
                        adjacentSymbols.Add(adjacentSymbol);
                    }

                    currentNumber = currentNumber * 10 + (line[col] - '0');
                }

                // at the end of a number save this number for every adjacent symbol in the lookup symbolsWithAdjacentNumbers

                if(inNumber && (!char.IsDigit(line[col]) || col == line.Length - 1))
                {
                    foreach(var adjacentSymbol in adjacentSymbols)
                    {
                        if(!symbolsWithAdjacentNumbers.TryGetValue(adjacentSymbol, out List<int>? numbers))
                        {
                            numbers = [];
                            symbolsWithAdjacentNumbers[adjacentSymbol] = numbers;
                        }
                        numbers.Add(currentNumber);
                    }

                    inNumber      = false;
                    currentNumber = 0 ;
                    adjacentSymbols.Clear();
                }
            }

            row++;
        }

        // in symbolsWithAdjacentNumbers are all symbols and all their adjacent number
        // if any of them has excatly two => sum their gear ratio up as asked

        return symbolsWithAdjacentNumbers.Where(s => s.Value.Count == 2)
                                         .Sum(s => s.Value.Aggregate(1, (prod, number) => prod * number));
    }

    private static IEnumerable<(int, int)> Neighbors(int col, int row)
    {
        yield return (col - 1, row - 1);
        yield return (col,     row - 1);
        yield return (col + 1, row - 1);
        yield return (col - 1, row);
        yield return (col + 1, row);
        yield return (col - 1, row + 1);
        yield return (col,     row + 1);
        yield return (col + 1, row + 1);
    }
}
