using System;

namespace Phase3_Intermediate_CSharp
{
    internal class ExceptionHandlingExamples
    {
        private static void Main()
        {
            try
            {
                int result = Divide(10, 0);
                Console.WriteLine($"Result: {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Caught DivideByZeroException: {ex.Message}");
                // Re-throw while preserving stack trace
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught general exception: {ex.Message}");
                // Rethrow with stack trace of this method (not recommended)
                // throw ex;
            }
            finally
            {
                Console.WriteLine("Finally block always runs (cleanup, release resources)");
            }

            // Custom exception usage
            try
            {
                ValidateAge(-5);
            }
            catch (ValidationException vex)
            {
                Console.WriteLine($"Validation failed: {vex.Message}");
            }
        }

        private static int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Divider cannot be zero.");

            return a / b;
        }

        private static void ValidateAge(int age)
        {
            if (age < 0 || age > 120)
                throw new ValidationException("Age must be between 0 and 120.");
        }

        private class ValidationException : Exception
        {
            public ValidationException(string message)
                : base(message)
            {
            }
        }
    }
}
