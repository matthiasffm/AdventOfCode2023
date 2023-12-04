import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day04';

describe('solving day04', () => {

    const sampleData = [
        "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
        "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
        "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
        "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
        "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
        "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11", ];

    test('puzzle1 should return the total worth of all cards for the sample data', () => {
        expect(puzzle1(sampleData)).toBe(8 + 2 + 2 + 1 + 0 + 0);
    });

    test('puzzle2 should return how many total scratchcards you end up with for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(30);
    });

    const data = readFileSync(join(__dirname, '../../.input/day04.data'), 'utf-8').split('\n');

    test('puzzle1 should return the total worth of all cards for the real AoC data', () => {
        expect(puzzle1(data)).toBe(21821);
    });

    test('puzzle2 should return how many total scratchcards you end up with for the real AoC data', () => {
        expect(puzzle2(data)).toBe(5539496);
    });
});
