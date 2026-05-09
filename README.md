# Markov Name Generator

A C#/.NET 10 Port of [markov-namgen-lib](https://github.com/Tw1ddle/markov-namegen-lib) originally written in Haxe by [samcodes](https://lib.haxe.org/u/samcodes). The
port was created using AI.

This library generates procedural names and words using Markov chains with Katz's back-off model and Dirichlet prior smoothing.

## Features

- **Markov Chain Generation**: Uses statistical modeling to generate realistic-sounding names based on training data
- **Katz Back-off Model**: Falls back to lower-order models when higher-order models fail, preventing dead-ends
- **Dirichlet Prior Smoothing**: Adds controlled randomness to prevent overfitting
- **Flexible Filtering**: Filter generated names by length, prefix, suffix, substring inclusion/exclusion, and regex patterns
- **Modern C# (.NET 8)**: Takes advantage of the latest C# features and performance improvements
- **Well-Tested**: Comprehensive unit tests ensure reliability
- **Easy to Use**: Simple, intuitive API

## Usage

### Run Example

```bash
dotnet run Example.cs
```

### Building from Source

```bash
cd csharp
dotnet build
```

### Running Tests

```bash
dotnet test
```

## Quick Start

```csharp
using MarkovNameGenerator.NameGen;

// Load training data (one word per line)
var trainingData = File.ReadAllLines("data/animals.txt").ToList();

// Create generator
var generator = new NameGenerator(
    data: trainingData,
    order: 3,          // Look back 3 characters
    prior: 0.01,       // Small randomness factor
    backoff: false     // Don't use Katz back-off
);

// Generate a single name with constraints
var name = generator.GenerateName(
    minLength: 4,
    maxLength: 10,
    startsWith: "",
    endsWith: "",
    includes: "",
    excludes: ""
);

Console.WriteLine($"Generated name: {name}");

// Generate multiple names
var names = generator.GenerateNames(
    n: 10,
    minLength: 4,
    maxLength: 10
);

foreach (var n in names)
{
    Console.WriteLine(n);
}
```

## API Reference

### NameGenerator Class

The main entry point for name generation.

#### Constructor

```csharp
public NameGenerator(
    List<string> data,    // Training data (array of words)
    int order,            // Order of the model (1-9, typically 2-4)
    double prior,         // Dirichlet prior (0.0-1.0, typically 0.001-0.05)
    bool backoff = false  // Enable Katz back-off
)
```

**Parameters:**
- `data`: Array of training words. More data = better results
- `order`: How many previous characters to consider (higher = more similar to training data)
- `prior`: Randomness factor (higher = more random, lower = more like training data)
- `backoff`: Enable falling back to lower-order models when generation fails

#### Methods

##### GenerateName

Generates a single name with optional constraints.

```csharp
public string? GenerateName(
    int minLength,
    int maxLength,
    string startsWith = "",
    string endsWith = "",
    string includes = "",
    string excludes = "",
    Regex? regexMatch = null
)
```

Returns `null` if the generated name doesn't meet constraints.

##### GenerateNames

Generates multiple names within a time limit.

```csharp
public List<string> GenerateNames(
    int n,                          // Number of names to generate
    int minLength,
    int maxLength,
    string startsWith = "",
    string endsWith = "",
    string includes = "",
    string excludes = "",
    double maxTimePerName = 0.02,   // Time limit in seconds
    Regex? regexMatch = null
)
```

Returns up to `n` names that meet the constraints, potentially fewer if time limit is reached.

## Parameters Explained

### Order

The "order" parameter controls how many previous characters the model looks at when predicting the next character:

- **Order 1**: Only looks at the previous character (very random)
- **Order 2-3**: Good balance (recommended for most use cases)
- **Order 4+**: More similar to training data (less creative)

### Prior (Dirichlet Smoothing)

The "prior" parameter adds randomness:

- **0.0**: Completely deterministic (will only use patterns from training data)
- **0.001-0.01**: Slight randomness (recommended)
- **0.05+**: High randomness (very creative but less realistic)

### Backoff

When enabled, the generator uses multiple models of different orders and falls back to lower orders when higher orders fail:

- **false**: Single model (faster, recommended for most cases)
- **true**: Multiple models with fallback (better for small training datasets)

## Utility Classes

The library also includes useful utility classes:

### EditDistanceMetrics

Calculate similarity between strings:

```csharp
using MarkovNameGenerator.Util;

int distance = EditDistanceMetrics.Levenshtein("kitten", "sitting");  // Returns 3
int dlDistance = EditDistanceMetrics.DamerauLevenshtein("ab", "ba");  // Returns 1 (transposition)
```

### PrefixTrie

Fast string lookup and duplicate detection:

```csharp
var trie = new PrefixTrie();
trie.Insert("cat");
trie.Insert("car");
bool exists = trie.Find("cat");  // Returns true
```

### StringExtensions

Helpful string operations:

```csharp
using MarkovNameGenerator.Util;

string reversed = "hello".Reverse();      // "olleh"
string repeated = "hi".Repeat(3);         // "hihihi"
string capitalized = "hello".Capitalize(); // "Hello"
```

## Examples

### Fantasy Character Names

```csharp
var names = new List<string>
{
    "aragorn", "gandalf", "frodo", "legolas", "gimli",
    "boromir", "samwise", "pippin", "merry"
};

var generator = new NameGenerator(names, order: 2, prior: 0.01);

for (int i = 0; i < 10; i++)
{
    var name = generator.GenerateName(minLength: 4, maxLength: 10);
    if (name != null)
        Console.WriteLine(name);
}
```

### Animal-inspired Names

```csharp
// Load from file
var animals = File.ReadAllLines("data/animals.txt").ToList();
var generator = new NameGenerator(animals, order: 3, prior: 0.005);

// Generate names starting with 'dr'
var dragonNames = generator.GenerateNames(
    n: 5,
    minLength: 5,
    maxLength: 12,
    startsWith: "dr"
);
```

### Business Names

```csharp
var companies = new List<string>
{
    "microsoft", "google", "amazon", "apple", "meta",
    "tesla", "netflix", "spotify", "twitter", "uber"
};

var generator = new NameGenerator(companies, order: 3, prior: 0.02);

var businessName = generator.GenerateName(
    minLength: 6,
    maxLength: 10,
    endsWith: "",
    regexMatch: new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase)
);
```

## Training Data

The library works best with:
- **50+ training words** (more is better)
- **Similar style/theme** (fantasy names, real names, animals, etc.)
- **Consistent casing** (all lowercase or all capitalized)

Sample training data files are provided in the `data/` directory:
- `animals.txt` - Common animal names
- `american_forenames.txt` - American first names

More training data is available in the original Haxe library at:
https://github.com/Tw1ddle/markov-namegen-lib/tree/master/word_lists

## Performance

- Name generation is very fast (microseconds per name)
- Training time is proportional to: `(training data size) × (order)`
- Memory usage is proportional to: `(unique character sequences) × (alphabet size)`

For large-scale generation, create one generator and reuse it.

## Algorithm

This implementation uses:

1. **Markov Chains**: Statistical model that predicts the next character based on previous characters
2. **Katz Back-off Model**: Smoothing technique that falls back to lower-order models
3. **Dirichlet Prior**: Additive smoothing that adds a constant probability for random selection

For more information, see:
- [Wikipedia: Markov Chain](https://en.wikipedia.org/wiki/Markov_chain)
- [Wikipedia: Katz's Back-off Model](https://en.wikipedia.org/wiki/Katz%27s_back-off_model)
- [Wikipedia: Additive Smoothing](https://en.wikipedia.org/wiki/Additive_smoothing)
- [Original Project](http://www.samcodes.co.uk/project/markov-namegen/)

## Credits

- **Original Haxe Library**: [Sam Twidale (Tw1ddle)](https://github.com/Tw1ddle/markov-namegen-lib)
- **C# Port**: Based on the Haxe implementation
- **Algorithm**: Described at [RogueBasin](http://www.roguebasin.com/index.php?title=Names_from_a_high_order_Markov_Process_and_a_simplified_Katz_back-off_scheme)

## License

MIT License - see the original project for details.

The core library is licensed under MIT (permissive for commercial use).


## Contributing

This is a port of the original Haxe library. For improvements to the core algorithm, please contribute to the [original project](https://github.com/Tw1ddle/markov-namegen-lib).

## See Also

- [Original Haxe Library](https://github.com/Tw1ddle/markov-namegen-lib)
- [Live Demo](http://www.samcodes.co.uk/project/markov-namegen/) (web version)
- [Previous C# Port](https://github.com/MagicMau/ProceduralNameGenerator) (may be outdated)
