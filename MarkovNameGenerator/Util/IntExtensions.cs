using System.Runtime.CompilerServices;

namespace MarkovNameGenerator.Util;

/// <summary>
/// Extension methods for integers.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Clamps a value between min and max inclusive.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The value clamped to the range [min, max].</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Returns the smaller of two integers.
    /// </summary>
    /// <param name="a">The first integer.</param>
    /// <param name="b">The second integer.</param>
    /// <returns>The smaller of the two integers.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }
}
