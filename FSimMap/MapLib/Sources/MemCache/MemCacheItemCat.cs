using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MapLib.Sources.MemCache
{
  /// <summary>
  /// A Catalog of cached items  (NOT IN USE)
  /// </summary>
  internal class MemCacheItemCat : IDisposable
  {
    // the cache
    private const int c_MaxNumberEntries = 100;
    private readonly int c_WaterMark = (int)(c_MaxNumberEntries * 0.8); // watermark is 80%
    private ConcurrentDictionary<string, MemCacheItem> _cache = new ConcurrentDictionary<string, MemCacheItem>( 4, c_MaxNumberEntries );


    /// <summary>
    /// Add an Image to the cache
    /// </summary>
    /// <param name="mapImage">A MapImage</param>
    public void Add( MapImage mapImage )
    {
      var mItem = new MemCacheItem( mapImage );
      if (mItem.MapImage.IsValid) {
        if (mItem.MapProvider != MapProvider.DummyProvider) {
          _cache.GetOrAdd( mItem.Key, mItem ); // don't care about the return value here
          MaintainCacheSize( );
        }
      }
      else {
        Debug.WriteLine( $"MemCacheItemCat.Add: Error could not add item: {mItem.Key} - Invalid Image.." );
      }
    }

    /// <summary>
    /// Retrieves a MapImage (copy) for a FullKey
    /// </summary>
    /// <param name="fullKey">A MapImage full key</param>
    /// <returns>A MapImage or null if not found</returns>
    public MapImage Retrieve( string fullKey )
    {
      if (_cache.ContainsKey( fullKey )) {
        if (_cache.TryGetValue( fullKey, out MemCacheItem memCacheItem )) {
          return MapImage.FromArray( memCacheItem.MapImage.DataStream.GetBuffer( ), memCacheItem.MapImage.MapImageID );
        }
      }
      return null;
    }


    /// <summary>
    /// Remove all items from the Cache
    /// </summary>
    public void Remove_All( )
    {
      foreach (var entry in _cache) {
        if (_cache.TryRemove( entry.Key, out MemCacheItem _i )) {
          _i.Dispose( );
        }
      }
    }

    /// <summary>
    /// Remove all items from a Provider from the Cache
    /// </summary>
    public void Remove_ByProvider( MapProvider mapProvider )
    {
      var items = _cache.Where( x => x.Value.MapProvider == mapProvider );
      foreach (var entry in items) {
        if (_cache.TryRemove( entry.Key, out MemCacheItem _i )) {
          _i.Dispose( );
        }
      }
    }

    /// <summary>
    /// Remove all items older than the argument from the Cache
    /// </summary>
    public void Remove_OlderThan( DateTime dateTime )
    {
      var items = _cache.Where( x => x.Value.TimeStamp < dateTime );
      foreach (var entry in items) {
        if (_cache.TryRemove( entry.Key, out MemCacheItem _i )) {
          _i.Dispose( );
        }
      }
    }

    /// <summary>
    /// Removes the oldest items to maintain a max level for the cache
    /// </summary>
    public void MaintainCacheSize( )
    {
      if (_cache.Count > c_MaxNumberEntries) {
        // get below watermark
        int itemsToRemove = _cache.Count - c_WaterMark;
        var rItems = _cache.OrderBy( x => x.Value.TimeStamp );
        Debug.WriteLine( $"MemCacheItemCat.MaintainCacheSize: must remove {itemsToRemove} items" );
        foreach (var entry in rItems) {
          if (_cache.TryRemove( entry.Key, out MemCacheItem _i )) {
            _i.Dispose( );
            itemsToRemove--;
          }
          if (itemsToRemove <= 0) break;
        }
      }
    }


    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          Remove_All( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MemCacheItemCat()
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
