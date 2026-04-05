using System;
using System.Collections.Generic;

namespace Phase1_Core_CSharp_Fundamentals
{
    internal class ControlFlowExamples
    {
        private static void Main()
        {
            // if / else
            int temperature = 72;
            if (temperature > 80)
            {
                Console.WriteLine("It's hot.");
            }
            else if (temperature >= 60)
            {
                Console.WriteLine("It's comfortable.");
            }
            else
            {
                Console.WriteLine("It's cold.");
            }

            // Modern switch expression
            int score = 85;
            string grade = score switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                _ => "F"
            };

            Console.WriteLine($"Score {score} -> Grade {grade}");

            // for loop (indexed)
            var numbers = new[] { 10, 20, 30, 40, 50 };
            for (int i = 0; i < numbers.Length; i++)
            {
                // Modify in-place using index.
                numbers[i] += 1;
            }

            // foreach loop (read-only iteration)
            Console.WriteLine("Numbers:");
            foreach (var n in numbers)
            {
                Console.WriteLine(n);
            }

            // while loop (condition-driven)
            int counter = 0;
            while (counter < 3)
            {
                Console.WriteLine($"while loop iteration {counter}");
                counter++;
            }

            // Demonstration: when foreach can't modify collection directly
            List<string> colors = new() { "Red", "Green", "Blue" };
            for (int i = 0; i < colors.Count; i++)
            {
                colors[i] = colors[i].ToUpper();
            }
            Console.WriteLine("Modified colors: " + string.Join(", ", colors));
        }
    }
}
