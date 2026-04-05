// ============================================================
// PHASE 24 — PROJECT 4: TRIE-BASED SEARCH ENGINE
// Demonstrates: Trie data structure for prefix-based search
// Features: Insert words with metadata, autocomplete,
//           spell check suggestions, search statistics
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;

// ============================================================
// TRIE (PREFIX TREE) REVIEW:
//
// A tree where each path from root to a node spells a prefix.
// All strings sharing a prefix share the same path up to that prefix.
//
//   Insert: "cat", "car", "card", "care", "bat"
//
//         root
//        /    \
//       c      b
//       |      |
//       a      a
//      / \     |
//     t   r    t
//         |  \
//        d    e
//
// Time: Insert/Search/StartsWith = O(key length)
// Space: O(total characters in all keys) — shared prefixes save space
// ============================================================

class TrieNode
{
    public Dictionary<char, TrieNode> Children = new();
    public bool IsWordEnd;
    public int Frequency;        // how many times this word was added/searched
    public string? Definition;   // optional metadata
}

class SearchTrie
{
    private readonly TrieNode _root = new TrieNode();
    private int _totalWords;

    // INSERT a word with optional metadata
    public void Insert(string word, string? definition = null, int frequency = 1)
    {
        if (string.IsNullOrEmpty(word)) return;

        word = word.ToLowerInvariant();   // normalize to lowercase
        var node = _root;

        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
                node.Children[c] = new TrieNode();
            node = node.Children[c];
        }

        node.IsWordEnd = true;
        node.Frequency += frequency;
        if (definition != null) node.Definition = definition;
        _totalWords++;
    }

    // EXACT SEARCH: check if word exists
    public (bool found, int frequency, string? definition) Search(string word)
    {
        word = word.ToLowerInvariant();
        var node = NavigateTo(word);
        if (node?.IsWordEnd == true)
        {
            node.Frequency++;  // increment search count (tracks popularity)
            return (true, node.Frequency, node.Definition);
        }
        return (false, 0, null);
    }

    // PREFIX CHECK: does any word start with this prefix?
    public bool StartsWith(string prefix)
    {
        return NavigateTo(prefix.ToLowerInvariant()) != null;
    }

    // AUTOCOMPLETE: return top N completions for a prefix
    public List<(string word, int frequency)> Autocomplete(string prefix, int topN = 5)
    {
        prefix = prefix.ToLowerInvariant();
        var prefixNode = NavigateTo(prefix);
        if (prefixNode == null) return new List<(string, int)>();

        var results = new List<(string word, int freq)>();
        CollectAllWords(prefixNode, prefix, results);

        return results.OrderByDescending(x => x.freq)
                      .Take(topN)
                      .ToList();
    }

    // SPELL CHECK: find closest words if exact match not found
    // Uses edit distance ≤ maxDistance for suggestions
    public List<string> SpellCheck(string word, int maxDistance = 2)
    {
        word = word.ToLowerInvariant();
        var suggestions = new List<(string word, int dist)>();

        // Collect all words and compute edit distance
        var allWords = new List<(string, int)>();
        CollectAllWords(_root, "", allWords);

        foreach (var (w, _) in allWords)
        {
            int dist = EditDistance(word, w);
            if (dist <= maxDistance && dist > 0)  // dist > 0 to exclude exact match
                suggestions.Add((w, dist));
        }

        return suggestions.OrderBy(x => x.dist)
                          .Take(5)
                          .Select(x => x.word)
                          .ToList();
    }

    // GET STATISTICS
    public void PrintStats()
    {
        var allWords = new List<(string, int)>();
        CollectAllWords(_root, "", allWords);

        Console.WriteLine($"  Total words: {allWords.Count}");
        Console.WriteLine($"  Total insertions: {_totalWords}");

        var topWords = allWords.OrderByDescending(x => x.Item2).Take(3);
        Console.WriteLine("  Most searched:");
        foreach (var (w, freq) in topWords)
            Console.WriteLine($"    '{w}': {freq} times");
    }

    // Navigate to a node for a given prefix/word (returns null if prefix not found)
    private TrieNode? NavigateTo(string prefix)
    {
        var node = _root;
        foreach (char c in prefix)
        {
            if (!node.Children.TryGetValue(c, out node)) return null;
        }
        return node;
    }

    // DFS: collect all complete words below a node
    private void CollectAllWords(TrieNode node, string current, List<(string, int)> results)
    {
        if (node.IsWordEnd) results.Add((current, node.Frequency));
        foreach (var (c, child) in node.Children)
            CollectAllWords(child, current + c, results);
    }

    // Standard Edit Distance DP (Levenshtein)
    private static int EditDistance(string a, string b)
    {
        int m = a.Length, n = b.Length;
        var dp = new int[m + 1, n + 1];
        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;
        for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
                dp[i, j] = a[i-1] == b[j-1]
                    ? dp[i-1, j-1]
                    : 1 + Math.Min(dp[i-1, j], Math.Min(dp[i, j-1], dp[i-1, j-1]));
        return dp[m, n];
    }
}

class TrieSearchEngineProject
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PROJECT 4: TRIE-BASED SEARCH ENGINE");
        Console.WriteLine("======================================================\n");

        var trie = new SearchTrie();

        // Build search index (word → definition)
        var dictionary = new[]
        {
            ("algorithm",    "A step-by-step problem-solving procedure"),
            ("algebra",      "Branch of mathematics using symbols and rules"),
            ("abstract",     "Dealing in ideas rather than concrete things"),
            ("abstraction",  "The process of hiding complexity"),
            ("array",        "A data structure storing elements sequentially"),
            ("async",        "Asynchronous: non-blocking execution"),
            ("binary",       "Relating to base-2 number system"),
            ("breadth",      "Width or extent of something"),
            ("bubble",       "A round pocket of gas in a liquid"),
            ("cache",        "A fast storage layer"),
            ("class",        "A blueprint for creating objects in OOP"),
            ("complexity",   "A measure of algorithm performance"),
            ("concurrent",   "Happening at the same time"),
            ("delegate",     "A type-safe function pointer in C#"),
            ("dictionary",   "Key-value pair collection in C#"),
            ("dynamic",      "Relating to forces producing motion"),
            ("encapsulation","Hiding implementation details in OOP"),
            ("exception",    "An error event disrupting normal program flow"),
            ("generic",      "Parameterized type in C#"),
            ("graph",        "A non-linear data structure with vertices and edges"),
            ("hash",         "A fixed-size value derived from input data"),
            ("heap",         "A specialized tree for priority queue"),
            ("interface",    "A contract defining what a class must implement"),
            ("iterator",     "An object enabling traversal of a collection"),
            ("lambda",       "An anonymous function expression"),
            ("linked",       "Connected in a chain"),
            ("list",         "An ordered collection of elements"),
            ("merge",        "To combine multiple sorted arrays into one"),
            ("polymorphism", "The ability of an object to take multiple forms"),
            ("queue",        "A FIFO data structure"),
            ("recursion",    "A function that calls itself"),
            ("stack",        "A LIFO data structure"),
            ("static",       "Fixed; not changing at runtime"),
            ("string",       "A sequence of characters"),
            ("tree",         "A hierarchical data structure"),
            ("trie",         "A prefix tree for string storage and retrieval"),
        };

        foreach (var (word, def) in dictionary)
            trie.Insert(word, definition: def, frequency: new Random().Next(1, 50));

        // AUTOCOMPLETE demos
        Console.WriteLine("=== Autocomplete ===");
        string[] testPrefixes = { "al", "gr", "re", "str", "con", "dy" };
        foreach (string prefix in testPrefixes)
        {
            var suggestions = trie.Autocomplete(prefix, topN: 4);
            Console.WriteLine($"  '{prefix}*' → {string.Join(", ", suggestions.Select(s => s.word))}");
        }

        // EXACT SEARCH demo
        Console.WriteLine("\n=== Exact Search ===");
        string[] searchWords = { "graph", "trie", "python", "stack", "hashmap" };
        foreach (string word in searchWords)
        {
            var (found, freq, def) = trie.Search(word);
            if (found)
                Console.WriteLine($"  '{word}': Found! freq={freq}  — {def}");
            else
                Console.WriteLine($"  '{word}': Not found.");
        }

        // SPELL CHECK demo
        Console.WriteLine("\n=== Spell Check ===");
        string[] misspelled = { "algorythm", "recusion", "dynaamic", "stck" };
        foreach (string word in misspelled)
        {
            var suggestions = trie.SpellCheck(word, maxDistance: 3);
            Console.WriteLine($"  '{word}' → Did you mean: {string.Join(", ", suggestions)}?");
        }

        // STATS
        Console.WriteLine("\n=== Search Index Statistics ===");
        trie.PrintStats();

        Console.WriteLine("\n--- Why Trie? ---");
        Console.WriteLine("  O(L) insert/search (L = word length) — independent of vocab size!");
        Console.WriteLine("  O(V) autocomplete where V = vocabulary in subtree");
        Console.WriteLine("  Shared prefixes save space: 'array', 'abstract', 'async' share 'a'");
        Console.WriteLine("  Used in: Google Search, IDEs, spell checkers, DNS lookups, IP routing");
    }
}
