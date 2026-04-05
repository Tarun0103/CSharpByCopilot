# 2. Dependency Injection (Basic)

Dependency Injection (DI) is a technique where an object receives its dependencies from the outside.

## Constructor Injection
Most common form of DI: dependencies are passed via constructor parameters.

```csharp
public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger;
    }
}
```

## Why DI is used
- Improves testability (you can inject mocks).
- Decouples concrete implementations from consumers.
- Enables swapping implementations (e.g., different logging providers).

---

## ✅ Interview Prep (Dependency Injection)
- Explain the difference between tight coupling and dependency injection.
- Show how to register services in a DI container (e.g., `Microsoft.Extensions.DependencyInjection`).
- Describe constructor injection vs property injection.
