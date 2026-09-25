// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Finds the frequency of all letters in a word list and displays the top 7 most frequent letters.
// ------------------------------------------------------------------------------------------------
using static System.Console;

Dictionary<char, int> frequency = [];
foreach (char ch in File.ReadAllText ("words.txt")) {
   if (char.IsLetter (ch))
      frequency[ch] = frequency.GetValueOrDefault (ch) + 1;
}
WriteLine ("Seven most frequently occuring letters in the word list are:");
foreach (var item in frequency.OrderByDescending (x => x.Value).Take (7))
   WriteLine ($"{item.Key} : {item.Value}");