using DbgLib;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Timers;

namespace MapLib.Tiles
{
  /// <summary>
  /// Utility to serve MapTiles 
  /// </summary>
  internal sealed class MapTileServer : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // expected number of tiles to handle
    private uint _numTiles = 0;

    // tracking list of all tiles created
    //private readonly List<MapTile> _tiles;
    private readonly SynchronizedCollection<MapTile> _tiles;

    // manage no longer used tiles
    private readonly SynchronizedCollection<MapTile> _obsoleteTiles;

    // server queue
    private ConcurrentQueue<MapTile> _tileQueue;

    private readonly Timer _timer;

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="numTiles">Expected mumber of Tiles</param>
    public MapTileServer( uint numTiles )
    {
      _numTiles = numTiles;
      _tileQueue = new ConcurrentQueue<MapTile>( );
      _tiles = new SynchronizedCollection<MapTile>( );
      _obsoleteTiles = new SynchronizedCollection<MapTile>( );

      _timer = new Timer {
        Interval = 5000,
        AutoReset = true
      };
      _timer.Stop( );

      _timer.Elapsed += _timer_Elapsed;
      _timer.Start( );
    }

    // cleanup when needed
    private void _timer_Elapsed( object sender, ElapsedEventArgs e )
    {
      ClearObsoleteTileChore( );
    }


    /// <summary>
    /// Get a new MapTile
    /// </summary>
    /// <returns>A MapTile</returns>
    public MapTile GetTile( )
    {
      if (_tileQueue.TryDequeue( out var result )) {
        // get one from stock
        return result;
      }
      else {
        // must create a new one
        var tile = new MapTile( );
        tile.ClearTileContent( );
        _tiles.Add( tile );

        if (_tiles.Count > _numTiles) {
#if DEBUG
          // many consumed - track the behavior on Slow Providers (Stamen...)
          LOG.Info( "GetTile", $"Tiles in circulation: {_tiles.Count} (mark is {_numTiles})" );
#endif
        }
        return tile;
      }
    }

    /// <summary>
    /// Return a no longer used MapTile back to Stock
    /// </summary>
    /// <param name="mapTile">The returned MapTile</param>
    private void ReturnTile( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      if (_tiles.Contains( mapTile )) {
        if (mapTile.JobPending) {
          // there is a job pending - must wait in the Obsolete stock until done
          ReturnObsoleteTile( mapTile );
        }
        else {
          // either back to stock or dispose if it is an overflow tile
          mapTile.ClearTileContent( );
          if (_tiles.Count > _numTiles) {
            // maintain a max number of tiles in the stock
            if (_tiles.Remove( mapTile )) {
              mapTile.Dispose( );
            }
          }
          else {
            // back into stock if the limit is not reached
            _tileQueue.Enqueue( mapTile );
          }
        }
      }
      else {
        // we did not serve this returned tile... (Programm Error)
        LOG.Error( "ReturnTile( MapTile mapTile )\r\n  {", $"Returned unsolicited MapTile {mapTile.TrackKey}" );
#if DEBUG
        throw new ApplicationException( $"Returned unsolicited MapTile {mapTile.TrackKey}" );
#endif
      }
    }

    #region Obsolete Handling

    // Remove all Obsoletes, must be called at intervals
    private void ClearObsoleteTileChore( )
    {
      // should not change the collection in foreach, so find and then remove
      List<MapTile> obsTiles = new List<MapTile>( );
      lock (_obsoleteTiles.SyncRoot) {
        foreach (var ot in _obsoleteTiles.Where( t => t.JobPending == false )) {
          obsTiles.Add( ot );
        }
      }

      // now remove the found ones
      foreach (var obsTile in obsTiles) {
        // handle obsoletes after the the loading cycle
        if (_obsoleteTiles.Remove( obsTile )) {
          this.ReturnTile( obsTile );
        }
      }
    }

    /// <summary>
    /// Returns a MapTile and marks it as Obsolete
    /// </summary>
    /// <param name="mapTile">An obsolete Tile</param>
    public void ReturnObsoleteTile( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      mapTile.MarkObsolete( );
      _obsoleteTiles.Add( mapTile ); // add - to be removed after completion
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    private void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          ClearObsoleteTileChore( );
          foreach (var tile in _tiles) { tile.Dispose( ); }
          _tiles.Clear( );
          _tileQueue = null;
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MapTileServer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}
