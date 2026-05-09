using MarkovNameGenerator.NameGen;
using Xunit;

namespace MarkovNameGenerator.Tests;

public class NameGeneratorTests
{
    [Fact]
    public void GenerateName_ProducesValidName()
    {
        // Arrange
        var trainingData = new List<string> { "alice", "bob", "charlie", "david", "eve" };
        var generator = new NameGenerator(trainingData, order: 2, prior: 0.01);

        // Act
        var name = generator.GenerateName(minLength: 2, maxLength: 10);

        // Assert
        Assert.NotNull(name);
        Assert.True(name.Length >= 2 && name.Length <= 10);
    }

    [Fact]
    public void GenerateName_RespectsMinMaxLength()
    {
        // Arrange
        var trainingData = new List<string> { "cat", "dog", "elephant", "lion", "tiger" };
        var generator = new NameGenerator(trainingData, order: 2, prior: 0.01);

        // Act - try multiple times since generation is probabilistic
        bool foundValidName = false;
        for (int i = 0; i < 100; i++)
        {
            var name = generator.GenerateName(minLength: 4, maxLength: 6);
            if (name != null)
            {
                foundValidName = true;
                Assert.True(name.Length >= 4 && name.Length <= 6);
                break;
            }
        }

        Assert.True(foundValidName, "Should generate at least one name within constraints");
    }

    [Fact]
    public void GenerateName_RespectsStartsWith()
    {
        // Arrange
        var trainingData = new List<string> { "alice", "amanda", "amy", "andrew", "anthony" };
        var generator = new NameGenerator(trainingData, order: 2, prior: 0.01);

        // Act - try multiple times
        bool foundValidName = false;
        for (int i = 0; i < 100; i++)
        {
            var name = generator.GenerateName(
                minLength: 2,
                maxLength: 15,
                startsWith: "a");

            if (name != null)
            {
                foundValidName = true;
                Assert.StartsWith("a", name, StringComparison.OrdinalIgnoreCase);
                break;
            }
        }

        Assert.True(foundValidName, "Should generate at least one name starting with 'a'");
    }

    [Fact]
    public void GenerateNames_ProducesMultipleNames()
    {
        // Arrange
        var trainingData = new List<string> { "alpha", "beta", "gamma", "delta", "epsilon" };
        var generator = new NameGenerator(trainingData, order: 2, prior: 0.01);

        // Act
        var names = generator.GenerateNames(
            n: 5,
            minLength: 3,
            maxLength: 10);

        // Assert
        Assert.NotEmpty(names);
        Assert.All(names, name =>
        {
            Assert.True(name.Length >= 3 && name.Length <= 10);
        });
    }

    [Fact]
    public void GenerateName_WithBackoff_ProducesValidName()
    {
        // Arrange - use more training data for better results
        var trainingData = new List<string>
        {
            "alpha", "beta", "gamma", "delta", "epsilon",
            "theta", "lambda", "omega", "sigma", "phi"
        };
        var generator = new NameGenerator(trainingData, order: 3, prior: 0.01, backoff: true);

        // Act - try multiple times since generation is probabilistic
        bool foundValidName = false;
        for (int i = 0; i < 50; i++)
        {
            var name = generator.GenerateName(minLength: 2, maxLength: 10);
            if (name != null)
            {
                foundValidName = true;
                Assert.True(name.Length >= 2 && name.Length <= 10);
                break;
            }
        }

        // Assert
        Assert.True(foundValidName, "Should generate at least one valid name with backoff enabled");
    }
}
