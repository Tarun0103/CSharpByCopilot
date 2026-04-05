# Phase 18: Bit Manipulation

## What Is Bit Manipulation?

Every integer is stored in binary (1s and 0s). Bit manipulation operates **directly on those bits** using bitwise operators — no intermediate conversions.

Benefits:
- Extremely fast (single CPU instruction)
- Memory-efficient (pack multiple booleans into one integer)
- Elegant solutions to problems that otherwise need complex loops

---

## Binary Representation

| Decimal | Binary (8-bit) |
|---------|---------------|
| 0 | 00000000 |
| 1 | 00000001 |
| 5 | 00000101 |
| 15 | 00001111 |
| 255 | 11111111 |
| -1 | 11111111 (two's complement) |

**Two's Complement** (how C# stores negative numbers):
- `-n = ~n + 1` (flip all bits, add 1)
- `-1 = 11111111...1111` in binary
- This makes arithmetic consistent (no separate negative zero)

---

## Bitwise Operators

### AND (`&`) — both bits must be 1
```
  1010  (10)
& 1100  (12)
------
  1000  (8)
```
Use case: Check/clear specific bits, mask off bits.

### OR (`|`) — at least one bit must be 1
```
  1010  (10)
| 1100  (12)
------
  1110  (14)
```
Use case: Set specific bits.

### XOR (`^`) — exactly one bit must be 1 (not both)
```
  1010  (10)
^ 1100  (12)
------
  0110  (6)
```
**XOR special properties:**
- `n ^ n = 0` (same values cancel)
- `n ^ 0 = n` (XOR with zero = identity)
- `a ^ b ^ a = b` (used for swap without temp variable!)
- Commutative: `a ^ b = b ^ a`

### NOT (`~`) — flip all bits
```
~5 = ~00000101 = 11111010 = -6  (in two's complement)
```
Note: `~n = -(n+1)` always in two's complement.

### Left Shift (`<<`) — multiply by 2
```
5 << 1 = 10  (5 * 2¹)
5 << 2 = 20  (5 * 2²)
```

### Right Shift (`>>`) — divide by 2
```
20 >> 1 = 10  (20 / 2¹)
20 >> 2 = 5   (20 / 2²)
```
Note: `>>` is arithmetic right shift (preserves sign bit). Unsigned: use `>>>` in C# 11+.

---

## Essential Bit Tricks Reference Card

| Operation | Code | Explanation |
|-----------|------|-------------|
| Check if odd | `n & 1 == 1` | Last bit is 1 for odd numbers |
| Check if even | `n & 1 == 0` | Last bit is 0 for even numbers |
| Check bit k | `(n >> k) & 1 == 1` | Shift bit to position 0, check |
| Set bit k | `n \| (1 << k)` | OR with 1 at position k |
| Clear bit k | `n & ~(1 << k)` | AND with 0 at position k |
| Toggle bit k | `n ^ (1 << k)` | XOR with 1 at position k |
| Check power of 2 | `n > 0 && (n & (n-1)) == 0` | Powers of 2 have exactly one set bit |
| Count set bits | Brian Kernighan: `while(n>0){n &= n-1; count++;}` | Each iter clears lowest set bit |
| Lowest set bit | `n & (-n)` | -n = ~n+1, isolates lowest bit |
| Clear lowest set bit | `n & (n-1)` | Standard trick |
| Swap two numbers | `a^=b; b^=a; a^=b;` | No temp variable needed |
| Multiply by 2^k | `n << k` | Faster than multiplication |
| Divide by 2^k | `n >> k` | Faster than division |
| Absolute value | `x ^ (x >> 31)` (then add carry) | Without branching |

---

## XOR Properties — Why They Matter

XOR's property that `n ^ n = 0` enables clever interview tricks:

**Find the single non-duplicate in an array:**
```
[4, 1, 2, 1, 2, 4, 3]
XOR all: 4^1^2^1^2^4^3 = (4^4)^(1^1)^(2^2)^3 = 0^0^0^3 = 3
```
All duplicates cancel, leaving the lonely number!

**In-place swap:**
```
a = a ^ b
b = a ^ b  = (a^b)^b = a
a = a ^ b  = (a^b)^a = b
```

---

## Bitmask — Representing Sets with Integers

A single `int` (32 bits) can represent a **set of 32 boolean values**.
- Bit position i = 1 means "element i is in the set"
- All subsets of n elements = 2^n integers from 0 to 2^n - 1

```csharp
// Represent subsets of {0, 1, 2, 3, 4}
int set = 0b10101 = 21;  // elements {0, 2, 4} are selected

// Add element 1:    set | (1 << 1)
// Remove element 2: set & ~(1 << 2)
// Toggle element 3: set ^ (1 << 3)
// Has element 0?    (set >> 0) & 1 == 1
```

This enables **"bitmask DP"** — represent visited states as integers, fit entire DP table in memory.

---

## Common Interview Problems

| Problem | Key Insight |
|---------|-------------|
| Single Number (one duplicate rest) | XOR all elements → duplicates cancel |
| Missing Number (0..n) | XOR all elements + all indices |
| Hamming Distance | `int diff = x ^ y; count set bits in diff` |
| Reverse Bits | Shift + mask loop from LSB to MSB |
| Power of Two Check | `n > 0 && (n & (n-1)) == 0` |
| Count Set Bits (Popcount) | Brian Kernighan's algorithm |
| Print All Subsets | Iterate i from 0 to 2^n-1; check each bit |
| Two Singles (find two unique from array of pairs) | XOR all → get a^b. Partition by set bit. |

---

## Operator Precedence Warning

Common bug: bitwise operators have lower precedence than comparison operators!

```csharp
// WRONG — evaluates as: 5 & (3 == 1) → 5 & false → error
if (5 & 3 == 1) ...

// CORRECT — parentheses required
if ((5 & 3) == 1) ...
```

Always parenthesize bitwise expressions!

---

## Interview Questions & Answers

**Q1: How would you check if a number is a power of 2 using bit manipulation?**

**A:** A power of 2 has exactly one set bit (e.g., 8 = 1000, 16 = 10000). `n & (n-1)` clears the lowest set bit — if result is 0, there was only one set bit. Full check: `n > 0 && (n & (n-1)) == 0`. The `n > 0` guard handles zero (which is not a power of 2). Time: O(1), no loops needed.

---

**Q2: How does Brian Kernighan's algorithm count set bits?**

**A:** The trick: `n & (n-1)` clears the lowest set bit of n. So we loop: `while (n > 0) { n &= n-1; count++; }`. We iterate exactly once per set bit, not per total bit. For n=7 (0111): iteration 1: 0111 & 0110 = 0110 (count=1), iteration 2: 0110 & 0101 = 0100 (count=2), iteration 3: 0100 & 0011 = 0000 (count=3). Done in 3 iterations instead of 32.

---

**Q3: Given an array where every element appears twice except one, find the single element. O(n) time, O(1) space.**

**A:** XOR all elements together. Pairs cancel: `x ^ x = 0`. The single element XORed with 0 remains: `x ^ 0 = x`. For `[4,1,2,1,2]`: `4^1^2^1^2 = 4^(1^1)^(2^2) = 4^0^0 = 4`. No hash map, no sort — pure O(n) time and O(1) space.

---

**Q4: What practical applications does bit manipulation have in real-world code?**

**A:** 
1. **Permissions/flags** — Unix file permissions (rwxrwxrwx = 9 bits), database column flags
2. **Bloom filters** — bit arrays for probabilistic set membership
3. **Cryptography** — XOR-based encryption, hash functions
4. **Network programming** — IP address subnet masking (`ip & subnetMask`)
5. **Graphics** — pixel operations, color channel extraction
6. **State compression DP** — represent visited cities as bitmask (Traveling Salesman)
7. **Game programming** — board states as bitboards (chess engines)

---

**Q5: How do you swap two numbers without a temporary variable using XOR?**

**A:** Three XOR operations: `a ^= b; b ^= a; a ^= b;`. Step by step: after `a ^= b`: a holds `a_original ^ b`. After `b ^= a`: b holds `b ^ (a_original ^ b)` = `a_original`. After `a ^= b`: a holds `(a_original ^ b) ^ a_original` = `b`. Caveats: (1) doesn't work if a and b are the same variable/same memory location — would zero out both. (2) Prefer the temp-variable approach in real code for readability; XOR swap is mainly an interview trick.

---

**Q6: Find two numbers that appear once in an array where everything else appears twice.**

**A:** 1) XOR all elements: `xor = a ^ b` (the two unique numbers). 2) Find any set bit in `xor` (e.g., rightmost: `diff = xor & (-xor)`). This bit is 1 in exactly one of a or b. 3) Partition array into two groups: elements with this bit set vs not set. 4) XOR each group separately → pairs cancel, leaving a and b respectively. Time: O(n), Space: O(1).

---

## Scenario-Based Questions

**Scenario 1:** Your application has 50 feature flags that are enabled/disabled per user. Currently you store them as a `bool[]` taking 50+ bytes per user. With 10 million users, this is 500 MB just for flags. How do you optimize?

**Answer:** Store flags as two `long` values (64 bits each = 128 bit flags capacity). For 50 flags, one `long` suffices. Set bit k = flag k is enabled: `flags |= (1L << k)` to enable, `flags &= ~(1L << k)` to disable, `(flags >> k) & 1L == 1L` to check. Memory: 8 bytes per user × 10M = 80 MB vs 500 MB. That's an 85% memory reduction. Also faster CPU operations (single CPU instruction vs. array indexing).

---

**Scenario 2:** You need to enumerate all possible subsets of a string's unique characters for a word puzzle game. The string has at most 20 characters. How do you efficiently generate all subsets?

**Answer:** With n ≤ 20 characters, there are 2^20 ≈ 1 million subsets — manageable. Use bitmask enumeration: iterate `mask` from 0 to `(1 << n) - 1`. For each mask, the subset contains character `i` if bit `i` in mask is set: `if ((mask >> i) & 1 == 1) → include chars[i]`. This is O(n × 2^n) total. The elegant part: the integer `mask` itself represents the state of all inclusions, and incrementing naturally cycles through all 2^n combinations.

---

**Scenario 3:** Design a compact in-memory role-permission system. Users have roles; roles have permissions. There are 32 possible permissions. Check if a user has a specific permission must be O(1).

**Answer:** Store each role's permissions as a single `uint` (32 bits). Store each user's combined permissions as a bitwise OR of their roles' permission masks. To check permission: `(userPermissions & (1u << permissionId)) != 0`. To grant: `userPermissions |= (1u << permissionId)`. To revoke: `userPermissions &= ~(1u << permissionId)`. This is exactly how Unix/Linux file permissions work. The entire permission check is a single CPU instruction. For more than 32 permissions, use `ulong` (64 bits) or a bit array.

---

## Common Mistakes

1. **Operator precedence** — `n & 3 == 0` evaluates as `n & (3 == 0)`. Always use parentheses: `(n & 3) == 0`
2. **Signed vs unsigned shifts** — `>>` is arithmetic (sign-preserving) in C#, `>>>` (C# 11+) is logical shift
3. **Checking power of 2 without n>0` guard** — 0 is not a power of 2, but `0 & (0-1) = 0 & -1 = 0` passes the check
4. **XOR swap on same variable** — `a = a ^ a = 0` destroys the value; always check a != b first
5. **Off-by-one in bit positions** — bit positions are 0-indexed; bit 0 is the least significant bit
6. **Forgetting `L` suffix for long shifts** — `1 << 40` overflows int; use `1L << 40`
7. **Assuming ~n = -n** — it's `~n = -(n+1)`, not `-n`. To negate: use `-n` not `~n`
