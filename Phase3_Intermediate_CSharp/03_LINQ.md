# 3. LINQ (Language Integrated Query)

LINQ provides a consistent query syntax for in-memory collections and data sources.

## Common Operators

- `Select` - projection
- `Where` - filtering
- `OrderBy` / `OrderByDescending` - sorting
- `GroupBy` - grouping

### Example

```csharp
var products = new[]
{
    new { Name = "Apple", Price = 1.0 },
    new { Name = "Banana", Price = 0.5 },
};

var cheap = products
    .Where(p => p.Price < 1)
    .OrderBy(p => p.Name)
    .Select(p => p.Name);
```

## Deferred Execution

LINQ queries are typically not executed until you enumerate them (e.g., with `foreach` or `ToList()`).

```csharp
var query = items.Where(x => x > 0);
// execution happens here:
var list = query.ToList();
```

👉 **Interview focus**: Difference between `IEnumerable<T>` vs `IQueryable<T>`.

- `IEnumerable<T>`: Queries executed in-memory (LINQ to Objects). Deferred execution but in-process.
- `IQueryable<T>`: Can translate expression trees to other query languages (e.g., SQL). Execution is deferred until enumeration.

---

## ✅ Interview Prep (LINQ)
- Know when query execution happens and how `ToList`, `ToArray`, `First`, `Count` trigger it.
- Explain `GroupBy` and its typical use cases.
- Be able to compare LINQ query syntax (`from ... in ... select`) vs method syntax.

---

## Concepts checklist — covered?

- Deferred execution: covered. Remember: most LINQ operators are deferred until enumeration.
- Immediate execution triggers: `ToList()`, `ToArray()`, `First()`, `FirstOrDefault()`, `Count()`, `Sum()`, etc.
- Query translation: `IQueryable<T>` vs `IEnumerable<T>` — provider translates expression trees.
- Grouping and buffering: `GroupBy` may buffer and increase memory usage.
- Exception cases: modifying a collection while enumerating can throw `InvalidOperationException`.

## Interview Q&A, edge cases & suggested answers

- Q: "When does LINQ execute a query?"
    - A: "Most operators are deferred; execution happens on enumeration. Methods like `ToList`, `ToArray`, `First`, `Count`, and aggregation operators force immediate execution."

- Q: "What's the difference between `IEnumerable<T>` and `IQueryable<T>`?"
    - A: "`IEnumerable<T>` executes in-memory using delegates; `IQueryable<T>` builds expression trees that providers (like EF) translate to other query languages such as SQL. Use `IQueryable` when you want server-side evaluation."

- Q: "What happens if you modify a collection while iterating a LINQ query?"
    - A: "You may encounter `InvalidOperationException` because many collections throw when modified during enumeration. For LINQ queries based on snapshots (e.g., `ToList()`), modifications after materialization won't affect the snapshot."

- Q: "Performance pitfalls with LINQ?"
    - A: "Repeatedly enumerating a deferred query can re-execute work; materialize results with `ToList()` if you need to reuse them. Avoid complex expressions inside loops; prefer projecting once. Watch out for `GroupBy` and `OrderBy` which can allocate and sort large sequences."

- Q: "When should you prefer query syntax vs method syntax?"
    - A: "Mostly a style choice. Query syntax can be clearer for complex joins and query-like flows; method syntax is necessary for extension methods and some operators."

## Edge cases & exceptions to discuss

- Modifying source during enumeration → `InvalidOperationException`.
- Null sources: many LINQ methods will throw `ArgumentNullException` if source is null; validate inputs or use null-coalescing.
- Deferred exceptions: errors in lambdas (e.g., division by zero) may be thrown only when the query is enumerated.

## Pros / Cons / Use cases

- Pros:
    - Declarative, composable queries.
    - Expressive transformations with fewer lines of code.
    - Provider model enables translation to remote queries (IQueryable).

- Cons:
    - Can be less obvious performance characteristics (deferred execution, multiple enumerations).
    - Some queries allocate intermediate collections.

- Common use cases:
    - Transforming in-memory collections, filtering, grouping, projection.
    - Query translation for ORMs (server-side filtering and projection).
    - Building pipelines for ETL and data processing.

## Quick practice tasks

- Show a deferred query and then materialize it with `ToList()` to demonstrate difference.
- Convert a LINQ query to SQL via `IQueryable` in EF and show generated SQL.
- Demonstrate safe enumeration by copying to a list before modifying the source.
