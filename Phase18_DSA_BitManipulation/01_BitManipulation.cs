// ============================================================
// PHASE 18: BIT MANIPULATION
// Topics: Binary representation, bitwise operators,
//         common bit tricks, interview problems
// Run this file in a Console Application project
// ============================================================

using System;

// ============================================================
// SECTION 1: BINARY REPRESENTATION & BITWISE OPERATORS
//
// Computers store everything in binary (base-2): 0s and 1s (bits)
// int in C# = 32 bits:  5 = 00000000 00000000 00000000 00000101
//
// OPERATORS:
//   &   AND   — 1 if BOTH bits are 1
//   |   OR    — 1 if EITHER bit is 1
//   ^   XOR   — 1 if bits are DIFFERENT
//   ~   NOT   — flips all bits
//   <<  Left shift  — multiply by 2^n
//   >>  Right shift — divide by 2^n (arithmetic, sign-extended)
//   >>> Unsigned right shift — fills with 0 (no sign extension) [C# 11+]
// ============================================================

class BitwiseOperators
{
    public static void Demonstrate()
    {
        Console.WriteLine("=== Bitwise Operators ===");
        int a = 12;  //  12 = 1100 in binary
        int b = 10;  //  10 = 1010 in binary

        // AND: bit is 1 only if BOTH bits are 1
        Console.WriteLine($"  {a} & {b}  = {a & b}   (1100 & 1010 = 1000 = 8)");

        // OR: bit is 1 if EITHER bit is 1
        Console.WriteLine($"  {a} | {b}  = {a | b}  (1100 | 1010 = 1110 = 14)");

        // XOR: bit is 1 if bits are DIFFERENT
        Console.WriteLine($"  {a} ^ {b}  = {a ^ b}   (1100 ^ 1010 = 0110 = 6)");

        // NOT: flips ALL bits (bitwise complement)
        // ~12 = -(12+1) = -13 (two's complement)
        Console.WriteLine($"  ~{a}     = {~a}  (two's complement: -(n+1))");

        // LEFT SHIFT: shifts bits left, fills with 0 on right = multiply by 2^n
        Console.WriteLine($"  {a} << 1  = {a << 1}  (1100 << 1 = 11000 = 24 = 12*2)");
        Console.WriteLine($"  {a} << 2  = {a << 2}  (1100 << 2 = 110000 = 48 = 12*4)");

        // RIGHT SHIFT: shifts bits right = divide by 2^n (floor)
        Console.WriteLine($"  {a} >> 1  = {a >> 1}   (1100 >> 1 = 0110 = 6 = 12/2)");
        Console.WriteLine($"  {a} >> 2  = {a >> 2}   (1100 >> 2 = 0011 = 3 = 12/4)");
    }
}

// ============================================================
// SECTION 2: COMMON BIT TRICKS
// These are patterns that appear very frequently in interviews
// ============================================================

class BitTricks
{
    // --- Check if a number is ODD or EVEN ---
    // The last bit (bit 0) is 1 for odd, 0 for even
    // n & 1 == 1 → odd;   n & 1 == 0 → even
    public static bool IsOdd(int n) => (n & 1) == 1;

    // --- Check if n-th bit is SET (1) ---
    // Create a MASK with only bit n set: (1 << n)
    // AND with number: if result ≠ 0, bit n is set
    // Example: n=5 (0101), bit 0 set? 0101 & 0001 = 0001 ≠ 0 → YES
    public static bool IsBitSet(int num, int bitPos) => (num & (1 << bitPos)) != 0;

    // --- SET the n-th bit (turn it to 1) ---
    // OR with mask (1 << n): forces bit n to 1, others unchanged
    public static int SetBit(int num, int bitPos) => num | (1 << bitPos);

    // --- CLEAR the n-th bit (turn it to 0) ---
    // AND with inverted mask: ~(1 << n) = all 1s except bit n
    public static int ClearBit(int num, int bitPos) => num & ~(1 << bitPos);

    // --- TOGGLE the n-th bit (flip it) ---
    // XOR with mask: 0^0=0, 0^1=1, 1^0=1, 1^1=0 (flips only bit n)
    public static int ToggleBit(int num, int bitPos) => num ^ (1 << bitPos);

    // --- Check if number is a POWER OF 2 ---
    // A power of 2 has exactly ONE bit set.
    // n: 0001 0000 (= 16)
    // n-1: 0000 1111 (= 15)
    // n & (n-1) == 0 → true for all powers of 2
    // Note: 0 is NOT a power of 2, so check n > 0
    public static bool IsPowerOfTwo(int n) => n > 0 && (n & (n - 1)) == 0;

    // --- COUNT set bits (Brian Kernighan's Algorithm) ---
    // n & (n-1) removes the LOWEST set bit.
    // Count how many times we can do this before n becomes 0.
    // Time: O(number of set bits), vs O(32) for naive approach
    public static int CountSetBits(int n)
    {
        int count = 0;
        while (n != 0)
        {
            n = n & (n - 1);  // removes lowest set bit
            count++;
        }
        return count;
    }

    // --- SWAP two numbers WITHOUT a temp variable ---
    // Using XOR: a^b^b = a,  a^b^a = b
    // a = a ^ b;  → a now contains a XOR b
    // b = a ^ b;  → b = (a^b) ^ b = a
    // a = a ^ b;  → a = (a^b) ^ a = b
    public static void SwapXOR(ref int a, ref int b)
    {
        a = a ^ b;   // a = a XOR b
        b = a ^ b;   // b = (a XOR b) XOR b = a
        a = a ^ b;   // a = (a XOR b) XOR a = b
    }

    // --- Get the LOWEST SET BIT ---
    // n & (-n) isolates the lowest set bit
    // -n in two's complement = ~n + 1
    // Example: n = 12 (1100), -n = (0011+1) = 0100, n & -n = 0100 = 4
    public static int GetLowestSetBit(int n) => n & (-n);

    // --- Turn off the LOWEST SET BIT ---
    // n & (n-1) removes the lowest set bit
    // Example: n = 12 (1100), n-1 = 11 (1011), n & (n-1) = 1000 = 8
    public static int ClearLowestSetBit(int n) => n & (n - 1);

    // --- Reverse all bits (32-bit integer) ---
    public static uint ReverseBits(uint n)
    {
        uint result = 0;
        for (int i = 0; i < 32; i++)
        {
            result = (result << 1) | (n & 1);  // take lowest bit of n, append to result
            n >>= 1;
        }
        return result;
    }
}

// ============================================================
// SECTION 3: INTERVIEW PROBLEMS
// ============================================================

class BitInterviewProblems
{
    // --- PROBLEM 1: Single Number ---
    // PROBLEM: Every element appears twice except ONE. Find it.
    // XOR MAGIC: x ^ x = 0, x ^ 0 = x
    // XOR all elements → pairs cancel out → single number remains
    // Time: O(n), Space: O(1)
    public static int SingleNumber(int[] nums)
    {
        int result = 0;
        foreach (int n in nums)
            result ^= n;       // XOR: duplicates cancel to 0
        return result;
    }

    // --- PROBLEM 2: Missing Number ---
    // PROBLEM: Array of n distinct numbers from 0 to n. One number missing. Find it.
    // Approach 1: XOR all indices 0..n with all nums → missing pair remains
    // Approach 2 (cleaner): sum formula 0+1+...+n = n*(n+1)/2, subtract actual sum
    public static int MissingNumber(int[] nums)
    {
        int n = nums.Length;
        int expectedSum = n * (n + 1) / 2;
        int actualSum = 0;
        foreach (int x in nums) actualSum += x;
        return expectedSum - actualSum;
    }

    // XOR approach (demonstrates bit manipulation)
    public static int MissingNumberXOR(int[] nums)
    {
        int xor = 0;
        for (int i = 0; i <= nums.Length; i++)
            xor ^= i;          // XOR all expected indices
        foreach (int n in nums)
            xor ^= n;          // XOR all actual numbers
        return xor;            // missing number remains
    }

    // --- PROBLEM 3: Hamming Distance ---
    // PROBLEM: Count number of positions where two integers differ in binary.
    // XOR gives 1 for differing bits → count set bits in XOR result.
    public static int HammingDistance(int x, int y)
    {
        int xorResult = x ^ y;
        return BitTricks.CountSetBits(xorResult);
    }

    // --- PROBLEM 4: Reverse Bits ---
    // PROBLEM: Reverse the bits of a 32-bit unsigned integer.
    public static uint ReverseBits(uint n) => BitTricks.ReverseBits(n);

    // --- PROBLEM 5: Power of Two ---
    public static bool IsPowerOfTwo(int n) => BitTricks.IsPowerOfTwo(n);

    // --- PROBLEM 6: Subsets using Bitmask ---
    // PROBLEM: Generate all subsets of a set using bitmask.
    // For n elements, there are 2^n subsets.
    // Each number from 0 to (2^n - 1) represents a subset via its bits.
    // If bit i is set in the mask → include element i.
    public static void PrintAllSubsets(int[] arr)
    {
        int n = arr.Length;
        int totalSubsets = 1 << n;  // 2^n

        for (int mask = 0; mask < totalSubsets; mask++)
        {
            Console.Write("    {");
            bool first = true;
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)  // is bit i set?
                {
                    if (!first) Console.Write(", ");
                    Console.Write(arr[i]);
                    first = false;
                }
            }
            Console.WriteLine("}");
        }
    }

    // --- PROBLEM 7: Two Numbers Appearing Once (all others appear twice) ---
    // XOR all → x ^ y (since all pairs cancel)
    // Find any set bit in x^y → it differs between x and y
    // Use that bit to partition array into two groups → XOR each group
    public static (int, int) TwoSingleNumbers(int[] nums)
    {
        int xorAll = 0;
        foreach (int n in nums) xorAll ^= n;     // xorAll = x ^ y

        // Find the rightmost set bit (differs between x and y)
        int diffBit = xorAll & (-xorAll);

        int x = 0, y = 0;
        foreach (int n in nums)
        {
            if ((n & diffBit) != 0) x ^= n;     // group 1: bit is set
            else                    y ^= n;      // group 2: bit is not set
        }

        return (x, y);
    }
}

// ============================================================
// MAIN PROGRAM
// ============================================================
class BitManipulationProgram
{
    static void Main()
    {
        Console.WriteLine("======================================================");
        Console.WriteLine("PHASE 18: BIT MANIPULATION");
        Console.WriteLine("======================================================");

        BitwiseOperators.Demonstrate();

        Console.WriteLine("\n=== Common Bit Tricks ===");
        Console.WriteLine($"  IsOdd(5):         {BitTricks.IsOdd(5)}   (5 = 101, last bit = 1)");
        Console.WriteLine($"  IsOdd(4):         {BitTricks.IsOdd(4)}  (4 = 100, last bit = 0)");
        Console.WriteLine($"  IsBitSet(5, 0):   {BitTricks.IsBitSet(5, 0)}   (5=101, bit 0 is 1)");
        Console.WriteLine($"  IsBitSet(5, 1):   {BitTricks.IsBitSet(5, 1)}  (5=101, bit 1 is 0)");
        Console.WriteLine($"  SetBit(5, 1):     {BitTricks.SetBit(5, 1)}   (5=101, set bit 1 → 111 = 7)");
        Console.WriteLine($"  ClearBit(7, 1):   {BitTricks.ClearBit(7, 1)}   (7=111, clear bit 1 → 101 = 5)");
        Console.WriteLine($"  ToggleBit(5, 1):  {BitTricks.ToggleBit(5, 1)}   (5=101, toggle bit 1 → 111 = 7)");
        Console.WriteLine($"  IsPowerOfTwo(16): {BitTricks.IsPowerOfTwo(16)}   (16 = 10000, one bit set)");
        Console.WriteLine($"  IsPowerOfTwo(12): {BitTricks.IsPowerOfTwo(12)}  (12 = 1100, two bits set)");
        Console.WriteLine($"  CountSetBits(255):{BitTricks.CountSetBits(255)}   (255 = 11111111)");
        Console.WriteLine($"  CountSetBits(13): {BitTricks.CountSetBits(13)}   (13 = 1101, three 1s)");

        int x = 7, y = 3;
        Console.WriteLine($"  Before swap: x={x}, y={y}");
        BitTricks.SwapXOR(ref x, ref y);
        Console.WriteLine($"  After XOR swap: x={x}, y={y}");

        Console.WriteLine("\n=== Interview Problems ===");
        int[] nums1 = { 4, 1, 2, 1, 2 };
        Console.WriteLine($"  SingleNumber([4,1,2,1,2]):      {BitInterviewProblems.SingleNumber(nums1)}");

        int[] nums2 = { 3, 0, 1 };
        Console.WriteLine($"  MissingNumber([3,0,1]):         {BitInterviewProblems.MissingNumber(nums2)}  (missing: 2)");
        Console.WriteLine($"  MissingNumber XOR([3,0,1]):     {BitInterviewProblems.MissingNumberXOR(nums2)}");

        Console.WriteLine($"  HammingDistance(1, 4):          {BitInterviewProblems.HammingDistance(1, 4)}   (001 vs 100 → 2 differing bits)");
        Console.WriteLine($"  IsPowerOfTwo(32):               {BitInterviewProblems.IsPowerOfTwo(32)}");

        Console.WriteLine("\n  All subsets of [1, 2, 3]:");
        BitInterviewProblems.PrintAllSubsets(new[] { 1, 2, 3 });

        int[] pairs = { 1, 2, 1, 3, 2, 5 };
        var (a, b) = BitInterviewProblems.TwoSingleNumbers(pairs);
        Console.WriteLine($"\n  Two single numbers in [1,2,1,3,2,5]: {a} and {b}  (expected: 3 and 5)");

        Console.WriteLine("\n======================================================");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  1. XOR: x^x=0, x^0=x — cancels duplicates");
        Console.WriteLine("  2. n & (n-1) removes lowest set bit");
        Console.WriteLine("  3. n & (-n) isolates lowest set bit");
        Console.WriteLine("  4. n & 1 checks parity (odd/even)");
        Console.WriteLine("  5. 1 << n creates a mask for bit n");
        Console.WriteLine("  6. Bitmask DP: enumerate all 2^n subsets");
        Console.WriteLine("  7. Power of 2: n>0 && (n&(n-1))==0");
        Console.WriteLine("======================================================");
    }
}
