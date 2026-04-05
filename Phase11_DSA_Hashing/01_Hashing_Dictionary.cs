// ============================================================
// PHASE 11: HASHING & DICTIONARIES — Complete C# Implementation
// Topics: Hashing concepts, Dictionary<K,V>, HashSet<T>,
//         Hashtable, Collision handling, Internal working,
//         Common interview problems
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ============================================================
// SECTION 1: WHAT IS HASHING?
//
// Hashing converts a KEY into an INTEGER INDEX (hash code)
// which is used to directly locate data in an array (bucket array).
//
// Hash Function: key → hash code → bucket index
// Example:
//   key = "Alice"
//   GetHashCode("Alice") = 12345678   (some integer)
//   bucketIndex = 12345678 % bucketCount = 3  (modulo to fit array size)
//   Store value at array[3]
//
// This allows O(1) AVERAGE lookup — no need to scan all elements!
// ============================================================

// ============================================================
// SECTION 2: BUILDING A SIMPLE HASH TABLE FROM SCRATCH
// This demonstrates how Dictionary<K,V> works internally.
// Uses: separate chaining for collision handling
// ============================================================

public class SimpleHashTable<TKey, TValue>
{
    // Each "bucket" is a list of (key, value) pairs
    // Multiple pairs share a bucket when they hash to the same index (collision!)
    private List<(TKey key, TValue value)>[] _buckets;
    private int _bucketCount;
    public int Count { get; private set; }

    // Load factor = Count / _bucketCount
    // When load factor > threshold (typically 0.75), we RESIZE (rehash)
    // This keeps average bucket length short → O(1) lookup maintained
    private const double LoadFactorThreshold = 0.75;

    public SimpleHashTable(int initialBuckets = 8)
    {
        _bucketCount = initialBuckets;
        _buckets = new List<(TKey, TValue)>[_bucketCount];
    }

    // Compute which bucket this key belongs to
    private int GetBucketIndex(TKey key)
    {
        // GetHashCode() returns an int (may be negative!)
        // Math.Abs + modulo → valid bucket index
        int hashCode = key.GetHashCode();
        return Math.Abs(hashCode) % _bucketCount;
    }

    // --- PUT (INSERT OR UPDATE) — O(1) average ---
    public void Put(TKey key, TValue value)
    {
        // Check if load factor exceeded — if so, resize first
        if ((double)Count / _bucketCount > LoadFactorThreshold)
            Resize();

        int index = GetBucketIndex(key);

        // Initialize bucket if this is first item in it
        if (_buckets[index] == null)
            _buckets[index] = new List<(TKey, TValue)>();

        // Search bucket for existing key (handles duplicate keys → update)
        for (int i = 0; i < _buckets[index].Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(_buckets[index][i].key, key))
            {
                // Key exists → UPDATE the value
                _buckets[index][i] = (key, value);
                return;
            }
        }

        // Key not found → INSERT new entry
        _buckets[index].Add((key, value));
        Count++;
    }

    // --- GET — O(1) average ---
    public bool TryGet(TKey key, out TValue value)
    {
        int index = GetBucketIndex(key);

        if (_buckets[index] != null)
        {
            // Search the bucket (chain) for the key
            foreach (var pair in _buckets[index])
            {
                if (EqualityComparer<TKey>.Default.Equals(pair.key, key))
                {
                    value = pair.value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    // --- REMOVE — O(1) average ---
    public bool Remove(TKey key)
    {
        int index = GetBucketIndex(key);

        if (_buckets[index] == null) return false;

        for (int i = 0; i < _buckets[index].Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(_buckets[index][i].key, key))
            {
                _buckets[index].RemoveAt(i);
                Count--;
                return true;
            }
        }
        return false;
    }

    // --- RESIZE (REHASH) — O(n) ---
    // When too many items collide into same buckets, increase bucket count
    // and redistribute all entries to new (larger) bucket array.
    private void Resize()
    {
        int newBucketCount = _bucketCount * 2;
        Console.WriteLine($"  [HashTable resize: {_bucketCount} → {newBucketCount} buckets]");

        var oldBuckets = _buckets;
        _bucketCount = newBucketCount;
        _buckets = new List<(TKey, TValue)>[_bucketCount];
        Count = 0;

        // Re-insert all existing items (they get new bucket positions!)
        foreach (var bucket in oldBuckets)
        {
            if (bucket != null)
            {
                foreach (var pair in bucket)
                    Put(pair.key, pair.value);
            }
        }
    }

    // Print internal bucket structure (shows collision chains)
    public void PrintBuckets()
    {
        Console.WriteLine($"  HashTable Internal State ({Count} items, {_bucketCount} buckets):");
        for (int i = 0; i < _bucketCount; i++)
        {
            if (_buckets[i] != null && _buckets[i].Count > 0)
            {
                Console.Write($"  Bucket[{i}]: ");
                foreach (var pair in _buckets[i])
                    Console.Write($"({pair.key}→{pair.value}) ");
                Console.WriteLine();
            }
        }
    }
}

// ============================================================
// SECTION 3: COMMON INTERVIEW PROBLEMS USING HASHING
// ============================================================

class HashingProblems
{
    // --- PROBLEM 1: Two Sum ---
    // Given array and target, find indices of two numbers that add to target.
    // O(n) time using HashMap.
    public static (int, int) TwoSum(int[] nums, int target)
    {
        // Map: value → index
        Dictionary<int, int> seen = new Dictionary<int, int>();

        for (int i = 0; i < nums.Length; i++)
        {
            int complement = target - nums[i];  // what we need to find

            if (seen.ContainsKey(complement))
                return (seen[complement], i);   // found! return both indices

            seen[nums[i]] = i;  // store this number and its index
        }

        return (-1, -1);  // no solution
    }

    // --- PROBLEM 2: Word Frequency Count ---
    // Count how many times each word appears in an array.
    public static Dictionary<string, int> WordFrequency(string[] words)
    {
        var freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (string word in words)
        {
            // GetValueOrDefault is safer than direct access
            freq[word] = freq.GetValueOrDefault(word, 0) + 1;

            // Alternative: if (freq.ContainsKey(word)) freq[word]++; else freq[word] = 1;
        }

        return freq;
    }

    // --- PROBLEM 3: Find First Non-Repeating Character ---
    // Find the first character in a string that appears only once.
    public static char? FirstNonRepeating(string s)
    {
        // Two-pass approach:
        // Pass 1: count frequencies
        Dictionary<char, int> freq = new Dictionary<char, int>();
        foreach (char c in s)
            freq[c] = freq.GetValueOrDefault(c, 0) + 1;

        // Pass 2: find first with count = 1 (preserves original order)
        foreach (char c in s)
            if (freq[c] == 1) return c;

        return null;  // all characters repeat
    }

    // --- PROBLEM 4: Group Anagrams ---
    // Group strings that are anagrams of each other.
    // "eat", "tea", "tan", "ate", "nat", "bat"
    // → [["eat","tea","ate"], ["tan","nat"], ["bat"]]
    public static List<List<string>> GroupAnagrams(string[] strs)
    {
        // Two strings are anagrams if they have the same sorted characters.
        // Use sorted string as the KEY in dictionary.
        var groups = new Dictionary<string, List<string>>();

        foreach (string s in strs)
        {
            char[] chars = s.ToCharArray();
            Array.Sort(chars);                // sort: "eat" → "aet", "tea" → "aet"
            string key = new string(chars);   // key = "aet"

            if (!groups.ContainsKey(key))
                groups[key] = new List<string>();

            groups[key].Add(s);  // group all anagrams together
        }

        return groups.Values.ToList();
    }

    // --- PROBLEM 5: Subarray with Given Sum (using prefix sums + hashmap) ---
    // Find if there's a subarray that sums to target. O(n) time.
    public static bool SubarrayWithSum(int[] nums, int target)
    {
        // Key insight: if prefix[j] - prefix[i] = target, then subarray [i+1..j] = target
        // So for each prefix sum, check if (prefixSum - target) was seen before

        HashSet<int> seen = new HashSet<int>();
        seen.Add(0);  // empty subarray has prefix sum = 0

        int prefixSum = 0;
        foreach (int num in nums)
        {
            prefixSum += num;

            if (seen.Contains(prefixSum - target))
                return true;  // found subarray!

            seen.Add(prefixSum);
        }

        return false;
    }

    // --- PROBLEM 6: Longest Consecutive Sequence ---
    // Find the length of the longest consecutive sequence.
    // [100, 4, 200, 1, 3, 2] → answer is 4 (sequence: 1, 2, 3, 4)
    public static int LongestConsecutive(int[] nums)
    {
        HashSet<int> numSet = new HashSet<int>(nums);  // O(1) lookup
        int longest = 0;

        foreach (int n in nums)
        {
            // Only start counting a sequence from its LOWEST number
            // (n-1 not in set = n is the start of a new sequence)
            if (!numSet.Contains(n - 1))
            {
                int current = n;
                int length = 1;

                // Count how long this consecutive run is
                while (numSet.Contains(current + 1))
                {
                    current++;
                    length++;
                }

                longest = Math.Max(longest, length);
            }
        }

        return longest;
    }
}

// ============================================================
// SECTION 4: HASHSET<T> — FAST MEMBERSHIP TESTING
// HashSet is like Dictionary but stores KEYS ONLY (no values)
// Use when you need UNIQUENESS or O(1) "Have I seen this?" checks
// ============================================================

class HashSetDemo
{
    public static void Run()
    {
        Console.WriteLine("\n--- HashSet<T> Demo ---");

        HashSet<int> setA = new HashSet<int> { 1, 2, 3, 4, 5 };
        HashSet<int> setB = new HashSet<int> { 3, 4, 5, 6, 7 };

        Console.WriteLine($"  Set A: {{{string.Join(", ", setA)}}}");
        Console.WriteLine($"  Set B: {{{string.Join(", ", setB)}}}");

        // Add — returns false if duplicate
        bool added = setA.Add(6);
        Console.WriteLine($"  Added 6 to A: {added}");   // true
        bool dup = setA.Add(1);
        Console.WriteLine($"  Added 1 to A (dup): {dup}"); // false — already exists

        // Contains — O(1)
        Console.WriteLine($"  A.Contains(3): {setA.Contains(3)}");  // true

        // Remove — O(1)
        setA.Remove(6);  // undo earlier add

        // --- SET OPERATIONS ---
        HashSet<int> unionSet = new HashSet<int>(setA);
        unionSet.UnionWith(setB);          // A ∪ B
        Console.WriteLine($"  A ∪ B: {{{string.Join(", ", unionSet)}}}");  // 1,2,3,4,5,6,7

        HashSet<int> intersectSet = new HashSet<int>(setA);
        intersectSet.IntersectWith(setB);  // A ∩ B
        Console.WriteLine($"  A ∩ B: {{{string.Join(", ", intersectSet)}}}"); // 3,4,5

        HashSet<int> exceptSet = new HashSet<int>(setA);
        exceptSet.ExceptWith(setB);        // A - B (elements in A but not in B)
        Console.WriteLine($"  A - B: {{{string.Join(", ", exceptSet)}}}");    // 1,2

        // IsSubsetOf, IsSupersetOf
        HashSet<int> subset = new HashSet<int> { 3, 4 };
        Console.WriteLine($"  {{3,4}} ⊆ B: {subset.IsSubsetOf(setB)}");      // true
        Console.WriteLine($"  B ⊇ {{3,4}}: {setB.IsSupersetOf(subset)}");    // true
    }
}

// ============================================================
// SECTION 5: HASHTABLE (NON-GENERIC, LEGACY)
// Hashtable is the old non-generic version from .NET 1.0
// Use Dictionary<K,V> in modern code — this is just for reference
// Key differences:
//   - Hashtable: non-generic (stores object), box/unbox for value types
//   - Dictionary: generic (type-safe, no boxing, IntelliSense support)
//   - Hashtable: thread-safe for multiple readers, NOT for writer
//   - Dictionary: NOT thread-safe (use ConcurrentDictionary for threading)
// ============================================================

class HashtableDemo
{
    public static void Run()
    {
        Console.WriteLine("\n--- Hashtable (Legacy, Non-Generic) ---");

        Hashtable ht = new Hashtable();

        // Keys and values are 'object' — no type safety!
        ht["name"]  = "Alice";   // string key → string value
        ht["age"]   = 30;        // string key → int value (boxed!)
        ht[42]      = "Answer";  // int key → string value
        ht[DateTime.Today] = "Today's entry";  // any key type!

        Console.WriteLine($"  ht[\"name\"] = {ht["name"]}");
        Console.WriteLine($"  ht[\"age\"] = {ht["age"]}");

        // Requires casting to use (no type safety)
        int age = (int)ht["age"];          // explicit cast needed
        string name = (string)ht["name"];  // explicit cast needed

        // ContainsKey / ContainsValue
        Console.WriteLine($"  Contains key 'age': {ht.ContainsKey("age")}");
        Console.WriteLine($"  Contains value 30: {ht.ContainsValue(30)}");

        // Iteration (key and value are object)
        Console.WriteLine("  All entries:");
        foreach (DictionaryEntry entry in ht)
            Console.WriteLine($"    {entry.Key} → {entry.Value}");

        Console.WriteLine("\n  ⚠ PREFER Dictionary<K,V> over Hashtable in modern C#!");
        Console.WriteLine("    Reasons: type safety, no boxing, better performance, LINQ support");
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class HashingProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 11: HASHING & DICTIONARIES");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Custom Hash Table — shows internal mechanics
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Custom Hash Table (Separate Chaining) ===");

        var hashTable = new SimpleHashTable<string, int>(4);  // small size to force collisions

        hashTable.Put("Alice",   90);
        hashTable.Put("Bob",     85);
        hashTable.Put("Charlie", 92);
        hashTable.Put("Dave",    78);
        hashTable.Put("Eve",     88);  // This may trigger resize
        hashTable.PrintBuckets();

        hashTable.TryGet("Alice", out int aliceScore);
        Console.WriteLine($"  Alice's score: {aliceScore}");  // 90

        hashTable.Put("Alice", 95);    // Update existing key
        hashTable.TryGet("Alice", out aliceScore);
        Console.WriteLine($"  Alice's updated score: {aliceScore}");  // 95

        hashTable.Remove("Bob");
        Console.WriteLine($"  After removing Bob, Count: {hashTable.Count}");

        // -------------------------------------------------------
        // DEMO 2: C# Built-in Dictionary<K,V>
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: C# Dictionary<TKey, TValue> ===");

        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Adding entries
        dict["apple"]  = 5;
        dict["banana"] = 3;
        dict["cherry"] = 8;
        dict.Add("date", 2);

        // Reading — safe with TryGetValue
        if (dict.TryGetValue("apple", out int count))
            Console.WriteLine($"  apple count: {count}");  // 5

        // This would throw KeyNotFoundException:
        // int bad = dict["orange"];

        // Safe default if key missing
        int orangeCount = dict.GetValueOrDefault("orange", 0);
        Console.WriteLine($"  orange count (default 0): {orangeCount}");

        // Contains check
        Console.WriteLine($"  ContainsKey 'banana': {dict.ContainsKey("banana")}");
        Console.WriteLine($"  ContainsValue 8: {dict.ContainsValue(8)}");

        // Iterating
        Console.WriteLine("  All entries:");
        foreach (KeyValuePair<string, int> kvp in dict)
            Console.WriteLine($"    {kvp.Key} → {kvp.Value}");

        // LINQ on Dictionary
        var expensive = dict.Where(kv => kv.Value > 4)
                            .OrderByDescending(kv => kv.Value)
                            .Select(kv => $"{kv.Key}({kv.Value})");
        Console.WriteLine($"  Count > 4: {string.Join(", ", expensive)}");

        // Remove
        dict.Remove("date");
        Console.WriteLine($"  After Remove('date'): {dict.Count} entries");

        // -------------------------------------------------------
        // DEMO 3: Interview Problems
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Interview Problems ===");

        // Two Sum
        int[] nums = { 2, 7, 11, 15 };
        var (i1, i2) = HashingProblems.TwoSum(nums, 9);
        Console.WriteLine($"  TwoSum([2,7,11,15], target=9): indices ({i1},{i2}) → values {nums[i1]}+{nums[i2]}=9");

        // Word Frequency
        string[] words = { "hello", "world", "hello", "c#", "world", "hello" };
        var freq = HashingProblems.WordFrequency(words);
        Console.WriteLine("  Word frequencies:");
        foreach (var kv in freq.OrderByDescending(x => x.Value))
            Console.WriteLine($"    '{kv.Key}' → {kv.Value} times");

        // First Non-Repeating Character
        string str = "aabbcddfe";
        char? first = HashingProblems.FirstNonRepeating(str);
        Console.WriteLine($"  First non-repeating in '{str}': '{first}'");  // 'c'

        // Group Anagrams
        string[] anagramInput = { "eat", "tea", "tan", "ate", "nat", "bat" };
        var groups = HashingProblems.GroupAnagrams(anagramInput);
        Console.WriteLine("  Anagram groups:");
        foreach (var group in groups)
            Console.WriteLine($"    [{string.Join(", ", group)}]");

        // Longest Consecutive Sequence
        int[] seqNums = { 100, 4, 200, 1, 3, 2 };
        int longest = HashingProblems.LongestConsecutive(seqNums);
        Console.WriteLine($"  Longest consecutive in [100,4,200,1,3,2]: {longest}");  // 4

        // Subarray with sum
        int[] subArr = { 1, 4, 20, 3, 10, 5 };
        bool found = HashingProblems.SubarrayWithSum(subArr, 33);
        Console.WriteLine($"  Subarray with sum 33 in [1,4,20,3,10,5]: {found}");  // true (20+3+10)

        // -------------------------------------------------------
        // DEMO 4: HashSet Operations
        // -------------------------------------------------------
        HashSetDemo.Run();

        // -------------------------------------------------------
        // DEMO 5: Hashtable (Legacy)
        // -------------------------------------------------------
        HashtableDemo.Run();

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Hashing: key → hash code → bucket index → O(1) avg");
        Console.WriteLine("  2. Collision handling: separate chaining (C# uses this)");
        Console.WriteLine("  3. Load factor > 0.75 triggers resize/rehash → O(n)");
        Console.WriteLine("  4. Dictionary<K,V>: generic, type-safe, O(1) avg get/put");
        Console.WriteLine("  5. HashSet<T>: unique keys only, fast O(1) membership check");
        Console.WriteLine("  6. Hashtable: legacy, non-generic, avoid in new code");
        Console.WriteLine("  7. Worst case O(n) if all keys hash to same bucket (bad hash)");
        Console.WriteLine("  8. TryGetValue is safer than direct [] indexer (no exception)");
        Console.WriteLine("======================================================");
    }
}
