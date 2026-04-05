using System;

namespace Phase1_Core_CSharp_Fundamentals
{
    /// <summary>
    /// Demonstrates basic C# language features and shows how value vs reference types behave.
    /// </summary>
    internal class LanguageBasics
    {
        private static void Main()
        {
            // Value types store the value directly.
            int a = 42;
            int b = a; // Copies the value.
            b = 100;
            Console.WriteLine($"Value types - a: {a}, b: {b}");

            // Reference types store a reference to the object on the heap.
            var alice = new Person { Name = "Alice" };
            var bob = alice; // Copies the reference.
            bob.Name = "Bob";
            Console.WriteLine($"Reference types - alice.Name: {alice.Name}, bob.Name: {bob.Name}");

            // Boxing and unboxing
            int valueType = 123;
            object boxed = valueType; // Boxing: copies value into heap object.
            int unboxed = (int)boxed; // Unboxing: cast back to value type.
            Console.WriteLine($"Boxing/unboxing: boxed={boxed}, unboxed={unboxed}");

            // Implicit and explicit casting
            int small = 42;
            long bigger = small; // implicit

            double pi = 3.14;
            int truncated = (int)pi; // explicit
            Console.WriteLine($"Casting: bigger={bigger}, truncated={truncated}");
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}
