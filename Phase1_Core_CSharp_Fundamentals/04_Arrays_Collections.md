# 4. Arrays & Collections

## Arrays
- Fixed-size, contiguous block of memory.
- Accessed by index.

```csharp
int[] numbers = new int[3] { 1, 2, 3 };
string[] names = { "Alice", "Bob" };
```

### List<T>
- Dynamic-size collection.
- Backed by an array internally; resizes automatically.

```csharp
var list = new List<string> { "apple", "banana" };
list.Add("cherry");
```

### Dictionary<TKey, TValue>
- Stores key/value pairs.
- Uses hashing (hash code + buckets) internally.
- Fast lookups, additions, and removals (amortized O(1)).

```csharp
var dict = new Dictionary<string, int>
{
    ["apple"] = 1,
    ["banana"] = 2
};
```

### HashSet<T>
- Unordered collection of unique items.
- Uses hashing for fast membership tests and insertion.

```csharp
var set = new HashSet<int> { 1, 2, 3 };
set.Add(2); // no-op, already exists
```

👉 **Interview focus**: List vs Array.

- **Array**: fixed size, better for tight memory and when size is known.
- **List<T>**: resizable, easier API, good for dynamic collections.

👉 **Interview focus**: How `Dictionary` works internally.

- Each key is hashed to an integer (`GetHashCode`).
- Hash code determines a bucket index.
- Collisions are handled via chaining (linked list) or probing.
- The dictionary resizes when load factor grows too large.

---

## ✅ Interview Prep Checklist (Arrays & Collections)
- Explain the difference between arrays and `List<T>`.
- Describe how `Dictionary<TKey, TValue>` uses hashing and why good hash code distribution matters.
- Explain when to use `HashSet<T>` vs `List<T>`.
