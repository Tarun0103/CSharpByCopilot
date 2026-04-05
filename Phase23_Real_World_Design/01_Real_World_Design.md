# Phase 23: Real-World Design with Data Structures

## Overview

Knowing *what* a data structure does is necessary but not sufficient. Senior engineers know *which* data structure to choose and *why*, under real-world constraints: latency targets, memory limits, concurrent access, and scale.

---

## Data Structure Decision Framework

```
What is the primary operation?
│
├─ LOOKUP (find by key)
│   ├─ O(1) needed? → HashMap / Dictionary<K,V>
│   ├─ Sorted keys needed? → SortedDictionary / SortedList
│   └─ Prefix search? → Trie
│
├─ MEMBERSHIP (is X in the set?)
│   ├─ Exact, O(1)? → HashSet<T>
│   ├─ Sorted membership + range? → SortedSet<T>
│   └─ Approximate (memory-efficient)? → Bloom Filter
│
├─ ORDERING (process by priority)
│   ├─ FIFO (strict order)? → Queue<T>
│   ├─ LIFO (reverse order)? → Stack<T>
│   └─ Priority-based? → PriorityQueue<T, P>
│
├─ RECENT ACCESS (LRU, eviction)
│   └─ O(1) get/put with eviction? → Dictionary + LinkedList (LRU Cache)
│
├─ RANGE QUERY (find all X where min ≤ X ≤ max)
│   ├─ In-memory? → SortedSet.GetViewBetween()
│   └─ Temporal? → Sliding window (Queue of timestamps)
│
└─ GRAPH PROBLEMS
    ├─ Shortest path (weighted)? → Dijkstra / Bellman-Ford + PriorityQueue
    ├─ Connectivity? → Union-Find
    └─ Reachability? → BFS (unweighted) / DFS
```

---

## LRU Cache Design

**Problem:** Cache with capacity K. On access: return cached value (update recency). On miss: fetch+cache (evict least recently used if full).

**Requirements:** O(1) get, O(1) put, O(1) evict.

**Solution:** `Dictionary<K, LinkedListNode<(K,V)>>` + `LinkedList<(K,V)>`

```
Dictionary (for O(1) key lookup):
"A" → Node("A", 1)
"B" → Node("B", 2)
"C" → Node("C", 3)

LinkedList (ordered by recency, MRU at front):
[C] ↔ [B] ↔ [A]
MRU               LRU
```

- **Get(key):** Find node via dictionary O(1) → move node to front of linked list O(1) → return value
- **Put(key, value):** If exists: update + move to front. If new: add node at front. If over capacity: remove tail node + remove from dictionary.
- **Why Dictionary + LinkedList?** Dictionary gives O(1) key→node lookup. LinkedList gives O(1) node insertion/deletion given the node reference. Together: O(1) for all operations.

```csharp
// Core data structure
private readonly Dictionary<int, LinkedListNode<(int Key, int Value)>> _map = new();
private readonly LinkedList<(int Key, int Value)> _list = new();
private readonly int _capacity;

// Get: find + move to front
public int Get(int key)
{
    if (!_map.ContainsKey(key)) return -1;
    var node = _map[key];
    _list.Remove(node);        // O(1) — LinkedList.Remove(node) given the node
    _list.AddFirst(node);      // O(1)
    return node.Value.Value;
}
```

---

## Rate Limiter Design

### Fixed Window Rate Limiter
Count requests per time window. Simpler but has edge-case burst.
```
Window: [0s, 60s) → max 100 requests
Window: [60s, 120s) → max 100 requests again
Problem: 100 at t=59s + 100 at t=61s = 200 in 2 seconds!
```

### Sliding Window Rate Limiter (better)
Track timestamps of recent requests in a queue.
```csharp
private readonly Queue<DateTime> _requests = new();
private readonly int _maxRequestsPerWindow;
private readonly TimeSpan _windowSize;

public bool IsAllowed(DateTime now)
{
    // Remove requests outside the current window
    while (_requests.Count > 0 && now - _requests.Peek() > _windowSize)
        _requests.Dequeue();

    if (_requests.Count >= _maxRequestsPerWindow)
        return false;   // Rate limited

    _requests.Enqueue(now);
    return true;
}
```
**Data structure:** `Queue<DateTime>` (FIFO). Old timestamps expire from the front; new ones go to the back.

---

## Top-K Leaderboard

**Problem:** Maintain a live leaderboard of top K scores among millions of players. Fast updates required.

**Approach:** Keep a min-heap of size K.
- Each update: insert score. If heap size > K → extract min.
- After all updates: heap contains exactly top K scores.
- Min of heap = Kth highest score.

```csharp
// PriorityQueue is a min-heap in C#
var topK = new PriorityQueue<(string Name, int Score), int>(capacity: k + 1);

foreach (var player in players)
{
    topK.Enqueue((player.Name, player.Score), player.Score);
    if (topK.Count > k)
        topK.Dequeue(); // Remove lowest (min-heap removes minimum first)
}
// Remaining items = top K scores
```

For a live leaderboard with updates:
- `Dictionary<playerId, score>` for O(1) lookups
- `SortedDictionary<score, List<playerId>>` for ranked access

---

## Autocomplete System with Trie

**Problem:** Given a prefix, return K most frequent completions.

**Trie for Autocomplete:**
```
Words: "cat"(5), "car"(3), "card"(2), "care"(4), "bar"(1)

Trie:
     root
    /    \
   c      b
   |      |
   a      a
  / \     |
 t   r    r
(5) (3)  (1)
    / \
   d   e
  (2) (4)
```

- Each TrieNode stores: children, frequency (how often this word ends here), max frequency in subtree (for pruning)
- Search prefix in O(L) where L = prefix length
- Collect all words in subtree using DFS
- Use max-heap of size K to get top K results

**Key optimization:** Store `maxFreqInSubtree` at each node → prune branches that can't beat current K-th best.

---

## Event Queue / Priority Scheduler

**Problem:** Process events in order of priority. Same-priority events: process by arrival order.

```csharp
// Event with stable ordering for ties
record ScheduledEvent(
    int Priority,        // Lower = more urgent
    long SequenceNumber, // Arrival order for tie-breaking
    Action Task
);

var queue = new PriorityQueue<ScheduledEvent, (int, long)>();
long seq = 0;

void Enqueue(int priority, Action task)
{
    var evt = new ScheduledEvent(priority, seq++, task);
    queue.Enqueue(evt, (priority, evt.SequenceNumber));
    // Tuple comparison: first by priority, then by sequence (FIFO for ties)
}
```

This ensures FIFO ordering within the same priority level — critical for fairness.

---

## System Design Trade-off Cheatsheet

| Scenario | Data Structure | Complexity | Tradeoff |
|----------|--------------|------------|---------|
| Cache with eviction | Dict + LinkedList | O(1) get/put | Memory = capacity × entry size |
| Rate limiting | Queue of timestamps | O(1) amortized | Memory grows with request rate |
| Leaderboard top-K | Min-heap size K | O(n log K) | Not fully ordered beyond K |
| Word autocomplete | Trie | O(L) search | O(W × L) build, higher memory |
| Nearest K tasks by deadline | PriorityQueue | O(log n) | Not random-access |
| Deduplication | HashSet | O(1) avg | O(n) memory |
| Approximate deduplication | Bloom Filter | O(1) | False positives possible |
| Range queries | SortedSet/BTree | O(log n) | Higher overhead than HashMap |
| Graph shortest path | Dijkstra + PQ | O(E log V) | Only for non-negative weights |

---

## Interview Questions & Answers

**Q1: Design an LRU cache that supports O(1) get and O(1) put. What data structures do you use and why?**

**A:** Combine a `Dictionary<K, LinkedListNode<(K,V)>>` and a `LinkedList<(K,V)>`. The dictionary maps each key to its linked list node for O(1) key lookup. The linked list maintains recency order (MRU at front, LRU at tail) and since we have direct node references from the dictionary, list operations (remove + reinsert at front) are O(1). On get: find node via dict → move to front. On put: if key exists, update + move to front; if new, add at front; if over capacity, evict tail + remove from dict. The critical design decision: dictionary stores the LinkedListNode reference directly, enabling O(1) arbitrary removal from the linked list.

---

**Q2: How would you design a rate limiter that handles concurrent requests?**

**A:** For a single-instance service: use the sliding window approach with a `ConcurrentQueue<DateTime>` of request timestamps + Interlocked/lock for thread safety. On each request:
1. Dequeue all timestamps older than the window (from front)
2. If count ≥ limit: reject
3. Else: enqueue current timestamp → allow

For distributed rate limiting (multiple servers): use Redis with a sorted set (zset) where score = timestamp, or Redis counter with TTL (token bucket/fixed window). The key challenge: atomic check-and-increment — Redis Lua scripts or INCR+EXPIRE provide atomicity without distributed locking overhead.

---

**Q3: How would you build an autocomplete feature that returns the top 5 most popular completions?**

**A:** 
1. **Build a Trie** from the word dictionary where each terminal node stores word frequency
2. Optionally, store `maxFreq` in each node (max frequency in its subtree) for pruning
3. For a query prefix: traverse the trie to the prefix node O(L)
4. DFS/BFS from that node collecting all words in subtree with a **max-heap of size 5** — keep only top 5 by frequency
5. Return heap contents

Optimization: for very frequent queries on the same prefix, cache the top 5 results at each node during preprocessing. Also consider: DAWG (Directed Acyclic Word Graph) reduces memory by sharing suffixes.

---

**Q4: You have 1 million users. For each user you need to quickly check if they are in a "premium" set. Memory is limited. What data structure?**

**A:** For exact membership: `HashSet<userId>` (or long/int if IDs are numeric). O(1) lookup, O(n) memory ≈ 4 bytes × 1M = ~4 MB — very fast and small.

If memory is extremely tight and occasional false positives are acceptable: **Bloom Filter**. A 1M-element bloom filter with 1% false positive rate needs only ~9.6 bits per element = ~1.2 MB vs 4 MB for HashSet. The tradeoff: bloom filters can say "definitely not in set" or "probably in set" — the "probably" means 1% incorrect "yes" answers. This could mean 1% of free users are incorrectly given premium access, which may be acceptable for some use cases.

---

**Q5: How do you implement a task scheduler that processes high-priority tasks first but ensures low-priority tasks aren't starved forever?**

**A:** Use **aging** with a priority queue. The naive approach (always process highest priority) can starve low-priority tasks if high-priority tasks keep arriving. Aging solution: maintain a `PriorityQueue<Task, int>`. A background process periodically scans low-priority tasks waiting > threshold and promotes them (reduce their priority value, re-enqueue). Alternatively, use a **multilevel feedback queue** (like Linux's completely fair scheduler): tasks that have been waiting long are promoted to higher-priority queues. The simplest correct approach: effective_priority = base_priority - (wait_time_ms / aging_factor); re-evaluate priority periodically.

---

**Q6: In a graph of cities and roads, how would you find the fastest route between two cities?**

**A:** Use **Dijkstra's algorithm** with a `PriorityQueue<int, int>` (node, accumulated_distance):
1. Start: enqueue (source, 0). Initialize `dist[source] = 0`, all others = infinity.
2. Each iteration: dequeue minimum-distance unvisited node.
3. For each neighbor: if `dist[current] + edge_weight < dist[neighbor]`, update and enqueue.
4. Stop when destination is dequeued.
Complexity: O((V + E) log V) with a binary heap.

Limitations: Dijkstra requires **non-negative edge weights** (no negative roads). For negative weights: use Bellman-Ford O(VE). For heuristic shortcuts (knowing city coordinates): use A* with h(n) = Euclidean distance to destination — typically faster in practice than Dijkstra.

---

## Scenario-Based Questions

**Scenario 1:** You're designing a stock ticker system. The system receives millions of price updates per second. Users need: O(1) get current price for any stock, find all stocks in a price range [low, high] within 100ms, and receive notifications when prices cross their alert thresholds. Design the data model.

**Answer:**
- **Current prices:** `Dictionary<string, decimal>` (ticker → price) for O(1) get/update
- **Range queries:** `SortedDictionary<decimal, HashSet<string>>` (price → stocks at that price) maintained in sync with the main dictionary; range query = iterate `GetViewBetween(low, high).Values` which is O(log n + k) where k = results
- **Alert system:** `SortedList<decimal, List<AlertCallback>>` (threshold → callbacks). On price update, binary search for any alerts crossed. Use `ObservableCollection` pattern or event-based system for real-time push to alert subscribers.
- **Concurrency:** Use `ConcurrentDictionary` for main prices; use reader-writer lock for the sorted structures (many readers, occasional bulk write)

---

**Scenario 2:** You're building a cached search engine for an e-commerce site. When a user types "shoe", results are cached. The cache has a fixed memory budget and should evict the least recently queried items. 30% of queries are for the same top 100 searches. Design the cache.

**Answer:** Use an **LRU cache** with capacity proportional to memory budget. Key = search query string, value = `List<Product>` results. Structure: `Dictionary<string, LinkedListNode<(string query, List<Product> results)>>` + `LinkedList`. The 30% popular queries will almost never be evicted since they're accessed frequently (LRU keeps them near front). For the search results, compute result size to enforce memory budget more accurately: instead of a fixed item count limit, track total memory and evict the LRU item when adding would exceed the budget. Consider adding expiry time per entry (TTL) so stale results are also evicted — combine LRU with TTL by storing the `insertTime` alongside each entry.

---

**Scenario 3:** Design a leaderboard for a live tournament. 10,000 players. Clients need: get top 10 players (name + score) in real-time, get rank of any specific player, update a player's score after each game round. All operations should be fast.

**Answer:**

Data structures:
1. `Dictionary<int, int> playerToScore` — O(1) score lookup/update per player
2. `SortedDictionary<int, HashSet<int>> scoreToPlayers` — O(log n) ordered access

**Get Top 10:** Iterate `scoreToPlayers` in descending order (via Reverse()), collect from each score bucket until you have 10 players. O(log n + 10) with the SortedDictionary.

**Get Player Rank:** Count how many players have higher score — `scoreToPlayers.Where(kvp => kvp.Key > playerScore).Sum(kvp => kvp.Value.Count) + 1`. Or pre-compute cumulative counts if needed.

**Update Score:** Remove player from old score bucket, add to new bucket, update dictionary. O(log n) per update.

For very high update frequency (millions/sec), batch updates and recompute periodically rather than maintaining the sorted structure synchronously.

---

## Common Mistakes in Real-World Design

1. **Choosing data structures without considering actual operations** — list what operations matter FIRST, then choose
2. **Ignoring concurrency** — works in dev (single-threaded), fails in prod (many requests simultaneously)
3. **Not accounting for memory** — an O(1) lookup with O(n) memory may cause GC pauses worse than a slightly slower O(log n) with less memory
4. **Over-engineering** — using a trie when a sorted list + binary search is sufficient for 1,000 words
5. **Choosing by familiarity** — `List<T>` is not always the answer; know when `HashSet`, `SortedSet`, or `Queue` is the right tool
6. **Forgetting about eviction/cleanup** — caches that grow unbounded eventually OOM; always design eviction
7. **Ignoring cache coherence in distributed systems** — in multi-server setups an in-memory cache may serve stale data; design TTLs and invalidation strategies
