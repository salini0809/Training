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
      WriteLine ("Eight Queens problem");
      var (solutions, title) = SelectSolutions ();
      BrowseSolutions (solutions, title);
   }

   // Finds all valid N-Queens solutions and optionally keeps only unique solutions.
   static List<int[]> SolveNQueens (bool uniqueOnly) {
      List<int[]> solutions = [];
      int[] pos = new int[N];
      PlaceQueen (0);
      return solutions;

      // Recursively places a queen in each column using backtracking.
      void PlaceQueen (int col) {
         for (pos[col] = 0; pos[col] < N; pos[col]++) {
            if (IsValid (col)) {
               if (col == N - 1) AddSolution ([.. pos]);
               else PlaceQueen (col + 1);
            }
         }
      }

      // Checks whether the queen at the current column is safe from previously placed queens.
      bool IsValid (int col) {
         for (int previousCol = 0; previousCol < col; previousCol++) {
            int dy = Math.Abs (pos[col] - pos[previousCol]);
            int dx = col - previousCol;
            if (dy == 0 || dy == dx) return false;
         }
         return true;
      }

      // Adds the solution directly or only if it is unique, depending on the selected mode.
      void AddSolution (int[] solution) {
         if (!uniqueOnly || IsUnique (solution)) solutions.Add (solution);
      }

      // Checks whether the solution is different from all previously stored solutions.
      bool IsUnique (int[] solution) {
         int[] temp = solution;
         for (int i = 0; i < 4; i++) {
            temp = Rotate (temp);
            if (SolutionExists (temp) || SolutionExists (Mirror (temp))) return false;
         }
         return true;
      }

      // Checks whether an equivalent solution already exists in the solution list.
      bool SolutionExists (int[] sol) => solutions.Any (x => x.SequenceEqual (sol));
   }

   // Rotates the chessboard solution by 90 degrees.
   static int[] Rotate (int[] pos) {
      int[] rotated = new int[N];
      for (int col = 0; col < N; col++) {
         int row = pos[col];
         rotated[N - 1 - row] = col;
      }
      return rotated;
   }
   // Creates a vertically mirrored version of the solution.
   static int[] Mirror (int[] pos) => [.. pos.Reverse ()];

   // Gets the user's choice and returns either all or unique  solutions.
   static (List<int[]>, string) SelectSolutions () {
      Write ("Press Key to see [A]ll or [U]nique solutions: ");
      while (true) {
         switch (ReadKey (true).Key) {
            case ConsoleKey.A:
               return (SolveNQueens (uniqueOnly: false), "All Solutions");
            case ConsoleKey.U:
               return (SolveNQueens (uniqueOnly: true), "Unique Solutions");
            default:
               Write ("\nInvalid key. Press A or U: ");
               break;
         }
      }
   }

   // Displays the solutions one by one and allows navigation between them.
   static void BrowseSolutions (List<int[]> solutions, string title) {
      int ind = 0;
      while (true) {
         Clear ();
         WriteLine (title);
         WriteLine ($"Solution {ind + 1} of {solutions.Count}\n");
         PrintBoard (solutions[ind]);
         WriteLine ();
         WriteLine ("Press: [→]- Next [←]- Previous [Q]- Quit");
         int row = CursorTop + 1;
         ConsoleKey key;
         while (true) {
            key = ReadKey (true).Key;
            if (key is ConsoleKey.RightArrow or ConsoleKey.LeftArrow or ConsoleKey.Q) break;
            SetCursorPosition (0, row);
            WriteLine ("Invalid Key. Press →, ← or Q.");
         }
         switch (key) {
            case ConsoleKey.RightArrow:
               if (ind < solutions.Count - 1) ind++;
               break;
            case ConsoleKey.LeftArrow:
               if (ind > 0) ind--;
               break;
            case ConsoleKey.Q: return;
         }
      }
   }

   // Prints the given N-Queens solution as a formatted chessboard.
   static void PrintBoard (int[] pos) {
      WriteLine (BuildBorder (TOP));
      for (int row = 0; row < N; row++) {
         Write (VERTICAL);
         for (int col = 0; col < N; col++)
            Write ((pos[col] == row ? QUEEN : EMPTY) + VERTICAL);
         WriteLine ();
         WriteLine (BuildBorder (row < N - 1 ? MIDDLE : BOTTOM));
      }
   }

   // Builds a horizontal border for the chessboard.
   static string BuildBorder (string edges) =>
       edges[0] + string.Join (edges[1].ToString (), Repeat (HORIZONTAL, N)) + edges[2];

   // generates a string repeatedly for the specified number of times.
   static IEnumerable<string> Repeat (string s, int count) {
      for (int i = 0; i < count; i++)
         yield return s;
   }
   #region const-----------------------------------------------------------------------------------
   const string TOP = "┌┬┐";
   const string MIDDLE = "├┼┤";
   const string BOTTOM = "└┴┘";
   const string VERTICAL = "│";
   const string HORIZONTAL = "────";
   const string EMPTY = "    ";
   const string QUEEN = " ♕  ";
   const int N = 8;
   #endregion
}