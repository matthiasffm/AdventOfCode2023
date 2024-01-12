// You pull out your handy Oasis And Sand Instability Sensor and analyze your surroundings. The OASIS produces a report of many values and how they are changing over
// time (your puzzle input). Each line in the report contains the history of a single value.
// To best protect the oasis, your environmental report should include a prediction of the next value in each history. To do this, start by making a new sequence from
// the difference at each step of your history. If that sequence is not all zeroes, repeat this process, using the sequence you just generated as the input sequence.
// Once all of the values in your latest sequence are zeroes, you can extrapolate what the next value of the original history should be.
// Analyze your OASIS report and extrapolate the next value for each history. 
//
// Puzzle == What is the sum of these extrapolated values?
export function puzzle1(lines: Array<string>) : number {
    return parseData(lines).reduce((sum, line) => sum + extrapolateNextValue(line), 0);
}

function extrapolateNextValue(line: number[]): number {
    if(line.filter(l => l == 0).length == line.length) {
        return 0;
    } else {
        return line.slice(-1)[0] + extrapolateNextValue(diff(line));
    }
}

// Of course, it would be nice to have even more history included in your report. Surely it's safe to just extrapolate backwards as well, right?
// For each history, repeat the process of finding differences until the sequence of differences is entirely zero. Then, rather than adding a zero to the end and filling
// in the next values of each previous sequence, you should instead add a zero to the beginning of your sequence of zeroes, then fill in new first values for each
// previous sequence.
// Analyze your OASIS report again, this time extrapolating the previous value for each history.
//
// Puzzle ==  What is the sum of these extrapolated values?
export function puzzle2(lines: Array<string>) : number {
    return parseData(lines).reduce((sum, line) => sum + extrapolatePreviousValue(line), 0);
}

function extrapolatePreviousValue(line: number[]): number {
    if(line.filter(l => l == 0).length == line.length) {
        return 0;
    } else {
        return line[0] - extrapolatePreviousValue(diff(line));
    }
}

function parseData(lines: Array<string>) : number[][] {
    return lines.map(l => l.split(/\s+/).map(n => parseInt(n)));
}

// calculates the differences between each pair of values
function diff(line: number[]): number[] {
    return line.slice(0, -1)
               .map((l, i) => line[i + 1] - l);
}
