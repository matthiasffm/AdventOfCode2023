import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day07';

describe('solving day07', () => {

    const sampleData = [
        "32T3K 765",
        "T55J5 684",
        "KK677 28",
        "KTJJT 220",
        "QQQJA 483", ];

    test('puzzle1 should return the total winnings for the hands in the sample data', () => {
        expect(puzzle1(sampleData)).toBe(765 * 1 + 220 * 2 + 28 * 3 + 684 * 4 + 483 * 5);
    });

    test('puzzle2 should return the total winnings with the new joker rules for the hands in the sample data', () => {
        expect(puzzle2(sampleData)).toBe(5905);
    });

    const data = readFileSync(join(__dirname, '../../.input/day07.data'), 'utf-8').split('\n');
    data.splice(-1);

    test('puzzle1 should return the total winnings for the hands in the real AoC data', () => {
        expect(puzzle1(data)).toBe(250347426);
    });

    test('puzzle2 should return the total winnings with the new joker rules for the hands in the real AoC data', () => {
        expect(puzzle2(data)).toBe(251224870);
    });
});
