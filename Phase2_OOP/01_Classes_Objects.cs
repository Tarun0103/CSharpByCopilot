using System;

namespace Phase2_OOP
{
    /// <summary>
    /// Demonstrates basic class and object usage, including constructors and properties.
    /// </summary>
    internal class ClassesObjects
    {
        private static void Main()
        {
            // Program entry point — execution starts here.
            // First access to the `Person` type (via creating an instance) triggers the
            // static constructor for `Person` (if it hasn't already run). The static
            // constructor runs once per AppDomain and is used for one-time type setup.

            // 1) Create a Person using the parameterized constructor.
            //    This demonstrates passing initial state at construction time.
            var person = new Person("Alice", 30);
            Console.WriteLine(person);

            // 2) Create a Person using the default (parameterless) constructor,
            //    then set properties afterwards. This shows an alternate creation
            //    pattern when callers prefer property initialization over parameters.
            var person2 = new Person();
            person2.Name = "Bob";
            person2.Age = 28;
            Console.WriteLine(person2);

            // 3) Show static state shared across all instances.
            //    `InitializedCount` shows how many Person instances have been created.
            Console.WriteLine($"Static initialization count: {Person.InitializedCount}");
        }

        private class Person
        {
            // Backing field for the Name property. Keeping fields private enforces
            // encapsulation so callers interact through properties.
            private string _name;

            // Property with explicit getter/setter that wraps the backing field.
            // Properties provide a public API while allowing future validation
            // or side-effects to be added without changing callers.
            public string Name
            {
                get => _name;
                set => _name = value;
            }

            // Auto-property for Age — a concise way to expose simple data.
            public int Age { get; set; }

            // Static property holds data shared by the type (not per-instance).
            // We make the setter private to control mutation from inside the type.
            public static int InitializedCount { get; private set; }

            // Default (parameterless) constructor chains to the parameterized
            // constructor to avoid duplicated initialization logic. The
            // parameterized constructor is responsible for incrementing
            // `InitializedCount` so chaining ensures a single increment.
            public Person() : this("Unknown", 0)
            {
            }

            // Parameterized constructor lets callers provide initial state.
            // This avoids the need for property assignment after construction.
            public Person(string name, int age)
            {
                Name = name;
                Age = age;
                InitializedCount++;
            }

            // Copy constructor: creates a new instance copying state from another
            // instance. This is a common pattern for cloning/shallow-copying.
            public Person(Person other) : this(other?.Name ?? throw new ArgumentNullException(nameof(other)), other.Age)
            {
            }

            // Static constructor: used to initialize static state for the type.
            // It is called automatically by the runtime before the type is used
            // (before the first instance is created or any static member is accessed).
            // It runs only once.
            static Person()
            {
                InitializedCount = 0;
                Console.WriteLine("Static constructor called to initialize Person type.");
            }

            // Example: private constructor usage for controlled instantiation.
            // Private constructors are commonly used for singletons or when the
            // type provides static factory methods and wants to prevent callers
            // from calling `new` directly. Below is a minimal singleton example.
            private class PersonSingleton
            {
                // single shared instance
                public static readonly PersonSingleton Instance = new PersonSingleton();

                // private constructor prevents external `new` calls
                private PersonSingleton()
                {
                    // one-time initialization
                }
            }

            // Override ToString to provide a readable representation for Console.WriteLine.
            public override string ToString() => $"Person(Name={Name}, Age={Age})";
        }
    }
}
