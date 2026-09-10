// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to find all valid Spelling Bee words from a dictionary, calculate their scores,
// identify pangrams, and display the results in descending order of score.
// ------------------------------------------------------------------------------------------------
using static System.Console;
using static System.ConsoleColor;

class Program {
   static char[] letters = ['U', 'X', 'A', 'L', 'T', 'N', 'E'];
   static List<(string Word, int Score, bool Pangram)> results = [];
   static void Main () {
      string[] words = File.ReadAllLines ("words.txt");
      int total = 0;
      foreach (string word in words.Select (word => word.Trim ()).Where (IsValidWord)) {
         bool pangram = letters.All (word.Contains);
         int score = (word.Length > 4 ? word.Length : 1) + (pangram ? 7 : 0);
         results.Add ((word, score, pangram));
      }
      results = [.. results.OrderByDescending (x => x.Score).ThenBy (x => x.Word)];
      foreach (var (Word, Score, Pangram) in results) {
         total += Score;
         ForegroundColor = Pangram ? Green : White;
         WriteLine ($"{Score,2}. {Word}");
      }
      ResetColor ();
      WriteLine ($"----\n{total} total");
   }

   // Checks if the word is valid based on length, required letter, and allowed letters.
   static bool IsValidWord (string word)
      => word.Length >= 4 && word.Contains (letters[0]) && word.All (letters.Contains);
}