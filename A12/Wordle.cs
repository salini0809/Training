// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Wordle.cs
// Implements the Wordle game by handling user input, word validation and result display.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static LetterStatus;
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
      while (!mGameOver) {
         UpdateGameState (Console.ReadKey (true));
         DisplayBoard ();
      }
      ReadKey (true);
      SetCursorPosition (0, TOP + 16);
      CursorVisible = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Loads the puzzle words and valid dictionary words from text files
   void LoadWords () {
      mPuzzleWords.AddRange (File.ReadAllLines ("puzzle-5.txt"));
      mDictionary.UnionWith (File.ReadAllLines ("dict-5.txt"));
      mDictionary.UnionWith (mPuzzleWords);
   }

   // Randomly selects a secret word from the available puzzle words
   void SelectWord () => mSecret = mPuzzleWords[new Random ().Next (mPuzzleWords.Count)];

   // Processes keyboard input and updates the current word
   void UpdateGameState (ConsoleKeyInfo key) {
      mMessage = "";
      switch (key.Key) {
         case ConsoleKey.Backspace:
         case ConsoleKey.LeftArrow:
            if (mCurrent.Length > 0)
               mCurrent = mCurrent.Substring (0, mCurrent.Length - 1);
            break;
         case ConsoleKey.Enter:
            if (mCurrent.Length == 5)
               SubmitWord ();
            break;
         default:
            char ch = char.ToUpper (key.KeyChar);
            if (ch >= 'A' && ch <= 'Z' && mCurrent.Length < 5)
               mCurrent += ch;
            break;
      }
   }

   // Validates the submitted word, calculates letter scores,updates the keyboard status and checks the game result
   void SubmitWord () {
      if (!mDictionary.Contains (mCurrent)) {
         mMessage = mCurrent + " is not a valid  word";
         return;
      }
      LetterStatus[] scores = ScoreGuess (mCurrent, mSecret);
      mGuesses.Add (mCurrent);
      mGuessScores.Add (scores);
      for (int i = 0; i < 5; i++) {
         LetterStatus old = mKeyStatus.GetValueOrDefault (mCurrent[i], Unused);
         mKeyStatus[mCurrent[i]] = scores[i] > old ? scores[i] : old;
      }
      mWon = mCurrent == mSecret;
      mGameOver = mWon || mGuesses.Count == MAXTRIES;
      mCurrent = "";
   }

   // Evaluates a guess against the secret word and returns the status of each letter
   LetterStatus[] ScoreGuess (string guess, string answer) {
      LetterStatus[] result = new LetterStatus[5];
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
   ConsoleColor ColorFor (LetterStatus status) => status switch {
      Green => ConsoleColor.Green,
      Blue => ConsoleColor.Blue,
      Gray => ConsoleColor.DarkGray,
      _ => ConsoleColor.White
   };

   // Clears the specified console row
   void ClearLine (int row) {
      SetCursorPosition (0, row);
      Write (new string (' ', WindowWidth - 1));
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
       WriteAt (left + Math.Max (0, (WIDTH - text.Length) / 2), row, text, color);

   // Renders the game grid, on-screen keyboard and game messages
   void DisplayBoard () {
      int left = Math.Max (0, (WindowWidth - WIDTH) / 2);
      for (int r = 0; r < MAXTRIES; r++) {
         for (int c = 0; c < 5; c++) {
            bool submitted = r < mGuesses.Count;
            bool typingRow = r == mGuesses.Count;
            string text = submitted ? mGuesses[r][c].ToString ()
                        : typingRow && c < mCurrent.Length ? mCurrent[c].ToString ()
                        : typingRow && c == mCurrent.Length ? "◌"
                        : "·";
            ConsoleColor color = submitted ? ColorFor (mGuessScores[r][c]) : ConsoleColor.White;
            WriteAt (left + 8 + c * 2, TOP + r, text, color);
         }
      }
      WriteAt (left, TOP + 6, new string ('─', WIDTH), ConsoleColor.DarkGray);
      for (int k = 0; k < 26; k++) {
         char letter = (char)('A' + k);
         LetterStatus status = mKeyStatus.GetValueOrDefault (letter, Unused);
         WriteAt (left + (k % 7) * 4, TOP + 7 + k / 7, letter.ToString (), ColorFor (status));
      }
      for (int r = 11; r <= 14; r++) ClearLine (TOP + r);
      if (mGameOver) {
         WriteAt (left, TOP + 11, new string ('─', WIDTH), ConsoleColor.DarkGray);
         WriteCentered (left, TOP + 12,
             mWon ? "You found the word in " + mGuesses.Count + " tries" : "Sorry - the word was " + mSecret,
             mWon ? ConsoleColor.Green : ConsoleColor.Yellow);
         WriteCentered (left, TOP + 14, "Press any key to quit", ConsoleColor.White);
      } else if (mMessage != "") {
         WriteCentered (left, TOP + 12, mMessage, ConsoleColor.Yellow);
      }
   }
   #endregion

   #region Private Data ---------------------------------------------
   List<string> mPuzzleWords = [];                  // Possible secret words
   HashSet<string> mDictionary = [];                // All valid guess words
   string mSecret = "";                             // Randomly selected secret word
   List<string> mGuesses = [];                      // Submitted valid guesses
   List<LetterStatus[]> mGuessScores = [];          // Letter colors for submitted guesses
   string mCurrent = "";                            //  Word currently being typed
   string mMessage = "";                            // Feedback or error message
   Dictionary<char, LetterStatus> mKeyStatus = [];  // Best status of each keyboard letter
   bool mGameOver = false;                          // Indicates whether the game has ended
   bool mWon = false;                               // Indicates whether the player guessed correctly
   #endregion

   #region Constants ------------------------------------------------
   const int WIDTH = 25;                     // Total width of the game display
   const int TOP = 1;                        // Starting console row of the game board
   const int MAXTRIES = 6;                   // Maximum number of valid guesses
   #endregion
}
#endregion

#region Enum LetterStatus -------------------------------------------------------------------------
/// <summary>Represents the evaluation status of each letter in a Wordle guess</summary>
public enum LetterStatus { Unused, Gray, Blue, Green }
#endregion