// ============================================================
// PHASE 15: HEAPS & PRIORITY QUEUE — Complete C# Implementation
// Topics: Min Heap, Max Heap, heap operations, PriorityQueue<T,P>,
//         Heap Sort, Top-K problems, applications
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;

// ============================================================
// SECTION 1: WHAT IS A HEAP?
//
// A Heap is a COMPLETE BINARY TREE (all levels filled left to right)
// with the HEAP PROPERTY:
//   - Min Heap: parent ≤ both children (root = MINIMUM)
//   - Max Heap: parent ≥ both children (root = MAXIMUM)
//
// Clever trick: stored as an ARRAY (not linked nodes)!
// For node at index i:
//   - Left child:  index = 2*i + 1
//   - Right child: index = 2*i + 2
//   - Parent:      index = (i - 1) / 2
//
// Min Heap example:
//         1         ← ROOT (minimum!)
//        / \
//       3   5
//      / \ / \
//     7  4 8  6
//
// Array: [1, 3, 5, 7, 4, 8, 6]
//         0  1  2  3  4  5  6
// ============================================================

// ============================================================
// SECTION 2: CUSTOM MIN HEAP IMPLEMENTATION
// Shows what PriorityQueue<T,P> does internally
// ============================================================

public class MinHeap
{
    private int[] _data;    // internal array
    private int _size;      // current number of elements
    private int _capacity;

    public int Count => _size;
    public bool IsEmpty => _size == 0;

    public MinHeap(int capacity = 16)
    {
        _capacity = capacity;
        _data = new int[_capacity];
        _size = 0;
    }

    // Index helpers
    private int Parent(int i)    => (i - 1) / 2;
    private int LeftChild(int i) => 2 * i + 1;
    private int RightChild(int i)=> 2 * i + 2;
    private bool HasParent(int i) => i > 0;
    private bool HasLeft(int i)  => LeftChild(i) < _size;
    private bool HasRight(int i) => RightChild(i) < _size;

    private void Swap(int i, int j)
    {
        (_data[i], _data[j]) = (_data[j], _data[i]);
    }

    // --- INSERT (Enqueue) — O(log n) ---
    // Add element at the END of the array, then BUBBLE UP (sift up)
    // to restore the heap property.
    //
    // Example: insert 2 into [1,3,5,7,4,8,6]
    // Place 2 at end: [1,3,5,7,4,8,6,2] → index 7, parent at index 3 (value 7)
    // 2 < 7 → swap: [1,3,5,2,4,8,6,7] → index 3, parent at index 1 (value 3)
    // 2 < 3 → swap: [1,2,5,3,4,8,6,7] → index 1, parent at index 0 (value 1)
    // 2 > 1 → stop! Heap property restored.
    public void Insert(int val)
    {
        if (_size == _capacity)
        {
            _capacity *= 2;
            Array.Resize(ref _data, _capacity);
        }

        _data[_size++] = val;   // place at end
        BubbleUp(_size - 1);    // restore heap property
    }

    private void BubbleUp(int i)
    {
        // Keep swapping with parent while parent > child (violates min-heap)
        while (HasParent(i) && _data[Parent(i)] > _data[i])
        {
            Swap(i, Parent(i));
            i = Parent(i);  // move up to former parent's position
        }
    }

    // --- PEEK — O(1) ---
    // Root is always the minimum — just return it
    public int Peek()
    {
        if (IsEmpty) throw new InvalidOperationException("Heap is empty");
        return _data[0];  // root = minimum
    }

    // --- EXTRACT MIN (Dequeue) — O(log n) ---
    // Remove and return the root (minimum).
    // Replace root with LAST element, then BUBBLE DOWN (sift down)
    // to restore heap property.
    //
    // Example: extract from [1,2,5,3,4,8,6,7]
    // Move last element to root: [7,2,5,3,4,8,6] (removed 7 from end)
    // 7 > min(2,5)=2 → swap with left child: [2,7,5,3,4,8,6]
    // 7 > min(3,4)=3 → swap with left child: [2,3,5,7,4,8,6]
    // 7 > min(no children)? wait 7 is a leaf now → STOP
    public int ExtractMin()
    {
        if (IsEmpty) throw new InvalidOperationException("Heap is empty");

        int min = _data[0];              // save the minimum (root)
        _data[0] = _data[_size - 1];     // move last element to root
        _size--;                          // shrink heap
        if (_size > 0) BubbleDown(0);    // restore heap property

        return min;
    }

    private void BubbleDown(int i)
    {
        while (HasLeft(i))  // while node has at least one child (no left = no children in complete tree)
        {
            // Find the smaller child
            int smallerChild = LeftChild(i);
            if (HasRight(i) && _data[RightChild(i)] < _data[LeftChild(i)])
                smallerChild = RightChild(i);

            // If current node ≤ smaller child → heap property satisfied, stop
            if (_data[i] <= _data[smallerChild]) break;

            Swap(i, smallerChild);  // swap with smaller child
            i = smallerChild;       // continue from that position
        }
    }

    // --- BUILD HEAP from array — O(n) ---
    // More efficient than inserting n elements one by one (which would be O(n log n))
    public static MinHeap FromArray(int[] arr)
    {
        MinHeap heap = new MinHeap(arr.Length * 2);
        Array.Copy(arr, heap._data, arr.Length);
        heap._size = arr.Length;

        // Heapify from bottom up (start from last non-leaf node)
        // Last non-leaf index = (size/2) - 1
        for (int i = heap._size / 2 - 1; i >= 0; i--)
            heap.BubbleDown(i);

        return heap;
    }

    public void Print()
    {
        Console.Write("  Heap (array): [");
        for (int i = 0; i < _size; i++)
        {
            Console.Write(_data[i]);
            if (i < _size - 1) Console.Write(", ");
        }
        Console.WriteLine($"]  Min = {(IsEmpty ? "none" : _data[0].ToString())}");
    }
}

// ============================================================
// SECTION 3: HEAP SORT — O(n log n) time, O(1) space
//
// Two phases:
// 1. BUILD MAX HEAP from array — O(n)
// 2. Extract max elements one by one — O(n log n)
//    Place each extracted max at the END of the array
// ============================================================

class HeapSort
{
    public static void Sort(int[] arr)
    {
        int n = arr.Length;

        // Phase 1: Build max heap — heapify from bottom up
        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(arr, n, i);  // treat full array as heap

        // Phase 2: Extract elements one by one
        for (int i = n - 1; i > 0; i--)
        {
            // Max element (root) is at index 0 → move it to the end
            (arr[0], arr[i]) = (arr[i], arr[0]);

            // Heapify the reduced heap (first i elements)
            Heapify(arr, i, 0);
        }
    }

    // Max-Heapify: ensure subtree rooted at i satisfies max-heap property
    private static void Heapify(int[] arr, int heapSize, int i)
    {
        int largest = i;                 // assume root is largest
        int left    = 2 * i + 1;
        int right   = 2 * i + 2;

        if (left  < heapSize && arr[left]  > arr[largest]) largest = left;
        if (right < heapSize && arr[right] > arr[largest]) largest = right;

        if (largest != i)  // root is NOT the largest → swap and continue
        {
            (arr[i], arr[largest]) = (arr[largest], arr[i]);
            Heapify(arr, heapSize, largest);  // continue heapifying down
        }
    }
}

// ============================================================
// SECTION 4: TOP K PROBLEMS USING HEAPS
// Very common in interviews!
// ============================================================

class TopKProblems
{
    // --- TOP K LARGEST ELEMENTS --- O(n log k)
    // Use a MIN HEAP of size k.
    // After processing all elements, heap contains the k largest.
    // (Min heap of size k: when we see an element > heap.min, replace heap.min with it)
    public static int[] TopKLargest(int[] nums, int k)
    {
        // C# PriorityQueue acts as min-heap (lowest priority = first out)
        PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();

        foreach (int num in nums)
        {
            minHeap.Enqueue(num, num);  // element and priority are the same

            // Keep only the k largest (evict minimum when size exceeds k)
            if (minHeap.Count > k)
                minHeap.Dequeue();  // removes the smallest element
        }

        // Extract remaining k elements
        int[] result = new int[k];
        for (int i = k - 1; i >= 0; i--)
            result[i] = minHeap.Dequeue();  // extract in ascending order

        return result;
    }

    // --- KTH LARGEST ELEMENT --- O(n log k)
    // Same as above, but return only the top element.
    public static int KthLargest(int[] nums, int k)
    {
        PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();

        foreach (int num in nums)
        {
            minHeap.Enqueue(num, num);
            if (minHeap.Count > k)
                minHeap.Dequeue();
        }

        return minHeap.Peek();  // root of k-element min heap = kth largest
    }

    // --- MERGE K SORTED ARRAYS --- O(n log k)
    // Use a min-heap to efficiently find the minimum across k array heads.
    public static int[] MergeKSortedArrays(int[][] arrays)
    {
        // Heap element: (value, arrayIndex, elementIndex)
        var heap = new PriorityQueue<(int val, int arrIdx, int elemIdx), int>();

        // Initialize: push first element of each array into heap
        for (int i = 0; i < arrays.Length; i++)
        {
            if (arrays[i].Length > 0)
                heap.Enqueue((arrays[i][0], i, 0), arrays[i][0]);
        }

        var result = new List<int>();

        while (heap.Count > 0)
        {
            var (val, arrIdx, elemIdx) = heap.Dequeue();
            result.Add(val);

            // Push next element from the same array (if any)
            int nextElemIdx = elemIdx + 1;
            if (nextElemIdx < arrays[arrIdx].Length)
            {
                int nextVal = arrays[arrIdx][nextElemIdx];
                heap.Enqueue((nextVal, arrIdx, nextElemIdx), nextVal);
            }
        }

        return result.ToArray();
    }

    // --- FIND MEDIAN FROM DATA STREAM ---
    // Maintain two heaps:
    //   maxHeap (lower half) | median | minHeap (upper half)
    // Both heaps straddle the median.
    public static void MedianFinderDemo()
    {
        Console.WriteLine("  Median from data stream:");

        // Max-heap for lower half (negate priority for max-heap behavior)
        PriorityQueue<int, int> maxHeap = new PriorityQueue<int, int>();
        // Min-heap for upper half
        PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();

        int[] stream = { 5, 15, 1, 3, 2, 8, 7, 9, 10, 6, 11, 4 };

        foreach (int num in stream)
        {
            // Add to appropriate heap
            if (maxHeap.Count == 0 || num <= GetMax(maxHeap))
                maxHeap.Enqueue(num, -num);  // negate → max-heap behavior
            else
                minHeap.Enqueue(num, num);

            // Balance: ensure maxHeap.length ∈ {minHeap.length, minHeap.length + 1}
            while (maxHeap.Count > minHeap.Count + 1)
            {
                int top = GetMaxAndRemove(maxHeap);
                minHeap.Enqueue(top, top);
            }
            while (minHeap.Count > maxHeap.Count)
            {
                int top = minHeap.Dequeue();
                maxHeap.Enqueue(top, -top);
            }

            // Calculate current median
            double median;
            if (maxHeap.Count == minHeap.Count)
                median = (GetMax(maxHeap) + minHeap.Peek()) / 2.0;
            else
                median = GetMax(maxHeap);

            Console.WriteLine($"    Added {num,3} → median = {median}");
        }
    }

    private static int GetMax(PriorityQueue<int, int> maxHeap) =>
        maxHeap.TryPeek(out int val, out _) ? val : 0;

    private static int GetMaxAndRemove(PriorityQueue<int, int> maxHeap)
    {
        maxHeap.TryDequeue(out int val, out _);
        return val;
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class HeapsProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 15: HEAPS & PRIORITY QUEUE");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Custom Min Heap
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Custom Min Heap ===");

        var heap = new MinHeap();

        int[] toInsert = { 10, 5, 14, 3, 8, 1, 9 };
        Console.Write("  Inserting: ");
        foreach (int v in toInsert) { Console.Write(v + " "); heap.Insert(v); }
        Console.WriteLine();
        heap.Print();  // array representation of heap

        Console.WriteLine($"  Peek (min): {heap.Peek()}");        // 1
        Console.WriteLine($"  ExtractMin: {heap.ExtractMin()}");  // 1
        Console.WriteLine($"  ExtractMin: {heap.ExtractMin()}");  // 3
        heap.Print();

        // Build heap from array (O(n) — more efficient than n insertions)
        int[] arr = { 7, 2, 9, 4, 1, 8, 3 };
        var builtHeap = MinHeap.FromArray(arr);
        Console.Write($"  BuildHeap from [{string.Join(",", arr)}]: ");
        builtHeap.Print();

        // -------------------------------------------------------
        // DEMO 2: C# Built-in PriorityQueue<T,P>
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: C# PriorityQueue<TElement, TPriority> ===");

        // Lower priority value = dequeued FIRST (min-heap behavior)
        var pq = new PriorityQueue<string, int>();

        pq.Enqueue("Task C", 3);
        pq.Enqueue("Task A", 1);   // urgent
        pq.Enqueue("Task D", 4);
        pq.Enqueue("Task B", 2);

        Console.WriteLine("  Dequeue order (lowest priority value first):");
        while (pq.Count > 0)
        {
            pq.TryDequeue(out string task, out int prio);
            Console.WriteLine($"    [{prio}] {task}");
        }
        // Task A (1), Task B (2), Task C (3), Task D (4)

        // -------------------------------------------------------
        // DEMO 3: Heap Sort
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Heap Sort ===");

        int[] sortArr = { 12, 11, 13, 5, 6, 7 };
        Console.WriteLine($"  Before: [{string.Join(", ", sortArr)}]");
        HeapSort.Sort(sortArr);
        Console.WriteLine($"  After:  [{string.Join(", ", sortArr)}]");

        // -------------------------------------------------------
        // DEMO 4: Top K Problems
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: Top K Problems ===");

        int[] nums = { 3, 1, 5, 12, 2, 11, 8, 4, 9, 7, 6, 10 };
        Console.WriteLine($"  Array: [{string.Join(", ", nums)}]");

        int[] top5 = TopKProblems.TopKLargest(nums, 5);
        Console.WriteLine($"  Top 5 largest: [{string.Join(", ", top5)}]");

        int kth3 = TopKProblems.KthLargest(nums, 3);
        Console.WriteLine($"  3rd largest: {kth3}");

        // Merge K sorted arrays
        int[][] arrays = {
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            new[] { 3, 6, 9 }
        };
        int[] merged = TopKProblems.MergeKSortedArrays(arrays);
        Console.WriteLine($"  Merge [[1,4,7],[2,5,8],[3,6,9]]: [{string.Join(", ", merged)}]");

        // -------------------------------------------------------
        // DEMO 5: Median from Stream
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: Running Median (Two Heaps) ===");
        TopKProblems.MedianFinderDemo();

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Heap = complete binary tree stored in array");
        Console.WriteLine("  2. Min Heap: parent ≤ children, root = minimum");
        Console.WriteLine("  3. Insert: add at end, bubble UP — O(log n)");
        Console.WriteLine("  4. Extract: remove root, move last to root, bubble DOWN — O(log n)");
        Console.WriteLine("  5. Build heap from array in O(n) (not O(n log n))");
        Console.WriteLine("  6. PriorityQueue in C# = min-heap (lowest priority = first out)");
        Console.WriteLine("  7. Top-K problems: min-heap of size k — O(n log k)");
        Console.WriteLine("  8. Running median: two heaps (max for lower half, min for upper)");
        Console.WriteLine("======================================================");
    }
}
