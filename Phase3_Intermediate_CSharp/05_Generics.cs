using System;
using System.Collections.Generic;

namespace Phase3_Intermediate_CSharp
{
    internal class GenericsExamples
    {
        private static void Main()
        {
            var box = new Box<int> { Value = 42 };
            Console.WriteLine($"Box contains: {box.Value}");

            var result = Echo("Hello");
            Console.WriteLine(result);

            // Using a generic constraint
            var repository = new Repository<Person>();
            repository.Add(new Person { Name = "Alice" });
            foreach (var person in repository.GetAll())
            {
                Console.WriteLine(person.Name);
            }

            // Performance: no boxing/unboxing
            var ints = new List<int> { 1, 2, 3 };
            int sum = 0;
            foreach (var i in ints)
            {
                sum += i;
            }
            Console.WriteLine($"Sum: {sum}");
        }

        private class Box<T>
        {
            public T Value { get; set; }
        }

        // Generic method: type parameter `T` inferred from the call site.
        // Generics avoid boxing for value types and provide compile-time type safety.
        private static T Echo<T>(T value) => value;

        private interface IEntity
        {
            string Name { get; set; }
        }

        private class Person : IEntity
        {
            public string Name { get; set; } = string.Empty;
        }

        // Generic type with constraints: `IEntity` ensures required APIs, `new()` allows
        // parameterless construction inside the type.
        private class Repository<T> where T : IEntity, new()
        {
            private readonly List<T> _items = new();

            public void Add(T item) => _items.Add(item);
            public List<T> GetAll() => new List<T>(_items);

            public T CreateNew()
            {
                // `new()` constraint guarantees this is allowed
                return new T();
            }
        }

        // Interview tips:
        // - Explain why generics remove the need for casting and avoid boxing.
        // - Discuss variance: `IEnumerable<Derived>` is assignable to `IEnumerable<Base>` (covariance),
        //   but most generic mutable interfaces are invariant.
        // - Show constraints to express required capabilities (`where T : ...`).
    }
}
