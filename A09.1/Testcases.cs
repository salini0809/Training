// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Testcases.cs
// Test cases for the TQueue<T> circular-buffer queue.
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class TestCases----------------------------------------------------------------------------
/// <summary>Runs test cases for the circular-buffer queue</summary>
class TestCases {
   #region Implementation--------------------------------------------
   // Runs every test and prints a summary
   static void Main () {
      Check ("Dequeue from an empty queue", DequeueEmpty);
      Check ("Single element, then empty again", SingleElement);
      Check ("First In First Out order", FifoOrder);
      Check ("Reuses freed slots at the front (wrap around)", WrapAround);
      Check ("Grows when completely full", GrowWhenFull);
      Check ("Grows after a wrap around", GrowAfterWrap);
      WriteLine ($"\n{mPassed} passed, {mFailed} failed");
   }

   // Tests whether Dequeue throws an exception when the queue is empty
   static bool DequeueEmpty () {
      var q = new TQueue<int> ();
      try {
         q.Dequeue ();
         return false;
      } catch (InvalidOperationException) {
         return true;
      }
   }

   // Tests whether a single element can be added and removed correctly
   static bool SingleElement () {
      var q = new TQueue<int> ();
      if (!q.IsEmpty) return false;
      q.Enqueue (42);
      if (q.IsEmpty) return false;
      return q.Dequeue () == 42 && q.IsEmpty;
   }

   // Tests whether elements are dequeued in FIFO order
   static bool FifoOrder () {
      var q = new TQueue<string> ();
      var data = new[] { "red", "green", "blue" };
      Fill (q, data);
      return Drains (q, data);
   }

   // Tests whether the queue reuses freed array slots correctly
   static bool WrapAround () {
      var q = new TQueue<int> ();
      Fill (q, 1, 2, 3, 4);
      q.Dequeue ();
      q.Dequeue ();
      Fill (q, 5, 6);
      return Drains (q, 3, 4, 5, 6);
   }

   // Tests whether the queue grows when its capacity is exceeded.
   static bool GrowWhenFull () {
      var q = new TQueue<int> ();
      var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      Fill (q, data);
      return Drains (q, data);
   }

   // Tests whether the queue grows correctly when the buffer is wrapped.
   static bool GrowAfterWrap () {
      var q = new TQueue<char> ();
      Fill (q, 'A', 'B', 'C', 'D');
      q.Dequeue ();
      q.Dequeue ();
      Fill (q, 'E', 'F', 'G');
      return Drains (q, 'C', 'D', 'E', 'F', 'G');
   }

   // Runs a test case and records whether it passed or failed.
   static void Check (string name, Func<bool> test) {
      bool ok;
      string note = "";
      try {
         ok = test ();
      } catch (Exception e) {
         ok = false;
         note = $" (unexpected {e.GetType ().Name})";
      }
      if (ok) mPassed++; else mFailed++;
      ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
      Write (ok ? "[PASS] " : "[FAIL] ");
      ResetColor ();
      WriteLine (name + note);
   }

   // Adds the given elements to the queue in order.
   static void Fill<T> (TQueue<T> q, params T[] items) {
      foreach (T item in items) q.Enqueue (item);
   }

   // Tests whether the queue returns the expected elements and becomes empty.
   static bool Drains<T> (TQueue<T> q, params T[] expected) {
      foreach (T item in expected)
         if (q.IsEmpty || !EqualityComparer<T>.Default.Equals (q.Dequeue (), item)) return false;
      return q.IsEmpty;
   }
   #endregion

   #region Private data----------------------------------------------
   static int mPassed, mFailed;    // Number of test cases passed and failed
   #endregion
}
#endregion