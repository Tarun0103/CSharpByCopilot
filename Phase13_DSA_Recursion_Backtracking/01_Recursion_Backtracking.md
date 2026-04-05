# Phase 13: Recursion & Backtracking

## What is Recursion?

**Recursion** is a technique where a function **calls itself** to solve a smaller version of the same problem.

Every recursive function needs:
1. **Base case(s):** Condition to stop recursion (prevent infinite loop)
2. **Recursive case:** Call itself with a smaller/simpler input

```
factorial(5) = 5 × factorial(4)
             = 5 × 4 × factorial(3)
             = 5 × 4 × 3 × factorial(2)
             = 5 × 4 × 3 × 2 × factorial(1)
             = 5 × 4 × 3 × 2 × 1
             = 120
```

---

## How Recursion Works Internally (Call Stack)

Each function call is pushed onto the **call stack**. When the base case is hit, the stack unwinds (pops frames one by one).

```
factorial(3)
  └─ factorial(2)
       └─ factorial(1)
            └─ returns 1   ← base case
       └─ returns 2×1 = 2
  └─ returns 3×2 = 6
```

**Stack Overflow** occurs when recursion goes too deep (default stack ~1MB in C# ≈ ~10,000-20,000 frames).

---

## Recursion Types

### Direct Recursion
Function calls itself directly: `f() { f(); }`

### Indirect Recursion
A calls B, B calls A: `f() { g(); }  g() { f(); }`

### Tail Recursion
Recursive call is the LAST operation. Some languages optimize this to a loop (C# does NOT optimize tail calls in most cases).

```csharp
// NOT tail-recursive (multiply after recursive call returns)
int factorial(int n) => n * factorial(n-1);

// Tail-recursive (accumulator carries result)
int factorial(int n, int acc = 1) =>
    n <= 1 ? acc : factorial(n - 1, n * acc);
```

---

## Recursion vs Iteration

| | Recursion | Iteration |
|-|-----------|-----------|
| Readability | Often cleaner for divided problems | Usually more straightforward |
| Performance | Function call overhead, stack frames | Generally faster |
| Stack overflow risk | Yes (deep recursion) | No |
| Memory | O(depth) call stack | O(1) usually |
| Use for | Tree traversal, divide & conquer, backtracking | Loops, sequential processing |

---

## What is Backtracking?

**Backtracking** is a refined recursion strategy for **constraint satisfaction** and **combinatorial enumeration** problems.

The pattern:
1. **Choose**: Make a choice (add an element, place a queen, etc.)
2. **Explore**: Recurse with the choice applied
3. **Unchoose (Backtrack)**: Undo the choice and try the next option

```
                Explore
                  /
Choose ———→ State
                  \
              Unchoose (if dead end or explored)
```

Backtracking = **Intelligent Brute Force**: prune branches that can't lead to a valid solution.

---

## Classic Backtracking Problems

### N-Queens
Place N queens on an N×N chessboard so no two queens attack each other.

```
For each column: try placing queen in each row
→ if valid placement (no conflicts): place queen, recurse next column
→ if not valid or no solution found: remove queen, try next row
```

### Sudoku Solver
Fill 9×9 grid with digits 1-9. Each row, column, and 3×3 box must contain each digit exactly once.

```
Find empty cell → try digits 1-9
→ if valid: fill digit, recurse
→ if contradiction found: erase digit, try next
```

### Subsets / Power Set
Generate all subsets of a set.

```
Include element → recurse → exclude element → recurse
```

For `[1,2,3]`: `{}, {1}, {2}, {3}, {1,2}, {1,3}, {2,3}, {1,2,3}` (2³ = 8 subsets)

### Permutations
Generate all orderings of elements.

For `[1,2,3]`: `123, 132, 213, 231, 312, 321` (3! = 6 permutations)

---

## Time Complexities

| Problem | Time Complexity |
|---------|----------------|
| Factorial | O(n) |
| Fibonacci (naive) | O(2^n) |
| Fibonacci (memoized) | O(n) |
| Subsets | O(2^n) |
| Permutations | O(n!) |
| N-Queens | O(n!) |
| Combination Sum | O(2^(target/min_coin)) |

---

## The Recursion Template

```csharp
void Backtrack(state, choices)
{
    if (goalReached(state)) {
        addToResults(state);
        return;
    }

    foreach (var choice in choices) {
        if (isValid(choice, state)) {
            makeChoice(choice);          // modify state
            Backtrack(newState, remainingChoices);
            undoChoice(choice);          // restore state (backtrack!)
        }
    }
}
```

---

## Interview Questions & Answers

**Q1: What makes a good base case for recursion?**

**A:** A good base case:
1. Is reached in a finite number of steps (convergence guarantee)
2. Returns a meaningful, directly computable answer without further recursion
3. Covers all termination scenarios (e.g., `n <= 0` not just `n == 0`)

Without a proper base case, you get infinite recursion → `StackOverflowException`.

---

**Q2: How does memoization differ from dynamic programming?**

**A:** Memoization is the **top-down approach** to DP — write the natural recursion, add a cache to store subproblem results. DP (tabulation) is the **bottom-up approach** — iteratively fill a table from base cases. Memoization only computes needed subproblems (lazy); tabulation computes all subproblems (eager). Both achieve the same asymptotic complexity; tabulation avoids call stack overhead.

---

**Q3: How do you convert a recursive solution to an iterative one?**

**A:** Use an explicit `Stack<T>` to simulate the call stack. Push the initial state, then loop: pop state, process it (checking base case), push sub-states to continue. This eliminates call stack overhead and prevents stack overflow but makes code more complex. Not all recursive algorithms benefit equally — tree traversals and DFS convert cleanly; divide-and-conquer algorithms are trickier.

---

**Q4: What is the difference between all subsets and all permutations?**

**A:**
- **Subsets (Power Set):** All combinations ignoring order. `{1,2}` and `{2,1}` are the same subset. For n elements: 2^n subsets. Recursion: at each step, choose to include or exclude the current element.
- **Permutations:** All orderings. `[1,2]` and `[2,1]` are different permutations. For n elements: n! permutations. Recursion: at each position, swap with each remaining element, recurse, swap back.

---

**Q5: What is the N-Queens problem and why is it famous?**

**A:** Place N non-attacking queens on an N×N chessboard. Queens attack horizontally, vertically, and diagonally. It's famous because it demonstrates backtracking elegantly: try placing a queen in each row for the current column; check validity (no conflict with existing queens); if valid recurse to next column; if all columns filled → solution found; if no valid row → backtrack to previous column. Time O(N!), but pruning makes it much faster in practice.

---

**Q6: When should you use backtracking vs DP?**

**A:** Use **backtracking** when:
- You need to enumerate ALL solutions (all subsets, all permutations)
- The solution is a sequence of decisions with constraints to check
- Pruning can cut large branches of the search space

Use **DP** when:
- You need ONE optimal solution (max, min, count)
- Subproblems overlap (same sub-input solved multiple times)
- You only need the final answer, not the path/configuration

---

## Scenario-Based Questions

**Scenario 1:** Design a maze solver. Given a 2D grid where 0=open and 1=wall, find a path from top-left to bottom-right.

**Answer:** Use backtracking DFS. From the current cell, try all 4 directions (up/down/left/right). Mark the current cell as visited (avoid cycles). If you reach the destination → return true. If a direction leads to a dead end → unmark and try next direction (backtrack). Time: O(m×n) in the worst case. Space: O(m×n) for the recursion stack in the worst case.

---

**Scenario 2:** A word processor needs to split a string into lines that fit within a given width, minimizing "raggedness" (excess space at end of lines). How do you solve it?

**Answer:** This is the **word wrap problem** (TeX/Knuth). Naive approach: try all ways to split words across lines (exponential with backtracking). Optimal: use DP. Define `dp[i] = minimum cost of wrapping words[i..n]`. Cost of placing words[i..j] on one line = `(width - total_length)²`. Process words backwards. This is O(n²) vs exponential for backtracking.

---

**Scenario 3:** Build a configuration validator that ensures a set of rule constraints are satisfied (like dependency rules: "A requires B", "B conflicts with C"). Find a valid configuration.

**Answer:** Model as a **constraint satisfaction problem (CSP)**. Use backtracking with constraint propagation. At each step: assign a value to the current variable, propagate constraints (remove invalid values from domains of other variables — "arc consistency"), recurse. If contradiction found → backtrack. Enhancement: use heuristics like MRV (Minimum Remaining Values) — pick the variable with the fewest valid options next. This is exactly how Sudoku solvers work.

---

## Common Mistakes

1. **Missing or wrong base case** → StackOverflowException or incorrect results
2. **Not backtracking** → forgetting to undo the choice (e.g., not removing from path, not unswapping)
3. **Wrong recursion structure** building up** → calling `f(n)` instead of `f(n-1)` (no progress toward base case)
4. **Off-by-one in recursion range** → off-by-one in loop bounds within the recursive call
5. **Not handling duplicate elements** → in subsets/combinations, if input has duplicates, sort first and skip repeated choices at the same recursion level
6. **Using global state without cleanup** → if using a global list, make sure to remove the last-added element on backtrack
