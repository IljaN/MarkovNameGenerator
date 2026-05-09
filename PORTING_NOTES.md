# Porting Notes: Haxe to C# (.NET 8)

## Summary

Successfully ported the Markov Name Generator library from Haxe to modern C# (.NET 8).

## What Was Ported

### Core Library Classes (MarkovNameGenerator/)

1. **Model.cs** - Markov chain model implementation
   - Uses `Dictionary<string, List<T>>` instead of Haxe's `StringMap`
   - Uses `Random.Shared` for random number generation
   - Implements weighted random selection for character picking

2. **Generator.cs** - Base generator with Katz back-off
   - Manages multiple Model instances for different orders
   - Implements back-off strategy
   - Extracts unique alphabet from training data

3. **NameGenerator.cs** - High-level API
   - Constraint filtering (length, prefix, suffix, includes, excludes, regex)
   - Batch generation with time limits
   - Uses `Stopwatch` for timing instead of `Date.now()`

### Utility Classes (MarkovNameGenerator/Util/)

1. **StringExtensions.cs**
   - Extension methods: Reverse, Repeat, Capitalize, Lowercase, etc.
   - Uses `StringBuilder` for efficient string building
   - Aggressive inlining for performance

2. **IntExtensions.cs**
   - Clamp and Min methods
   - Used by EditDistanceMetrics

3. **EditDistanceMetrics.cs**
   - Levenshtein distance algorithm
   - Damerau-Levenshtein distance (with transpositions)
   - Distance matrix computation

4. **PrefixTrie.cs**
   - Trie data structure for fast string lookup
   - Used for deduplication in the demo app
   - O(m) lookup time where m is string length

5. **ArraySet.cs**
   - Set implementation backed by `List<T>`
   - Prevents duplicates
   - Set operations: Union, Intersection, Difference

### Tests (MarkovNameGenerator.Tests/)

- **17 unit tests** covering all major functionality
- Uses xUnit testing framework
- Tests for utility classes and name generation
- All tests passing

## Key Translation Decisions

### Language Feature Mappings

| Haxe | C# |
|------|-----|
| `StringMap<T>` | `Dictionary<string, T>` |
| `Array<T>` | `List<T>` |
| `Vector<T>` | `int[]` or `T[]` |
| `UInt` | `uint` |
| `Float` | `double` |
| `inline function` | `[MethodImpl(AggressiveInlining)]` |
| `Sure.sure(condition)` | `ArgumentNullException.ThrowIfNull()`, etc. |
| `using Lambda` | LINQ extension methods |
| `Date.now().getTime()` | `Stopwatch` |
| `Math.random()` | `Random.Shared.NextDouble()` |

### Design Choices

1. **Assertions**: Replaced Haxe's `Sure.sure()` with C#'s modern argument validation:
   - `ArgumentNullException.ThrowIfNull()`
   - `ArgumentOutOfRangeException.ThrowIfLessThan()`
   - `ArgumentException.ThrowIfNullOrEmpty()`

2. **String Operations**: Case-insensitive comparisons use `StringComparison.OrdinalIgnoreCase`

3. **Random Numbers**: Uses `Random.Shared` (thread-safe, .NET 6+) instead of creating new instances

4. **Collections**:
   - `List<T>` instead of Haxe's `Array<T>` (more idiomatic C#)
   - `HashSet<T>` could be used instead of `ArraySet<T>` but kept for API compatibility

5. **Visibility**: Made `Model` class `internal` since it's only used by `Generator`

6. **Nullable References**: Enabled nullable reference types for better null safety

## Project Structure

```
csharp/
├── MarkovNameGenerator/              # Main library (.NET 8 class library)
│   ├── NameGen/
│   │   ├── Generator.cs              # 89 lines
│   │   ├── Model.cs                  # 165 lines
│   │   └── NameGenerator.cs          # 94 lines
│   └── Util/
│       ├── ArraySet.cs               # 244 lines
│       ├── EditDistanceMetrics.cs    # 140 lines
│       ├── IntExtensions.cs          # 36 lines
│       ├── PrefixTrie.cs             # 188 lines
│       └── StringExtensions.cs       # 132 lines
├── MarkovNameGenerator.Tests/        # Test project (xUnit)
│   ├── NameGeneratorTests.cs         # 121 lines
│   └── UtilTests.cs                  # 152 lines
├── data/                             # Sample training data
│   ├── animals.txt
│   └── american_forenames.txt
├── Example.cs                        # Usage examples
├── README.md                         # Comprehensive documentation
└── MarkovNameGenerator.sln           # Solution file
```

## Not Ported

The following were intentionally excluded from the port:

1. **Web Demo Application** (`Main.hx`, `TrainingData.hx`, etc.)
   - UI-specific code for the browser demo
   - JavaScript interop (`NoUiSlider`, `WNumb`)
   - HTML manipulation code

2. **Build-time Macros**
   - `TrainingDataBuilder.hx` - Generates training data fields at compile time
   - `CodeCompletion.hx` - Scrapes HTML for element IDs
   - `FileReader.hx` - Embeds files at compile time
   - In C#, use embedded resources or runtime file loading instead

3. **ShareResults.hx** - URL query string handling for sharing generated names

## Build and Test Results

```bash
# Build: SUCCESS
$ dotnet build
Build succeeded.
    0 Warning(s)
    0 Error(s)

# Tests: 17/17 PASSED
$ dotnet test
Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17
```

## Performance Characteristics

- **Name generation**: Microseconds per name
- **Model training**: Proportional to `(training_data_size) × (order)`
- **Memory usage**: Proportional to `(unique_sequences) × (alphabet_size)`

No significant performance differences expected between Haxe and C# versions.

## Usage Example

```csharp
using MarkovNameGenerator.NameGen;

var data = new List<string> { "alice", "bob", "charlie", "david" };
var generator = new NameGenerator(data, order: 2, prior: 0.01);
var name = generator.GenerateName(minLength: 4, maxLength: 10);
Console.WriteLine(name);
```

## Testing

All 17 tests pass, covering:
- Name generation with various constraints
- String manipulation utilities
- Edit distance calculations
- Trie operations
- Set operations

## Documentation

- **README.md**: Comprehensive guide with API reference, examples, and algorithm explanation
- **Example.cs**: Runnable examples demonstrating various use cases
- **XML Documentation**: All public APIs documented with XML comments

## Future Improvements

Potential enhancements (not implemented):

1. **Performance**:
   - Use `Span<T>` for string operations
   - Consider `ArrayPool<T>` for temporary allocations
   - Profile dictionary lookups

2. **API Enhancements**:
   - Async generation methods
   - `IEnumerable<string>` streaming API
   - Custom alphabet support

3. **NuGet Package**:
   - Package for distribution
   - Add package metadata
   - Include training data as embedded resources

## References

- Original Haxe Library: https://github.com/Tw1ddle/markov-namegen-lib
- Algorithm: http://www.roguebasin.com/index.php?title=Names_from_a_high_order_Markov_Process_and_a_simplified_Katz_back-off_scheme
- Demo: http://www.samcodes.co.uk/project/markov-namegen/
