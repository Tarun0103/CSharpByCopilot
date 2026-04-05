# Phase 2 – OOP (Very Important)

## 1. Classes & Objects

### What is a class?
A class is a blueprint for creating objects. It defines **state** (fields/properties) and **behavior** (methods).

### What is an object?
An object is an instance of a class. It carries its own state.

### Fields and Properties
- **Fields** are variables inside a class.
- **Properties** provide controlled access to fields.

```csharp
public class Person
{
    // field (private implementation detail)
    private string _name;

    // property (public API)
    public string Name
    {
        get => _name;
        set => _name = value;
    }
}
```

### Constructors
- **Default constructor**: provided by the compiler if no constructor is defined.
- **Parameterized constructor**: lets you initialize state.
- **Static constructor**: runs once before the first instance or any static member is accessed.

- **Copy constructor (pattern)**: constructs a new instance by copying state from an existing instance. Common for cloning/shallow copies.
- **Private constructor**: used to prevent external `new` calls, enabling patterns like singletons or static factories.
- **Primary constructor (records / modern C#)**: `record` types (C# 9+) support concise primary constructors, e.g. `record PersonRecord(string Name, int Age);` which creates immutable properties from parameters.

```csharp
public Person(string name)
{
    Name = name;
}

static Person()
{
    // static initialization
}
```

// Examples:
// Copy constructor
// public Person(Person other) : this(other?.Name ?? throw new ArgumentNullException(nameof(other)), other.Age) { }

// Private constructor / Singleton
// private Person() { }
// public static Person Instance { get; } = new Person();

// Record primary constructor (C# 9+)
// public record PersonRecord(string Name, int Age);

---

## ✅ Interview Prep (Classes & Objects)
- Explain what a class is vs an object.
- Describe fields vs properties and why properties are recommended.
- Show examples of default, parameterized, and static constructors.

---

## Detailed execution flow — `01_Classes_Objects.cs`

This section walks through how the program runs and why the constructors behave the way they do. Read this as a step-by-step narrative you can use to explain the code in an interview.

1. Program start: `Main()` is the entry point.
    - The first line that touches the `Person` type (creating an instance) triggers the runtime to ensure the type is initialized.

2. Static constructor runs once (if not already executed):
    - `static Person()` executes before the first instance is created or any static member is accessed.
    - In the sample it sets `InitializedCount = 0` and writes a message. This demonstrates one-time type initialization.

3. Parameterized constructor call:
    - `var person = new Person("Alice", 30);` runs the instance (non-static) constructor `Person(string name, int age)`.
    - That constructor assigns properties and increments `InitializedCount`.
    - `Console.WriteLine(person)` calls the overridden `ToString()` that prints the object's state.

4. Default constructor + property assignment:
    - `var person2 = new Person();` calls the parameterless constructor which sets defaults and increments `InitializedCount`.
    - Properties are then set (`Name`, `Age`) which demonstrates the two-step initialization pattern.

5. Static shared state readout:
    - `Console.WriteLine($"Static initialization count: {Person.InitializedCount}");` prints the number of times instances were created.

Tip to explain in interviews: point out the difference between per-instance state (`Name`, `Age`) and per-type state (`InitializedCount`). Show how static initialization is guaranteed to happen once.

## Interview-style conceptual scenarios and suggested answers

- **Scenario: When does the static constructor run?**
  - Suggested answer: "It runs automatically before the type is first used — either before the first instance is created or before any static member is accessed. It executes once and is typically used to initialize static fields or perform one-time setup."

- **Scenario: Why use a parameterized constructor vs properties?**
  - Suggested answer: "Parameterized constructors let callers establish valid state at creation time, avoiding partially-initialized objects and ensuring invariants. Properties are useful when state may be optional or when a parameterless constructor is required for frameworks."

- **Scenario: What are constructor chaining and base constructor calls?**
  - Suggested answer: "Use `: this(...)` to call another constructor in the same class to reuse initialization logic, and `: base(...)` to call a base class constructor when inheriting. This reduces duplication and keeps initialization consistent."

- **Scenario: What are common pitfalls?**
  - Suggested answer bullets:
     - Avoid expensive or I/O work inside static constructors (it can delay type initialization across threads).
     - Be careful calling virtual methods from constructors — derived overrides may run before the derived class is fully initialized.
     - Handle exceptions: an exception in a static constructor prevents type initialization and can make the type unusable for the remainder of the AppDomain.

## Quick checklist to discuss during interviews

- **Explain execution order**: static constructor → instance constructor → instance methods.
- **Describe encapsulation**: private fields with public properties.
- **Show sample variations**: constructor chaining, private constructors for singletons, copy constructor pattern `Person(Person other)`.
- **Mention modern alternatives**: `record` types and primary constructors (C# 9+) for immutable data.

## Example follow-up tasks you can propose in an interview

- Add constructor chaining to reduce duplicated initialization.
- Add validation in setters or constructors (e.g., throw `ArgumentOutOfRangeException` for negative ages).
- Implement a copy constructor and a `Clone()` method, explaining shallow vs deep copy.

---

Keep this guide handy when discussing `01_Classes_Objects.cs` during interviews: it frames the code as a small, clear demonstration of constructors, static vs instance state, and sensible object initialization patterns.
