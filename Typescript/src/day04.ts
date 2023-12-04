// Each scratchcard has two lists of numbers separated by a vertical bar (|): a list of winning numbers and then a list of
// numbers you have. You organize the information into a table (your puzzle input). As far as the Elf has been able to figure
// out, you have to figure out which of the numbers you have appear in the list of winning numbers. The first match makes the
// card worth one point and each match after the first doubles the point value of that card.
//
// Puzzle == How many points are they worth in total?
export function puzzle1(lines: Array<string>) : number {
    return lines.map(line => parseLine(line))
                .map(card => drawValue(card.draw.filter(d => card.winning.indexOf(d) >= 0).length))
                .reduce((sum, card) => sum + card, 0);
}

// The rules have actually been printed on the back of every card this whole time. There's no such thing as "points". Instead,
// scratchcards only cause you to win more scratchcards equal to the number of winning numbers you have.
// Specifically, you win copies of the scratchcards below the winning card equal to the number of matches. So, if card 10 were
// to have 5 matching numbers, you would win one copy each of cards 11, 12, 13, 14, and 15.
// Copies of scratchcards are scored like normal scratchcards and have the same card number as the card they copied. So, if you
// win a copy of card 10 and it has 5 matching numbers, it would then win a copy of the same cards that the original card 10
// won: cards 11, 12, 13, 14, and 15. This process repeats until none of the copies cause you to win any more cards. (Cards
// will never make you copy a card past the end of the table.)
// Process all of the original and copied scratchcards until no more scratchcards are won. Including the original set of scratchcards.
//
// Puzzle == How many total scratchcards do you end up with?
export function puzzle2(lines: Array<string>) : number {
    let wins  = lines.map(line => parseLine(line))
                     .map(card => card.draw.filter(d => card.winning.indexOf(d) >= 0).length);
    let stack = Array<number>(lines.length).fill(1);

    for(let i = 1; i < stack.length; i++) {
        for(let win = 0; win < wins[i - 1]; win++) {
            stack[i + win] += stack[i - 1];
        }
    }

    return stack.reduce((sum, s) => sum + s, 0);
}

function parseLine(line: string) : {winning: Array<number>, draw: Array<number>} {
    let split = line.split(': ')[1].split(' | ');
    return { winning: split[0].split(' ').filter(n => n != ' ' && n != '').map(n => parseInt(n.trim())),
                draw:    split[1].split(' ').filter(n => n != ' ' && n != '').map(n => parseInt(n.trim())) };
}

function drawValue(pow: number) : number {
    return pow === 0 ? 0 : (pow === 1 ? 1 : 2 << (pow - 2));
}
