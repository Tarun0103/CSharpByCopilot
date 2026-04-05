# Phase 11: Hashing & Dictionary

## What is Hashing?

**Hashing** is the process of transforming data (a key) into a fixed-size integer (a hash code) that serves as an index into an array called a **hash table**.

```
Key → Hash Function → Index → Store/Retrieve Value

"Alice" → hash("Alice") → 42 → table[42] = "Engineer"
```

---

## How a Hash Table Works Internally

1. **Hash function** converts key to an integer index
2. **Array** stores values at the computed index
3. **Collision resolution** handles when two keys hash to the same index

```
Array size = 10
"Alice"  → hash("Alice")  % 10 = 2 → store at index 2
"Bob"    → hash("Bob")    % 10 = 5 → store at index 5
"Carol"  → hash("Carol")  % 10 = 2 → COLLISION! Same as "Alice"
```

---

## Collision Resolution Strategies

### 1. Separate Chaining (C# Dictionary uses this)
Each bucket holds a linked list of key-value pairs that hash to the same index.

```
Index 2: Alice → Carol → ... (linked list)
Index 5: Bob
```

### 2. Open Addressing (Probing)
Find another empty slot in the array.
- **Linear Probing**: try index+1, index+2, ...
- **Quadratic Probing**: try index+1², index+2², ...
- **Double Hashing**: use second hash function for step size

---

## C# Dictionary<K,V> Internals

| Property | Detail |
|----------|--------|
| Implementation | Hash table with open addressing (not separate chaining in modern .NET) |
| Load factor | ~0.72 — resizes when 72% full |
| Resize strategy | Doubles capacity, rehashes all elements |
| Hash function | Key's `GetHashCode()` method |
| Time complexity | O(1) average, O(n) worst case |
| Thread safety | NOT thread-safe — use `ConcurrentDictionary` |

---

## Common Operations

```csharp
var dict = new Dictionary<string, int>();

dict["Alice"] = 30;              // Add or update
dict.Add("Bob", 25);             // Add only (throws if exists)
dict.TryAdd("Carol", 35);        // Add only (returns false if exists)
dict.TryGetValue("Alice", out int age);  // Safe get (no KeyNotFoundException)
dict.ContainsKey("Alice");       // O(1) existence check
dict.Remove("Bob");              // Remove
dict.Count;                      // Number of entries

// Iteration
foreach (var (key, value) in dict) { ... }
foreach (var key in dict.Keys)    { ... }
foreach (var val in dict.Values)  { ... }
```

---

## HashSet<T> vs Dictionary<K,V>

| Feature | HashSet<T> | Dictionary<K,V> |
|---------|-----------|----------------|
| Purpose | Unique elements | Key → Value mapping |
| Lookup | O(1) by value | O(1) by key |
| Memory | Less (no value) | More (key + value) |
| Use when | Need to track existence | Need key-value association |

---

## Time Complexity

| Operation | Average | Worst Case |
|-----------|---------|------------|
| Insert | O(1) | O(n) [resize] |
| Lookup | O(1) | O(n) [collisions] |
| Delete | O(1) | O(n) |
| Iteration | O(n) | O(n) |

---

## Common Interview Problems Using Hashing

| Problem | Technique | Complexity |
|---------|-----------|------------|
| Two Sum | dictionary: value → index | O(n) |
| Word frequency | dictionary: word → count | O(n) |
| First non-repeating | two passes with dictionary | O(n) |
| Group anagrams | dictionary: sorted_word → list | O(n·k log k) |
| Subarray with sum K | prefix sum + dictionary | O(n) |
| Longest consecutive | HashSet + expand each start | O(n) |

---

## Interview Questions & Answers

**Q1: How does a hash collision reduce to O(n)?**

**A:** In separate chaining, if all keys hash to the same index (worst case), one bucket becomes a linked list of n elements. Every lookup must scan the entire list → O(n). This happens when the hash function distributes poorly or with adversarial inputs. In practice, good hash functions + load factor control keep this at O(1) average.

---

**Q2: What must you implement when using a custom class as a Dictionary key?**

**A:** You must override `GetHashCode()` and `Equals()` (or implement `IEquatable<T>`). The contract is:
- If `Equals(a, b)` is true → `a.GetHashCode() == b.GetHashCode()`
- The reverse is not required (hash collisions are allowed)
Failing to override `GetHashCode()` when overriding `Equals()` can cause items to be "lost" in the dictionary.

---

**Q3: What is the difference between `TryGetValue` and the indexer `[]`?**

**A:** `dict[key]` throws `KeyNotFoundException` if key is missing. `TryGetValue(key, out value)` returns false without throwing. Use `TryGetValue` when the key may not exist — it's one lookup instead of two (ContainsKey + indexer).

---

**Q4: How does Two Sum work with O(n) time using hashing?**

**A:** For each number `n`, compute `complement = target - n`. Check if complement exists in the dictionary. If yes → found the pair. If no → store `n → index` in dictionary. This is O(n) single pass because each hash lookup and insert is O(1).

---

**Q5: Why is string concatenation used as a key in Group Anagrams?**

**A:** Anagrams contain the same characters with the same frequencies. Sorting the characters of each word produces the same "canonical form" regardless of order. Using this sorted string as a dictionary key groups all anagrams together: O(k log k) per word (k = word length).

---

**Q6: What is a prefix sum and how does it help find subarrays with sum K?**

**A:** A prefix sum `pre[i]` = sum of all elements from index 0 to i. The subarray sum from i to j = `pre[j] - pre[i-1]`. To find a subarray summing to K, we need `pre[j] - pre[i-1] == K` → `pre[i-1] == pre[j] - K`. Store prefix sums in a dictionary → O(1) lookup for each element = O(n) total.

---

## Scenario-Based Questions

**Scenario 1:** Design a system to detect the first duplicate in a stream of millions of user IDs (strings). Memory is limited.

**Answer:** Use a `HashSet<string>`. For each incoming ID, call `Add()` — it returns false if the ID already exists (duplicate detected). HashSet offers O(1) average lookup and insert. Memory use is O(unique elements seen). If memory is extremely limited, use a Bloom filter (probabilistic, space-efficient) with a tolerable false positive rate.

---

**Scenario 2:** Build a real-time word frequency counter for streaming text data. Support top-K frequent words query.

**Answer:** Use `Dictionary<string, int>` for frequency counts (O(1) updates). For top-K queries, use a min-heap (PriorityQueue) of size K — for each new query, scan the dictionary, maintain only top K elements. Alternatively, maintain a sorted data structure. The frequency dictionary handles streaming efficiently; the heap provides top-K in O(n log K).

---

**Scenario 3:** You're implementing a route caching system. Given a source and destination, cache the computed route. Keys are pairs (source, destination).

**Answer:** Use `Dictionary<(string, string), Route>` with a tuple as the key. C# value tuples use structural equality by default (both components compared). Implement an LRU eviction policy by combining the dictionary with a `LinkedList<(string,string)>` — this gives O(1) add/evict/lookup even as the cache grows.

---

## Common Mistakes

1. **Using mutable objects as keys** → If an object changes after insertion, its hash code changes and the dictionary can't find it
2. **Not handling missing keys** → `dict[key]` on missing key throws `KeyNotFoundException`; use `TryGetValue` or `GetValueOrDefault`
3. **Forgetting GetHashCode with custom Equals** → Breaks dictionary invariants
4. **Thread safety** → `Dictionary<K,V>` is NOT thread-safe; use `ConcurrentDictionary` for concurrent access
5. **Hashtable vs Dictionary** → `Hashtable` is legacy, non-generic, slower; always use `Dictionary<K,V>` in modern C#
6. **Assuming ordering** → `Dictionary<K,V>` does NOT guarantee insertion order; use `SortedDictionary` or `LinkedList` if order matters
