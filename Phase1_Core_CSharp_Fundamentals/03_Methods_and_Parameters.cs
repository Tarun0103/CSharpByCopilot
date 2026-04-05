using System;

namespace Phase1_Core_CSharp_Fundamentals
{
    internal class MethodExamples
    {
        private static void Main()
        {
            // Overloaded methods
            Console.WriteLine(Add(2, 3));
            Console.WriteLine(Add(1.5, 2.5));

            // ref vs out
            int value = 10;
            RefIncrement(ref value);
            Console.WriteLine($"After ref increment: {value}");

            int outResult;
            OutInitialize(out outResult);
            Console.WriteLine($"After out initialization: {outResult}");

            // in parameter
            int original = 7;
            Console.WriteLine($"Square (in): {Square(in original)}");

            // Optional parameters
            Greet();
            Greet("Alice", newline: false);

            // Expression-bodied method in action
            Console.WriteLine($"Sum of 4 and 5 = {AddExpression(4, 5)}");
        }

        private static int Add(int a, int b) => a + b;
        private static double Add(double a, double b) => a + b;

        private static void RefIncrement(ref int x)
        {
            x += 1; // caller sees change
        }

        private static void OutInitialize(out int x)
        {
            x = 999; // must assign before returning
        }

        private static int Square(in int x)
        {
            // x = x * x; // compile error, `in` forbids modification
            return x * x;
        }

        private static void Greet(string name = "World", bool newline = true)
        {
            if (newline)
                Console.WriteLine($"Hello, {name}!");
            else
                Console.Write($"Hello, {name}!");
        }

        private static int AddExpression(int a, int b) => a + b;
    }
}
