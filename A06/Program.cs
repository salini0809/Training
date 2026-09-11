// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to find solutions to the Eight Queens problem using backtracking.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static System.Console;

class Program {
   static void Main () {
      OutputEncoding = Encoding.UTF8;
      List<int[]> allSolutions = SolveNQueens (N);
      List<int[]> uniqueSolutions = RemoveIdenticalSolutions (allSolutions, N);
      WriteLine ("Eight queens problem ");
      WriteLine ($"Total solutions: {allSolutions.Count}");
      WriteLine ($"Unique solutions: {uniqueSolutions.Count}");
      List<int[]> solutions = SelectSolutions (allSolutions, uniqueSolutions);
      BrowseSolutions (solutions, N);
   }

   static void Solve (int col, int n, int[] queens, bool[] usedRow, bool[] usedLowerDiag,
       bool[] usedUpperDiag, List<int[]> solutions) {
      if (col == n) {
         solutions.Add ((int[])queens.Clone ());
         return;
      }
      for (int row = 0; row < n; row++) {
         int lower = row + col;
         int upper = n - 1 + col - row;
         if (usedRow[row] || usedLowerDiag[lower] || usedUpperDiag[upper])
            continue;
         queens[col] = row;
         usedRow[row] = usedLowerDiag[lower] = usedUpperDiag[upper] = true;
         Solve (col + 1, n, queens, usedRow, usedLowerDiag, usedUpperDiag, solutions);
         usedRow[row] = usedLowerDiag[lower] = usedUpperDiag[upper] = false;
      }
   }

   static List<int[]> SolveNQueens (int n) {
      int[] queens = new int[n];
      bool[] usedRow = new bool[n];
      bool[] usedLowerDiag = new bool[2 * n - 1];
      bool[] usedUpperDiag = new bool[2 * n - 1];
      List<int[]> solutions = [];
      Solve (0, n, queens, usedRow, usedLowerDiag, usedUpperDiag, solutions);
      return solutions;
   }

   static int[] Transform (int[] pos, int n, Func<int, int, (int newRow, int newCol)> map) {
      int[] result = new int[n];
      for (int col = 0; col < n; col++) {
         var (newRow, newCol) = map (pos[col], col);
         result[newCol] = newRow;
      }
      return result;
   }

   static int[] Rotate90 (int[] pos, int n) =>
       Transform (pos, n, (row, col) => (col, n - 1 - row));

   static int[] MirrorHorizontal (int[] pos, int n) =>
       Transform (pos, n, (row, col) => (n - 1 - row, col));

   static string ToKey (int[] pos) => string.Join (",", pos);

   static string GetCanonicalForm (int[] pos, int n) {
      var rotations = new List<int[]> { pos };
      for (int i = 0; i < ROTATIONS - 1; i++)
         rotations.Add (Rotate90 (rotations[^1], n));
      var mirrored = MirrorHorizontal (pos, n);
      var mirroredRotations = new List<int[]> { mirrored };
      for (int i = 0; i < ROTATIONS - 1; i++)
         mirroredRotations.Add (Rotate90 (mirroredRotations[^1], n));
      return rotations.Concat (mirroredRotations).Select (ToKey)
         .OrderBy (s => s, StringComparer.Ordinal).First ();
   }

   static List<int[]> RemoveIdenticalSolutions (List<int[]> allSolutions, int n) {
      var seen = new HashSet<string> ();
      var unique = new List<int[]> ();
      foreach (var solution in allSolutions) {
         if (seen.Add (GetCanonicalForm (solution, n)))
            unique.Add (solution);
      }
      return unique;
   }

   static string BuildBorder (string edges, int n) =>
       edges[0] + string.Join (edges[1].ToString (), Repeat (HORIZONTAL, n))
      + edges[2];

   static void PrintBoard (int[] pos, int n) {
      WriteLine (BuildBorder (TOP, n));
      for (int row = 0; row < n; row++) {
         Write (VERTICAL);
         for (int col = 0; col < n; col++)
            Write ((pos[col] == row ? QUEEN : EMPTY) + VERTICAL);
         WriteLine ();
         WriteLine (BuildBorder (row < n - 1 ? MIDDLE : BOTTOM, n));
      }
   }

   static IEnumerable<string> Repeat (string s, int count) {
      for (int i = 0; i < count; i++)
         yield return s;
   }

   static List<int[]> SelectSolutions (List<int[]> allSolutions, List<int[]> uniqueSolutions) {
      Write ("Press Key to see [A]ll or [U]nique solutions:");
      while (true) {
         switch (ReadKey (true).Key) {
            case ConsoleKey.A:
               return allSolutions;
            case ConsoleKey.U:
               return uniqueSolutions;
            default:
               Write ("\nInvalid key. Press A or U:");
               break;
         }
      }
   }

   static void BrowseSolutions (List<int[]> solutions, int n) {
      int index = 0;
      while (true) {
         Clear ();
         WriteLine ($"Solution {index + 1} of {solutions.Count}");
         WriteLine ();
         PrintBoard (solutions[index], n);
         if (index == solutions.Count - 1)
            return;
         WriteLine ();
         WriteLine ("Press: [→]- Next [←]- Previous [Q]- Quit");
         switch (ReadKey (true).Key) {
            case ConsoleKey.RightArrow:
               if (index < solutions.Count - 1)
                  index++;
               break;
            case ConsoleKey.LeftArrow:
               if (index > 0)
                  index--;
               break;
            case ConsoleKey.Q:
               return;
            default: Write ("\nInvalid Key. Press →, ← or Q:"); ReadKey (true); break;
         }
      }
   }

   #region Constants ------------------------------------------------
   const string TOP = "┌┬┐";
   const string MIDDLE = "├┼┤";
   const string BOTTOM = "└┴┘";
   const string VERTICAL = "│";
   const string HORIZONTAL = "────";
   const string EMPTY = "    ";
   const string QUEEN = " ♕  ";
   const int ROTATIONS = 4;
   const int N = 8;
   #endregion
}