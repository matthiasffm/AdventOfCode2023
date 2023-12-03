import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day02';

describe('solving day02', () => {

    const sampleData = [
        "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
        "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
        "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
        "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
        "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green" ];

    test('puzzle1 should return the correct sum of the IDs of possible games for the sample data', () => {
        expect(puzzle1(sampleData, 12, 13, 14)).toBe(1 + 2 + 5);
    });

    test('puzzle2 should return the correct sum for the minimum set of cubes for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(48 + 12 + 1560 + 630 + 36);
    });

    const data = readFileSync(join(__dirname, '../../.input/day02.data'), 'utf-8').split('\n');

    test('puzzle1 should return the correct sum of the IDs of possible games for the real AoC data', () => {
        expect(puzzle1(data, 12, 13, 14)).toBe(1931);
    });

    test('puzzle2 should return the correct sum for the minimum set of cubes for the real AoC data', () => {
        expect(puzzle2(data)).toBe(83105);
    });
});
