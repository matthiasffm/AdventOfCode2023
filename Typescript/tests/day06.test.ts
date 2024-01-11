import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day06';

describe('solving day06', () => {

    const sampleData = [
        "Time:      7  15   30",
        "Distance:  9  40  200", ];

    test('puzzle1 should return the number of times the record of the boat race can be broken for the sample data', () => {
        expect(puzzle1(sampleData)).toBe(4 * 8 * 9);
    });

    test('puzzle2 should return for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(71503);
    });

    const data = readFileSync(join(__dirname, '../../.input/day06.data'), 'utf-8').split('\n');

    test('puzzle1 should return the number of times the record of the boat race can be broken for the real AoC data', () => {
        expect(puzzle1(data)).toBe(281600);
    });

    test('puzzle2 should return for the real AoC data', () => {
        expect(puzzle2(data)).toBe(33875953);
    });
});
