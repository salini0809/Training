// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Queue.cs
// Generic FIFO queue stored in an array that is used as a circular buffer.
// ------------------------------------------------------------------------------------------------

#region Class TQueue ------------------------------------------------------------------------------
/// <summary>Represents a generic FIFO queue implemented using a circular buffer</summary>
/// <typeparam name="T">Type of elements stored in the queue</typeparam>
class TQueue<T> {
   #region Properties -----------------------------------------------
   /// <summary>Checks whether the queue is empty</summary>
   public bool IsEmpty => mCount == 0;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Adds an element to the end of the queue</summary>
   /// <param name="a">Element to enqueue</param>
   public void Enqueue (T a) {
      if (mCount == mItems.Length) Grow ();
      mItems[(mHead + mCount) % mItems.Length] = a;
      mCount++;
   }

   /// <summary>Removes and returns the element at the beginning of the queue</summary>
   /// <exception cref="InvalidOperationException">Thrown when the queue is empty</exception>
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
      T[] temp = new T[mItems.Length * 2];
      int end = Math.Min (mCount, mItems.Length - mHead);
      Array.Copy (mItems, mHead, temp, 0, end);
      Array.Copy (mItems, 0, temp, end, mCount - end);
      mItems = temp;
      mHead = 0;
   }
   #endregion

   #region Private data ---------------------------------------------
   T[] mItems = new T[4];   // Stores the queue elements in a circular buffer
   int mHead;               // Index of the first element in the queue
   int mCount;              // Number of elements currently stored in the queue
   #endregion
}
#endregion