// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to guess a number thought of by the user between 1 and 100 using the user's responses
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main (string[] args) {
      int low = 1, high = 100;
      WriteLine ("Think of a number between 1 and 100.");
      WriteLine ("Press \n L-Low \n H-High \n C-Correct \nfor each guess ");
      while (low <= high) {
         int guess = (low + high) / 2;
         Write ($"Is your number {guess}?");
         ConsoleKey key = ReadKey ().Key;
         WriteLine ();
         switch (key) {
            case ConsoleKey.L:
               low = guess + 1;
               break;
            case ConsoleKey.H:
               high = guess - 1;
               break;
            case ConsoleKey.C:
               WriteLine ($"I guessed your number! It is {guess}.");
               return;
            default:
               WriteLine ("Invalid input. Enter (L)ow or (H)igh or (C)orrect");
               break;
         }
      }
      WriteLine ("Your answers were inconsistent.");
   }
}
