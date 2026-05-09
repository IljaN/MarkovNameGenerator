using MarkovNameGenerator.Util;

namespace MarkovNameGenerator.NameGen;

/// <summary>
/// A procedural word generator that uses Markov chains built from a user-provided array of words.
/// This uses Katz's back-off model, which is an approach that uses high-order models.
/// It looks for the next letter based on the last "n" letters, backing down to lower order models when higher models fail.
/// This also uses a Dirichlet prior, which acts as an additive smoothing factor, introducing a chance for random letters to be picked.
/// </summary>
/// <remarks>
/// <para>See <see href="http://www.samcodes.co.uk/project/markov-namegen/"/></para>
/// <para>See <see href="https://en.wikipedia.org/wiki/Katz%27s_back-off_model">Wikipedia: Katz's back-off model</see></para>
/// <para>See <see href="https://en.wikipedia.org/wiki/Additive_smoothing">Wikipedia: Additive smoothing</see></para>
/// </remarks>
public class Generator
{
    /// <summary>
    /// The highest order model used by this generator.
    /// Generators own models of order 1 through order "n".
    /// Generators of order "n" look back up to "n" characters when choosing the next character.
    /// </summary>
    public uint Order { get; }

    /// <summary>
    /// Dirichlet prior, acts as an additive smoothing factor.
    /// The prior adds a constant probability that a random letter is picked from the alphabet when generating a new letter.
    /// </summary>
    public double Prior { get; }

    /// <summary>
    /// Whether to fall back to lower orders of models when a higher-order model fails to generate a letter.
    /// </summary>
    private readonly bool _backoff;

    /// <summary>
    /// The array of Markov models used by this generator, starting from highest order to lowest order.
    /// </summary>
    private readonly List<Model> _models;

    /// <summary>
    /// Creates a new procedural word Generator.
    /// </summary>
    /// <param name="data">Training data for the generator, an array of words.</param>
    /// <param name="order">Highest order of model to use - models of order 1 through order will be generated.</param>
    /// <param name="prior">The dirichlet prior/additive smoothing "randomness" factor.</param>
    /// <param name="backoff">Whether to fall back to lower order models when the highest order model fails to generate a letter.</param>
    public Generator(List<string> data, uint order, double prior, bool backoff)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentOutOfRangeException.ThrowIfLessThan(order, 1u);
        ArgumentOutOfRangeException.ThrowIfNegative(prior);

        Order = order;
        Prior = prior;
        _backoff = backoff;

        // Identify and sort the alphabet used in the training data
        var letters = new ArraySet<string>();
        foreach (var word in data)
        {
            for (int i = 0; i < word.Length; i++)
            {
                letters.Add(word[i].ToString());
            }
        }

        letters.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        var domain = letters.ToArray().ToList();
        domain.Insert(0, "#");

        // Create models
        _models = new List<Model>();
        if (_backoff)
        {
            for (uint i = 0; i < order; i++)
            {
                // From highest to lowest order
                _models.Add(new Model(new List<string>(data), order - i, prior, domain));
            }
        }
        else
        {
            _models.Add(new Model(new List<string>(data), order, prior, domain));
        }
    }

    /// <summary>
    /// Generates a word.
    /// </summary>
    /// <returns>The generated word (without the boundary markers).</returns>
    public string Generate()
    {
        return Generate("");
    }

    /// <summary>
    /// Generates a word starting with the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix to start the word with.</param>
    /// <returns>The generated word (without the boundary markers).</returns>
    public string Generate(string prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        string word = "#".Repeat((int)Order);

        // Add the prefix to the word
        foreach (char c in prefix)
        {
            word += c.ToString();
        }

        string? letter = GetLetter(word);
        while (letter != "#" && letter != null)
        {
            word += letter;
            letter = GetLetter(word);
        }

        return word.Replace("#", "");
    }

    /// <summary>
    /// Generates the next letter in a word.
    /// </summary>
    /// <param name="word">The context the models will use for generating the next letter.</param>
    /// <returns>The generated letter, or null if no model could generate one.</returns>
    private string? GetLetter(string word)
    {
        ArgumentException.ThrowIfNullOrEmpty(word);

        string? letter = null;
        string context = word.Substring(word.Length - (int)Order, (int)Order);

        foreach (var model in _models)
        {
            letter = model.Generate(context);

            if (letter == null || letter == "#")
            {
                context = context[1..];
            }
            else
            {
                break;
            }
        }

        return letter;
    }
}
