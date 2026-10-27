// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Evaluates mathematical expressions interactively and through predefined test cases.
// ------------------------------------------------------------------------------------------------
using static System.Console;
using static System.ConsoleColor;

#region class Program -----------------------------------------------------------------------------
/// <summary>Displays the main menu, handles interactive mode ,and runs predefined tests</summary>
class Program {
   #region  Methods--------------------------------------------------
   /// <summary>Application entry point that displays the menu and handles the user's choice</summary>
   public static void Main (string[] args) {
      for (; ; ) {
         Print ("\n=== Expression Evaluator ===\n");
         Print ("  I - Interactive mode (type your own expressions)\n");
         Print ("  T - Run all test cases\n");
         Print ("  Q - Quit\n");
         Print ("Choice: ");
         switch (ReadKey (true).Key) {
            case ConsoleKey.I: Print ("\n"); InteractiveMode (); return;
            case ConsoleKey.T: RunTests (); break;
            case ConsoleKey.Q: return;
            default: Print ("\nInvalid choice:", Red); break;
         }
      }
   }
   #endregion

   #region Implementation -------------------------------------------
   // Reads and evaluates user expressions until the user enters "exit" or input ends.
   static void InteractiveMode () {
      Print ("enter \"exit\" to leave the interactive mode\n");
      var eval = new Evaluator ();
      for (; ; ) {
         Print ("> ");
         string? line = Console.ReadLine ();
         if (line == null || line.Trim ().Equals ("exit", StringComparison.OrdinalIgnoreCase))
            break;
         if (string.IsNullOrWhiteSpace (line)) continue;
         try {
            double result = eval.Evaluate (line);
            Print ($"{Format (result)}\n", Green);
         } catch (Exception e) {
            Print ($"{e.Message}\n", Red);
         }
      }
   }

   // Runs predefined test cases, compares actual and expected results, and displays PASS/FAIL.
   static bool RunTests () {
      var eval = new Evaluator ();
      string line = new ('-', 118);
      Print ($"\n{"Expression",-24} | {"Expected",-10} | {"Actual",-10} | {"Status",-6} | Error message\n");
      Print (line + "\n");
      int passed = 0;
      foreach (var (input, expected) in sTests) {
         double actual = double.NaN;
         string error = "";
         try {
            actual = eval.Evaluate (input);
         } catch (Exception e) {
            error = e.Message;
         }
         bool pass = double.IsNaN (expected) ? error != ""
                                             : error == "" && Math.Abs (actual - expected) < Tolerance;
         if (pass) passed++;
         Print ($"{input,-24} | {Format (expected),-10} | {Format (actual),-10} | ");
         Print ($"{(pass ? "PASS" : "FAIL"),-6}", pass ? Green : Red);
         Print (" | ");
         Print ($"{error}\n", Red);
      }
      Print (line + "\n");
      Print ($"Passed {passed} of {sTests.Length}\n", passed == sTests.Length ? Green : Red);
      return passed == sTests.Length;
   }

   // Formats a number to 15 significant digits or returns "NaN" for Not-a-Number.
   static string Format (double d) => double.IsNaN (d) ? "NaN" : d.ToString ("G15");

   // Prints text in an optional console colour and then resets the colour.
   static void Print (string text, ConsoleColor? color = null) {
      if (color != null) Console.ForegroundColor = color.Value;
      Console.Write (text);
      Console.ResetColor ();
   }
   #endregion

   #region Private data----------------------------------------------
   const double NAN = double.NaN;  // Represents an expected error in a test case.
   const double Tolerance = 1e-9;  // Maximum allowed difference when comparing numerical results.
   // Stores predefined expressions and their expected results for testing
   static readonly (string Input, double Expected)[] sTests = {
      // Unary minus: basic cases
      ("-5", -5),
      ("--5", 5),
      ("-(2+3)", -5),
      ("2*-(3+4)", -14),
      // Same character, unary or binary, decided by the previous token
      ("10-4", 6),
      ("10--4", 14),
      ("5---2", 3),
      ("10+-4", 6),
      ("2*-3", -6),
      ("-2*-3", 6),
      ("(5)-3", 2),
      ("(-5)-3", -8),
      // Unary minus priority (lower than ^, higher than * and /)
      ("-2^2", -4),
      ("(-2)^2", 4),
      ("2^-2", 0.25),
      ("-2^3^2", -512),
      ("10-3-2", 5),
      // With functions
      ("-sqrt(16)", -4),
      ("sin(-30)", -0.5),
      // With variables
      ("a = 10", 10),
      ("-a", -10),
      ("a = -a", -10),
      ("5 - -a", -5),
      // Unary plus
      ("+5", 5),
      ("5++2", 7),
      ("+-5", -5),
      // Errors
      ("3-", NAN),
      ("-", NAN),
      ("*3", NAN),
      ("5/0", NAN),
      ("3 + 2 3", NAN),
      ("2 + (3*4", NAN),
      ("2 + 3)", NAN),
      ("abs(-23)", NAN),
      ("sqrt(-4)", NAN),
      ("y + 1", NAN),
      ("2 $ 3", NAN),
      ("5 = 3", NAN),
      ("()", NAN),
      ("1.2.3", NAN),
   };
   #endregion
}
#endregion
