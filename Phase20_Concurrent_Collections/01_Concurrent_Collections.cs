// ============================================================
// PHASE 20: CONCURRENT COLLECTIONS
// Topics: Thread safety, ConcurrentDictionary, ConcurrentQueue,
//         ConcurrentStack, ConcurrentBag, BlockingCollection,
//         lock vs concurrent collections
// Run this file in a .NET Console Application project
// ============================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ============================================================
// SECTION 1: WHY CONCURRENT COLLECTIONS?
//
// Standard collections (List<T>, Dictionary<T,V>, Queue<T>) are
// NOT thread-safe. If multiple threads modify them simultaneously:
//   - Data corruption (lost updates, torn reads)
//   - IndexOutOfRangeException
//   - Infinite loops (dictionary resizing)
//
// Solutions:
//   1. lock() — serialize access (simple but blocking)
//   2. System.Collections.Concurrent — lock-free or fine-grained locking
//
// Concurrent collections in .NET:
//   ConcurrentDictionary<K,V>  — thread-safe dictionary
//   ConcurrentQueue<T>         — FIFO, thread-safe
//   ConcurrentStack<T>         — LIFO, thread-safe
//   ConcurrentBag<T>           — unordered, thread-safe
//   BlockingCollection<T>      — producer-consumer with blocking
// ============================================================

// ============================================================
// SECTION 2: PROBLEM WITH REGULAR COLLECTIONS IN MULTITHREADING
// ============================================================

class ThreadSafetyProblemDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Thread Safety Problem ===");

        // UNSAFE: multiple threads incrementing without synchronization
        int counter = 0;
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                    counter++;  // NOT atomic! Read-modify-write race condition
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"  Unsafe counter (expected 5000, likely != 5000): {counter}");

        // SAFE: using Interlocked for atomic increment
        int safeCounter = 0;
        var safeTasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            safeTasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                    Interlocked.Increment(ref safeCounter);  // atomic!
            }));
        }

        Task.WaitAll(safeTasks.ToArray());
        Console.WriteLine($"  Safe counter (exactly 5000): {safeCounter}");
    }
}

// ============================================================
// SECTION 3: ConcurrentDictionary<K,V>
//
// Thread-safe dictionary. Uses fine-grained locking internally
// (one lock per bucket segment, not one global lock).
//
// Key methods:
//   TryAdd(key, value)                   — add if key doesn't exist
//   TryGetValue(key, out value)          — safe get
//   TryUpdate(key, newValue, expected)   — update only if current value matches expected
//   TryRemove(key, out value)            — remove and return value
//   GetOrAdd(key, factory)               — get or atomically add
//   AddOrUpdate(key, addFactory, updateFactory) — add or update atomically
// ============================================================

class ConcurrentDictionaryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ConcurrentDictionary<K,V> ===");

        var wordCount = new ConcurrentDictionary<string, int>();
        string[] words = { "apple", "banana", "apple", "cherry", "banana", "apple" };

        // Count word frequencies concurrently
        Parallel.ForEach(words, word =>
        {
            // AddOrUpdate: if key absent → add with value 1
            //              if key exists → increment
            wordCount.AddOrUpdate(
                key: word,
                addValue: 1,
                updateValueFactory: (key, existingCount) => existingCount + 1
            );
        });

        Console.WriteLine("  Word frequencies:");
        foreach (var kv in wordCount.OrderBy(x => x.Key))
            Console.WriteLine($"    {kv.Key}: {kv.Value}");

        // GetOrAdd: returns existing value or adds new lazily
        var cache = new ConcurrentDictionary<int, string>();
        string result = cache.GetOrAdd(1, id => $"ExpensiveComputation_{id}");
        Console.WriteLine($"\n  GetOrAdd result: {result}");
        string sameResult = cache.GetOrAdd(1, id => "This won't be called");  // cache hit
        Console.WriteLine($"  Second GetOrAdd (cache hit): {sameResult}");

        // TryUpdate: atomic compare-and-swap
        wordCount.TryAdd("mango", 5);
        bool updated = wordCount.TryUpdate("mango", newValue: 10, comparisonValue: 5);
        Console.WriteLine($"\n  TryUpdate mango 5→10: {updated}, new value: {wordCount["mango"]}");
    }
}

// ============================================================
// SECTION 4: ConcurrentQueue<T> — Thread-safe FIFO
//
// TryDequeue(out T) — returns false if empty (never blocks)
// TryPeek(out T)    — peek without removing
// Enqueue(T)        — always succeeds
// IsEmpty, Count
// ============================================================

class ConcurrentQueueDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ConcurrentQueue<T> ===");

        var queue = new ConcurrentQueue<int>();

        // Producers: enqueue from multiple threads
        var producers = Enumerable.Range(1, 3).Select(id =>
            Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    queue.Enqueue(id * 100 + i);
                    Console.WriteLine($"    Producer {id}: enqueued {id * 100 + i}");
                    Thread.Sleep(10);
                }
            })
        ).ToArray();

        Task.WaitAll(producers);

        Console.WriteLine("  Consuming all items:");
        while (queue.TryDequeue(out int item))
            Console.Write($"    Consumed: {item}\n");
    }
}

// ============================================================
// SECTION 5: BlockingCollection<T> — Producer-Consumer Pattern
//
// Wraps any IProducerConsumerCollection<T> (default = ConcurrentQueue).
// KEY FEATURE: Can BLOCK producers (when full) and consumers (when empty).
//
//   Add(item)        — blocks if at capacity
//   Take()           — blocks if empty
//   CompleteAdding()  — signals no more items will be added
//   GetConsumingEnumerable() — yields items until completed and empty
// ============================================================

class BlockingCollectionDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== BlockingCollection<T> (Producer-Consumer) ===");

        // Create bounded buffer (max 3 items at a time)
        var buffer = new BlockingCollection<int>(boundedCapacity: 3);

        // PRODUCER: adds 6 items (will block temporarily when buffer is full)
        var producer = Task.Run(() =>
        {
            for (int i = 1; i <= 6; i++)
            {
                buffer.Add(i);  // blocks if buffer is full (capacity reached)
                Console.WriteLine($"    Producer: added {i} (buffer count: {buffer.Count})");
                Thread.Sleep(50);
            }
            buffer.CompleteAdding();  // IMPORTANT: signal that no more items will come
            Console.WriteLine("    Producer: done adding.");
        });

        // CONSUMER: processes items as they arrive
        var consumer = Task.Run(() =>
        {
            // GetConsumingEnumerable: blocks on empty, exits when CompleteAdding + empty
            foreach (int item in buffer.GetConsumingEnumerable())
            {
                Console.WriteLine($"    Consumer: processing {item}");
                Thread.Sleep(150);  // consumer is slower than producer
            }
            Console.WriteLine("    Consumer: done.");
        });

        Task.WaitAll(producer, consumer);
    }
}

// ============================================================
// SECTION 6: LOCK VS CONCURRENT COLLECTIONS
// ============================================================

class LockVsConcurrentDemo
{
    // Approach 1: Manual locking — SIMPLE but CONTENTION-PRONE
    // Every operation acquires the SAME lock → only 1 thread at a time
    static readonly object _lock = new object();
    static Dictionary<string, int> _lockedDict = new Dictionary<string, int>();

    public static void AddLocked(string key, int value)
    {
        lock (_lock)  // blocks ALL other threads until released
        {
            _lockedDict[key] = value;
        }
    }

    // Approach 2: ConcurrentDictionary — FINE-GRAINED locking
    // Multiple threads can operate on different buckets simultaneously
    static ConcurrentDictionary<string, int> _concurrentDict = new ConcurrentDictionary<string, int>();

    public static void AddConcurrent(string key, int value)
    {
        _concurrentDict[key] = value;  // no explicit lock needed
    }

    public static void Run()
    {
        Console.WriteLine("\n=== When to use lock vs ConcurrentDictionary ===");
        Console.WriteLine("  lock(): Simple, works for any code section, but all threads wait");
        Console.WriteLine("  ConcurrentDictionary: Fine-grained, better throughput under high contention");
        Console.WriteLine("  Rule: For simple atomic operations → ConcurrentDictionary");
        Console.WriteLine("        For multi-step operations needing atomicity → lock()");
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class ConcurrentCollectionsProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 20: CONCURRENT COLLECTIONS");
        Console.WriteLine("======================================================");

        ThreadSafetyProblemDemo.Run();
        ConcurrentDictionaryDemo.Run();
        ConcurrentQueueDemo.Run();
        BlockingCollectionDemo.Run();
        LockVsConcurrentDemo.Run();

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Standard collections are NOT thread-safe — data corruption in multi-threaded use");
        Console.WriteLine("  2. ConcurrentDictionary: fine-grained locking, GetOrAdd/AddOrUpdate for atomic ops");
        Console.WriteLine("  3. ConcurrentQueue: lock-free FIFO, TryDequeue returns false if empty");
        Console.WriteLine("  4. BlockingCollection: producer-consumer, blocks producers/consumers as needed");
        Console.WriteLine("  5. Interlocked: atomic operations on primitives (increment, compare-exchange)");
        Console.WriteLine("  6. lock() is deadlock-prone; prefer ConcurrentXxx for simple operations");
        Console.WriteLine("  7. Always call CompleteAdding() in BlockingCollection to avoid consumer deadlock");
        Console.WriteLine("======================================================");
    }
}
