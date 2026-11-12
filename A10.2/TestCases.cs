// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TestCases.cs
// Test cases for the TDeque<T> circular-buffer double ended queue.
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class DequeTestCases-----------------------------------------------------------------------
/// <summary>Runs test cases for the circular-buffer double ended queue</summary>
class DequeTestCases {
   #region Implementation--------------------------------------------
   // Runs every test and prints a summary
   static void Main () {
      Check ("Dequeue from an empty deque (both ends)", DequeueEmpty);
      Check ("Single element via tail, removed from head", SingleElementTail);
      Check ("Single element via head, removed from tail", SingleElementHead);
      Check ("EnqueueTail + DequeueHead (queue order)", QueueOrder);
      Check ("EnqueueHead + DequeueTail (queue order, other direction)", ReverseQueueOrder);
      Check ("Mixed ends with resize", MixedEnds);
      Check ("Reuses freed slots at the front (wrap around at tail)", WrapAroundTail);
      Check ("Grows when full (EnqueueTail)", GrowWhenFullTail);
      Check ("Grows when full (EnqueueHead)", GrowWhenFullHead);
      Check ("Grows after a wrap around at tail", GrowAfterWrapTail);
      Check ("Grows after a wrap around at head (EnqueueTail)", GrowAfterWrapHeadTail);
      WriteLine ($"\n{mPassed} passed, {mFailed} failed");
   }

   // Tests whether both DequeueHead and DequeueTail throw an exception when the deque is empty
   static bool DequeueEmpty () {
      var d = new TDeque<int> ();
      bool headThrew = false, tailThrew = false;
      try { d.DequeueHead (); } catch (InvalidOperationException) { headThrew = true; }
      try { d.DequeueTail (); } catch (InvalidOperationException) { tailThrew = true; }
      return headThrew && tailThrew;
   }

   // Tests a single element added at the tail and removed from the head
   static bool SingleElementTail () {
      var d = new TDeque<int> ();
      if (!d.IsEmpty) return false;
      d.EnqueueTail (42);
      if (d.IsEmpty) return false;
      return d.DequeueHead () == 42 && d.IsEmpty;
   }

   // Tests a single element added at the head and removed from the tail
   static bool SingleElementHead () {
      var d = new TDeque<int> ();
      if (!d.IsEmpty) return false;
      d.EnqueueHead (42);
      if (d.IsEmpty) return false;
      return d.DequeueTail () == 42 && d.IsEmpty;
   }

   // Tests whether adding at the tail and removing from the head gives FIFO order
   static bool QueueOrder () {
      var d = new TDeque<string> ();
      var data = new[] { "red", "green", "blue" };
      Fill (d, atHead: false, data);
      return Drains (d, fromHead: true, data);
   }

   // Tests whether adding at the head and removing from the tail gives FIFO order
   static bool ReverseQueueOrder () {
      var d = new TDeque<int> ();
      var data = new[] { 1, 2, 3, 4, 5 };
      Fill (d, atHead: true, data);
      return Drains (d, fromHead: false, data);
   }

   // Tests elements added at both ends (forcing a resize while the buffer is wrapped),
   // then removed alternately from both ends
   static bool MixedEnds () {
      var d = new TDeque<int> ();
      d.EnqueueTail (2);
      d.EnqueueHead (1);
      d.EnqueueTail (3);
      d.EnqueueHead (0);   // Buffer (capacity 4) is now full and wrapped
      d.EnqueueTail (4);   // Forces a resize
      d.EnqueueHead (-1);
      return d.DequeueHead () == -1 && d.DequeueTail () == 4
          && d.DequeueHead () == 0 && d.DequeueTail () == 3
          && d.DequeueHead () == 1 && d.DequeueTail () == 2
          && d.IsEmpty;
   }

   // Tests whether the deque reuses freed array slots at the front when adding at the tail
   static bool WrapAroundTail () {
      var d = new TDeque<int> ();
      Fill (d, atHead: false, 1, 2, 3, 4);
      d.DequeueHead ();
      d.DequeueHead ();
      Fill (d, atHead: false, 5, 6);
      return Drains (d, fromHead: true, 3, 4, 5, 6);
   }

   // Tests whether the deque grows when it is full and elements are added at the tail
   static bool GrowWhenFullTail () {
      var d = new TDeque<int> ();
      var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      Fill (d, atHead: false, data);
      return Drains (d, fromHead: true, data);
   }

   // Tests whether the deque grows when it is full and elements are added at the head
   static bool GrowWhenFullHead () {
      var d = new TDeque<int> ();
      Fill (d, atHead: true, 1, 2, 3, 4, 5, 6, 7, 8, 9);
      return Drains (d, fromHead: true, 9, 8, 7, 6, 5, 4, 3, 2, 1);
   }

   // Tests whether the deque grows correctly when the buffer has wrapped at the tail
   static bool GrowAfterWrapTail () {
      var d = new TDeque<char> ();
      Fill (d, atHead: false, 'A', 'B', 'C', 'D');
      d.DequeueHead ();
      d.DequeueHead ();
      Fill (d, atHead: false, 'E', 'F', 'G');
      return Drains (d, fromHead: true, 'C', 'D', 'E', 'F', 'G');
   }

   // Tests whether growth keeps the order when the head had wrapped backwards (EnqueueTail triggers it)
   static bool GrowAfterWrapHeadTail () {
      var d = new TDeque<int> ();
      Fill (d, atHead: false, 1, 2);
      d.EnqueueHead (0);
      d.EnqueueHead (-1);
      d.EnqueueTail (3);
      return Drains (d, fromHead: true, -1, 0, 1, 2, 3);
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

   // Adds the given elements to the head (atHead = true) or the tail (atHead = false) of the deque in order.
   static void Fill<T> (TDeque<T> d, bool atHead, params T[] items) {
      foreach (T item in items) {
         if (atHead) d.EnqueueHead (item);
         else d.EnqueueTail (item);
      }
   }

   // Tests whether removing from the head (fromHead = true) or the tail (fromHead = false)
   // returns the expected elements and the deque becomes empty.
   static bool Drains<T> (TDeque<T> d, bool fromHead, params T[] expected) {
      foreach (T item in expected) {
         if (d.IsEmpty) return false;
         T actual = fromHead ? d.DequeueHead () : d.DequeueTail ();
         if (!EqualityComparer<T>.Default.Equals (actual, item)) return false;
      }
      return d.IsEmpty;
   }
   #endregion

   #region Private data----------------------------------------------
   static int mPassed, mFailed;    // Number of test cases passed and failed
   #endregion
}
#endregion