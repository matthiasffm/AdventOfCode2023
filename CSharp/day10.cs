namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Immutable;

/// <summary>
/// Theme: find the animal in a pipe maze
/// </summary>
[TestFixture]
public class Day10
{
    internal record struct Point(int Row, int Col);

    private static (Point start, Dictionary<Point, HashSet<Point>> pipes) ParseData(string[] lines)
    {
        Dictionary<Point, HashSet<Point>> pipes = [];

        // build maze lookup

        Dictionary<Point, char> maze = [];
        for(int row = 0; row < lines.Length; row++)
        {
            for(int col = 0; col < lines[row].Length; col++)
            {
                if(lines[row][col] != '.')
                {
                    maze.Add(new Point(row, col), lines[row][col]);
                }
            }
        }

        // find pipe connections

        foreach(var tile in maze.Where(m => m.Value != 'S'))
        {
            if(tile.Value == '|')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row - 1 }, ['|', '7', 'F']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row + 1 }, ['|', 'L', 'J']);
            }
            else if(tile.Value == '-')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col - 1 }, ['-', 'L', 'F']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col + 1 }, ['-', '7', 'J']);
            }
            else if(tile.Value == 'L')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row - 1 }, ['|', '7', 'F']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col + 1 }, ['-', '7', 'J']);
            }
            else if(tile.Value == 'J')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row - 1 }, ['|', '7', 'F']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col - 1 }, ['-', 'L', 'F']);
            }
            else if(tile.Value == '7')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row + 1 }, ['|', 'L', 'J']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col - 1 }, ['-', 'L', 'F']);
            }
            else if(tile.Value == 'F')
            {
                TryConnect(pipes, maze, tile.Key, tile.Key with { Row = tile.Key.Row + 1 }, ['|', 'L', 'J']);
                TryConnect(pipes, maze, tile.Key, tile.Key with { Col = tile.Key.Col + 1 }, ['-', '7', 'J']);
            }
        }

        var start = maze.Single(m => m.Value == 'S').Key;

        return (start, pipes);
    }

    private static void TryConnect(Dictionary<Point, HashSet<Point>> pipes,
                                   Dictionary<Point, char> maze,
                                   Point src,
                                   Point dest,
                                   char[] validConnections)
    {
        if(maze.TryGetValue(dest, out var tile))
        {
            if(tile == 'S' || validConnections.Contains(tile))
            {
                Connect(pipes, src, dest);
                Connect(pipes, dest, src);
            }
        }
    }

    private static void Connect(Dictionary<Point, HashSet<Point>> pipes, Point src, Point dest)
    {
        if(!pipes.TryGetValue(src, out var connections))
        {
            connections = [];
            pipes.Add(src, connections);
        }
        connections.Add(dest);
    }

    [Test]
    public void TestSamplesPuzzle1()
    {
        string[] lines = [
            ".....",
            ".S-7.",
            ".|.|.",
            ".L-J.",
            ".....",
        ];

        var (start, pipes) = ParseData(lines);

        var longestLoop = Puzzle1(start, pipes);
        (longestLoop.Length / 2).Should().Be(4);

        lines = [
            "..F7.",
            ".FJ|.",
            "SJ.L7",
            "|F--J",
            "LJ...",
        ];

        (start, pipes) = ParseData(lines);

        longestLoop = Puzzle1(start, pipes);
        (longestLoop.Length / 2).Should().Be(8);
    }

    [Test]
    public void TestSamplesPuzzle2()
    {
        string[] lines = [
            ".....",
            ".S-7.",
            ".|.|.",
            ".L-J.",
            ".....",
        ];

        var (start, pipes) = ParseData(lines);
        var longestLoop = Puzzle1(start, pipes);
        Puzzle2(longestLoop).Should().Be(1);

        lines = [
            "...........",
            ".S-------7.",
            ".|F-----7|.",
            ".||.....||.",
            ".||.....||.",
            ".|L-7.F-J|.",
            ".|..|.|..|.",
            ".L--J.L--J.",
            "...........",
        ];

        (start, pipes) = ParseData(lines);
        longestLoop = Puzzle1(start, pipes);
        Puzzle2(longestLoop).Should().Be(4);

        lines = [
            ".F----7F7F7F7F-7....",
            ".|F--7||||||||FJ....",
            ".||.FJ||||||||L7....",
            "FJL7L7LJLJ||LJ.L-7..",
            "L--J.L7...LJS7F-7L7.",
            "....F-J..F7FJ|L7L7L7",
            "....L7.F7||L7|.L7L7|",
            ".....|FJLJ|FJ|F7|.LJ",
            "....FJL-7.||.||||...",
            "....L---J.LJ.LJLJ...",
        ];

        (start, pipes) = ParseData(lines);
        longestLoop = Puzzle1(start, pipes);
        Puzzle2(longestLoop).Should().Be(8);

        lines = [
            "FF7FSF7F7F7F7F7F---7",
            "L|LJ||||||||||||F--J",
            "FL-7LJLJ||||||LJL-77",
            "F--JF--7||LJLJ7F7FJ-",
            "L---JF-JLJ.||-FJLJJ7",
            "|F|F-JF---7F7-L7L|7|",
            "|FFJF7L7F-JF7|JL---7",
            "7-L-JL7||F7|L7F-7F7|",
            "L.L7LFJ|||||FJL7||LJ",
            "L7JLJL-JLJLJL--JLJ.L",
        ];

        (start, pipes) = ParseData(lines);
        longestLoop = Puzzle1(start, pipes);
        Puzzle2(longestLoop).Should().Be(10);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);

        var (start, pipes) = ParseData(lines);

        var longestLoop = Puzzle1(start, pipes);
        (longestLoop.Length / 2).Should().Be(6786);
        Puzzle2(longestLoop).Should().Be(495);
    }

    // As you stop to admire some metal grass, you notice something metallic scurry away in your peripheral vision and jump into a big pipe! It didn't look like any animal you've
    // ever seen; if you want a better look, you'll need to get ahead of it.
    // Scanning the area, you discover that the entire field you're standing on is densely packed with pipes. You make a quick sketch of all of the surface pipes you can see.
    // The pipes are arranged in a two-dimensional grid of tiles: |, -, L, J, 7, F. S is the starting position of the animal and . is the ground. There is a pipe on the starting
    // position tile, but your sketch doesn't show what shape that pipe has.
    // Based on the acoustics of the animal's scurrying, you're confident the pipe that contains the animal is one large, continuous loop. Unfortunately, there are also many
    // pipes that aren't connected to the loop!
    // If you want to get out ahead of the animal, you should find the tile in the loop that is farthest from the starting position. Because the animal is in the pipe, it doesn't
    // make sense to measure this by direct distance. Instead, you need to find the tile that would take the longest number of steps along the loop to reach from the starting
    // point - regardless of which way around the loop the animal went.
    //
    // Puzzle == Find the single giant loop starting at S. How many steps along the loop does it take to get from the starting position to the point farthest from the starting position?
    private static Point[] Puzzle1(Point start, Dictionary<Point, HashSet<Point>> pipes)
    {
        Point[] longestLoop = [];

        Stack<ImmutableList<Point>> open = new([[start]]);

        // TODO: orders of magnitude too slow, rethink approach
        //       - add visited set?
        //       - should be only one loop
        //       - start is already on loop isnt it?
        while(open.Count > 0)
        {
            var current = open.Pop();

            foreach(var neighbor in pipes[current.Last()])
            {
                if(neighbor == start)
                {
                    if(current.Count + 1 > longestLoop.Length)
                    {
                        longestLoop = [..current];
                    }
                }
                else if(!current.Contains(neighbor))
                {
                    var next = current.Add(neighbor);
                    open.Push(next);
                }
            }
        }

        return longestLoop;
    }

    private enum State { Outside = 1, OnLoop = 2, Inside = 3 };

    // You quickly reach the farthest point of the loop, but the animal never emerges. Maybe its nest is within the area enclosed by the loop?
    // To determine whether it's even worth taking the time to search for such a nest, you should calculate how many tiles are contained within the loop.
    // Figure out whether you have time to search for the nest by calculating the area within the loop.
    //
    // Puzzle == How many tiles are enclosed by the loop?
    private static int Puzzle2(Point[] loop)
    {
        var minCol = loop.Min(p => p.Col);
        var maxCol = loop.Max(p => p.Col);
        var minRow = loop.Min(p => p.Row);
        var maxRow = loop.Max(p => p.Row);

        HashSet<Point> insideTilesFound = [];
        var loopByRowAndCol = loop.Select((p, i) => (p, i))
                                  .OrderBy(tpl => tpl.p.Row)
                                  .ThenBy(tpl => tpl.p.Col)
                                  .GetEnumerator();
        loopByRowAndCol.MoveNext();

        // basic scanline fill algo for 2d
        // - move from top to bottom
        // - move from left to right
        // - keep a 'inside' flag updated and switch it everytime the loop is crossed
        //   the loop is crossed if the tiles in the row have the form of | or ┌──┘ or └─┐
        // - if the algorithm is 'inside' and not on a loop tile => add this tile to the inside list
        // runs in O(row*cols)

        for(int row = minRow; row <= maxRow; row++)
        {
            var inside     = false;
            var isOnLoop   = false;
            var signBefore = 0;

            for(int col = minCol - 1; col <= maxCol; col++)
            {
                if(loopByRowAndCol.Current.p.Row != row)
                {
                    continue;
                }

                if(loopByRowAndCol.Current.p.Col == col)
                {
                    var connectedToUp   = IsConnectedToUp(loopByRowAndCol.Current, loop);
                    var connectedToDown = IsConnectedToDown(loopByRowAndCol.Current, loop);
                    if(connectedToUp == connectedToDown)
                    {
                        // still on - or direct cross |
                        // - => isOnLoop stays true, inside stays same
                        // | => isOnLoop stays false, inside flips
                        inside = isOnLoop ? inside : !inside;
                    }
                    else
                    {
                        if(!isOnLoop)
                        {
                            // entering loop with ┌ or └ => remember signBefore

                            signBefore = connectedToUp ? -1 : 1;
                            isOnLoop = true;
                        }
                        else
                        {
                            // leaving loop with ┐ or ┘
                            // - not a crossing with ┌─┐ or └─┘  => inside stays the same
                            // - a crossing with ┌─┘ or └─┐      => inside flips

                            var signAfter = connectedToUp ? -1 : 1;
                            inside = signAfter == signBefore ? inside : !inside;

                            isOnLoop = false;
                            signBefore = 0;
                        }

                    }
                }
                else if(inside)
                {
                    insideTilesFound.Add(new Point(row, col));
                }

                if(col >= loopByRowAndCol.Current.p.Col)
                {
                    if(!loopByRowAndCol.MoveNext())
                    {
                        break;
                    }
                }
            }
        }

        return insideTilesFound.Count;
    }

    // next or previous tile in the loop is down
    private static bool IsConnectedToDown((Point p, int i) current, Point[] loop)
        => loop[(current.i + 1) % loop.Length].Row == current.p.Row + 1 ||
           loop[(current.i + loop.Length - 1) % loop.Length].Row == current.p.Row + 1;

    // next or previous tile in the loop is up
    private static bool IsConnectedToUp((Point p, int i) current, Point[] loop)
        =>  loop[(current.i + 1) % loop.Length].Row == current.p.Row - 1 ||
            loop[(current.i + loop.Length - 1) % loop.Length].Row == current.p.Row - 1;
}
