// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// WordBank.cs
// Handles loading, selection and validation of Wordle words.
// ------------------------------------------------------------------------------------------------
#region class WordBank ----------------------------------------------------------------------------
/// <summary>Manages puzzle words and valid dictionary words for the Wordle game</summary>
class WordBank {
   #region Constructor ------------------------------------------------
   /// <summary>Loads puzzle words and dictionary words from text files</summary>
   public WordBank () {
      mPuzzleWords.AddRange (
         File.ReadAllLines (Path.Join (AppContext.BaseDirectory, "puzzle-5.txt")));
      mDictionary.UnionWith (
         File.ReadAllLines (Path.Join (AppContext.BaseDirectory, "dict-5.txt")));
      mDictionary.UnionWith (mPuzzleWords);
   }
   #endregion

   #region Methods ----------------------------------------------------
   /// <summary>Returns a randomly selected puzzle word </summary>
   public string GetRandomWord () =>
      mPuzzleWords[Random.Shared.Next (mPuzzleWords.Count)];

   /// <summary>Checks whether the given word exists in the dictionary</summary>
   public bool IsValidWord (string word) => mDictionary.Contains (word);
   #endregion

   #region Private Data -----------------------------------------------
   List<string> mPuzzleWords = [];       // Possible secret words
   HashSet<string> mDictionary = [];     // All valid guess words
   #endregion
}
#endregion