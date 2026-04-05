using System;
using System.Collections.Generic;

namespace Phase2_OOP
{
    internal class EncapsulationExamples
    {
        private static void Main()
        {
            // 1) Create a BankAccount instance using the parameterized constructor.
            //    This demonstrates construction that establishes a valid starting state.
            var bankAccount = new BankAccount("Alice", 100m);

            // 2) Use public methods to modify state. Methods encapsulate validation
            //    and ensure invariants (e.g., balance cannot go negative without
            //    an explicit allowed operation).
            bankAccount.Deposit(50m);
            bankAccount.Withdraw(30m);

            // 3) Read-only accessors expose state for consumers while keeping
            //    mutation controlled by the type itself.
            Console.WriteLine($"Balance: {bankAccount.Balance:C}");

            // 4) Demonstrate adding a manual transaction note (internal helper
            //    that still respects encapsulation of the transactions list).
            bankAccount.AddTransaction("Manual note: Customer service fee waived");

            // 5) Show transactions via the public read-only view. Consumers cannot
            //    mutate the underlying list directly which prevents external
            //    code from breaking invariants or corrupting internal state.
            Console.WriteLine("Transactions:");
            foreach (var t in bankAccount.Transactions)
            {
                Console.WriteLine(t);
            }

            // Notes for interviews:
            // - Attempting to assign to Balance directly is not possible because
            //   there is no public setter. Example: bankAccount.Balance = -100; // error
            // - Attempting to access _balance or _transactions from outside the
            //   BankAccount class is not possible because they are private fields.
        }

        private class BankAccount
        {
            // Private field holding mutable balance. Keeping it private prevents
            // arbitrary external mutation and forces callers to use methods that
            // can enforce validation and invariants.
            private decimal _balance;

            // Internal list used to record transaction history. We keep it private
            // and expose a read-only view to callers to prevent external mutation.
            private readonly List<string> _transactions = new();

            // Public read-only ownership property. There is no setter because
            // ownership should not change after construction in this example.
            public string Owner { get; }

            // Expose a read-only view so callers can iterate transactions but
            // cannot add/remove items directly. This preserves encapsulation.
            public IReadOnlyList<string> Transactions => _transactions;

            // Read-only balance accessor. To change the balance callers must use
            // Deposit/Withdraw methods which perform validation and mutation.
            public decimal Balance => _balance;

            // Constructor: enforces that every BankAccount starts with a valid
            // owner and a non-negative initial balance (could validate here).
            public BankAccount(string owner, decimal initialBalance)
            {
                if (string.IsNullOrWhiteSpace(owner))
                    throw new ArgumentException("Owner must be provided", nameof(owner));

                if (initialBalance < 0)
                    throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative");

                Owner = owner;
                _balance = initialBalance;
                _transactions.Add($"Account opened with {initialBalance:C}");
            }

            // Deposit: validates input, updates private balance, and records a transaction.
            public void Deposit(decimal amount)
            {
                if (amount <= 0)
                    throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive.");

                _balance += amount;
                _transactions.Add($"Deposit {amount:C}");
            }

            // Withdraw: validates input and ensures balance cannot go negative.
            public void Withdraw(decimal amount)
            {
                if (amount <= 0)
                    throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal must be positive.");

                if (amount > _balance)
                    throw new InvalidOperationException("Insufficient funds.");

                _balance -= amount;
                _transactions.Add($"Withdraw {amount:C}");
            }

            // AddTransaction: internal-style helper for recording notes. It remains
            // public to demonstrate how callers can add contextual notes without
            // being able to change the underlying data structures directly.
            public void AddTransaction(string note)
            {
                if (string.IsNullOrWhiteSpace(note))
                    note = "(empty note)";

                _transactions.Add(note);
            }

            // Interview talking points (not executed):
            // - Why use IReadOnlyList<T> instead of exposing List<T>? To prevent
            //   callers from calling Add/Remove and breaking invariants.
            // - Why validate in methods (Deposit/Withdraw) rather than in public setters?
            //   Because setters would allow partial mutation; method-based APIs allow
            //   enforcing operation-level semantics and atomic checks.
            // - Thread-safety: this class is not thread-safe. For multi-threaded use
            //   you'd need locking or use of concurrent collections.
        }
    }
}
