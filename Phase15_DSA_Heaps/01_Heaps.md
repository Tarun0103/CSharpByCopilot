# Phase 15: Heaps & Priority Queue

## What is a Heap?

A **Heap** is a **complete binary tree** satisfying the **heap property**:
- **Min-Heap:** Parent ≤ both children (root = minimum element)
- **Max-Heap:** Parent ≥ both children (root = maximum element)

```
Min-Heap:           Max-Heap:
       1                  9
      / \               /   \
     3   2            7     6
    / \ / \          / \   / \
   7  8 5  9        4   3  1   2
```

"Complete binary tree" = all levels filled except possibly the last, which is filled **left to right**.

---

## Array Representation (No Pointers!)

A heap is stored as an array. Index relationships:
- Parent of node at index `i`: `(i-1) / 2`
- Left child of node at index `i`: `2*i + 1`
- Right child of node at index `i`: `2*i + 2`

```
Heap: [1, 3, 2, 7, 8, 5, 9]
       0  1  2  3  4  5  6   (indices)

Node 0 (value=1): children at 1 (=3) and 2 (=2)
Node 1 (value=3): parent at 0 (=1), children at 3 (=7) and 4 (=8)
```

---

## Core Operations

### Insert — O(log n)
1. Add element at end of array
2. **Bubble Up (Sift Up):** Compare with parent, swap if out of order. Repeat until heap property restored.

```
Insert 2 into [1, 3, 5, 7, 8]:
→ [1, 3, 5, 7, 8, 2]  (added at end)
→ 2 < parent 5 → swap:  [1, 3, 2, 7, 8, 5]
→ 2 > parent 1 → stop.  Min-Heap restored!
```

### Extract Min — O(log n)
1. Min is always at root (index 0)
2. Swap root with last element
3. Remove last element
4. **Bubble Down (Sift Down):** Swap with smallest child until heap property restored.

### Peek — O(1)
Return root without removing.

### Build Heap from Array — O(n)
Heapify from bottom up. More efficient than n individual inserts (O(n log n)).

---

## Operations Summary

| Operation | Time | Notes |
|-----------|------|-------|
| Insert | O(log n) | Bubble up |
| Extract min/max | O(log n) | Bubble down |
| Peek (min/max) | O(1) | Root element |
| Build from array | O(n) | Heapify in-place |
| Heap Sort | O(n log n) | Extract n times |
| Find arbitrary element | O(n) | Not designed for this |
| Delete arbitrary element | O(log n) | Find O(n) + delete O(log n) |

---

## Heap Sort

1. Build a max-heap from the array — O(n)
2. Repeatedly extract max (root), place at end — O(n log n)
3. Result: sorted ascending array, in-place, O(1) extra space

---

## C# PriorityQueue<TElement, TPriority>

Added in .NET 6. A **min-heap** by default (smallest priority value = highest priority).

```csharp
var pq = new PriorityQueue<string, int>();

pq.Enqueue("Low priority",    10);
pq.Enqueue("High priority",   1);
pq.Enqueue("Medium priority", 5);

pq.TryDequeue(out string item, out int priority);
// item = "High priority", priority = 1  (smallest first!)
```

For max-heap behavior, **negate the priority**:
```csharp
pq.Enqueue("Critical", -100);  // -100 < -5 → dequeued first
pq.Enqueue("Low",       -1);
// Dequeue gets "Critical" first (priority -100, most negative = smallest)
```

---

## Common Interview Problems

### Top K Largest Elements
Use a **min-heap of size K**. Keep only K largest elements.
- Add each element to the heap
- If heap size > K, remove the minimum
- Final heap contains K largest; root = Kth largest

Time: O(n log K) — much better than O(n log n) sort for large n and small K

### Kth Largest Element
Same as above — extract min when heap exceeds K; final min = Kth largest.

### Merge K Sorted Arrays
Use min-heap of size K:
- Initialize with first element from each array
- Repeat: extract min → add to result → push next element from same array
- Time: O(n log K) where n = total elements, K = number of arrays

### Median from Data Stream
Maintain two heaps:
- Max-heap for lower half
- Min-heap for upper half
- Balance sizes to differ by at most 1
- Median = top of larger heap (odd total) or average of both tops (even total)

---

## Heap vs other data structures

| | Array | Sorted Array | BST | Heap |
|-|-------|-------------|-----|------|
| Insert | O(1) | O(n) | O(log n) | O(log n) |
| Find min/max | O(n) | O(1) | O(log n) | O(1) |
| Delete min/max | O(n) | O(1) | O(log n) | O(log n) |
| Arbitrary search | O(n) | O(log n) | O(log n) | O(n) |
| **Best use** | General | Sorted access | All | Only min/max access |

---

## Interview Questions & Answers

**Q1: What is the difference between a heap and a binary search tree?**

**A:** A BST maintains order (left subtree < root < right subtree) allowing O(log n) search for any element. A heap maintains the heap property (parent ≤ children in min-heap), optimized ONLY for finding/removing the minimum (or maximum). BST supports arbitrary ordered operations; heap only supports efficient min/max operations. Heap uses an array (no pointers, cache-friendly); BST uses linked nodes.

---

**Q2: Why is building a heap O(n) and not O(n log n)?**

**A:** Building by inserting one at a time is O(n log n). The O(n) method (heapify) works bottom-up: start from the last non-leaf node (index n/2-1), apply sift-down to each node toward the root. Lower nodes require fewer swaps (less sifting). By the mathematical analysis of this sum: ~n/2 nodes need 0 swaps (leaves), ~n/4 nodes need 1 swap, ~n/8 need 2 swaps... Total operations = O(n) by summing the geometric series.

---

**Q3: What makes PriorityQueue different from a regular Queue?**

**A:** A regular Queue (FIFO) dequeues in the order elements were enqueued. A PriorityQueue dequeues the element with the **smallest priority value** (in C#'s min-heap implementation), regardless of insertion order. This allows high-priority items to "skip ahead" of lower-priority ones. Use it for: task scheduling, Dijkstra's algorithm, event-driven simulation, hospital triage.

---

**Q4: How would you efficiently find the Kth largest element?**

**A:** Use a min-heap of size K. Add each element; remove the minimum when heap exceeds K. After processing all n elements, the heap root is the Kth largest. Time: O(n log K). This is more efficient than sorting (O(n log n)) when K << n. Alternative: QuickSelect O(n) average, but O(n²) worst case.

---

**Q5: How does HeapSort compare to MergeSort and QuickSort?**

**A:**
- **HeapSort:** O(n log n) guaranteed, O(1) extra space, unstable sort. Worst for cache performance (random memory access during heap operations).
- **MergeSort:** O(n log n) guaranteed, O(n) extra space, stable sort. Excellent for linked lists and external sort.
- **QuickSort:** O(n log n) average, O(n²) worst, O(log n) extra space, unstable. Best practical performance due to cache locality.
- In practice: C#'s Array.Sort (Introsort) combines QuickSort + HeapSort + InsertionSort.

---

**Q6: Explain "Find the Median in a Data Stream" problem.**

**A:** Maintain two heaps: a max-heap (lower half) and a min-heap (upper half). For each new number:
1. Add to max-heap
2. Move max-heap root to min-heap (balance)
3. If min-heap is larger than max-heap, move min-heap root to max-heap (rebalance)

Invariant: max-heap.size == min-heap.size OR max-heap.size == min-heap.size + 1

Median: if sizes equal → average of both tops; else → max-heap top. All operations O(log n).

---

## Scenario-Based Questions

**Scenario 1:** You're building a hospital emergency room triage system. Patients arrive continuously and must be treated in order of severity (critical before urgent before non-urgent). New patients can arrive at any time, even between treatments. How do you implement this?

**Answer:** Use a **PriorityQueue<Patient, int>** with priority = severity level (1=Critical, 2=Urgent, 3=Non-urgent). New patients: `pq.Enqueue(patient, severity)`. Treating: `pq.TryDequeue(out patient, out _)` — always gets the most critical patient. If two patients have the same severity, add a timestamp as tiebreaker: `Enqueue(patient, (severity, timestamp))`. Time: O(log n) per arrival/treatment.

---

**Scenario 2:** You have 1 billion log entries sorted by timestamp. Memory allows loading only K=1,000 entries at a time. How do you find the top 100 entries by severity?

**Answer:** Process entries in chunks of K. For each chunk, use a **min-heap of size 100** to track top 100 severities seen so far. When the heap reaches 100 elements, compare each new entry's severity with the heap minimum — if larger, pop minimum, push new entry. After processing all chunks, the heap contains the top 100 overall. Time: O(n log 100) = O(n).

---

**Scenario 3:** Implement a "job scheduler" that handles recurring tasks — some tasks should run every 5 minutes, others every 1 hour. No two tasks should run at exactly the same time.

**Answer:** Use a min-heap keyed by **next scheduled time**. Initialize heap with (nextRunTime, task) for all tasks. The scheduler loop: peek the heap. If `nextRunTime <= now` → dequeue and execute task, compute next run time, re-enqueue. Else → sleep until next task. This event-driven approach uses O(n) memory for n tasks and O(log n) per scheduling operation — the standard approach in cron schedulers and event loops.

---

## Common Mistakes

1. **Forgetting PriorityQueue is a min-heap** → largest priority number dequeues LAST in C#; negate for max-heap
2. **Using a regular Queue for priority tasks** → correctness issue; Queue is FIFO only
3. **Confusing Heap with BST** → Heap doesn't support arbitrary search; BST does
4. **Build heap wrong → O(n log n) instead of O(n)** → should start from `n/2-1` and sift down toward root
5. **Using Heap when BST is better** → if you need min, max, AND search by value, use SortedSet (BST), not Heap
6. **Off-by-one in parent/child index formulas** — parent: `(i-1)/2`, left child: `2i+1`, right child: `2i+2`
