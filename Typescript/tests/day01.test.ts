import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day01';

describe('solving day01 (find calibration digits)', () => {
    test('puzzle1 should return the correct sum for the sample calibration data', () => {
        expect(puzzle1(["1abc2", "pqr3stu8vwx", "a1b2c3d4e5f", "treb7uchet" ]))
                .toBe(12 + 38 + 15 + 77);
    });

    test('puzzle2 should return the correct sum for the sample calibration data', () => {
        expect(puzzle2(["two1nine", "eightwothree", "abcone2threexyz", "xtwone3four", "4nineeightseven2", "zoneight234", "7pqrstsixteen" ]))
                .toBe(29 + 83 + 13 + 24 + 42 + 14 + 76);
    });

    const data = readFileSync(join(__dirname, '../../.input/day01.data'), 'utf-8').split('\n');

    test('puzzle1 should return the correct sum for the real AoC calibration data', () => {
        expect(puzzle1(data)).toBe(55816);
    });

    test('puzzle2 should return the correct sum for the real AoC calibration data', () => {
        expect(puzzle2(data)).toBe(54980);
    });
});
