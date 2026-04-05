# Phase 10: Queue (FIFO) — Complete Guide

## What is a Queue?

A **Queue** is a linear data structure that follows the **FIFO** principle:
> **First In, First Out** — the first element added is the first one removed.

Real-world analogy: A **checkout line at a supermarket**. People join at the back and are served from the front. The first person to arrive is the first to be served.

```
ENQUEUE (add at rear)                    DEQUEUE (remove from front)
                     ↓                       ↑
Front →  [ 1 | 2 | 3 | 4 | 5 ]  ← Rear
         ^served first           ^added last
```

---

## Core Operations

| Operation | Description | Time Complexity |
|---|---|---|
| `Enqueue(x)` | Add element to rear | O(1) |
| `Dequeue()` | Remove and return front element | O(1) |
| `Peek()` | Read front element without removing | O(1) |
| `Count` | Number of elements | O(1) |
| `Contains(x)` | Check if element exists | O(n) |

---

## C# Queue<T> API

```csharp
Queue<string> queue = new Queue<string>();

// Add to the back
queue.Enqueue("Alice");
queue.Enqueue("Bob");
queue.Enqueue("Charlie");

// Peek — see front without removing
string first = queue.Peek();       // "Alice" (not removed)

// Dequeue — remove and return front
string served = queue.Dequeue();   // "Alice" (removed)
// Queue is now: Bob, Charlie

// Safe versions (don't throw on empty)
if (queue.TryDequeue(out string element)) { /* success */ }
if (queue.TryPeek(out string front))      { /* success */ }

// Properties
int count = queue.Count;           // 2
bool empty = queue.Count == 0;     // false

// Contains — O(n) linear scan
bool hasBob = queue.Contains("Bob");  // true

// Convert to array (front to back order)
string[] arr = queue.ToArray();     // ["Bob", "Charlie"]

// Create from collection
Queue<int> fromArr = new Queue<int>(new[] { 1, 2, 3 });
fromArr.Dequeue();  // returns 1 (first element = front)

// Clear
queue.Clear();
```

---

## Circular Queue Concept

A naive array-based queue wastes memory as the front pointer marches right:

```
After many enqueue/dequeue operations:
[ _ | _ | _ | 5 | 6 | 7 ]   ← spaces at front are WASTED
                ^front   ^rear

Solution: Use MODULO to wrap the rear pointer around:
next_position = (current_position + 1) % capacity

[ 8 | 9 | _ | 5 | 6 | 7 ]   ← wraps around! New items fill "empty" slots
  ^rear(8,9)    ^front
```

This is why it's called a **circular queue** — it treats the array as if the ends are connected in a circle.

---

## Queue vs Stack vs LinkedList

| Feature | Queue | Stack | LinkedList |
|---|---|---|---|
| Order | FIFO | LIFO | Insertion order |
| Add | To rear | To top | At any position |
| Remove | From front | From top | At any position |
| Use case | Scheduling, BFS | Undo, DFS, parsing | General-purpose |
| Access | Front only | Top only | All positions |

---

## Priority Queue (C# 6.0+)

`PriorityQueue<TElement, TPriority>` dequeues the element with the **lowest priority value** first (min-heap).

```csharp
var pq = new PriorityQueue<string, int>();

pq.Enqueue("Low priority",    10);
pq.Enqueue("Critical",         1);
pq.Enqueue("Medium priority",  5);

// Dequeue always returns the element with LOWEST priority number
pq.Dequeue()  // "Critical" (priority=1)
pq.Dequeue()  // "Medium priority" (priority=5)
pq.Dequeue()  // "Low priority" (priority=10)
```

> **Important**: C#'s PriorityQueue is a **min-heap** by default. For max-heap behavior, negate your priorities (e.g., store `-importance`).

---

## Real-World Applications

| Use Case | How Queue Helps |
|---|---|
| **Task/Job scheduling** | Tasks queued in order, processed in FIFO order |
| **Print spooler** | Documents queued, printed one by one |
| **Web server request queue** | HTTP requests served in arrival order |
| **BFS graph traversal** | Nodes explored level by level |
| **Message queue** (RabbitMQ, Kafka) | Async message passing between services |
| **CPU scheduling (round-robin)** | Each process gets a time slice, queued for next turn |
| **Buffer for I/O operations** | Read-ahead buffers, keyboard input buffering |
| **Call center queue** | Callers held in FIFO order until agent is free |

---

## Interview Questions & Answers

### Q1: What is the difference between Queue and Stack?

**Answer:**
- **Queue** = FIFO. Add at rear, remove from front. First come, first served.
- **Stack** = LIFO. Add and remove from same end (top). Last in, first out.

Queue has two "active" ends; Stack has one. Queue is used for scheduling and BFS; Stack is used for undo/redo and DFS.

---

### Q2: How do you implement a Stack using two Queues?

**Answer (push-expensive approach):**
```csharp
class StackUsingTwoQueues<T>
{
    Queue<T> q1 = new(); // main queue
    Queue<T> q2 = new(); // helper queue

    public void Push(T item)
    {
        q2.Enqueue(item);              // put new item in q2

        while (q1.Count > 0)           // move everything from q1 to q2
            q2.Enqueue(q1.Dequeue());  // q2 now has: new item at front, then old items

        (q1, q2) = (q2, q1);           // swap: q1 is now correct order, q2 empty
    }

    public T Pop() => q1.Dequeue();    // O(1)
    public T Peek() => q1.Peek();      // O(1)
}
// Push is O(n), Pop/Peek is O(1)
```

---

### Q3: What is a Circular Queue and why is it needed?

**Answer:** In a simple array-based queue, after many enqueue/dequeue operations, the front pointer marches to the right and the left portion of the array is "wasted" even though space is logically available. A circular queue uses `(index + 1) % capacity` to wrap around, reusing the free space at the front. This gives O(1) enqueue/dequeue without memory waste.

---

### Q4: How does BFS use a Queue?

**Answer:** In Breadth-First Search:
1. Start: enqueue the source node
2. While queue is not empty: dequeue a node, process it, enqueue all its unvisited neighbors
3. Because Queue is FIFO, we always process nodes in order of increasing distance from source

This guarantees BFS explores level-by-level — level 0 (start), level 1 (neighbors), level 2 (neighbors of neighbors), etc. This property makes BFS perfect for **finding shortest paths**.

---

### Q5: When would you use PriorityQueue instead of Queue?

**Answer:**
- Use `Queue<T>` when all items have the **same priority** and you want strict FIFO order
- Use `PriorityQueue<T,P>` when items have **different urgency levels** and higher-priority items should be processed before lower-priority ones

Examples where PriorityQueue wins:
- Hospital triage (critical patients before minor cases)
- CPU process scheduling (higher-priority processes first)
- Dijkstra's algorithm (always expand the lowest-cost node next)
- Event simulation (process earliest-timestamp events first)

---

### Q6: What is the time complexity of PriorityQueue operations?

**Answer:** `PriorityQueue<T,P>` is backed by a **min-heap** (binary heap):
- `Enqueue`: O(log n) — element must bubble up to its correct heap position
- `Dequeue`: O(log n) — root removed, last element moves to root and sinks down
- `Peek`: O(1) — root is always accessible

vs. `Queue<T>` (array-based):
- `Enqueue`: O(1) amortized
- `Dequeue`: O(1)

---

## Scenario-Based Questions

### Scenario 1: Order Processing System
> "An e-commerce site receives orders continuously. Orders should be processed in the order they arrive."

**Answer:** Use `Queue<Order>`:
- Customer places order → `queue.Enqueue(order)`
- Warehouse picks up order → `queue.Dequeue()` (FIFO = fair, first placed = first processed)
- Can add priority lane for premium customers with `PriorityQueue<Order, int>`

---

### Scenario 2: Hospital Emergency Room
> "Design a triage system where critical patients are seen before non-critical ones, but within the same severity level, earlier arrivals are seen first."

**Answer:** Use `PriorityQueue<Patient, (int severity, DateTime arrived)>`:
- `severity = 1` (critical) → seen first
- Within same severity, earlier `arrived` timestamp → seen first
- Implement `IComparer` for compound priority

---

### Scenario 3: Rate Limiter / Sliding Window
> "Implement a rate limiter that allows at most N requests per minute."

**Answer:** Use `Queue<DateTime>` storing timestamps of recent requests:
```csharp
Queue<DateTime> window = new Queue<DateTime>();
int maxRequests = 100;

bool Allow()
{
    DateTime now = DateTime.UtcNow;
    DateTime oneMinuteAgo = now.AddMinutes(-1);

    // Remove old timestamps outside the window
    while (window.Count > 0 && window.Peek() < oneMinuteAgo)
        window.Dequeue();

    if (window.Count < maxRequests)
    {
        window.Enqueue(now);
        return true;  // allowed
    }
    return false;  // rate limited
}
```

---

## Common Mistakes

1. **Dequeue on empty queue**: Always check `Count > 0` or use `TryDequeue()`
2. **Using Queue for LIFO**: Queue is FIFO only — use Stack for LIFO
3. **Iterating while modifying**: Don't Enqueue/Dequeue while using foreach — use a while loop with Count
4. **Assuming PriorityQueue is FIFO for equal priorities**: Equal-priority elements may not be in FIFO order — C#'s PriorityQueue does not guarantee stable ordering of equal priorities
5. **Index access**: Queue doesn't support indexing — use `ToArray()` first if you need it

---

## Summary

```
Queue:          FIFO | Add rear, remove front | O(1) Enqueue/Dequeue
CircularQueue:  Same FIFO but wraps array to avoid wasted space  
PriorityQueue:  Min-heap | Lowest priority value dequeued first | O(log n)
Deque:          Double-ended | Add/remove from BOTH ends | O(1) all sides
```
