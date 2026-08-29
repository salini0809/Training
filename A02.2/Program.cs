// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to guess a number between 0 and 127 by asking the user questions based on the remainder.
// ------------------------------------------------------------------------------------------------
using static System.Console;
using static System.ConsoleColor;

class Program {
   static void Main (string[] args) {
      WriteLine ("Think of a number between 0 and 127, I'll guess it!");
      WriteLine ("Please answer my questions with (Y)es or (N)o");
      int number = 0, divisor = 2, remainder = 1;
      for (int i = 0; i < 7; i++) {
         Write ($"When divided by {divisor}, is the remainder >= {remainder} ? ");
         switch (ReadKey (true).Key) {
            case ConsoleKey.Y:
               number += remainder;
               PrintMsg ("Y", Blue); break;
            case ConsoleKey.N: PrintMsg ("N", Magenta); break;
            default:
               WriteLine ("\nInvalid Input", Red);
               i--; continue;
         }
         divisor *= 2; remainder *= 2;
      }
      PrintMsg ($"The number you thought of is {number}", Green);
   }

   //Prints the specified message in the given color.
   static void PrintMsg (string msg, ConsoleColor color) {
      ForegroundColor = color;
      WriteLine (msg);
      ResetColor ();
   }
}