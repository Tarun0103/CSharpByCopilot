// ============================================================
// PHASE 21: IMMUTABLE COLLECTIONS
// Topics: System.Collections.Immutable, ImmutableList, 
//         ImmutableDictionary, ImmutableHashSet, ImmutableArray,
//         Builder pattern, when/why to use immutable collections
// Run this file in a .NET Console Application project
// Note: Add NuGet package System.Collections.Immutable if targeting
//       .NET Framework; it's included in .NET 5+ by default
// ============================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

// ============================================================
// WHAT ARE IMMUTABLE COLLECTIONS?
//
// Immutable collections cannot be changed after creation.
// "Modifications" return a NEW collection with the change applied.
// The original remains unchanged.
//
// Why use them?
//   1. THREAD SAFETY: no locks needed (data can't change)
//   2. PREDICTABILITY: state never changes unexpectedly
//   3. FUNCTIONAL STYLE: transformations produce new values
//   4. SAFE SHARING: share references without defensive copying
//   5. UNDO/HISTORY: keep snapshots for undo operations
//
// Trade-off: Each "modification" allocates a new collection.
//            Performance can suffer for frequent modifications.
//            Use Builders for batch modifications.
// ============================================================

// ============================================================
// SECTION 1: ImmutableList<T>
// Internally a balanced binary tree (AVL-style) — NOT an array!
// O(log n) for add, remove, index access (unlike List<T> O(1) index)
// ============================================================

class ImmutableListDemo
{
    public static void Run()
    {
        Console.WriteLine("=== ImmutableList<T> ===");

        // Create from existing collection
        var list1 = ImmutableList.Create(1, 2, 3, 4, 5);
        Console.Write("  list1: "); list1.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();

        // Add returns a NEW list — list1 is UNCHANGED
        var list2 = list1.Add(6);
        Console.Write("  list2 (after Add 6): "); list2.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();
        Console.Write("  list1 (unchanged):   "); list1.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();

        // Remove returns a NEW list
        var list3 = list2.Remove(3);
        Console.Write("  list3 (removed 3):  "); list3.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();

        // SetItem (replace at index) — new list returned
        var list4 = list1.SetItem(0, 100);
        Console.Write("  list4 (index 0 = 100): "); list4.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();

        // AddRange — add multiple items
        var list5 = list1.AddRange(new[] { 10, 20, 30 });
        Console.Write("  list5 (AddRange):   ");
        list5.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();

        // BUILDER: batch modifications without creating intermediate collections
        Console.WriteLine("\n  Using Builder for batch modifications:");
        var builder = list1.ToBuilder();  // mutable builder on top of immutable list
        builder.Add(99);
        builder.Remove(1);
        builder.Insert(0, 0);
        var list6 = builder.ToImmutable();  // back to immutable
        Console.Write("  list6 (batch modified): "); list6.ForEach(x => Console.Write($"{x} ")); Console.WriteLine();
    }
}

// ============================================================
// SECTION 2: ImmutableDictionary<K,V>
// O(log n) add/remove/lookup (tree-based, not hash-based)
// ============================================================

class ImmutableDictionaryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ImmutableDictionary<K,V> ===");

        // Build from scratch using builder (efficient for initial construction)
        var builder = ImmutableDictionary.CreateBuilder<string, int>();
        builder["Alice"] = 30;
        builder["Bob"]   = 25;
        builder["Carol"] = 35;
        var dict1 = builder.ToImmutable();

        Console.WriteLine("  dict1:");
        foreach (var kv in dict1) Console.WriteLine($"    {kv.Key}: {kv.Value}");

        // Add returns new dictionary
        var dict2 = dict1.Add("Dave", 28);
        Console.WriteLine($"\n  dict2 has Dave: {dict2.ContainsKey("Dave")}");
        Console.WriteLine($"  dict1 has Dave: {dict1.ContainsKey("Dave")} (unchanged)");

        // SetItem: update value (returns new dict)
        var dict3 = dict1.SetItem("Alice", 31);
        Console.WriteLine($"\n  dict3 Alice age: {dict3["Alice"]}  (was {dict1["Alice"]})");

        // Remove
        var dict4 = dict1.Remove("Bob");
        Console.WriteLine($"  dict4 has Bob: {dict4.ContainsKey("Bob")}");
    }
}

// ============================================================
// SECTION 3: ImmutableHashSet<T>
// Unique elements, no guaranteed order, O(log n) operations
// ============================================================

class ImmutableHashSetDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ImmutableHashSet<T> ===");

        var set1 = ImmutableHashSet.Create(1, 2, 3, 4, 5);
        var set2 = ImmutableHashSet.Create(4, 5, 6, 7, 8);

        // Set operations return NEW immutable sets
        var union     = set1.Union(set2);
        var intersect = set1.Intersect(set2);
        var except    = set1.Except(set2);

        Console.WriteLine($"  set1:      {{ {string.Join(", ", set1.OrderBy(x=>x))} }}");
        Console.WriteLine($"  set2:      {{ {string.Join(", ", set2.OrderBy(x=>x))} }}");
        Console.WriteLine($"  Union:     {{ {string.Join(", ", union.OrderBy(x=>x))} }}");
        Console.WriteLine($"  Intersect: {{ {string.Join(", ", intersect.OrderBy(x=>x))} }}");
        Console.WriteLine($"  Except:    {{ {string.Join(", ", except.OrderBy(x=>x))} }}");
    }
}

// ============================================================
// SECTION 4: ImmutableArray<T>
// Backed by a plain array — O(1) index access (unlike ImmutableList)
// Better performance for read-heavy scenarios.
// Downside: O(n) for structural modifications (creates full copy).
// ============================================================

class ImmutableArrayDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ImmutableArray<T> ===");

        var arr = ImmutableArray.Create(10, 20, 30, 40, 50);
        Console.Write("  arr: "); foreach (int x in arr) Console.Write($"{x} "); Console.WriteLine();
        Console.WriteLine($"  arr[2]: {arr[2]}  (O(1) index access!)");

        // Add/Remove still return new arrays (and copy all elements)
        var arr2 = arr.Add(60);
        Console.Write("  arr2: "); foreach (int x in arr2) Console.Write($"{x} "); Console.WriteLine();

        // Span access for high-performance reads
        var span = arr.AsSpan();
        Console.WriteLine($"  First via Span: {span[0]}");
    }
}

// ============================================================
// SECTION 5: REAL-WORLD USE CASE
// Immutable collections are ideal for:
//   - Configuration objects
//   - State snapshots (undo/redo)
//   - Sharing cached data across threads
//   - Functional transformations
// ============================================================

class ImmutableStateDemo
{
    // Application state as an immutable record + immutable collections
    // This pattern is common in Redux-style state management
    record AppState(
        string UserName,
        ImmutableList<string> Items,
        ImmutableDictionary<string, string> Settings
    );

    public static void Run()
    {
        Console.WriteLine("\n=== Immutable State Pattern ===");

        // Initial state
        var state1 = new AppState(
            UserName: "Alice",
            Items: ImmutableList.Create("Item1", "Item2"),
            Settings: ImmutableDictionary<string, string>.Empty
                .Add("Theme", "Dark")
                .Add("Lang", "EN")
        );

        Console.WriteLine($"  State1: User={state1.UserName}, Items={state1.Items.Count}");

        // "Mutate" state by creating a new state (immutable functional update)
        var state2 = state1 with
        {
            Items = state1.Items.Add("Item3"),
            Settings = state1.Settings.SetItem("Theme", "Light")
        };

        Console.WriteLine($"  State2: Items={state2.Items.Count}, Theme={state2.Settings["Theme"]}");
        Console.WriteLine($"  State1 unchanged: Items={state1.Items.Count}, Theme={state1.Settings["Theme"]}");
        Console.WriteLine("  (state1 is the 'undo' point — perfect for undo/redo history)");
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class ImmutableCollectionsProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 21: IMMUTABLE COLLECTIONS");
        Console.WriteLine("======================================================");

        ImmutableListDemo.Run();
        ImmutableDictionaryDemo.Run();
        ImmutableHashSetDemo.Run();
        ImmutableArrayDemo.Run();
        ImmutableStateDemo.Run();

        Console.WriteLine("\n=== Performance Comparison ===");
        Console.WriteLine("  ImmutableArray<T>: O(1) read, O(n) write  — best for read-heavy");
        Console.WriteLine("  ImmutableList<T>:  O(log n) read/write   — balanced tree");
        Console.WriteLine("  ImmutableDictionary: O(log n) all ops     — tree-based, not hash");
        Console.WriteLine("  Builder pattern: batch changes in O(1), convert to immutable once");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Immutable collections are thread-safe by design — no locks needed");
        Console.WriteLine("  2. 'Modifications' return NEW collections; originals are unchanged");
        Console.WriteLine("  3. Use Builder for batch modifications — avoids many intermediate allocations");
        Console.WriteLine("  4. ImmutableArray: best read performance (O(1)), worst write performance");
        Console.WriteLine("  5. ImmutableDictionary: O(log n) vs Dictionary O(1) — slower but safe");
        Console.WriteLine("  6. Ideal for: config, cached data, state management, undo/redo history");
        Console.WriteLine("  7. Namespace: System.Collections.Immutable (NuGet or included in .NET 5+)");
        Console.WriteLine("======================================================");
    }
}
