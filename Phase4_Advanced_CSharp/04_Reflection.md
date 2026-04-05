# 4. Reflection (Basic understanding)

Reflection allows inspecting metadata and types at runtime.

## Common uses
- Discover types and members dynamically.
- Build serializers/deserializers.
- Implement dependency injection containers.

### Example: Inspecting a type

```csharp
Type type = typeof(MyClass);
var properties = type.GetProperties();
```

### Example: Creating an instance

```csharp
object instance = Activator.CreateInstance(type);
```

---

## ✅ Interview Prep (Reflection)
- Explain what metadata is and where it is stored (assemblies).
- Know how to get `Type` objects and inspect properties/methods.
- Understand performance cost and why it should be used sparingly.
