# 2. Encapsulation

Encapsulation is about bundling data and behavior together, and restricting access to implementation details.

## Access Modifiers

| Modifier | Accessible From | Typical Use |
|---------|------------------|-------------|
| `public` | anywhere | Public API |
| `private` | within the same class | Implementation detail |
| `protected` | same class or derived class | Extensibility point |
| `internal` | same assembly | Library-internal APIs |
| `protected internal` | same assembly *or* derived class | Broad but controlled access |

### Why encapsulation matters in real projects
- Prevents external code from depending on internal state.
- Makes it safer to refactor without breaking consumers.
- Helps enforce invariants and validation.

Example: exposing a read-only collection to consumers while keeping internal mutability.

```csharp
private readonly List<string> _items = new();
public IReadOnlyList<string> Items => _items;
```

---

## ✅ Interview Prep (Encapsulation)
- Explain how access modifiers help with encapsulation.
- Give a real-world example where `private` fields + public properties protect invariants.
- Explain why you might use `internal` in a library.

---

## Detailed execution flow — `02_Encapsulation.cs`

This walkthrough explains how the sample program executes and why encapsulation matters.

1. Program start: `Main()` is the entry point.
	 - The program constructs a `BankAccount` instance with `new BankAccount("Alice", 100m)`.
	 - The constructor validates inputs (owner required, non-negative initial balance) and records an "Account opened" transaction.

2. Mutating state via methods:
	 - `Deposit(50m)` is invoked. The method validates the amount, updates the private `_balance`, and appends a transaction note to `_transactions`.
	 - `Withdraw(30m)` is invoked. The method validates amount and checks available balance before mutating `_balance` and recording the withdrawal.

3. Reading state through read-only members:
	 - `Balance` returns the current balance but does not allow direct assignment.
	 - `Transactions` exposes `IReadOnlyList<string>` so callers can iterate history but cannot change it.

4. Additional notes:
	 - `AddTransaction(string note)` allows callers to append contextual notes without giving them direct access to the list object.
	 - All mutations happen through methods on the type which is the core idea of encapsulation: the type controls its own state and enforces invariants.

## Interview-style conceptual scenarios and suggested answers

- Scenario: "Why expose transactions as `IReadOnlyList<string>` instead of `List<string>`?"
	- Suggested answer: "Exposing `IReadOnlyList<T>` prevents callers from adding or removing items directly, which could break invariants or corrupt internal state. The type keeps control and records transactions consistently."

- Scenario: "Where should validation live — in setters or in methods like `Deposit`/`Withdraw`?"
	- Suggested answer: "For operations that have semantic meaning and involve multiple checks or side-effects, use methods. Methods allow atomic validation and related actions (like recording transactions). Setters are fine for simple property assignment but can encourage partially-initialized or inconsistent state."

- Scenario: "How would you make `BankAccount` thread-safe?"
	- Suggested answer: "Introduce a lock around mutable operations (`Deposit`, `Withdraw`, `AddTransaction`) or use atomic/concurrent types. Consider performance trade-offs and whether operations must be transactional."

- Scenario: "What are common pitfalls when exposing internal collections?"
	- Suggested answer bullets:
		- Returning the collection object directly allows external mutation.
		- Returning a shallow copy hides changes but can have performance costs.
		- Prefer `IReadOnlyList<T>` or `AsReadOnly()` for safer APIs.

## Pitfalls & Best Practices

- Avoid exposing mutable collections or fields publicly.
- Prefer method-based APIs for operations that need validation, logging, or multiple state changes.
- Keep constructors responsible for creating valid objects (validate arguments and set up any essential invariant state).
- Document whether your API is thread-safe. If not, state that consumers must synchronize.

## Quick checklist to discuss during interviews

- **Explain encapsulation**: private fields + public methods/properties.
- **Explain invariants**: balance must never be directly assigned to a negative value.
- **Show sample variations**: validate in constructor; use `IReadOnlyList<T>`; demonstrate a defensive copy if needed.
- **Mention thread-safety**: note that the sample is not thread-safe and describe locking strategies.

## Example follow-up tasks you can propose

- Add thread-safety (use `lock` or `Interlocked` operations).
- Implement a `TryWithdraw` pattern that returns a bool instead of throwing exceptions for expected conditions.
- Add unit tests to verify validation and transaction recording (e.g., deposit/withdraw sequences and edge cases).

---

Keep this guide handy when discussing `02_Encapsulation.cs` during interviews: it frames the code as a clear demonstration of controlled mutation, validation, and safe API design.
