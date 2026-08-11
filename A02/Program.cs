// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to generate a random number between 1 and 100 and allow the user to guess it.
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main (string[] args) {
      PlayGame ();
      while (PlayAgain ())
         PlayGame ();
      WriteLine ("Thank you for playing!");
   }
   static void PlayGame () {
      int SecretNumber = new Random ().Next (1, 101), attempts = 0, guess;
      do {
         Write ("Enter the number you have guessed between 1 and 100 : ");
         if (!int.TryParse (ReadLine (), out guess) || guess < 1 || guess > 100) {
            WriteLine ("Please enter a valid number");
            continue;
         }
         attempts++;
         WriteLine (guess == SecretNumber ? $"You have guessed the number in {attempts} attempts."
                   : guess < SecretNumber ? "Your guess is too low." : "Your guess is too high.");
      } while (guess != SecretNumber);
   }
   static bool PlayAgain () {
      while (true) {
         Write ("Do you want to play again? (Y/N): ");
         ConsoleKey key = ReadKey ().Key;
         WriteLine ();
         switch (key) {
            case ConsoleKey.Y: return true;
            case ConsoleKey.N: return false;
            default:
               WriteLine ("Invalid input. Please press (Y)es or (N)o.");
               break;
         }
      }
   }
}
