// ============================================================
// PHASE 22: ALGORITHMIC COMPLEXITY ANALYSIS
// Topics: Big-O, Big-Omega, Big-Theta, Space Complexity,
//         Best/Worst/Average cases, Amortized analysis,
//         Practical examples demonstrating each complexity class
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

// ============================================================
// SECTION 1: WHAT IS ALGORITHMIC COMPLEXITY?
//
// Complexity analysis describes how an algorithm's resource usage
// (time or memory) GROWS relative to input size (n).
//
// We use ASYMPTOTIC notation — describes behavior as n → ∞:
//
//   Big-O (O): UPPER BOUND — worst case guarantee
//     "The algorithm takes AT MOST f(n) steps"
//
//   Big-Omega (Ω): LOWER BOUND — best case guarantee
//     "The algorithm takes AT LEAST f(n) steps"
//
//   Big-Theta (Θ): TIGHT BOUND — both upper and lower
//     "The algorithm grows EXACTLY like f(n)"
//
// We DROP constants and lower-order terms:
//   3n² + 2n + 100 → O(n²)  (3 and 2n and 100 don't matter at large n)
// ============================================================

// ============================================================
// SECTION 2: COMPLEXITY CLASSES — from best to worst
// ============================================================

class ComplexityExamples
{
    // -----------------------------------------------------------
    // O(1) — CONSTANT TIME
    // Performance does NOT depend on input size.
    // Array index access, hash table lookup, push/pop stack.
    // -----------------------------------------------------------
    public static int GetFirst(int[] arr)
    {
        return arr[0];  // always 1 operation, regardless of arr size
    }

    public static bool IsEvenBit(int n)
    {
        return (n & 1) == 0;  // always 1 bitwise operation
    }

    // -----------------------------------------------------------
    // O(log n) — LOGARITHMIC TIME
    // Input is HALVED at each step. Base of log doesn't matter asymptotically.
    // Binary search, balanced BST operations.
    // log₂(1,000,000) ≈ 20 → only ~20 steps for 1 million elements!
    // -----------------------------------------------------------
    public static int BinarySearch(int[] sortedArr, int target)
    {
        int low = 0, high = sortedArr.Length - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;  // prevent overflow vs (low+high)/2

            if (sortedArr[mid] == target) return mid;
            else if (sortedArr[mid] < target) low = mid + 1;   // eliminate left half
            else                              high = mid - 1;  // eliminate right half
        }

        return -1;
    }

    // -----------------------------------------------------------
    // O(n) — LINEAR TIME
    // Process each element once.
    // Array traversal, linear search, counting elements.
    // -----------------------------------------------------------
    public static int LinearSearch(int[] arr, int target)
    {
        for (int i = 0; i < arr.Length; i++)  // n iterations
        {
            if (arr[i] == target) return i;
        }
        return -1;
    }

    public static long SumArray(int[] arr)
    {
        long sum = 0;
        foreach (int x in arr) sum += x;  // exactly n additions
        return sum;
    }

    // -----------------------------------------------------------
    // O(n log n) — LINEARITHMIC TIME
    // Most efficient sorting algorithms: MergeSort, HeapSort, QuickSort (avg)
    // For n = 1,000,000: about 20,000,000 operations
    // -----------------------------------------------------------
    public static void MergeSort(int[] arr, int left, int right)
    {
        if (left >= right) return;

        int mid = (left + right) / 2;
        MergeSort(arr, left, mid);       // T(n/2)
        MergeSort(arr, mid + 1, right);  // T(n/2)
        Merge(arr, left, mid, right);    // O(n)
        // Total: 2T(n/2) + O(n) → O(n log n) by Master Theorem
    }

    private static void Merge(int[] arr, int left, int mid, int right)
    {
        int[] temp = new int[right - left + 1];
        int i = left, j = mid + 1, k = 0;

        while (i <= mid && j <= right)
            temp[k++] = arr[i] <= arr[j] ? arr[i++] : arr[j++];

        while (i <= mid)   temp[k++] = arr[i++];
        while (j <= right) temp[k++] = arr[j++];
        Array.Copy(temp, 0, arr, left, temp.Length);
    }

    // -----------------------------------------------------------
    // O(n²) — QUADRATIC TIME
    // Nested loops over the same data.
    // BubbleSort, SelectionSort, comparing all pairs.
    // For n = 10,000: 100,000,000 operations — becomes slow!
    // -----------------------------------------------------------
    public static bool HasDuplicateNaive(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)         // outer loop: n
            for (int j = i + 1; j < arr.Length; j++) // inner loop: n
                if (arr[i] == arr[j]) return true;   // O(n²) comparisons

        return false;
    }

    // O(n) solution using HashSet — MUCH better
    public static bool HasDuplicateFast(int[] arr)
    {
        var seen = new HashSet<int>();
        foreach (int x in arr)
        {
            if (!seen.Add(x)) return true;  // Add returns false if already exists
        }
        return false;
    }

    // -----------------------------------------------------------
    // O(2^n) — EXPONENTIAL TIME
    // Generates all subsets / tries all combinations.
    // Naive Fibonacci, Traveling Salesman brute force.
    // n = 40 → 2^40 ≈ 1 trillion operations — completely impractical!
    // -----------------------------------------------------------
    public static long FibNaive(int n)
    {
        if (n <= 1) return n;
        return FibNaive(n - 1) + FibNaive(n - 2);  // branches into 2 at every step
    }

    // -----------------------------------------------------------
    // O(n!) — FACTORIAL TIME
    // Brute force permutations.
    // n = 20 → 2,432,902,008,176,640,000 operations
    // Only feasible for n ≤ ~10
    // -----------------------------------------------------------
    public static List<List<int>> GetAllPermutations(List<int> nums)
    {
        var results = new List<List<int>>();

        void Backtrack(int start)
        {
            if (start == nums.Count) { results.Add(new List<int>(nums)); return; }
            for (int i = start; i < nums.Count; i++)
            {
                (nums[start], nums[i]) = (nums[i], nums[start]);  // swap
                Backtrack(start + 1);
                (nums[start], nums[i]) = (nums[i], nums[start]);  // swap back
            }
        }

        Backtrack(0);
        return results;
    }
}

// ============================================================
// SECTION 3: SPACE COMPLEXITY
//
// Measures extra memory used relative to input size.
// INPUT itself is not counted in auxiliary space complexity.
//
//   O(1): fixed extra variables (most in-place algorithms)
//   O(n): store all elements (copy, recursive call stack)
//   O(n²): 2D matrix/table
//   O(log n): recursive divide-and-conquer (call stack depth)
// ============================================================

class SpaceComplexityExamples
{
    // O(1) space — only a few variables regardless of n
    public static int Sum(int[] arr)
    {
        int total = 0;          // O(1): just one variable
        foreach (int x in arr) total += x;
        return total;
    }

    // O(n) space — recursive call stack = n levels deep
    public static int SumRecursive(int[] arr, int idx = 0)
    {
        if (idx == arr.Length) return 0;
        return arr[idx] + SumRecursive(arr, idx + 1);  // n recursive calls
    }

    // O(log n) space — binary search recursion = log n levels deep
    public static int BinarySearchRecursive(int[] arr, int target, int low, int high)
    {
        if (low > high) return -1;
        int mid = (low + high) / 2;
        if (arr[mid] == target) return mid;
        if (arr[mid] < target) return BinarySearchRecursive(arr, target, mid + 1, high);
        return BinarySearchRecursive(arr, target, low, mid - 1);
    }

    // O(n²) space — 2D DP table for LCS / edit distance
    public static int[,] BuildLCSTable(string a, string b)
    {
        var dp = new int[a.Length + 1, b.Length + 1];  // (m+1)*(n+1) space
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                dp[i, j] = a[i-1] == b[j-1] ? dp[i-1, j-1] + 1 : Math.Max(dp[i-1, j], dp[i, j-1]);
        return dp;
    }
}

// ============================================================
// SECTION 4: AMORTIZED ANALYSIS
//
// Some operations are occasionally O(n) but AMORTIZED O(1).
// Example: List<T>.Add() — usually O(1), but occasionally O(n)
// when the internal array needs to double in size.
// Over n adds, the total work is O(n), so per-add = O(1) amortized.
//
// The doubling strategy:
//   Capacity: 1 → 2 → 4 → 8 → 16 → ... (doubles each time)
//   Copy cost: 1 + 2 + 4 + 8 = 2^k - 1 = O(n) total
//   Amortized per add: O(n) / n = O(1)
// ============================================================

class AmortizedDemo
{
    public static void Demonstrate()
    {
        Console.WriteLine("\n=== Amortized Analysis: List<T>.Add() ===");

        var list = new List<int>();
        var sw = Stopwatch.StartNew();

        int lastCapacity = 0;
        for (int i = 0; i < 100; i++)
        {
            list.Add(i);
            if (list.Capacity != lastCapacity)
            {
                Console.WriteLine($"    Count={list.Count:D3}, Capacity doubled to: {list.Capacity}");
                lastCapacity = list.Capacity;
            }
        }

        sw.Stop();
        Console.WriteLine($"  100 adds completed. Amortized O(1) per add.");
        Console.WriteLine("  Occasional O(n) copies are rare — total cost = O(n)");
    }
}

// ============================================================
// MAIN PROGRAM — includes a simple benchmarking comparison
// ============================================================
class AlgorithmicAnalysisProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 22: ALGORITHMIC COMPLEXITY ANALYSIS");
        Console.WriteLine("======================================================");

        // Show complexity hierarchy
        Console.WriteLine("\n=== Complexity Hierarchy (n = 1000) ===");
        int n = 1000;
        Console.WriteLine($"  O(1):      {1}         (constant — best!)");
        Console.WriteLine($"  O(log n):  {(int)Math.Log2(n)} (log₂ 1000 ≈ 10)");
        Console.WriteLine($"  O(n):      {n}       (linear)");
        Console.WriteLine($"  O(n log n):{n * (int)Math.Log2(n):N0} (linearithmic)");
        Console.WriteLine($"  O(n²):     {(long)n * n:N0}   (quadratic)");
        Console.WriteLine($"  O(2^n):    way too big!   (exponential — avoid for large n)");
        Console.WriteLine($"  O(n!):     astronomically large! (factorial — avoid)");

        // Binary search demonstration
        Console.WriteLine("\n=== Binary Search O(log n) — In Action ===");
        int[] sorted = new int[1000000];
        for (int i = 0; i < sorted.Length; i++) sorted[i] = i * 2;
        int target = 999998;
        int steps = 0;
        int lo = 0, hi = sorted.Length - 1;
        while (lo <= hi) { int mid = (lo + hi) / 2; steps++; if (sorted[mid] == target) break; else if (sorted[mid] < target) lo = mid + 1; else hi = mid - 1; }
        Console.WriteLine($"  Searching in 1,000,000 elements took only {steps} steps (log₂(1M) ≈ 20)");

        // Duplicate check comparison
        Console.WriteLine("\n=== O(n²) vs O(n): Duplicate Check ===");
        int[] data = Enumerable.Range(1, 5000).ToArray();

        var sw = Stopwatch.StartNew();
        bool r1 = ComplexityExamples.HasDuplicateNaive(data);
        sw.Stop(); long naiveMs = sw.ElapsedMilliseconds;

        sw.Restart();
        bool r2 = ComplexityExamples.HasDuplicateFast(data);
        sw.Stop(); long fastMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  O(n²) naive: {naiveMs}ms");
        Console.WriteLine($"  O(n) HashSet: {fastMs}ms  (HashSet wins!)");

        AmortizedDemo.Demonstrate();

        Console.WriteLine("\n=== Choosing the Right Algorithm ===");
        Console.WriteLine("  If n ≤ 10:         Any algorithm works, even O(n!)");
        Console.WriteLine("  If n ≤ 100:        O(n²) is fine");
        Console.WriteLine("  If n ≤ 1,000:      O(n²) ok, O(n log n) better");
        Console.WriteLine("  If n ≤ 100,000:    O(n log n) required");
        Console.WriteLine("  If n ≤ 10,000,000: O(n) or O(n log n)");
        Console.WriteLine("  If n = very large: O(log n) or O(1) required");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Big-O = worst case upper bound (used most often)");
        Console.WriteLine("  2. Drop constants and lower-order terms");
        Console.WriteLine("  3. Nested loops = multiply: O(n) × O(n) = O(n²)");
        Console.WriteLine("  4. Consecutive operations = add (but O(n²)+O(n) = O(n²))");
        Console.WriteLine("  5. Recursion time = levels × work per level");
        Console.WriteLine("  6. Amortized O(1): occasional expensive ops spread over many");
        Console.WriteLine("  7. Always consider BOTH time AND space complexity");
        Console.WriteLine("======================================================");
    }
}
