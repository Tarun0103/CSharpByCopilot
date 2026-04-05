// ============================================================
// PHASE 23: REAL-WORLD DESIGN — Choosing the Right Data Structure
// Topics: Decision framework, caching, rate limiting,
//         LRU Cache, system design intro for interviews,
//         data structure trade-off comparison
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

// ============================================================
// SECTION 1: DATA STRUCTURE DECISION FRAMEWORK
//
// Ask these questions to choose the right data structure:
//
//  1. Need KEY-VALUE lookup?          → Dictionary<K,V>
//  2. Need SORTED keys?               → SortedDictionary<K,V>
//  3. Need UNIQUE elements only?      → HashSet<T>
//  4. Need FAST add/remove from ends? → LinkedList<T>
//  5. Need LIFO (last-in, first-out)? → Stack<T>
//  6. Need FIFO (first-in, first-out)?→ Queue<T>
//  7. Need PRIORITY queue?            → PriorityQueue<T,P>
//  8. Need both fast lookup + order?  → Dictionary + LinkedList (LRU)
//  9. Need PREFIX matching?           → Trie
//  10. Need RANGE queries?            → Segment Tree / Sorted collections
//  11. Need GRAPH traversal?          → Adjacency List (Dictionary<int,List<int>>)
//  12. Multi-threaded access?         → ConcurrentDictionary / ConcurrentQueue
// ============================================================

// ============================================================
// SECTION 2: LRU CACHE (Least Recently Used)
//
// REAL INTERVIEW PROBLEM: Design a data structure that:
//   - get(key): returns value if exists, -1 otherwise. O(1)
//   - put(key, value): insert/update. Evict least recently used if full. O(1)
//
// SOLUTION: Dictionary<key, LinkedListNode> + LinkedList
//   - Dictionary: O(1) lookup by key
//   - LinkedList: O(1) move to front (most recently used = front)
//                 O(1) remove from back (least recently used = back)
//
// This is used in: browser cache, CPU cache, database page cache,
//                  CDN caching, memoization systems
// ============================================================

class LRUCache
{
    private int _capacity;
    // Dictionary: key → LinkedList node  (for O(1) lookup + O(1) move)
    private Dictionary<int, LinkedListNode<(int key, int value)>> _map;
    // LinkedList: most recent = head, least recent = tail
    private LinkedList<(int key, int value)> _list;

    public LRUCache(int capacity)
    {
        _capacity = capacity;
        _map  = new Dictionary<int, LinkedListNode<(int, int)>>(capacity);
        _list = new LinkedList<(int, int)>();
    }

    // GET: O(1) — lookup + move to front
    public int Get(int key)
    {
        if (!_map.TryGetValue(key, out var node)) return -1;

        // Move to front (most recently used)
        _list.Remove(node);
        _list.AddFirst(node);
        return node.Value.value;
    }

    // PUT: O(1) — update or insert
    public void Put(int key, int value)
    {
        if (_map.TryGetValue(key, out var existing))
        {
            // Key exists: update value and move to front
            _list.Remove(existing);
            var updated = _list.AddFirst((key, value));
            _map[key] = updated;
        }
        else
        {
            // New key: check capacity
            if (_map.Count >= _capacity)
            {
                // Evict least recently used (tail of linked list)
                var lruNode = _list.Last!;
                _list.RemoveLast();
                _map.Remove(lruNode.Value.key);
            }

            // Add new item at front
            var newNode = _list.AddFirst((key, value));
            _map[key] = newNode;
        }
    }

    public void PrintState()
    {
        Console.Write("    [");
        foreach (var (k, v) in _list) Console.Write($"({k}:{v}) ");
        Console.WriteLine("] ← LRU at tail");
    }
}

// ============================================================
// SECTION 3: RATE LIMITER (Token Bucket / Sliding Window)
//
// PROBLEM: Limit API calls to N requests per time window.
// Used in: API gateways, DDoS protection, throttling services.
//
// SLIDING WINDOW with Queue: track timestamps of recent requests.
// Allow if fewer than N requests in the last T seconds.
// ============================================================

class SlidingWindowRateLimiter
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    // Queue of request timestamps; oldest at front
    private readonly Queue<DateTime> _timestamps;
    private readonly object _lock = new object();

    public SlidingWindowRateLimiter(int maxRequests, TimeSpan window)
    {
        _maxRequests = maxRequests;
        _window = window;
        _timestamps = new Queue<DateTime>();
    }

    // Returns true if request is allowed
    public bool TryRequest(string clientId)
    {
        lock (_lock)
        {
            DateTime now = DateTime.UtcNow;
            DateTime windowStart = now - _window;

            // Remove timestamps outside the window (too old)
            while (_timestamps.Count > 0 && _timestamps.Peek() < windowStart)
                _timestamps.Dequeue();

            if (_timestamps.Count < _maxRequests)
            {
                _timestamps.Enqueue(now);  // record this request
                return true;               // allowed
            }

            return false;  // rate limited!
        }
    }
}

// ============================================================
// SECTION 4: TASK SCHEDULER (Priority Queue real-world use)
//
// PROBLEM: Tasks arrive with priorities. Execute highest priority first.
// SOLUTION: PriorityQueue<T,P> (min-heap by default → invert priority for max-heap)
// ============================================================

class PriorityTaskScheduler
{
    record Task(int Id, string Name, int Priority);

    // Priority queue: (Task, priority) — C# PQ is a min-heap
    // Use negative priority for max-heap behavior
    private PriorityQueue<Task, int> _queue = new PriorityQueue<Task, int>();
    private int _nextId = 1;

    public void SubmitTask(string name, int priority)
    {
        var task = new Task(_nextId++, name, priority);
        _queue.Enqueue(task, -priority);  // negate for max-heap (higher priority = smaller value)
        Console.WriteLine($"  Submitted: {name} (priority={priority})");
    }

    public void ProcessAll()
    {
        Console.WriteLine("  Processing tasks by priority:");
        while (_queue.Count > 0)
        {
            var task = _queue.Dequeue();
            Console.WriteLine($"    Executing: [{task.Priority}] {task.Name}");
        }
    }
}

// ============================================================
// SECTION 5: AUTOCOMPLETE SYSTEM (Trie)
//
// PROBLEM: Given a list of words, implement autocomplete.
// Type a prefix → get top N suggestions.
// SOLUTION: Trie (prefix tree) — O(prefix length) search
// ============================================================

class AutocompleteSystem
{
    class TrieNode
    {
        public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
        public bool IsWord;
        public int Frequency;  // for ranking suggestions
    }

    private TrieNode _root = new TrieNode();

    public void Insert(string word, int frequency = 1)
    {
        var node = _root;
        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
                node.Children[c] = new TrieNode();
            node = node.Children[c];
        }
        node.IsWord = true;
        node.Frequency += frequency;  // accumulate frequency
    }

    // Get top N suggestions for a prefix
    public List<string> GetSuggestions(string prefix, int topN = 5)
    {
        var node = _root;
        foreach (char c in prefix)
        {
            if (!node.Children.ContainsKey(c)) return new List<string>();
            node = node.Children[c];
        }

        // DFS from current node to find all completions
        var results = new List<(string word, int freq)>();
        DFS(node, prefix, results);

        // Return top N by frequency
        return results.OrderByDescending(x => x.freq)
                       .Take(topN)
                       .Select(x => x.word)
                       .ToList();
    }

    private void DFS(TrieNode node, string current, List<(string, int)> results)
    {
        if (node.IsWord) results.Add((current, node.Frequency));
        foreach (var (c, child) in node.Children)
            DFS(child, current + c, results);
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class RealWorldDesignProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 23: REAL-WORLD DESIGN");
        Console.WriteLine("======================================================");

        // --- LRU Cache ---
        Console.WriteLine("\n=== LRU Cache Demo (capacity=3) ===");
        var cache = new LRUCache(3);
        cache.Put(1, 10); cache.PrintState();
        cache.Put(2, 20); cache.PrintState();
        cache.Put(3, 30); cache.PrintState();
        Console.WriteLine($"  Get(1) = {cache.Get(1)} (moves 1 to front)");
        cache.PrintState();
        cache.Put(4, 40);  // evicts LRU (key 2)
        Console.WriteLine("  After Put(4,40) — key 2 evicted:");
        cache.PrintState();
        Console.WriteLine($"  Get(2) = {cache.Get(2)} (evicted)");

        // --- Rate Limiter ---
        Console.WriteLine("\n=== Sliding Window Rate Limiter (3 req/sec) ===");
        var limiter = new SlidingWindowRateLimiter(3, TimeSpan.FromSeconds(1));
        for (int i = 1; i <= 5; i++)
        {
            bool allowed = limiter.TryRequest("user1");
            Console.WriteLine($"  Request {i}: {(allowed ? "ALLOWED" : "RATE LIMITED")}");
        }

        // --- Priority Task Scheduler ---
        Console.WriteLine("\n=== Priority Task Scheduler ===");
        var scheduler = new PriorityTaskScheduler();
        scheduler.SubmitTask("Send Email",     priority: 1);
        scheduler.SubmitTask("Fix Critical Bug", priority: 5);
        scheduler.SubmitTask("Code Review",    priority: 3);
        scheduler.SubmitTask("Deploy Now",     priority: 5);
        scheduler.SubmitTask("Update Docs",    priority: 1);
        scheduler.ProcessAll();

        // --- Autocomplete ---
        Console.WriteLine("\n=== Autocomplete System ===");
        var ac = new AutocompleteSystem();
        string[] words = { "apple", "app", "application", "apply", "apt", "banana", "band", "bandana" };
        foreach (string w in words) ac.Insert(w, frequency: new Random().Next(1, 100));

        string prefix = "app";
        Console.WriteLine($"  Prefix '{prefix}' → {string.Join(", ", ac.GetSuggestions(prefix))}");
        prefix = "ban";
        Console.WriteLine($"  Prefix '{prefix}' → {string.Join(", ", ac.GetSuggestions(prefix))}");

        Console.WriteLine("\n=== Data Structure Selection Summary ===");
        Console.WriteLine("  Need:                         → Use:");
        Console.WriteLine("  Fast lookup by key            → Dictionary<K,V>       O(1)");
        Console.WriteLine("  Sorted key-value              → SortedDictionary<K,V> O(log n)");
        Console.WriteLine("  Unique items                  → HashSet<T>            O(1)");
        Console.WriteLine("  Priority ordering             → PriorityQueue<T,P>    O(log n)");
        Console.WriteLine("  Prefix matching               → Trie                  O(key length)");
        Console.WriteLine("  LRU eviction                  → Dict + LinkedList     O(1)");
        Console.WriteLine("  Sliding window                → Queue                 O(1)");
        Console.WriteLine("  Thread-safe lookup            → ConcurrentDictionary  O(1)");
        Console.WriteLine("  Producer-consumer             → BlockingCollection    O(1)");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. LRU Cache = Dictionary (O(1) lookup) + LinkedList (O(1) reorder)");
        Console.WriteLine("  2. Rate Limiter = Queue to track timestamps in sliding window");
        Console.WriteLine("  3. Task Scheduler = PriorityQueue with inverted priority for max-heap");
        Console.WriteLine("  4. Autocomplete = Trie with DFS traversal from matched prefix node");
        Console.WriteLine("  5. Always ask: What operations are most frequent? Optimize for those.");
        Console.WriteLine("  6. 'Best' structure depends on READ vs WRITE vs MEMORY trade-offs");
        Console.WriteLine("======================================================");
    }
}
