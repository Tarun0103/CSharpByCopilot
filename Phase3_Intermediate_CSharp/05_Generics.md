# 5. Generics

Generics enable type-safe code that works with any data type.

## Generic classes

```csharp
public class Box<T>
{
    public T Value { get; set; }
}
```

## Generic methods

```csharp
public static T Echo<T>(T value) => value;
```

## Constraints

- `where T : class` (reference type)
- `where T : struct` (value type)
- `where T : new()` (default constructor)
- `where T : BaseClass` (inherits BaseClass)
- `where T : IInterface` (implements interface)

### Why generics improve performance
- Avoids boxing/unboxing for value types.
- Reduces runtime casts.
- Enables high-performance data structures.

---

## ✅ Interview Prep (Generics)
- Explain why `List<int>` is better than `ArrayList`.
- Show a generic repository or utility method.
- Demonstrate constraints and why they matter.

---

## Concepts checklist — covered?

- Generic types and methods: `class Box<T>`, `T Echo<T>(T value)`.
- Constraints: `where T : class`, `struct`, `new()`, base types, interfaces.
- Variance: covariance and contravariance on interfaces/delegates.
- Performance: generics avoid boxing/unboxing for value types.

## Interview Q&A, edge cases & suggested answers

- Q: "Why prefer `List<int>` over `ArrayList`?"
    - A: "`List<int>` is type-safe at compile time and avoids boxing/unboxing for value types, giving better performance and safety. `ArrayList` stores objects and requires casts."

- Q: "What is the `new()` constraint used for?"
    - A: "It allows code inside the generic type to create instances using `new T()`. Useful for factories or repositories, but restricts types to those with a public parameterless constructor."

- Q: "Explain variance in generics."
    - A: "Covariance (out) allows `IEnumerable<Derived>` to be used where `IEnumerable<Base>` is expected. Contravariance (in) applies to consumers like `IComparer<Base>` that can accept `Derived`. Mutable collections are invariant."

## Edge cases & exceptions

- Constraints restrict usable types; `new()` excludes types without parameterless constructors.
- Generic code that uses reflection or `Activator.CreateInstance` may still require runtime checks and can fail if constraints aren't met.

## Pros / Cons / Use cases

- Pros:
    - Compile-time type safety, no runtime casts.
    - Better performance for value types (no boxing).

- Cons:
    - Can add complexity with variance and constraints.
    - API surface may require more type parameters or overloads in some cases.

- Use cases:
    - Collections, repositories, caches, algorithms that operate on many types.
    - Utilities like `TryGetValue<T>` or `Parse<T>` with constraints.

## Quick practice tasks

- Implement a generic `Cache<TKey, TValue>` with eviction.
- Demonstrate covariance/contravariance with delegates and interfaces.
