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
      do {
         int guessedNumber = PlayGame ();
         WriteLine ($"\nI guessed your number it is {guessedNumber}");
         Write ("\nDo you want to play again? (Y/N): ");
      } while (GetYesNo ());
      WriteLine ("\nThank you for playing!");
   }

   // Uses binary search to narrow the range and guess the user's number.
   static int PlayGame () {
      int low = 1, high = 100, guess;
      WriteLine ("\nThink of a number between 1 and 100.");
      WriteLine ("I will try to guess it. Please answer my questions with (Y)es or (N)o.");
      while (low < high) {
         guess = (low + high) / 2;
         Write ($"\nIs your number greater than {guess}? ");
         if (GetYesNo ()) low = guess + 1;
         else high = guess;
      }
      return low;
   }

   // Gets a valid Yes/No response from the user.
   static bool GetYesNo () {
      while (true) {
         switch (ReadKey ().Key) {
            case ConsoleKey.Y: return true;
            case ConsoleKey.N: return false;
            default:
               Write ("\nInvalid input. Please press (Y)es or (N)o: ");
               break;
         }
      }
   }
}