import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day03';

describe('solving day03 (find gears for the gondola)', () => {

    const sampleData = [
        "467..114..",
        "...*......",
        "..35..633.",
        "......#...",
        "617*......",
        ".....+.58.",
        "..592.....",
        "......755.",
        "...$.*....",
        ".664.598.." ];

    test('puzzle1 should return the sum of all valid part numbers for the sample data', () => {
        expect(puzzle1(sampleData)).toBe(4361);
    });

    test('puzzle2 should return the sum of all of the gear ratios for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(467 * 35 + 755 * 598);
    });

    const data = readFileSync(join(__dirname, '../../.input/day03.data'), 'utf-8').split('\n');

    test('puzzle1 should return the sum of all valid part numbers for the real AoC data', () => {
        expect(puzzle1(data)).toBe(535235);
    });

    test('puzzle2 should return the sum of all of the gear ratios for the real AoC data', () => {
        expect(puzzle2(data)).toBe(79844424);
    });
});
