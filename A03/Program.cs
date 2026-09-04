// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to find all valid Spelling Bee words from a dictionary, calculate their scores,
// identify pangrams, and display the results in descending order of score.
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main () {
      char[] letters = ['U', 'X', 'A', 'L', 'T', 'N', 'E'];
      char requiredLetter = letters[0];
      int total = 0;
      List<(string Word, int Score, bool Pangram)> results = [];
      foreach (string line in File.ReadLines ("words.txt")) {
         string word = line.Trim ();
         if (IsValidWord (word, requiredLetter, letters)) {
            bool pangram = IsPangram (word, letters);
            int score = (word.Length > 4 ? word.Length : 1) + (pangram ? 7 : 0);
            results.Add ((word, score, pangram));
         }
      }
      results = [.. results.OrderByDescending (x => x.Score)
                .ThenBy (x => x.Word)];
      foreach (var (Word, Score, Pangram) in results) {
         total += Score;
         if (Pangram) ForegroundColor = ConsoleColor.Green;
         else ResetColor ();
         WriteLine ($"{Score,2}. {Word}");
      }
      ResetColor ();
      WriteLine ($"----\n{total} total");
   }

   // Checks if the word is valid based on length, required letter, and allowed letters.
   static bool IsValidWord (string word, char required, char[] allowed) =>
      word.Length >= 4 && word.Contains (required) && word.All (allowed.Contains);

   // Checks if the word contains every allowed letter, making it a pangram.
   static bool IsPangram (string word, char[] allowed) =>
     allowed.All (word.Contains);
}