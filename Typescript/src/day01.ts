// The Elves discover that their calibration document has been amended by a very young Elf who was apparently just excited to show off her art skills. Consequently, the Elves
// are having trouble reading the values on the document. The newly-improved calibration document consists of lines of text; each line originally contained a specific calibration
// value that the Elves now need to recover. On each line, the calibration value can be found by combining the first digit and the last digit (in that order) to form a single
// two-digit number.
//
// Puzzle == Consider your entire calibration document. What is the sum of all of the calibration values?
export function puzzle1(lines: Array<string>) : number {
    return lines.reduce((sum, line) => sum + calibrationValue(line), 0);
}

// Your calculation isn't quite right. It looks like _some_ of the digits are actually spelled out with letters: one, two, three, four, five, six, seven, eight, and nine _also_
// count as valid "digits".
// Equipped with this new information, you now need to find the real first and last digit on each line.
//
// Puzzle == What is the sum of all of the calibration values?
export function puzzle2(lines: Array<string>) : number {
    return lines.reduce((sum, line) => sum + calibrationValue(normalize(line)), 0);
}

function calibrationValue(line: string) : number {
    let numbersOnly = filterDigits(line);
    return numbersOnly[0] * 10 + numbersOnly.at(-1)!;
}

function filterDigits(line : string) : number[] {
    return line.split('')
               .filter(c => isDigit(c))
               .map(c => parseInt(c[0]));
}

function isDigit(c: string) : boolean {
    return c[0] >= '0' && c[0] <= '9';
}

const digitMapping : Map<string, string> = new Map([
    ["one",     "1" ],
    ["two",     "2" ],
    ["three",   "3" ],
    ["four",    "4" ],
    ["five",    "5" ],
    ["six",     "6" ],
    ["seven",   "7" ],
    ["eight",   "8" ],
    ["nine",    "9" ]
]);

// normalizes the string by adding digit char for all occurences of words for numbers, ignores overlaps
function normalize(line: string) : string {
    return line.replace(/(?=(one|two|three|four|five|six|seven|eight|nine))/gi, (dummy, match) => digitMapping.get(match)!);
}
