# 1. String Handling

## `string` vs `StringBuilder`

- `string` is immutable. Operations like concatenation create new string instances.
- `StringBuilder` is mutable and efficient for many manipulations.

### Example: concatenation

```csharp
string s = "";
for (int i = 0; i < 1000; i++)
    s += i; // creates many intermediate strings

var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i);
```

👉 **VERY COMMON interview question**: When to use `StringBuilder` vs `string`.

> Use `string` for simple cases and when you don’t do many concatenations. Use `StringBuilder` when building up large strings in a loop.

---

## ✅ Interview Prep (String Handling)
- Compare `string` immutability with `StringBuilder` mutability.
- Know common string methods: `Substring`, `Split`, `Replace`, `IndexOf`, `StartsWith`, `Contains`.
- Be prepared to discuss memory implications of repeated string concatenation.
