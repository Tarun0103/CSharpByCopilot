// ============================================================
// PHASE 24 — PROJECT 1: UNDO/REDO TEXT EDITOR
// Demonstrates: Stack data structure in a real application
// Features: Type text, Undo, Redo, Show history
// ============================================================

using System;
using System.Collections.Generic;
using System.Text;

// ============================================================
// HOW IT WORKS:
//
// Two stacks are used:
//   _undoStack: stores past states (each state = full document text)
//   _redoStack: stores states that were undone (for redo)
//
// Undo: pop from _undoStack, push current to _redoStack, apply previous
// Redo: pop from _redoStack, push current to _undoStack, apply redone state
//
// Real editors use "command objects" instead of full snapshots for efficiency.
// This demo uses string snapshots for simplicity.
// ============================================================

class TextEditor
{
    private StringBuilder _document = new StringBuilder(); // current text
    private Stack<string> _undoStack = new Stack<string>(); // history
    private Stack<string> _redoStack = new Stack<string>(); // redo history
    private const int MaxUndoLevels = 50;                   // limit memory use

    // Save current state to undo stack before any modification
    private void SaveState()
    {
        _undoStack.Push(_document.ToString());

        // Clear redo stack on new action (redo history is invalid after new change)
        _redoStack.Clear();

        // Limit undo history to prevent unbounded memory use
        if (_undoStack.Count > MaxUndoLevels)
        {
            var temp = new Stack<string>();
            int count = 0;
            foreach (var state in _undoStack)
            {
                if (count++ < MaxUndoLevels) temp.Push(state);
            }
            _undoStack.Clear();
            foreach (var s in temp.Reverse()) _undoStack.Push(s);
        }
    }

    // Type text at the end
    public void Type(string text)
    {
        SaveState();
        _document.Append(text);
        Console.WriteLine($"  Typed: '{text}' → Document: \"{_document}\"");
    }

    // Delete the last N characters
    public void Delete(int count = 1)
    {
        if (_document.Length == 0) { Console.WriteLine("  Nothing to delete."); return; }
        count = Math.Min(count, _document.Length);
        SaveState();
        _document.Remove(_document.Length - count, count);
        Console.WriteLine($"  Deleted {count} char(s) → Document: \"{_document}\"");
    }

    // Undo: restore previous state
    public void Undo()
    {
        if (_undoStack.Count == 0) { Console.WriteLine("  Nothing to undo."); return; }

        _redoStack.Push(_document.ToString());   // save current for redo
        string previous = _undoStack.Pop();
        _document.Clear();
        _document.Append(previous);
        Console.WriteLine($"  Undo → Document: \"{_document}\"");
    }

    // Redo: re-apply an undone action
    public void Redo()
    {
        if (_redoStack.Count == 0) { Console.WriteLine("  Nothing to redo."); return; }

        _undoStack.Push(_document.ToString());   // save current for undo
        string next = _redoStack.Pop();
        _document.Clear();
        _document.Append(next);
        Console.WriteLine($"  Redo → Document: \"{_document}\"");
    }

    public void ShowDocument() =>
        Console.WriteLine($"  Current Document: \"{_document}\"  (Undo:{_undoStack.Count} Redo:{_redoStack.Count})");
}

class UndoRedoProject
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PROJECT 1: UNDO/REDO TEXT EDITOR (using Stack)");
        Console.WriteLine("======================================================\n");

        var editor = new TextEditor();

        editor.Type("Hello");
        editor.Type(", World");
        editor.Type("!");
        editor.ShowDocument();

        Console.WriteLine();
        editor.Delete(7);     // delete ", World!"
        editor.ShowDocument();

        Console.WriteLine();
        editor.Undo();         // restore ", World!" deleted state → brings back ", World!"
        editor.Undo();         // restore before last type
        editor.ShowDocument();

        Console.WriteLine();
        editor.Redo();         // re-apply
        editor.ShowDocument();

        Console.WriteLine();
        editor.Type(" New Text");  // this clears redo history
        editor.ShowDocument();
        editor.Redo();             // nothing to redo

        Console.WriteLine("\nKey Data Structures used:");
        Console.WriteLine("  Stack<string> _undoStack — LIFO: last typed = first undone");
        Console.WriteLine("  Stack<string> _redoStack — LIFO: last undone = first redone");
        Console.WriteLine("  StringBuilder _document  — efficient string mutations");
    }
}
