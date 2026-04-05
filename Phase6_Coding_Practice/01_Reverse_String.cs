using System;

namespace Phase6_Coding_Practice
{
    internal class ReverseString
    {
        private static void Main()
        {
            string input = "hello";
            Console.WriteLine($"Input: {input}");

            string reversed = Reverse(input);
            Console.WriteLine($"Reversed: {reversed}");

            string alt = ReverseUsingStringBuilder(input);
            Console.WriteLine($"Reversed (StringBuilder): {alt}");
        }

        private static string Reverse(string s)
        {
            var chars = s.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        private static string ReverseUsingStringBuilder(string s)
        {
            var builder = new System.Text.StringBuilder(s.Length);
            for (int i = s.Length - 1; i >= 0; i--)
            {
                builder.Append(s[i]);
            }
            return builder.ToString();
        }
    }
}
