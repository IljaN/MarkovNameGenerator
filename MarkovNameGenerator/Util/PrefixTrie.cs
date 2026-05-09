namespace MarkovNameGenerator.Util;

/// <summary>
/// An unoptimized prefix trie. A type of ordered tree structure for storing and organizing strings.
/// </summary>
/// <remarks>
/// See <see href="https://en.wikipedia.org/wiki/Trie">Wikipedia: Trie</see>
/// </remarks>
public class PrefixTrie
{
    /// <summary>
    /// The root node of the trie.
    /// </summary>
    public PrefixNode Root { get; }

    /// <summary>
    /// Creates a new trie with only a root node.
    /// </summary>
    public PrefixTrie()
    {
        Root = new PrefixNode(null, string.Empty, 0);
    }

    /// <summary>
    /// Inserts a word into the trie. For nodes that already exist, it increments a frequency count.
    /// Marks the node that represents the final character in the word with the "word" flag.
    /// </summary>
    /// <param name="word">The word to add to the trie.</param>
    /// <returns>The number of times the word exists in the trie.</returns>
    public uint Insert(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        var current = Root;

        for (int i = 0; i < word.Length; i++)
        {
            string ch = word[i].ToString();
            var child = FindChild(current, ch);

            if (child == null)
            {
                child = new PrefixNode(current, ch, (uint)i);
                current.Children.Add(child);
            }
            else
            {
                child.Frequency++;
            }

            current = child;
        }

        current.Word = true;
        return current.Frequency;
    }

    /// <summary>
    /// Attempts to find a word in the trie.
    /// If the boolean "word" flag is set on the terminal node of the word in the trie,
    /// it returns true, else it returns false.
    /// </summary>
    /// <param name="word">The word to find.</param>
    /// <returns>True if the word was found, false if it was not.</returns>
    public bool Find(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        var current = Root;

        for (int i = 0; i < word.Length; i++)
        {
            current = FindChild(current, word[i].ToString());
            if (current == null)
            {
                return false;
            }
        }

        return current.Word;
    }

    /// <summary>
    /// Builds an array of all the words that have been inserted into the trie.
    /// This is only appropriate for debugging or small data sets, as it performs
    /// a slow breadth-first search that works back up to the root every time it reconstructs a word.
    /// </summary>
    /// <returns>An array containing the set of unique words that have been inserted into the trie.</returns>
    public List<string> GetWords()
    {
        var queue = new Queue<PrefixNode>();
        queue.Enqueue(Root);
        var words = new List<string>();

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (node.Word)
            {
                string word = node.Letter;
                var parent = node.Parent;

                while (parent != null)
                {
                    word += parent.Letter;
                    parent = parent.Parent;
                }

                words.Add(word.Reverse());
            }

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }

        return words;
    }

    /// <summary>
    /// Attempts to find an immediate child node with the given letter.
    /// </summary>
    /// <param name="node">The node whose children will be searched.</param>
    /// <param name="letter">The letter to search for.</param>
    /// <returns>The child node with the matching letter, null if none is found.</returns>
    private static PrefixNode? FindChild(PrefixNode node, string letter)
    {
        foreach (var child in node.Children)
        {
            if (child.Letter == letter)
            {
                return child;
            }
        }
        return null;
    }
}

/// <summary>
/// A node in the prefix trie.
/// </summary>
public class PrefixNode
{
    /// <summary>
    /// The parent of the current node. Null if the node is a root node.
    /// </summary>
    public PrefixNode? Parent { get; }

    /// <summary>
    /// The children of this node. Empty if there are no children, never null.
    /// </summary>
    public List<PrefixNode> Children { get; }

    /// <summary>
    /// The letter contained in this node.
    /// </summary>
    public string Letter { get; }

    /// <summary>
    /// The depth of the node in the trie.
    /// </summary>
    public uint Depth { get; }

    /// <summary>
    /// The number of times this node is used in the trie.
    /// i.e. a trie containing the word "AS" and "AD" would have A -> 2, S -> 1, D -> 1.
    /// </summary>
    public uint Frequency { get; set; }

    /// <summary>
    /// Whether this node is the end of a word.
    /// This includes all the terminal nodes, but may also include intermediate nodes.
    /// i.e. for "LAD" and "LADS", the "D" node would be a "word" node, despite "D" not being a terminal node.
    /// </summary>
    public bool Word { get; set; }

    /// <summary>
    /// Creates a new trie node.
    /// </summary>
    /// <param name="parent">The parent of this node. Null if the node is the root node.</param>
    /// <param name="letter">The letter this node represents.</param>
    /// <param name="depth">The depth of the node in the trie.</param>
    public PrefixNode(PrefixNode? parent, string letter, uint depth)
    {
        if (letter.Length != 1 && !(parent == null && depth == 0))
        {
            throw new ArgumentException("Letter must be a single character, except for root node", nameof(letter));
        }

        Parent = parent;
        Children = new List<PrefixNode>();
        Letter = letter;
        Depth = depth;
        Frequency = 1;
        Word = false;
    }
}
