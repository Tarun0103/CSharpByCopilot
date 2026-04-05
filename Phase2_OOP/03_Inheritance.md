# 3. Inheritance

Inheritance lets a derived class reuse and extend behavior from a base class.

### Base & Derived classes

```csharp
public class Animal
{
    public void Eat() { }
}

public class Dog : Animal
{
    public void Bark() { }
}
```

### Method overriding
- Use `virtual` on base method.
- Use `override` in derived class.

```csharp
public class Animal
{
    public virtual string Speak() => "...";
}

public class Dog : Animal
{
    public override string Speak() => "Woof";
}
```

### `sealed` keyword
- Applies to classes or methods.
- A sealed class cannot be inherited.
- A sealed method cannot be overridden.

```csharp
public sealed class Logger { }
public class Base
{
    public virtual void DoWork() { }
}
public class Derived : Base
{
    public sealed override void DoWork() { }
}
```

👉 **Interview focus**: Why multiple inheritance is not supported in C#?

- Avoids **diamond problem** (ambiguity when two base classes implement the same method/property).
- Simpler object model and runtime.
- C# provides multiple inheritance of behavior via **interfaces**.

---

## ✅ Interview Prep (Inheritance)
- Explain how base and derived classes are declared.
- Show overriding a method and call `base.Method()`.
- Describe the use of `sealed` and when to apply it.
- Explain why C# supports single inheritance but multiple interfaces.
