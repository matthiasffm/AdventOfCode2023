type Category = {
    readonly from:     string;
    readonly to:       string;
    readonly mappings: Mapping[];
}

type Mapping = {
    readonly start:  number;
    readonly end:    number;
    readonly offset: number;
};

// The almanac contains a list of maps which describe how to convert numbers from a source category into numbers in a destination category. That is, the section
// that starts with seed-to-soil map: describes how to convert a seed number (the source) to a soil number (the destination). This lets the gardener and his team
// know which soil to use with which seeds, which water to use with which fertilizer, and so on. Rather than list every source number and its corresponding destination
// number one by one, the maps describe entire ranges of numbers that can be converted. Each line within a map contains three numbers: the destination range start, the
// source range start, and the range length.
// Any source numbers that aren't mapped correspond to the same destination number.
// The gardener and his team want to get started as soon as possible, so they'd like to know the closest location that needs a seed. Using these maps, find the lowest
// location number that corresponds to any of the initial seeds. To do this, you'll need to convert each seed number through other categories until you can find its
// corresponding location number.
//
// Puzzle == What is the lowest location number that corresponds to any of the initial seed numbers?
export function puzzle1(lines: Array<string>) : number {
    const seedLocations = parseSeeds(lines[0]);
    const categories    = parseMappings(lines);

    return Math.min(...categories.reduce((current, category) => mapSeeds(current, category.mappings),
                                         seedLocations));
}

// maps all seed locations in seeds forward based on the offsets in mappings
function mapSeeds(seeds: number[], mappings: Mapping[]): number[] {
    return seeds.map(s => mapSeed(s, mappings));
}

// maps a seed location forward based on the offsets in mappings
function mapSeed(seed: number, mappings: Mapping[]): number {
    for(let mapping of mappings) {
        if(mapping.start <= seed && seed <= mapping.end) {
            return seed + mapping.offset;
        }
    }
    return seed;
}

class Range {
    public readonly start: number;
    public readonly end:   number;

    public constructor(start: number, end: number) {
        this.start = start;
        this.end   = end;
    }

    public length() : number {
        return this.end - this.start + 1;
    }

    public add(offset: number) : Range {
        return new Range(this.start + offset, this.end + offset);
    }

    public intersect(mapping: Mapping): Range {
        return new Range(Math.max(this.start, mapping.start),
                         Math.min(this.end, mapping.end));
    }
}

// Everyone will starve if you only plant such a small number of seeds. Re-reading the almanac, it looks like the seeds: line actually describes ranges of seed
// numbers. The values on the initial seeds: line come in pairs. Within each pair, the first value is the start of the range and the second value is the length
// of the range.
// Puzzle == Consider all of the initial seed numbers listed in the ranges on the first line of the almanac. What is the lowest location number that corresponds
//           to any of the initial seed numbers?
export function puzzle2(lines: Array<string>) : number {
    const seedRanges = parseSeeds(lines[0]).map((s, i, seeds) => new Range(s, s + seeds[i + 1] - 1))
                                           .filter((range, i) => i % 2 == 0)
                                           .sort((a, b) => a.start - b.start);
    const categories = parseMappings(lines);

    const mappedRanges = categories.reduce((current, category) => mapRanges(current, category.mappings),
                                           seedRanges);
    return Math.min(...mappedRanges.map(range => range.start));
}

// maps all seed ranges in ranges forward based on the offsets in mappings
function mapRanges(ranges: Range[], mappings: Mapping[]): Range[] {

    const newRanges: Range[] = [];

    for(let range of ranges) {
        for(let mapping of mappings) {

            // intersect ranges with ordered mappings and
            // map these new ranges forward with mapping offsets to next level

            const newRange = range.intersect(mapping);
            if(newRange.length() > 0) {
                newRanges.push(newRange.add(mapping.offset));
            }
        }
    }

    return newRanges.sort((a, b) => a.start - b.start);
}

function parseSeeds(firstLine: string) : number[] {
    return firstLine.substring(7)
                    .split(' ')
                    .map(nmbr => parseInt(nmbr));
}

function parseMappings(lines: Array<string>) : Category[] {
    let categories: Array<Category> = [];

    var current: Category = { from: "", to: "", mappings: [] };

    for(var line = 2; line < lines.length; line++) {
        if(lines[line].indexOf("map:") > 0) {
            current = {from: lines[line].split('-')[0], to: lines[line].split(/\s|-/)[2], mappings: []};
            categories.push(current);
        }
        else if(lines[line] !== "") {
            let nmbrs = lines[line].split(' ').map(nmbr => parseInt(nmbr));
            current.mappings.push({ start: nmbrs[1], end: nmbrs[1] + nmbrs[2] - 1, offset: nmbrs[0] - nmbrs[1] });
        }
    }

    for(var category of categories) {
        addZeroOffsets(category.mappings, 0, 7952449232);
    }

    return categories;
}

// fills potential gaps in mappings from min to max with zero offset mappings
function addZeroOffsets(mappings: Mapping[], min: number, max: number) {
    mappings.sort((a, b) => a.start - b.start);

    var current = min;
    for(var i = 0; i < mappings.length; i++) {
        if(current < mappings[i].start) {
            mappings.splice(i, 0, {start: current, end: mappings[i].start - 1, offset: 0});
            i++;
        }
        current = mappings[i].end + 1;
    }

    if(mappings[mappings.length - 1].end < max) {
        mappings.push({start: mappings[mappings.length - 1].end + 1, end: max, offset: 0});
    }
}

