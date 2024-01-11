namespace AdventOfCode2023;// matthiasffm.Common.Math;

public static class Euclid
{
    public static long Lcm(long a, long b) => a * b / matthiasffm.Common.Math.Euclid.Gcd(a, b);
    public static int  Lcm(int a, int  b)  => a * b / matthiasffm.Common.Math.Euclid.Gcd(a, b);

    public static long Lcm(IEnumerable<long> numbers) => numbers.Aggregate(1L, (lcm, number) => Lcm(lcm, number));
    public static int  Lcm(IEnumerable<int> numbers)  => numbers.Aggregate(1, (lcm, number) => Lcm(lcm, number));
}
