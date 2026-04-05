# 5. Attributes

Attributes are metadata applied to types, members, and parameters.

## Built-in attributes
- `[Obsolete]` marks a member as deprecated.
- `[Serializable]` marks a type as serializable.
- `[DebuggerStepThrough]`, `[DebuggerDisplay]` are for debugging.

## Custom attributes

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ExampleAttribute : Attribute
{
    public string Info { get; }
    public ExampleAttribute(string info) => Info = info;
}
```

Attributes can be inspected using reflection.

---

## ✅ Interview Prep (Attributes)
- Explain what attributes are and how they are stored in metadata.
- Show how to define a custom attribute and read it at runtime.
- Mention common framework attributes (e.g., `[DataContract]`, `[JsonIgnore]`, `[TestMethod]`).
