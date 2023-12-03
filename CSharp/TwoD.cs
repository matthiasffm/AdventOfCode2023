namespace AdventOfCode2023;

public static class TwoD
{
    /// <summary>
    /// Offset coordinates of all 4 direct neighbors in a 2D grid
    /// </summary>
    /// <remarks>
    /// Neighbors x for point O
    ///   x
    ///  xOx
    ///   x
    public static readonly IEnumerable<(int col, int row)> Neighbors4 =
        [ (0, -1), (-1, 0), (1, 0), (0, 1) ];

    /// <summary>
    /// Offset coordinates of all 8 direct neighbors in a 2D grid
    /// </summary>
    /// Neighbors x for point O
    ///  xxx
    ///  xOx
    ///  xxx
    public static readonly IEnumerable<(int col, int row)> Neighbors8 =
        [ (-1, -1), (0, -1), (+1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) ];

}
