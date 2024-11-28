namespace AdventOfCode2023;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: hash initialization sequence
/// </summary>
[TestFixture]
public class Day15
{
    private record Instruction(string Label, char Operation, int FocalLength);

    private static (string, Instruction)[] ParseData(string data)
        => data.Split(',').Select(s => (s, s.IndexOfAny(['-', '='])))
                          .Select(t => (t.s, new Instruction(t.s[.. t.Item2], t.s[t.Item2], t.s[t.Item2] == '=' ? int.Parse(t.s[(t.Item2 + 1)..]) : 0)))
                          .ToArray();

    [Test]
    public void TestSamples()
    {
        var data = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
        var steps = ParseData(data);

        Hash("HASH").Should().Be(52);

        Puzzle1(steps).Should().Be(30 + 253 + 97 + 47 + 14 + 180 + 9 + 197 + 48 + 214 + 231);
        Puzzle2(steps).Should().Be(1 + 4 + 28 + 40 + 72);
    }

    [Test]
    public void TestAocInput()
    {
        var data  = FileUtils.ReadAllText(this);
        var steps = ParseData(data);

        Puzzle1(steps).Should().Be(513214);
        Puzzle2(steps).Should().Be(258826);
    }

    // The HASH algorithm is a way to turn any string of characters into a single number in the range 0 to 255. To run the HASH algorithm on a string, start with a
    // current value of 0. Then, for each character in the string starting from the beginning: hash = (Ascii-Code(character) * 17) mod 256. The HASH value of a text
    // is the sum of the HASH values of all characters.
    // The initialization sequence (your puzzle input) is a comma-separated list of steps to start the Lava Production Facility. To verify that your HASH algorithm is
    // working, the book offers the sum of the result of running the HASH algorithm on each step in the initialization sequence.
    //
    // Puzzle == Run the HASH algorithm on each step in the initialization sequence. What is the sum of the results?
    private static int Puzzle1((string step, Instruction instr)[] steps)
        => steps.Sum(s => Hash(s.step));

    // The book goes on to describe a series of 256 boxes numbered 0 through 255. The boxes are arranged in a line starting from the point where light enters the facility.
    // The boxes have holes that allow light to pass from one box to the next all the way down the line. Inside each box, there are several lens slots that will keep a lens
    // correctly positioned to focus light passing through the box. The side of each box has a panel that opens to allow you to insert or remove lenses as necessary.
    // Along the wall running parallel to the boxes is a large library containing lenses organized by focal length ranging from 1 through 9. The book goes on to explain how
    // to perform each step in the initialization sequence, a process it calls the Holiday ASCII String Helper Manual Arrangement Procedure, or HASHMAP for short.
    // Each step begins with a sequence of letters that indicate the label of the lens on which the step operates. The result of running the HASH algorithm on the label
    // indicates the correct box for that step.
    // The label will be immediately followed by a character that indicates the operation to perform: either an equals sign (=) or a dash (-).
    // If the operation character is a dash (-), go to the relevant box and remove the lens with the given label if it is present in the box. Then, move any remaining lenses
    // as far forward in the box as they can go without changing their order, filling any space made by removing the indicated lens.
    // If the operation character is an equals sign (=), it will be followed by a number indicating the focal length of the lens that needs to go into the relevant box; be
    // sure to use the label maker to mark the lens with the label given in the beginning of the step so you can find it later. There are two possible situations: If there is
    // already a lens in the box with the same label, replace the old lens with the new lens: remove the old lens and put the new lens in its place, not moving any other lenses
    // in the box. If there is not already a lens in the box with the same label, add the lens to the box immediately behind any lenses already in the box. Don't move any of
    // the other lenses when you do this. If there aren't any lenses in the box, the new lens goes all the way to the front of the box.
    // To confirm that all of the lenses are installed correctly, add up the focusing power of all of the lenses. The focusing power of a single lens is the result of
    // multiplying together: One plus the box number of the lens in question, the slot number of the lens within the box and the focal length of the lens.
    //
    // Puzzle == What is the focusing power of the resulting lens configuration?
    private static long Puzzle2((string step, Instruction instr)[] steps)
    {
        var boxes = new LinkedList<(string label, int focalLength)>[256];
        for(int b = 0; b < boxes.Length; b++)
        {
            boxes[b] = new LinkedList<(string label, int focalLength)>();
        }

        foreach(var step in steps)
        {
            var label       = step.instr.Label;
            var box         = Hash(label);
            
            if(step.instr.Operation == '-')
            {
                // remove lens with label from box if present

                if(FindLensInBox(boxes[box], label, out var lensWithLabel))
                {
                    boxes[box].Remove(lensWithLabel);
                }
            }
            else
            {
                // replace or insert lens with label focalLength into box

                var focalLength = step.instr.FocalLength;

                if(FindLensInBox(boxes[box], label, out var lensWithLabel))
                {
                    lensWithLabel.Value = (label, focalLength);
                }
                else
                {
                    boxes[box].AddLast((label, focalLength));
                }
            }
        }

        return boxes.SelectMany((box, b) => box.Select((l, i) => (b + 1) * (i + 1) * l.focalLength))
                    .Sum();
    }

    private static bool FindLensInBox(LinkedList<(string label, int focalLength)> box,
                                      string label,
                                      [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]out LinkedListNode<(string label, int focalLength)> lens)
    {
        var listNode = box.First;
        while(listNode != null)
        {
            if(listNode.Value.label == label)
            {
                lens = listNode;
                return true;
            }
            listNode = listNode.Next;
        }
        lens = default;
        return false;
    }

    private static int Hash(string text)
    {
        var hash = 0;

        for(int c = 0; c < text.Length; c++)
        {
            hash = (hash + Convert.ToInt32(text[c])) * 17 % 256;
        }

        return hash;
    }
}
