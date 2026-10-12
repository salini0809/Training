// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to convert string to Double
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main () {
      var testCases = new string[] {"+","e5","12345","+123","-123","123.45",".45","45.","1e5",
         "1e+5","1e-5","1.25e3","1.25E3","  123.45  ","000123","1e308","1e-308","NaN","","1.2.3",
         "1e+","1e2e3","--123","12abc","1e309","1e-325"};
      foreach (string testCase in testCases) {
         double result = DoubleParse (testCase);
         double expected = double.TryParse (testCase, out double actual) ? actual : NAN;
         WriteLine ($"Input: \"{testCase}\"\n Result: {result}\n Expected:{expected}\n");
      }
   }

   // Converts a string to a double, handling signs, integer and decimal parts, and scientific notation.
   static double DoubleParse (string input) {
      input = input.Trim ();
      // Rejects empty or whitespace-only input.
      if (input.Length == 0) return NAN;
      int position = 0;
      bool hasDigits = false, isNegative = false;
      double number = 0;
      // Handles an optional leading positive or negative sign.
      if (input[0] == '+' || input[0] == '-') {
         isNegative = input[0] == '-';
         position++;
      }
      // Rejects inputs that do not start with a digit or decimal point after the optional sign.
      if (position == input.Length || (!char.IsDigit (input[position]) && input[position] != '.'))
         return NAN;
      // Parses the integer part by processing digits from left to right.
      while (position < input.Length && char.IsDigit (input[position])) {
         int digit = input[position] - '0';
         number = number * 10 + digit;
         hasDigits = true;
         position++;
      }
      // Parses the optional fractional part, including inputs such as ".45" and "45.".
      if (position < input.Length && input[position] == '.') {
         position++;
         double decimalNumber = 0;
         int decimalDigits = 0;
         while (position < input.Length && char.IsDigit (input[position])) {
            int digit = input[position] - '0';
            decimalNumber = decimalNumber * 10 + digit;
            decimalDigits++;
            hasDigits = true;
            position++;
         }
         number += decimalNumber / Math.Pow (10, decimalDigits);
      }
      // Rejects inputs that contain no digits, such as "." or "+.".
      if (!hasDigits) return NAN;
      // Parses the optional exponent, supporting 'e' or 'E' with an optional sign.
      if (position < input.Length && (input[position] == 'e' || input[position] == 'E')) {
         position++;
         bool expNegative = false;
         if (position < input.Length && (input[position] == '+' || input[position] == '-')) {
            expNegative = input[position] == '-';
            position++;
         }
         int exponent = 0;
         bool hasExpDigits = false;
         while (position < input.Length && char.IsDigit (input[position])) {
            int digit = input[position] - '0';
            exponent = exponent * 10 + digit;
            hasExpDigits = true;
            position++;
         }
         // Rejects incomplete exponents such as "1e", "1e+", and "1e-".
         if (!hasExpDigits) return NAN;
         exponent = expNegative ? -exponent : exponent;
         number *= Math.Pow (10, exponent);
      }
      // Rejects invalid trailing characters or malformed numbers such as "12abc" and "1.2.3".
      if (position != input.Length) return NAN;
      // Applies the original sign and returns the final parsed value.
      return isNegative ? -number : number;
   }

   const double NAN = double.NaN;
}