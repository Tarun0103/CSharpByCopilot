using System;
using System.Collections.Generic;

namespace Phase6_Coding_Practice
{
    internal class Fibonacci
    {
        private static void Main()
        {
            Console.WriteLine("First 10 Fibonacci numbers:");
            foreach (var n in Generate(10))
            {
                Console.WriteLine(n);
            }

            Console.WriteLine($"Fibonacci(20) = {Compute(20)}");
            Console.WriteLine($"Fibonacci(40) (memoized) = {ComputeMemoized(40)}");
        }

        // Iterative linear-time generator
        private static IEnumerable<long> Generate(int count)
        {
            long a = 0, b = 1;
            for (int i = 0; i < count; i++)
            {
                yield return a;
                (a, b) = (b, a + b);
            }
        }

        // Simple recursive (exponential time)
        private static long Compute(int n)
        {
            if (n <= 1) return n;
            return Compute(n - 1) + Compute(n - 2);
        }

        // Memoized recursion (linear time)
        private static long ComputeMemoized(int n)
        {
            var cache = new Dictionary<int, long> { [0] = 0, [1] = 1 };
            return ComputeMemoizedInternal(n, cache);
        }

        private static long ComputeMemoizedInternal(int n, Dictionary<int, long> cache)
        {
            if (cache.TryGetValue(n, out var value))
                return value;

            long result = ComputeMemoizedInternal(n - 1, cache) + ComputeMemoizedInternal(n - 2, cache);
            cache[n] = result;
            return result;
        }
    }
}
