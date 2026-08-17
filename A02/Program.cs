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

   // Generates a secret number,validates guesses,tracks attempts,and displays color-coded feedback.
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

   // Displays a message in the specified color and resets the console color.
   static void PrintMsg (string message, ConsoleColor color) {
      ForegroundColor = color;
      WriteLine (message);
      ResetColor ();
   }

   // Prompts the user to play again and returns true for 'Y' and false for 'N'.
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