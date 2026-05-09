using MarkovNameGenerator.Util;

namespace MarkovNameGenerator.NameGen;

/// <summary>
/// A Markov model built using string training data.
/// </summary>
internal class Model
{
    /// <summary>
    /// The order of the model i.e. how many characters this model looks back.
    /// </summary>
    private readonly uint _order;

    /// <summary>
    /// Dirichlet prior, like additive smoothing, increases the probability of any item being picked.
    /// </summary>
    private readonly double _prior;

    /// <summary>
    /// The alphabet of the training data.
    /// </summary>
    private readonly List<string> _alphabet;

    /// <summary>
    /// The observations.
    /// </summary>
    private readonly Dictionary<string, List<string>> _observations;

    /// <summary>
    /// The Markov chains.
    /// </summary>
    private Dictionary<string, List<double>> _chains;

    /// <summary>
    /// Creates a new Markov model.
    /// </summary>
    /// <param name="data">The training data for the model, an array of words.</param>
    /// <param name="order">The order of model to use, models of order "n" will look back "n" characters within their context when determining the next letter.</param>
    /// <param name="prior">The dirichlet prior, an additive smoothing "randomness" factor. Must be in the range 0 to 1.</param>
    /// <param name="alphabet">The alphabet of the training data i.e. the set of unique symbols used in the training data.</param>
    public Model(List<string> data, uint order, double prior, List<string> alphabet)
    {
        ArgumentNullException.ThrowIfNull(alphabet);
        ArgumentNullException.ThrowIfNull(data);
        ArgumentOutOfRangeException.ThrowIfZero(alphabet.Count);
        ArgumentOutOfRangeException.ThrowIfZero(data.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(prior);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(prior, 1.0);

        _order = order;
        _prior = prior;
        _alphabet = alphabet;
        _observations = new Dictionary<string, List<string>>();
        _chains = new Dictionary<string, List<double>>();

        Train(data);
        BuildChains();
    }

    /// <summary>
    /// Attempts to generate the next letter in the word given the context (the previous "order" letters).
    /// </summary>
    /// <param name="context">The previous "order" letters in the word.</param>
    /// <returns>The next letter, or null if no chain exists for the given context.</returns>
    public string? Generate(string context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!_chains.TryGetValue(context, out var chain))
        {
            return null;
        }

        if (chain.Count == 0)
        {
            return null;
        }

        return _alphabet[SelectIndex(chain)];
    }

    /// <summary>
    /// Retrains the model on the newly supplied data, regenerating the Markov chains.
    /// </summary>
    /// <param name="data">The new training data.</param>
    public void Retrain(List<string> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        Train(data);
        BuildChains();
    }

    /// <summary>
    /// Trains the model on the given training data.
    /// </summary>
    /// <param name="data">The training data.</param>
    private void Train(List<string> data)
    {
        // Process each word in reverse (using pop-like behavior)
        for (int idx = data.Count - 1; idx >= 0; idx--)
        {
            string d = data[idx];
            d = "#".Repeat((int)_order) + d + "#";

            for (int i = 0; i < d.Length - _order; i++)
            {
                string key = d.Substring(i, (int)_order);

                if (!_observations.TryGetValue(key, out var value))
                {
                    value = new List<string>();
                    _observations[key] = value;
                }

                value.Add(d[i + (int)_order].ToString());
            }
        }
    }

    /// <summary>
    /// Builds the Markov chains for the model.
    /// </summary>
    private void BuildChains()
    {
        _chains = new Dictionary<string, List<double>>();

        foreach (var context in _observations.Keys)
        {
            foreach (var prediction in _alphabet)
            {
                if (!_chains.TryGetValue(context, out var value))
                {
                    value = new List<double>();
                    _chains[context] = value;
                }

                value.Add(_prior + CountMatches(_observations[context], prediction));
            }
        }
    }

    /// <summary>
    /// Counts how many times a value appears in an array.
    /// </summary>
    private static int CountMatches(List<string>? arr, string v)
    {
        if (arr == null)
        {
            return 0;
        }

        int count = 0;
        foreach (var s in arr)
        {
            if (s == v)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Selects a random index from a chain based on weighted probabilities.
    /// </summary>
    private static int SelectIndex(List<double> chain)
    {
        var totals = new List<double>(chain.Count);
        double accumulator = 0;

        foreach (var weight in chain)
        {
            accumulator += weight;
            totals.Add(accumulator);
        }

        double rand = Random.Shared.NextDouble() * accumulator;

        for (int i = 0; i < totals.Count; i++)
        {
            if (rand < totals[i])
            {
                return i;
            }
        }

        return 0;
    }
}
