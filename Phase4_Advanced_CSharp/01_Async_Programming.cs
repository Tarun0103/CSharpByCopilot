using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Phase4_Advanced_CSharp
{
    internal class AsyncProgrammingExamples
    {
        private static async Task Main()
        {
            Console.WriteLine("Starting async example...");

            var result = await FetchDataAsync();
            Console.WriteLine($"Fetched data length: {result.Length}");

            // Demonstrate deadlock scenario (commented out to avoid freezing)
            // DeadlockExample().Wait();

            // Demonstrate Task vs Thread
            await RunTasksAsync();
        }

        private static async Task<string> FetchDataAsync()
        {
            using var client = new HttpClient();
            // Async I/O does not block threads while waiting
            var response = await client.GetStringAsync("https://example.com");
            return response;
        }

        private static async Task RunTasksAsync()
        {
            var tasks = new Task[3];
            for (int i = 0; i < tasks.Length; i++)
            {
                int capture = i;
                tasks[i] = Task.Run(() =>
                {
                    Console.WriteLine($"Task {capture} running on thread {Thread.CurrentThread.ManagedThreadId}");
                });
            }

            await Task.WhenAll(tasks);
        }

        // WARNING: This method can deadlock if called from a synchronous context that blocks.
        private static async Task DeadlockExample()
        {
            var task = Task.Run(async () =>
            {
                await Task.Delay(100);
                return "Done";
            });

            // Blocking call causes deadlock if called on a synchronization context that waits for completion
            string result = task.Result;
            Console.WriteLine(result);
        }
    }
}
