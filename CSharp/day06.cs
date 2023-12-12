namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: Boat races
/// </summary>
[TestFixture]
public class Day06
{
    private static IEnumerable<(long time, long distance)> ParseData(string[] lines)
    {
        var times     = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).ToArray();
        var distances = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).ToArray();

        System.Diagnostics.Debug.Assert(times.Length == distances.Length);

        return times.Select((t, i) => (long.Parse(t), long.Parse(distances[i])))
                    .ToArray();
    }

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "Time:      7  15   30",
            "Distance:  9  40  200",
        };

        var races = ParseData(lines);

        Puzzle1(races).Should().Be(4L * 8L * 9L);
        Puzzle2(races).Should().Be(71503L);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);
        var races = ParseData(lines);

        Puzzle1(races).Should().Be(281600L);
        Puzzle2(races).Should().Be(33875953L);
    }

    // As part of signing up, you get a sheet of paper (your puzzle input) that lists the time allowed for each race and also the best distance ever recorded in that race. To
    // guarantee you win the grand prize, you need to make sure you go farther in each race than the current record holder. The organizer brings you over to the area where the
    // boat races are held. Each boat is equiped with a big button on top. Holding down the button charges the boat, and releasing the button allows the boat to move. Boats move
    // faster if their button was held longer, but time spent holding the button counts against the total race time. You can only hold the button at the start of the race, and
    // boats don't move until the button is released.
    // Your toy boat has a starting speed of zero millimeters per millisecond. For each whole millisecond you spend at the beginning of the race holding down the button, the
    // boat's speed increases by one millimeter per millisecond.
    // To see how much margin of error you have, determine the number of ways you can beat the record in each race.
    //
    // Puzzle == Determine the number of ways you could beat the record in each race. What do you get if you multiply these numbers together?
    private static long Puzzle1(IEnumerable<(long time, long distance)> races)
        => races.Aggregate(1L, (prod, race) => prod * NumberOfWaysToBeatRecord(race.time, race.distance));

    // As the race is about to start, you realize the piece of paper with race times and record distances you got earlier actually just has very bad kerning. There's really only
    // one race - ignore the spaces between the numbers on each line.
    //
    // Puzzle == How many ways can you beat the record in this one much longer race?
    private static long Puzzle2(IEnumerable<(long time, long distance)> races)
        => NumberOfWaysToBeatRecord(long.Parse(string.Join("", races.Select(r => r.time))), 
                                     long.Parse(string.Join("", races.Select(r => r.distance))));

    // solve x * (time - x) > distance
    // roots of x * (time - x) - distance = 0 are
    //       r1 = 1/2 (time - sqrt(time^2 - 4 * distance))
    //       r2 = 1/2 (time + sqrt(time^2 - 4 * distance))
    private static long NumberOfWaysToBeatRecord(long time, long distance)
    {
        var sqrt = Math.Sqrt(time * time - 4 * distance);
        var root1 = (time - sqrt) / 2 + 0.0001;
        var root2 = (time + sqrt) / 2 - 0.0001;

        return Math.Abs((long)root2 - (long)root1);
    }
}
