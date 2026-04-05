# 1. Exception Handling

Exception handling allows you to gracefully handle errors at runtime.

## try / catch / finally

```csharp
try
{
    // risky code
}
catch (SpecificException ex)
{
    // handle
}
catch (Exception ex)
{
    // fallback
}
finally
{
    // runs always
}
```

## Custom exceptions
Create custom exception types for specific error conditions.

```csharp
public class MyAppException : Exception
{
    public MyAppException(string message) : base(message) { }
}
```

👉 **Interview focus**: Difference between `throw` vs `throw ex`.

- `throw;` preserves the original stack trace.
- `throw ex;` resets the stack trace to this line (loses original context).

---

## ✅ Interview Prep (Exception Handling)
- When to use exceptions vs return values.
- How `finally` guarantees cleanup.
- Why you generally should not catch `System.Exception` unless you rethrow.
- How to create and use custom exception types.
