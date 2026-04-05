# Phase 9: Stack (LIFO) — Complete Guide

## What is a Stack?

A **Stack** is a linear data structure that follows the **LIFO** principle:
> **Last In, First Out** — the last element added is the first one removed.

Real-world analogy: A **stack of plates**. You place plates on top, and when you need one, you take from the top too. You can't take from the middle without removing the ones above.

```
PUSH (add)          POP (remove)
    ↓                    ↑
  ┌───┐ ← TOP
  │ 4 │ ← most recently added
  │ 3 │
  │ 2 │
  │ 1 │ ← first element pushed
  └───┘
```

---

## Core Operations

| Operation | Description | Time Complexity |
|---|---|---|
| `Push(x)` | Add element to top | O(1) |
| `Pop()` | Remove and return top element | O(1) |
| `Peek()` | Read top element (no remove) | O(1) |
| `IsEmpty` | Check if stack has no elements | O(1) |
| `Count` | Number of elements | O(1) |

> All operations are **O(1)** — this is what makes Stack so powerful.

---

## C# Stack<T> API

```csharp
Stack<int> stack = new Stack<int>();

// Add to top
stack.Push(10);
stack.Push(20);
stack.Push(30);
// Stack (bottom→top): [10, 20, 30]

// Peek — see top without removing
int top = stack.Peek();      // 30 (NOT removed)

// Pop — remove and return top
int popped = stack.Pop();    // 30 (removed)
// Stack is now: [10, 20]

// Safe versions (don't throw on empty)
if (stack.TryPop(out int val))    { /* success */ }
if (stack.TryPeek(out int peeked)){ /* success */ }

// Properties
int count = stack.Count;          // 2
bool empty = stack.Count == 0;    // false

// Contains check — O(n) linear scan
bool has10 = stack.Contains(10);  // true

// Create from collection — note: reverses order (LIFO)
Stack<int> fromArr = new Stack<int>(new[] { 1, 2, 3 });
fromArr.Pop();  // returns 3 (last element = top of stack)

// Clear
stack.Clear();
```

---

## Internal Implementation

`Stack<T>` in C# is backed by a **dynamically-resizing array**, not a linked list:

```
Push order: 1, 2, 3, 4
Internal array: [ 1 | 2 | 3 | 4 | _ | _ | _ | _ ]
                                  ^ _top index = 3
```

- When the array is full, it **doubles in size** (amortized O(1) per push)
- Pop just decrements the `_top` pointer — very fast
- This gives better **cache performance** than a linked-list-based stack

---

## How Recursion Uses the Call Stack

Every time you call a function, the runtime pushes a **stack frame** onto the **call stack**. When the function returns, the frame is popped.

```csharp
int Factorial(int n)
{
    if (n == 0) return 1;        // base case
    return n * Factorial(n - 1); // recursive case
}

// Calling Factorial(4) creates this call stack:
// TOP → Factorial(0) returns 1
//       Factorial(1) returns 1*1 = 1
//       Factorial(2) returns 2*1 = 2
//       Factorial(3) returns 3*2 = 6
//       Factorial(4) returns 4*6 = 24 ← final answer
// BOTTOM → original call
```

> **StackOverflowException** occurs when recursion is too deep — the call stack runs out of space (typically ~1MB).

---

## Real-World Applications

| Use Case | How Stack Helps |
|---|---|
| **Undo/Redo** | Each action → push to undo stack. Undo → pop, push to redo stack |
| **Browser back button** | Each page visit → push. Back → pop |
| **Expression evaluation** | Parse RPN (Reverse Polish Notation) with a stack |
| **Balanced parentheses** | Push `(`, pop and match on `)` |
| **Function call stack** | Runtime uses LIFO for nested function calls |
| **DFS traversal** | Depth-First Search uses stack (or recursion = implicit stack) |
| **Compiler syntax parsing** | Parse code expressions, check block nesting |
| **Memory management** | Stack memory for local variables (auto-freed on return) |

---

## Interview Questions & Answers

### Q1: What is the difference between a Stack and a Queue?

**Answer:**
- **Stack** — LIFO: last element added is first removed. Like a stack of plates.
- **Queue** — FIFO: first element added is first removed. Like a queue at a checkout.

Key difference: Stack uses ONE end (top) while Queue uses TWO ends (front for remove, rear for add).

---

### Q2: How would you implement a Queue using two Stacks?

**Answer:** 
```csharp
// Two stacks: inbox (for Enqueue) and outbox (for Dequeue)
class QueueUsingTwoStacks<T>
{
    Stack<T> inbox = new();   // new items go here
    Stack<T> outbox = new();  // items come out from here

    public void Enqueue(T item)
    {
        inbox.Push(item);  // always push to inbox
    }

    public T Dequeue()
    {
        if (outbox.Count == 0)
        {
            // Move ALL inbox items to outbox (reverses order = FIFO)
            while (inbox.Count > 0)
                outbox.Push(inbox.Pop());
        }
        return outbox.Pop();  // FIFO order
    }
}
// Amortized O(1) per operation
```

---

### Q3: How do you implement a Min Stack (stack that returns minimum in O(1))?

**Answer:**
```csharp
class MinStack
{
    Stack<int> mainStack = new();  // stores all values
    Stack<int> minStack  = new();  // stores running minimums

    public void Push(int val)
    {
        mainStack.Push(val);
        // Push to minStack only if val ≤ current min
        if (minStack.Count == 0 || val <= minStack.Peek())
            minStack.Push(val);
    }

    public int Pop()
    {
        int val = mainStack.Pop();
        // If we're removing the current minimum, remove from minStack too
        if (val == minStack.Peek())
            minStack.Pop();
        return val;
    }

    public int GetMin() => minStack.Peek();  // O(1)!
}
```

---

### Q4: How would you check if parentheses are balanced?

**Answer:** Use a stack:
1. Scan characters left to right
2. Opening bracket `( { [` → push to stack
3. Closing bracket `) } ]` → pop from stack, verify it matches
4. If stack is empty when popping (mismatch) → false
5. If stack is non-empty at end → false (unclosed brackets)

Time: O(n), Space: O(n)

---

### Q5: What is the time complexity of all Stack operations?

**Answer:** All core operations are **O(1)**: Push, Pop, Peek, Count, IsEmpty.
`Contains` is **O(n)** because it must linearly scan all elements.

---

### Q6: What is a Monotonic Stack and when do you use it?

**Answer:** A monotonic stack maintains elements in either **strictly increasing** or **strictly decreasing** order. 

Used for problems like:
- **Next Greater Element**: for each element, find the next larger one
- **Daily Temperatures**: how many days until a warmer day
- **Largest Rectangle in Histogram**

Pattern: As you iterate, pop elements from the stack that violate the monotonic property — this triggers the "answer" for those popped elements.

---

## Scenario-Based Questions

### Scenario 1: Text Editor with Undo/Redo
> "Design a text editor where users can type text and use Ctrl+Z (undo) and Ctrl+Y (redo)."

**Answer:**
- Two stacks: `undoStack` and `redoStack`
- Before any change: push current state to `undoStack`
- `Undo()`: pop from `undoStack`, push to `redoStack`, restore state
- `Redo()`: pop from `redoStack`, push to `undoStack`, restore state
- Any new user action: clear `redoStack` (redo history invalidated)

---

### Scenario 2: Expression Evaluator
> "Build a calculator that evaluates expressions like `3 + (4 * 2) - 1`."

**Answer (two-stack approach):**
- Stack 1 (values): stores numbers
- Stack 2 (operators): stores +, -, *, /
- When seeing `(`: push to operator stack
- When seeing `)`: pop and evaluate until matching `(`
- When seeing operator: pop and evaluate higher-or-equal precedence operators first

---

### Scenario 3: Browser Navigation
> "How would you implement a browser's back/forward buttons?"

**Answer:**
```csharp
Stack<string> back    = new();  // pages visited
Stack<string> forward = new();  // undone pages
string current = "home";

void Visit(string url)
{
    back.Push(current);    // save current page
    forward.Clear();       // new visit clears forward history
    current = url;
}

void GoBack()
{
    if (back.Count == 0) return;
    forward.Push(current);  // save current for forward
    current = back.Pop();
}

void GoForward()
{
    if (forward.Count == 0) return;
    back.Push(current);     // save current for back
    current = forward.Pop();
}
```

---

## Common Mistakes

1. **Empty stack exception**: Always check `Count > 0` or use `TryPop`/`TryPeek` before Pop/Peek
2. **Wrong LIFO order**: Remember — Pop gives you the LAST pushed item, not the first
3. **Stack overflow in recursion**: Add a base case and ensure recursion terminates
4. **Using Stack for FIFO**: Stack is LIFO only — use Queue for FIFO
5. **Confusing call stack with Stack<T>**: The runtime's call stack is a stack concept but separate from `System.Collections.Generic.Stack<T>`

---

## Time & Space Complexity

| Operation | Time | Space |
|---|---|---|
| Push | O(1) amortized | — |
| Pop | O(1) | — |
| Peek | O(1) | — |
| Contains | O(n) | — |
| Overall space used | — | O(n) |
