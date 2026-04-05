using System;

namespace Phase2_OOP
{
    internal class InheritanceExamples
    {
        private static void Main()
        {
            Animal generic = new Animal();
            Animal dogAsAnimal = new Dog();
            Animal catAsAnimal = new Cat();

            Console.WriteLine(generic.Speak());
            Console.WriteLine(dogAsAnimal.Speak());
            Console.WriteLine(catAsAnimal.Speak());

            // Sealed class example
            var logger = new Logger();
            logger.Log("Hello from sealed class");
        }

        private class Animal
        {
            public virtual string Speak() => "";
        }

        private class Dog : Animal
        {
            public override string Speak() => "Woof";
        }

        private class Cat : Animal
        {
            public override string Speak() => "Meow";
        }

        private sealed class Logger
        {
            public void Log(string message)
            {
                Console.WriteLine($"[LOG] {message}");
            }
        }
    }
}
