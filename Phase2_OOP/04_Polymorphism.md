# 4. Polymorphism

Polymorphism means "many forms". In C#, it appears as:

- **Compile-time polymorphism** (method overloading)
- **Runtime polymorphism** (method overriding)

## Compile-time polymorphism (Method Overloading)
Multiple methods with the same name but different parameter types/counts.

```csharp
public int Compute(int x) => x * x;
public int Compute(int x, int y) => x * y;
```

## Runtime polymorphism (Method Overriding)
A base class defines a `virtual` method, derived classes `override` it.

```csharp
public virtual string Describe() => "Base";
public override string Describe() => "Derived";
```

👉 **Interview focus**: Difference between overriding vs overloading.

- **Overloading**: Same method name, different signature; resolved at compile time.
- **Overriding**: Same signature, different implementation in derived class; resolved at runtime via virtual dispatch.

---

## ✅ Interview Prep (Polymorphism)
- Give examples of method overloading and when it's useful.
- Explain how `virtual`/`override` enable runtime dispatch.
- Demonstrate calling an overridden method through a base type reference.
