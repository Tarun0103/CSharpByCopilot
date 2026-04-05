# Phase 20: Concurrent Collections

## Why Thread Safety Matters

When multiple threads read/write a shared data structure simultaneously, the results can be unpredictable — this is called a **race condition**.

```csharp
// UNSAFE — two threads may read the same count, both increment, both write back
// Result: might only increment by 1 instead of 2
int count = 0;
Thread t1 = new Thread(() => { for (int i = 0; i < 1000; i++) count++; });
Thread t2 = new Thread(() => { for (int i = 0; i < 1000; i++) count++; });
t1.Start(); t2.Start(); t1.Join(); t2.Join();
// count is NOT reliably 2000!
```

The problem: `count++` is not atomic. It's 3 operations: read → increment → write.

---

## Solutions Overview

| Approach | Mechanism | Best For |
|----------|-----------|---------|
| `lock` | Exclusive mutex | General purpose, full control |
| `Interlocked` | CPU atomic operations | Simple integer/reference operations |
| `ConcurrentDictionary` | Fine-grained locking | Concurrent key-value access |
| `ConcurrentQueue` | Lock-free algorithm | Producer-consumer queues |
| `ConcurrentBag` | Thread-local storage | Unordered frequently add/take same thread |
| `ConcurrentStack` | Lock-free algorithm | Concurrent LIFO access |
| `BlockingCollection` | Blocking wrapper + bound | Bounded producer-consumer |

---

## `Interlocked` — Atomic Integer Operations

`System.Threading.Interlocked` performs thread-safe arithmetic without locking.

```csharp
int count = 0;

// Atomic increment (equivalent to count++ but thread-safe)
Interlocked.Increment(ref count);

// Atomic decrement
Interlocked.Decrement(ref count);

// Atomic add (returns new value)
int newVal = Interlocked.Add(ref count, 5);

// Compare-and-swap: if count == expected, replace with newValue
int old = Interlocked.CompareExchange(ref count, newValue, expected);

// Atomic exchange (set to new value, returns old value)
int prev = Interlocked.Exchange(ref count, 0); // reset to 0
```

**Interlocked is faster than lock** for simple counters — no context switch, just a CPU-level lock prefix instruction.

---

## `ConcurrentDictionary<K,V>`

Thread-safe dictionary. Uses **striped locking** — divides buckets into segments, each with its own lock. Multiple threads can read/write different keys simultaneously.

```csharp
var cache = new ConcurrentDictionary<string, int>(
    concurrencyLevel: Environment.ProcessorCount,  // # of lock stripes
    capacity: 100                                   // initial capacity
);

// AddOrUpdate — thread-safe "upsert"
// If key absent: set value to 1
// If key present: call updateFactory with existing value
cache.AddOrUpdate("visits",
    addValue: 1,
    updateValueFactory: (key, currentValue) => currentValue + 1
);

// GetOrAdd — thread-safe "get or create"
string result = cache.GetOrAdd("userId",
    valueFactory: key => ExpensiveCompute(key)
);

// TryUpdate — only update if current value matches expected
bool updated = cache.TryUpdate("counter",
    newValue: 100,
    comparisonValue: 50  // won't update if current != 50
);

// TryRemove
cache.TryRemove("key", out int removedValue);

// TryGetValue
if (cache.TryGetValue("key", out int val))
    Console.WriteLine(val);
```

**Important caveat:** `AddOrUpdate` and `GetOrAdd` calls with factory functions are NOT atomic. The factory may run multiple times in race conditions — the factory should have no side effects.

---

## `ConcurrentQueue<T>`

Lock-free FIFO queue using linked segments. Multiple producers and consumers safe.

```csharp
var queue = new ConcurrentQueue<string>();

// Enqueue is always safe
queue.Enqueue("Task 1");
queue.Enqueue("Task 2");

// TryDequeue — returns false if queue is empty (non-blocking)
if (queue.TryDequeue(out string item))
    Console.WriteLine($"Processing: {item}");

// TryPeek — look without removing
if (queue.TryPeek(out string next))
    Console.WriteLine($"Next: {next}");

Console.WriteLine($"Remaining: {queue.Count}");
Console.WriteLine($"Is empty: {queue.IsEmpty}");
```

---

## `BlockingCollection<T>` — Producer-Consumer Pattern

Wraps any `IProducerConsumerCollection<T>` (default: `ConcurrentQueue`) and adds:
- **Blocking behavior** — `Take()` blocks if empty; `Add()` blocks if at capacity
- **Bounded capacity** — optional max size
- **Completion signaling** — `CompleteAdding()` lets consumers know no more items will come

```csharp
// Bounded buffer — producers block if 10 items are queued
using var buffer = new BlockingCollection<int>(boundedCapacity: 10);

// Producer task
Task producer = Task.Run(() =>
{
    for (int i = 1; i <= 20; i++)
    {
        buffer.Add(i);     // Blocks if buffer is full!
        Console.WriteLine($"Produced: {i}");
    }
    buffer.CompleteAdding();  // Signal: no more items coming
});

// Consumer task
Task consumer = Task.Run(() =>
{
    // GetConsumingEnumerable: lazy IEnumerable that blocks for each item
    // Terminates after CompleteAdding() is called AND queue is empty
    foreach (int item in buffer.GetConsumingEnumerable())
        Console.WriteLine($"Consumed: {item}");
});

await Task.WhenAll(producer, consumer);
```

**Multiple consumers** can all call `GetConsumingEnumerable()` — each item is delivered to exactly one consumer (work distribution, not broadcast).

---

## `lock` vs Concurrent Collections

### When to use `lock`:
```csharp
private readonly object _lock = new object();
private List<int> _list = new List<int>();

public void AddItem(int item)
{
    lock (_lock)           // Only one thread at a time
    {
        _list.Add(item);
    }
}
```
- **Pros:** Simple, full control over which operations are atomic together
- **Cons:** Contention (waiting), deadlock risk (nesting locks), coarse-grained

### When to use Concurrent Collections:
- **Pros:** Designed for concurrency, fine-grained locking, some lock-free
- **Cons:** Less control, some operations not atomic across multiple calls

### Multi-step atomicity — still needs lock:
```csharp
var dict = new ConcurrentDictionary<string, int>();

// WRONG — Check-then-Act is not atomic even with ConcurrentDictionary!
if (!dict.ContainsKey("key"))
    dict.TryAdd("key", ComputeValue()); // Another thread may add between these lines

// CORRECT — use GetOrAdd to make it atomic
dict.GetOrAdd("key", _ => ComputeValue());
```

---

## Thread Safety Summary

| Collection | Thread-Safe? | Notes |
|-----------|-------------|-------|
| `List<T>` | ❌ | Use `lock` or ConcurrentBag |
| `Dictionary<K,V>` | ❌ | Use ConcurrentDictionary |
| `Queue<T>` | ❌ | Use ConcurrentQueue |
| `Stack<T>` | ❌ | Use ConcurrentStack |
| `ConcurrentDictionary` | ✅ | Striped locking |
| `ConcurrentQueue` | ✅ | Lock-free |
| `ConcurrentStack` | ✅ | Lock-free |
| `ConcurrentBag` | ✅ | Thread-local storage |
| `BlockingCollection` | ✅ | Blocking wrapper |
| `ImmutableList` | ✅ | Immutable — no writes at all! |

---

## Interview Questions & Answers

**Q1: What is a race condition? Give an example.**

**A:** A race condition occurs when two or more threads access shared data concurrently and at least one modifies it, resulting in behavior that depends on the timing of thread scheduling. Example: two threads both do `count++`. Internally this is: `int temp = count; temp++; count = temp;`. If both threads read `count = 5` before either writes back, both write `6` — increment only happens once instead of twice. Fix: `Interlocked.Increment(ref count)` or `lock(_obj) { count++; }`.

---

**Q2: What is the difference between ConcurrentDictionary and a Dictionary protected by a lock?**

**A:**
- **`lock` + `Dictionary`**: One lock for the entire dictionary. Only one thread can enter any dictionary operation at a time. Simple, predictable, but high contention.
- **`ConcurrentDictionary`**: Uses striped locking — divides buckets into N stripes (N ≈ processor count), each with its own lock. Threads accessing different key hash buckets can proceed simultaneously, reducing contention. Also provides atomic combined operations: `GetOrAdd`, `AddOrUpdate`, `TryUpdate` that aren't possible atomically with a plain dictionary + lock without more complex code.
Use `ConcurrentDictionary` for concurrent scenarios. Use `lock + Dictionary` when you need multiple dictionary operations to be atomic together (e.g., read-then-write decisions).

---

**Q3: What is the producer-consumer pattern and how does BlockingCollection support it?**

**A:** The producer-consumer pattern decouples work production from processing. Producers add work items to a shared buffer; consumers take from the buffer. The buffer absorbs speed differences. `BlockingCollection<T>` enables this with:
1. **Backpressure**: `boundedCapacity` parameter limits buffer size; `Add()` blocks when full — slowing producers down
2. **Consumer blocking**: `Take()` / `GetConsumingEnumerable()` blocks when buffer is empty — consumers wait for work
3. **Graceful shutdown**: `CompleteAdding()` signals no more items; consumers drain the buffer and stop
This avoids busy-waiting (polling) and unbounded memory growth (unlimited producers).

---

**Q4: Why is Interlocked faster than lock for a simple counter?**

**A:** `lock` uses a **mutex** — when a thread acquires a lock, other threads must be suspended and wake up later (OS kernel call). This involves context switches: saving registers, kernel transition, potentially scheduler involvement — expensive (~microseconds per operation). `Interlocked` uses a **CPU lock prefix** instruction (atomic read-modify-write at the hardware level). No OS involvement, no context switch — just a memory bus lock during one CPU instruction. For a simple counter, `Interlocked.Increment` might be 10-100x faster than `lock`.

---

**Q5: What can go wrong when using ConcurrentDictionary.GetOrAdd with a factory?**

**A:** The `GetOrAdd` method with a factory is **not fully atomic**. The factory may be called multiple times if multiple threads simultaneously call `GetOrAdd` for the same key and the key is absent. The dictionary guarantees that only one value will be stored, but the factory can execute multiple times. This matters if the factory has side effects (creating database connections, writing files, etc.). Fix: use a lazy pattern — store `Lazy<T>` as the value; the `Lazy<T>` ensures computation happens exactly once even if constructed concurrently.

---

**Q6: When would you choose ConcurrentBag over ConcurrentQueue?**

**A:** `ConcurrentBag<T>` is optimized for scenarios where the same thread that adds items also tends to remove them (like an object pool). It uses thread-local storage internally — each thread has its own local stack of items, falling back to global stealing only when the local stack is empty. This minimizes cross-thread coordination. Use ConcurrentBag for work-stealing patterns and object pooling.
Use `ConcurrentQueue<T>` when: order matters (FIFO), or producers and consumers are distinct threads. ConcurrentBag has no ordering guarantee and performs worse in producer-consumer separation.

---

## Scenario-Based Questions

**Scenario 1:** You're building a web server that receives HTTP requests. You want to track request counts per endpoint for monitoring, updated on every request. Thousands of requests arrive per second concurrently. What data structure and update approach is fastest?

**Answer:** Use `ConcurrentDictionary<string, long>` (endpoint → count) with `AddOrUpdate`: `_counts.AddOrUpdate(endpoint, addValue: 1, updateValueFactory: (_, c) => c + 1)`. Alternatively, store `AtomicLong` equivalent using `ConcurrentDictionary<string, StatsCounter>` where `StatsCounter` wraps a `long` updated via `Interlocked.Increment`. For even less contention at very high throughput, use per-thread counters and periodically aggregate them (striped counters pattern). Never use `lock` + `Dictionary` for this — the single lock becomes a bottleneck under high request rates.

---

**Scenario 2:** Design a thread-safe download manager. Multiple "downloader" threads add completed file chunks to a queue; a single "writer" thread takes chunks and writes them to disk in order. The writer should process all chunks and stop cleanly when all downloads finish.

**Answer:** Use `BlockingCollection<FileChunk>` with the default ConcurrentQueue backing. Downloaders call `buffer.Add(chunk)`. The writer runs `foreach (var chunk in buffer.GetConsumingEnumerable())` — this blocks when the queue is empty and processes automatically in FIFO order (ConcurrentQueue maintains insertion order). When all downloaders finish, the last one calls `buffer.CompleteAdding()`. This signals the consuming enumerable to stop after the queue is drained. The writer thread exits cleanly. If writes must maintain global order across chunks from different files, add a sequence number and reorder in the writer.

---

**Scenario 3:** You have a cache backed by ConcurrentDictionary. On a cache miss, you need to load data from the database — this is expensive. Multiple threads may simultaneously request the same missing key. You want the database called exactly once per unique missing key, even under concurrent access.

**Answer:** Store `Lazy<Task<T>>` as the value type: `ConcurrentDictionary<string, Lazy<Task<T>>>`. On miss: `_cache.GetOrAdd(key, k => new Lazy<Task<T>>(() => LoadFromDb(k)))`. Even if multiple threads call GetOrAdd simultaneously for the same key and multiple Lazy instances are created, only one survives (the dictionary rejects duplicates). The `Lazy<T>` ensures the factory runs only once regardless. Access with `.Value` which returns the Task (shared by all threads accessing that key). `Lazy<T>` is thread-safe by default (`LazyThreadSafetyMode.ExecutionAndPublication`), making this pattern robust.

---

## Common Mistakes

1. **Using regular List/Dictionary across threads** — data corruption, exceptions; always use concurrent versions or lock
2. **Locking on `this` or `string`** — `lock(this)` is dangerous (external code can deadlock you); `lock(stringLiteral)` shares lock with all code using the same string. Always lock on a **private readonly object field**
3. **Assuming ConcurrentDictionary multi-call operations are atomic** — `ContainsKey` + `TryAdd` is not atomic; use `GetOrAdd` instead
4. **Forgetting CompleteAdding() in BlockingCollection** — consumer will block forever waiting for items that never come
5. **Calling Add() after CompleteAdding()** — throws `InvalidOperationException`; use `TryAdd()` which returns false instead
6. **Deadlock with nested locks** — always acquire locks in the same order across all code paths; avoid holding lock A while waiting for lock B if any code holds B and waits for A
7. **Using volatile as a substitute for Interlocked** — `volatile` ensures visibility but not atomicity; `count++` is still not atomic even on a volatile field
