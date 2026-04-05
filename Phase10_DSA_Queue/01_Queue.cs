// ============================================================
// PHASE 10: QUEUE (FIFO) — Complete C# Implementation
// Topics: Queue concept, Custom Queue, C# Queue<T>,
//         Circular Queue, PriorityQueue, Task Scheduling, BFS
// Run this file in a Console Application project
// ============================================================

using System;
using System.Collections.Generic;

// ============================================================
// SECTION 1: CUSTOM QUEUE — Circular Array Implementation
// FIFO = First In, First Out
// Think of a checkout line — first person in line is first served.
//
// Why circular? Without it, after many Enqueue/Dequeue operations
// the front pointer marches right and we "waste" space at the left.
// Circular array wraps around to reuse that space.
//
//  Enqueue →  [ _ | _ | 4 | 5 | 6 | _ ]  ← Dequeue
//                        ^front     ^rear
// ============================================================

public class MyCircularQueue<T>
{
    private T[] _data;      // internal circular buffer
    private int _front;     // index of the front element (for Dequeue)
    private int _rear;      // index where next element will be inserted
    private int _count;     // number of elements currently stored
    private int _capacity;

    public MyCircularQueue(int capacity = 8)
    {
        _capacity = capacity;
        _data = new T[_capacity];
        _front = 0;
        _rear = 0;
        _count = 0;
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;
    public bool IsFull => _count == _capacity;

    // --- ENQUEUE — O(1) amortized ---
    // Add element to the REAR of the queue
    public void Enqueue(T item)
    {
        if (IsFull)
        {
            // Grow the buffer — copy elements in correct (unwrapped) order
            T[] newData = new T[_capacity * 2];
            for (int i = 0; i < _count; i++)
                newData[i] = _data[(_front + i) % _capacity];

            _data = newData;
            _front = 0;
            _rear = _count;
            _capacity *= 2;
            Console.WriteLine($"  [Queue resized to {_capacity}]");
        }

        _data[_rear] = item;
        _rear = (_rear + 1) % _capacity;  // wrap around with modulo! This is what makes it circular.
        _count++;
    }

    // --- DEQUEUE — O(1) ---
    // Remove and return the FRONT element
    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty — cannot Dequeue()");

        T item = _data[_front];
        _data[_front] = default;               // clear reference (GC help)
        _front = (_front + 1) % _capacity;     // advance front pointer (circular)
        _count--;
        return item;
    }

    // --- PEEK — O(1) ---
    // Read the front element WITHOUT removing it
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty — cannot Peek()");

        return _data[_front];
    }

    public void Print(string label = "Queue")
    {
        Console.Write($"  {label} (front→rear): [");
        for (int i = 0; i < _count; i++)
        {
            Console.Write(_data[(_front + i) % _capacity]);
            if (i < _count - 1) Console.Write(", ");
        }
        Console.WriteLine("]");
    }
}

// ============================================================
// SECTION 2: TASK SCHEDULER using Queue<T>
// Real-world: CPU scheduling, print queues, web request queues,
// message queues (RabbitMQ), async job processors
// ============================================================

class Task
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int DurationMs { get; init; }

    public override string ToString() => $"Task#{Id}({Name}, {DurationMs}ms)";
}

class TaskScheduler
{
    private Queue<Task> _queue = new Queue<Task>();
    private int _nextId = 1;

    // Add a task to the queue
    public void AddTask(string name, int durationMs)
    {
        var task = new Task { Id = _nextId++, Name = name, DurationMs = durationMs };
        _queue.Enqueue(task);
        Console.WriteLine($"  Queued: {task}");
    }

    // Process all tasks in FIFO order
    public void ProcessAll()
    {
        Console.WriteLine($"\n  Processing {_queue.Count} tasks in FIFO order:");

        while (_queue.Count > 0)
        {
            Task task = _queue.Dequeue();  // get next task (first in line)
            Console.WriteLine($"  ▶ Executing {task}...");
            // Simulate work: System.Threading.Thread.Sleep(task.DurationMs);
        }

        Console.WriteLine("  All tasks completed.");
    }

    public void ShowQueue()
    {
        Console.Write($"  Queue ({_queue.Count} pending): ");
        foreach (var t in _queue) Console.Write($"[{t.Name}] ");
        Console.WriteLine();
    }
}

// ============================================================
// SECTION 3: PRIORITY QUEUE (C# 6.0+)
// Unlike a regular Queue (FIFO), a PriorityQueue dequeues the element
// with the LOWEST priority number first (min-heap under the hood).
//
// Use cases:
//   - Hospital ER triage (critical patients first)
//   - Dijkstra's shortest path algorithm
//   - OS process scheduling (higher priority tasks first)
//   - Event simulation systems
// ============================================================

class PriorityQueueDemo
{
    public static void Run()
    {
        Console.WriteLine("\n--- C# PriorityQueue<TElement, TPriority> ---");

        // PriorityQueue<element, priority> — lowest priority value = first to dequeue (min-heap)
        PriorityQueue<string, int> pq = new PriorityQueue<string, int>();

        // Enqueue: element + priority number (lower number = higher priority = dequeued first)
        pq.Enqueue("Low priority task",    priority: 10);
        pq.Enqueue("Critical bug fix",     priority: 1);   // urgent!
        pq.Enqueue("Medium priority task", priority: 5);
        pq.Enqueue("Another critical",     priority: 1);   // same priority as first
        pq.Enqueue("Nice to have",         priority: 20);

        Console.WriteLine($"  Enqueued 5 items. Count: {pq.Count}");
        Console.WriteLine("  Dequeuing (lowest priority number = first):");

        while (pq.Count > 0)
        {
            pq.TryDequeue(out string element, out int priority);
            Console.WriteLine($"  [{priority:D2}] {element}");
        }
        // Output order: priority 1, 1, 5, 10, 20

        // --- Hospital Triage Example ---
        Console.WriteLine("\n  Hospital Triage Queue:");

        var triage = new PriorityQueue<string, int>();

        triage.Enqueue("Broken arm",          priority: 3);  // urgent
        triage.Enqueue("Chest pain",          priority: 1);  // critical
        triage.Enqueue("Minor cut",           priority: 5);  // non-urgent
        triage.Enqueue("Difficulty breathing",priority: 1);  // critical
        triage.Enqueue("Fever",               priority: 4);  // semi-urgent

        Console.WriteLine("  Treating patients in triage order:");
        while (triage.Count > 0)
        {
            triage.TryDequeue(out string patient, out int severity);
            string urgency = severity == 1 ? "CRITICAL" : severity <= 2 ? "Urgent" : "Normal";
            Console.WriteLine($"  [{urgency}] Treating: {patient}");
        }
    }
}

// ============================================================
// SECTION 4: BREADTH-FIRST SEARCH (BFS) using Queue
// BFS traverses a graph/tree level by level.
// Level 0: root  → Level 1: root's children → Level 2: their children ...
// Queue ensures we process nodes in order of their distance from the start.
// ============================================================

class BfsDemo
{
    // Simple graph as adjacency list
    // Key = node name, Value = list of neighboring nodes
    private Dictionary<string, List<string>> graph;

    public BfsDemo()
    {
        graph = new Dictionary<string, List<string>>
        {
            ["A"] = new List<string> { "B", "C" },
            ["B"] = new List<string> { "A", "D", "E" },
            ["C"] = new List<string> { "A", "F" },
            ["D"] = new List<string> { "B" },
            ["E"] = new List<string> { "B", "F" },
            ["F"] = new List<string> { "C", "E" }
        };
        //
        // Graph looks like:
        //      A
        //     / \
        //    B   C
        //   / \   \
        //  D   E - F
    }

    public void BFS(string start)
    {
        Queue<string> queue = new Queue<string>();     // Nodes to process next
        HashSet<string> visited = new HashSet<string>(); // Nodes already seen

        queue.Enqueue(start);     // Start with the source node
        visited.Add(start);

        Console.Write($"  BFS from '{start}': ");

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();  // process the OLDEST queued node (FIFO!)
            Console.Write(node + " ");

            // Add all unvisited neighbors to the queue
            foreach (string neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);  // will be processed after all current-level nodes
                }
            }
        }

        Console.WriteLine();
    }

    // Find shortest path using BFS (works because BFS explores level by level)
    public List<string> ShortestPath(string start, string end)
    {
        if (start == end) return new List<string> { start };

        Queue<string> queue = new Queue<string>();
        HashSet<string> visited = new HashSet<string>();
        Dictionary<string, string> parent = new Dictionary<string, string>(); // track path

        queue.Enqueue(start);
        visited.Add(start);
        parent[start] = null;

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();

            foreach (string neighbor in graph[node])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    parent[neighbor] = node;  // record how we got here
                    queue.Enqueue(neighbor);

                    if (neighbor == end)  // found destination!
                    {
                        // Reconstruct path by following parent pointers back
                        var path = new List<string>();
                        string current = end;
                        while (current != null)
                        {
                            path.Insert(0, current);     // prepend
                            current = parent[current];
                        }
                        return path;
                    }
                }
            }
        }

        return null;  // no path found
    }
}

// ============================================================
// SECTION 5: SLIDING WINDOW MAXIMUM using Deque (Double-Ended Queue)
// Given array and window size k, find the max in each window.
// Example: [1,3,-1,-3,5,3,6,7], k=3 → [3,3,5,5,6,7]
//
// Key insight: Use a LinkedList as a deque (add/remove from BOTH ends)
// Store indices of potentially  maximum elements.
// Remove from front: indices outside current window
// Remove from back: elements smaller than current (they can never be max)
// ============================================================
class SlidingWindowMax
{
    public static int[] GetMaximums(int[] nums, int k)
    {
        int n = nums.Length;
        if (n == 0 || k == 0) return new int[0];

        int[] result = new int[n - k + 1];
        LinkedList<int> deque = new LinkedList<int>(); // stores indices (acts as deque)

        for (int i = 0; i < n; i++)
        {
            // Remove elements outside the current window from FRONT
            while (deque.Count > 0 && deque.First.Value < i - k + 1)
                deque.RemoveFirst();

            // Remove smaller elements from BACK (they can't be max in any future window)
            while (deque.Count > 0 && nums[deque.Last.Value] < nums[i])
                deque.RemoveLast();

            deque.AddLast(i);  // add current index from back

            // Window is full — record the max (front of deque = index of max)
            if (i >= k - 1)
                result[i - k + 1] = nums[deque.First.Value];
        }

        return result;
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class QueueProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 10: QUEUE (FIFO) — Complete Demonstration");
        Console.WriteLine("======================================================");

        // -------------------------------------------------------
        // DEMO 1: Custom Circular Queue
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 1: Custom Circular Queue ===");

        var circQueue = new MyCircularQueue<int>(4);  // capacity = 4

        circQueue.Enqueue(10);
        circQueue.Enqueue(20);
        circQueue.Enqueue(30);
        circQueue.Print("After Enqueue(10,20,30)");

        Console.WriteLine($"  Peek: {circQueue.Peek()}");       // 10
        Console.WriteLine($"  Dequeue: {circQueue.Dequeue()}"); // 10
        Console.WriteLine($"  Dequeue: {circQueue.Dequeue()}"); // 20
        circQueue.Print("After 2 Dequeues");

        circQueue.Enqueue(40);
        circQueue.Enqueue(50);
        circQueue.Enqueue(60);  // This triggers wraparound (circular behavior)
        circQueue.Print("After 3 more Enqueues (circular wraparound)");

        // -------------------------------------------------------
        // DEMO 2: C# Built-in Queue<T>
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 2: C# Built-in Queue<T> ===");

        Queue<string> builtIn = new Queue<string>();

        builtIn.Enqueue("Customer 1");
        builtIn.Enqueue("Customer 2");
        builtIn.Enqueue("Customer 3");

        Console.WriteLine($"  Count: {builtIn.Count}");
        Console.WriteLine($"  Peek (first in line): {builtIn.Peek()}");   // Customer 1

        while (builtIn.Count > 0)
        {
            string customer = builtIn.Dequeue();
            Console.WriteLine($"  Serving: {customer}");
        }

        // TryDequeue — safe version
        Console.WriteLine($"  TryDequeue on empty: {builtIn.TryDequeue(out _)}");  // False

        // -------------------------------------------------------
        // DEMO 3: Task Scheduler
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 3: Task Scheduler ===");

        var scheduler = new TaskScheduler();
        scheduler.AddTask("Database backup", 500);
        scheduler.AddTask("Send emails",     200);
        scheduler.AddTask("Generate report", 300);
        scheduler.AddTask("Clean temp files",100);

        scheduler.ShowQueue();
        scheduler.ProcessAll();

        // -------------------------------------------------------
        // DEMO 4: Priority Queue
        // -------------------------------------------------------
        PriorityQueueDemo.Run();

        // -------------------------------------------------------
        // DEMO 5: BFS Traversal and Shortest Path
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 5: BFS Graph Traversal ===");

        var bfs = new BfsDemo();
        bfs.BFS("A");
        // A B C D E F (level-by-level order)

        var path = bfs.ShortestPath("D", "F");
        Console.WriteLine($"  Shortest path D→F: {string.Join(" → ", path)}");
        // D → B → E → F  (3 hops)

        // -------------------------------------------------------
        // DEMO 6: Sliding Window Maximum
        // -------------------------------------------------------
        Console.WriteLine("\n=== DEMO 6: Sliding Window Maximum ===");

        int[] nums = { 1, 3, -1, -3, 5, 3, 6, 7 };
        int k = 3;
        int[] maxes = SlidingWindowMax.GetMaximums(nums, k);

        Console.Write($"  Array: [{string.Join(", ", nums)}]  k={k}");
        Console.WriteLine();
        Console.Write($"  Max in each window of size {k}: [{string.Join(", ", maxes)}]");
        Console.WriteLine();
        // [3, 3, 5, 5, 6, 7]

        // -------------------------------------------------------
        // SUMMARY
        // -------------------------------------------------------
        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. Queue = FIFO (First In, First Out)");
        Console.WriteLine("  2. Core ops: Enqueue O(1), Dequeue O(1), Peek O(1)");
        Console.WriteLine("  3. Circular queue avoids wasting space");
        Console.WriteLine("  4. PriorityQueue = min-heap, lowest priority value first");
        Console.WriteLine("  5. BFS uses a Queue to explore level by level");
        Console.WriteLine("  6. Use for: task scheduling, BFS, message queues, print spoolers");
        Console.WriteLine("======================================================");
    }
}
