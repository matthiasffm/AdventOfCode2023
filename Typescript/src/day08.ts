import { lcm } from '../src/euclid';

// The documents contains a list of left/right instructions, and the rest of the documents seem to describe some kind of network of labeled nodes. It seems like
// you're meant to use the left/right instructions to navigate the network. After examining the maps for a bit, two nodes stick out: AAA and ZZZ. You feel like
// AAA is where you are now, and you have to follow the left/right instructions until you reach ZZZ. Starting at AAA, follow the left/right instructions.
//
// Puzzle ==  How many steps are required to reach ZZZ?
export function puzzle1(lines: Array<string>) : number {
    const instructions = lines[0];
    const nodeMap      = createNodeMap(lines.slice(2));

    let node = 'AAA';
    let step = 0;

    while(node != 'ZZZ') {
        node = instructions[step % instructions.length] == 'L' ? nodeMap.get(node)!.left : nodeMap.get(node)!.right;
        step++;
    }

    return step;
}

// After examining the maps a bit longer, your attention is drawn to a curious fact: the number of nodes with names ending in A is equal to the number ending in Z! If
// you were a ghost, you'd probably just start at every node that ends with A and follow all of the paths at the same time until they all simultaneously end up at nodes
// that end with Z. Simultaneously start on every node that ends with A.
//
// Puzzle == How many steps does it take before you're only on nodes that end with Z?
export function puzzle2(lines: Array<string>) : number {

    const instructions = lines[0];
    const nodeMap      = createNodeMap(lines.slice(2));

    // (verified) assumption: all interations have the same length
    let current = [...nodeMap.keys()].filter(k => k.slice(-1) == 'A')
    let foundZ  = new Array<number>(current.length).fill(0);

    let step = 0;

    do {
        for(let i = 0; i < current.length; i++) {
            // detect period
            // data is special, first occurence of Z is also period length and there is only one period for every simultaneous path
            if(current[i].slice(-1) == 'Z' && foundZ[i] == 0) {
                foundZ[i] = step;
            }

            // move to next iteration like in puzzle1
            current[i] = instructions[step % instructions.length] == 'L' ? nodeMap.get(current[i])!.left : nodeMap.get(current[i])!.right;
        }

        step++;
    } while(foundZ.some(p => p == 0));

    return lcm(foundZ);
}

type Node = {
    left:  string;
    right: string;
}

// creates the map of the network from input data
// keys are the nodes and values are the left and right movements
function createNodeMap(lines: string[]) : Map<string, Node> {
    return lines.reduce((map, line) => {
        map.set(line.substring(0, 3), ({ left: line.substring(7, 10), right: line.substring(12, 15) }));
        return map;
    }, new Map<string, Node>);
}
