# 4. Collections Deep Dive

C# collections are built on a set of interfaces that define capabilities.

## IEnumerable<T>
- The base interface for iteration.
- Provides `GetEnumerator()` to support `foreach`.
- Read-only (does not allow modification).

## ICollection<T>
- Extends `IEnumerable<T>`.
- Adds `Count`, `Add`, `Remove`, `Contains`, `Clear`.
- Represents a general collection.

## IList<T>
- Extends `ICollection<T>`.
- Adds indexed access: `this[int index]`, `IndexOf`, `Insert`, `RemoveAt`.

### Typical implementers
- `List<T>` implements `IList<T>`, `ICollection<T>`, `IEnumerable<T>`.
- `HashSet<T>` implements `ICollection<T>`, `IEnumerable<T>`.
- `Dictionary<TKey, TValue>` implements `ICollection<KeyValuePair<TKey,TValue>>`, `IEnumerable<KeyValuePair<TKey,TValue>>`.

---

## ✅ Interview Prep (Collections Deep Dive)
- Explain the relationship between `IEnumerable`, `ICollection`, and `IList`.
- Understand when a method should accept `IEnumerable<T>` vs `IList<T>`.
- Describe why using the most general interface helps with flexibility (e.g., `IEnumerable<T>` for read-only iteration).

---

## Concepts checklist — covered?

- Interfaces and common implementers: `IEnumerable<T>`, `ICollection<T>`, `IList<T>`, `IDictionary<TKey,TValue>`, `ISet<T>` (HashSet).
- Differences: mutability, indexed access, counts, and performance characteristics.
- When to accept which interface in APIs (use least-powerful required).

## Interview Q&A, edge cases & answers

- Q: "When should a method accept `IEnumerable<T>` vs `IList<T>`?"
	- A: "If you only need to iterate, accept `IEnumerable<T>`; if you need Count or Add/Remove, accept `ICollection<T>`; if you need indexing, accept `IList<T>`. Using the least-powerful interface increases flexibility."

- Q: "What happens if you modify a List while enumerating it?"
	- A: "Most built-in collections will throw `InvalidOperationException`. To avoid this, iterate over a snapshot (`ToList()`) or collect changes and apply them after iteration."

- Q: "When should you use `Dictionary` vs `List` vs `HashSet`?"
	- A: "Use `Dictionary<TKey,TValue>` for fast key-based lookup, `List<T>` for ordered collections with index access, and `HashSet<T>` when membership uniqueness and O(1) lookups are needed."

## Pros / Cons / Use cases

- `List<T>`
	- Pros: simple, fast indexed access, good memory locality.
	- Cons: O(n) lookup by value, resizing costs when growing.
	- Use: ordered collections, buffers, stacks/queues via adapters.

- `Dictionary<TKey,TValue>`
	- Pros: O(1) average lookup, flexible key types.
	- Cons: memory overhead, not ordered (use `OrderedDictionary` or `SortedDictionary` if ordering needed).
	- Use: caches, lookup tables.

- `HashSet<T>`
	- Pros: enforces uniqueness, O(1) membership checks.
	- Cons: no ordering, higher memory use.
	- Use: deduplication, set operations (Union/Intersect).

## Edge cases & performance considerations

- Capacity and resizing: `List<T>` doubles capacity; pre-size lists with known size using the constructor to avoid reallocations.
- Enumeration performance: `List<T>` is usually fastest due to contiguous memory; linked lists are slower for indexed access.
- Thread-safety: collections are not thread-safe by default. Use concurrent collections (`ConcurrentDictionary`, etc.) for multi-threaded access.

## Quick practice tasks

- Implement a method that safely removes items from a list while iterating (collect and remove afterwards).
- Replace a `List<T>` lookup with a `Dictionary<TKey, T>` when performance requires O(1) lookups.
- Show how `IReadOnlyList<T>` can be used to expose indexed access without allowing mutation.
