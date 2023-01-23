using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Tiles
{
  /// <summary>
  /// Utility to server MapTiles 
  /// </summary>
  internal class MapTileServer : IDisposable
  {
    // expected number of tiles to handle
    private uint _numTiles = 0;

    // tracking list of all tiles created
    private List<MapTile> _tiles;
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
        _tiles.Add( tile );
        if (_tiles.Count > _numTiles) {
#if DEBUG
          // too many consumed - track the behavior on Slow Providers (Stamen...)
          Console.WriteLine( $"Tiles in circulation: {_tiles.Count}" );
          throw new ApplicationException( "No more tiles to serve" );
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
      if (_tiles.Contains( mapTile )) {
        mapTile.ClearTileContent( );
        _tileQueue.Enqueue( mapTile ); // back into stock
      }
      else {
        // we did not serve this returned tile... (Programm Error)
        throw new ApplicationException( "Returned unsolicited MapTile" );
      }
    }

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          foreach (var tile in _tiles) { tile.Dispose( ); }
          _tiles.Clear( );
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
