namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: navigate the network
/// </summary>
[TestFixture]
public class Day08
{
    private static IDictionary<string, (string Left, string Right)> ParseData(IEnumerable<string> lines) =>
        lines.Select(l => (Key: l[..3], Left: l[7..10], Right: l[12..15]))
             .ToDictionary(t => t.Key, t => (Left: t.Left, Right: t.Right));

    [Test]
    public void TestSamplesPuzzle1()
    {
        string[] lines = [
            "LLR",
            "",
            "AAA = (BBB, BBB)",
            "BBB = (AAA, ZZZ)",
            "ZZZ = (ZZZ, ZZZ)",
        ];

        var instructions = lines[0];
        var nodes = ParseData(lines[2..]);

        Puzzle1(instructions, nodes).Should().Be(6);

        lines = [
            "RL",
            "",
            "AAA = (BBB, CCC)",
            "BBB = (DDD, EEE)",
            "CCC = (ZZZ, GGG)",
            "DDD = (DDD, DDD)",
            "EEE = (EEE, EEE)",
            "GGG = (GGG, GGG)",
            "ZZZ = (ZZZ, ZZZ)",
        ];

        instructions = lines[0];
        nodes = ParseData(lines.Skip(2));

        Puzzle1(instructions, nodes).Should().Be(2);
    }

    [Test]
    public void TestSamplesPuzzle2()
    {
        string[] lines = [
            "LR",
            "",
            "11A = (11B, XXX)",
            "11B = (XXX, 11Z)",
            "11Z = (11B, XXX)",
            "22A = (22B, XXX)",
            "22B = (22C, 22C)",
            "22C = (22Z, 22Z)",
            "22Z = (22B, 22B)",
            "XXX = (XXX, XXX)",
        ];

        var instructions = lines[0];
        var nodes = ParseData(lines[2..]);

        Puzzle2(instructions, nodes).Should().Be(6L);
    }

    [Test]
    public void TestAocInput()
    {
        var lines = FileUtils.ReadAllLines(this);

        var instructions = lines[0];
        var nodes = ParseData(lines[2..]);

        Puzzle1(instructions, nodes).Should().Be(16579);
        Puzzle2(instructions, nodes).Should().Be(12927600769609L);
    }

    // The documents contains a list of left/right instructions, and the rest of the documents seem to describe some kind of network of labeled nodes. It seems like
    // you're meant to use the left/right instructions to navigate the network. After examining the maps for a bit, two nodes stick out: AAA and ZZZ. You feel like
    // AAA is where you are now, and you have to follow the left/right instructions until you reach ZZZ. Starting at AAA, follow the left/right instructions.
    //
    // Puzzle ==  How many steps are required to reach ZZZ?
    private static int Puzzle1(string instructions, IDictionary<string, (string Left, string Right)> nodes)
    {
        var step = 0;
        string currentNode = "AAA";

        while(currentNode != "ZZZ")
        {
            currentNode =  instructions[step % instructions.Length] == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;
            step++;
        }

        return step;
    }

    // After examining the maps a bit longer, your attention is drawn to a curious fact: the number of nodes with names ending in A is equal to the number ending in Z! If
    // you were a ghost, you'd probably just start at every node that ends with A and follow all of the paths at the same time until they all simultaneously end up at nodes
    // that end with Z. Simultaneously start on every node that ends with A.
    //
    // Puzzle == How many steps does it take before you're only on nodes that end with Z?
    private static long Puzzle2(string instructions, IDictionary<string, (string Left, string Right)> nodes)
    {
        var step = 0;

        // (verified) assumption: all interations have the same breadth
        var current = nodes.Keys.Where(k => k[^1] == 'A').ToArray();
        var foundZ  = current.Select(n => 0L).ToArray();

        do
        {
            for(int i = 0; i < current.Length; i++)
            {
                // detect period
                // data is special, first occurence of Z is also period length and there is only one period for every simultaneous path

                if(current[i][^1] == 'Z' && foundZ[i] == 0)
                {
                    foundZ[i] = step;
                }

                // move to next iteration

                current[i] = instructions[step % instructions.Length] == 'L' ? nodes[current[i]].Left: nodes[current[i]].Right;
            }

            step++;
        }
        while(foundZ.Any(p => p == 0));

        return Euclid.Lcm(foundZ);
    }
}
