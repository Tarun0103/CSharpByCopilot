using System;
using System.Linq;

namespace Phase4_Advanced_CSharp
{
    internal class ReflectionExamples
    {
        private static void Main()
        {
            var type = typeof(Person);
            Console.WriteLine($"Type: {type.FullName}");

            Console.WriteLine("Properties:");
            foreach (var prop in type.GetProperties())
            {
                Console.WriteLine($" - {prop.Name} ({prop.PropertyType.Name})");
            }

            Console.WriteLine("Methods:");
            foreach (var method in type.GetMethods().Where(m => m.DeclaringType == type))
            {
                Console.WriteLine($" - {method.Name}");
            }

            // Create instance dynamically
            var instance = Activator.CreateInstance(type, "Alice", 30);
            Console.WriteLine(instance);

            // Set property by reflection
            var nameProp = type.GetProperty("Name");
            nameProp?.SetValue(instance, "Bob");
            Console.WriteLine(instance);
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public override string ToString() => $"Person(Name={Name}, Age={Age})";

            private void Secret() => Console.WriteLine("This is private.");
        }
    }
}
