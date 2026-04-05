# 2. Delegates & Events

## Delegates
A delegate is a type-safe function pointer.

```csharp
public delegate void Logger(string message);
```

You can assign methods to delegates and invoke them.

### Multicast delegates
A delegate can reference multiple methods. When invoked, it calls each method in order.

## Events
Events are a publisher/subscriber pattern built on delegates.

```csharp
public event EventHandler<MyEventArgs> DataReceived;
```

Subscribers can attach handlers using `+=` and detach with `-=`.

👉 **Interview focus**: Real use case of delegates.

- Example: UI button click handlers, logging pipelines, pipeline behavior in middleware.

---

## Detailed explanation & execution flow — `02_Delegates_Events.cs`

This file demonstrates three related concepts: simple delegate usage, multicast delegates, and events (publisher/subscriber).

1. Simple delegate usage
	 - Define a delegate type `Logger` that represents `void(string)`.
	 - Assign a method (e.g., `ConsoleLogger`) to a delegate variable and invoke it.
	 - Use when you need to pass behavior around as a first-class value (callbacks, strategies).

2. Multicast delegates
	 - Add multiple target methods to a delegate instance using `+=`.
	 - Invoking the multicast delegate calls each target in order. Useful for broadcasting to multiple subscribers (e.g., logging to console + file).

3. Events (publisher/subscriber)
	 - Declare an event on the publisher type to expose subscription points.
	 - Subscribers attach handlers with `+=` and detach with `-=`.
	 - Only the publisher should raise the event; consumers cannot invoke it.

## Use cases — where you'd use delegates and events

- Callbacks and continuation: pass a delegate to a method to invoke a callback when work completes.
- Strategy pattern: inject behavior (sorting, comparison, retry logic) via delegates.
- Multicast notifications: logging frameworks that write to multiple sinks.
- UI frameworks: event handlers for user interactions (click, change, keypress).
- Decoupled systems: events let producers and consumers evolve independently (publish/subscribe).

## Interview-style Q&A and suggested answers

- Q: "What's the difference between a delegate and an event?"
	- A: "A delegate is a type that references methods. An event is a language-level construct built on delegates that exposes subscription (`+=`/`-=`) but restricts invocation so only the declaring type can raise it. Events therefore enforce a publisher/subscriber discipline."

- Q: "When would you use a multicast delegate vs an event?"
	- A: "Use multicast delegates if you need to maintain a list of handlers and invoke them directly. Use events when you want to expose subscription to external code but keep raising controlled by the owner. Events are the safer, more idiomatic choice for public APIs."

- Q: "How do you prevent exceptions in one subscriber from preventing others from running?"
	- A: "When invoking the invocation list manually, iterate over `GetInvocationList()` and invoke each target inside a try/catch so one failure doesn't stop others. For events, use the same approach if you need robust invocation and can accept per-subscriber handling."

- Q: "What are common pitfalls with events?"
	- A: "Not unsubscribing from long-lived publishers can cause memory leaks. Raising events on non-UI threads without synchronization can cause race conditions. Also exposing EventHandler<T> without null checks can raise exceptions if no handlers are attached."

- Q: "How would you test code that uses events?"
	- A: "Attach test handlers that set flags or capture arguments. Raise events and assert handlers were invoked the expected number of times with expected data. Use mocking for publishers in unit tests."

## Example talking points (short bullets you can say in interviews)

- "Delegates are type-safe function pointers; events provide a controlled subscription model." 
- "Multicast delegates are useful for broadcasting; events are preferred for public APIs." 
- "Always unsubscribe from events when subscribers have shorter lifetimes than publishers to avoid memory leaks." 
- "If subscriber exceptions are a concern, invoke each handler individually with try/catch."

## Advanced considerations

- Threading: Raising events from background threads may require marshaling to the UI thread (e.g., using SynchronizationContext or Dispatcher). 
- Performance: delegates are lightweight, but invoking many handlers frequently can be a hotspot — consider batching or dedicated pipelines.
- Weak event patterns: use weak references for subscribers when you cannot guarantee unsubscription to prevent memory leaks in long-running processes.

---

## Quick practice tasks to propose in an interview

- Implement a resilient event invoker that isolates subscriber exceptions.
- Add a `Logger` pipeline that writes to console and a file using multicast delegates.
- Replace custom delegate types with `Action<T>`/`Func<T>` where appropriate and explain trade-offs.

---

## ✅ Interview Prep (Delegates & Events)
- Explain the difference between a delegate type and an event.
- Describe how multicast delegates work.
- Show a practical example (e.g., a simple event aggregator or notification system).
