// ============================================================
// PHASE 9: STACK (LIFO) — Complete C# Implementation
// Topics: Stack concept, Custom Stack, C# Stack<T>,
//         Push/Pop/Peek, Undo/Redo, Expression Evaluation, Recursion
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;
using System.Text;

// ============================================================
// SECTION 1: CUSTOM STACK IMPLEMENTATION
// Shows HOW Stack<T> works internally (backed by an array)
// LIFO = Last In, First Out
// Think of a stack of plates — you add and remove from the TOP only
// ============================================================

public class MyStack<T>
{
    private T[] _data;          // Internal array to store elements
    private int _top;           // Index of the top element (-1 when empty)
    private int _capacity;

    public MyStack(int capacity = 16)
    {
        _capacity = capacity;
        _data = new T[_capacity];
        _top = -1;  // -1 means empty stack
    }

    // How many elements are currently in the stack
    public int Count => _top + 1;

    // Is the stack empty?
    public bool IsEmpty => _top == -1;

    // --- PUSH — O(1) amortized ---
    // Add element to the TOP of the stack
    // Like placing a plate on top of a pile
    public void Push(T item)
    {
        // Double capacity if full (same strategy as List<T>)
        if (_top == _capacity - 1)
        {
            _capacity *= 2;
            Array.Resize(ref _data, _capacity);
            Console.WriteLine($"  [Stack resized to {_capacity}]");
        }

        _top++;           // Move top pointer up
        _data[_top] = item;  // Place item at top
    }

    // --- POP — O(1) ---
    // Remove and RETURN the TOP element
    // Like removing the top plate from a pile
    public T Pop()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Stack is empty — cannot Pop()");

        T item = _data[_top];  // Get the top item
        _data[_top] = default; // Clear the reference (helps GC for reference types)
        _top--;                // Move top pointer down

        return item;
    }

    // --- PEEK — O(1) ---
    // READ the top element WITHOUT removing it
    // Like looking at the top plate without picking it up
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Stack is empty — cannot Peek()");

        return _data[_top];  // Just return, don't remove
    }

    // --- TryPop — safe version (no exception) ---
    public bool TryPop(out T item)
    {
        if (IsEmpty) { item = default; return false; }
        item = Pop();
        return true;
    }

    // --- TryPeek — safe version (no exception) ---
    public bool TryPeek(out T item)
    {
        if (IsEmpty) { item = default; return false; }
        item = Peek();
        return true;
    }

    // Print the stack (bottom to top for readability)
    public void Print(string label = "Stack")
    {
        Console.Write($"  {label} (bottom→top): [");
        for (int i = 0; i <= _top; i++)
        {
            Console.Write(_data[i]);
            if (i < _top) Console.Write(", ");
        }
        Console.WriteLine($"]  Top = {(IsEmpty ? "none" : _data[_top].ToString())}");
    }
}

// ============================================================
// SECTION 2: UNDO / REDO SYSTEM
// Real-world application: text editors, drawing apps, browser history
// Two stacks: undoStack and redoStack
// 
// How it works:
// - Action performed → push to undoStack, clear redoStack
// - Undo → pop from undoStack, push to redoStack
// - Redo → pop from redoStack, push to undoStack
// ============================================================

class TextEditor
{
    private string _text = "";                    // current document text
    private Stack<string> _undoStack = new();     // history of past states
    private Stack<string> _redoStack = new();     // undone states (for redo)

    // Make a change — saves current state for undo
    public void Type(string addition)
    {
        _undoStack.Push(_text);  // save CURRENT state before changing
        _redoStack.Clear();      // any new action clears redo history
        _text += addition;
        Console.WriteLine($"  Typed \"{addition}\" → Text: \"{_text}\"");
    }

    // Undo — restore previous state
    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            Console.WriteLine("  Nothing to undo.");
            return;
        }

        _redoStack.Push(_text);       // save current state for potential redo
        _text = _undoStack.Pop();     // restore previous state
        Console.WriteLine($"  Undo → Text: \"{_text}\"");
    }

    // Redo — re-apply an undone change
    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            Console.WriteLine("  Nothing to redo.");
            return;
        }

        _undoStack.Push(_text);       // save current state before redoing
        _text = _redoStack.Pop();     // restore the undone state
        Console.WriteLine($"  Redo → Text: \"{_text}\"");
    }

    public void ShowState() =>
        Console.WriteLine($"  Current: \"{_text}\" | Undo stack size: {_undoStack.Count} | Redo stack size: {_redoStack.Count}");
}

// ============================================================
// SECTION 3: BALANCED PARENTHESES CHECKER
// Classic stack interview problem!
// Check if brackets are correctly matched: ( ) { } [ ]
//
// Algorithm:
//   - Opening bracket: push onto stack
//   - Closing bracket: pop from stack and check if it matches
//   - If stack is empty when we need to pop → unbalanced
//   - If stack is non-empty at end → unbalanced
// ============================================================
class ParenthesesChecker
{
    public static bool IsBalanced(string expression)
    {
        Stack<char> stack = new Stack<char>();

        foreach (char c in expression)
        {
            // Opening brackets → push onto stack
            if (c == '(' || c == '{' || c == '[')
            {
                stack.Push(c);
            }
            // Closing brackets → must match top of stack
            else if (c == ')' || c == '}' || c == ']')
            {
                // Stack is empty but we need a matching open bracket
                if (stack.Count == 0) return false;

                char top = stack.Pop();

                // Check if top matches this closing bracket
                bool matches = (c == ')' && top == '(') ||
                               (c == '}' && top == '{') ||
                               (c == ']' && top == '[');

                if (!matches) return false;
            }
        }

        // Stack must be empty — every open bracket was matched
        return stack.Count == 0;
    }
}

// ============================================================
// SECTION 4: EVALUATE POSTFIX (REVERSE POLISH NOTATION) EXPRESSION
// Postfix: operators come AFTER their operands
// Example: "3 4 +" means (3 + 4) = 7
//          "5 1 2 + 4 * + 3 -" means 5 + ((1+2)*4) - 3 = 14
//
// Algorithm:
//   - Number: push onto stack
//   - Operator: pop two numbers, apply operator, push result
// ============================================================
class PostfixEvaluator
{
    public static double Evaluate(string expression)
    {
        Stack<double> stack = new Stack<double>();
        string[] tokens = expression.Split(' ');

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                // It's a number — push it
                stack.Push(number);
            }
            else
            {
                // It's an operator — pop TWO operands (order matters for - and /)
                double b = stack.Pop();  // right operand
                double a = stack.Pop();  // left operand

                double result = token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    _ => throw new ArgumentException($"Unknown operator: {token}")
                };

                stack.Push(result);  // push the result back
            }
        }

        return stack.Pop();  // final result
    }
}

// ============================================================
// SECTION 5: SORT A STACK
// Sort a stack so the smallest element is on top
// Constraint: only use another stack, no arrays
//
// Algorithm: Use a temporary stack as scratch space
// For each element from original stack:
//   - While temp stack's top < current element → push temp.top back to original
//   - Push current element to temp (now in sorted position)
// ============================================================
class StackSorter
{
    public static Stack<int> SortAscending(Stack<int> input)
    {
        Stack<int> sorted = new Stack<int>();  // will hold sorted result (largest-to-top)

        while (input.Count > 0)
        {
            int current = input.Pop();

            // Find correct position in sorted stack
            // Elements in sorted are in descending order (bottom to top)
            while (sorted.Count > 0 && sorted.Peek() > current)
            {
                input.Push(sorted.Pop());  // move back to input temporarily
            }

            sorted.Push(current);  // insert current in its sorted position
        }

        return sorted;  // Top has smallest element
    }
}

// ============================================================
// SECTION 6: DAILY TEMPERATURES — Stack classic interview problem
// Given temperatures array, for each day find how many days
// until a warmer temperature.
// 
// Example: [73,74,75,71,69,72,76,73]
// Answer:  [1,  1,  4,  2,  1, 1,  0, 0]
//
// Solution: Monotonic stack (decreasing from bottom to top)
// ============================================================
class DailyTemperatures
{
    public static int[] CalculateWaitDays(int[] temperatures)
    {
        int n = temperatures.Length;
        int[] result = new int[n];          // answer array (default 0)
        Stack<int> stack = new Stack<int>(); // stores INDICES, not temperatures

        for (int i = 0; i < n; i++)
        {
            // While stack has elements AND current temp is warmer than temp at stack's top index
            while (stack.Count > 0 && temperatures[i] > temperatures[stack.Peek()])
            {
                int prevIndex = stack.Pop();               // index of a colder day
                result[prevIndex] = i - prevIndex;         // how many days until warmer
            }

            stack.Push(i);  // push current day's index
        }

        // Any remaining indices in stack have no warmer day → result stays 0
        return result;
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class StackProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 9: STACK (LIFO) — Complete Demonstration");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Custom Stack basics
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Custom Stack — Push, Pop, Peek ===");

        var stack = new MyStack<int>();

        // Push elements — each goes ON TOP
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        stack.Print("After Push(10,20,30,40)");

        Console.WriteLine($"  Peek (top without removing): {stack.Peek()}");  // 40
        Console.WriteLine($"  Pop (remove from top): {stack.Pop()}");         // 40
        Console.WriteLine($"  Pop again: {stack.Pop()}");                      // 30
        stack.Print("After 2 Pops");
        Console.WriteLine($"  Count: {stack.Count}");  // 2

        // TryPop and TryPeek — safe versions
        if (stack.TryPop(out int val))
            Console.WriteLine($"  TryPop succeeded: {val}");  // 20

        // -------------------------------------------------------
        // DEMO 2: C# Built-in Stack<T>
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: C# Built-in Stack<T> ===");

        Stack<string> builtIn = new Stack<string>();

        // Push
        builtIn.Push("First");
        builtIn.Push("Second");
        builtIn.Push("Third");

        Console.WriteLine($"  Count: {builtIn.Count}");               // 3
        Console.WriteLine($"  Peek: {builtIn.Peek()}");               // Third
        Console.WriteLine($"  Pop: {builtIn.Pop()}");                  // Third
        Console.WriteLine($"  Pop: {builtIn.Pop()}");                  // Second

        // TryPop — won't throw if empty
        while (builtIn.TryPop(out string s))
            Console.WriteLine($"  TryPop: {s}");

        Console.WriteLine($"  TryPop on empty: {builtIn.TryPop(out _)}");  // False

        // Create from collection (note: reversed order — LIFO)
        Stack<int> fromArray = new Stack<int>(new[] { 1, 2, 3, 4, 5 });
        Console.Write("  Stack from [1,2,3,4,5], pop order: ");
        while (fromArray.Count > 0) Console.Write(fromArray.Pop() + " ");
        // Prints: 5 4 3 2 1 — LIFO order
        Console.WriteLine();

        // -------------------------------------------------------
        // DEMO 3: Undo / Redo Text Editor
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Undo/Redo System ===");

        var editor = new TextEditor();
        editor.Type("Hello");
        editor.Type(", World");
        editor.Type("!");
        editor.ShowState();

        editor.Undo();   // removes "!"
        editor.Undo();   // removes ", World"
        editor.ShowState();

        editor.Redo();   // re-applies ", World"
        editor.ShowState();

        editor.Type(" (edited)");  // new action clears redo stack
        editor.ShowState();

        editor.Redo();   // nothing to redo
        editor.Undo();   // undo " (edited)"
        editor.Undo();   // undo ", World"
        editor.Undo();   // undo "Hello"
        editor.Undo();   // nothing more

        // -------------------------------------------------------
        // DEMO 4: Balanced Parentheses
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: Balanced Parentheses Checker ===");

        string[] expressions = {
            "(a + b) * (c - d)",   // balanced
            "{[()]}",              // balanced
            "(((",                 // unbalanced - missing closing
            "([)]",                // unbalanced - wrong order
            "",                    // balanced (empty)
            "int[] arr = new int[5];"  // balanced (code line)
        };

        foreach (var expr in expressions)
        {
            bool balanced = ParenthesesChecker.IsBalanced(expr);
            Console.WriteLine($"  \"{expr}\" → {(balanced ? "BALANCED ✓" : "UNBALANCED ✗")}");
        }

        // -------------------------------------------------------
        // DEMO 5: Postfix Expression Evaluation
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: Postfix (RPN) Expression Evaluator ===");

        // "3 4 +"  means 3 + 4 = 7
        Console.WriteLine($"  \"3 4 +\" = {PostfixEvaluator.Evaluate("3 4 +")}");             // 7

        // "5 1 2 + 4 * + 3 -" means 5 + ((1+2)*4) - 3 = 5 + 12 - 3 = 14
        Console.WriteLine($"  \"5 1 2 + 4 * + 3 -\" = {PostfixEvaluator.Evaluate("5 1 2 + 4 * + 3 -")}");  // 14

        // "15 7 1 1 + - / 3 * 2 1 1 + + -" = ((15/(7-(1+1))) * 3) - (2+(1+1)) = 5
        Console.WriteLine($"  \"15 7 1 1 + - / 3 * 2 1 1 + + -\" = {PostfixEvaluator.Evaluate("15 7 1 1 + - / 3 * 2 1 1 + + -")}");  // 5

        // -------------------------------------------------------
        // DEMO 6: Sort a Stack
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 6: Sort a Stack (using temp stack only) ===");

        Stack<int> unsorted = new Stack<int>(new[] { 3, 1, 4, 1, 5, 9, 2, 6 });
        Console.Write("  Input (push order):  ");
        foreach (int n in unsorted) Console.Write(n + " ");
        Console.WriteLine();

        Stack<int> sorted = StackSorter.SortAscending(unsorted);
        Console.Write("  Sorted (pop order = smallest first): ");
        while (sorted.Count > 0) Console.Write(sorted.Pop() + " ");
        Console.WriteLine();

        // -------------------------------------------------------
        // DEMO 7: Daily Temperatures
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 7: Daily Temperatures (Monotonic Stack) ===");

        int[] temps = { 73, 74, 75, 71, 69, 72, 76, 73 };
        int[] days = DailyTemperatures.CalculateWaitDays(temps);

        Console.Write("  Temperatures: ");
        Console.WriteLine(string.Join(", ", temps));

        Console.Write("  Wait days:    ");
        Console.WriteLine(string.Join(", ", days));
        // Explanation: 73→74 (1 day), 74→75 (1 day), 75→76 (4 days), etc.

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Stack = LIFO (Last In, First Out)");
        Console.WriteLine("  2. Core operations: Push O(1), Pop O(1), Peek O(1)");
        Console.WriteLine("  3. Built on an array (auto-resizes when full)");
        Console.WriteLine("  4. Use for: undo/redo, bracket matching, expression eval");
        Console.WriteLine("  5. Recursion uses the CALL STACK (same LIFO principle)");
        Console.WriteLine("  6. Monotonic stack solves 'next greater/lesser element' problems");
        Console.WriteLine("======================================================");
    }
}
