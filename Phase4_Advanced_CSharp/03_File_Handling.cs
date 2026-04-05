using System;
using System.IO;
using System.Threading.Tasks;

namespace Phase4_Advanced_CSharp
{
    internal class FileHandlingExamples
    {
        private static async Task Main()
        {
            string path = "example.txt";

            // Basic file write/read
            File.WriteAllText(path, "Hello from C#\n");
            string content = File.ReadAllText(path);
            Console.WriteLine($"Read back: {content}");

            // Using StreamWriter/StreamReader
            await WriteWithStreamAsync(path, "Line 1");
            await WriteWithStreamAsync(path, "Line 2", append: true);

            Console.WriteLine("Contents via StreamReader:");
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                Console.WriteLine(await reader.ReadLineAsync());
            }

            // Handle IO exceptions
            try
            {
                File.Delete(path);
                using var stream = File.OpenRead(path); // will throw
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error: {ex.Message}");
            }
        }

        private static async Task WriteWithStreamAsync(string path, string line, bool append = false)
        {
            using var stream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(line);
        }
    }
}
