// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to generate a random number between 1 and 100 and allow the user to guess it.
// ------------------------------------------------------------------------------------------------
using static System.Console;
class Program
{
    static void Main(string[] args)
    {
      int n = new Random ().Next (1, 101);
      int attempts = 1;
      while (true){
         WriteLine ("Enter the number you have guessed between 1 and 100 :");
         if(!int.TryParse (ReadLine (), out int r) || r < 1 || r > 100){
            WriteLine ("Please enter a valid number");
            continue;
         }
         if (r == n) {
            WriteLine ($"You have guessed the number correctly in {attempts} attempts!!");
            break;
         }
         else if (r < n) {
            WriteLine ("your guess is too low");
         } else {
            WriteLine ("your guess is too high");
         }
         attempts++;
      }
    }
}