using System;
using System.Collections.Generic;

namespace Phase1_Core_CSharp_Fundamentals
{
    internal class CollectionsExamples
    {
        private static void Main()
        {
            // Array
            int[] fixedArray = { 1, 2, 3 };
            Console.WriteLine($"Array length: {fixedArray.Length}");

            // List<T>
            var list = new List<string> { "apple", "banana" };
            list.Add("cherry");
            Console.WriteLine($"List count: {list.Count}");
            list.Remove("banana");

            // Dictionary<TKey, TValue>
            var grades = new Dictionary<string, int>
            {
                ["Alice"] = 90,
                ["Bob"] = 85
            };

            if (grades.TryGetValue("Alice", out int aliceGrade))
            {
                Console.WriteLine($"Alice's grade: {aliceGrade}");
            }

            // HashSet<T>
            var primes = new HashSet<int> { 2, 3, 5, 7 };
            bool added = primes.Add(3); // false because 3 already exists
            Console.WriteLine($"Added 3: {added}");

            // Demonstration: List vs Array
            FixedSizeExample(fixedArray);
            DynamicSizeExample(list);

            // Dictionary hashing demonstration
            Console.WriteLine("Dictionary buckets (approx):");
            foreach (var kvp in grades)
            {
                int hash = kvp.Key.GetHashCode();
                Console.WriteLine($"Key={kvp.Key}, Hash={hash}");
            }
        }

        private static void FixedSizeExample(int[] items)
        {
            // items.Length cannot change, trying to resize requires new array.
            // items[0] = 42; // allowed.
        }

        private static void DynamicSizeExample(List<string> items)
        {
            items.Add("date");
        }
    }
}
