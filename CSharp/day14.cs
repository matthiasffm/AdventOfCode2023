namespace AdventOfCode2023;

using FluentAssertions;
using matthiasffm.Common.Math;
using NUnit.Framework;

/// <summary>
/// Theme: balance reflector dish
/// </summary>
[TestFixture]
public class Day14
{
    // comparer for [,] matrix objects to use them as keys in a dictionary
    private class MatrixEqualityComparer<T>(Func<T, int> convert) : IEqualityComparer<T[,]>
    {
        private readonly Func<T, int> _convert = convert;

        public bool Equals(T[,]? left, T[,]? right)
        {
            if(ReferenceEquals(left, right))
                return true;

            if(left is null || right is null)
                return false;
            
            return left.SequenceEquals(right);
        }

        public int GetHashCode(T[,] mtx)
            => mtx.GetLength(0) ^
               mtx.GetLength(1) ^
               ((mtx.GetLength(0) > 3 && mtx.GetLength(1) > 3) ? (_convert(mtx[0, 0]) ^ _convert(mtx[0, mtx.GetLength(1) - 1]) ^ _convert(mtx[mtx.GetLength(0) - 1, 0])) : _convert(mtx[0, 0]));
    }

    private static char[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray()).ToArray().ConvertToMatrix();

    [Test]
    public void TestSamples()
    {
        var lines = new [] {
            "O....#....",
            "O.OO#....#",
            ".....##...",
            "OO.#O....O",
            ".O.....O#.",
            "O.#..O.#.#",
            "..O..#O..O",
            ".......O..",
            "#....###..",
            "#OO..#....",
        };

        var reflectorDish = ParseData(lines);

        Puzzle1(reflectorDish).Should().Be(136);
        Puzzle2(reflectorDish, 1000000000).Should().Be(64);
    }

    [Test]
    public void TestAocInput()
    {
        var lines         = FileUtils.ReadAllLines(this);
        var reflectorDish = ParseData(lines);

        Puzzle1(reflectorDish).Should().Be(106997);
        Puzzle2(reflectorDish, 1000000000).Should().Be(99641);
    }

    // Upon closer inspection, individual mirrors each appear to be connected via an elaborate system of ropes and pulleys to a large metal platform below the dish. The
    // platform is covered in large rocks of various shapes. Depending on their position, the weight of the rocks deforms the platform, and the shape of the platform
    // controls which ropes move and ultimately the focus of the dish.
    // In short: if you move the rocks, you can focus the dish. The platform even has a control panel on the side that lets you tilt it in one of four directions! The
    // rounded rocks (O) will roll when the platform is tilted, while the cube-shaped rocks (#) will stay in place. You note the positions of all of the empty spaces (.) and
    // rocks (your puzzle input).
    // You notice that the support beams along the north side of the platform are damaged; to ensure the platform doesn't collapse, you should calculate the total load
    // on the north support beams. The amount of load caused by a single rounded rock (O) is equal to the number of rows from the rock to the south edge of the platform,
    // including the row the rock is on. (Cube-shaped rocks (#) don't contribute to load.) The total load is the sum of the load caused by all of the rounded rocks.
    //
    // Puzzle == Tilt the platform so that the rounded rocks all roll north. Afterward, what is the total load on the north support beams?
    private static int Puzzle1(char[,] origDish)
    {
        var reflectorDish = (char[,])origDish.Clone();

        TiltNorth(reflectorDish);
        return CalcTotalLoad(reflectorDish);
    }

    // The parabolic reflector dish deforms, but not in a way that focuses the beam. To do that, you'll need to move the rocks to the edges of the platform. Fortunately, a
    // button on the side of the control panel labeled "spin cycle" attempts to do just that! Each cycle tilts the platform four times so that the rounded rocks roll north,
    // then west, then south, then east. After each tilt, the rounded rocks roll as far as they can before the platform tilts in the next direction. After one cycle, the
    // platform will have finished rolling the rounded rocks in those four directions in that order.
    // This process should work if you leave it running long enough, but you're still worried about the north support beams. To make sure they'll survive for a while, you
    // need to calculate the total load on the north support beams after 1000000000 cycles.
    //
    // Puzzle == Run the spin cycle for 1000000000 cycles. Afterward, what is the total load on the north support beams?
    private static int Puzzle2(char[,] origDish, int maxCycles)
    {
        var reflectorDish = (char[,])origDish.Clone();

        // find out length of period after which the cycle ends on one previously encountered

        var pastCycles = new Dictionary<char[,], int>(new MatrixEqualityComparer<char>(c => (int)c)); 

        var cycles = 0;
        var cycleStart = 0;
        var periodLength = 0;

        do
        {
            Cycle(reflectorDish);
            cycles++;

            if(pastCycles.TryGetValue(reflectorDish, out cycleStart))
            {
                periodLength = cycles - cycleStart;
                break;
            }
            else
            {
                pastCycles.Add((char[,])reflectorDish.Clone(), cycles);
            }
        }
        while(cycles < maxCycles); // no worry, periods are short in these examples so memory and runtime doesnt explode

        // do rest of cycles needed to reach maxCycles

        var rest = (maxCycles - cycleStart) % periodLength;

        for(int i = 0; i < rest; i++)
        {
            Cycle(reflectorDish);
        }

        return CalcTotalLoad(reflectorDish);
    }

    // one complete cycle
    private static void Cycle(char[,] reflectorDish)
    {
        TiltNorth(reflectorDish);
        TiltWest(reflectorDish);
        TiltSouth(reflectorDish);
        TiltEast(reflectorDish);
    }

    private static void TiltNorth(char[,] reflectorDish)
    {
        for(int c = 0; c < reflectorDish.GetLength(1); c++)
        {
            var toSwap = 0;

            for(int r = 0; r < reflectorDish.GetLength(0); r++)
            {
                if(reflectorDish[r, c] == '#')
                {
                    toSwap = r + 1;
                }
                else if(reflectorDish[r, c] == 'O')
                {
                    reflectorDish[r, c]      = '.';
                    reflectorDish[toSwap, c] = 'O';
                    toSwap++;
                }
            }
        }
    }

    private static void TiltWest(char[,] reflectorDish)
    {
        for(int r = 0; r < reflectorDish.GetLength(1); r++)
        {
            var toSwap = 0;

            for(int c = 0; c < reflectorDish.GetLength(0); c++)
            {
                if(reflectorDish[r, c] == '#')
                {
                    toSwap = c + 1;
                }
                else if(reflectorDish[r, c] == 'O')
                {
                    reflectorDish[r, c]      = '.';
                    reflectorDish[r, toSwap] = 'O';
                    toSwap++;
                }
            }
        }
    }

    private static void TiltSouth(char[,] reflectorDish)
    {
        for(int c = 0; c < reflectorDish.GetLength(1); c++)
        {
            var toSwap = reflectorDish.GetLength(0) - 1;

            for(int r = reflectorDish.GetLength(0) - 1; r >= 0; r--)
            {
                if(reflectorDish[r, c] == '#')
                {
                    toSwap = r - 1;
                }
                else if(reflectorDish[r, c] == 'O')
                {
                    reflectorDish[r, c]      = '.';
                    reflectorDish[toSwap, c] = 'O';
                    toSwap--;
                }
            }
        }
    }

    private static void TiltEast(char[,] reflectorDish)
    {
        for(int r = 0; r < reflectorDish.GetLength(1); r++)
        {
            var toSwap = reflectorDish.GetLength(0) - 1;

            for(int c = reflectorDish.GetLength(0) - 1; c >= 0; c--)
            {
                if(reflectorDish[r, c] == '#')
                {
                    toSwap = c - 1;
                }
                else if(reflectorDish[r, c] == 'O')
                {
                    reflectorDish[r, c]      = '.';
                    reflectorDish[r, toSwap] = 'O';
                    toSwap--;
                }
            }
        }
    }

    private static int CalcTotalLoad(char[,] reflectorDish)
    {
        var rows = reflectorDish.GetLength(0);
        var load = 0;

        for(int c = 0; c < reflectorDish.GetLength(1); c++)
        {
            foreach(var point in reflectorDish.Col(c))
            {
                if(point.Item2 == 'O')
                {
                    load += rows - point.Item1;
                }
            }
        }

        return load;
    }
}
