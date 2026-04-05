# 5. Abstraction

Abstraction lets you define **what** something does, not **how** it does it.

## Abstract classes
- Can contain implementation and abstract members.
- Cannot be instantiated.

```csharp
public abstract class Shape
{
    public abstract double Area();
    public virtual void Describe() => Console.WriteLine("A shape.");
}
```

## Interfaces
- Define a contract; no implementation (until default interface methods in C# 8+).
- A class can implement multiple interfaces.

```csharp
public interface ILogger
{
    void Log(string message);
}
```

### Abstract class vs Interface (deep comparison)

| Feature | Abstract class | Interface |
|--------|----------------|-----------|
| Inheritance | Single inheritance | Multiple implementation
| Members | Can have fields + constructors | No fields (until C# 8 default implementations)
| Access modifiers | Can be `protected`, `private` | Members are public by default
| Use case | Base behavior + contract | Pure contract / polymorphic API

### When to use what
- Use **abstract class** when you have a base implementation and shared state.
- Use **interfaces** when you need multiple inheritance of behavior or want to define a plug-in contract.

---

## ✅ Interview Prep (Abstraction)
- Show an example where an abstract base class provides shared logic and requires derived types to implement a method.
- Show an interface used across unrelated classes.
- Explain why interfaces are useful for dependency injection.
