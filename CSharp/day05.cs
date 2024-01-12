namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

/// <summary>
/// Theme: help the gardener with the seeds
/// </summary>
[TestFixture]
public class Day05
{
    record Mapping(long Start, long End, long Offset)
    {
        public long     MapForward(long pos)    => pos + Offset;
        public Mapping  MapForward()            => new(Start + Offset, End + Offset, Offset);

        public long     MapBackward(long pos)   => pos - Offset;
        public Mapping  MapBackward()           => new(Start - Offset, End - Offset, Offset);

        public bool     Contains(long pos)      => Start <= pos && pos <= End;
    }

    record Category(string Source, string Dest, IEnumerable<Mapping> Mappings);

    private static (IEnumerable<long> seeds, IEnumerable<Category> almanac) ParseData(string[] lines)
    {
        var seeds = lines[0].Split(':')[1]
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => long.Parse(s))
                            .ToArray();

        int l = 2;

        var categories = new List<Category>();

        do
        {
            var dir     = lines[l].Split(' ')[0].Split('-');
            string src  = dir[0];
            string dest = dir[2];
            var    maps = new List<Mapping>();

            l++;

            do
            {
                var mapping = lines[l++].Split(' ');
                var destStart = long.Parse(mapping[0]);
                var srcStart  = long.Parse(mapping[1]);
                var length    = long.Parse(mapping[2]);
                maps.Add(new Mapping(srcStart, srcStart + length - 1, destStart - srcStart));
            }
            while(l < lines.Length && !string.IsNullOrWhiteSpace(lines[l]));

            l++;

            maps = Fill(maps.OrderBy(m => m.Start), 0L, long.MaxValue - 1000L);

            categories.Add(new Category(src, dest, maps));
        }
        while(l < lines.Length);

        return (seeds, categories);
    }

    // adds zero-offset mappings to completely fill the range of the mapping from rangeStart to rangeEnd
    private static List<Mapping> Fill(IEnumerable<Mapping> maps, long rangeStart, long rangeEnd)
    {
        List<Mapping> filledMaps = [];

        var i = rangeStart;
        foreach(var map in maps)
        {
            if(map.Start > i)
            {
                filledMaps.Add(new Mapping(i, map.Start - 1, 0));
            }

            filledMaps.Add(map);
            i = map.End + 1;
        }

        filledMaps.Add(new Mapping(i, rangeEnd, 0));

        return filledMaps;
    }

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "seeds: 79 14 55 13",
            "",
            "seed-to-soil map:",
            "50 98 2",
            "52 50 48",
            "",
            "soil-to-fertilizer map:",
            "0 15 37",
            "37 52 2",
            "39 0 15",
            "",
            "fertilizer-to-water map:",
            "49 53 8",
            "0 11 42",
            "42 0 7",
            "57 7 4",
            "",
            "water-to-light map:",
            "88 18 7",
            "18 25 70",
            "",
            "light-to-temperature map:",
            "45 77 23",
            "81 45 19",
            "68 64 13",
            "",
            "temperature-to-humidity map:",
            "0 69 1",
            "1 0 69",
            "",
            "humidity-to-location map:",
            "60 56 37",
            "56 93 4",
        };

        var (seeds, almanac) = ParseData(lines);

        var mergedMappings = MergeMappings(almanac);

        Puzzle1(seeds, mergedMappings).Should().Be(35L);
        Puzzle2(seeds, mergedMappings).Should().Be(46L);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);
        var (seeds, almanac) = ParseData(lines);

        var mergedMappings = MergeMappings(almanac);

        Puzzle1(seeds, mergedMappings).Should().Be(836040384L);
        Puzzle2(seeds, mergedMappings).Should().Be(10834440L);
    }

    // The almanac contains a list of maps which describe how to convert numbers from a source category into numbers in a destination category. That is, the section
    // that starts with seed-to-soil map: describes how to convert a seed number (the source) to a soil number (the destination). This lets the gardener and his team
    // know which soil to use with which seeds, which water to use with which fertilizer, and so on. Rather than list every source number and its corresponding destination
    // number one by one, the maps describe entire ranges of numbers that can be converted. Each line within a map contains three numbers: the destination range start, the
    // source range start, and the range length.
    // Any source numbers that aren't mapped correspond to the same destination number.
    // The gardener and his team want to get started as soon as possible, so they'd like to know the closest location that needs a seed. Using these maps, find the lowest
    // location number that corresponds to any of the initial seeds. To do this, you'll need to convert each seed number through other categories until you can find its
    // corresponding location number.
    //
    // Puzzle == What is the lowest location number that corresponds to any of the initial seed numbers?
    private static long Puzzle1(IEnumerable<long> seeds, IEnumerable<Mapping> mappings) =>
        seeds.Min(seed => mappings.Where(m => m.Contains(seed))
                                  .Select(m => m.MapForward(seed))
                                  .FirstOrDefault(seed));

    // Everyone will starve if you only plant such a small number of seeds. Re-reading the almanac, it looks like the seeds: line actually describes ranges of seed
    // numbers. The values on the initial seeds: line come in pairs. Within each pair, the first value is the start of the range and the second value is the length
    // of the range.
    // Puzzle == Consider all of the initial seed numbers listed in the ranges on the first line of the almanac. What is the lowest location number that corresponds
    //           to any of the initial seed numbers?
    private static long Puzzle2(IEnumerable<long> seedDefinitions, IEnumerable<Mapping> mappings)
    {
        // order mappings by lowest projection and then search the seeds definitions/buckets if they fit in
        // first seed bucket that fits in the first found mapping has to be the lowest projected value

        foreach(var mappingsByMinProjection in mappings.OrderBy(m => m.MapForward().Start))
        {
            foreach(var seedDefinition in seedDefinitions.Where((s, i) => i % 2 == 0)
                                                         .Zip(seedDefinitions.Where((s, i) => i % 2 == 1), Tuple.Create))
            {
                long from = seedDefinition.Item1;
                long to   = seedDefinition.Item1 + seedDefinition.Item2;

                if(mappingsByMinProjection.Start.Between(from, to))
                {
                    return mappingsByMinProjection.MapForward().Start;
                }
                else if(mappingsByMinProjection.End.Between(from, to))
                {
                    return mappingsByMinProjection.MapForward(from);
                }
            }
        }

        throw new InvalidProgramException();
    }

    private static IEnumerable<Mapping> MergeMappings(IEnumerable<Category> categories) =>
        categories.Skip(1)
                  .Aggregate(categories.First().Mappings,
                             (mergedMappings, nextCategory) => MergeMapping(mergedMappings, nextCategory.Mappings));

    // merges two mappings
    // precondition: both mappings are sorted by Start
    // postcondition: returns merged mapping sorted by Start
    private static List<Mapping> MergeMapping(IEnumerable<Mapping> current, IEnumerable<Mapping> next)
    {
        var maxEnd = current.Max(m => m.End);

        // project current mapping forward so we get all projected points of the current mapping

        var currentProjected = current.Select(m => m.MapForward())
                                      .OrderBy(m => m.Start)
                                      .ToArray();

        // merge all forward projected points from current mapping with all src points in the next mapping

        var mergedPoints = currentProjected.Select(m => m.Start)
                                           .Union(currentProjected.Select(m => m.End))
                                           .Union(next.Select(m => m.Start))
                                           .Union(next.Select(m => m.End))
                                           .Distinct()
                                           .Order();

        // project all these merged points back to current source with the matching mapping

        List<(long Start, long Offset)> backprojectedMergedPoints = [];

        foreach(var mergedPoint in mergedPoints)
        {
            var currentMappingForPoint = current.First(m => m.MapForward().Contains(mergedPoint));
            var nextMappingForPoint    = next.First(m => m.Contains(mergedPoint));

            // reproject back to current src with offset = current + next

            backprojectedMergedPoints.Add((currentMappingForPoint.MapBackward(mergedPoint), currentMappingForPoint.Offset + nextMappingForPoint.Offset));
        }

        // create result mapping from back projected merge points and merge adjacent mappings with _same_ offset in the process

        backprojectedMergedPoints = [.. backprojectedMergedPoints.OrderBy(p => p.Start)];

        List<Mapping> mergedMapping = [ new Mapping(backprojectedMergedPoints[0].Start, backprojectedMergedPoints[1].Start - 1, backprojectedMergedPoints[0].Offset)];
        for(int i = 1; i < backprojectedMergedPoints.Count; i++)
        {
            var nextEnd = i == backprojectedMergedPoints.Count - 1 ? maxEnd : backprojectedMergedPoints[i + 1].Start - 1;

            if(mergedMapping[^1].Offset == backprojectedMergedPoints[i].Offset)
            {
                mergedMapping[^1] = mergedMapping[^1] with { End = nextEnd };
            }
            else
            {
                mergedMapping.Add(new Mapping(backprojectedMergedPoints[i].Start, nextEnd, backprojectedMergedPoints[i].Offset));
            }
        }

        return mergedMapping;
    }
}
