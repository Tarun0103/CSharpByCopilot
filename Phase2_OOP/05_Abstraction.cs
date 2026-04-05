using System;

namespace Phase2_OOP
{
    internal class AbstractionExamples
    {
        private static void Main()
        {
            // Abstract class example
            Shape circle = new Circle(3);
            circle.Describe();
            Console.WriteLine($"Circle area: {circle.Area():F2}");

            // Interface example
            ILogger consoleLogger = new ConsoleLogger();
            consoleLogger.Log("Logging via interface reference");

            ILogger fileLogger = new FileLogger();
            fileLogger.Log("Writing to file (simulated)");
        }

        // Abstract base class
        private abstract class Shape
        {
            public abstract double Area();

            public virtual void Describe() => Console.WriteLine("I am a shape.");
        }

        private class Circle : Shape
        {
            public double Radius { get; }

            public Circle(double radius)
            {
                Radius = radius;
            }

            public override double Area() => Math.PI * Radius * Radius;

            public override void Describe() => Console.WriteLine($"I am a circle with radius {Radius}.");
        }

        // Interface
        private interface ILogger
        {
            void Log(string message);
        }

        private class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine($"[Console] {message}");
        }

        private class FileLogger : ILogger
        {
            public void Log(string message)
            {
                // In a real app, write to a file. Here we simulate.
                Console.WriteLine($"[File] {message}");
            }
        }
    }
}
