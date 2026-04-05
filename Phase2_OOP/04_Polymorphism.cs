using System;

namespace Phase2_OOP
{
    internal class PolymorphismExamples
    {
        private static void Main()
        {
            // Compile-time polymorphism (overloading)
            Console.WriteLine(Add(1, 2));
            Console.WriteLine(Add(3.5, 4.1));

            // Runtime polymorphism (overriding)
            Animal animal = new Animal();
            Animal dog = new Dog();
            Animal cat = new Cat();

            Console.WriteLine(animal.Describe());
            Console.WriteLine(dog.Describe());
            Console.WriteLine(cat.Describe());

            // Base-type reference using derived type shows runtime dispatch
            Animal[] animals = { new Dog(), new Cat() };
            foreach (var a in animals)
            {
                Console.WriteLine(a.Describe());
            }
        }

        private static int Add(int a, int b) => a + b;
        private static double Add(double a, double b) => a + b;

        private class Animal
        {
            public virtual string Describe() => "I am an animal.";
        }

        private class Dog : Animal
        {
            public override string Describe() => "I am a dog.";
        }

        private class Cat : Animal
        {
            public override string Describe() => "I am a cat.";
        }
    }
}
