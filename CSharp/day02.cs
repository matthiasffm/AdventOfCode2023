namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: draw colored cubes from bag
/// </summary>
[TestFixture]
public class Day02
{
    private record Game(int Id, IEnumerable<(int r, int g, int b)> draws);

    private static IEnumerable<Game> ParseData(string[] lines) => lines.Select(l => ParseGame(l)).ToArray();

    private static Game ParseGame(string l) => new Game(ParseId(l.Split(':')[0]), ParseDraws(l.Split(':')[1]));

    private static int ParseId(string gameId) => int.Parse(gameId.Split(' ')[1]);

    private static IEnumerable<(int r, int g, int b)> ParseDraws(string draw) => draw.Split(';').Select(d => ParseDraw(d)).ToArray();

    private static (int r, int g, int b) ParseDraw(string draw)
    {
        int r = 0;
        int b = 0;
        int g = 0;

        foreach(var cube in draw.Trim().Split(','))
        {
            var cubeNbr = cube.Trim().Split(' ')[0];

            if(cube.Contains("red"))
            {
                r = int.Parse(cubeNbr);
            }
            else if(cube.Contains("green"))
            {
                g = int.Parse(cubeNbr);
            }
            else
            {
                b = int.Parse(cubeNbr);
            }
        }

        return (r, g, b);
    }

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
            "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
            "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
            "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
            "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green",
        };

        var games = ParseData(data);

        Puzzle1(games, 12, 13, 14).Should().Be(1 + 2 + 5);
        Puzzle2(games).Should().Be(48 + 12 + 1560 + 630 + 36);
    }

    [Test]
    public void TestAocInput()
    {
        var data  = FileUtils.ReadAllLines(this);
        var games = ParseData(data);

        Puzzle1(games, 12, 13, 14).Should().Be(1931);
        Puzzle2(games).Should().Be(83105);
    }

    // As you walk, the Elf shows you a small bag and some cubes which are either red, green, or blue. Each time you play this game, he will hide a secret number of cubes of
    // each color in the bag, and your goal is to figure out information about the number of cubes. To get information, once a bag has been loaded with cubes, the Elf will
    // reach into the bag, grab a handful of random cubes, show them to you, and then put them back in the bag. He'll do this a few times per game.
    // You play several games and record the information from each game. Each game is listed with its ID number (like the 11 in Game 11: ...) followed by a semicolon-separated
    // list of subsets of cubes that were revealed from the bag (like 3 red, 5 green, 4 blue).
    //
    // Puzzle == Determine which games would have been possible if the bag had been loaded with only 12 red cubes, 13 green cubes, and 14 blue cubes. What is the sum of the
    //           IDs of those games?
    private static int Puzzle1(IEnumerable<Game> games, int rMax, int gMax, int bMax) =>
        games.Where(g => g.draws.All(d => d.r <= rMax && d.g <= gMax && d.b <= bMax))
             .Sum(g => g.Id);

    // As you continue your walk, the Elf poses a second question: in each game you played, what is the fewest number of cubes of each color that could have been in the bag
    // to make the game possible?
    // The power of a set of cubes is equal to the numbers of red, green, and blue cubes multiplied together.
    //
    // Puzzle == For each game, find the minimum set of cubes that must have been present. What is the sum of the power of these sets?
    private static int Puzzle2(IEnumerable<Game> games) =>
        games.Select(game => game.draws.Max(d => d.r) *
                             game.draws.Max(d => d.g) *
                             game.draws.Max(d => d.b))
             .Sum();
}
