using System;
using System.IO;

namespace Phase4_Advanced_CSharp
{
    internal class MemoryManagementExamples
    {
        private static void Main()
        {
            // Stack allocation: value types
            int x = 10;
            int y = x;
            Console.WriteLine($"x={x}, y={y}");

            // Heap allocation: reference types
            var person = new Person { Name = "Alice" };
            var samePerson = person;
            samePerson.Name = "Bob";
            Console.WriteLine($"person.Name = {person.Name}");

            // IDisposable + using
            using (var writer = new StreamWriter("temp.txt"))
            {
                writer.WriteLine("Hello");
            } // writer.Dispose() called here

            // Force a garbage collection (for demo only — not recommended in production)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine("Forced GC. You should avoid calling GC.Collect() in real apps.");
        }

        private class Person
        {
            public string Name { get; set; } = string.Empty;
        }

        // Example of IDisposable implementation
        private class ResourceHolder : IDisposable
        {
            private readonly FileStream _stream;

            public ResourceHolder(string path)
            {
                _stream = File.OpenWrite(path);
            }

            public void Dispose()
            {
                _stream?.Dispose();
            }
        }
    }
}
