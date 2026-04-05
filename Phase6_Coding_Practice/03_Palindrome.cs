using System;
using System.Linq;

namespace Phase6_Coding_Practice
{
    internal class Palindrome
    {
        private static void Main()
        {
            string input = "A man, a plan, a canal: Panama";
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Is palindrome: {IsPalindrome(input)}");
        }

        private static bool IsPalindrome(string s)
        {
            // Normalize: keep only letters/digits, ignore case
            var cleaned = new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            int left = 0;
            int right = cleaned.Length - 1;

            while (left < right)
            {
                if (cleaned[left] != cleaned[right])
                    return false;
                left++;
                right--;
            }

            return true;
        }
    }
}
