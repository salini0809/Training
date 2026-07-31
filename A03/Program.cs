using static System.Console;
class Program {
   static void Main () {
      char[] letters = {'U', 'X', 'A', 'L', 'T', 'N', 'E'};
      char requiredLetter = letters[0];
      List<(string Word, int Score, bool Pangram)> results = new ();
      foreach (string line in File.ReadLines ("C:\\Users\\kannansa\\Downloads\\words 1.txt")) {
         string word = line.Trim ();
         if (IsValidWord (word, requiredLetter, letters)) {
            bool pangram = IsPangram (word, letters);
            int score;
            if (word.Length == 4) score = 1;
            else score = word.Length;
            if (pangram) score += 7;
            results.Add ((word, score, pangram));
         }
      }
      results = results
          .OrderByDescending (x => x.Score)
          .ThenBy (x => x.Word)
          .ToList ();
      int total = 0;
      foreach (var item in results) {
         total += item.Score;
         if (item.Pangram) ForegroundColor = ConsoleColor.Green;
         else ResetColor ();
         WriteLine ($"{item.Score,2}. {item.Word}");
      }
      ResetColor ();
      WriteLine ($"-----\n{total} total");
   }

   static bool IsValidWord (string word, char required, char[] allowed) {
      if (word.Length < 4) return false;
      if (!word.Contains (required)) return false;
      foreach (char c in word) {
         if (!allowed.Contains (c))
            return false;
      }
      return true;
   }

   static bool IsPangram (string word, char[] allowed) {
      foreach (char c in allowed) {
         if (!word.Contains (c))
            return false;
      }
      return true;
   }
}