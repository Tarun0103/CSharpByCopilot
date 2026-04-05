using System;
using System.Text;

namespace Phase5_Real_Interview_Concepts
{
    internal class StringHandlingExamples
    {
        private static void Main()
        {
            // string is immutable
            string s = "Hello";
            s += ", World"; // creates a new string instance
            Console.WriteLine(s);

            // Repeated concatenation is expensive
            string large = "";
            for (int i = 0; i < 1000; i++)
            {
                large += i; // repeated allocations
            }
            Console.WriteLine($"Length of large string: {large.Length}");

            // StringBuilder is more efficient for many modifications
            var sb = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.Append(i);
            }
            string built = sb.ToString();
            Console.WriteLine($"Length of built string: {built.Length}");

            // Common string methods
            string example = "Hello, C# World!";
            Console.WriteLine(example.Substring(0, 5));
            Console.WriteLine(example.Contains("C#"));
            Console.WriteLine(example.Replace("C#", "CSharp"));
            Console.WriteLine(string.Join("|", example.Split(' ')));
        }
    }
}
