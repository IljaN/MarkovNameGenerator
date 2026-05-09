using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MarkovNameGenerator.NameGen;

/// <summary>
/// An example name generator that builds upon the Generator class.
/// This should be sufficient for most simple name generation scenarios.
/// For complex name generators, modifying the Generator class to your specifications
/// may be more appropriate or performant than extending this approach.
/// </summary>
public class NameGenerator
{
    /// <summary>
    /// The underlying word generator.
    /// </summary>
    private readonly Generator _generator;

    /// <summary>
    /// Creates a new procedural name generator.
    /// </summary>
    /// <param name="data">Training data for the generator, an array of words.</param>
    /// <param name="order">Highest order of model to use - models 1 to order will be generated.</param>
    /// <param name="prior">The dirichlet prior/additive smoothing "randomness" factor.</param>
    /// <param name="backoff">Whether to fall back to lower order models when the highest order model fails to generate a letter (defaults to false).</param>
    public NameGenerator(List<string> data, int order, double prior, bool backoff = false)
    {
        _generator = new Generator(data, (uint)order, prior, backoff);
    }

    /// <summary>
    /// Creates a word within the given constraints.
    /// If the generated word does not meet the constraints, this returns null.
    /// </summary>
    /// <param name="minLength">The minimum length of the word.</param>
    /// <param name="maxLength">The maximum length of the word.</param>
    /// <param name="startsWith">The text the word must start with (case-insensitive). Seeds the generation for efficiency.</param>
    /// <param name="endsWith">The text the word must end with (case-insensitive).</param>
    /// <param name="includes">The text the word must include (case-insensitive).</param>
    /// <param name="excludes">The text the word must exclude (case-insensitive).</param>
    /// <param name="regexMatch">Optional regex pattern the word must match (case-insensitive).</param>
    /// <returns>A word that meets the specified constraints, or null if the generated word did not meet the constraints.</returns>
    public string? GenerateName(
        int minLength,
        int maxLength,
        string startsWith = "",
        string endsWith = "",
        string includes = "",
        string excludes = "",
        Regex? regexMatch = null)
    {
        startsWith ??= "";
        endsWith ??= "";
        includes ??= "";
        excludes ??= "";

        // Seed the generator with the prefix for efficiency
        string name = _generator.Generate(startsWith.ToLowerInvariant());

        if (name.Length >= minLength && name.Length <= maxLength
            && name.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase)
            && name.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase)
            && (includes.Length == 0 || name.Contains(includes, StringComparison.OrdinalIgnoreCase))
            && (excludes.Length == 0 || !name.Contains(excludes, StringComparison.OrdinalIgnoreCase))
            && (regexMatch == null || regexMatch.IsMatch(name)))
        {
            return name;
        }

        return null;
    }

    /// <summary>
    /// Attempts to generate "n" unique names that meet the given constraints within an allotted time.
    /// </summary>
    /// <param name="n">The number of names to generate.</param>
    /// <param name="minLength">The minimum length of the word.</param>
    /// <param name="maxLength">The maximum length of the word.</param>
    /// <param name="startsWith">The text the word must start with (case-insensitive).</param>
    /// <param name="endsWith">The text the word must end with (case-insensitive).</param>
    /// <param name="includes">The text the word must include (case-insensitive).</param>
    /// <param name="excludes">The text the word must exclude (case-insensitive).</param>
    /// <param name="maxTimePerName">The maximum time in seconds to spend generating each name (default: 0.02s = 20ms).</param>
    /// <param name="regexMatch">Optional regex pattern the word must match (case-insensitive).</param>
    /// <returns>A list of unique words that meet the specified constraints. May contain fewer than n items if time limit was reached.</returns>
    public List<string> GenerateNames(
        int n,
        int minLength,
        int maxLength,
        string startsWith = "",
        string endsWith = "",
        string includes = "",
        string excludes = "",
        double maxTimePerName = 0.02,
        Regex? regexMatch = null)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var stopwatch = Stopwatch.StartNew();
        var maxTime = TimeSpan.FromSeconds(maxTimePerName * n);

        while (names.Count < n && stopwatch.Elapsed < maxTime)
        {
            var name = GenerateName(minLength, maxLength, startsWith, endsWith, includes, excludes, regexMatch);
            if (name != null)
            {
                names.Add(name);
            }
        }

        return names.ToList();
    }
}
