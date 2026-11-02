// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Implementation of file name parser with state machine.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static EState;
using static System.Console;

#region class Program -----------------------------------------------------------------------------
/// <summary>Parses file paths using a state machine and displays the parsing results</summary>
class Program {
   #region Methods --------------------------------------------------
   /// <summary>Tests the parser with different file paths and displays the results</summary>
   public static void Main () {
      WriteLine ($"{"Input",-40}{"Drive",-8}{"Folder",-18}{"File",-16}{"Ext",-8}Result");
      WriteLine (new string ('-', 100));
      int passed = 0; bool ok = false;
      foreach (var path in sTestData) {
         var result = Parse (path);
         ok = result.drive.Length > 0;
         if (ok) passed++;
         Write ($"{"\"" + path + "\"",-40}{Show (result.drive),-8}{Show (result.folder),-18}"
                + $"{Show (result.file),-16}{Show (result.extension),-8}");
         ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
         WriteLine (ok ? "Passed" : "Failed");
         ResetColor ();
      }
      WriteLine (new string ('-', 100));
      WriteLine ($"{sTestData.Length} test paths: {passed} parsed, {sTestData.Length - passed} rejected.");

      // Displays the parsed value if the path is valid otherwise displays "-"
      string Show (string s) => ok ? s : "-";
   }
   #endregion

   #region Implementation -------------------------------------------
   // Parses a file path and extracts the drive, folder, file name and extension
   static ParseResult Parse (string path) {
      EState state = A;
      Action none = () => { }, todo;
      string drive = "";
      StringBuilder folder = new (), file = new (), ext = new ();
      foreach (var ch in path.ToUpper () + '~') {
         (state, todo) = (state, ch) switch {
            (A, >= 'A' and <= 'Z') => (B, () => drive += ch),
            (B, ':') => (C, none),
            (C, '\\') => (D, none),
            (D or E, >= 'A' and <= 'Z') => (E, () => file.Append (ch)),
            (E, '\\') => (D, () => { folder.Append (folder.Length == 0 ? file : "\\" + file); file.Clear (); }),
            (E, '.') when folder.Length > 0 => (F, none),
            (F or G, >= 'A' and <= 'Z') => (G, () => ext.Append (ch)),
            (G, '~') => (H, none),
            _ => (Z, none)
         };
         todo ();
         if (state == Z) break;
      }
      return state == H ? new ParseResult (drive, folder.ToString (), file.ToString (), ext.ToString ())
                        : new ParseResult ("", "", "", "");
   }
   #endregion

   #region Private data ---------------------------------------------
   // Contains valid and invalid file paths used to test the parser
   static string[] sTestData = {
         @"C:\ABC\DEF\README.TXT", @"C:\ABC\FILE.TXT", @"D:\A\B.C", @"Z:\FOLDER\FILE.EXT",
         @"A:\X\Y.Z", @"C:\A\B\C\D\E\F\G\FILE.TXT", @"C:\ABC\FILE.T",
         @"E:\PROJECTFILES\DOCUMENTATION.PDFX",@"C:\ABC\DEF\GHI\IMAGE.JPG",
         @"C:\README.TXT", @"C:\ABC\DEF", @"README.TXT", @"C:\ABC\.BCF", @"C:\ABC\BCF.",
         @"C:ABC\FILE.TXT", @"C\ABC\FILE.TXT", @":\ABC\FILE.TXT", @"C:\ABC\DEF\", "", " ",
         @"c:\abc\def\r.txt", @"C:\abc\DEF\R.TXT", @"C:\ABC\DEF\r.TXT", @"C:\ABC\DEF\R.txt",
         @"C:\ABC6\DEF\R.TXT", @"C:\MY FOLDER\FILE.TXT", @"C:\AB.C\DEF\R.TXT",
         @"C:\ABC\FILE.TXT~",@"CD:\ABC\FILE.TXT", @"C:\ABC:D", @"C:\\ABC\FILE.TXT",
         @"C:\DIR\\FILE.TXT",@"C:\DIR\ARCHIVE.TAR.GZ", "C:/ABC/FILE.TXT", @"C:\ABC\FILE\TXT"
    };
   #endregion
}
#endregion

#region Struct Parseresult ------------------------------------------------------------------------
/// <summary>Stores the parsed components of a file path</summary>
readonly record struct ParseResult (
   string drive,
   string folder,
   string file,
   string extension
);
#endregion

#region enum EState -------------------------------------------------------------------------------
/// <summary>Represents the states used to validate and parse a file path</summary>
enum EState {
   A,   // A: nothing read yet
   B,   // B: drive letter read
   C,   // C: ':' read
   D,   // D: '\' read, a name must follow
   E,   // E: inside a folder name or the file name
   F,   // F: '.' read, an extension must follow
   G,   // G: inside the extension
   H,   // H: end of input, success
   Z    // invalid input
}
#endregion