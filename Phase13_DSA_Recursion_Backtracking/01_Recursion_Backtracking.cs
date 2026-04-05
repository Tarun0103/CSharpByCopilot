// ============================================================
// PHASE 13: RECURSION & BACKTRACKING — Complete C# Implementation
// Topics: Recursion basics, call stack, memoization,
//         Backtracking: subsets, permutations, N-Queens
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;

// ============================================================
// SECTION 1: RECURSION FUNDAMENTALS
//
// Recursion = a function that calls ITSELF.
// Every recursive function MUST have:
//   1. BASE CASE — stops the recursion (prevents infinite loop)
//   2. RECURSIVE CASE — calls itself with a SMALLER/SIMPLER input
//
// Think of it as: solve a big problem by solving a smaller version
// of the same problem, until you hit a problem so small it's trivial.
// ============================================================

class RecursionBasics
{
    // ========================================================
    // FACTORIAL — n! = n * (n-1) * (n-2) * ... * 1
    // Recursive definition: factorial(n) = n * factorial(n-1)
    // Base case: factorial(0) = 1
    //
    // Call stack for factorial(4):
    //   factorial(4) → 4 * factorial(3)
    //     factorial(3) → 3 * factorial(2)
    //       factorial(2) → 2 * factorial(1)
    //         factorial(1) → 1 * factorial(0)
    //           factorial(0) → 1          ← BASE CASE
    //         factorial(1) = 1*1 = 1      ← unwind begins
    //       factorial(2) = 2*1 = 2
    //     factorial(3) = 3*2 = 6
    //   factorial(4) = 4*6 = 24           ← final answer
    // ========================================================
    public static long Factorial(int n)
    {
        // Base case: stop the recursion
        if (n <= 1) return 1;

        // Recursive case: n! = n * (n-1)!
        return n * Factorial(n - 1);
    }

    // ========================================================
    // FIBONACCI SEQUENCE
    // F(0)=0, F(1)=1, F(2)=1, F(3)=2, F(4)=3, F(5)=5, ...
    //
    // NAIVE RECURSIVE — O(2^n) time, O(n) space
    // Very slow for n > 35 because it recalculates subproblems MANY times
    // fib(5) calls fib(4) and fib(3)
    // fib(4) calls fib(3) and fib(2)  ← fib(3) calculated AGAIN!
    // ========================================================
    public static long FibNaive(int n)
    {
        if (n <= 1) return n;  // base cases: fib(0)=0, fib(1)=1
        return FibNaive(n - 1) + FibNaive(n - 2);  // each call spawns 2 more!
    }

    // ========================================================
    // FIBONACCI WITH MEMOIZATION — O(n) time, O(n) space
    // "Remember" previously computed results in a dictionary.
    // First call for each n = compute and store.
    // Subsequent calls = return stored result immediately.
    // This is the TOP-DOWN Dynamic Programming approach.
    // ========================================================
    private static Dictionary<int, long> _memo = new Dictionary<int, long>();

    public static long FibMemoized(int n)
    {
        if (n <= 1) return n;

        // Check cache first — avoid recomputation
        if (_memo.ContainsKey(n))
            return _memo[n];  // already computed, return immediately

        // Compute and STORE
        long result = FibMemoized(n - 1) + FibMemoized(n - 2);
        _memo[n] = result;
        return result;
    }

    // ========================================================
    // POWER FUNCTION — x^n
    // Recursive: x^n = x * x^(n-1)
    // Optimized: x^n = (x^(n/2))^2 when n is even → O(log n)!
    // ========================================================
    public static double Power(double x, int n)
    {
        if (n == 0) return 1;    // x^0 = 1 for any x
        if (n < 0)  return 1.0 / Power(x, -n);  // x^(-n) = 1/x^n

        if (n % 2 == 0)
        {
            // x^n = (x²)^(n/2) — halves the problem size!
            double half = Power(x, n / 2);
            return half * half;
        }
        else
        {
            // x^n = x * x^(n-1)
            return x * Power(x, n - 1);
        }
    }

    // ========================================================
    // SUM OF DIGITS — recursive
    // sumDigits(123) = 3 + sumDigits(12) = 3 + 2 + sumDigits(1) = 3 + 2 + 1 = 6
    // ========================================================
    public static int SumOfDigits(int n)
    {
        n = Math.Abs(n);           // handle negative numbers
        if (n < 10) return n;      // single digit is its own sum
        return (n % 10) + SumOfDigits(n / 10);  // last digit + sum of remaining digits
    }

    // ========================================================
    // BINARY SEARCH — recursive
    // Same logic as iterative, but uses recursion instead of a while loop
    // ========================================================
    public static int BinarySearch(int[] arr, int target, int left, int right)
    {
        if (left > right) return -1;

        int mid = left + (right - left) / 2;

        if (arr[mid] == target) return mid;
        if (arr[mid] < target)  return BinarySearch(arr, target, mid + 1, right);
        else                    return BinarySearch(arr, target, left, mid - 1);
    }
}

// ============================================================
// SECTION 2: BACKTRACKING
//
// Backtracking = build a solution incrementally.
// If a partial solution CANNOT lead to a valid complete solution,
// UNDO the last choice ("backtrack") and try a different path.
//
// Template:
//   void Backtrack(currentState) {
//       if (currentState is a complete solution) {
//           add to results; return;
//       }
//       for each choice in possibleChoices:
//           if choice is valid:
//               make choice         (add to current state)
//               Backtrack(newState) (recurse)
//               undo choice         (remove from current state) ← BACKTRACK
//   }
// ============================================================

class BacktrackingProblems
{
    // ========================================================
    // GENERATE ALL SUBSETS (Power Set)
    // Input: [1, 2, 3]
    // Output: [], [1], [2], [3], [1,2], [1,3], [2,3], [1,2,3]
    //
    // For each element: either INCLUDE it or EXCLUDE it.
    // Total subsets = 2^n (2 choices for each of n elements)
    // ========================================================
    public static List<List<int>> Subsets(int[] nums)
    {
        List<List<int>> results = new List<List<int>>();
        Backtrack_Subsets(nums, 0, new List<int>(), results);
        return results;
    }

    private static void Backtrack_Subsets(int[] nums, int start, List<int> current, List<List<int>> results)
    {
        // Every state is a valid subset (empty subset is also valid)
        // So we add to results at EVERY call, not just at a specific depth
        results.Add(new List<int>(current));  // Add a COPY of current state

        // Try adding each remaining element
        for (int i = start; i < nums.Length; i++)
        {
            current.Add(nums[i]);               // make choice: include nums[i]
            Backtrack_Subsets(nums, i + 1, current, results);  // explore further
            current.RemoveAt(current.Count - 1); // ← BACKTRACK: undo the choice
        }
    }

    // ========================================================
    // GENERATE ALL PERMUTATIONS
    // Input: [1, 2, 3]
    // Output: [1,2,3], [1,3,2], [2,1,3], [2,3,1], [3,1,2], [3,2,1]
    //
    // Total permutations = n! (n choices for pos 0, n-1 for pos 1, etc.)
    // ========================================================
    public static List<List<int>> Permutations(int[] nums)
    {
        List<List<int>> results = new List<List<int>>();
        Backtrack_Perm(nums, new List<int>(), new bool[nums.Length], results);
        return results;
    }

    private static void Backtrack_Perm(int[] nums, List<int> current, bool[] used, List<List<int>> results)
    {
        // Base case: permutation is complete (used all elements)
        if (current.Count == nums.Length)
        {
            results.Add(new List<int>(current));  // add copy
            return;
        }

        for (int i = 0; i < nums.Length; i++)
        {
            if (used[i]) continue;     // skip elements already in current permutation

            used[i] = true;            // mark as used
            current.Add(nums[i]);      // make choice

            Backtrack_Perm(nums, current, used, results);  // recurse

            current.RemoveAt(current.Count - 1);  // ← BACKTRACK
            used[i] = false;           // mark as available again
        }
    }

    // ========================================================
    // N-QUEENS PROBLEM
    // Place N queens on an N×N chessboard such that no two queens
    // attack each other (same row, column, or diagonal).
    //
    // Input: n=4
    // Output: 2 solutions for 4-Queens:
    //   . Q . .      . . Q .
    //   . . . Q      Q . . .
    //   Q . . .      . . . Q
    //   . . Q .      . Q . .
    //
    // Approach: place one queen per row, backtrack if conflict
    // ========================================================
    public static List<List<string>> NQueens(int n)
    {
        List<List<string>> results = new List<List<string>>();
        int[] queenCols = new int[n];  // queenCols[row] = column where queen is placed in that row
        Array.Fill(queenCols, -1);

        Backtrack_NQueens(n, 0, queenCols, results);
        return results;
    }

    private static void Backtrack_NQueens(int n, int row, int[] queenCols, List<List<string>> results)
    {
        // Base case: placed queens in ALL rows → found a valid solution!
        if (row == n)
        {
            results.Add(BuildBoard(n, queenCols));
            return;
        }

        // Try placing queen in each column of the current row
        for (int col = 0; col < n; col++)
        {
            if (IsValidPlacement(queenCols, row, col))
            {
                queenCols[row] = col;      // place queen at (row, col)
                Backtrack_NQueens(n, row + 1, queenCols, results);  // next row
                queenCols[row] = -1;       // ← BACKTRACK: remove queen from this row
            }
        }
    }

    // Check if placing a queen at (row, col) is safe
    private static bool IsValidPlacement(int[] queenCols, int row, int col)
    {
        for (int r = 0; r < row; r++)
        {
            int c = queenCols[r];

            // Same column conflict
            if (c == col) return false;

            // Diagonal conflict: |row1 - row2| == |col1 - col2|
            if (Math.Abs(r - row) == Math.Abs(c - col)) return false;
        }
        return true;
    }

    // Convert array representation to visual board strings
    private static List<string> BuildBoard(int n, int[] queenCols)
    {
        List<string> board = new List<string>();
        for (int r = 0; r < n; r++)
        {
            char[] row = new char[n];
            Array.Fill(row, '.');
            row[queenCols[r]] = 'Q';  // place queen
            board.Add(new string(row));
        }
        return board;
    }

    // ========================================================
    // COMBINATION SUM
    // Given array of candidates and target value, find all unique
    // combinations that sum to target. Each number can be used multiple times.
    //
    // candidates = [2, 3, 6, 7], target = 7
    // Output: [[2,2,3], [7]]
    // ========================================================
    public static List<List<int>> CombinationSum(int[] candidates, int target)
    {
        List<List<int>> results = new List<List<int>>();
        Array.Sort(candidates);  // sort to enable pruning
        Backtrack_CombSum(candidates, target, 0, new List<int>(), results);
        return results;
    }

    private static void Backtrack_CombSum(int[] cands, int remaining, int start, List<int> current, List<List<int>> results)
    {
        if (remaining == 0)
        {
            results.Add(new List<int>(current));  // found a valid combination
            return;
        }

        for (int i = start; i < cands.Length; i++)
        {
            // PRUNING: if candidate > remaining, all larger candidates also too big
            if (cands[i] > remaining) break;

            current.Add(cands[i]);
            Backtrack_CombSum(cands, remaining - cands[i], i, current, results);  // i (not i+1) allows re-use
            current.RemoveAt(current.Count - 1);  // backtrack
        }
    }

    // ========================================================
    // SUDOKU SOLVER
    // Fill a 9×9 grid where each row, column, and 3×3 box
    // contains digits 1-9 exactly once.
    // 0 = empty cell to fill
    // ========================================================
    public static bool SolveSudoku(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)  // empty cell found
                {
                    // Try placing digits 1-9
                    for (int digit = 1; digit <= 9; digit++)
                    {
                        if (IsValidSudoku(board, row, col, digit))
                        {
                            board[row, col] = digit;      // place digit

                            if (SolveSudoku(board))       // recurse — try to solve rest
                                return true;              // solved!

                            board[row, col] = 0;          // ← BACKTRACK: remove digit
                        }
                    }

                    return false;  // no digit worked in this cell → backtrack to previous cell
                }
            }
        }

        return true;  // no empty cells left → solved!
    }

    private static bool IsValidSudoku(int[,] board, int row, int col, int digit)
    {
        // Check row
        for (int c = 0; c < 9; c++)
            if (board[row, c] == digit) return false;

        // Check column
        for (int r = 0; r < 9; r++)
            if (board[r, col] == digit) return false;

        // Check 3x3 box
        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;

        for (int r = boxRow; r < boxRow + 3; r++)
            for (int c = boxCol; c < boxCol + 3; c++)
                if (board[r, c] == digit) return false;

        return true;
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class RecursionProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 13: RECURSION & BACKTRACKING");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Recursion Basics
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Recursion Fundamentals ===");

        Console.WriteLine("  Factorials:");
        for (int i = 0; i <= 10; i++)
            Console.WriteLine($"    {i}! = {RecursionBasics.Factorial(i)}");

        Console.WriteLine("\n  Fibonacci (naive vs memoized):");
        Console.WriteLine("  [Naive is very slow for large n — avoid for n > 35]");
        for (int i = 0; i <= 15; i++)
            Console.WriteLine($"    fib({i,2}) = {RecursionBasics.FibMemoized(i)}");

        Console.WriteLine("\n  Power function (fast exponentiation O(log n)):");
        Console.WriteLine($"    2^10 = {RecursionBasics.Power(2, 10)}");
        Console.WriteLine($"    3^5  = {RecursionBasics.Power(3, 5)}");
        Console.WriteLine($"    2^-3 = {RecursionBasics.Power(2, -3)}");

        Console.WriteLine("\n  Sum of digits:");
        Console.WriteLine($"    SumDigits(123) = {RecursionBasics.SumOfDigits(123)}");   // 6
        Console.WriteLine($"    SumDigits(9999) = {RecursionBasics.SumOfDigits(9999)}"); // 36

        // -------------------------------------------------------
        // DEMO 2: Subsets (Power Set)
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: All Subsets of [1,2,3] ===");
        var subsets = BacktrackingProblems.Subsets(new[] { 1, 2, 3 });
        Console.WriteLine($"  Total subsets: {subsets.Count} (= 2^3 = 8)");
        foreach (var subset in subsets)
            Console.WriteLine($"  [{string.Join(", ", subset)}]");

        // -------------------------------------------------------
        // DEMO 3: Permutations
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: All Permutations of [1,2,3] ===");
        var perms = BacktrackingProblems.Permutations(new[] { 1, 2, 3 });
        Console.WriteLine($"  Total permutations: {perms.Count} (= 3! = 6)");
        foreach (var perm in perms)
            Console.WriteLine($"  [{string.Join(", ", perm)}]");

        // -------------------------------------------------------
        // DEMO 4: N-Queens
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: N-Queens (n=4) ===");
        var solutions = BacktrackingProblems.NQueens(4);
        Console.WriteLine($"  Total solutions: {solutions.Count}");

        for (int s = 0; s < solutions.Count; s++)
        {
            Console.WriteLine($"\n  Solution {s + 1}:");
            foreach (string row in solutions[s])
                Console.WriteLine($"    {row}");
        }

        Console.WriteLine($"\n  8-Queens has {BacktrackingProblems.NQueens(8).Count} solutions");  // 92

        // -------------------------------------------------------
        // DEMO 5: Combination Sum
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: Combination Sum (target=7, candidates=[2,3,6,7]) ===");
        var combos = BacktrackingProblems.CombinationSum(new[] { 2, 3, 6, 7 }, 7);
        foreach (var combo in combos)
            Console.WriteLine($"  [{string.Join("+", combo)}]={combo.Sum()}");
        // [2+2+3]=7 and [7]=7

        // -------------------------------------------------------
        // DEMO 6: Sudoku Solver
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 6: Sudoku Solver ===");

        int[,] sudoku = {
            {5,3,0, 0,7,0, 0,0,0},
            {6,0,0, 1,9,5, 0,0,0},
            {0,9,8, 0,0,0, 0,6,0},

            {8,0,0, 0,6,0, 0,0,3},
            {4,0,0, 8,0,3, 0,0,1},
            {7,0,0, 0,2,0, 0,0,6},

            {0,6,0, 0,0,0, 2,8,0},
            {0,0,0, 4,1,9, 0,0,5},
            {0,0,0, 0,8,0, 0,7,9}
        };

        Console.WriteLine("  Solving...");
        bool solved = BacktrackingProblems.SolveSudoku(sudoku);
        Console.WriteLine($"  Solved: {solved}");

        if (solved)
        {
            Console.WriteLine("  Solution:");
            for (int r = 0; r < 9; r++)
            {
                Console.Write("  ");
                for (int c = 0; c < 9; c++)
                    Console.Write(sudoku[r, c] + (c == 2 || c == 5 ? "|" : " "));
                Console.WriteLine();
                if (r == 2 || r == 5) Console.WriteLine("  ------+------+------");
            }
        }

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Recursion needs: base case (stop) + recursive case (smaller input)");
        Console.WriteLine("  2. Every recursive call uses stack frame memory (risk: StackOverflow)");
        Console.WriteLine("  3. Naive recursion can be O(2^n) — add memoization for O(n)");
        Console.WriteLine("  4. Backtracking = incremental build + undo (backtrack) on dead end");
        Console.WriteLine("  5. Backtracking template: choose → explore → unchoose");
        Console.WriteLine("  6. N-Queens, Sudoku, Subsets, Permutations = classic backtracking");
        Console.WriteLine("  7. Pruning = skip invalid branches early → huge speed improvement");
        Console.WriteLine("======================================================");
    }
}
