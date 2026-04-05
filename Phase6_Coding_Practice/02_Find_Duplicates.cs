using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase6_Coding_Practice
{
    internal class FindDuplicates
    {
        private static void Main()
        {
            var items = new[] { 1, 2, 3, 2, 4, 3, 5 };

            var duplicates = FindDuplicateValues(items);
            Console.WriteLine("Duplicates: " + string.Join(", ", duplicates));

            // LINQ approach
            var linqDupes = items
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            Console.WriteLine("Duplicates (LINQ): " + string.Join(", ", linqDupes));
        }

        private static IEnumerable<int> FindDuplicateValues(int[] values)
        {
            var seen = new HashSet<int>();
            var duplicates = new HashSet<int>();

            foreach (var v in values)
            {
                if (!seen.Add(v))
                {
                    duplicates.Add(v);
                }
            }

            return duplicates;
        }
    }
}
