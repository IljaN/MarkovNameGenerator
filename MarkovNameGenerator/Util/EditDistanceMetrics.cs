using System.Runtime.CompilerServices;

namespace MarkovNameGenerator.Util;

/// <summary>
/// Algorithms that calculate the edit distances between strings.
/// </summary>
public static class EditDistanceMetrics
{
    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// The Levenshtein distance is the number of insertions, deletions and replacements
    /// needed to transform a source string into a target string.
    /// This is a fast iterative method that doesn't create a whole distance table up front.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>The number of single-character edits needed to transform the source into the target.</returns>
    public static int Levenshtein(string source, string target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        int slen = source.Length;
        int tlen = target.Length;

        if (slen == 0) return tlen;
        if (tlen == 0) return slen;

        int[] costs = new int[tlen + 1];
        for (int i = 0; i < costs.Length; i++)
        {
            costs[i] = i;
        }

        for (int s = 0; s < source.Length; s++)
        {
            costs[0] = s + 1;
            int corner = s;

            for (int t = 0; t < target.Length; t++)
            {
                int upper = costs[t + 1];

                if (source[s] == target[t])
                {
                    costs[t + 1] = corner;
                }
                else
                {
                    int tc = upper < corner ? upper : corner;
                    costs[t + 1] = (costs[t] < tc ? costs[t] : tc) + 1;
                }

                corner = upper;
            }
        }

        return costs[^1];
    }

    /// <summary>
    /// Calculates the Damerau-Levenshtein distance between two strings.
    /// The Damerau-Levenshtein distance is the number of insertions, deletions,
    /// replacements and transpositions needed to transform a source string into a target string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>The number of character edits needed to transform the source into the target.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DamerauLevenshtein(string source, string target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        var table = DamerauLevenshteinMatrix(source, target, true);
        return table[^1];
    }

    /// <summary>
    /// Calculates the Levenshtein or Damerau-Levenshtein distance table for the edit operations
    /// (insertions, deletions, replacements and optionally transpositions) needed to transform
    /// a source string into a target string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <param name="enableTranspositions">Whether to allow adjacent symbols to be transposed (swapped).</param>
    /// <returns>The distance table, which can be queried to obtain sequences of operations to transform the source to the target.</returns>
    public static int[] DamerauLevenshteinMatrix(string source, string target, bool enableTranspositions = true)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        int w = source.Length;
        int h = target.Length;

        if (w == 0 || h == 0)
        {
            return Array.Empty<int>();
        }

        w += 1;
        h += 1;
        int[] costs = new int[w * h];

        // Initialize first row and column
        for (int i = 0; i < w; i++)
        {
            costs[i] = i;
        }
        for (int j = 1; j < h; j++)
        {
            costs[j * w] = j;
        }

        for (int x = 1; x < w; x++)
        {
            for (int y = 1; y < h; y++)
            {
                int cost = source[x - 1] == target[y - 1] ? 0 : 1;

                // Deletion, insertion, substitution
                costs[x + y * w] = IntExtensions.Min(
                    costs[(x - 1) + (y * w)] + 1,
                    IntExtensions.Min(
                        costs[x + ((y - 1) * w)] + 1,
                        costs[(x - 1) + ((y - 1) * w)] + cost
                    )
                );

                // Transposition
                if (enableTranspositions && x > 1 && y > 1
                    && source[x - 1] == target[y - 2]
                    && source[x - 2] == target[y - 1])
                {
                    costs[x + y * w] = IntExtensions.Min(
                        costs[x + y * w],
                        costs[x - 2 + ((y - 2) * w)] + cost
                    );
                }
            }
        }

        return costs;
    }
}
