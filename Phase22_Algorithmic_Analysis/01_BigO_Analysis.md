# Phase 22: Algorithmic Complexity & Big-O Analysis

## What is Big-O Notation?

Big-O notation describes how an algorithm's **running time or space requirements grow** as the input size `n` grows. It describes the **worst-case upper bound**.

Key rule: **Drop constants and lower-order terms.**
- O(3n + 5) → O(n)
- O(n² + n + 100) → O(n²)
- O(2^n + n³) → O(2^n)

---

## Complexity Classifications

| Notation | Name | 1K | 1M | 1B | Example |
|----------|------|-----|-----|-----|---------|
| O(1) | Constant | 1 | 1 | 1 | Hash table lookup |
| O(log n) | Logarithmic | 10 | 20 | 30 | Binary search |
| O(n) | Linear | 1K | 1M | 1B | Linear scan |
| O(n log n) | Linearithmic | 10K | 20M | 30B | Merge sort |
| O(n²) | Quadratic | 1M | 10¹² | ∞ | Bubble sort |
| O(n³) | Cubic | 10⁹ | ∞ | ∞ | Matrix multiply |
| O(2^n) | Exponential | 10³⁰⁰ | ∞ | ∞ | Brute force subsets |
| O(n!) | Factorial | ∞ | ∞ | ∞ | Brute force permutations |

---

## Complexity Classes in C# Code

### O(1) — Constant Time
Running time does NOT depend on input size. Always one operation (or a fixed number).

```csharp
int last = array[array.Length - 1];     // Index access
dict.ContainsKey("key");                 // Hash table lookup
stack.Push(item);                        // Stack push
queue.Dequeue();                         // Queue dequeue with head pointer
hashSet.Contains(value);                 // Hash set membership
```

### O(log n) — Logarithmic Time
Each step eliminates HALF the remaining candidates. Input halving.

```csharp
// Binary search — halves search space each iteration
int BinarySearch(int[] sorted, int target)
{
    int lo = 0, hi = sorted.Length - 1;
    while (lo <= hi)
    {
        int mid = lo + (hi - lo) / 2;   // avoids overflow (vs (lo+hi)/2)
        if (sorted[mid] == target) return mid;
        else if (sorted[mid] < target) lo = mid + 1;
        else hi = mid - 1;
    }
    return -1;
}
// After each iteration: n → n/2 → n/4 → ... → 1
// Steps needed: log₂(n) iterations
```

Also: BST operations, heap insert/extract, SortedDictionary operations.

### O(n) — Linear Time
Must visit every element at least once.

```csharp
// Find max — must check all n elements
int FindMax(int[] arr)
{
    int max = int.MinValue;
    foreach (int x in arr)       // n iterations
        if (x > max) max = x;
    return max;
}
```

### O(n log n) — Linearithmic
Sorting algorithms (optimal comparison-based sort lower bound).

```csharp
// Merge sort: O(n log n) — divide: log n levels, each level processes all n elements
void MergeSort(int[] arr, int lo, int hi)
{
    if (lo >= hi) return;
    int mid = (lo + hi) / 2;
    MergeSort(arr, lo, mid);     // log n recursive levels
    MergeSort(arr, mid + 1, hi);
    Merge(arr, lo, mid, hi);     // O(n) merge at each level
}
```

### O(n²) — Quadratic
Nested loops where both iterate over the input.

```csharp
// Check all pairs — n * n/2 iterations
bool HasDuplicate_Naive(int[] arr)
{
    for (int i = 0; i < arr.Length; i++)        // n
        for (int j = i + 1; j < arr.Length; j++) // n/2
            if (arr[i] == arr[j]) return true;
    return false;
}
// Better: Use HashSet → O(n)
```

### O(2^n) — Exponential
All subsets of a set have size 2^n.

```csharp
// Generate all subsets via recursion — 2^n subsets
void GenerateSubsets(int[] arr, int idx, List<int> current)
{
    if (idx == arr.Length) { PrintSubset(current); return; }
    // Each element: include or exclude → binary tree of depth n
    GenerateSubsets(arr, idx + 1, current);                // exclude
    current.Add(arr[idx]);
    GenerateSubsets(arr, idx + 1, current);               // include
    current.RemoveAt(current.Count - 1);
}
```

---

## Space Complexity

Big-O applies to memory too. Count extra memory allocated (not including input).

| Algorithm | Space Complexity | Notes |
|-----------|----------------|-------|
| In-place sort (HeapSort) | O(1) | No extra allocation |
| MergeSort | O(n) | Temp array for merging |
| QuickSort | O(log n) | Recursion stack depth |
| BFS (graph) | O(n) | Queue can hold all nodes |
| DFS (recursive) | O(h) | h = depth of recursion/tree |
| Dynamic Programming table | O(n²) | 2D DP table |
| Hash table | O(n) | Extra space per element |

---

## How to Analyze Time Complexity

**Step-by-step method:**

1. **Identify the dominant loop** — the deepest nesting level usually dominates
2. **Count iterations of the innermost operation** as a function of n
3. **Add parallel sections** — `O(a) + O(b) = O(max(a,b))`
4. **Multiply nested sections** — outer O(n) × inner O(n) = O(n²)
5. **Apply recursion analysis** — use recursion trees or Master Theorem

```csharp
void Example(int[] arr)
{
    // Step 1: O(n)
    for (int i = 0; i < arr.Length; i++)
        Console.WriteLine(arr[i]);

    // Step 2: O(n²)
    for (int i = 0; i < arr.Length; i++)        // n
        for (int j = 0; j < arr.Length; j++)    // n
            Console.WriteLine(arr[i] + arr[j]); // O(1)

    // Total: O(n) + O(n²) = O(n²)  [dominant term wins]
}
```

---

## Recursion Analysis — Master Theorem

For recurrences of the form: `T(n) = a * T(n/b) + O(n^d)`
- `a` = number of recursive calls
- `b` = factor by which problem shrinks
- `d` = work done outside recursion

| Condition | Time Complexity | Example |
|-----------|----------------|---------|
| a < b^d | O(n^d) | Most work at root |
| a = b^d | O(n^d log n) | Work evenly distributed |
| a > b^d | O(n^log_b(a)) | Most work at leaves |

**Merge Sort:** a=2, b=2, d=1 → 2 = 2¹ → O(n log n) ✓  
**Binary Search:** a=1, b=2, d=0 → 1 = 2⁰ → O(log n) ✓  
**BinaryTree traversal:** a=2, b=2, d=0 → 2 > 2⁰ → O(n^log₂2) = O(n) ✓

---

## Amortized Analysis — List\<T\>.Add()

Not every operation is equally expensive. **Amortized** analysis looks at the **average cost over a sequence** of operations.

`List<T>.Add()` with dynamic array doubling:
- Most adds: O(1) — just write to next slot
- Occasional: O(n) — double the capacity, copy all elements

Amortized analysis: array doubles at sizes 1, 2, 4, 8, ..., n.
Total copies = 1 + 2 + 4 + ... + n = 2n.
So n adds cost 2n operations total → **O(1) amortized per add**.

---

## Common Complexity Pitfalls

```csharp
// WRONG: Thinking this is O(n log n)
foreach (string s in list)            // O(n)
    data.Contains(s);                 // O(n) using List.Contains!
// ACTUAL: O(n²) — hidden linear Contains!

// FIX: Use HashSet for O(n) total
var set = new HashSet<string>(data);  // O(n) to build
foreach (string s in list)           // O(n)
    set.Contains(s);                  // O(1) per lookup
// TOTAL: O(n)

// WRONG: Thinking string concatenation is O(n)
string result = "";
for (int i = 0; i < n; i++)
    result += items[i];               // Creates new string each time!
// ACTUAL: O(n²)  — ith concat copies i characters

// FIX: Use StringBuilder
var sb = new StringBuilder();
for (int i = 0; i < n; i++)
    sb.Append(items[i]);              // O(1) amortized each
string result = sb.ToString();        // O(n)
```

---

## C# Built-in Collections Complexity

| Operation | Array | List\<T\> | LinkedList | Dictionary | HashSet | SortedDict |
|-----------|-------|---------|-----------|-----------|---------|-----------|
| Access by index | O(1) | O(1) | O(n) | N/A | N/A | O(log n) |
| Search | O(n) | O(n) | O(n) | O(1) avg | O(1) avg | O(log n) |
| Insert at beginning | O(n) | O(n) | O(1) | N/A | N/A | O(log n) |
| Insert at end | O(n)* | O(1)** | O(1) | O(1)** | O(1)** | O(log n) |
| Remove | O(n) | O(n) | O(1)*** | O(1) avg | O(1) avg | O(log n) |

*Fixed-size arrays can't grow; **amortized; ***given node reference

---

## Interview Questions & Answers

**Q1: How do you determine the time complexity of a nested loop?**

**A:** Multiply the work at each level. For two nested loops each running n times: n × n = O(n²). If the inner loop runs from `j=i+1` to n (upper triangle), it's n×(n-1)/2 ≈ O(n²) (constant halving). If the inner loop runs log n times (binary search-like) per outer iteration: n × log n = O(n log n). Key: count the actual number of inner-body executions as a precise function of n, then simplify.

---

**Q2: What is the difference between O(n log n) and O(n²) practically?**

**A:** For n = 1,000,000: O(n log n) ≈ 20,000,000 operations (fast ≈ 0.02s), O(n²) = 10¹² operations (impossible ≈ 11 days at 10⁹ ops/sec). This is why sorting algorithms matter — QuickSort and MergeSort (O(n log n)) vs BubbleSort (O(n²)) isn't just theoretical. On a real machine, n=10,000: O(n log n) ≈ 130K ops (instant), O(n²) = 100M ops (noticeable lag). Always aim for at most O(n log n) for n > 10,000.

---

**Q3: What does O(1) space mean? Is an O(1) space algorithm always better?**

**A:** O(1) space means the algorithm uses a **constant amount of extra memory** regardless of input size — only a fixed number of variables (loop counters, temp variables, etc.), no arrays/collections sized relative to input. This is ideal for memory-constrained systems. However, tradeoffs exist: HeapSort is O(1) space but slower in practice than MergeSort (O(n) space) due to poor cache behavior. Hash tables give O(1) average time but require O(n) extra space. Time-space tradeoffs are common; choose based on the constraint that's more critical for your system.

---

**Q4: Why is string concatenation in a loop O(n²) and how do you fix it?**

**A:** In C#, strings are immutable. Each `s1 + s2` creates a new string, copying both. Concatenating strings of lengths 1, 2, 3, ..., n-1 in a loop copies 0+1+2+...+(n-1) = n(n-1)/2 characters total → O(n²). Fix: use `StringBuilder`, which maintains a mutable char array internally. Each `Append()` is O(1) amortized (just writes to the next position, doubles capacity when full). Final `ToString()` does one O(n) copy. Total: O(n). The performance difference becomes catastrophic at n > 10,000.

---

**Q5: What is amortized complexity? Give an example.**

**A:** Amortized complexity is the average cost per operation over a sequence of operations, even if individual operations vary. `List<T>.Add()`: usually O(1) (just stores element), but occasionally O(n) (copies the whole array when doubling). Over n adds, the total work is O(n) — at most 2n copies total (doubling series: 1+2+4+...+n ≤ 2n). So the amortized cost per operation is O(1). C# StringBuilder.Append is similar. The key insight: expensive operations are rare enough they don't change the per-operation average.

---

**Q6: How would you approach optimizing an algorithm that "feels slow"?**

**A:** 
1. **Measure first** — use `Stopwatch` or profiler to find the actual hot spot (don't guess)
2. **Determine current complexity** — analyze the algorithm's Big-O
3. **Set a target** — what complexity is needed for expected n?
4. **Algorithmic fix first** — O(n²) → O(n log n) beats any micro-optimization
5. **Data structure change** — replacing `List.Contains` (O(n)) with `HashSet.Contains` (O(1)) often transforms O(n²) to O(n)
6. **Then micro-optimize** — remove allocations, use Span, avoid unnecessary copies
7. **Verify with benchmarks** — BenchmarkDotNet for accurate measurements

---

## Scenario-Based Questions

**Scenario 1:** Your team's code review includes this: `foreach (user in users) { if (reports.Any(r => r.UserId == user.Id)) … }`. The code reviewer says it's "O(n²)". Explain why and how to fix it.

**Answer:** `reports.Any(r => r.UserId == user.Id)` is O(m) where m = reports.Count (linear scan). The outer loop runs O(n) times (n = users.Count). Total: O(n × m). If n ≈ m, this is O(n²). Fix: build a `HashSet<int> reporterIds = new HashSet<int>(reports.Select(r => r.UserId))` before the loop — O(m). Then `if (reporterIds.Contains(user.Id))` inside the loop — O(1). Total: O(n + m) — near-linear instead of quadratic. For 10,000 users and 10,000 reports: 100M comparisons vs 20,000 operations.

---

**Scenario 2:** You have an autocomplete feature. Given a dictionary of 100,000 words, when a user types 3 characters, you must return all words starting with those characters within 50ms. A linear scan over all words takes 80ms. What's your approach?

**Answer:** Pre-process the dictionary into a **Trie** (O(total_chars) build time). Each autocomplete query: navigate to the 3-character prefix node in O(3) = O(1), then DFS/BFS to collect all words in that subtree. If there are K matching words, retrieval is O(K + prefix tree depth) not O(n). For n=100,000 with average length 8, this reduces from O(100,000) scan to O(K) where K might be tens or hundreds of matches. Alternative: sort words alphabetically and binary search for prefix range using `SortedSet.GetViewBetween(prefix, prefix + "\uffff")` for O(log n + K).

---

**Scenario 3:** Your monthly analytics job processes 10 million records, aggregating by category. It currently takes 4 hours. You profile it and find the bottleneck is `list.Contains(category)` inside a loop. How much speedup do you expect from optimizing this?

**Answer:** `list.Contains` on unsorted data is O(n). If checking n=10M records against a category list of size m, the current complexity is O(n × m). Replace with `HashSet<string> categorySet = new HashSet<string>(categories)` → `categorySet.Contains(category)` is O(1). New complexity: O(n × 1) = O(n). If m=1,000 categories, this delivers a 1,000x speedup theoretically — from 4 hours to ~14 seconds. This shows why algorithmic improvements dwarf hardware upgrades: no server upgrade would give 1,000x speedup, but a data structure change achieves it instantly.

---

## Common Mistakes

1. **Ignoring hidden complexity** — `List.Contains` inside a loop is often the culprit for accidental O(n²)
2. **Forgetting constant factors** — O(n log n) with a small constant beats O(n) with a large constant for small n
3. **Not accounting for memory operations** — cache misses make O(n log n) work that accesses random memory slower than O(n) work with sequential memory
4. **Confusing average vs worst case** — QuickSort is O(n log n) average but O(n²) worst case; HashMap is O(1) average but O(n) worst case
5. **String concatenation trap** — `+=` in a loop is O(n²); always use StringBuilder for more than a few concats
6. **Forgetting allocation cost** — O(1) amortized for List.Add still causes GC pressure; in latency-sensitive code, consider pre-sizing with `new List<T>(expectedCapacity)`
7. **Premature optimization** — optimize algorithms (Big-O) first, then micro-optimize; profiling is essential before guessing
