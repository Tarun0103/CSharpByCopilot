# 3. Design Patterns (Important)

## Singleton
Ensures only one instance of a class exists.

### Example

```csharp
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    public static Logger Instance => _instance.Value;
    private Logger() { }
}
```

## Factory
Creates objects without exposing the creation logic.

### Example

```csharp
public interface IShape { }
public class Circle : IShape { }
public class Square : IShape { }

public static class ShapeFactory
{
    public static IShape Create(string type) => type switch
    {
        "circle" => new Circle(),
        "square" => new Square(),
        _ => throw new InvalidOperationException()
    };
}
```

## Repository
Encapsulates data access logic.

### Example

```csharp
public interface IRepository<T>
{
    void Add(T item);
    T Get(int id);
    IEnumerable<T> GetAll();
}
```

---

## ✅ Interview Prep (Design Patterns)
- Explain why Singleton is useful and what problems it can cause (global state, testability).
- Describe how Factory helps decouple client code from concrete implementations.
- Show how Repository abstracts the persistence layer and aligns with SOLID principles.
