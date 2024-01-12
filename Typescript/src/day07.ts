type CamelCard = {
    hand: PokerHand;
    bid:  number;
}

// In Camel Cards, you get a list of hands, and your goal is to order them based on the strength of each hand. To play the game, you are given a list of hands
// and their corresponding bid (your puzzle input).
// A hand consists of five cards labeled one of A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2. The relative strength of each card follows this order, where A is the highest and
// 2 is the lowest.
// Every hand is exactly one type. From strongest to weakest, they are:
// - Five of a kind, where all five cards have the same label: AAAAA
// - Four of a kind, where four cards have the same label and one card has a different label: AA8AA
// - Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
// - Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
// - Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
// - One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
// - High card, where all cards' labels are distinct: 23456
// Hands are primarily ordered based on type; for example, every full house is stronger than any three of a kind.
// If two hands have the same type, a second ordering rule takes effect. Start by comparing the first card in each hand. If these cards are different, the hand with the
// stronger first card is considered stronger. If the first card in each hand have the same label, however, then move on to considering the second card in each hand. If
// they differ, the hand with the higher second card wins; otherwise, continue with the third card in each hand, then the fourth, then the fifth.
// Each hand is followed by its bid amount. Each hand wins an amount equal to its bid multiplied by its rank, where the weakest hand gets rank 1, the second-weakest
// hand gets rank 2, and so on up to the strongest hand.
// Now, you can determine the total winnings of this set of hands by adding up the result of multiplying each hand's bid with its rank.
//
// Puzzle == Find the rank of every hand in your set. What are the total winnings?
export function puzzle1(lines: Array<string>) : number {
    const cards = parsePokerHands(lines);

    return cards.sort((a, b) => a.hand.compare(b.hand))
                .map((card, i) => (i + 1) * card.bid)
                .reduce((sum, bid) => sum + bid, 0);
}

function parsePokerHands(lines: string[]) : CamelCard[] {
    return lines.map(l => ({ hand: new PokerHand(mapCards(l.substring(0, 5))),
                             bid:  parseInt(l.substring(6)) }));
}

// To make things a little more interesting, the Elf introduces one additional rule. Now, J cards are jokers - wildcards that can act like whatever card would make the
// hand the strongest type possible.
// To balance this, J cards are now the weakest individual cards, weaker even than 2. The other cards stay in the same order: A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, J.
// Joker cards can pretend to be whatever card is best for the purpose of determining hand type; for example, QJJQ2 is now considered four of a kind. However, for the
// purpose of breaking ties between two hands of the same type, J is always treated as J (aka the weakest card), not the card it's pretending to be.
//
// Puzzle == Find the rank of every hand in your set. What are the total winnings?
export function puzzle2(lines: Array<string>) : number {
    const cards = parseJokerHands(lines);

    return cards.sort((a, b) => a.hand.compare(b.hand))
                .map((card, i) => (i + 1) * card.bid)
                .reduce((sum, bid) => sum + bid, 0);
}

function parseJokerHands(lines: string[]) : CamelCard[] {
    return lines.map(l => ({ hand: new JokerHand(mapCards(l.substring(0, 5))),
                             bid:  parseInt(l.substring(6)) }));
}

function mapCards(hand: string): string {
    return hand.split('')
               .map(c => mapCard(c))
               .join('');
}

// for basic string comparison to work with poker cards they have to be mapped to characters with the correct ordering
function mapCard(card: string): string {
    switch(card) {
        case 'T': return 'A';
        case 'J': return 'B';
        case 'Q': return 'C';
        case 'K': return 'D';
        case 'A': return 'E';
        default:  return card;
    }
}

enum HandType {
    HighCard        = 1,
    OnePair         = 2,
    TwoPair         = 3,
    ThreeOfAKind    = 4,
    FullHouse       = 5,
    FourOfAKind     = 6,
    FiveOfAKind     = 7,
};

class PokerHand {
    private readonly _type:  HandType;
    private readonly _cards: string;

    get type() {
        return this._type;
    }

    get cards() {
        return this._cards;
    }

    constructor(cards: string) {
        this._cards = cards;
        this._type  = this.calcHandType(cards);
    }

    protected calcHandType(cards: string): HandType {
        const groups = this._cards.split('')
                                  .reduce((groups, item) => {
                                       groups.set(item, 1+ (groups.get(item) ?? 0));
                                       return groups;
                                   }, new Map<string, number>);
        const occurences = Array.from(groups.values()).sort().reverse();

        if(occurences[0] == 5) {
            return HandType.FiveOfAKind;
        } else if(occurences[0] == 4) {
            return HandType.FourOfAKind;
        } else if(occurences[0] == 3 && occurences[1] == 2) {
            return HandType.FullHouse;
        } else if(occurences[0] == 3) {
            return HandType.ThreeOfAKind;
        } else if(occurences[0] == 2 && occurences[1] == 2) {
            return HandType.TwoPair;
        } else if(occurences[0] == 2) {
            return HandType.OnePair;
        } else {
            return HandType.HighCard;
        }
    }

    compare(hand2: PokerHand): number {
        if(this.type == hand2.type) {
            return this.cards.localeCompare(hand2.cards);
        } else {
            return this.type - hand2.type;
        }
    }
    
}

class JokerHand extends PokerHand {

    constructor(cards: string) {
        // maps the joker card (already mapped to 'B' in base class constructor) to the weakest card (here '1')
        // in this way the joker card is in front of all other cards for sorting and comparing
        super(cards.replaceAll('B', '1'));
    }

    // overwrite base class ranking considering the number of joker cards
    protected override calcHandType(cards: string): HandType {
        const handType   = super.calcHandType(cards);
        const nbmrJokers = cards.split('').filter(c => c == '1').length;

        if(handType == HandType.FourOfAKind && (nbmrJokers == 1 || nbmrJokers == 4)) {
            return HandType.FiveOfAKind;
        } else if(handType == HandType.FullHouse && (nbmrJokers == 2 || nbmrJokers == 3)) {
            return HandType.FiveOfAKind;
        } else if(handType == HandType.ThreeOfAKind && (nbmrJokers == 1 || nbmrJokers == 3)) {
            return HandType.FourOfAKind;
        } else if(handType == HandType.TwoPair && nbmrJokers == 2) {
            return HandType.FourOfAKind;
        } else if(handType == HandType.TwoPair && nbmrJokers == 1) {
            return HandType.FullHouse;
        } else if(handType == HandType.OnePair && (nbmrJokers == 1 || nbmrJokers == 2)) {
            return HandType.ThreeOfAKind;
        } else if(handType == HandType.HighCard && nbmrJokers == 1) {
            return HandType.OnePair;
        } else {
            return handType;
        }
    }
}
