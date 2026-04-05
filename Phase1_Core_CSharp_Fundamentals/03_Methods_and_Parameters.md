# 3. Methods & Parameters

Methods (functions) are how you organize behavior.

## Method signatures

```csharp
returnType MethodName(parameterType arg1, parameterType arg2)
{
    // body
}
```

### Method overloading
Same method name, different parameter lists.

### ref / out / in
- `ref`: caller must initialize variable before passing; callee can read and write.
- `out`: caller does not have to initialize; callee must assign before returning.
- `in`: caller provides a value; callee can read but cannot modify.

### Optional parameters
Specify default values.

```csharp
void Print(string message = "Hello", bool newline = true)
{
    if (newline)
        Console.WriteLine(message);
    else
        Console.Write(message);
}
```

### Expression-bodied methods
Provide concise method syntax.

```csharp
int Add(int a, int b) => a + b;
```

👉 **Interview focus**: Difference between `ref` and `out`.

- `ref`: variable must be initialized by caller; can be read and written by callee.
- `out`: variable is treated as uninitialized; callee must assign it before returning.

---

## ✅ Interview Prep Checklist (Methods)
- Explain how method overloading works.
- Describe how `ref`, `out`, and `in` differ.
- Give an example of optional parameters and how they affect calling code.
- Show expression-bodied methods and when they are most useful.
