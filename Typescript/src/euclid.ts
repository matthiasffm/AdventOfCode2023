export function gcd(a: number, ...rest: number[]) : number;
export function gcd(numbers: number[]) : number;
export function gcd(a: number | number[], ...rest: number[]) : number {
    let numbers: number[] = (typeof a === "number") ? [a] : a;
    if(rest !== undefined && rest.length > 0) {
        numbers = numbers.concat(rest);
    }

    if(numbers.length == 0) {
        throw new Error("gcd needs at least one input value");
    }

    return numbers.reduce((gcd, next) => gcdInternal(gcd, next), 1);
}

export function lcm(a: number, ...rest: number[]) : number;
export function lcm(numbers: number[]) : number;
export function lcm(a: number | number[], ...rest: number[]) : number {
    let numbers: number[] = (typeof a === "number") ? [a] : a;
    if(rest !== undefined && rest.length > 0) {
        numbers = numbers.concat(rest);
    }

    if(numbers.length == 0) {
        throw new Error("lcm needs at least one input value");
    }

    return numbers.reduce((lcm, next) => lcmInternal(lcm, next), 1);
}

function gcdInternal(a: number, b: number): number {
    if(a == 0)
    {
        return Math.abs(b);
    }

    while(b != 0)
    {
        var h = a % b;
        a = b;
        b = h;
    }

    return Math.abs(a);
}

function lcmInternal(a: number, b: number): number {
    return a * b / gcdInternal(a, b);
}
