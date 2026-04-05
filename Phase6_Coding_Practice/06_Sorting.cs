using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase6_Coding_Practice
{
    internal class Sorting
    {
        private static void Main()
        {
            var items = new List<Person>
            {
                new(1, "Charlie", 30),
                new(2, "Alice", 25),
                new(3, "Bob", 25),
            };

            // In-place sort by name
            items.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            Console.WriteLine("Sorted by name:");
            foreach (var p in items)
            {
                Console.WriteLine(p);
            }

            // Sort by age then name using LINQ (creates new sequence)
            var sorted = items
                .OrderBy(p => p.Age)
                .ThenBy(p => p.Name)
                .ToList();

            Console.WriteLine("Sorted by age then name:");
            foreach (var p in sorted)
            {
                Console.WriteLine(p);
            }
        }

        private record Person(int Id, string Name, int Age);
    }
}
