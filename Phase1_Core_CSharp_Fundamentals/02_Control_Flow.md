# 2. Control Flow

Control flow structures determine the order in which statements execute.

## if / else

```csharp
if (condition)
{
    // executed when condition is true
}
else if (otherCondition)
{
    // executed when otherCondition is true
}
else
{
    // executed when none of the above are true
}
```

## switch (modern switch expressions)

Modern C# supports **switch expressions** and **pattern matching**.

```csharp
int score = 85;
string grade = score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _ => "F"
};
```

## Loops

### for
Useful when you know the number of iterations.

### while
Useful when the number of iterations is not known ahead of time.

### foreach
Iterates over collections/arrays without needing an index.

👉 **Interview focus**: When to use `foreach` vs `for`.

- Use `foreach` when you only need to read values from a collection and want cleaner, safer code.
- Use `for` when you need an index, need to modify elements in place, or need to skip items.

---

## ✅ Interview Prep Checklist (Control Flow)
- Explain how `switch` expressions differ from classic `switch` statements.
- Describe the difference between `for`, `while`, and `foreach`.
- Know when to prefer `foreach` for readability and safety.
- Show an example where `for` is required (index manipulation).
