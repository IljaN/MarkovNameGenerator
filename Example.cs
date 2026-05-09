#:project ./MarkovNameGenerator/MarkovNameGenerator.csproj

using MarkovNameGenerator.NameGen;
using System.Text.RegularExpressions;

// Example 1: Simple name generation
Console.WriteLine("=== Example 1: Simple Name Generation ===\n");

var simpleNames = new List<string>
{
    "alice", "bob", "charlie", "david", "eve",
    "frank", "grace", "henry", "iris", "jack"
};

var simpleGenerator = new NameGenerator(simpleNames, order: 3, prior: 0.01);

Console.WriteLine("Generating 10 random names:");
for (int i = 0; i < 10; i++)
{
    var name = simpleGenerator.GenerateName(minLength: 3, maxLength: 10);
    if (name != null)
        Console.WriteLine($"  {i + 1}. {name}");
}

// Example 2: Fantasy names with constraints
Console.WriteLine("\n=== Example 2: Fantasy Names Starting with 'dr' ===\n");

var fantasyNames = new List<string>
{
    "aragorn", "gandalf", "frodo", "legolas", "gimli",
    "boromir", "samwise", "galadriel", "elrond", "arwen",
    "eowyn", "faramir", "theoden", "saruman", "sauron"
};

var fantasyGenerator = new NameGenerator(fantasyNames, order: 3, prior: 0.005, backoff: true);

var dragonNames = fantasyGenerator.GenerateNames(
    n: 5,
    minLength: 6,
    maxLength: 12,
    startsWith: "dr"
);

Console.WriteLine($"Generated {dragonNames.Count} dragon-like names:");
foreach (var name in dragonNames)
{
    Console.WriteLine($"  • {name}");
}

// Example 3: Names from file (if available)
Console.WriteLine("\n=== Example 3: Names from Training Data ===\n");

string dataPath = Path.Combine("data", "animals.txt");
if (File.Exists(dataPath))
{
    var animals = File.ReadAllLines(dataPath)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .ToList();

    var animalGenerator = new NameGenerator(animals, order: 3, prior: 0.01);

    Console.WriteLine("Generating animal-inspired portmanteau names:");
    var animalNames = animalGenerator.GenerateNames(n: 10, minLength: 8, maxLength: 12);

    foreach (var name in animalNames)
    {
        Console.WriteLine($"  • {name}");
    }
}
else
{
    Console.WriteLine($"Training data file not found: {dataPath}");
    Console.WriteLine("Place training data files in the data/ directory to use this example.");
}

// Example 4: Using backoff for small datasets
Console.WriteLine("\n=== Example 4: Using Katz Back-off ===\n");

var smallDataset = new List<string> { "alpha", "beta", "gamma", "delta", "epsilon" };

var backoffGenerator = new NameGenerator(smallDataset, order: 3, prior: 0.02, backoff: true);

Console.WriteLine("Generating names with back-off enabled:");
for (int i = 0; i < 5; i++)
{
    var name = backoffGenerator.GenerateName(minLength: 4, maxLength: 9);
    if (name != null)
        Console.WriteLine($"  {i + 1}. {name}");
}

// Example 5: Advanced filtering with regex
Console.WriteLine("\n=== Example 5: Advanced Filtering ===\n");

var techNames = new List<string>
{
    "microsoft", "google", "amazon", "apple", "meta",
    "tesla", "nvidia", "intel", "adobe", "oracle"
};

var techGenerator = new NameGenerator(techNames, order: 2, prior: 0.02);

// Only generate names with alternating consonants and vowels
var pattern = new Regex(@"^([bcdfghjklmnpqrstvwxyz][aeiou])+$", RegexOptions.IgnoreCase);

Console.WriteLine("Generating tech-inspired names (alternating consonant-vowel pattern):");
var techResults = techGenerator.GenerateNames(
    n: 5,
    minLength: 4,
    maxLength: 8,
    regexMatch: pattern
);

foreach (var name in techResults)
{
    Console.WriteLine($"  • {name}");
}


// Example 6: Human names from training file
Console.WriteLine("\n=== Example 6: Human German fore names from Training Data ===\n");

if (File.Exists(dataPath))
{
    var germanForeNames = File.ReadAllLines(Path.Combine("data", "german_forenames.txt"))
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .ToList();

    var humanNameGenerator = new NameGenerator(germanForeNames, order: 3, prior: 0.01);

    Console.WriteLine("Generating german fore names:");
    var humanNames = humanNameGenerator.GenerateNames(n: 10, minLength: 8, maxLength: 12);

    foreach (var name in humanNames)
    {
        Console.WriteLine($"  • {name}");
    }
}
else
{
    Console.WriteLine($"Training data file not found: {dataPath}");
    Console.WriteLine("Place training data files in the data/ directory to use this example.");
}

Console.WriteLine("\n=== Done! ===");
