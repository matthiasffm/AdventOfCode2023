namespace AdventOfCode2023;// matthiasffm.Common.Math;

public static class MathExtensions
{
    public static T Clamp<T>(this T val, T min) where T : IComparable<T>
        => val.CompareTo(min) < 0 ? min : val;

    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        => val.CompareTo(min) < 0 ? min : (val.CompareTo(max) > 0 ? max : val);

    public static int Pow2(int pow) => pow.Clamp(0) == 0 ? 1 : 2 << (pow - 1);
}
