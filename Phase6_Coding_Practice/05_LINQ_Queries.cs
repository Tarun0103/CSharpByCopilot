using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase6_Coding_Practice
{
    internal class LinqQueries
    {
        private static void Main()
        {
            var customers = new List<Customer>
            {
                new Customer(1, "Alice", active: true, new [] { 10m, 20m }),
                new Customer(2, "Bob", active: false, new [] { 5m }),
                new Customer(3, "Carol", active: true, new [] { 15m, 25m, 5m })
            };

            // Filter and project
            var activeTotals = customers
                .Where(c => c.IsActive)
                .Select(c => new { c.Name, TotalOrders = c.Orders.Sum() });

            foreach (var item in activeTotals)
            {
                Console.WriteLine($"{item.Name}: {item.TotalOrders:C}");
            }

            // GroupBy
            var grouped = customers.GroupBy(c => c.IsActive);
            Console.WriteLine("Groups:");
            foreach (var group in grouped)
            {
                Console.WriteLine($"Active={group.Key}");
                foreach (var c in group)
                {
                    Console.WriteLine($"  - {c.Name}");
                }
            }

            // Join (simple demonstration using LINQ join syntax)
            var orders = new List<Order>
            {
                new Order(1, 1, 50m),
                new Order(2, 3, 30m),
                new Order(3, 1, 20m)
            };

            var customerOrders = from c in customers
                                 join o in orders on c.Id equals o.CustomerId
                                 select new { c.Name, o.Amount };

            Console.WriteLine("Customer orders:");
            foreach (var co in customerOrders)
            {
                Console.WriteLine($"{co.Name}: {co.Amount:C}");
            }
        }

        private record Customer(int Id, string Name, bool IsActive, decimal[] Orders);

        private record Order(int Id, int CustomerId, decimal Amount);
    }
}
