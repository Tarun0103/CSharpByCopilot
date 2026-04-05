# 1. Async Programming

Async programming helps keep applications responsive by not blocking threads while waiting for I/O.

## async / await

- Mark a method `async` and return `Task`/`Task<T>` (or `ValueTask<T>`).
- Use `await` to asynchronously wait for a task to complete.

```csharp
public async Task<int> FetchAsync()
{
    await Task.Delay(1000);
    return 42;
}
```

## Task
- Represents an asynchronous operation.
- `Task` for void-like operations, `Task<T>` for results.

## Thread vs Task
- `Thread`: OS-level thread; expensive to create.
- `Task`: lightweight abstraction; uses thread pool.

### Deadlocks (basic understanding)
- Common in UI apps: calling `Task.Result` or `.Wait()` on the UI thread.
- Avoid by using `await` all the way (async/await all the way).

👉 **Interview focus**: Difference between synchronous vs asynchronous.

- Synchronous blocks the current thread until work is done.
- Asynchronous returns control to the caller while work proceeds.

---

## ✅ Interview Prep (Async)
- Explain what `async`/`await` do and why they are not magic threads.
- Show a scenario where synchronous blocking can deadlock.
- Understand `ConfigureAwait(false)` for library code.
