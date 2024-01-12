import { readFileSync } from 'fs';
import { join } from 'path';

import { puzzle1, puzzle2 } from '../src/day09';

describe('solving day09 (predict next or previous environment value)', () => {

    let sampleData = [
        "0 3 6 9 12 15",
        "1 3 6 10 15 21",
        "10 13 16 21 30 45", ];

    test('puzzle1 should return the sum of the next values for the sample data', () => {
        expect(puzzle1(sampleData)).toBe(18 + 28 + 68);
    });

    test('puzzle2 should return the sum of the next values for the sample data', () => {
        expect(puzzle2(sampleData)).toBe(-3 + 0 + 5);
    });

    const data = readFileSync(join(__dirname, '../../.input/day09.data'), 'utf-8').split('\n');
    data.splice(-1);

    test('puzzle1 should return the sum of the previous values for the real AoC data', () => {
        expect(puzzle1(data)).toBe(1887980197);
    });

    test('puzzle2 should return the sum of the previous values for the real AoC data', () => {
        expect(puzzle2(data)).toBe(990);
    });
});
