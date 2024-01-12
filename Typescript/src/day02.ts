// As you walk, the Elf shows you a small bag and some cubes which are either red, green, or blue. Each time you play this game, he will hide a secret number of cubes of
// each color in the bag, and your goal is to figure out information about the number of cubes. To get information, once a bag has been loaded with cubes, the Elf will
// reach into the bag, grab a handful of random cubes, show them to you, and then put them back in the bag. He'll do this a few times per game.
// You play several games and record the information from each game. Each game is listed with its ID number (like the 11 in Game 11: ...) followed by a semicolon-separated
// list of subsets of cubes that were revealed from the bag (like 3 red, 5 green, 4 blue).
//
// Puzzle == Determine which games would have been possible if the bag had been loaded with only 12 red cubes, 13 green cubes, and 14 blue cubes. What is the sum of the
//           IDs of those games?
export function puzzle1(games: Array<string>, rMax: number, gMax: number, bMax: number) : number {
    return games.map(g => new Game(g))
                .filter(game => game.draws.filter(d => d.r <= rMax && d.g <= gMax && d.b <= bMax).length == game.draws.length)
                .reduce((sum, game) => sum + game.id, 0);
}

// As you continue your walk, the Elf poses a second question: in each game you played, what is the fewest number of cubes of each color that could have been in the bag
// to make the game possible?
// The power of a set of cubes is equal to the numbers of red, green, and blue cubes multiplied together.
//
// Puzzle == For each game, find the minimum set of cubes that must have been present. What is the sum of the power of these sets?
export function puzzle2(games: Array<string>) : number {
    return games.map(g => new Game(g))
                .map(g => Math.max(...g.draws.map(d => d.r)) * Math.max(...g.draws.map(d => d.g)) * Math.max(...g.draws.map(d => d.b)))
                .reduce((sum, prod) => sum + prod, 0);
}

class Draw {
    readonly r: number;
    readonly g: number;
    readonly b: number;

    constructor(draw: string) {
        this.r = 0;
        this.g = 0;
        this.b = 0;

        for(let color of draw.split(',')) {
            if(color.indexOf('red') > 0) {
                this.r = parseInt(color);
            }
            else if(color.indexOf('green') > 0) {
                this.g = parseInt(color);
            }
            else {
                this.b = parseInt(color);
            }
        }
    }
}

class Game {
    readonly id : number;
    readonly draws: Array<Draw>;

    constructor(game: string) {
        const gameSplit = game.split(':');
        this.id = parseInt(gameSplit[0].split(" ")[1]);

        this.draws = [];
        for(let draw of gameSplit[1].split(';')) {
            this.draws.push(new Draw(draw));
        }
    }
}
