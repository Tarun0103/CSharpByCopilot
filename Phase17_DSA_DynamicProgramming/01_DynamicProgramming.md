# Phase 17: Dynamic Programming (DP)

## What is Dynamic Programming?

**Dynamic Programming** is an algorithm design technique that solves complex problems by:
1. Breaking them into **overlapping subproblems**
2. Solving each subproblem **once** and **storing** the result
3. **Reusing** stored results instead of recomputing

> "DP = Careful Brute Force" — the brute force is recursion; the care is memoization.

---

## Two Required Properties

| Property | Meaning |
|----------|---------|
| **Optimal Substructure** | The optimal solution can be constructed from optimal solutions to subproblems |
| **Overlapping Subproblems** | The same subproblems are solved repeatedly in a naive recursive approach |

If a problem has both → Dynamic Programming is applicable.

---

## Two Approaches

### Top-Down (Memoization)
- Write the natural recursive solution
- Add a cache (dictionary or array) to store computed results
- Before computing, check cache; if found, return cached value

```csharp
Dictionary<int, long> memo = new();

long Fib(int n) {
    if (n <= 1) return n;
    if (memo.ContainsKey(n)) return memo[n];  // cache hit
    memo[n] = Fib(n-1) + Fib(n-2);           // compute and cache
    return memo[n];
}
```

**Pros:** Intuitive, only computes needed subproblems  
**Cons:** Recursion overhead, potential stack overflow for large n

### Bottom-Up (Tabulation)
- Start from base cases, iteratively build up to the answer
- Fill a table (array) in a specific order (usually left to right)

```csharp
long[] dp = new long[n + 1];
dp[0] = 0; dp[1] = 1;
for (int i = 2; i <= n; i++)
    dp[i] = dp[i-1] + dp[i-2];
```

**Pros:** No recursion overhead, cache-friendly, easier to optimize space  
**Cons:** May compute unnecessary subproblems, order matters

---

## Classic DP Problems

### 1. Fibonacci
```
fib(n) = fib(n-1) + fib(n-2),  fib(0)=0, fib(1)=1

| Approach | Time | Space |
|----------|------|-------|
| Naive recursion | O(2^n) | O(n) stack |
| Memoization | O(n) | O(n) |
| Tabulation | O(n) | O(n) |
| Space-optimized | O(n) | O(1) |
```

### 2. 0/1 Knapsack
**Problem:** Given items with weights/values and capacity W, maximize value.

```
dp[i][w] = max value using first i items with capacity w

if weight[i] > w:  dp[i][w] = dp[i-1][w]
else:              dp[i][w] = max(dp[i-1][w], dp[i-1][w-wt[i]] + val[i])

Time: O(n × W),  Space: O(n × W) → optimized to O(W)
```

### 3. Coin Change (Minimum Coins)
**Problem:** Min coins to make amount. Infinite supply of each denomination.

```
dp[i] = min coins to make amount i
dp[0] = 0
dp[i] = min(dp[i - coin] + 1) for each coin ≤ i

Time: O(amount × coins),  Space: O(amount)
```

### 4. Coin Change II (Number of Ways)
**Problem:** Count distinct ways to make amount.

```
dp[0] = 1  (one way to make 0: use nothing)
For each coin: dp[i] += dp[i - coin],  for i from coin to amount

Key: outer loop = COIN (not amount) prevents counting same combination twice
```

### 5. Longest Common Subsequence (LCS)
**Problem:** Longest subsequence common to both strings. (Not necessarily contiguous)

```
dp[i][j] = LCS length of text1[0..i-1] and text2[0..j-1]

if text1[i-1] == text2[j-1]:  dp[i][j] = dp[i-1][j-1] + 1
else:                          dp[i][j] = max(dp[i-1][j], dp[i][j-1])

Time: O(m×n),  Space: O(m×n) → optimized to O(min(m,n))
```

### 6. Longest Increasing Subsequence (LIS)
**Problem:** Longest strictly increasing subsequence.

```
dp[i] = LIS length ending at index i
dp[i] = max(dp[j] + 1) for all j < i where nums[j] < nums[i]
Base: dp[i] = 1 (each element alone)

Time: O(n²) → O(n log n) with binary search + patience sorting
```

### 7. Edit Distance (Levenshtein)
**Problem:** Min operations (insert, delete, replace) to convert word1 → word2.

```
dp[i][j] = min ops to convert word1[0..i-1] to word2[0..j-1]

if word1[i-1] == word2[j-1]:  dp[i][j] = dp[i-1][j-1]
else: dp[i][j] = 1 + min(dp[i-1][j],    // delete
                          dp[i][j-1],    // insert
                          dp[i-1][j-1]) // replace

Time: O(m×n),  Space: O(m×n)
```

### 8. House Robber
**Problem:** Max sum of non-adjacent elements.

```
dp[i] = max(dp[i-1], dp[i-2] + nums[i])
      = max(skip house i, rob house i)

Space-optimized: only track prev1, prev2
```

---

## DP Problem-Solving Template

1. **Define the state**: What does `dp[i]` (or `dp[i][j]`) represent?
2. **Identify base cases**: What are the simplest known values?
3. **Write the recurrence**: How does `dp[i]` relate to smaller subproblems?
4. **Determine fill order**: Ensure all needed values are computed before use
5. **Return the answer**: Which cell holds the final answer?

---

## Common DP Patterns

| Pattern | Description | Example Problems |
|---------|-------------|-----------------|
| **1D DP** | `dp[i]` depends on `dp[i-1]`, `dp[i-2]` | Fibonacci, House Robber, Climbing Stairs |
| **2D DP** | `dp[i][j]` depends on adjacent cells | LCS, Edit Distance, Matrix Chain |
| **Knapsack** | Include or exclude each item | 0/1 Knapsack, Subset Sum, Coin Change |
| **Path DP** | Moving through grid/matrix | Min path sum, Unique paths |
| **Interval DP** | `dp[i][j]` over substrings/subarrays | Palindrome partitioning, Burst Balloons |
| **State machine** | Multiple states with transitions | Stock buy/sell problems |

---

## Interview Questions & Answers

**Q1: How do you identify if a problem can be solved with DP?**

**A:** Look for these signals:
1. **"Optimal"** in the problem — maximize, minimize, most, least
2. The problem asks to **count** the number of ways
3. A brute-force recursive solution has **repeated computation** of the same inputs
4. You can define a clear **state** that captures all info needed for a subproblem

Try writing the recursive solution first. If you see repeated subproblems (same function arguments called multiple times), add memoization.

---

**Q2: What is the difference between memoization and tabulation?**

**A:**
| | Memoization (Top-Down) | Tabulation (Bottom-Up) |
|-|------------------------|------------------------|
| Approach | Recursive + cache | Iterative table |
| Order | Only needed subproblems | All subproblems |
| Overhead | Function call stack | None |
| Space | Cache (hash map or array) | DP array/table |
| Easier to write? | Often yes (natural recursion) | Requires figuring out order |
| Stack overflow risk? | Yes (for large n) | No |

---

**Q3: Explain the 0/1 Knapsack problem and its DP solution.**

**A:** In the 0/1 knapsack, you have `n` items each with a weight and value, and a bag of capacity `W`. Goal: maximize value without exceeding capacity. Each item can be taken at most once (0 = skip, 1 = take).

Define `dp[i][w]` = max value using first `i` items with capacity `w`. For each item, decide: skip (don't change value) or take (if it fits, add its value and reduce capacity). The answer is `dp[n][W]`. Time: O(n×W), Space: O(n×W) → optimizable to O(W) by using 1D array with reverse traversal.

---

**Q4: Why does Coin Change use bottom-up DP instead of greedy?**

**A:** Greedy fails for certain coin sets. Example: coins = [1, 3, 4], amount = 6.
- Greedy picks 4+1+1 = 3 coins
- Optimal is 3+3 = 2 coins

Greedy always picks the largest coin, which doesn't guarantee the global optimum. DP explores all possibilities and picks the minimum, guaranteeing correctness.

---

**Q5: What is the time complexity of the LCS algorithm and can it be improved?**

**A:** Standard LCS is O(m×n) time and O(m×n) space. Space can be improved to O(min(m,n)) since we only ever need the current and previous rows. For very long sequences (bioinformatics), the Hirschberg algorithm achieves O(m×n) time with O(min(m,n)) space and can also reconstruct the LCS. For approximate matching, we can use the suffix array approach.

---

**Q6: What distinguishes LCS from LIS?**

**A:**
- **LCS** (Longest Common Subsequence): characters from **two different strings** that appear in the same relative order in both. O(m×n).
- **LIS** (Longest Increasing Subsequence): subsequence within a **single array** where each element is strictly greater than the previous. O(n²) naive DP, O(n log n) with patience sorting/binary search.

Both are "subsequence" problems (elements maintain relative order, don't need to be adjacent).

---

## Scenario-Based Questions

**Scenario 1:** Your company's deployment system has hundreds of build steps with dependencies. Given the build steps and their estimated durations, find the minimum time to complete all steps if you can run independent steps in parallel.

**Answer:** This is a **Critical Path Method** problem on a DAG. Model steps as vertices with weights (duration), dependencies as directed edges. Use topological sort to order steps. Define `dp[v]` = earliest completion time for step `v` = max(dp[predecessor]) + duration[v]. Process in topological order. The answer is the max `dp[v]` across all vertices (the critical path length). O(V + E) time.

---

**Scenario 2:** You're implementing a spell-checker autocorrect feature. When a user types a word, suggest the closest valid dictionary word. How do you measure "closeness"?

**Answer:** Use **Edit Distance (Levenshtein Distance)**. Compute the edit distance between the typed word and each dictionary word. Suggest words within a threshold (e.g., distance ≤ 2). Optimization: use a BK-tree (Burkhard-Keller tree) to reduce the number of edit distance computations from O(n) to O(log n) per query. The DP for each pair runs in O(m×k) where m, k are word lengths.

---

**Scenario 3:** A financial application processes transactions. Given a sequence of stock prices, find the maximum profit from a series of buy-sell operations with a cooling period of 1 day after each sale.

**Answer:** This is the **Stock Buy Sell with Cooldown** DP problem. Use 3 states:
- `held`: max profit when holding a stock
- `sold`: max profit on the day you just sold
- `rest`: max profit on a cooldown/rest day

Transitions: `held = max(held, rest - price)`, `sold = held + price`, `rest = max(rest, sold)`. Process each day. Final answer: `max(sold, rest)`. O(n) time, O(1) space.

---

## Common Mistakes

1. **Not defining the dp state precisely** → leads to wrong recurrences
2. **Wrong traversal order** → using a value before it's computed
3. **Off-by-one errors** → forgetting 0-indexed vs 1-indexed dp arrays
4. **Missing base cases** → uninitialized dp values cause wrong answers
5. **Greedy when DP is needed** → greedy doesn't always give optimal for all coin sets
6. **Forgetting the "1D knapsack reverse traversal"** → processing forward causes each item to be used multiple times
7. **Returning wrong cell** → dp is 1-indexed for string problems (dp[m][n], not dp[m-1][n-1])
