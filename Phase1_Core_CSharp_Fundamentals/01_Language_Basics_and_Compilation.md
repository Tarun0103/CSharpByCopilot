# 1. Language Basics & .NET Compilation Flow

## ✅ What You Should Know

- **C#** is a statically typed, object-oriented language that compiles to **IL (Intermediate Language)**.
- **.NET compilation flow**:
  1. **C# source code** (`*.cs`) is compiled by the C# compiler (`csc`) into **IL** and metadata in a **PE (Portable Executable)** file (`.exe`, `.dll`).
  2. At runtime, the **CLR (Common Language Runtime)** loads the assembly and performs **JIT compilation** to native machine code.
  3. The GC (Garbage Collector) manages memory for reference types.

## 🧠 Key Concepts

### Value Types vs Reference Types
- **Value types** store data directly. They live on the **stack** (or inline in arrays/objects).
- **Reference types** store a reference (pointer) to the data on the **heap**.

| Feature | Value Type | Reference Type |
|--------|-----------|----------------|
| Memory location | Stack / inline | Heap + reference on stack
| Assignment | Copies value | Copies reference
| Null allowed | No (unless nullable) | Yes
| Examples | `int`, `bool`, `struct` | `string`, `class`, `interface`, `delegate`

👉 **Interview focus**: Explain what happens when you assign one variable to another for each category.

### Variables and Constants
- **Variable**: storage location that can be changed.
- **Constant**: value cannot change after compilation (`const`).

```csharp
const int MaxAttempts = 5;
int currentAttempt = 0;
```

### Type Casting
- **Implicit casting** (safe, no data loss): `int -> long`, `int -> double`
- **Explicit casting** (possible data loss): `double -> int`, requires cast operator.

```csharp
int n = 100;
long big = n;        // implicit

double d = 3.14;
int i = (int)d;      // explicit
```

- **Boxing**: converting a value type to `object` or an interface (allocates on heap).
- **Unboxing**: extracting the value type back from object (requires explicit cast).

👉 **Interview focus**: Describe what happens in boxing/unboxing, and why it can impact performance.

### Operators

#### Arithmetic
`+`, `-`, `*`, `/`, `%`

#### Logical
`&&`, `||`, `!`, `^` (XOR)

#### Bitwise
`&`, `|`, `^`, `<<`, `>>`

---

## ✅ Interview Prep Checklist (Language Basics)
- Explain the .NET compilation path from `.cs` to machine code.
- Differentiate value vs reference types with examples.
- Describe boxing/unboxing impact on performance.
- Show examples of implicit vs explicit casting.
- Name common operators and when you'd use bitwise operators.
