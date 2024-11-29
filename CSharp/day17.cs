namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;
using matthiasffm.Common.Algorithms;

// TODO: one order of magnitude too slow
//       performance profile needed to find optimization areas

/// <summary>
/// Theme: direct the crucible from the lava pool to the gear factory
/// </summary>
[TestFixture]
public class Day17
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "2413432311323",
            "3215453535623",
            "3255245654254",
            "3446585845452",
            "4546657867536",
            "1438598798454",
            "4457876987766",
            "3637877979653",
            "4654967986887",
            "4564679986453",
            "1224686865563",
            "2546548887735",
            "4322674655533",
        };
        var map = ParseData(data);

        Puzzle1(map, Vec2<int>.Zero, new Vec2<int>(12, 12)).Should().Be(102);
        Puzzle2(map, Vec2<int>.Zero, new Vec2<int>(12, 12)).Should().Be(94);

        data = [
            "111111111111",
            "999999999991",
            "999999999991",
            "999999999991",
            "999999999991",
        ];
        map = ParseData(data);
        Puzzle2(map, Vec2<int>.Zero, new Vec2<int>(11, 4)).Should().Be(71);
    }

    [Test]
    public void TestAocInput()
    {
        var data  = FileUtils.ReadAllLines(this);
        var map   = ParseData(data);

        Puzzle1(map, Vec2<int>.Zero, new Vec2<int>(map.GetLength(1) - 1, map.GetLength(0) - 1)).Should().Be(1256);
        Puzzle2(map, Vec2<int>.Zero, new Vec2<int>(map.GetLength(1) - 1, map.GetLength(0) - 1)).Should().Be(1382);
    }

    private static int[,] ParseData(string[] lines)
        => lines.Select(l => l.Select(c => Convert.ToInt32(c) - 48).ToArray()).ToArray().ConvertToMatrix();

    // You land near the gradually-filling pool of lava at the base of your new lavafall. Lavaducts will eventually carry the lava throughout the city, but to make
    // use of it immediately, Elves are loading it into large crucibles on wheels. The crucibles are top-heavy and pushed by hand. Unfortunately, the crucibles become
    // very difficult to steer at high speeds, and so it can be hard to go in a straight line for very long.
    // To get Desert Island the machine parts it needs as soon as possible, you'll need to find the best way to get the crucible from the lava pool to the machine parts
    // factory. To do this, you need to minimize heat loss while choosing a route that doesn't require the crucible to go in a straight line for too long. Each city block
    // is marked by a single digit that represents the amount of heat loss if the crucible enters that block. The starting point, the lava pool, is the top-left city block;
    // the destination, the machine parts factory, is the bottom-right city block. (Because you already start in the top-left block, you don't incur that block's heat loss
    // unless you leave that block and then return to it.)
    // Because it is difficult to keep the top-heavy crucible going in a straight line for very long, it can move at most three blocks in a single direction before it must
    // turn 90 degrees left or right. The crucible also can't reverse direction; after entering each city block, it may only turn left, continue straight, or turn right.
    //
    // Puzzle == Directing the crucible from the lava pool to the machine parts factory, but not moving more than three consecutive blocks in the same direction, what is the
    //           least heat loss it can incur?
    private static int Puzzle1(int[,] map, Vec2<int> lavaPool, Vec2<int> factory)
    {
        var bestPath = Search.AStar(new CruciblePos(lavaPool, 0, 0),
                                    a => a.Pos.X == factory.X && a.Pos.Y == factory.Y,
                                    a => Neighbors(map, a),
                                    (a, b) => map[b.Pos.Y, b.Pos.X],
                                    a => factory.X - a.Pos.X + factory.Y - a.Pos.Y,
                                    int.MaxValue)
                             .ToArray();

        // PrintMap(map);
        // PrintMapWithBestPath(map, bestPath);

        return bestPath.Select(a => map[a.Pos.Y, a.Pos.X]).Sum() - map[lavaPool.Y, lavaPool.X];
    }

    // possible neighbors for a normal crucible which is allowed to travel a straight line of max 3 blocks
    private static IEnumerable<CruciblePos> Neighbors(int[,] map, CruciblePos current)
    {
        if(current.NmbrStraight < 3 && IsInside(map, current.Pos, Directions[current.Dir], 1))
        {
            yield return current with { Pos = current.Pos + Directions[current.Dir], NmbrStraight = current.NmbrStraight + 1};
        }

        var leftDir = TurnLeft(current.Dir);
        if(IsInside(map, current.Pos, Directions[leftDir], 1))
        {
            yield return new CruciblePos(current.Pos + Directions[leftDir], leftDir, 1);
        }

        var rightDir = TurnRight(current.Dir);
        if(IsInside(map, current.Pos, Directions[rightDir], 1))
        {
            yield return new CruciblePos(current.Pos + Directions[rightDir], rightDir, 1);
        }
    }

    // The crucibles of lava simply aren't large enough to provide an adequate supply of lava to the machine parts factory. Instead, the Elves are going to
    // upgrade to ultra crucibles. Ultra crucibles are even more difficult to steer than normal crucibles. Not only do they have trouble going in a straight
    // line, but they also have trouble turning! Once an ultra crucible starts moving in a direction, it needs to move a minimum of four blocks in that
    // direction before it can turn (or even before it can stop at the end). However, it will eventually start to get wobbly: an ultra crucible can move a
    // maximum of ten consecutive blocks without turning.
    //
    // Puzzle == Directing the ultra crucible from the lava pool to the machine parts factory, what is the least heat loss it can incur?
    private static int Puzzle2(int[,] map, Vec2<int> lavaPool, Vec2<int> factory)
    {
        var bestPath = Search.AStar(new CruciblePos(lavaPool, 0, 0),
                                    a => a.Pos.X == factory.X && a.Pos.Y == factory.Y && a.NmbrStraight >= 4,
                                    a => NeighborsForUltraCrucible(map, a),
                                    (a, b) => map[b.Pos.Y, b.Pos.X],
                                    a => factory.X - a.Pos.X + factory.Y - a.Pos.Y,
                                    int.MaxValue)
                             .ToArray();

        // PrintMap(map);
        // PrintMapWithBestPath(map, bestPath);

        return bestPath.Select(a => map[a.Pos.Y, a.Pos.X]).Sum() - map[lavaPool.Y, lavaPool.X];
    }

    // possible neighbors for a wobbly ultra crucible which is allowed to travel a straight line of min 4 and max 10 blocks
    private static IEnumerable<CruciblePos> NeighborsForUltraCrucible(int[,] map, CruciblePos current)
    {
        if(current.NmbrStraight < 10 && IsInside(map, current.Pos, Directions[current.Dir], 1))
        {
            yield return current with { Pos = current.Pos + Directions[current.Dir], NmbrStraight = current.NmbrStraight + 1};
        }

        if(current.NmbrStraight >= 4)
        {
            var leftDir = TurnLeft(current.Dir);
            if(IsInside(map, current.Pos, Directions[leftDir], 4))
            {
                yield return new CruciblePos(current.Pos + Directions[leftDir], leftDir, 1);
            }

            var rightDir = TurnRight(current.Dir);
            if(IsInside(map, current.Pos, Directions[rightDir], 4))
            {
                yield return new CruciblePos(current.Pos + Directions[rightDir], rightDir, 1);
            }
        }
    }

    private record struct CruciblePos(Vec2<int> Pos, int Dir, int NmbrStraight);

    // movement vectors for all 4 directions
    private static readonly Vec2<int>[] Directions = [
        new( 1,  0),
        new( 0,  1),
        new(-1,  0),
        new( 0, -1)
    ];
    private static int TurnRight(int currentDir) => (currentDir + 1) % 4;
    private static int TurnLeft(int currentDir) => (currentDir + 3) % 4;

    // check if a move stays is inside the map (count is for puzzle 2 where the crucible has to move at least 4 straight before it can turn again)
    private static bool IsInside(int[,] map, Vec2<int> pos, Vec2<int> dir, int count)
    {
        var newPos = pos + count * dir;
        return newPos.X >= 0 && newPos.X < map.GetLength(1) &&
               newPos.Y >= 0 && newPos.Y < map.GetLength(0);
    }

    private static void PrintMap(int[,] map)
    {
        Console.WriteLine("\nMap:");
        for(int row = 0; row < map.GetLength(0); row++)
        {
            for(int col = 0; col < map.GetLength(1); col++)
            {
                Console.Write(map[row, col]);
            }
            Console.WriteLine();
        }
    }

    private static void PrintMapWithBestPath(int[,] map, CruciblePos[] bestPath)
    {
        const string ARROWS = ">v<^";

        Console.WriteLine("\nPath:");
        for(int row = 0; row < map.GetLength(0); row++)
        {
            for(int col = 0; col < map.GetLength(1); col++)
            {
                if(bestPath.Any(p => p.Pos.X == col && p.Pos.Y == row))
                {
                    var inPath = bestPath.FirstOrDefault(p => p.Pos.X == col && p.Pos.Y == row);
                    Console.Write(ARROWS[inPath.Dir]);
                }
                else
                {
                    Console.Write(map[row, col]);
                }
            }
            Console.WriteLine();
        }
    }
}
