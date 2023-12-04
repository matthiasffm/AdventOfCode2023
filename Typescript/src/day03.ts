// An engine part seems to be missing from the engine, but nobody can figure out which one. If you can add up all the part numbers in the engine schematic, it should be
// easy to work out which part is missing. The engine schematic (your puzzle input) consists of a visual representation of the engine. There are lots of numbers and symbols
// you don't really understand, but apparently any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.
//
// Puzzle == What is the sum of all of the part numbers in the engine schematic?
export function puzzle1(lines: Array<string>) : number {
    let symbols = extractSymbols(lines);

    var sumValidNumbers = 0;

    for(let row = 0; row < lines.length; row++) {
        let currentNumber = 0;
        let hasAdjacentSymbol = false;

        for(let col = 0; col < lines[row].length; col++) {
            let cell = lines[row][col];

            // build the numbers digit by digit and remember all the adjacent symbols in the process
            if(isDigit(cell)) {
                currentNumber = currentNumber * 10 + parseInt(cell);
                hasAdjacentSymbol ||= neighbors(col, row).filter(n => symbols.has(n)).length > 0;
            }

            // at the end of a number sum it up
            if(currentNumber > 0 && (!isDigit(cell) || col === lines[row].length - 1)) {
                if(hasAdjacentSymbol) {
                    sumValidNumbers += currentNumber;
                }
                currentNumber = 0;
                hasAdjacentSymbol = false;
            }
        }
    }

    return sumValidNumbers;
}

// One of the gears in the engine is wrong. A gear is any * symbol that is adjacent to exactly two part numbers. Its gear ratio is the result of multiplying those two numbers
// together.
// This time, you need to find the gear ratio of every gear and add them all up so that the engineer can figure out which gear needs to be replaced.
//
// Puzzle == What is the sum of all of the gear ratios in your engine schematic?
export function puzzle2(lines: Array<string>) : number {
    let gears = extractGears(lines);
    let gearsWithAdjacentNumber = new Map<number, Array<number>>();

    for(let row = 0; row < lines.length; row++) {
        let currentNumber = 0;
        let adjacentGears = new Set<number>();

        for(let col = 0; col < lines[row].length; col++) {
            let cell = lines[row][col];

            // build the numbers digit by digit and remember all the adjacent gears in the process
            if(isDigit(cell)) {
                currentNumber = currentNumber * 10 + parseInt(cell);
                neighbors(col, row).filter(n => gears.has(n)).forEach(g => adjacentGears.add(g));
            }

            // at the end of a number save this number for every adjacent gear in the lookup gearsWithAdjacentNumbers
            if(currentNumber > 0 && (!isDigit(cell) || col === lines[row].length - 1)) {
                adjacentGears.forEach(g => {
                    if(!gearsWithAdjacentNumber.has(g)) {
                        gearsWithAdjacentNumber.set(g, [ currentNumber]);
                    }
                    else {
                        gearsWithAdjacentNumber.get(g)!.push(currentNumber);
                    }
                });

                currentNumber = 0;
                adjacentGears.clear();
            }
        }
    }

    // in gearsWithAdjacentNumbers are all gears and all their adjacent numbers mapped
    // if any of them has excatly two => sum their gear ratio up as asked

    var sum = 0;

    for(const [gear, adjacentNumbers] of gearsWithAdjacentNumber) {
        if(adjacentNumbers.length == 2) {
            sum += adjacentNumbers[0] * adjacentNumbers[1];
        }
    }

    return sum;
}

function extractSymbols(lines: string[]) : Set<number> {
    let symbolPos = new Set<number>();

    for(let row = 0; row < lines.length; row++) {
        for(let col = 0; col < lines[row].length; col++) {
            if(lines[row][col] != '.' && !isDigit(lines[row][col])) {
                symbolPos.add(pos(col, row));
            }
        }
    }

    return symbolPos;
}

function extractGears(lines: string[]) : Set<number> {
    let gearPos = new Set<number>();

    for(let row = 0; row < lines.length; row++) {
        for(let col = 0; col < lines[row].length; col++) {
            if(lines[row][col] === '*') {
                gearPos.add(pos(col, row));
            }
        }
    }

    return gearPos;
}

function isDigit(c: string) : boolean {
    return c[0] >= '0' && c[0] <= '9';
}

// encodes a point coordinate in a single number so it can be used in a set
function pos(col: number, row: number): number {
    return col + row * 1000;
};

// decodes the column coordinate from a position code
function col(pos: number) : number {
    return pos % 1000;
}

// decodes the row coordinate from a position code
function row(pos: number) : number {
    return Math.trunc(pos / 1000);
}

function neighbors(col: number, row: number) : number[] {
    let center = pos(col, row);
    return offsets.map(offset => center + offset);
}

let offsets: number[] = [ pos(-1, -1), pos(0, -1), pos(1, -1), pos(-1, 0), pos(1, 0), pos(-1, 1), pos(0, 1), pos(1, 1) ] as const;
