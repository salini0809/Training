// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to generate a random number between 1 and 100 and allow the user to guess it.
// ------------------------------------------------------------------------------------------------
using static System.Console;
using static System.ConsoleColor;

class Program {
   static void Main (string[] args) {
      do PlayGame ();
      while (PlayAgain ());
      WriteLine ("\nThank you for playing!");
   }
   static void PlayGame () {
      int secretNumber = new Random ().Next (1, 101), attempts = 0, guess;
      do {
         Write ("Guess a number between 1 and 100 : ");
         if (!int.TryParse (ReadLine (), out guess) || guess < 1 || guess > 100) {
            PrintMsg ("Please enter a valid number", Red);
            continue;
         }
         attempts++;
         var (message, color) = guess == secretNumber ? ("You have guessed the number in " +
                   $"{attempts} attempts.\n", Green)
                   : guess < secretNumber ? ("Your guess is too low.", Yellow)
                   : ("Your guess is too high.", Magenta);
         PrintMsg (message, color);
      } while (guess != secretNumber);
   }
   static void PrintMsg (string message, ConsoleColor color) {
      ForegroundColor = color;
      WriteLine (message);
      ResetColor ();
   }
   static bool PlayAgain () {
      while (true) {
         Write ("Do you want to play again? (Y/N): ");
         switch (ReadKey ().Key) {
            case ConsoleKey.Y: WriteLine (); return true;
            case ConsoleKey.N: return false;
            default:
               PrintMsg ("\nInvalid input. Please press (Y)es or (N)o.", Red);
               break;
         }
      }
   }
}


