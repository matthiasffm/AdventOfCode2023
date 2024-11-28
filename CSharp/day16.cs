namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

// TODO: a bit slow
//       right now every position the beam touches is tested separately
//       this could be faster by _only_ testing mirror positions and border and
//       moving a beam from mirror position to border/mirror position in a straight line
//       all these intermediate steps for testing empty space could be eliminated that way
//       (only the visited set would need to be updated for every step of a line from border/mirror to mirror/border)
//       additionally puzzle2 could easily be parallelized for every starting beam

/// <summary>
/// Theme: following mirrored beams
/// </summary>
[TestFixture]
public class Day16
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            @".|...\....",
            @"|.-.\.....",
            @".....|-...",
            @"........|.",
            @"..........",
            @".........\",
            @"..../.\\..",
            @".-.-/..|..",
            @".|....-|.\",
            @"..//.|....",
        };
        var contraption = ParseData(data);

        Puzzle1(contraption).Should().Be(46);
        Puzzle2(contraption).Should().Be(51);
    }

    [Test]
    public void TestAocInput()
    {
        var data        = FileUtils.ReadAllLines(this);
        var contraption = ParseData(data);

        Puzzle1(contraption).Should().Be(7472);
        Puzzle2(contraption).Should().Be(7716);
    }

    private static char[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray()).ToArray().ConvertToMatrix();

    private record Beam(Vec2<int> Pos, Vec2<int> Dir);

    // Inside the cave you discover a contraption. It appears to be a flat, two-dimensional square grid containing empty space (.), mirrors (/ and \), and
    // splitters (| and -). The light isn't energizing enough tiles to produce lava; to debug the contraption, you need to start by analyzing the current situation. 
    // The contraption is aligned so that most of the beam bounces around the grid, but each tile on the grid converts some of the beam's light into heat to
    // melt the rock in the cavern.
    // The beam enters in the top-left corner from the left and heading to the right. Then, its behavior depends on what it encounters as it moves:
    // - If the beam encounters empty space (.), it continues in the same direction.
    // - If the beam encounters a mirror (/ or \), the beam is reflected 90 degrees depending on the angle of the mirror.
    // - If the beam encounters the pointy end of a splitter (| or -), the beam passes through the splitter as if the splitter were empty space.
    // - If the beam encounters the flat side of a splitter (| or -), the beam is split into two beams going in each of the two directions the splitter's pointy
    //   ends are pointing.
    // Beams do not interact with other beams; a tile can have many beams passing through it at the same time. A tile is energized if that tile has at least
    // one beam pass through it, reflect in it, or split in it.
    //
    // Puzzle == With the beam starting in the top-left heading right, how many tiles end up being energized?
    private static int Puzzle1(char[,] contraption)
        => CountEnergizedTiles(contraption, new Beam(new Vec2<int>(-1, 0), new Vec2<int>(1, 0)));

    // As you try to work out what might be wrong, the reindeer tugs on your shirt and leads you to a nearby control panel. There, a collection of buttons lets you align
    // the contraption so that the beam enters from any edge tile and heading away from that edge. So, the beam could start on any tile in the top row (heading downward),
    // any tile in the bottom row (heading upward), any tile in the leftmost column (heading right), or any tile in the rightmost column (heading left).
    // To produce lava, you need to find the configuration that energizes as many tiles as possible.
    //
    // Puzzle == Find the initial beam configuration that energizes the largest number of tiles; how many tiles are energized in that configuration?
    private static int Puzzle2(char[,] contraption)
        => Enumerable.Range(0, contraption.GetLength(0))
                     .SelectMany<int, Beam>(i => [new Beam(new Vec2<int>(i, -1), new Vec2<int>(0, 1)),
                                                  new Beam(new Vec2<int>(i, contraption.GetLength(0)), new Vec2<int>(0, -1)),
                                                  new Beam(new Vec2<int>(-1, i), new Vec2<int>(1, 0)),
                                                  new Beam(new Vec2<int>(contraption.GetLength(0), i), new Vec2<int>(-1, 0))])
                     .Select(beam => CountEnergizedTiles(contraption, beam))
                     .Max();

    private static int CountEnergizedTiles(char[,] contraption, Beam initialBeam)
    {
        var beams = new Stack<Beam>();
        beams.Push(new(initialBeam.Pos, initialBeam.Dir));

        var visited = new HashSet<Beam>();

        while(beams.TryPop(out var nextBeam))
        {
            // detect cycle
            if(visited.Contains(nextBeam))
            {
                continue;
            }

            visited.Add(nextBeam);

            var nextPos = nextBeam.Pos + nextBeam.Dir;

            // outside contraption?
            if(nextPos.X < 0 || nextPos.X >= contraption.GetLength(0) || nextPos.Y < 0 || nextPos.Y >= contraption.GetLength(1))
            {
                continue;
            }

            // move beam
            switch(contraption[nextPos.Y, nextPos.X])
            {
                case '.':
                    beams.Push(nextBeam with { Pos = nextPos });
                    break;

                case '/':
                    beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>(-nextBeam.Dir.Y, -nextBeam.Dir.X) });
                    break;

                case '\\':
                    beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>(nextBeam.Dir.Y, nextBeam.Dir.X) });
                    break;

                case '-':
                    if(nextBeam.Dir.Y == 0)
                    {
                        beams.Push(nextBeam with { Pos = nextPos });
                    }
                    else
                    {
                        beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>(-1, 0) });
                        beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>( 1, 0) });
                    }
                    break;

                case '|':
                    if(nextBeam.Dir.X == 0)
                    {
                        beams.Push(nextBeam with { Pos = nextPos });
                    }
                    else
                    {
                        beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>(0, -1) });
                        beams.Push(nextBeam with { Pos = nextPos, Dir = new Vec2<int>(0,  1) });
                    }
                    break;
            }
        }

        // energized == visited by beam - starting pos outside the contraption
        return visited.Select(v => v.Pos)
                      .Distinct()
                      .Count() - 1;
    }
}
