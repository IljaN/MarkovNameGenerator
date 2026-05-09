using MarkovNameGenerator.Util;
using Xunit;

namespace MarkovNameGenerator.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void Reverse_ReversesString()
    {
        Assert.Equal("cba", "abc".Reverse());
        Assert.Equal("54321", "12345".Reverse());
    }

    [Fact]
    public void Repeat_RepeatsString()
    {
        Assert.Equal("aaa", "a".Repeat(3));
        Assert.Equal("foofoo", "foo".Repeat(2));
    }

    [Fact]
    public void Capitalize_CapitalizesFirstLetter()
    {
        Assert.Equal("Hello", "hello".Capitalize());
        Assert.Equal("World", "world".Capitalize());
    }
}

public class EditDistanceMetricsTests
{
    [Fact]
    public void Levenshtein_CalculatesCorrectDistance()
    {
        Assert.Equal(0, EditDistanceMetrics.Levenshtein("test", "test"));
        Assert.Equal(1, EditDistanceMetrics.Levenshtein("test", "text"));
        Assert.Equal(3, EditDistanceMetrics.Levenshtein("kitten", "sitting"));
    }

    [Fact]
    public void DamerauLevenshtein_CalculatesCorrectDistance()
    {
        Assert.Equal(0, EditDistanceMetrics.DamerauLevenshtein("test", "test"));
        Assert.Equal(1, EditDistanceMetrics.DamerauLevenshtein("test", "text"));
        Assert.Equal(1, EditDistanceMetrics.DamerauLevenshtein("ab", "ba")); // Transposition
    }
}

public class PrefixTrieTests
{
    [Fact]
    public void Insert_And_Find_WorksCorrectly()
    {
        var trie = new PrefixTrie();

        trie.Insert("cat");
        trie.Insert("car");
        trie.Insert("card");

        Assert.True(trie.Find("cat"));
        Assert.True(trie.Find("car"));
        Assert.True(trie.Find("card"));
        Assert.False(trie.Find("ca"));
        Assert.False(trie.Find("dog"));
    }

    [Fact]
    public void Insert_IncrementsFrequency()
    {
        var trie = new PrefixTrie();

        var freq1 = trie.Insert("test");
        var freq2 = trie.Insert("test");

        Assert.Equal(1u, freq1);
        Assert.Equal(2u, freq2);
    }

    [Fact]
    public void GetWords_ReturnsAllWords()
    {
        var trie = new PrefixTrie();

        trie.Insert("cat");
        trie.Insert("car");
        trie.Insert("dog");

        var words = trie.GetWords();

        Assert.Equal(3, words.Count);
        Assert.Contains("cat", words);
        Assert.Contains("car", words);
        Assert.Contains("dog", words);
    }
}

public class ArraySetTests
{
    [Fact]
    public void Add_PreventsDuplicates()
    {
        var set = new ArraySet<string>();

        Assert.True(set.Add("a"));
        Assert.False(set.Add("a")); // Duplicate
        Assert.Equal(1, set.Length);
    }

    [Fact]
    public void Contains_FindsElements()
    {
        var set = new ArraySet<int> { 1, 2, 3 };

        Assert.True(set.Contains(2));
        Assert.False(set.Contains(4));
    }

    [Fact]
    public void Union_CombinesSets()
    {
        var set1 = new ArraySet<int> { 1, 2, 3 };
        var set2 = new ArraySet<int> { 3, 4, 5 };

        var union = set1.Union(set2);

        Assert.Equal(5, union.Length);
        Assert.True(union.Contains(1));
        Assert.True(union.Contains(5));
    }

    [Fact]
    public void Intersection_FindsCommonElements()
    {
        var set1 = new ArraySet<int> { 1, 2, 3 };
        var set2 = new ArraySet<int> { 2, 3, 4 };

        var intersection = set1.Intersection(set2);

        Assert.Equal(2, intersection.Length);
        Assert.True(intersection.Contains(2));
        Assert.True(intersection.Contains(3));
        Assert.False(intersection.Contains(1));
    }
}
