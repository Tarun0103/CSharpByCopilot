// ============================================================
// PHASE 12: SORTING & SEARCHING — Complete C# Implementation
// Topics: Linear Search, Binary Search, Bubble Sort,
//         Selection Sort, Insertion Sort, Merge Sort, Quick Sort,
//         Built-in C# sorting, Time Complexity Comparison
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

// ============================================================
// SEARCHING ALGORITHMS
// ============================================================

class SearchingAlgorithms
{
    // ========================================================
    // LINEAR SEARCH — O(n) time, O(1) space
    // Check every element one by one until found (or not found).
    // Works on UNSORTED arrays.
    // Best for: small arrays, unsorted data, single searches
    // ========================================================
    public static int LinearSearch(int[] arr, int target)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == target)   // found it!
                return i;           // return the index
        }
        return -1;  // -1 means "not found" (convention used everywhere)
    }

    // Generic version — works with any comparable type
    public static int LinearSearchGeneric<T>(T[] arr, T target) where T : IEquatable<T>
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(target))
                return i;
        }
        return -1;
    }

    // ========================================================
    // BINARY SEARCH — O(log n) time, O(1) space
    // Works ONLY on SORTED arrays.
    // Repeatedly halves the search range by comparing the middle element.
    //
    // Visual example: search for 7 in [1, 3, 5, 7, 9, 11, 13]
    //   Step 1: left=0, right=6, mid=3 → arr[3]=7 == 7 → FOUND!
    //
    // Another: search for 11 in [1, 3, 5, 7, 9, 11, 13]
    //   Step 1: left=0, right=6, mid=3 → arr[3]=7 < 11 → search RIGHT half
    //   Step 2: left=4, right=6, mid=5 → arr[5]=11 == 11 → FOUND!
    // ========================================================
    public static int BinarySearch(int[] arr, int target)
    {
        int left = 0;               // Start of search range
        int right = arr.Length - 1; // End of search range

        while (left <= right)
        {
            // Calculate midpoint carefully — avoid integer overflow!
            // WRONG: mid = (left + right) / 2  ← can overflow for large indices
            // CORRECT: mid = left + (right - left) / 2
            int mid = left + (right - left) / 2;

            if (arr[mid] == target)
                return mid;       // Found!

            if (arr[mid] < target)
                left = mid + 1;   // Target is in the RIGHT half
            else
                right = mid - 1;  // Target is in the LEFT half
        }

        return -1;  // Not found
    }

    // Recursive Binary Search — same logic, uses call stack instead of loop
    public static int BinarySearchRecursive(int[] arr, int target, int left, int right)
    {
        if (left > right) return -1;  // base case: search range empty

        int mid = left + (right - left) / 2;

        if (arr[mid] == target) return mid;
        if (arr[mid] < target)
            return BinarySearchRecursive(arr, target, mid + 1, right);  // search right
        else
            return BinarySearchRecursive(arr, target, left, mid - 1);   // search left
    }

    // Binary Search — First occurrence (for arrays with duplicates)
    // Returns the LEFTMOST index of target
    public static int BinarySearchFirst(int[] arr, int target)
    {
        int left = 0, right = arr.Length - 1, result = -1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (arr[mid] == target)
            {
                result = mid;       // Record this as a candidate answer
                right = mid - 1;   // Keep searching LEFT for earlier occurrence
            }
            else if (arr[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return result;
    }
}

// ============================================================
// SORTING ALGORITHMS
// ============================================================

class SortingAlgorithms
{
    // Helper to print an array
    private static void Print(int[] arr, string label = "")
    {
        if (!string.IsNullOrEmpty(label)) Console.Write($"  {label}: ");
        Console.WriteLine("[" + string.Join(", ", arr) + "]");
    }

    // Helper to swap two elements in an array
    private static void Swap(int[] arr, int i, int j)
    {
        // Tuple swapping — clean and readable
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }

    // ========================================================
    // BUBBLE SORT — O(n²) best/avg/worst, O(1) space
    // Repeatedly compare adjacent elements and swap them if out of order.
    // After each full pass, the LARGEST unsorted element "bubbles" to its correct position.
    //
    // Visual: [5, 3, 8, 1, 2]
    // Pass 1: [3,5,8,1,2] → [3,5,1,8,2] → [3,5,1,2,8]  ← 8 bubbled to end
    // Pass 2: [3,1,5,2,8] → [3,1,2,5,8]                ← 5 bubbled to position
    //
    // STABLE sort (equal elements maintain original relative order)
    // Simple but inefficient — only use for tiny arrays or teaching
    // ========================================================
    public static void BubbleSort(int[] arr)
    {
        int n = arr.Length;

        for (int pass = 0; pass < n - 1; pass++)
        {
            bool swapped = false;  // Optimization: detect if already sorted

            // Each pass: push the largest unsorted element to the right
            // After pass i, the last i+1 elements are sorted (we stop early)
            for (int j = 0; j < n - pass - 1; j++)
            {
                if (arr[j] > arr[j + 1])      // out of order?
                {
                    Swap(arr, j, j + 1);       // swap them
                    swapped = true;
                }
            }

            // If no swaps happened this pass → array is already sorted!
            if (!swapped) break;  // early exit optimization (makes best case O(n))
        }
    }

    // ========================================================
    // SELECTION SORT — O(n²) all cases, O(1) space
    // Each pass: FIND the minimum element in the unsorted portion,
    //            then PLACE it at the beginning of that portion.
    //
    // Visual: [5, 3, 8, 1, 2] → find min(5,3,8,1,2)=1, swap with pos 0
    //         [1, 3, 8, 5, 2] → find min(3,8,5,2)=2, swap with pos 1
    //         [1, 2, 8, 5, 3] → find min(8,5,3)=3, swap with pos 2
    //         etc.
    //
    // UNSTABLE sort (may change relative order of equal elements)
    // Makes exactly n-1 swaps regardless — useful if swaps are EXPENSIVE
    // ========================================================
    public static void SelectionSort(int[] arr)
    {
        int n = arr.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;  // assume arr[i] is the minimum

            // Find the actual minimum in the unsorted portion [i+1 .. n-1]
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j] < arr[minIndex])
                    minIndex = j;  // found a new minimum
            }

            // Place minimum at position i (only swap if needed)
            if (minIndex != i)
                Swap(arr, i, minIndex);
        }
    }

    // ========================================================
    // INSERTION SORT — O(n²) avg/worst, O(n) best, O(1) space
    // Build the sorted array one element at a time.
    // Pick each element and INSERT it in the correct position in the already-sorted left portion.
    //
    // Visual: [5, 3, 8, 1, 2]
    //         [5|3, 8, 1, 2] → key=3: shift 5 right, insert 3 → [3, 5, 8, 1, 2]
    //         [3, 5|8, 1, 2] → key=8: 8>5, stays → [3, 5, 8, 1, 2]
    //         [3, 5, 8|1, 2] → key=1: shift 8,5,3 right, insert 1 → [1, 3, 5, 8, 2]
    //         etc.
    //
    // STABLE sort. Excellent for nearly-sorted data and small arrays.
    // Used as the base case in hybrid sorts (like Timsort used by Python)
    // ========================================================
    public static void InsertionSort(int[] arr)
    {
        int n = arr.Length;

        for (int i = 1; i < n; i++)
        {
            int key = arr[i];   // the element to be inserted
            int j = i - 1;      // start comparing with the element before it

            // Shift elements that are GREATER than key to the right
            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];  // shift right
                j--;
            }

            // Insert key at its correct position
            arr[j + 1] = key;
        }
    }

    // ========================================================
    // MERGE SORT — O(n log n) all cases, O(n) space
    // Divide and Conquer:
    //   1. DIVIDE: split array in half recursively until single elements
    //   2. CONQUER: merge pairs of sorted sub-arrays into a larger sorted array
    //
    // Visual: [5, 3, 8, 1, 2]
    //   Split: [5,3,8] and [1,2]
    //   Split: [5,3] and [8] and [1] and [2]
    //   Split: [5] and [3] and [8] and [1] and [2]
    //   Merge: [3,5] [8] [1,2]
    //   Merge: [3,5,8] [1,2]
    //   Merge: [1,2,3,5,8] ← final sorted array
    //
    // STABLE sort. Best general-purpose sort if memory is not a concern.
    // Used for external sorting (files too large for RAM).
    // ========================================================
    public static void MergeSort(int[] arr, int left, int right)
    {
        if (left >= right) return;  // base case: 0 or 1 element — already sorted

        int mid = left + (right - left) / 2;

        MergeSort(arr, left, mid);       // recursively sort left half
        MergeSort(arr, mid + 1, right);  // recursively sort right half
        Merge(arr, left, mid, right);    // merge the two sorted halves
    }

    // Merge two adjacent sorted sub-arrays: arr[left..mid] and arr[mid+1..right]
    private static void Merge(int[] arr, int left, int mid, int right)
    {
        // Create temporary arrays for the two halves
        int leftLen = mid - left + 1;
        int rightLen = right - mid;

        int[] leftArr = new int[leftLen];
        int[] rightArr = new int[rightLen];

        // Copy data into temporary arrays
        Array.Copy(arr, left, leftArr, 0, leftLen);
        Array.Copy(arr, mid + 1, rightArr, 0, rightLen);

        // Merge: compare elements from both halves and put smaller one back into arr
        int i = 0, j = 0, k = left;

        while (i < leftLen && j < rightLen)
        {
            // Pick the smaller of the two current elements
            if (leftArr[i] <= rightArr[j])  // <= ensures STABLE sort
                arr[k++] = leftArr[i++];
            else
                arr[k++] = rightArr[j++];
        }

        // Copy any remaining elements (only one side will have leftovers)
        while (i < leftLen)  arr[k++] = leftArr[i++];
        while (j < rightLen) arr[k++] = rightArr[j++];
    }

    // ========================================================
    // QUICK SORT — O(n log n) avg, O(n²) worst, O(log n) space
    // Divide and Conquer with a PIVOT:
    //   1. Choose a pivot element
    //   2. Partition: place all elements < pivot on the LEFT,
    //                 all elements > pivot on the RIGHT
    //   3. Recursively sort left and right sub-arrays
    //
    // The pivot ends up at its FINAL SORTED POSITION after partitioning.
    //
    // Best pivot: median of array (complex to find)
    // Simple pivot: last element, first element, or random
    //
    // UNSTABLE sort. In-place (O(log n) stack space for recursion).
    // Typically fastest in practice due to cache-friendly access patterns.
    // ========================================================
    public static void QuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            // Partition: place pivot in correct position
            int pivotIndex = Partition(arr, low, high);

            // Recursively sort elements before and after pivot
            QuickSort(arr, low, pivotIndex - 1);    // left of pivot
            QuickSort(arr, pivotIndex + 1, high);   // right of pivot
        }
    }

    // Lomuto partition scheme: pivot = arr[high]
    // Rearranges elements so: [elements < pivot | pivot | elements > pivot]
    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];  // choose last element as pivot
        int i = low - 1;        // i tracks the boundary of "less than pivot" zone

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)  // this element belongs in the left partition
            {
                i++;              // expand "less than" zone
                Swap(arr, i, j);  // move this element into the left partition
            }
        }

        // Place pivot in its correct position (between left and right partitions)
        Swap(arr, i + 1, high);
        return i + 1;  // return pivot's final index
    }

    // ========================================================
    // BUILT-IN C# SORTING
    // Array.Sort() and List.Sort() use an adaptive algorithm called
    // Introsort: Quick Sort + Heap Sort + Insertion Sort hybrid
    //   - Starts as Quick Sort
    //   - Switches to Heap Sort if recursion depth too high (avoids O(n²) worst case)  
    //   - Uses Insertion Sort for small arrays (< 16 elements, cache-friendly)
    //   - Result: O(n log n) guaranteed, fast in practice
    // ========================================================
    public static void BuiltInSortingDemo()
    {
        Console.WriteLine("\n--- Built-in C# Sorting ---");

        // Array.Sort — in-place
        int[] nums = { 5, 3, 8, 1, 2, 9, 4, 7, 6 };
        Array.Sort(nums);
        Console.WriteLine("  Array.Sort(): " + string.Join(", ", nums));

        // Array.Sort with custom comparer (sort descending)
        Array.Sort(nums, (a, b) => b.CompareTo(a));
        Console.WriteLine("  Descending:   " + string.Join(", ", nums));

        // List<T>.Sort — in-place
        var list = new List<string> { "banana", "apple", "cherry", "date" };
        list.Sort();
        Console.WriteLine("  List.Sort():  " + string.Join(", ", list));

        // LINQ OrderBy — returns new sorted sequence (original unchanged)
        var ordered = list.OrderByDescending(s => s.Length).ThenBy(s => s);
        Console.WriteLine("  LINQ OrderByDescending(length): " + string.Join(", ", ordered));

        // Sort objects by property
        var people = new List<(string Name, int Age)>
        {
            ("Charlie", 30), ("Alice", 25), ("Bob", 25), ("Dave", 35)
        };

        // Sort by Age, then by Name for ties
        people.Sort((a, b) => {
            int ageComp = a.Age.CompareTo(b.Age);
            return ageComp != 0 ? ageComp : a.Name.CompareTo(b.Name);
        });

        Console.WriteLine("  People sorted (age, then name):");
        foreach (var p in people)
            Console.WriteLine($"    {p.Name}, {p.Age}");
    }

    public static void RunAllSorts()
    {
        Console.WriteLine("\n--- All Sorting Algorithms ---");

        int[] original = { 64, 34, 25, 12, 22, 11, 90 };
        Console.WriteLine($"  Original: [{string.Join(", ", original)}]");

        int[] arr;

        arr = (int[])original.Clone();
        BubbleSort(arr);
        Console.WriteLine($"  Bubble Sort:    [{string.Join(", ", arr)}]");

        arr = (int[])original.Clone();
        SelectionSort(arr);
        Console.WriteLine($"  Selection Sort: [{string.Join(", ", arr)}]");

        arr = (int[])original.Clone();
        InsertionSort(arr);
        Console.WriteLine($"  Insertion Sort: [{string.Join(", ", arr)}]");

        arr = (int[])original.Clone();
        MergeSort(arr, 0, arr.Length - 1);
        Console.WriteLine($"  Merge Sort:     [{string.Join(", ", arr)}]");

        arr = (int[])original.Clone();
        QuickSort(arr, 0, arr.Length - 1);
        Console.WriteLine($"  Quick Sort:     [{string.Join(", ", arr)}]");

        arr = (int[])original.Clone();
        Array.Sort(arr);
        Console.WriteLine($"  Array.Sort():   [{string.Join(", ", arr)}]");
    }
}

// ============================================================
// PERFORMANCE BENCHMARK — shows relative speeds
// ============================================================
class PerformanceBenchmark
{
    public static void Run()
    {
        Console.WriteLine("\n--- Performance Benchmark (10,000 elements) ---");

        // Generate the same random array for each algorithm
        Random rand = new Random(42);
        int n = 10_000;
        int[] source = new int[n];
        for (int i = 0; i < n; i++) source[i] = rand.Next(100_000);

        BenchmarkSort("Bubble Sort",    source, arr => SortingAlgorithms.BubbleSort(arr));
        BenchmarkSort("Selection Sort", source, arr => SortingAlgorithms.SelectionSort(arr));
        BenchmarkSort("Insertion Sort", source, arr => SortingAlgorithms.InsertionSort(arr));
        BenchmarkSort("Merge Sort",     source, arr => SortingAlgorithms.MergeSort(arr, 0, arr.Length - 1));
        BenchmarkSort("Quick Sort",     source, arr => SortingAlgorithms.QuickSort(arr, 0, arr.Length - 1));
        BenchmarkSort("Array.Sort()",   source, arr => Array.Sort(arr));
    }

    private static void BenchmarkSort(string name, int[] source, Action<int[]> sortFn)
    {
        int[] arr = (int[])source.Clone();  // fresh copy each time
        var sw = Stopwatch.StartNew();
        sortFn(arr);
        sw.Stop();
        Console.WriteLine($"  {name,-16}: {sw.ElapsedMilliseconds,4}ms");
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class SortingSearchingProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 12: SORTING & SEARCHING");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Searching
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Searching Algorithms ===");

        int[] unsorted = { 64, 25, 12, 22, 11 };
        int[] sorted   = { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };

        Console.WriteLine($"  Unsorted array: [{string.Join(", ", unsorted)}]");
        Console.WriteLine($"  Sorted array:   [{string.Join(", ", sorted)}]");

        // Linear search works on any array
        int target1 = 22;
        int linearResult = SearchingAlgorithms.LinearSearch(unsorted, target1);
        Console.WriteLine($"\n  Linear Search for {target1}: index {linearResult}");  // index 3

        int target2 = 99;
        int notFound = SearchingAlgorithms.LinearSearch(unsorted, target2);
        Console.WriteLine($"  Linear Search for {target2}: {(notFound == -1 ? "NOT FOUND" : notFound.ToString())}");

        // Binary search REQUIRES sorted array
        int target3 = 11;
        int binaryResult = SearchingAlgorithms.BinarySearch(sorted, target3);
        Console.WriteLine($"\n  Binary Search for {target3}: index {binaryResult}");  // index 5

        // Binary Search with C# built-in
        int builtInResult = Array.BinarySearch(sorted, target3);
        Console.WriteLine($"  Array.BinarySearch for {target3}: index {builtInResult}");

        // Find first occurrence in array with duplicates
        int[] withDups = { 1, 2, 2, 2, 3, 4, 4, 5 };
        int firstOccurrence = SearchingAlgorithms.BinarySearchFirst(withDups, 2);
        Console.WriteLine($"\n  First occurrence of 2 in [{string.Join(", ", withDups)}]: index {firstOccurrence}");  // 1

        // -------------------------------------------------------
        // DEMO 2: All Sorting Algorithms
        // -------------------------------------------------------
        SortingAlgorithms.RunAllSorts();

        // -------------------------------------------------------
        // DEMO 3: Built-in Sorting
        // -------------------------------------------------------
        SortingAlgorithms.BuiltInSortingDemo();

        // -------------------------------------------------------
        // DEMO 4: Performance (note: O(n²) vs O(n log n) clearly visible)
        // -------------------------------------------------------
        PerformanceBenchmark.Run();

        // -------------------------------------------------------
        // TIME COMPLEXITY COMPARISON
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("TIME COMPLEXITY COMPARISON:");
        Console.WriteLine();
        Console.WriteLine("  Algorithm     | Best     | Average  | Worst    | Space  | Stable?");
        Console.WriteLine("  --------------|----------|----------|----------|--------|--------");
        Console.WriteLine("  Bubble Sort   | O(n)     | O(n²)    | O(n²)    | O(1)   | Yes");
        Console.WriteLine("  Selection Sort| O(n²)    | O(n²)    | O(n²)    | O(1)   | No");
        Console.WriteLine("  Insertion Sort| O(n)     | O(n²)    | O(n²)    | O(1)   | Yes");
        Console.WriteLine("  Merge Sort    | O(n logn)| O(n logn)| O(n logn)| O(n)   | Yes");
        Console.WriteLine("  Quick Sort    | O(n logn)| O(n logn)| O(n²)    | O(logn)| No");
        Console.WriteLine("  Array.Sort()  | O(n logn)| O(n logn)| O(n logn)| O(logn)| No*");
        Console.WriteLine();
        Console.WriteLine("  Searching:");
        Console.WriteLine("  Linear Search | O(1)     | O(n)     | O(n)     | O(1)   | -");
        Console.WriteLine("  Binary Search | O(1)     | O(log n) | O(log n) | O(1)   | -");
        Console.WriteLine("  (SORTED array required for binary search)");
        Console.WriteLine("======================================================");
    }
}
