# Phase 21: Immutable Collections

## What is Immutability?

An **immutable** object cannot be modified after creation. Any "modification" creates and returns a **new object** with the change applied; the original remains unchanged.

```csharp
// Mutable (regular) list:
var list = new List<int> { 1, 2, 3 };
list.Add(4);   // modifies list IN PLACE → list = [1,2,3,4]

// Immutable list:
var imList = ImmutableList.Create(1, 2, 3);
var newList = imList.Add(4);  // returns NEW list → imList still = [1,2,3]
                              //                    newList = [1,2,3,4]
```

---

## Why Use Immutable Collections?

| Reason | Explanation |
|--------|-------------|
| **Thread safety** | No locking needed — multiple threads can read simultaneously; no writes ever occur |
| **Predictability** | Functions that receive an immutable collection cannot modify it — pure functions |
| **History/undo** | Keep old versions by storing references (structural sharing conserves memory) |
| **Defensive copies** | Expose internal state without fear callers will mutate it |
| **Functional patterns** | Foundation of functional programming style in C# |
| **No defensive cloning** | No need to `ToArray()` or `.ToList()` to protect internal state |

---

## Namespace & Package

```csharp
using System.Collections.Immutable;
// Requires NuGet: System.Collections.Immutable
// (Included by default in .NET 5+)
```

---

## ImmutableList\<T\>

Backed by an **AVL tree** (balanced BST). O(log n) for all operations.

```csharp
// Creation
ImmutableList<int> list = ImmutableList.Create(1, 2, 3, 4, 5);
ImmutableList<int> fromLinq = Enumerable.Range(1, 5).ToImmutableList();

// All operations return a NEW ImmutableList — original unchanged
ImmutableList<int> added    = list.Add(6);          // [1,2,3,4,5,6]
ImmutableList<int> inserted = list.Insert(0, 0);    // [0,1,2,3,4,5]
ImmutableList<int> removed  = list.Remove(3);       // [1,2,4,5]  (removes first 3)
ImmutableList<int> replaced = list.SetItem(2, 99);  // [1,2,99,4,5] (index 2 = 99)
ImmutableList<int> ranged   = list.AddRange(new[]{6,7,8});

// Reading is the same as List<T>
int first = list[0];           // 1
int count = list.Count;        // 5
bool has3 = list.Contains(3);  // true
int idx   = list.IndexOf(4);   // 3
```

---

## ImmutableDictionary\<K, V\>

```csharp
// Creation
var dict = ImmutableDictionary.Create<string, int>();
var withDefaults = ImmutableDictionary<string, int>.Empty
    .Add("Alice", 95)
    .Add("Bob", 88);

// All operations return new dictionary
var updated = withDefaults.SetItem("Alice", 99);   // update existing
var withNew = withDefaults.Add("Charlie", 72);      // add new key
var removed = withDefaults.Remove("Bob");           // remove key

// Reading
int aliceScore = withDefaults["Alice"];             // 95
bool hasBob    = withDefaults.ContainsKey("Bob");   // true
withDefaults.TryGetValue("Dave", out int val);      // val = 0, returns false
```

---

## ImmutableHashSet\<T\>

```csharp
var setA = ImmutableHashSet.Create(1, 2, 3, 4);
var setB = ImmutableHashSet.Create(3, 4, 5, 6);

// Set operations — all return NEW sets
var union     = setA.Union(setB);      // {1,2,3,4,5,6}
var intersect = setA.Intersect(setB);  // {3,4}
var except    = setA.Except(setB);     // {1,2}

var added   = setA.Add(10);   // {1,2,3,4,10}
var removed = setA.Remove(2); // {1,3,4}
bool has3   = setA.Contains(3); // true
```

---

## ImmutableArray\<T\>

Backed by a plain **array** — zero overhead wrapper. Most efficient for read-only access.

```csharp
ImmutableArray<int> arr = ImmutableArray.Create(1, 2, 3);

int first = arr[0];           // 1 — O(1) like normal array
int len   = arr.Length;       // 3

// "Mutation" creates a new array (O(n) — must copy)
ImmutableArray<int> added    = arr.Add(4);       // [1,2,3,4]
ImmutableArray<int> replaced = arr.SetItem(1, 99); // [1,99,3]

// Can convert to Span for zero-allocation reads
ReadOnlySpan<int> span = arr.AsSpan();
```

---

## ImmutableList vs ImmutableArray

| Feature | ImmutableList\<T\> | ImmutableArray\<T\> |
|---------|-----------|----------|
| Backing | AVL tree | Plain array |
| Indexed access | O(log n) | O(1) |
| Add/Remove | O(log n) — structural sharing | O(n) — full copy |
| Memory | Higher (tree nodes) | Lower (just the array) |
| Equality | Reference equality | Value equality (element-by-element) |
| Cache performance | Poor (scattered tree nodes) | Excellent (contiguous memory) |
| **Best for** | Frequent modifications | Rarely modified, fast reads |
| `default` value | `null` (reference type) | Empty (struct — BEWARE!) |

**ImmutableArray gotcha:** `ImmutableArray<T>` is a `struct`, so its default is an uninitialized struct — accessing `.Length` throws `NullReferenceException`! Always initialize: use `ImmutableArray<T>.Empty` or `ImmutableArray.Create(...)`.

---

## Builder Pattern — Efficient Batch Modifications

Creating a new immutable collection for every change is expensive. Use **builders** for bulk operations:

```csharp
// Inefficient: creates N intermediate immutable lists
var list = ImmutableList<int>.Empty;
for (int i = 0; i < 1000; i++)
    list = list.Add(i); // 1000 allocations!

// Efficient: use a Builder
var builder = ImmutableList.CreateBuilder<int>();
for (int i = 0; i < 1000; i++)
    builder.Add(i); // mutable during construction
ImmutableList<int> result = builder.ToImmutable(); // ONE immutable result
```

Builder is a **mutable** temporary object. Once you call `ToImmutable()`, it returns an immutable collection. The builder can then be discarded.

```csharp
// Dictionary builder example
var dictBuilder = ImmutableDictionary.CreateBuilder<string, int>();
dictBuilder["key1"] = 1;
dictBuilder["key2"] = 2;
dictBuilder.Remove("key1");
ImmutableDictionary<string, int> finalDict = dictBuilder.ToImmutable();
```

---

## Immutable State with Record Types

C# `record` types combine immutability with easy "copy with changes":

```csharp
public record GameState(
    int Score,
    int Lives,
    ImmutableList<string> Inventory
);

// Create initial state
var state = new GameState(0, 3, ImmutableList<string>.Empty);

// "Mutate" — creates a new state, leaves old state intact
var withPoints = state with { Score = state.Score + 100 };
var withItem   = state with { Inventory = state.Inventory.Add("Sword") };

// Old state unchanged — can use for undo/history
Console.WriteLine(state.Score);     // 0
Console.WriteLine(withPoints.Score); // 100
```

This pattern (immutable records + immutable collections) is used in **Redux** and **Event Sourcing** architectures.

---

## Performance Comparison

| Operation | List\<T\> | ImmutableList\<T\> | ImmutableArray\<T\> |
|-----------|----------|----------|----------|
| Read by index | O(1) | O(log n) | O(1) |
| Add to end | O(1) amortized | O(log n) | O(n) |
| Insert at index | O(n) | O(log n) | O(n) |
| Remove element | O(n) | O(log n) | O(n) |
| Contains | O(n) | O(n) | O(n) |
| Thread safety | ❌ Needs lock | ✅ Built-in | ✅ Built-in |

---

## Interview Questions & Answers

**Q1: What is the difference between `IReadOnlyList<T>` and `ImmutableList<T>`?**

**A:** Both prevent callers from adding/removing, but in different ways:
- `IReadOnlyList<T>` is just an **interface**. The underlying object may be a mutable `List<T>` — someone else holding the original reference can still modify it, and your read-only view will reflect those changes.
- `ImmutableList<T>` is a **truly immutable data structure**. Nobody can modify it — ever. Once created, the contents are guaranteed to never change.
Use `IReadOnlyList<T>` to convey "the caller shouldn't modify this." Use `ImmutableList<T>` when you need a strong guarantee of no changes, for thread safety, or to preserve versions.

---

**Q2: Why is `ImmutableArray<T>` typically more efficient than `ImmutableList<T>` for reads?**

**A:** `ImmutableArray<T>` wraps a plain array — index access is O(1) with excellent cache locality (contiguous memory). `ImmutableList<T>` uses an AVL tree — index access is O(log n) traversal through tree nodes scattered across memory (poor cache performance). For modifications, they're reversed: ImmutableList uses structural sharing (only modified branches reallocated), so bulk modifications are O(log n) per operation. ImmutableArray must copy the entire array on any modification, O(n). Choose ImmutableArray for collections modified rarely but read frequently; ImmutableList for collections that change often.

---

**Q3: What is structural sharing in immutable collections?**

**A:** When you "modify" an immutable data structure (e.g., add an element to ImmutableList), instead of copying the entire structure, the new version **reuses most of the original structure** and only allocates new nodes for the changed path. In an AVL tree with n elements, adding one element only creates O(log n) new nodes — the rest are shared with the original. Memory savings: if you have 100 versions of a 1000-element list, you don't have 100,000 node allocations; most nodes are shared across versions. This makes "copy-on-write" efficient.

---

**Q4: Explain the ImmutableArray struct default value gotcha.**

**A:** `ImmutableArray<T>` is a `struct`, so its default value (from `new ImmutableArray<T>()` or uninitialized field) is an uninitialized struct wrapping `null` internally. Calling `.Length`, `Count`, or any method on this default value throws `NullReferenceException` — confusing since it's a value type! Always initialize with `ImmutableArray<T>.Empty` or `ImmutableArray.Create(...)`. Also: check `array.IsDefault` before use to detect uninitialized state.

---

**Q5: When would you choose immutable collections over concurrent collections for thread safety?**

**A:** Immutable collections are inherently thread-safe because they cannot be modified — concurrent reads need no synchronization. Concurrent collections allow modifications but use locking internally. Choose immutable when: (1) The collection changes infrequently (rebuild and swap the reference), (2) You need to share a "snapshot" across multiple threads that should all see the same view, (3) You're doing event sourcing or maintaining history. Choose concurrent when: Multiple threads continuously produce/consume items; high write throughput is required. The tradeoff: immutable collections have coordination-free reads but O(n) or O(log n) writes; concurrent collections can have O(1) writes but need synchronization.

---

**Q6: How does the Builder pattern help with immutable collections?**

**A:** Creating one immutable collection per operation (in a loop) causes O(n) allocations and is much slower than building a mutable list and converting. The Builder is a temporary mutable counterpart — use it, then call `ToImmutable()` to get one final, efficient immutable collection. This is especially important in hotpaths and initialization. Example: initializing a 10,000-element ImmutableList: building with a builder takes O(n) total; building by chained `.Add()` calls creates 10,000 intermediate ImmutableLists — O(n log n) total work and heavy GC pressure.

---

## Scenario-Based Questions

**Scenario 1:** You're building a multi-player card game server. The game state (deck, players, scores) must be shared with all N player threads. Players can't modify each other's state. When a move occurs, a new state snapshot is created. Old states should remain accessible for replay. How do you design this?

**Answer:** Use immutable records + immutable collections for game state. `GameState` is an immutable record with `ImmutableList<Card>` for the deck, `ImmutableDictionary<int, PlayerState>` for players. When a player makes a move, compute the new state (fast with structural sharing) and atomically update a shared reference: `Interlocked.Exchange(ref _currentState, newState)`. Reader threads always see a consistent snapshot; no reader is blocked by writers. Store the last N states in a `Stack<GameState>` for replays. This matches event sourcing and Redux-style state management.

---

**Scenario 2:** Your API returns a snapshot of configuration that should never change while a request is processing, even if another thread is updating the config. How do you guarantee this?

**Answer:** Store configuration in an `ImmutableDictionary<string, ConfigValue>`. The update path: read current reference, rebuild with changes, atomically swap the reference using `Interlocked.CompareExchange`. Each request handler takes a local copy of the reference _once at request start_: `var config = _currentConfig;`. Since the dictionary is immutable, no subsequent updates will affect this reference — the request sees a consistent snapshot throughout its lifetime without any locking. This is the "published-immutable" pattern used in many ASP.NET Core configuration systems.

---

**Scenario 3:** You're implementing a version history feature. Each document edit should be undoable. You expect users to frequently undo/redo through up to 100 steps.

**Answer:** Use a `Stack<ImmutableDocument>` for undo history and another for redo. `ImmutableDocument` contains `ImmutableList<string>` for lines and other metadata. On edit: push current state to undo stack, compute new state (fast with structural sharing — only changed lines are new nodes), set as current. Undo: pop from undo stack, push current to redo stack, restore popped state. Since ImmutableList uses structural sharing, 100 versions of a 1000-line document don't consume 100,000 node allocations — most nodes are shared. Memory cost is proportional to the total number of distinct changes, not total document size × version count.

---

## Common Mistakes

1. **Ignoring the return value** — `list.Add(item)` in ImmutableList returns the new list; discarding it means the "add" has no effect
2. **Flying ImmutableArray default trap** — `ImmutableArray<T>` default struct is broken; always use `.Empty` or `Create()`
3. **Avoiding builders in loops** — building by chained `.Add()` inside a loop creates O(n²) work; always use the `Builder` for bulk initialization
4. **Treating IReadOnlyList as immutable** — it's not; the underlying data may change
5. **Using ImmutableList when ImmutableArray suffices** — ImmutableArray is dramatically faster for read-heavy workloads
6. **Confusing with readonly** — `readonly List<int> list` means the list reference can't change, but the list contents CAN. Use `ImmutableList<int>` for true content immutability
7. **Over-using immutability in hot paths** — immutable operations allocate; GC pressure can outweigh the threading benefits for high-frequency mutations
