using System;
using System.Collections.Generic;

namespace Phase2_OOP
{
    /// <summary>
    /// Simple examples of SOLID principles in action.
    /// This is intentionally simple; the goal is to show the concept.
    /// </summary>
    internal class SolidExamples
    {
        private static void Main()
        {
            // Single Responsibility Principle example
            var order = new Order { Id = 1, Amount = 100m };
            var orderProcessor = new OrderProcessor(new ConsoleLogger());
            orderProcessor.Process(order);

            // Open/Closed Principle example - adding a new tax strategy without modifying processor
            var taxCalc = new ProgressiveTaxStrategy();
            var tax = taxCalc.CalculateTax(1000m);
            Console.WriteLine($"Tax: {tax:C}");

            // Liskov Substitution Principle example
            Rectangle rect = new Square(5);
            Console.WriteLine($"Square as rectangle area: {rect.Area}");

            // Interface Segregation Principle example
            IPrinter printer = new ConsolePrinter();
            var report = new Report(printer);
            report.Print("Monthly report");

            // Dependency Inversion Principle example
            IRepository<Order> repo = new InMemoryRepository<Order>();
            repo.Add(order);
            Console.WriteLine($"Stored orders: {repo.GetAll().Count}");
        }

        // SRP: Order and order processing separated
        private class Order
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
        }

        private interface ILogger
        {
            void Log(string message);
        }

        private class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine($"[LOG] {message}");
        }

        private class OrderProcessor
        {
            private readonly ILogger _logger;

            public OrderProcessor(ILogger logger)
            {
                _logger = logger;
            }

            public void Process(Order order)
            {
                _logger.Log($"Processing order {order.Id} for {order.Amount:C}");
                // Real work would go here (e.g., validating, saving, charging)
            }
        }

        // OCP: Define an abstraction for tax calculation
        private interface ITaxStrategy
        {
            decimal CalculateTax(decimal amount);
        }

        private class FlatTaxStrategy : ITaxStrategy
        {
            public decimal CalculateTax(decimal amount) => amount * 0.1m;
        }

        private class ProgressiveTaxStrategy : ITaxStrategy
        {
            public decimal CalculateTax(decimal amount) => amount * (amount > 500 ? 0.15m : 0.05m);
        }

        // LSP: Square substitutable for Rectangle
        private class Rectangle
        {
            public virtual int Width { get; set; }
            public virtual int Height { get; set; }

            public int Area => Width * Height;
        }

        private class Square : Rectangle
        {
            public override int Width
            {
                get => base.Width;
                set
                {
                    base.Width = value;
                    base.Height = value;
                }
            }

            public override int Height
            {
                get => base.Height;
                set
                {
                    base.Width = value;
                    base.Height = value;
                }
            }
        }

        // ISP: Smaller interfaces
        private interface IPrinter
        {
            void Print(string content);
        }

        private class ConsolePrinter : IPrinter
        {
            public void Print(string content) => Console.WriteLine(content);
        }

        private class Report
        {
            private readonly IPrinter _printer;

            public Report(IPrinter printer)
            {
                _printer = printer;
            }

            public void Print(string content)
            {
                _printer.Print(content);
            }
        }

        // DIP: Depend on abstraction
        private interface IRepository<T>
        {
            void Add(T item);
            List<T> GetAll();
        }

        private class InMemoryRepository<T> : IRepository<T>
        {
            private readonly List<T> _items = new();
            public void Add(T item) => _items.Add(item);
            public List<T> GetAll() => new List<T>(_items);
        }
    }
}
