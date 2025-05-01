using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// Track of an aircraft to be drawn on the map
  /// </summary>
  internal class DispTrack
  {
    // how many items can be stored in a track
    private const int c_maxTrackLength = 100;
    private ConcurrentQueue<LatLon> _trackQueue;

    private int _maxTrackLength = c_maxTrackLength;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="maxLength">Max number of track point to maintain</param>
    public DispTrack( int maxLength = c_maxTrackLength )
    {
      _trackQueue = new ConcurrentQueue<LatLon>( );
      _maxTrackLength = maxLength;
    }

    /// <summary>
    /// Clear the Track
    /// </summary>
    public void Clear( )
    {
      while (!_trackQueue.IsEmpty) _trackQueue.TryDequeue( out LatLon _ );
    }

    /// <summary>
    /// Number of track points currently held
    /// </summary>
    public int Count( ) => _trackQueue.Count( );

    public void AddPoint( LatLon trackPoint )
    {
      _trackQueue.Enqueue( trackPoint );
      if (_trackQueue.Count( ) > c_maxTrackLength) _trackQueue.TryDequeue( out LatLon _ );
    }

    /// <summary>
    /// Returns a List of stored Track Points
    /// </summary>
    public IList<LatLon> TrackPoints( )
    {
      return _trackQueue.ToList( );
    }

  }
}
