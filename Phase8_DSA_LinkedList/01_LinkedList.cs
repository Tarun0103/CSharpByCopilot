// ============================================================
// PHASE 8: LINKED LIST — Complete C# Implementation
// Topics: Singly Linked List, Doubly Linked List (C# built-in),
//         Custom Implementation, Classic Problems, Real-world Use Cases
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;

// ============================================================
// SECTION 1: CUSTOM NODE DEFINITION
// ============================================================

// A node is the basic building block of a linked list.
// It holds data AND a reference (pointer) to the next node.
public class SinglyNode<T>
{
    public T Data;              // The value stored in this node
    public SinglyNode<T> Next;  // Reference to the next node (null if this is the last node)

    public SinglyNode(T data)
    {
        Data = data;
        Next = null;  // When created, it points to nothing
    }

    public override string ToString() => Data.ToString();
}

// ============================================================
// SECTION 2: CUSTOM SINGLY LINKED LIST IMPLEMENTATION
// This shows HOW the built-in LinkedList<T> works internally.
// ============================================================

public class MySinglyLinkedList<T>
{
    // Head = first node in the list
    // If head is null, the list is empty
    private SinglyNode<T> head;

    // Track how many nodes are in the list
    public int Count { get; private set; }

    // --- INSERT AT FRONT (HEAD) — O(1) ---
    // O(1) because we only change 2 pointers regardless of list size.
    // This is the KEY ADVANTAGE over arrays (which would need to shift all elements).
    public void InsertAtHead(T data)
    {
        SinglyNode<T> newNode = new SinglyNode<T>(data);

        // New node's next points to current head
        // If list is empty, this sets Next = null (which is correct)
        newNode.Next = head;

        // Head is now the new node
        head = newNode;

        Count++;
    }

    // --- INSERT AT TAIL (END) — O(n) without tail pointer ---
    // We must walk the entire list to find the last node.
    // O(1) if you maintain a "tail" pointer (like C# LinkedList<T> does).
    public void InsertAtTail(T data)
    {
        SinglyNode<T> newNode = new SinglyNode<T>(data);

        // Edge case: list is empty → new node becomes the head
        if (head == null)
        {
            head = newNode;
            Count++;
            return;
        }

        // Walk to the last node (node whose Next is null)
        SinglyNode<T> current = head;
        while (current.Next != null)
        {
            current = current.Next;  // Move to the next node
        }

        // Now current is the last node — make it point to our new node
        current.Next = newNode;
        Count++;
    }

    // --- INSERT AT POSITION — O(n) ---
    // Insert a new node at position 'index' (0-based)
    public void InsertAt(int index, T data)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        // Position 0 = insert at head
        if (index == 0) { InsertAtHead(data); return; }

        SinglyNode<T> newNode = new SinglyNode<T>(data);

        // Walk to the node BEFORE the target position
        SinglyNode<T> current = head;
        for (int i = 0; i < index - 1; i++)
        {
            current = current.Next;
        }

        // Wire up: newNode.Next = node that was at position 'index'
        //          current.Next = newNode
        newNode.Next = current.Next;
        current.Next = newNode;
        Count++;
    }

    // --- DELETE BY VALUE — O(n) ---
    // Find the node with this value and remove it.
    // Key trick: keep a reference to the PREVIOUS node so we can "skip over" the deleted node.
    public bool Delete(T data)
    {
        if (head == null) return false;  // Empty list, nothing to delete

        // Special case: the node to delete IS the head
        if (EqualityComparer<T>.Default.Equals(head.Data, data))
        {
            head = head.Next;  // Skip the head → new head is the second node
            Count--;
            return true;
        }

        // Walk the list, keeping a reference to the PREVIOUS node
        SinglyNode<T> current = head;
        while (current.Next != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Next.Data, data))
            {
                // Found it at current.Next — skip over it
                current.Next = current.Next.Next;
                Count--;
                return true;
            }
            current = current.Next;
        }

        return false;  // Value not found
    }

    // --- SEARCH — O(n) ---
    // Linear search — must check every node
    public bool Contains(T data)
    {
        SinglyNode<T> current = head;
        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, data))
                return true;
            current = current.Next;
        }
        return false;
    }

    // --- REVERSE THE LIST IN-PLACE — O(n), O(1) space ---
    // Classic interview question!
    // Before: [1] -> [2] -> [3] -> [4] -> null
    // After:  [4] -> [3] -> [2] -> [1] -> null
    //
    // Technique: three pointers — prev, current, next
    public void Reverse()
    {
        SinglyNode<T> prev = null;     // starts as null (new tail's Next)
        SinglyNode<T> current = head;  // starts at head

        while (current != null)
        {
            SinglyNode<T> next = current.Next;  // 1. Save next before we overwrite it

            current.Next = prev;                 // 2. Reverse this node's pointer

            prev = current;                      // 3. Advance prev forward
            current = next;                      // 4. Advance current forward
        }

        // When the loop ends, current is null and prev is the old tail = new head
        head = prev;
    }

    // --- DETECT CYCLE — Floyd's Tortoise and Hare — O(n), O(1) space ---
    // If there's a cycle, the fast pointer will eventually catch up with the slow pointer.
    public bool HasCycle()
    {
        SinglyNode<T> slow = head;  // moves 1 step at a time (tortoise)
        SinglyNode<T> fast = head;  // moves 2 steps at a time (hare)

        while (fast != null && fast.Next != null)
        {
            slow = slow.Next;        // tortoise: 1 step
            fast = fast.Next.Next;   // hare: 2 steps

            if (slow == fast)        // They met inside the cycle!
                return true;
        }

        return false;  // fast reached the end → no cycle
    }

    // --- FIND MIDDLE NODE — O(n), O(1) space ---
    // Fast pointer moves 2x as fast as slow.
    // When fast reaches end, slow is at the middle.
    public SinglyNode<T> FindMiddle()
    {
        if (head == null) return null;

        SinglyNode<T> slow = head;  // Will be at middle when fast is at end
        SinglyNode<T> fast = head;

        while (fast != null && fast.Next != null)
        {
            slow = slow.Next;        // 1 step
            fast = fast.Next.Next;   // 2 steps
        }

        return slow;  // slow is now at the middle
    }

    // --- PRINT ALL NODES ---
    public void Print(string label = "List")
    {
        Console.Write($"{label}: ");
        SinglyNode<T> current = head;
        while (current != null)
        {
            Console.Write(current.Data);
            if (current.Next != null) Console.Write(" -> ");
            current = current.Next;
        }
        Console.WriteLine(" -> null");
    }
}

// ============================================================
// SECTION 3: DETECT CYCLE DEMO HELPER
// We manually create a cycle by setting a node's Next to an earlier node
// ============================================================
class CycleDemo
{
    public static void CreateAndDetectCycle()
    {
        // Manually create nodes (can't use MySinglyLinkedList because InsertAtTail
        // would loop forever if there's a cycle)
        var n1 = new SinglyNode<int>(1);
        var n2 = new SinglyNode<int>(2);
        var n3 = new SinglyNode<int>(3);
        var n4 = new SinglyNode<int>(4);
        var n5 = new SinglyNode<int>(5);

        // Build list: 1 -> 2 -> 3 -> 4 -> 5 -> (back to n3) [cycle!]
        n1.Next = n2;
        n2.Next = n3;
        n3.Next = n4;
        n4.Next = n5;
        n5.Next = n3;  // Creates cycle: 5 points back to 3

        // Use Floyd's algorithm
        var slow = n1;
        var fast = n1;
        bool hasCycle = false;

        while (fast != null && fast.Next != null)
        {
            slow = slow.Next;
            fast = fast.Next.Next;
            if (slow == fast) { hasCycle = true; break; }
        }

        Console.WriteLine($"  Cycle detected: {hasCycle}");  // True
    }
}

// ============================================================
// SECTION 4: C# BUILT-IN LinkedList<T> — DOUBLY LINKED LIST
// ============================================================
class BuiltInLinkedListDemo
{
    public static void Run()
    {
        Console.WriteLine("\n--- C# Built-in LinkedList<T> (Doubly Linked) ---");

        LinkedList<string> playlist = new LinkedList<string>();

        // AddLast = add to end of list — O(1) because it maintains a tail pointer
        playlist.AddLast("Song A");
        playlist.AddLast("Song B");
        playlist.AddLast("Song C");
        playlist.AddLast("Song D");

        // AddFirst = add to front — O(1)
        playlist.AddFirst("Intro Song");

        PrintLinkedList(playlist, "Initial playlist");
        // Output: Intro Song <-> Song A <-> Song B <-> Song C <-> Song D

        // Find a specific node — O(n) linear search
        LinkedListNode<string> nodeB = playlist.Find("Song B");

        // AddAfter/AddBefore — O(1) once you have the node reference
        // This is ONE MAJOR ADVANTAGE of doubly linked list over arrays
        playlist.AddAfter(nodeB, "Song B.5");    // insert between B and C
        playlist.AddBefore(nodeB, "Song A.5");   // insert between A and B

        PrintLinkedList(playlist, "After insertions");

        // Remove operations
        playlist.Remove("Intro Song");     // remove by value — O(n) to find it
        playlist.RemoveFirst();            // remove head — O(1)
        playlist.RemoveLast();             // remove tail — O(1)
        playlist.Remove(nodeB);            // remove by node reference — O(1)! (doubly linked advantage)

        PrintLinkedList(playlist, "After removals");

        // --- TRAVERSAL ---
        Console.WriteLine("\n  Forward traversal:");
        LinkedListNode<string> current = playlist.First;
        while (current != null)
        {
            Console.Write($"  [{current.Value}]");
            if (current.Next != null) Console.Write(" <-> ");
            current = current.Next;
        }
        Console.WriteLine();

        Console.WriteLine("  Backward traversal (doubly linked advantage):");
        current = playlist.Last;
        while (current != null)
        {
            Console.Write($"  [{current.Value}]");
            if (current.Previous != null) Console.Write(" <-> ");
            current = current.Previous;
        }
        Console.WriteLine();

        // Properties
        Console.WriteLine($"\n  Count: {playlist.Count}");
        Console.WriteLine($"  First: {playlist.First?.Value}");
        Console.WriteLine($"  Last: {playlist.Last?.Value}");
        Console.WriteLine($"  Contains 'Song C': {playlist.Contains("Song C")}");
    }

    private static void PrintLinkedList(LinkedList<string> list, string label)
    {
        Console.Write($"  {label}: ");
        foreach (var item in list)
            Console.Write($"[{item}] <-> ");
        Console.WriteLine("null");
    }
}

// ============================================================
// SECTION 5: LRU CACHE — Classic real-world LinkedList problem
// An LRU (Least Recently Used) Cache evicts the LEAST recently
// accessed item when it reaches capacity.
//
// Solution: Dictionary (O(1) lookup) + LinkedList (O(1) move-to-front)
// ============================================================
class LRUCache
{
    private readonly int _capacity;

    // Dictionary maps key → linked list node for O(1) lookup
    private readonly Dictionary<int, LinkedListNode<(int key, int value)>> _map;

    // LinkedList: front = most recently used, back = least recently used
    private readonly LinkedList<(int key, int value)> _list;

    public LRUCache(int capacity)
    {
        _capacity = capacity;
        _map = new Dictionary<int, LinkedListNode<(int, int)>>();
        _list = new LinkedList<(int, int)>();
    }

    // Get value by key — O(1)
    // Also marks the item as "recently used" by moving it to front
    public int Get(int key)
    {
        if (!_map.ContainsKey(key)) return -1;  // not found

        var node = _map[key];

        // Move to front = mark as most recently used
        _list.Remove(node);        // O(1) because we have node reference (doubly linked!)
        _list.AddFirst(node);      // O(1)

        return node.Value.value;
    }

    // Put key-value — O(1)
    // If key exists: update. If cache full: evict LRU item.
    public void Put(int key, int value)
    {
        if (_map.ContainsKey(key))
        {
            // Update existing: remove then re-add at front
            _list.Remove(_map[key]);
            _map.Remove(key);
        }
        else if (_list.Count == _capacity)
        {
            // Cache is full — remove least recently used (back of list)
            var lru = _list.Last;
            _list.RemoveLast();       // O(1)
            _map.Remove(lru.Value.key); // O(1)
            Console.WriteLine($"    Evicted LRU item: key={lru.Value.key}");
        }

        // Add new item to front (most recently used)
        var newNode = new LinkedListNode<(int, int)>((key, value));
        _list.AddFirst(newNode);  // O(1)
        _map[key] = newNode;      // O(1)
    }

    public void PrintState()
    {
        Console.Write("    Cache state (front=MRU, back=LRU): ");
        foreach (var item in _list)
            Console.Write($"[{item.key}:{item.value}] ");
        Console.WriteLine();
    }
}

// ============================================================
// SECTION 6: PALINDROME CHECK ON LINKED LIST
// Find middle → Reverse second half → Compare with first half
// ============================================================
class PalindromeLinkedList
{
    public static bool IsPalindrome(int[] values)
    {
        // Build a list manually
        SinglyNode<int> head = null;
        SinglyNode<int> tail = null;

        foreach (int v in values)
        {
            var node = new SinglyNode<int>(v);
            if (head == null) { head = tail = node; }
            else { tail.Next = node; tail = node; }
        }

        // Step 1: Find middle using slow/fast pointers
        SinglyNode<int> slow = head, fast = head, prev = null;
        while (fast != null && fast.Next != null)
        {
            prev = slow;
            slow = slow.Next;
            fast = fast.Next.Next;
        }

        // Step 2: Reverse second half (slow is now at middle)
        if (prev != null) prev.Next = null;  // cut the list in half
        SinglyNode<int> secondHalf = Reverse(slow);

        // Step 3: Compare first half and reversed second half
        SinglyNode<int> p1 = head, p2 = secondHalf;
        while (p1 != null && p2 != null)
        {
            if (p1.Data != p2.Data) return false;
            p1 = p1.Next;
            p2 = p2.Next;
        }
        return true;
    }

    private static SinglyNode<int> Reverse(SinglyNode<int> head)
    {
        SinglyNode<int> prev = null, current = head;
        while (current != null)
        {
            var next = current.Next;
            current.Next = prev;
            prev = current;
            current = next;
        }
        return prev;
    }
}

// ============================================================
// MAIN PROGRAM — Runs all demonstrations
// ============================================================
class LinkedListProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 8: LINKED LIST — Complete Demonstration");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Custom Singly Linked List
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Custom Singly Linked List ===");

        var myList = new MySinglyLinkedList<int>();

        // Insert at tail
        myList.InsertAtTail(10);
        myList.InsertAtTail(20);
        myList.InsertAtTail(30);
        myList.InsertAtTail(40);
        myList.Print("After InsertAtTail(10,20,30,40)");
        // 10 -> 20 -> 30 -> 40 -> null

        // Insert at head
        myList.InsertAtHead(5);
        myList.Print("After InsertAtHead(5)");
        // 5 -> 10 -> 20 -> 30 -> 40 -> null

        // Insert at position
        myList.InsertAt(2, 15);
        myList.Print("After InsertAt(2, 15)");
        // 5 -> 10 -> 15 -> 20 -> 30 -> 40 -> null

        Console.WriteLine($"  Count: {myList.Count}");
        Console.WriteLine($"  Contains 20: {myList.Contains(20)}");
        Console.WriteLine($"  Contains 99: {myList.Contains(99)}");

        // Delete
        myList.Delete(15);
        myList.Print("After Delete(15)");
        // 5 -> 10 -> 20 -> 30 -> 40 -> null

        myList.Delete(5);   // delete head
        myList.Print("After Delete(5) [head]");
        // 10 -> 20 -> 30 -> 40 -> null

        // Find middle
        var middle = myList.FindMiddle();
        Console.WriteLine($"  Middle node: {middle.Data}");  // 20 (positions 0-3, mid = 1 or 2)

        // Reverse
        myList.Reverse();
        myList.Print("After Reverse()");
        // 40 -> 30 -> 20 -> 10 -> null

        // -------------------------------------------------------
        // DEMO 2: Cycle Detection
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: Cycle Detection (Floyd's Algorithm) ===");
        Console.Write("  ");
        CycleDemo.CreateAndDetectCycle();

        // -------------------------------------------------------
        // DEMO 3: C# Built-in Doubly LinkedList<T>
        // -------------------------------------------------------
        BuiltInLinkedListDemo.Run();

        // -------------------------------------------------------
        // DEMO 4: LRU Cache using LinkedList
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 4: LRU Cache (capacity=3) ===");
        var cache = new LRUCache(3);

        cache.Put(1, 100);
        cache.Put(2, 200);
        cache.Put(3, 300);
        cache.PrintState();
        // [3:300] [2:200] [1:100]

        Console.WriteLine($"    Get key=1: {cache.Get(1)}");  // 100, moves key 1 to front
        cache.PrintState();
        // [1:100] [3:300] [2:200]  — key 1 moved to front

        cache.Put(4, 400);  // Cache full, evicts LRU (key 2 at back)
        cache.PrintState();
        // [4:400] [1:100] [3:300]  — key 2 was evicted

        Console.WriteLine($"    Get key=2: {cache.Get(2)}");  // -1 (evicted)

        // -------------------------------------------------------
        // DEMO 5: Palindrome Check
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: Palindrome Linked List ===");

        int[] pal1 = { 1, 2, 3, 2, 1 };
        int[] pal2 = { 1, 2, 3, 4, 5 };
        int[] pal3 = { 1, 2, 2, 1 };

        Console.WriteLine($"  [1,2,3,2,1] is palindrome: {PalindromeLinkedList.IsPalindrome(pal1)}");  // True
        Console.WriteLine($"  [1,2,3,4,5] is palindrome: {PalindromeLinkedList.IsPalindrome(pal2)}");  // False
        Console.WriteLine($"  [1,2,2,1]   is palindrome: {PalindromeLinkedList.IsPalindrome(pal3)}");  // True

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. LinkedList = O(1) insert/delete at head, O(n) by-index access");
        Console.WriteLine("  2. Array/List = O(1) by-index access, O(n) insert at front");
        Console.WriteLine("  3. C# LinkedList<T> is DOUBLY linked (prev + next pointers)");
        Console.WriteLine("  4. Use LinkedList for: LRU Cache, playlist, undo/redo, deque");
        Console.WriteLine("  5. Floyd's algorithm detects cycles in O(n) time O(1) space");
        Console.WriteLine("  6. Reverse a list with 3 pointers: prev, current, next");
        Console.WriteLine("======================================================");
    }
}
