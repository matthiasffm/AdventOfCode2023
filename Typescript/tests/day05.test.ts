import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day05';

describe('solving day05', () => {

    const sampleData = [
        "seeds: 79 14 55 13",
        "",
        "seed-to-soil map:",
        "50 98 2",
        "52 50 48",
        "",
        "soil-to-fertilizer map:",
        "0 15 37",
        "37 52 2",
        "39 0 15",
        "",
        "fertilizer-to-water map:",
        "49 53 8",
        "0 11 42",
        "42 0 7",
        "57 7 4",
        "",
        "water-to-light map:",
        "88 18 7",
        "18 25 70",
        "",
        "light-to-temperature map:",
        "45 77 23",
        "81 45 19",
        "68 64 13",
        "",
        "temperature-to-humidity map:",
        "0 69 1",
        "1 0 69",
        "",
        "humidity-to-location map:",
        "60 56 37",
        "56 93 4", ];

    test('puzzle1 should return the lowest location number of the mapped seed values for the sample data', () => {
        expect(puzzle1(sampleData)).toBe(35);
    });

    test('puzzle2 should return the lowest location number of the mapped seed ranges for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(46);
    });

    const data = readFileSync(join(__dirname, '../../.input/day05.data'), 'utf-8').split('\n');

    test('puzzle1 should return the lowest location number of the mapped seed values for the real AoC data', () => {
        expect(puzzle1(data)).toBe(836040384);
    });

    test('puzzle2 should return the lowest location number of the mapped seed ranges for the real AoC data', () => {
        expect(puzzle2(data)).toBe(10834440);
    });
});
