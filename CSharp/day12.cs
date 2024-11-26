namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

/// <summary>
/// Theme: damaged hot springs
/// </summary>
[TestFixture]
public class Day12
{
    private record ConditionRecord(string Pattern, int[] DamagedSprings);

    private static IEnumerable<ConditionRecord> ParseData(string[] input)
        => input.Select(l => new ConditionRecord(l.Split(' ')[0],
                                                 l.Split(' ')[1].Split(',').Select(i => int.Parse(i)).ToArray()));

    [Test]
    public void TestSamples()
    {
        var input = new [] {
            "???.### 1,1,3",
            ".??..??...?##. 1,1,3",
            "?#?#?#?#?#?#?#? 1,3,1,6",
            "????.#...#... 4,1,1",
            "????.######..#####. 1,6,5",
            "?###???????? 3,2,1",
        };
        var conditionRecords = ParseData(input);

        Puzzle1(conditionRecords).Should().Be(1L +     4L + 1L +  1L +    4L +     10L);
        Puzzle2(conditionRecords).Should().Be(1L + 16384L + 1L + 16L + 2500L + 506250L);
    }

    [Test]
    public void TestAocInput()
    {
        var input = FileUtils.ReadAllLines(this);
        var conditionRecords = ParseData(input);

        Puzzle1(conditionRecords).Should().Be(7718L);

        // is correct but too slow for ci builds
        // Puzzle2(conditionRecords).Should().Be(128741994134728L);
    }

    private static long Puzzle1(IEnumerable<ConditionRecord> conditionRecords)
        => conditionRecords.Sum(cr => NmbrArrangements(string.Concat(cr.Pattern.SkipWhile(c => c == '.')), cr.DamagedSprings));

    private static long Puzzle2(IEnumerable<ConditionRecord> conditionRecords)
        => conditionRecords.Select(cr => new ConditionRecord($"{cr.Pattern}?{cr.Pattern}?{cr.Pattern}?{cr.Pattern}?{cr.Pattern}",
                                                            [.. cr.DamagedSprings, .. cr.DamagedSprings, .. cr.DamagedSprings, .. cr.DamagedSprings, .. cr.DamagedSprings]))
                        .Sum(cr => NmbrArrangements(string.Concat(cr.Pattern.SkipWhile(c => c == '.')), cr.DamagedSprings));

    private static readonly Dictionary<(string, int []), long> g_precalcCombinations = new(new CacheComparer());

    class CacheComparer : IEqualityComparer<(string, int[])>
    {
        public bool Equals((string, int[]) left, (string, int[]) right)
        {
            return left.Item1 == right.Item1 &&
                   left.Item2.Length == right.Item2.Length &&
                   Enumerable.SequenceEqual(left.Item2, right.Item2);
        }

        public int GetHashCode((string, int[]) obj) => obj.Item1.GetHashCode() ^ obj.Item2[0].GetHashCode();
    }

    private static long NmbrArrangements(string pattern, int[] damagedSprings)
    {
        if(g_precalcCombinations.TryGetValue((pattern, damagedSprings), out long combinations))
        {
            return combinations;
        }

        var damagedSpring = damagedSprings[0];
        var upperBound    = pattern.Length - Math.Max(0, damagedSprings.Skip(1).Sum() + damagedSprings.Length - 2);

        for(int i = 0; i < upperBound; i++)
        {
            if(pattern[i] == '.')
            {
                continue;
            }
            else if(i > 0 && pattern.IndexOf('#').Between(0, i - 1))
            {
                return combinations;
            }
            else
            {
                var nextGap = pattern.IndexOf('.', i) - 1;
                if(nextGap < 0)
                {
                    nextGap = pattern.Length - 1;
                }
                var end = Math.Min(i + damagedSpring - 1, nextGap);

                if(end - i + 1 >= damagedSpring && (end == pattern.Length - 1 || pattern[end + 1] != '#'))
                {
                    if(damagedSprings.Length == 1)
                    {
                        combinations += ((i + damagedSpring + 1 < pattern.Length) && pattern.IndexOf('#', i + damagedSpring + 1) > 0) ? 0L : 1L;
                    }
                    else if(i + damagedSpring + 1 < pattern.Length)
                    {
                        combinations += NmbrArrangements(pattern[(i + damagedSpring + 1)..], damagedSprings[1..]);
                    }
                }
            }
        }

        g_precalcCombinations.Add((pattern, damagedSprings), combinations);
        return combinations;
    }
}
