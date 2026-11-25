// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Wordle.cs
// Implements the Wordle game by handling user input, word validation and result display.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static System.Console;
#region class Wordle ------------------------------------------------------------------------------
/// <summary>Implements a console-based Wordle game where players guess a five-letter secret word within six attempts</summary>
class Wordle {
   #region Methods --------------------------------------------------
   /// <summary>Initializes and runs the Wordle game, handling user input and updating the game board until the game ends</summary>
   public void Run () {
      OutputEncoding = Encoding.UTF8;
      CursorVisible = false;
      Clear ();
      LoadWords ();
      SelectWord ();
      DisplayBoard ();
      while (!gameOver) {
         UpdateGameState (Console.ReadKey (true));
         DisplayBoard ();
      }
      ReadKey (true);
      SetCursorPosition (0, Top + 16);
      CursorVisible = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Loads the puzzle words and valid dictionary words from text files
   void LoadWords () {
      puzzleWords.AddRange (File.ReadAllLines ("puzzle-5.txt"));
      dictionary.UnionWith (File.ReadAllLines ("dict-5.txt"));
      dictionary.UnionWith (puzzleWords);
   }

   // Randomly selects a secret word from the available puzzle words
   void SelectWord () => secret = puzzleWords[new Random ().Next (puzzleWords.Count)];

   // Processes keyboard input and updates the current word
   void UpdateGameState (ConsoleKeyInfo key) {
      message = "";
      switch (key.Key) {
         case ConsoleKey.Backspace:
         case ConsoleKey.LeftArrow:
            if (current.Length > 0)
               current = current.Substring (0, current.Length - 1);
            break;
         case ConsoleKey.Enter:
            if (current.Length == 5)
               SubmitWord ();
            break;
         default:
            char ch = char.ToUpper (key.KeyChar);
            if (ch >= 'A' && ch <= 'Z' && current.Length < 5)
               current += ch;
            break;
      }
   }

   // Validates the submitted word, calculates letter scores,updates the keyboard status and checks the game result
   void SubmitWord () {
      if (!dictionary.Contains (current)) {
         message = current + " is not a valid  word";
         return;
      }
      int[] scores = ScoreGuess (current, secret);
      guesses.Add (current);
      guessScores.Add (scores);
      for (int i = 0; i < 5; i++)
         keyStatus[current[i]] = Math.Max (keyStatus.GetValueOrDefault (current[i], Unused), scores[i]);
      won = current == secret;
      gameOver = won || guesses.Count == MaxTries;
      current = "";
   }

   // Evaluates a guess against the secret word and returns the status of each letter
   int[] ScoreGuess (string guess, string answer) {
      int[] result = new int[5];
      int[] remaining = new int[26];
      for (int i = 0; i < 5; i++) {
         result[i] = guess[i] == answer[i] ? Green : Gray;
         if (result[i] == Gray)
            remaining[answer[i] - 'A']++;
      }
      for (int i = 0; i < 5; i++) {
         if (result[i] == Gray && remaining[guess[i] - 'A'] > 0) {
            result[i] = Blue;
            remaining[guess[i] - 'A']--;
         }
      }
      return result;
   }

   // Returns the console color corresponding to a letter status
   ConsoleColor ColorFor (int status) => status switch {
      Green => ConsoleColor.Green,
      Blue => ConsoleColor.Blue,
      Gray => ConsoleColor.DarkGray,
      _ => ConsoleColor.White
   };

   // Clears the specified console row
   void ClearLine (int row) {
      Console.SetCursorPosition (0, row);
      Console.Write (new string (' ', Console.WindowWidth - 1));
   }

   // Displays colored text at the specified console position
   void WriteAt (int left, int row, string text, ConsoleColor color) {
      SetCursorPosition (left, row);
      ForegroundColor = color;
      Write (text);
      ResetColor ();
   }

   // Displays text centered within the game board
   void WriteCentered (int left, int row, string text, ConsoleColor color) =>
       WriteAt (left + Math.Max (0, (Width - text.Length) / 2), row, text, color);

   // Renders the game grid, on-screen keyboard and game messages
   void DisplayBoard () {
      int left = Math.Max (0, (WindowWidth - Width) / 2);
      for (int r = 0; r < MaxTries; r++) {
         for (int c = 0; c < 5; c++) {
            bool submitted = r < guesses.Count;
            bool typingRow = r == guesses.Count;
            string text = submitted ? guesses[r][c].ToString ()
                        : typingRow && c < current.Length ? current[c].ToString ()
                        : typingRow && c == current.Length ? "◌"
                        : "·";
            ConsoleColor color = submitted ? ColorFor (guessScores[r][c]) : ConsoleColor.White;
            WriteAt (left + 8 + c * 2, Top + r, text, color);
         }
      }
      WriteAt (left, Top + 6, new string ('─', Width), ConsoleColor.DarkGray);
      for (int k = 0; k < 26; k++) {
         char letter = (char)('A' + k);
         int status = keyStatus.GetValueOrDefault (letter, Unused);
         WriteAt (left + (k % 7) * 4, Top + 7 + k / 7, letter.ToString (), ColorFor (status));
      }
      for (int r = 11; r <= 14; r++) ClearLine (Top + r);
      if (gameOver) {
         WriteAt (left, Top + 11, new string ('─', Width), ConsoleColor.DarkGray);
         WriteCentered (left, Top + 12,
             won ? "You found the word in " + guesses.Count + " tries" : "Sorry - the word was " + secret,
             won ? ConsoleColor.Green : ConsoleColor.Yellow);
         WriteCentered (left, Top + 14, "Press any key to quit", ConsoleColor.White);
      } else if (message != "") {
         WriteCentered (left, Top + 12, message, ConsoleColor.Yellow);
      }
   }
   #endregion

   #region Private Data ---------------------------------------------
   List<string> puzzleWords = [];            // Possible secret words
   HashSet<string> dictionary = [];          // All valid guess words
   string secret = "";                       // Randomly selected secret word
   List<string> guesses = [];                // Submitted valid guesses
   List<int[]> guessScores = [];             // Letter scores for submitted guesses
   string current = "";                      //  Word currently being typed
   string message = "";                      // Feedback or error message
   Dictionary<char, int> keyStatus = [];     // Best status of each keyboard letter
   bool gameOver = false;                    // Indicates whether the game has ended
   bool won = false;                         // Indicates whether the player guessed correctly
   #endregion

   #region Constants ------------------------------------------------
   const int Width = 25;                     // Total width of the game display
   const int Top = 1;                        // Starting console row of the game board
   const int MaxTries = 6;                   // Maximum number of valid guesses
   const int Unused = 0, Gray = 1, Blue = 2, Green = 3;// Letter status values
   #endregion
}
#endregion