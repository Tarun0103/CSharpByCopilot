using System;

namespace Phase5_Real_Interview_Concepts
{
    internal class DependencyInjectionExamples
    {
        private static void Main()
        {
            // Manual DI
            ILogger logger = new ConsoleLogger();
            var orderService = new OrderService(logger);
            orderService.PlaceOrder(123);

            // In real apps, a DI container would create these objects for you.
        }

        private interface ILogger
        {
            void Log(string message);
        }

        private class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine($"[Log] {message}");
        }

        private class OrderService
        {
            private readonly ILogger _logger;

            public OrderService(ILogger logger)
            {
                _logger = logger;
            }

            public void PlaceOrder(int orderId)
            {
                _logger.Log($"Placing order {orderId}");
                // order processing
            }
        }
    }
}
