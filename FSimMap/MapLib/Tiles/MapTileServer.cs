using DbgLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private List<MapTile> _tiles;
    // manage no longer used tiles
    private List<MapTile> _obsoleteTiles;

    // server queue
    private ConcurrentQueue<MapTile> _tileQueue;

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="numTiles">Expected mumber of Tiles</param>
    public MapTileServer( uint numTiles )
    {
      _numTiles = numTiles;
      _tileQueue = new ConcurrentQueue<MapTile>( );
      _tiles = new List<MapTile>( (int)numTiles );
      _obsoleteTiles = new List<MapTile>( (int)numTiles / 2 );
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
        lock (_tiles) {
          _tiles.Add( tile );
        }
        if (_tiles.Count > _numTiles) {
#if DEBUG
          // many consumed - track the behavior on Slow Providers (Stamen...)
          LOG.Log( "GetTile", $"Tiles in circulation: {_tiles.Count} (mark is {_numTiles})" );
#endif
        }
        return tile;
      }
    }

    /// <summary>
    /// Return a MapTile back to Stock
    /// </summary>
    /// <param name="mapTile">The returned MapTile</param>
    public void ReturnTile( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      if (_tiles.Contains( mapTile )) {
        mapTile.ClearTileContent( );
        if (_tiles.Count > _numTiles) {
          // maintain a max number of tiles in the stock
          lock (_tiles) {
            _tiles.Remove( mapTile );
          }
          mapTile.Dispose( );
        }
        else {
          // back into stock if the limit is not reached
          _tileQueue.Enqueue( mapTile );
        }
      }
      else {
        // we did not serve this returned tile... (Programm Error)
        LOG.LogError( "ReturnTile( MapTile mapTile )\r\n    {", $"Returned unsolicited MapTile {mapTile.TrackKey}" );
        throw new ApplicationException( $"Returned unsolicited MapTile {mapTile.TrackKey}" );
      }
    }

    /// <summary>
    /// Returns an obsolete tile - to be hold until removed
    /// </summary>
    /// <param name="mapTile">An obsolete Tile</param>
    public void ReturnObsoleteTile( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      lock (_obsoleteTiles) {
        _obsoleteTiles.Add( mapTile ); // add - to be removed after completion
        mapTile.MarkObsolete( );
      }
    }

    /// <summary>
    /// Remove this tile from the obsolete list
    /// </summary>
    /// <param name="tileKey">A tile Key</param>
    public void RemoveObsoleteTile( string tileKey )
    {
      if (string.IsNullOrEmpty( tileKey )) { return; } // fast exit

      // handle obsoletes after the the loading cycle
      lock (_obsoleteTiles) {
        var obsolete = _obsoleteTiles.FirstOrDefault( x => x.TrackKey == tileKey );
        if (obsolete != null) {
          _obsoleteTiles.Remove( obsolete );
          this.ReturnTile( obsolete );
        }
      }
    }

    /// <summary>
    /// Remove all Obsoletes
    /// </summary>
    public void ClearObsoleteTiles( )
    {
      lock (_obsoleteTiles) {
        var oTiles = _obsoleteTiles.ToList( );
        foreach (var ot in oTiles) {
          this.ReturnTile( ot );
        }
        _obsoleteTiles.Clear( );
      }
    }

    #region DISPOSE

    private bool disposedValue;

    private void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          ClearObsoleteTiles( );
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
