# Phase 19: Advanced Collections

## Overview

C# offers specialized collection types beyond the basic `List<T>` and `Dictionary<K,V>`. Knowing when to use each one is critical for writing performant, correct code.

---

## The Collection Interface Hierarchy

```
IEnumerable<T>           ← Supports foreach (only: GetEnumerator)
    └── ICollection<T>   ← Count, Add, Remove, Contains, CopyTo
            ├── IList<T>            ← Index access (this[int])
            │       └── List<T>
            │       └── Array (T[])
            └── IDictionary<K,V>    ← Key-value access (this[K])
                    └── Dictionary<K,V>
                    └── SortedDictionary<K,V>

IEnumerable<T>
    └── ISet<T>          ← Set operations: UnionWith, IntersectWith, IsSubsetOf
            └── HashSet<T>
            └── SortedSet<T>

IReadOnlyList<T>         ← Read-only index access (no Add/Remove)
IReadOnlyDictionary<K,V> ← Read-only key access
IReadOnlyCollection<T>   ← Read-only Count + GetEnumerator
```

---

## Sorted Collections

### SortedList\<K, V\>
- Backed by **two parallel arrays** (keys array + values array)
- Keys always sorted
- Binary search for O(log n) lookup
- **Random insert: O(n)** — must shift array elements
- **Good when:** Reading much more than writing; need index-based access (`list.Values[0]`)
- **Bad when:** Frequent insertions/deletions at random positions

```csharp
var scores = new SortedList<string, int>();
scores["Alice"] = 95;
scores["Charlie"] = 87;
scores["Bob"] = 92;
// Internally: keys = ["Alice", "Bob", "Charlie"], values = [95, 92, 87]
// Access by index: scores.Values[0] = 95 (Alice's score)
// Access by key:   scores["Bob"] = 92
```

### SortedDictionary\<K, V\>
- Backed by a **Red-Black Tree** (self-balancing BST)
- Keys always sorted
- O(log n) for insert, delete, AND lookup
- **No index-based access** (can't do `dict.Values[0]`)
- **Good when:** Frequently inserting/deleting while maintaining sorted order
- **Bad when:** You need access by index position

```csharp
var events = new SortedDictionary<DateTime, string>();
events[new DateTime(2024, 3, 15)] = "Product launch";
events[new DateTime(2024, 1, 1)] = "New year";
// Always iterated in date order!
foreach (var (date, name) in events)
    Console.WriteLine($"{date:d}: {name}");
```

### SortedSet\<T\>
- Red-Black Tree of **unique values** in sorted order
- O(log n) insert, delete, contains
- **Unique feature: `GetViewBetween(lo, hi)`** — returns a live view of a range; O(log n)
- Bidirectional traversal (`Min`, `Max`, `Reverse()`)

```csharp
var temperatures = new SortedSet<double> { 22.5, 18.0, 25.3, 19.8 };
temperatures.Add(21.1);
Console.WriteLine(temperatures.Min); // 18.0
Console.WriteLine(temperatures.Max); // 25.3

// Get all temps between 19 and 23
var mild = temperatures.GetViewBetween(19.0, 23.0);
// mild = {19.8, 21.1, 22.5}
```

---

## SortedList vs SortedDictionary vs SortedSet

| Feature | SortedList\<K,V\> | SortedDictionary\<K,V\> | SortedSet\<T\> |
|---------|-----------|----------|--------|
| Data structure | Two arrays | Red-Black Tree | Red-Black Tree |
| Insert | O(n) | O(log n) | O(log n) |
| Delete | O(n) | O(log n) | O(log n) |
| Lookup by key | O(log n) | O(log n) | O(log n) |
| Access by index | ✅ O(log n) | ❌ | ❌ |
| Memory | Less (arrays) | More (nodes) | Less than dict |
| Values stored | Key + value | Key + value | Value only |
| Range query | ❌ | ❌ | ✅ GetViewBetween |
| Use case | Read-heavy sorted dict | Balanced insert + lookup | Unique sorted values |

---

## ObservableCollection\<T\>

Raises events whenever the collection changes. Part of `System.Collections.ObjectModel`.

```csharp
var items = new ObservableCollection<string>();

// Subscribe to changes BEFORE modifying
items.CollectionChanged += (sender, e) =>
{
    Console.WriteLine($"Action: {e.Action}");
    if (e.NewItems != null) Console.WriteLine($"Added: {string.Join(", ", e.NewItems.Cast<string>())}");
    if (e.OldItems != null) Console.WriteLine($"Removed: {string.Join(", ", e.OldItems.Cast<string>())}");
};

items.Add("Apple");     // Action: Add, Added: Apple
items.Remove("Apple");  // Action: Remove, Removed: Apple
items.Add("Banana");
items.Clear();          // Action: Reset
```

**Primary use case:** WPF/MAUI data binding — UI automatically updates when collection changes.

Key events in `NotifyCollectionChangedEventArgs`:
- `Action`: `Add`, `Remove`, `Replace`, `Move`, `Reset`
- `NewItems`: Items that were added
- `OldItems`: Items that were removed
- `NewStartingIndex`, `OldStartingIndex`: Where in the list

---

## ReadOnlyCollection\<T\>

Wraps a mutable collection to expose it as read-only. A **view** — not a copy!

```csharp
var mutableList = new List<string> { "A", "B", "C" };
IReadOnlyList<string> readOnly = mutableList.AsReadOnly();

// readOnly.Add("D"); // ← Compile error! No Add method
Console.WriteLine(readOnly[0]);  // ✅ Read access is fine
Console.WriteLine(readOnly.Count); // ✅

// The original is still mutable:
mutableList.Add("D");
Console.WriteLine(readOnly[3]); // ✅ "D" — it's a live view!
```

**Return `IReadOnlyList<T>` from methods** to prevent callers from mutating internal state — a common encapsulation pattern.

---

## Custom IEnumerable\<T\> Implementation

Implementing `IEnumerable<T>` allows using `foreach`, LINQ, and collection initializers.

Minimal implementation:
```csharp
public class NumberRange : IEnumerable<int>
{
    private readonly int _start;
    private readonly int _end;

    public NumberRange(int start, int end) => (_start, _end) = (start, end);

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = _start; i <= _end; i++)
            yield return i;   // C# compiler creates a state machine
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); // non-generic required
}

// Now works with foreach and LINQ:
var range = new NumberRange(1, 5);
foreach (int n in range) Console.Write(n); // 1 2 3 4 5
int sum = range.Sum();                     // LINQ works too!
```

---

## HashSet\<T\> vs SortedSet\<T\>

| Feature | HashSet\<T\> | SortedSet\<T\> |
|---------|----------|----------|
| Backing structure | Hash table | Red-Black Tree |
| Add/Remove/Contains | O(1) avg | O(log n) |
| Maintains order | ❌ | ✅ (sorted) |
| Range query | ❌ | ✅ GetViewBetween |
| Min/Max | O(n) | O(1) |
| Memory | Higher (load factor) | Lower |
| Use case | Fast set operations | Ordered unique values |

---

## When to Use Which Collection?

```
Need key-value mapping?
├─ Need sorted keys?
│   ├─ Frequent inserts/deletes? → SortedDictionary<K,V>
│   └─ Mostly reads, occasional writes? → SortedList<K,V>
└─ Don't need sorted? → Dictionary<K,V> (fastest)

Need only values (no keys)?
├─ Need sorted + unique? → SortedSet<T>
├─ Need sorted (duplicates OK)? → List<T> + Sort()
├─ Need unique only (unordered)? → HashSet<T>
└─ Just need a list? → List<T>

Need UI notifications on change? → ObservableCollection<T>
Need to expose collection as read-only? → AsReadOnly() / IReadOnlyList<T>
Need thread safety? → See Phase 20 (Concurrent Collections)
```

---

## Interview Questions & Answers

**Q1: When would you use SortedDictionary over a regular Dictionary?**

**A:** When you need key-value storage **and** need to iterate in sorted key order efficiently. A regular `Dictionary<K,V>` offers O(1) average lookup but no sorted traversal. `SortedDictionary<K,V>` uses a Red-Black Tree giving O(log n) for all operations, but always iterates in sorted key order. Examples: ordered event log, leaderboard (sorted by score), word frequency analysis where you want results alphabetically.

---

**Q2: What's the difference between SortedList and SortedDictionary?**

**A:** Both maintain sorted key order, but differ in backing structure:
- `SortedList<K,V>`: Two parallel arrays. O(n) insert (array shift), O(log n) lookup, supports index-based access (`list.Values[i]`), less memory.
- `SortedDictionary<K,V>`: Red-Black Tree. O(log n) insert AND lookup, no index access, more memory per node.
Choose `SortedList` for read-heavy scenarios or when index access needed. Choose `SortedDictionary` when insert/delete frequency is comparable to reads.

---

**Q3: Why use ObservableCollection instead of List in WPF/MAUI?**

**A:** WPF/MAUI data binding subscribes to `INotifyCollectionChanged`. When you bind a `List<T>` to a UI control and add an item, the UI doesn't know the list changed — it won't update. `ObservableCollection<T>` raises `CollectionChanged` events on every Add/Remove/Replace/Move, so the bound UI control refreshes automatically. It's essentially a `List<T>` + event notification. For programmatic (non-UI) use, `List<T>` is faster with no overhead.

---

**Q4: What does AsReadOnly() return and why would you use it?**

**A:** `AsReadOnly()` returns a `ReadOnlyCollection<T>` wrapping the original list — it's a **view, not a copy**. Changes to the original list are visible through the read-only wrapper, but callers cannot mutate it. Use it to enforce encapsulation: expose internal collections without allowing external code to add/remove items. Return type should be `IReadOnlyList<T>` for best flexibility. If you need a true copy that's isolated from changes, use `list.ToList().AsReadOnly()` or just expose `IEnumerable<T>`.

---

**Q5: How would you implement a custom collection that supports LINQ and foreach?**

**A:** Implement `IEnumerable<T>` (and its non-generic form `IEnumerable`). The heart is `GetEnumerator()` which returns an `IEnumerator<T>`. The most common approach is using `yield return` in the implementation — the C# compiler generates a state machine class automatically. This is sufficient for full LINQ support and foreach compatibility. For a full collection (`Add`, `Remove`, `Count`), also implement `ICollection<T>`.

---

**Q6: When would you choose SortedSet over HashSet?**

**A:** When you need **both uniqueness and sorted order**. `HashSet<T>` is faster (O(1) vs O(log n)) but has no ordering. Use `SortedSet<T>` when: (1) you need to iterate in sorted order, (2) you need range queries via `GetViewBetween(lo, hi)`, (3) you need min/max efficiently (O(1) via `Min`/`Max` properties). Example: maintaining a set of active session expiration times and needing to efficiently find all sessions expiring within the next 5 minutes.

---

## Scenario-Based Questions

**Scenario 1:** You're building a task management system where tasks have deadlines (DateTime). Users frequently add, remove, and reschedule tasks but always need to view the next upcoming task. Which collection would you use?

**Answer:** Use `SortedDictionary<DateTime, Task>` (or `SortedSet<Task>` with IComparable by deadline). The red-black tree gives O(log n) add/remove/reschedule. The next upcoming task is always `dict.Keys.First()` or `set.Min`. If two tasks can share a deadline, use `SortedDictionary<DateTime, List<Task>>`. If using a priority queue for dequeue-and-process pattern instead of random removal, use `PriorityQueue<Task, DateTime>`.

---

**Scenario 2:** You have a public-facing API method that returns your internal list of users. You want callers to iterate users but not be able to add or remove from the list (protecting internal state), while still seeing live updates when your internal list changes. How do you expose this?

**Answer:** Store the internal list as `List<User>`. In the public property, return `_users.AsReadOnly()` typed as `IReadOnlyList<User>`. This returns a `ReadOnlyCollection<User>` — a live view wrapper. Callers can iterate (foreach) and index (`[i]`), but any attempt to call `Add()` or `Remove()` results in a compile error. If the internal list changes (you add a user internally), the read-only view reflects the change immediately since it's a view not a copy. For API clients who shouldn't see live updates, return `_users.ToArray()` (a true copy).

---

**Scenario 3:** Build an autocomplete system that stores city names. Given a prefix, find all cities starting with that prefix in alphabetical order. The set of cities changes rarely but is queried millions of times.

**Answer:** Use a `SortedSet<string>` of city names. For prefix search, use `GetViewBetween(prefix, prefix + "\uffff")` — this leverages the sorted structure to return only names in the range [prefix, prefix + max char) in O(log n + k) where k is the number of results. The `\uffff` (largest Unicode character) ensures any string starting with the prefix falls in range. This is efficient, requires no external indices, and lives entirely in memory. For very large datasets, a Trie may be more efficient, but SortedSet is elegant for moderate size.

---

## Common Mistakes

1. **Using SortedList for frequent writes** — O(n) insert due to array shifting; use SortedDictionary for balanced insert+lookup
2. **Confusing SortedDictionary with Dictionary sorted after insertion** — Dictionary is NEVER sorted; SortedDictionary maintains sort via tree structure
3. **Expecting ObservableCollection to update on item property changes** — CollectionChanged fires for Add/Remove/Replace only; for property changes within items, items must implement INotifyPropertyChanged
4. **Assuming AsReadOnly() copies the data** — it returns a live view wrapper; original changes are visible through it
5. **Implementing IEnumerable without the non-generic version** — always also implement `IEnumerable.GetEnumerator()` (the non-generic form required by foreach pre-generics)
6. **Using SortedSet when duplicates are expected** — SortedSet rejects duplicates; use SortedDictionary with List<T> values if duplicates needed with sorted keys
