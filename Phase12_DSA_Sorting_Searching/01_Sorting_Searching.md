# Phase 12: Sorting & Searching Algorithms

## Overview

Sorting and searching are the most fundamental algorithmic operations. Nearly every application requires them, and they are a cornerstone of technical interviews.

---

## Searching Algorithms

### Linear Search
Scan every element one by one until found.

- **Time:** O(n) — must check each element
- **Space:** O(1)
- **Requirement:** Array can be unsorted
- **Best for:** Small arrays, unsorted data

```csharp
for (int i = 0; i < arr.Length; i++)
    if (arr[i] == target) return i;
return -1;
```

---

### Binary Search
Repeatedly halve the search space. Requires a **sorted** array.

```
Array: [1, 3, 5, 7, 9, 11, 13]
Search: 7

Step 1: mid = 3 → arr[3]=7 → FOUND!
```

- **Time:** O(log n) — halves search space each step
- **Space:** O(1) iterative, O(log n) recursive
- **Requirement:** Array MUST be sorted
- **Best for:** Large sorted arrays

```csharp
int lo = 0, hi = arr.Length - 1;
while (lo <= hi) {
    int mid = lo + (hi - lo) / 2;  // avoids integer overflow
    if (arr[mid] == target) return mid;
    else if (arr[mid] < target) lo = mid + 1;
    else                        hi = mid - 1;
}
return -1;
```

**Why `lo + (hi - lo) / 2` instead of `(lo + hi) / 2`?**  
`lo + hi` can overflow for large indices; `hi - lo` stays within bounds.

---

## Sorting Algorithms Comparison

| Algorithm | Best | Average | Worst | Space | Stable? |
|-----------|------|---------|-------|-------|---------|
| **Bubble Sort** | O(n) | O(n²) | O(n²) | O(1) | ✅ Yes |
| **Selection Sort** | O(n²) | O(n²) | O(n²) | O(1) | ❌ No |
| **Insertion Sort** | O(n) | O(n²) | O(n²) | O(1) | ✅ Yes |
| **Merge Sort** | O(n log n) | O(n log n) | O(n log n) | O(n) | ✅ Yes |
| **Quick Sort** | O(n log n) | O(n log n) | O(n²) | O(log n) | ❌ No |
| **Heap Sort** | O(n log n) | O(n log n) | O(n log n) | O(1) | ❌ No |
| **C# Array.Sort** | O(n log n) | O(n log n) | O(n log n) | O(log n) | ❌ No |

A **stable** sort maintains the relative order of equal elements.

---

## Algorithm Details

### Bubble Sort
Repeatedly swap adjacent elements if out of order. "Bubbles" largest to end.

```
[5, 3, 1, 4] → [3, 1, 4, 5] → [1, 3, 4, 5]
```

Key optimization: if no swaps in a pass → already sorted → break early.

### Selection Sort
Find the minimum in the unsorted portion, swap it to the front.

```
[5, 3, 1, 4]
min=1 → swap → [1, 3, 5, 4]
min=3 → swap → [1, 3, 5, 4]
min=4 → swap → [1, 3, 4, 5]
```

Always O(n²) — no early exit possible.

### Insertion Sort
Like sorting playing cards. Take each element and insert it into its correct position in the sorted portion.

```
[5, 3, 1, 4]
i=1: insert 3 before 5 → [3, 5, 1, 4]
i=2: insert 1 → [1, 3, 5, 4]
i=3: insert 4 between 3 and 5 → [1, 3, 4, 5]
```

**Best for:** Nearly sorted data (O(n)) or small arrays. Used as base case in hybrid sorts.

### Merge Sort
**Divide and Conquer**: recursively split in half, sort each half, merge.

```
[5, 3, 1, 4]
Split → [5, 3] and [1, 4]
Split → [5][3]  and  [1][4]
Merge → [3, 5]  and  [1, 4]
Merge → [1, 3, 4, 5]
```

- Always O(n log n) — no worst case
- **Stable** sort
- Best for: linked lists (no extra memory), stable sorting

### Quick Sort
**Divide and Conquer**: choose a **pivot**, partition array so all < pivot on left, all > pivot on right, recurse.

```
[5, 3, 1, 4, 2]  pivot = 2
Partition: [1] [2] [5, 3, 4]
Recurse:   [1]     [3, 4, 5]
Result:    [1, 2, 3, 4, 5]
```

- Average O(n log n), but worst case O(n²) if pivot is always min or max (sorted array!)
- Fixed by random pivot selection
- In practice faster than Merge Sort due to cache locality

---

## C# Sorting Built-ins

```csharp
// Sort array in-place (uses Introsort = QuickSort + HeapSort + InsertionSort)
Array.Sort(arr);                              // ascending
Array.Sort(arr, (a, b) => b.CompareTo(a));   // descending

// Sort List<T>
list.Sort();
list.Sort((a, b) => a.Name.CompareTo(b.Name));  // by property

// LINQ (returns new IEnumerable, doesn't modify original)
var sorted = arr.OrderBy(x => x).ToArray();
var desc   = arr.OrderByDescending(x => x).ToArray();
var multi  = list.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
```

---

## Interview Questions & Answers

**Q1: If an array is nearly sorted (few elements out of place), which algorithm is best?**

**A:** Insertion Sort is best for nearly sorted data. It runs in O(n + k) where k is the number of inversions (out-of-order pairs). For nearly sorted data, k is small → approaches O(n). Bubble Sort with early termination also works. QuickSort and MergeSort wouldn't benefit and would still run in O(n log n).

---

**Q2: Why doesn't Binary Search work on unsorted arrays?**

**A:** Binary Search assumes that all elements smaller than mid are to the left and all larger are to the right. This assumption only holds in a sorted array. On an unsorted array, eliminating half the search space is invalid — the target could be anywhere in the discarded half.

---

**Q3: What is the difference between a stable and unstable sort? When does it matter?**

**A:** A stable sort preserves the relative order of elements with equal keys. Example: sorting `[(Alice, 25), (Bob, 25), (Carol, 30)]` by age — a stable sort guarantees Alice stays before Bob. Stability matters when sorting by multiple keys sequentially (sort by last name first, then by first name), or when the sort order carries semantic meaning (display order).

---

**Q4: What is QuickSort's worst case and how do you avoid it?**

**A:** QuickSort's worst case O(n²) occurs when the pivot is always the min or max element — happens with sorted/reverse-sorted arrays using the first/last element as pivot. Solutions: (1) **Random pivot** — pick a random element; (2) **Median of three** — pivot = median of first, middle, last; (3) Use **Introsort** (C#'s Array.Sort) which switches to HeapSort when recursion depth exceeds 2·log n.

---

**Q5: How does Array.Sort() work internally in C#?**

**A:** C# uses **Introsort** — a hybrid algorithm combining: QuickSort (fast in practice), HeapSort (O(n log n) worst case guarantee), and InsertionSort (for small arrays ≤ 16 elements, very fast). This gives O(n log n) worst case with fast practical performance.

---

**Q6: How would you find the kth smallest element in an array?**

**A:** Multiple approaches:
1. **Sort + Index**: Sort array, return `arr[k-1]`. O(n log n)
2. **Min-Heap**: Build min-heap, extract k times. O(n + k log n)
3. **QuickSelect**: Partition like QuickSort — if pivot position = k, return pivot; else recurse on one side. **O(n) average**, O(n²) worst case. This is the most efficient for interviews.

---

## Scenario-Based Questions

**Scenario 1:** You have a list of 10 million customer records that are updated daily with ~0.1% changes. You need to sort them by last name for display. What approach?

**Answer:** Since the data is nearly sorted (99.9% unchanged), use **Insertion Sort** or **TimSort** (Python's default, also used by Java). TimSort is a hybrid of Merge Sort and Insertion Sort that detects "runs" (already sorted sequences) and merges them — runs in O(n) for nearly sorted data. In C# (and .NET), `List<T>.Sort()` uses a similar strategy.

---

**Scenario 2:** A search bar needs to autocomplete as users type. The dictionary has 500,000 words. How do you implement efficient prefix matching?

**Answer:** Don't sort and binary search — use a **Trie** (prefix tree). Insert all words during initialization. For each keystroke, traverse the Trie in O(prefix length) and collect all words in the subtree. Binary search on a sorted array gives O(log n + results) but doesn't scale as naturally for prefix queries as a Trie does.

---

**Scenario 3:** You need to merge 100 sorted log files (each with 1 million lines) into one sorted file. How?

**Answer:** Use a **K-way merge** with a min-heap. Initialize a min-heap of size 100 with one line from each file. Repeatedly extract the minimum, write it to output, and insert the next line from that file. Time: O(n log K) where n = total lines, K = number of files (100). This avoids loading all files into memory — only one line per file in the heap at a time.

---

## Common Mistakes

1. **Using Binary Search on unsorted data** → always verify the precondition (sorted)
2. **Integer overflow in mid:** `(lo + hi) / 2` overflows for large indices → use `lo + (hi - lo) / 2`
3. **QuickSort on already-sorted data without random pivot** → O(n²) worst case
4. **Choosing wrong algorithm for data type:**
   - Strings with common prefixes → Radix Sort or Trie
   - Integers in small range → Counting Sort O(n+k)
   - Objects with multiple keys → stable sort (Merge Sort or TimSort)
5. **Forgetting to handle duplicate elements** in Binary Search (LeetCode "Find First/Last Position")
