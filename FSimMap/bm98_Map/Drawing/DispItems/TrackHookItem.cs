using System;
using System.Collections.Generic;

using CoordLib;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Implements a ManagedHook which also maintains a Track Queue 
  /// the Queue will compress points after a number of points
  /// </summary>
  internal class TrackHookItem<T> : ManagedHookItem where T : AcftTrackItem
  {
    // main queue
    private int _maxQueueLength = 0;
    private Queue<int> _keyQueue = new Queue<int>( );
    // compressed queue
    private int _maxQueueLengthEx = 0;
    private Queue<int> _keyQueueEx = new Queue<int>( );

    /// <summary>
    /// cTor: 
    /// </summary>
    public TrackHookItem( int maxLenght )
      : base( )
    {
      // Ex is 1/3 of the desired length (or get if the desired length is too short)
      _maxQueueLengthEx = maxLenght / 3;
      if ((_maxQueueLengthEx % 2) == 1) _maxQueueLengthEx++; // must be an Even size or 0
      // the main queue gets the rest
      _maxQueueLength = maxLenght - _maxQueueLengthEx;
      // preset storage size (this will never limit)
      _keyQueue = new Queue<int>( _maxQueueLength + 1 );// add one to add/remove
      _keyQueueEx = new Queue<int>( _maxQueueLengthEx + 1 ); // add one to add/remove
    }

    // maintains the size of the compressed queue
    // this will thin out the compressed queue over time 
    private void MaintainCompressed( )
    {
      if (_keyQueueEx.Count < _maxQueueLengthEx) return; // nothing to do here

      // for all, average two items and leave it half filled until next run
      for (int i = 0; i < _keyQueueEx.Count; i += 2) {
        var delKey = _keyQueueEx.Dequeue( );
        var holdKey = _keyQueueEx.Dequeue( );
        // this will re-add the average to the compressed again
        AddToCompressed( holdKey, delKey );
      }
    }

    // add these to the compressed part and manage the DispList accordingly
    private void AddToCompressed( int holdKey, int delKey )
    {
      // average the two and let item with holdKey survive
      T del = this.SubItemList[delKey] as T;
      T hold = this.SubItemList[holdKey] as T;
      LatLon midPoint = del.CoordPoint.MidpointTo( hold.CoordPoint ); // this is an expensive operation...
      hold.CoordPoint = midPoint;
      // dismiss one point
      this.SubItemList.RemoveItem( delKey );

      // maintain the size of the compressed queue 
      // Note: AddToCompressed is called by MaintainCompressed but by then
      // it should just return as the queue is no longer full .. 
      MaintainCompressed( );
      // then add this one to the compressed queue
      _keyQueueEx.Enqueue( holdKey );
    }

    /// <summary>
    /// Add to the Managed Sublist
    /// </summary>
    public void ManagedAddItem( DisplayItem item )
    {
      _keyQueue.Enqueue( item.Key );
      this.SubItemList.AddItem( item );

      if (_keyQueue.Count > _maxQueueLength) {
        // the main queue is full - 2 items get removed from main
        // and are sent into the compressing Queue
        var delKey = _keyQueue.Dequeue( );
        var holdKey = _keyQueue.Dequeue( );
        AddToCompressed( holdKey, delKey );
      }
    }

    /// <summary>
    /// Clear the managed Sublist
    /// </summary>
    public void ManagedClear( )
    {
      _keyQueue.Clear( );
      _keyQueueEx.Clear( );
      this.SubItemList.Clear( );
    }
  }
}
