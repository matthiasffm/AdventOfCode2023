type BoatRace = {
    readonly time:     number;
    readonly distance: number;
};

// As part of signing up, you get a sheet of paper (your puzzle input) that lists the time allowed for each race and also the best distance ever recorded in that race. To
// guarantee you win the grand prize, you need to make sure you go farther in each race than the current record holder. The organizer brings you over to the area where the
// boat races are held. Each boat is equiped with a big button on top. Holding down the button charges the boat, and releasing the button allows the boat to move. Boats move
// faster if their button was held longer, but time spent holding the button counts against the total race time. You can only hold the button at the start of the race, and
// boats don't move until the button is released.
// Your toy boat has a starting speed of zero millimeters per millisecond. For each whole millisecond you spend at the beginning of the race holding down the button, the
// boat's speed increases by one millimeter per millisecond.
// To see how much margin of error you have, determine the number of ways you can beat the record in each race.
//
// Puzzle == Determine the number of ways you could beat the record in each race. What do you get if you multiply these numbers together?
export function puzzle1(lines: Array<string>) : number {
    const boatRaces = parseBoatRaces(lines);
    return boatRaces.reduce((prod, race) => prod * numberOfWaysToBeatRecord(race), 1);
}

function parseBoatRaces(lines: string[]) : BoatRace[] {
    const times     = parseLineToInts(lines[0].substring(9));
    const distances = parseLineToInts(lines[1].substring(9));

    return times.map((t, i, arr) => ({time: t, distance: distances[i]}));
}

function parseLineToInts(line: string) : number[] {
    return line.split(/\s+/)
               .filter(s => !isNullOrWhitespace(s))
               .map(s => parseInt(s));
}

function isNullOrWhitespace(s: string){
    return s === null || s.match(/^ *$/) !== null;
}

// As the race is about to start, you realize the piece of paper with race times and record distances you got earlier actually just has very bad kerning. There's really only
// one race - ignore the spaces between the numbers on each line.
//
// Puzzle == How many ways can you beat the record in this one much longer race?
export function puzzle2(lines: Array<string>) : number {
    const time     = lines[0].substring(9).split(/\s+/).join('');
    const distance = lines[1].substring(9).split(/\s+/).join('');

    return numberOfWaysToBeatRecord({time: parseInt(time), distance: parseInt(distance)});
}

// solve x * (time - x) > distance by calculating the roots of
// x * (time - x) - distance = 0. the difference between the root values
// is the number of times the record can be6 beaten.
//       root r1 = 1/2 (time - sqrt(time^2 - 4 * distance))
//       root r2 = 1/2 (time + sqrt(time^2 - 4 * distance))
function numberOfWaysToBeatRecord(race: BoatRace) : number {
    var sqrt  = Math.sqrt(race.time * race.time - 4 * race.distance);
    var root1 = (race.time - sqrt) / 2 + 0.0001;
    var root2 = (race.time + sqrt) / 2 - 0.0001;

    return Math.abs(Math.floor(root2) - Math.floor(root1));
}
