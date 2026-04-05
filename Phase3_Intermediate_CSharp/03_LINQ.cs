using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase3_Intermediate_CSharp
{
    internal class LinqExamples
    {
        private static void Main()
        {
            // Sample data set
            // Note: LINQ queries operate on this in-memory collection; for
            // IQueryable (ORM-backed) providers the expression tree is translated.
            var products = new List<Product>
            {
                new("Apple", 1.0m, "Fruit"),
                new("Banana", 0.5m, "Fruit"),
                new("Carrot", 0.3m, "Vegetable"),
                new("Steak", 10.0m, "Meat")
            };

            // Where, Select, OrderBy
            var query = products
                .Where(p => p.Price < 5)
                .OrderBy(p => p.Name)
                .Select(p => p.Name);

            // Deferred execution example: `query` is not executed until iterated.
            Console.WriteLine("Cheap products:");
            foreach (var name in query)
            {
                Console.WriteLine(name);
            }

            // Deferred execution: adding an item to `products` after creating
            // the query means the new item will appear when the query is iterated.
            var deferred = products.Where(p => p.Category == "Fruit");
            products.Add(new Product("Orange", 0.8m, "Fruit"));
            Console.WriteLine("Fruits (deferred):");
            foreach (var fruit in deferred)
            {
                Console.WriteLine(fruit.Name);
            }

            // GroupBy: projects a grouping of items. Be mindful that GroupBy
            // will buffer grouped results; for large data sets consider memory impact.
            var grouped = products.GroupBy(p => p.Category);
            Console.WriteLine("Products grouped by category:");
            foreach (var group in grouped)
            {
                Console.WriteLine($"Category: {group.Key}");
                foreach (var item in group)
                {
                    Console.WriteLine($"  - {item.Name} ({item.Price:C})");
                }
            }

            // IEnumerable vs IQueryable (concept demonstration)
            // - `IEnumerable<T>`: LINQ to Objects, executed in-memory. Deferred execution
            //   applies to iterator-based operators.
            // - `IQueryable<T>`: expression trees that can be translated (e.g., to SQL)
            //   by providers such as Entity Framework. Execution is deferred until
            //   enumeration but translation happens on the provider side.
            Console.WriteLine("(IEnumerable vs IQueryable: see notes in markdown)");
        }

        private record Product(string Name, decimal Price, string Category);
    }
}
