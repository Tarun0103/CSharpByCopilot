# 2. Memory Management

C# is managed: the runtime (CLR) handles memory allocation and garbage collection.

## Stack vs Heap

- **Stack**: stores value types and reference variables. Fast, automatically freed when method returns.
- **Heap**: stores objects (reference types). Managed by the garbage collector.

## Garbage Collection (GC)

- The GC periodically scans the heap, identifies unreachable objects, and reclaims memory.
- Objects are organized into **generations** (0, 1, 2) to optimize for short-lived objects.

### IDisposable & using
- Implement `IDisposable` to release unmanaged resources (file handles, database connections).
- Use `using` statement to ensure `Dispose` is called even if exceptions occur.

```csharp
using var stream = File.OpenRead("file.txt");
// stream.Dispose() is called automatically.
```

---

## ✅ Interview Prep (Memory Management)
- Explain when objects are allocated on the heap vs stack.
- Talk about GC generations and why Gen 0 is collected more frequently.
- Explain `IDisposable`, `using`, and when to implement finalizers.
