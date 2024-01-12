import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day08';

describe('solving day08 (navigate the network)', () => {

    let sampleData1 = [
        "LLR",
        "",
        "AAA = (BBB, BBB)",
        "BBB = (AAA, ZZZ)",
        "ZZZ = (ZZZ, ZZZ)", ];

    test('puzzle1 should return the number of steps to reach ZZZ for the first sample data', () => {
        expect(puzzle1(sampleData1)).toBe(6);
    });

    let sampleData2 = [
        "RL",
        "",
        "AAA = (BBB, CCC)",
        "BBB = (DDD, EEE)",
        "CCC = (ZZZ, GGG)",
        "DDD = (DDD, DDD)",
        "EEE = (EEE, EEE)",
        "GGG = (GGG, GGG)",
        "ZZZ = (ZZZ, ZZZ)", ];

    test('puzzle1 should return the number of steps to reach ZZZ for the second sample data', () => {
        expect(puzzle1(sampleData2)).toBe(2);
    });

    let sampleData3 = [
        "LR",
        "",
        "11A = (11B, XXX)",
        "11B = (XXX, 11Z)",
        "11Z = (11B, XXX)",
        "22A = (22B, XXX)",
        "22B = (22C, 22C)",
        "22C = (22Z, 22Z)",
        "22Z = (22B, 22B)",
        "XXX = (XXX, XXX)", ];

    test('puzzle2 should return the number of steps to end on **Z only nodes for the third sample data', () => {
        expect(puzzle2(sampleData3)).toBe(6);
    });

    const data = readFileSync(join(__dirname, '../../.input/day08.data'), 'utf-8').split('\n');

    test('puzzle1 should return the number of steps to reach ZZZ for the real AoC data', () => {
        expect(puzzle1(data)).toBe(16579);
    });

    test('puzzle2 should return the number of steps to end on **Z only nodes for the real AoC data', () => {
        expect(puzzle2(data)).toBe(12927600769609);
    });
});
