# 5. LINQ Queries

Practice common LINQ query patterns.

## Common query patterns
- Filtering: `Where`
- Projection: `Select`
- Sorting: `OrderBy`, `ThenBy`
- Grouping: `GroupBy`
- Joining: `Join`, `GroupJoin`

### Example: Customer orders

```csharp
var result = customers
    .Where(c => c.IsActive)
    .Select(c => new { c.Name, Total = c.Orders.Sum(o => o.Total) });
```

---

## ✅ Interview Prep (LINQ Queries)
- Know how to translate between query syntax and method syntax.
- Be able to write a `GroupBy` query and access the key and items.
- Explain deferred execution and when it is triggered.
