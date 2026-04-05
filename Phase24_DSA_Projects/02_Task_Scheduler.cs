// ============================================================
// PHASE 24 — PROJECT 2: TASK SCHEDULER (Queue + PriorityQueue)
// Demonstrates: Queue for regular scheduling,
//               PriorityQueue for priority-based dispatch
// Features: Add tasks, execute FIFO or by priority, statistics
// ============================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

enum TaskStatus { Pending, InProgress, Done }
enum TaskPriority { Low = 1, Normal = 2, High = 3, Critical = 4 }

class ScheduledTask
{
    public int Id { get; }
    public string Name { get; }
    public TaskPriority Priority { get; }
    public int DurationMs { get; }           // simulated work duration
    public DateTime SubmittedAt { get; }
    public TaskStatus Status { get; set; }

    public ScheduledTask(int id, string name, TaskPriority priority, int durationMs = 100)
    {
        Id = id; Name = name; Priority = priority;
        DurationMs = durationMs;
        SubmittedAt = DateTime.Now;
        Status = TaskStatus.Pending;
    }

    public override string ToString() =>
        $"[{Priority,8}] Task#{Id:D3} '{Name}' ({DurationMs}ms)";
}

// ============================================================
// FIFO Queue Scheduler — tasks executed in submission order
// Used for: print queues, HTTP request queues, message queues
// ============================================================

class FIFOTaskScheduler
{
    private Queue<ScheduledTask> _queue = new Queue<ScheduledTask>();
    private int _nextId = 1;
    private List<ScheduledTask> _completed = new List<ScheduledTask>();

    public void Submit(string name, TaskPriority priority = TaskPriority.Normal, int durationMs = 100)
    {
        var task = new ScheduledTask(_nextId++, name, priority, durationMs);
        _queue.Enqueue(task);
        Console.WriteLine($"  Submitted: {task}");
    }

    public void ProcessAll()
    {
        Console.WriteLine($"\n  Processing {_queue.Count} tasks in FIFO order...");
        var sw = Stopwatch.StartNew();

        while (_queue.TryDequeue(out ScheduledTask task))
        {
            task.Status = TaskStatus.InProgress;
            Console.WriteLine($"    Executing: {task}");
            System.Threading.Thread.Sleep(50);    // simulate work (shortened for demo)
            task.Status = TaskStatus.Done;
            _completed.Add(task);
        }

        sw.Stop();
        Console.WriteLine($"  Done! All {_completed.Count} tasks completed in {sw.ElapsedMilliseconds}ms");
    }
}

// ============================================================
// Priority Queue Scheduler — higher priority tasks run first
// Used for: OS process scheduling, hospital triage, job queues
// ============================================================

class PriorityTaskQueueScheduler
{
    // PriorityQueue<item, priority>: C# min-heap by default
    // Negate priority value to simulate MAX-heap (Critical=4 → -4, runs first)
    private PriorityQueue<ScheduledTask, int> _queue = new PriorityQueue<ScheduledTask, int>();
    private int _nextId = 1;

    public void Submit(string name, TaskPriority priority, int durationMs = 100)
    {
        var task = new ScheduledTask(_nextId++, name, priority, durationMs);
        int negPriority = -(int)priority;   // negate: highest priority = smallest value in min-heap
        _queue.Enqueue(task, negPriority);
        Console.WriteLine($"  Submitted: {task}");
    }

    public void ProcessAll()
    {
        Console.WriteLine($"\n  Processing by PRIORITY (Critical first, Low last)...");

        while (_queue.TryDequeue(out ScheduledTask task, out _))
        {
            Console.WriteLine($"    Executing: {task}");
            System.Threading.Thread.Sleep(20);  // simulate work
        }

        Console.WriteLine("  All priority tasks done!");
    }
}

class TaskSchedulerProject
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PROJECT 2: TASK SCHEDULER (Queue + PriorityQueue)");
        Console.WriteLine("======================================================");

        Console.WriteLine("\n--- FIFO Scheduler (Queue<T>) ---");
        var fifoScheduler = new FIFOTaskScheduler();
        fifoScheduler.Submit("Send Welcome Email",    TaskPriority.Low);
        fifoScheduler.Submit("Process Payment",       TaskPriority.Critical);
        fifoScheduler.Submit("Generate Report",       TaskPriority.Normal);
        fifoScheduler.Submit("Cleanup Temp Files",    TaskPriority.Low);
        fifoScheduler.Submit("Update User Profile",   TaskPriority.Normal);
        fifoScheduler.ProcessAll();  // executes in order submitted

        Console.WriteLine("\n--- Priority Scheduler (PriorityQueue<T,P>) ---");
        var prioScheduler = new PriorityTaskQueueScheduler();
        prioScheduler.Submit("Low: Cleanup Logs",         TaskPriority.Low);
        prioScheduler.Submit("Critical: Security Alert",  TaskPriority.Critical);
        prioScheduler.Submit("Normal: Send Newsletter",   TaskPriority.Normal);
        prioScheduler.Submit("High: Deploy Hotfix",       TaskPriority.High);
        prioScheduler.Submit("Critical: Database Backup", TaskPriority.Critical);
        prioScheduler.Submit("Low: Archive Old Records",  TaskPriority.Low);
        prioScheduler.ProcessAll();  // executes Critical > High > Normal > Low

        Console.WriteLine("\n--- Why Queue vs PriorityQueue? ---");
        Console.WriteLine("  Queue:          Fair (FIFO) — every task waits its turn");
        Console.WriteLine("  PriorityQueue:  Efficient — urgent tasks skip the queue");
        Console.WriteLine("  Real-world: OS scheduler, hospital triage, network packet routing");
        Console.WriteLine("  Time complexity: Queue add/remove O(1), PQ add/remove O(log n)");
    }
}
