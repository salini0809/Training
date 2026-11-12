// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Deque.cs
// Generic double ended queue stored in an array that is used as a circular buffer.
// ------------------------------------------------------------------------------------------------
#region Class TDeque ------------------------------------------------------------------------------
/// <summary>Represents a generic double ended queue implemented using a circular buffer</summary>
class TDeque<T> {
   #region Properties -----------------------------------------------
   /// <summary>Checks whether the deque is empty</summary>
   public bool IsEmpty => mCount == 0;

   /// <summary>Number of elements currently stored in the deque</summary>
   public int Count => mCount;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Adds an element at the head  of the deque</summary>
   public void EnqueueHead (T a) {
      if (mCount == mItems.Length) Grow ();
      mHead = (mHead - 1 + mItems.Length) % mItems.Length;
      mItems[mHead] = a;
      mCount++;
   }

   /// <summary>Adds an element at the tail of the deque</summary>
   public void EnqueueTail (T a) {
      if (mCount == mItems.Length) Grow ();
      mItems[(mHead + mCount) % mItems.Length] = a;
      mCount++;
   }

   /// <summary>Removes and returns the element at the head of the deque</summary>
   public T DequeueHead () {
      if (IsEmpty) throw new InvalidOperationException ("Cannot dequeue from an empty deque");
      T item = mItems[mHead];
      mItems[mHead] = default!;
      mHead = (mHead + 1) % mItems.Length;
      mCount--;
      return item;
   }

   /// <summary>Removes and returns the element at the tail of the deque</summary>
   public T DequeueTail () {
      if (IsEmpty) throw new InvalidOperationException ("Cannot dequeue from an empty deque");
      int tail = (mHead + mCount - 1) % mItems.Length;
      T item = mItems[tail];
      mItems[tail] = default!;
      mCount--;
      return item;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Increases the deque capacity and copies elements in head-to-tail order
   void Grow () {
      T[] temp = new T[mItems.Length * 2];
      int end = Math.Min (mCount, mItems.Length - mHead);
      Array.Copy (mItems, mHead, temp, 0, end);
      Array.Copy (mItems, 0, temp, end, mCount - end);
      mItems = temp;
      mHead = 0;
   }
   #endregion

   #region Private data ---------------------------------------------
   T[] mItems = new T[4];   // Stores the deque elements in a circular buffer
   int mHead;               // Index of the first element in the deque
   int mCount;              // Number of elements currently stored in the deque
   #endregion
}
#endregion