using System;

namespace MapLib
{
  /// <summary>
  /// Event Args for the LoadComplete Event
  /// </summary>
  public class LoadCompleteEventArgs : EventArgs
  {
    /// <summary>
    /// A Full Tile Key (provider¦Znn¦Xnnnn¦Ynnnn)
    /// </summary>
    public string TileKey { get; set; }
    /// <summary>
    /// A Full Tile Tracking Key (provider¦Znn¦Xnnnn¦Ynnnn|n..)
    /// </summary>
    public string TrackKey { get; set; }
    /// <summary>
    /// True when Tile or Matrix loading was cancelled
    /// </summary>
    public bool LoadCancelled { get; set; }
    /// <summary>
    /// True when Tile or Matrix loading has failed
    /// </summary>
    public bool LoadFailed { get; set; }
    /// <summary>
    /// True when the Matrix loading has completed
    /// </summary>
    public bool MatrixComplete { get; set; }

    /// <summary>
    /// Event Args for Matrix Load Complete events
    /// </summary>
    /// <param name="tileKey">A tile Image Key</param>
    /// <param name="trackKey">A tile tracking Key</param>
    /// <param name="loadCancelled">True if loading was cancelled</param>
    /// <param name="loadFailed">True if loading failed</param>
    /// <param name="matComplete">True if Matrix loading has completed</param>
    public LoadCompleteEventArgs( string tileKey, string trackKey, bool loadCancelled, bool loadFailed, bool matComplete )
    {
      TileKey = tileKey;
      TrackKey = trackKey;
      LoadCancelled = loadCancelled;
      LoadFailed = loadFailed;
      MatrixComplete = matComplete;
    }

  }
}
