// ============================================================
// PHASE 19: ADVANCED COLLECTIONS IN C#
// Topics: SortedList, SortedDictionary, SortedSet, 
//         ObservableCollection, ReadOnlyCollection,
//         IEnumerable / ICollection / IList interfaces,
//         Custom collection implementation
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

// ============================================================
// SECTION 1: SORTED COLLECTIONS
// These automatically maintain elements in sorted order.
// Useful when you need ordered traversal or sorted lookup.
// ============================================================

class SortedCollectionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== SortedList<K,V> ===");
        // SortedList: Key-value pairs sorted by KEY.
        // Internally uses TWO parallel arrays (keys[], values[]).
        // - Access by index (keys.IndexOfKey, Values[i]) — arrays = fast index
        // - Slower for large inserts/deletes vs SortedDictionary (O(n) vs O(log n))
        var sortedList = new SortedList<string, int>();
        sortedList["Banana"] = 3;
        sortedList["Apple"]  = 1;
        sortedList["Cherry"] = 2;

        // Keys are auto-sorted alphabetically
        Console.Write("  Keys in order: ");
        foreach (var kv in sortedList) Console.Write($"{kv.Key}={kv.Value}  ");
        Console.WriteLine();
        Console.WriteLine($"  First key by index: {sortedList.Keys[0]}");

        Console.WriteLine("\n=== SortedDictionary<K,V> ===");
        // SortedDictionary: Key-value pairs sorted by KEY.
        // Internally uses a Red-Black Tree (BST).
        // - O(log n) insert/delete/lookup — better for frequent modifications
        // - NO index-based access (unlike SortedList)
        // - More memory than SortedList (tree nodes vs arrays)
        var sortedDict = new SortedDictionary<int, string>();
        sortedDict[30] = "Charlie";
        sortedDict[10] = "Alice";
        sortedDict[20] = "Bob";

        Console.Write("  Keys in order: ");
        foreach (var kv in sortedDict) Console.Write($"{kv.Key}:{kv.Value}  ");
        Console.WriteLine();

        Console.WriteLine("\n=== SortedSet<T> ===");
        // SortedSet: Unique elements sorted automatically.
        // Backed by Red-Black Tree. O(log n) add/remove/contains.
        // Unlike SortedList/SortedDictionary, this is SET semantics (no keys).
        var sortedSet = new SortedSet<int> { 5, 2, 8, 1, 9, 3 };
        Console.Write("  Sorted elements: ");
        foreach (int x in sortedSet) Console.Write($"{x} ");
        Console.WriteLine();
        Console.WriteLine($"  Min: {sortedSet.Min},  Max: {sortedSet.Max}");

        // GetViewBetween: elements in a range — very useful!
        var inRange = sortedSet.GetViewBetween(3, 8);
        Console.Write("  Elements in [3,8]: ");
        foreach (int x in inRange) Console.Write($"{x} ");
        Console.WriteLine();
    }
}

// ============================================================
// SECTION 2: ObservableCollection<T>
//
// Notifies subscribers when items are Added, Removed, or Replaced.
// Used heavily in MVVM (WPF, Xamarin, MAUI) as data binding source.
// Implements INotifyCollectionChanged interface.
//
// Events raised:
//   CollectionChanged (NotifyCollectionChangedAction: Add/Remove/Replace/Reset/Move)
// ============================================================

class ObservableCollectionDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ObservableCollection<T> ===");
        var items = new ObservableCollection<string>();

        // Subscribe to changes — fired on every modification
        items.CollectionChanged += (sender, e) =>
        {
            Console.WriteLine($"  Collection changed! Action: {e.Action}");

            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    Console.WriteLine($"    Added:   {item}");

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    Console.WriteLine($"    Removed: {item}");
        };

        items.Add("Apple");      // triggers Add event
        items.Add("Banana");     // triggers Add event
        items.Remove("Apple");   // triggers Remove event
        items[0] = "Cherry";     // triggers Replace event

        Console.WriteLine($"  Final count: {items.Count}");
    }
}

// ============================================================
// SECTION 3: ReadOnlyCollection<T> and Immutable Wrappers
//
// Expose a collection as READ-ONLY without copying the data.
// The underlying list can still be modified by the owner —
// the wrapper just prevents external modification.
//
// Use: return read-only view from a class to prevent callers
//      from modifying internal state.
// ============================================================

class ReadOnlyDemo
{
    private List<string> _internalList = new List<string> { "A", "B", "C" };

    // Return safe read-only view — caller can enumerate but not modify
    public ReadOnlyCollection<string> GetItems()
        => _internalList.AsReadOnly();       // wraps _internalList, no copy

    public static void Run()
    {
        Console.WriteLine("\n=== ReadOnlyCollection<T> ===");
        var demo = new ReadOnlyDemo();
        var readOnly = demo.GetItems();

        Console.Write("  Items: ");
        foreach (var s in readOnly) Console.Write($"{s} ");
        Console.WriteLine();

        // Attempting to call readOnly.Add() would cause compile-time error
        // because ReadOnlyCollection doesn't implement IList<T>.Add
        Console.WriteLine("  (ReadOnlyCollection prevents Add/Remove — compile-time safety)");
    }
}

// ============================================================
// SECTION 4: COLLECTION INTERFACES HIERARCHY
//
// IEnumerable<T>                  ← foreach support (GetEnumerator)
//   └── ICollection<T>            ← Count, Add, Remove, Contains, Clear
//         └── IList<T>            ← index-based access [i], Insert, RemoveAt
//               └── List<T>       ← concrete implementation
//
// IEnumerable<T>
//   └── IReadOnlyCollection<T>    ← Count (read-only)
//         └── IReadOnlyList<T>    ← this[int] (read-only)
//
// Understanding interfaces helps write FLEXIBLE methods:
//   - IEnumerable<T>: just need to iterate → MOST flexible
//   - ICollection<T>: need Count or mutation
//   - IList<T>:      need index access
// ============================================================

class InterfaceHierarchyDemo
{
    // Method accepts ANY collection — most flexible
    public static void PrintAll(IEnumerable<string> items)
    {
        foreach (var item in items)
            Console.Write($"{item} ");
        Console.WriteLine();
    }

    // Method needs count + add — accepts List<T>, HashSet<T>, etc.
    public static void AddIfNotEmpty(ICollection<string> col, string item)
    {
        if (!string.IsNullOrEmpty(item))
        {
            col.Add(item);
            Console.WriteLine($"  Added '{item}'. Count now: {col.Count}");
        }
    }

    // Method needs index access — uses IList<T>
    public static void SwapFirstAndLast(IList<int> list)
    {
        if (list.Count < 2) return;
        (list[0], list[list.Count - 1]) = (list[list.Count - 1], list[0]);
    }

    public static void Run()
    {
        Console.WriteLine("\n=== Collection Interfaces ===");

        // IEnumerable<T>: works with any collection
        PrintAll(new List<string> { "Apple", "Banana" });
        PrintAll(new HashSet<string> { "Cherry", "Date" });

        // ICollection<T>
        var col = new List<string>();
        AddIfNotEmpty(col, "Item1");
        AddIfNotEmpty(col, "Item2");

        // IList<T>
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        SwapFirstAndLast(numbers);
        Console.Write("  After swap: ");
        numbers.ForEach(n => Console.Write($"{n} "));
        Console.WriteLine();
    }
}

// ============================================================
// SECTION 5: CUSTOM COLLECTION
//
// Implementing a custom type-safe bounded collection.
// Demonstrates: implementing IEnumerable<T>, indexer,
// Count property, Add, and custom iteration.
// ============================================================

class BoundedQueue<T> : IEnumerable<T>
{
    private Queue<T> _queue;
    private int _maxSize;

    public int Count => _queue.Count;
    public bool IsFull => _queue.Count >= _maxSize;

    public BoundedQueue(int maxSize)
    {
        _maxSize = maxSize;
        _queue = new Queue<T>(maxSize);
    }

    // Enqueue: if full, dequeue oldest (sliding window behavior)
    public void Enqueue(T item)
    {
        if (IsFull) _queue.Dequeue();   // evict oldest if full
        _queue.Enqueue(item);
    }

    public T Dequeue() => _queue.Dequeue();
    public T Peek()    => _queue.Peek();

    // Implement IEnumerable<T> for foreach support
    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"[{string.Join(", ", _queue)}]";
}

// ============================================================
// SECTION 6: COMPARISON OF COLLECTION TYPES
// ============================================================

class CollectionComparisonDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== BoundedQueue<T> Demo ===");
        var log = new BoundedQueue<string>(3);  // keep last 3 events only
        log.Enqueue("Event1");
        log.Enqueue("Event2");
        log.Enqueue("Event3");
        Console.WriteLine($"  After 3 adds: {log}");
        log.Enqueue("Event4");  // evicts Event1
        Console.WriteLine($"  After Event4:  {log}");
        log.Enqueue("Event5");  // evicts Event2
        Console.WriteLine($"  After Event5:  {log}");

        Console.Write("  Foreach: ");
        foreach (var e in log) Console.Write($"{e} ");
        Console.WriteLine();
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class AdvancedCollectionsProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 19: ADVANCED COLLECTIONS");
        Console.WriteLine("======================================================");

        SortedCollectionsDemo.Run();
        ObservableCollectionDemo.Run();
        ReadOnlyDemo.Run();
        InterfaceHierarchyDemo.Run();
        CollectionComparisonDemo.Run();

        Console.WriteLine("\n=== SortedList vs SortedDictionary vs SortedSet ===");
        Console.WriteLine("  SortedList<K,V>:      parallel arrays, index access, O(n) insert");
        Console.WriteLine("  SortedDictionary<K,V>: red-black tree, O(log n) insert, no index");
        Console.WriteLine("  SortedSet<T>:          red-black tree, unique sorted elements, GetViewBetween");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. SortedDictionary is better for frequent modifications (O(log n))");
        Console.WriteLine("  2. SortedList is better for infrequent changes, lots of index access");
        Console.WriteLine("  3. ObservableCollection is for UI data binding (MVVM pattern)");
        Console.WriteLine("  4. ReadOnlyCollection wraps without copying — prevents external mutation");
        Console.WriteLine("  5. Accept IEnumerable<T> in method params for maximum flexibility");
        Console.WriteLine("  6. Implement IEnumerable<T> in custom collections for foreach support");
        Console.WriteLine("======================================================");
    }
}
