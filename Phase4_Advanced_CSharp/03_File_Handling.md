# 3. File Handling

File I/O is part of many applications. Use the `System.IO` namespace.

## Read/Write Files

- `File.WriteAllText` / `File.ReadAllText` for simple cases.
- `File.WriteAllLines` / `File.ReadAllLines` for line-based content.

## Streams

Streams provide a unified way to read/write bytes.

- `FileStream` for file access.
- `StreamReader` / `StreamWriter` for text.

Example pattern:

```csharp
using var stream = new FileStream(path, FileMode.Create);
using var writer = new StreamWriter(stream);
await writer.WriteLineAsync("Hello");
```

---

## ✅ Interview Prep (File Handling)
- Explain the difference between `File.ReadAllText` and `StreamReader`.
- Why `using` (or `using var`) is important for file streams.
- Show how to handle exceptions from I/O (e.g., `IOException`).
