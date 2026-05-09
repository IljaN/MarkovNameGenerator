using System.Runtime.CompilerServices;
using System.Text;

namespace MarkovNameGenerator.Util;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Creates a reversed copy of the given string.
    /// </summary>
    /// <param name="str">The string to copy.</param>
    /// <returns>A reversed copy of the given string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Reverse(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        var chars = str.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>
    /// Repeats the given string the specified number of times.
    /// </summary>
    /// <param name="str">The string to repeat.</param>
    /// <param name="times">The number of times to repeat the string.</param>
    /// <returns>The repeated string.</returns>
    /// <example>Repeat("foo", 3) => "foofoofoo"</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this string str, int times)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentOutOfRangeException.ThrowIfLessThan(times, 1);

        if (times == 1) return str;

        var sb = new StringBuilder(str.Length * times);
        for (int i = 0; i < times; i++)
        {
            sb.Append(str);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Creates a capitalized version of a string (first letter uppercase).
    /// </summary>
    /// <param name="str">The string to capitalize.</param>
    /// <returns>A capitalized copy of the string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Capitalize(this string str)
    {
        ArgumentException.ThrowIfNullOrEmpty(str);

        if (str.Length == 1)
            return str.ToUpperInvariant();

        return char.ToUpperInvariant(str[0]) + str[1..];
    }

    /// <summary>
    /// Creates a lowercased version of a string (first letter lowercase).
    /// </summary>
    /// <param name="str">The string to lowercase.</param>
    /// <returns>A lowercased copy of the string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Lowercase(this string str)
    {
        ArgumentException.ThrowIfNullOrEmpty(str);

        if (str.Length == 1)
            return str.ToLowerInvariant();

        return char.ToLowerInvariant(str[0]) + str[1..];
    }

    /// <summary>
    /// Capitalizes the first letter of every word (separated by spaces) in the string.
    /// </summary>
    /// <param name="str">The string containing words to capitalize.</param>
    /// <returns>A string with each word capitalized.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CapitalizeWords(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        var words = str.Split(' ');
        var sb = new StringBuilder(str.Length);

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                sb.Append(Capitalize(words[i]));
            }
            if (i < words.Length - 1)
            {
                sb.Append(' ');
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Lowercases the first letter of every word (separated by spaces) in the string.
    /// </summary>
    /// <param name="str">The string containing words to lowercase.</param>
    /// <returns>A string with each word's first letter lowercased.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LowercaseWords(this string str)
    {
        ArgumentNullException.ThrowIfNull(str);

        var words = str.Split(' ');
        var sb = new StringBuilder(str.Length);

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                sb.Append(Lowercase(words[i]));
            }
            if (i < words.Length - 1)
            {
                sb.Append(' ');
            }
        }

        return sb.ToString();
    }
}
