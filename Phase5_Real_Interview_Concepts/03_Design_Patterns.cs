using System;
using System.Collections.Generic;

namespace Phase5_Real_Interview_Concepts
{
    internal class DesignPatternsExamples
    {
        private static void Main()
        {
            // Singleton usage
            var logger1 = Logger.Instance;
            var logger2 = Logger.Instance;
            Console.WriteLine($"Same instance: {ReferenceEquals(logger1, logger2)}");
            logger1.Log("Singleton logger in action");

            // Factory usage
            IShape circle = ShapeFactory.Create("circle");
            IShape square = ShapeFactory.Create("square");
            Console.WriteLine(circle.GetType().Name);
            Console.WriteLine(square.GetType().Name);

            // Repository usage
            IRepository<Person> repo = new InMemoryRepository<Person>();
            repo.Add(new Person(1, "Alice"));
            repo.Add(new Person(2, "Bob"));
            foreach (var p in repo.GetAll())
            {
                Console.WriteLine(p.Name);
            }
        }

        // Singleton
        private sealed class Logger
        {
            private static readonly Lazy<Logger> _instance = new(() => new Logger());
            public static Logger Instance => _instance.Value;
            private Logger() { }

            public void Log(string message) => Console.WriteLine($"[Logger] {message}");
        }

        // Factory
        private interface IShape { }
        private class Circle : IShape { }
        private class Square : IShape { }

        private static class ShapeFactory
        {
            public static IShape Create(string type) => type.ToLowerInvariant() switch
            {
                "circle" => new Circle(),
                "square" => new Square(),
                _ => throw new InvalidOperationException("Unknown shape type")
            };
        }

        // Repository
        private interface IRepository<T>
        {
            void Add(T item);
            T? Get(int id);
            IReadOnlyList<T> GetAll();
        }

        private class InMemoryRepository<T> : IRepository<T> where T : IEntity
        {
            private readonly Dictionary<int, T> _items = new();

            public void Add(T item) => _items[item.Id] = item;

            public T? Get(int id) => _items.TryGetValue(id, out var value) ? value : default;

            public IReadOnlyList<T> GetAll() => new List<T>(_items.Values);
        }

        private interface IEntity { int Id { get; } }

        private class Person : IEntity
        {
            public int Id { get; }
            public string Name { get; }

            public Person(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}
