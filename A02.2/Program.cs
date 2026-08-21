// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to guess a number between 1 and 127 by asking the user questions based on the remainder.
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main (string[] args) {
      WriteLine ("Think of a number between 0 and 127, I'll guess it!");
      WriteLine ("Please answer my questions with (Y)es or (N)o");
      int number = 0, divisor = 2, remainder = 1;
      for (int i = 0; i < 7; i++) {
         Write ($"\nWhen divided by {divisor}, is the remainder >= {remainder} ? ");
         int result = ProcessAnswer (number, remainder);
         if (result == -1) {
            i--;
            continue;
         }
         number = result;
         divisor *= 2;
         remainder *= 2;
      }
      WriteLine ($"\nThe number you thought of is {number}");
   }
   static int ProcessAnswer (int number, int remainder) {
      switch (ReadKey ().Key) {
         case ConsoleKey.Y: return number + remainder;
         case ConsoleKey.N: return number;
         default:
            WriteLine ("\nInvalid Input");
            return -1;
      }
   }
}
