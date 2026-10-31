// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Implementation of file name parser with state machine.
// ------------------------------------------------------------------------------------------------
using static EState;
using System.Text;
using static System.Console;

#region class Program -----------------------------------------------------------------------------
/// <summary>Parses file paths using a state machine and displays the parsing results</summary>
class Program {
   #region Methods---------------------------------------------------
   /// <summary> Tests the parser with different file paths and displays the results</summary>
   public static void Main () {
      WriteLine ($"{"Input",-40}{"Drive",-8}{"Folder",-18}{"File",-16}{"Ext",-8}Result");
      WriteLine (new string ('-', 100));
      int passed = 0;
      foreach (var path in testPaths) {
         var (drive, folder, file, ext) = Parse (path);
         bool ok = drive.Length > 0;
         if (ok) passed++;
         string Show (string s) => ok ? s : "-";
         Write ($"{"\"" + path + "\"",-40}{Show (drive),-8}{Show (folder),-18}" +
                        $"{Show (file),-16}{Show (ext),-8}");
         ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
         WriteLine (ok ? "Passed" : "Failed");
         ResetColor ();
      }
      WriteLine (new string ('-', 100));
      WriteLine ($"{testPaths.Length} test paths: {passed} parsed, {testPaths.Length - passed} rejected.");
   }
   #endregion

   #region Implementation--------------------------------------------
   // Parses a file path and extracts the drive, folder, file name and extension.
   static (string, string, string, string) Parse (string path) {
      EState s = A;
      Action none = () => { }, todo;
      StringBuilder drive = new (), folder = new (), file = new (), ext = new ();
      foreach (var ch in path.ToUpper () + '~') {
         (s, todo) = (s, ch) switch {
            (A, >= 'A' and <= 'Z') => (B, () => drive.Append (ch)),
            (B, ':') => (C, none),
            (C, '\\') => (D, none),
            (D or E, >= 'A' and <= 'Z') => (E, () => file.Append (ch)),
            (E, '\\') => (D, () => {folder.Append (folder.Length == 0 ? file : "\\" + file);
                                    file.Clear ();}),
            (E, '.') when folder.Length > 0 => (F, none),
            (F or G, >= 'A' and <= 'Z') => (G, () => ext.Append (ch)),
            (G, '~') => (H, none),
            _ => (Z, none)
         };
         todo ();
         if (s == Z) break;
      }
      return s == H ? (drive.ToString (), folder.ToString (), file.ToString (), ext.ToString ()) : ("", "", "", "");
   }
   #endregion

   #region Private data----------------------------------------------
   // Contains valid and invalid file paths used to test the parser.
   static string[] testPaths = {
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

#region enum EStatte-------------------------------------------------------------------------------
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