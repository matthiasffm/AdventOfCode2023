namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

[TestFixture]
public class Day01
{
    [Test]
    public void TestSamples()
    {
        var calibrations1 = new [] {
            "1abc2",
            "pqr3stu8vwx",
            "a1b2c3d4e5f",
            "treb7uchet",
        };
        Puzzle1(calibrations1).Should().Be(12 + 38 + 15 + 77);

        var calibrations2 = new [] {
            "two1nine",
            "eightwothree",
            "abcone2threexyz",
            "xtwone3four",
            "4nineeightseven2",
            "zoneight234",
            "7pqrstsixteen",
        };
        Puzzle2(calibrations2).Should().Be(29 + 83 + 13 + 24 + 42 + 14 + 76);
    }

    [Test]
    public void TestAocInput()
    {
        var calibrations  = FileUtils.ReadAllLines(this);

        Puzzle1(calibrations).Should().Be(55816);
        Puzzle2(calibrations).Should().Be(54980);
    }

    // The Elves discover that their calibration document has been amended by a very young Elf who was apparently just excited to show off her art skills. Consequently, the Elves
    // are having trouble reading the values on the document. The newly-improved calibration document consists of lines of text; each line originally contained a specific calibration
    // value that the Elves now need to recover. On each line, the calibration value can be found by combining the first digit and the last digit (in that order) to form a single
    // two-digit number.
    //
    // Puzzle == Consider your entire calibration document. What is the sum of all of the calibration values?
    private static int Puzzle1(IEnumerable<string> lines) =>
        lines.Sum(l => CalibrationValue(l.ToCharArray()));

    private static int CalibrationValue(char[] lettersAndDigits) =>
        (lettersAndDigits.First(c => char.IsNumber(c)) - 48) * 10 +
        (lettersAndDigits.Last(c => char.IsNumber(c)) - 48);

    // Your calculation isn't quite right. It looks like _some_ of the digits are actually spelled out with letters: one, two, three, four, five, six, seven, eight, and nine _also_
    // count as valid "digits".
    // Equipped with this new information, you now need to find the real first and last digit on each line.
    //
    // Puzzle == What is the sum of all of the calibration values?
    private static int Puzzle2(IEnumerable<string> lines) =>
        lines.Sum(l => CalibrationValue(l));

    private static int CalibrationValue(string line) =>
        Enumerable.Range(0, line.Length)
                  .Select(i => ToDigit(line[i..]))
                  .First(digit => digit > 0) * 10 +
        Enumerable.Range(0, line.Length)
                  .Select(i => ToDigit(line[(line.Length - i - 1)..]))
                  .First(digit => digit > 0);

    private static readonly string[] _Digits = [ "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" ];

    private static int ToDigit(string toConvert) =>
        char.IsNumber(toConvert[0]) ? toConvert[0] - 48 : DigitFromText(toConvert);

    private static int DigitFromText(string toConvert) =>
        _Digits.Select((digit, i) => toConvert.StartsWith(digit) ? i + 1 : -1)
               .FirstOrDefault(d => d > 0, -1);
}
