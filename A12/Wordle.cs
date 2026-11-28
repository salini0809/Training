// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Wordle.cs
// Implements the Wordle game by handling user input, word validation and result display.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static ELetterStatus;
using static EGameState;
using static System.Console;
using static System.ConsoleColor;

#region class Wordle ------------------------------------------------------------------------------
/// <summary>Implements a console-based Wordle game where players guess a five-letter secret word within six attempts</summary>
class Wordle {
   #region Constructor ----------------------------------------------
   /// <summary>Initializes a new Wordle game with the specified word bank</summary>
   public Wordle (WordBank wordBank) {
      mWordBank = wordBank;
   }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Initializes and runs the Wordle game, handling user input and updating the game board until the game ends</summary>
   public void Run () {
      OutputEncoding = Encoding.UTF8;
      CursorVisible = false;
      Clear ();
      mSecret = mWordBank.GetRandomWord ();
      DisplayBoard ();
      while (mState == InProgress) {
         UpdateGameState (ReadKey (true));
         DisplayBoard ();
      }
      ReadKey (true);
      SetCursorPosition (0, TOP + 16);
      CursorVisible = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Processes keyboard input and updates the current word
   void UpdateGameState (ConsoleKeyInfo key) {
      mMessage = "";
      switch (key.Key) {
         case ConsoleKey.Backspace:
         case ConsoleKey.LeftArrow:
            if (mCurrent.Length > 0) mCurrent = mCurrent[..^1];
            break;
         case ConsoleKey.Enter:
            if (mCurrent.Length == WORDLENGTH) SubmitWord ();
            break;
         default:
            char ch = char.ToUpperInvariant (key.KeyChar);
            if (ch is >= 'A' and <= 'Z' && mCurrent.Length < WORDLENGTH) mCurrent += ch;
            break;
      }
   }

   // Validates the submitted word, calculates letter scores,updates the keyboard status and checks the game result
   void SubmitWord () {
      if (!mWordBank.IsValidWord (mCurrent)) {
         mMessage = $"{mCurrent} is not a valid word.";
         return;
      }
      ELetterStatus[] scores = ScoreGuess (mCurrent, mSecret);
      mGuesses.Add (mCurrent);
      mGuessScores.Add (scores);
      for (int i = 0; i < WORDLENGTH; i++) {
         int k = mCurrent[i] - 'A';
         if (scores[i] > mKeyStatus[k]) mKeyStatus[k] = scores[i];
      }
      if (mCurrent == mSecret) mState = Won;
      else if (mGuesses.Count == MAXTRIES) mState = Lost;
      mCurrent = "";
   }

   // Evaluates a guess against the secret word and returns the status of each letter
   ELetterStatus[] ScoreGuess (string guess, string answer) {
      ELetterStatus[] result = new ELetterStatus[WORDLENGTH];
      int[] remaining = new int[26];
      for (int i = 0; i < WORDLENGTH; i++) {
         result[i] = guess[i] == answer[i] ? CorrectPosition : NotInWord;
         if (result[i] == NotInWord) remaining[answer[i] - 'A']++;
      }
      for (int i = 0; i < WORDLENGTH; i++) {
         if (result[i] == NotInWord && remaining[guess[i] - 'A'] > 0) {
            result[i] = WrongPosition;
            remaining[guess[i] - 'A']--;
         }
      }
      return result;
   }

   // Returns the console color corresponding to a letter status
   ConsoleColor ColorFor (ELetterStatus status) => status switch {
      CorrectPosition => Green,
      WrongPosition => Blue,
      NotInWord => DarkGray,
      _ => White
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

   // Returns the character to display in the specified game board cell
   string CellText (int row, int col) {
      if (row < mGuesses.Count) return mGuesses[row][col].ToString ();
      if (row > mGuesses.Count) return "\u00B7";
      if (col < mCurrent.Length) return mCurrent[col].ToString ();
      return col == mCurrent.Length ? "\u25CC" : "\u00B7";
   }

   // Renders the game grid, on-screen keyboard and game messages
   void DisplayBoard () {
      int left = Math.Max (0, (WindowWidth - WIDTH) / 2);
      for (int r = 0; r < MAXTRIES; r++) {
         for (int c = 0; c < WORDLENGTH; c++) {
            string text = CellText (r, c);
            ConsoleColor color = r < mGuesses.Count ? ColorFor (mGuessScores[r][c]) : White;
            WriteAt (left + 8 + c * 2, TOP + r, text, color);
         }
      }
      WriteAt (left, TOP + 6, sSeparator, DarkGray);
      for (int k = 0; k < 26; k++) {
         char letter = (char)('A' + k);
         ELetterStatus status = mKeyStatus[letter - 'A'];
         WriteAt (left + (k % 7) * 4, TOP + 7 + k / 7, letter.ToString (), ColorFor (status));
      }
      ClearLine (TOP + 12);
      if (mState != InProgress) {
         WriteAt (left, TOP + 11, sSeparator, DarkGray);
         if (mState == Won)
            WriteCentered (left, TOP + 12, $"You found the word in {mGuesses.Count} " +
               $"{(mGuesses.Count == 1 ? "try" : "tries")}.", Green);
         else
            WriteCentered (left, TOP + 12, $"Sorry - the word was {mSecret}", Yellow);
         WriteCentered (left, TOP + 14, "Press any key to quit", White);
      } else if (!string.IsNullOrEmpty (mMessage))
         WriteCentered (left, TOP + 12, mMessage, Yellow);
   }
   #endregion

   #region Private Data ---------------------------------------------
   WordBank mWordBank;
   EGameState mState;
   string mSecret = "";                                       // Randomly selected secret word
   string mCurrent = "";                                      //  Word currently being typed
   string mMessage = "";                                      // Feedback or error message
   List<string> mGuesses = [];                                // Submitted valid guesses
   List<ELetterStatus[]> mGuessScores = [];                   // Letter colors for submitted guesses
   ELetterStatus[] mKeyStatus = new ELetterStatus[26];        // Best status of each keyboard letter
   static readonly string sSeparator = new ('\u2500', WIDTH); // Horizontal separator displayed between the game board and keyboard
   const int WIDTH = 25;                                      // Total width of the game display
   const int TOP = 1;                                         // Starting console row of the game board
   const int MAXTRIES = 6;                                    // Maximum number of valid guesses
   const int WORDLENGTH = 5;                                  // Maximum Length of the word
   #endregion
}
#endregion

#region enum --------------------------------------------------------------------------------------
/// <summary>Represents the evaluation status of each letter in a Wordle guess</summary>
enum ELetterStatus { Unused, NotInWord, WrongPosition, CorrectPosition }

/// <summary>Represents the current state of the Wordle game</summary>
enum EGameState { InProgress, Won, Lost }
#endregion