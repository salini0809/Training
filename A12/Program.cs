// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Entry point for the wordle game.
// ------------------------------------------------------------------------------------------------
#region class Program -----------------------------------------------------------------------------
/// <summary>Serves as the entry point for the Wordle application</summary>
class Program {
   #region Methods --------------------------------------------------
   /// <summary>Creates a Wordle game instance and starts the game</summary>
   public static void Main () {
      try {
         WordBank wordBank = new WordBank ();
         Wordle game = new (wordBank);
         game.Run ();
      } catch (FileNotFoundException ex) {
         Console.WriteLine ($"Required file not found: {ex.FileName}");
      }
   }
   #endregion
}
#endregion