// ============================================================
// PHASE 17: DYNAMIC PROGRAMMING (DP)
// Topics: Memoization (Top-Down), Tabulation (Bottom-Up),
//         Classic DP problems: Fibonacci, Knapsack 0/1,
//         Longest Common Subsequence, Coin Change,
//         Longest Increasing Subsequence, Edit Distance
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;

// ============================================================
// CORE CONCEPT: WHAT IS DYNAMIC PROGRAMMING?
//
// DP = Recursion + Memoization  OR  Iteration + Tabulation
//
// Two key properties required:
// 1. OPTIMAL SUBSTRUCTURE: optimal solution built from optimal sub-solutions
// 2. OVERLAPPING SUBPROBLEMS: same sub-problems solved repeatedly
//
// Two main approaches:
//   TOP-DOWN (Memoization): recursive, cache results in a dictionary/array
//   BOTTOM-UP (Tabulation): iterative, fill a table from base cases up
// ============================================================

// ============================================================
// SECTION 1: FIBONACCI — THE DP GATEWAY PROBLEM
// fib(n) = fib(n-1) + fib(n-2),  fib(0)=0, fib(1)=1
// ============================================================

class FibonacciDP
{
    // NAIVE RECURSION: O(2^n) time — exponential!
    // Each call branches into 2 more calls. fib(5) calls fib(3) and fib(4)
    // fib(3) is computed TWICE. For fib(40), it's computed ~100 million times.
    public static long Naive(int n)
    {
        if (n <= 1) return n;
        return Naive(n - 1) + Naive(n - 2);  // overlapping subproblems!
    }

    // TOP-DOWN (MEMOIZATION): O(n) time, O(n) space
    // Same recursion but CACHE results. If already computed, return cached value.
    // "Memo" = memory for storing previously computed results.
    private static Dictionary<int, long> _memo = new Dictionary<int, long>();

    public static long Memoized(int n)
    {
        if (n <= 1) return n;

        // Check cache first — if already computed, return instantly
        if (_memo.ContainsKey(n)) return _memo[n];

        // Compute and STORE result before returning
        _memo[n] = Memoized(n - 1) + Memoized(n - 2);
        return _memo[n];
    }

    // BOTTOM-UP (TABULATION): O(n) time, O(n) space
    // Build the table from base cases up. No recursion needed.
    // Most interviews prefer this approach — no call stack overhead.
    public static long Tabulation(int n)
    {
        if (n <= 1) return n;

        long[] dp = new long[n + 1];  // dp[i] = fib(i)
        dp[0] = 0;
        dp[1] = 1;

        // Fill from bottom up
        for (int i = 2; i <= n; i++)
            dp[i] = dp[i - 1] + dp[i - 2];

        return dp[n];
    }

    // SPACE-OPTIMIZED: O(n) time, O(1) space
    // Notice we only ever need the LAST TWO values → save just those
    public static long SpaceOptimized(int n)
    {
        if (n <= 1) return n;

        long prev2 = 0;  // fib(n-2)
        long prev1 = 1;  // fib(n-1)

        for (int i = 2; i <= n; i++)
        {
            long current = prev1 + prev2;
            prev2 = prev1;
            prev1 = current;
        }

        return prev1;
    }
}

// ============================================================
// SECTION 2: 0/1 KNAPSACK PROBLEM
//
// PROBLEM: Given items with weights and values, and a knapsack
// with capacity W, pick items to MAXIMIZE total value.
// Each item can be picked AT MOST ONCE (0/1).
//
// dp[i][w] = max value using first i items with capacity w
//
// Recurrence:
//   if weight[i] > w:  dp[i][w] = dp[i-1][w]   (can't include item i)
//   else:              dp[i][w] = max(dp[i-1][w],  ← skip item i
//                                    dp[i-1][w-weight[i]] + value[i])  ← include item i
// ============================================================

class KnapsackProblem
{
    // Bottom-Up DP: O(n * W) time, O(n * W) space
    public static int Knapsack01(int[] weights, int[] values, int capacity)
    {
        int n = weights.Length;
        // dp[i][w] = max value from first i items with capacity w
        int[,] dp = new int[n + 1, capacity + 1];

        for (int i = 1; i <= n; i++)
        {
            for (int w = 0; w <= capacity; w++)
            {
                // Option 1: Skip item i (same as without item i)
                dp[i, w] = dp[i - 1, w];

                // Option 2: Take item i (only if it fits)
                int itemWeight = weights[i - 1];
                int itemValue  = values[i - 1];

                if (itemWeight <= w)
                {
                    int valueIfTaken = dp[i - 1, w - itemWeight] + itemValue;
                    dp[i, w] = Math.Max(dp[i, w], valueIfTaken);
                }
            }
        }

        return dp[n, capacity];
    }

    // SPACE-OPTIMIZED: O(W) space — only need previous row
    // Process capacity in REVERSE to avoid using updated values from current row
    public static int Knapsack01Optimized(int[] weights, int[] values, int capacity)
    {
        int[] dp = new int[capacity + 1];  // dp[w] = max value with capacity w

        for (int i = 0; i < weights.Length; i++)
        {
            // Traverse capacity in REVERSE to avoid counting item twice
            for (int w = capacity; w >= weights[i]; w--)
            {
                dp[w] = Math.Max(dp[w], dp[w - weights[i]] + values[i]);
            }
        }

        return dp[capacity];
    }
}

// ============================================================
// SECTION 3: COIN CHANGE
//
// PROBLEM: Given coin denominations and a target amount,
// find the MINIMUM NUMBER of coins to make the amount.
// Infinite supply of each coin.
//
// dp[i] = min coins needed to make amount i
//
// Recurrence:
//   dp[0] = 0
//   dp[i] = min(dp[i - coin] + 1) for each coin ≤ i
// ============================================================

class CoinChange
{
    // Returns minimum number of coins, or -1 if impossible
    public static int MinCoins(int[] coins, int amount)
    {
        int[] dp = new int[amount + 1];
        Array.Fill(dp, amount + 1);  // fill with "infinity" (impossible value)
        dp[0] = 0;                   // base case: 0 coins needed for amount 0

        for (int i = 1; i <= amount; i++)
        {
            foreach (int coin in coins)
            {
                if (coin <= i)
                {
                    // Try using this coin: 1 coin + min coins for (i - coin)
                    dp[i] = Math.Min(dp[i], dp[i - coin] + 1);
                }
            }
        }

        return dp[amount] > amount ? -1 : dp[amount];
    }

    // COIN CHANGE II: count the NUMBER OF WAYS to make the amount
    // This is the UNBOUNDED KNAPSACK variant
    public static int CountWays(int[] coins, int amount)
    {
        int[] dp = new int[amount + 1];
        dp[0] = 1;  // 1 way to make 0: use no coins

        // Process each coin type (outer loop = coin ensures no duplicate counting)
        foreach (int coin in coins)
        {
            for (int i = coin; i <= amount; i++)
            {
                dp[i] += dp[i - coin];  // ways using this coin + ways without
            }
        }

        return dp[amount];
    }
}

// ============================================================
// SECTION 4: LONGEST COMMON SUBSEQUENCE (LCS)
//
// PROBLEM: Find the longest subsequence present in BOTH strings.
// A subsequence preserves relative order but not necessarily contiguous.
//
// "ABCBDAB" and "BDCABA" → LCS = "BCBA" or "BDAB" (length 4)
//
// dp[i][j] = LCS length of text1[0..i-1] and text2[0..j-1]
//
// Recurrence:
//   if text1[i-1] == text2[j-1]:  dp[i][j] = dp[i-1][j-1] + 1
//   else:                          dp[i][j] = max(dp[i-1][j], dp[i][j-1])
// ============================================================

class LongestCommonSubsequence
{
    public static int LCS(string text1, string text2)
    {
        int m = text1.Length;
        int n = text2.Length;

        // dp[i][j] = LCS of first i chars of text1 and first j chars of text2
        int[,] dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (text1[i - 1] == text2[j - 1])
                {
                    // Characters match: extend LCS by 1
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    // Characters don't match: take best of skipping either char
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        return dp[m, n];
    }

    // Get the actual LCS string (backtrack through DP table)
    public static string GetLCSString(string text1, string text2)
    {
        int m = text1.Length, n = text2.Length;
        int[,] dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
                dp[i, j] = text1[i - 1] == text2[j - 1]
                    ? dp[i - 1, j - 1] + 1
                    : Math.Max(dp[i - 1, j], dp[i, j - 1]);

        // BACKTRACK to reconstruct the LCS
        var result = new System.Text.StringBuilder();
        int r = m, c = n;
        while (r > 0 && c > 0)
        {
            if (text1[r - 1] == text2[c - 1])
            {
                result.Insert(0, text1[r - 1]);  // this character is in LCS
                r--; c--;
            }
            else if (dp[r - 1, c] > dp[r, c - 1])
                r--;
            else
                c--;
        }

        return result.ToString();
    }
}

// ============================================================
// SECTION 5: LONGEST INCREASING SUBSEQUENCE (LIS)
//
// PROBLEM: Find the length of the longest strictly increasing subsequence.
// [10, 9, 2, 5, 3, 7, 101, 18] → LIS = [2, 5, 7, 101] → length 4
//
// dp[i] = LIS ending at index i
//
// Recurrence:
//   dp[i] = max(dp[j] + 1) for all j < i where nums[j] < nums[i]
// ============================================================

class LongestIncreasingSubsequence
{
    // O(n²) DP solution
    public static int LIS(int[] nums)
    {
        int n = nums.Length;
        int[] dp = new int[n];
        Array.Fill(dp, 1);  // each element is an LIS of length 1 by itself

        int maxLen = 1;

        for (int i = 1; i < n; i++)
        {
            for (int j = 0; j < i; j++)
            {
                // If nums[j] < nums[i], we can extend the LIS ending at j
                if (nums[j] < nums[i])
                {
                    dp[i] = Math.Max(dp[i], dp[j] + 1);
                }
            }
            maxLen = Math.Max(maxLen, dp[i]);
        }

        return maxLen;
    }
}

// ============================================================
// SECTION 6: EDIT DISTANCE (Levenshtein Distance)
//
// PROBLEM: Minimum number of operations (insert, delete, replace)
// to transform one string into another.
//
// dp[i][j] = min operations to convert word1[0..i-1] to word2[0..j-1]
//
// Recurrence:
//   if word1[i-1] == word2[j-1]:  dp[i][j] = dp[i-1][j-1]  (no operation)
//   else: dp[i][j] = 1 + min(
//           dp[i-1][j],    // delete from word1
//           dp[i][j-1],    // insert into word1
//           dp[i-1][j-1]   // replace in word1
//         )
// ============================================================

class EditDistance
{
    public static int MinDistance(string word1, string word2)
    {
        int m = word1.Length, n = word2.Length;
        int[,] dp = new int[m + 1, n + 1];

        // Base cases: converting to/from empty string
        for (int i = 0; i <= m; i++) dp[i, 0] = i;  // delete all chars
        for (int j = 0; j <= n; j++) dp[0, j] = j;  // insert all chars

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (word1[i - 1] == word2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];  // characters match, no op needed
                }
                else
                {
                    dp[i, j] = 1 + Math.Min(
                        dp[i - 1, j],       // delete
                        Math.Min(
                            dp[i, j - 1],   // insert
                            dp[i - 1, j - 1] // replace
                        )
                    );
                }
            }
        }

        return dp[m, n];
    }
}

// ============================================================
// SECTION 7: EXTRA — HOUSE ROBBER (Classic 1D DP)
//
// PROBLEM: Row of houses. Can't rob adjacent houses.
// Maximize money robbed.
// [2, 7, 9, 3, 1] → rob houses 0, 2, 4 → 2+9+1 = 12
//
// dp[i] = max money robbing first i houses
//   dp[i] = max(dp[i-1], dp[i-2] + nums[i])
//           (skip house i)  (rob house i)
// ============================================================

class HouseRobber
{
    public static int Rob(int[] nums)
    {
        if (nums.Length == 1) return nums[0];

        int prev2 = 0;        // dp[i-2]
        int prev1 = nums[0];  // dp[i-1]

        for (int i = 1; i < nums.Length; i++)
        {
            int current = Math.Max(prev1, prev2 + nums[i]);
            prev2 = prev1;
            prev1 = current;
        }

        return prev1;
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class DPProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 17: DYNAMIC PROGRAMMING");
        Console.WriteLine("======================================================");

        // --- Fibonacci ---
        Console.WriteLine("\n=== 1. Fibonacci Sequence ===");
        Console.WriteLine($"  fib(10) naive:          {FibonacciDP.Naive(10)}");
        Console.WriteLine($"  fib(10) memoized:       {FibonacciDP.Memoized(10)}");
        Console.WriteLine($"  fib(10) tabulation:     {FibonacciDP.Tabulation(10)}");
        Console.WriteLine($"  fib(10) space-optimized:{FibonacciDP.SpaceOptimized(10)}");
        Console.WriteLine($"  fib(50) space-optimized:{FibonacciDP.SpaceOptimized(50)}  (naive would take hours!)");

        // --- Knapsack ---
        Console.WriteLine("\n=== 2. 0/1 Knapsack ===");
        int[] weights = { 1, 3, 4, 5 };
        int[] values  = { 1, 4, 5, 7 };
        int capacity  = 7;
        Console.WriteLine($"  Items: weights=[1,3,4,5] values=[1,4,5,7] capacity=7");
        Console.WriteLine($"  Max value (DP):       {KnapsackProblem.Knapsack01(weights, values, capacity)}");
        Console.WriteLine($"  Max value (optimized):{KnapsackProblem.Knapsack01Optimized(weights, values, capacity)}");

        // --- Coin Change ---
        Console.WriteLine("\n=== 3. Coin Change ===");
        int[] coins = { 1, 5, 10, 25 };
        Console.WriteLine($"  Coins=[1,5,10,25], amount=41");
        Console.WriteLine($"  Min coins:    {CoinChange.MinCoins(coins, 41)}  (25+10+5+1)");
        Console.WriteLine($"  Ways to make 11 cents with [1,5,10]: {CoinChange.CountWays(new[]{1,5,10}, 11)}");

        // --- LCS ---
        Console.WriteLine("\n=== 4. Longest Common Subsequence ===");
        string s1 = "ABCBDAB", s2 = "BDCABA";
        Console.WriteLine($"  Text1: {s1},  Text2: {s2}");
        Console.WriteLine($"  LCS length: {LongestCommonSubsequence.LCS(s1, s2)}");
        Console.WriteLine($"  LCS string: {LongestCommonSubsequence.GetLCSString(s1, s2)}");

        // --- LIS ---
        Console.WriteLine("\n=== 5. Longest Increasing Subsequence ===");
        int[] arr = { 10, 9, 2, 5, 3, 7, 101, 18 };
        Console.WriteLine($"  Array: [{string.Join(", ", arr)}]");
        Console.WriteLine($"  LIS length: {LongestIncreasingSubsequence.LIS(arr)}  (2,5,7,101)");

        // --- Edit Distance ---
        Console.WriteLine("\n=== 6. Edit Distance ===");
        Console.WriteLine($"  horse → ros: {EditDistance.MinDistance("horse", "ros")} operations");
        Console.WriteLine($"  kitten → sitting: {EditDistance.MinDistance("kitten", "sitting")} operations");

        // --- House Robber ---
        Console.WriteLine("\n=== 7. House Robber ===");
        int[] houses = { 2, 7, 9, 3, 1 };
        Console.WriteLine($"  Houses: [{string.Join(", ", houses)}]");
        Console.WriteLine($"  Max loot: {HouseRobber.Rob(houses)}  (2+9+1=12)");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. DP requires: Optimal Substructure + Overlapping Subproblems");
        Console.WriteLine("  2. Top-Down (Memoization) = Recursion + Cache");
        Console.WriteLine("  3. Bottom-Up (Tabulation) = Iterative table filling");
        Console.WriteLine("  4. Space optimization: often only need previous row/2 values");
        Console.WriteLine("  5. Define dp[i] meaning clearly: 'dp[i] = ...'");
        Console.WriteLine("  6. Identify base cases and recurrence relation first");
        Console.WriteLine("======================================================");
    }
}
