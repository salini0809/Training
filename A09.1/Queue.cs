// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TQueue.cs
// Generic FIFO queue stored in an array that is used as a circular buffer.
// ------------------------------------------------------------------------------------------------

#region Class TQueue ------------------------------------------------------------------------------
/// <summary> Represents a generic FIFO queue implemented using a circular buffer </summary>
/// <typeparam name="T"></typeparam>
class TQueue<T> {
   #region Properties -----------------------------------------------
   /// <summary> checks whether the queue is empty </summary>
   public bool IsEmpty => mCount == 0;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary> Adds an element to the end of the queue </summary>
   /// <param name="a"></param>
   public void Enqueue (T a) {
      if (mCount == mItems.Length) Grow ();
      mItems[(mHead + mCount) % mItems.Length] = a;
      mCount++;
   }

   /// <summary>Removes and returns the element at the beginning of the queue </summary>
   /// <exception cref="InvalidOperationException"></exception>
   public T Dequeue () {
      if (IsEmpty) throw new InvalidOperationException ("Cannot dequeue from an empty queue");
      T item = mItems[mHead];
      mItems[mHead] = default!;
      mHead = (mHead + 1) % mItems.Length;
      mCount--;
      return item;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Increases the queue capacity and copies elements in FIFO order
   void Grow () {
      T[] bigger = new T[mItems.Length * 2];
      int toEnd = Math.Min (mCount, mItems.Length - mHead);
      Array.Copy (mItems, mHead, bigger, 0, toEnd);
      Array.Copy (mItems, 0, bigger, toEnd, mCount - toEnd);
      mItems = bigger;
      mHead = 0;
   }
   #endregion

   #region Private data ---------------------------------------------
   T[] mItems = new T[4];   // Stores the queue elements in a circular buffer
   int mHead;               //Index of the first element in the queue
   int mCount;              //Number of elements currently stored in the queue
   #endregion
}
#endregion