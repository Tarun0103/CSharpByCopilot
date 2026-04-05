using System;
using System.Collections.Generic;

namespace Phase3_Intermediate_CSharp
{
    internal class CollectionsDeepDive
    {
        private static void Main()
        {
            // IEnumerable<T> example (read-only enumeration)
            // Use the most general interface possible for APIs that only need enumeration.
            IEnumerable<int> numbers = new List<int> { 1, 2, 3 };
            PrintEnumerable(numbers);

            // ICollection<T> example (count + add/remove)
            ICollection<string> words = new List<string> { "hello", "world" };
            Console.WriteLine($"Count before: {words.Count}");
            words.Add("!");
            Console.WriteLine($"Count after: {words.Count}");

            // IList<T> example (indexed access)
            IList<string> list = new List<string> { "a", "b", "c" };
            list[1] = "B";
            Console.WriteLine($"Second element: {list[1]}");

            // Pass the most general interface possible
            PrintEnumerable(list);

            // Important edge case: modifying a collection while enumerating it
            // (e.g., calling Add/Remove during foreach) will typically throw
            // `InvalidOperationException` for many collection implementations.
            // If you need to modify while iterating, either collect items to
            // remove and apply after iteration, or iterate over a snapshot
            // (e.g., `list.ToList()`).
        }

        private static void PrintEnumerable(IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }
    }
}
