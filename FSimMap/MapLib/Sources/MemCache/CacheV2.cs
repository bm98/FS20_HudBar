using DbgLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MapLib.Sources.MemCache
{
  /// <summary>
  /// Implements a Chache for class objects implementing the ICacheItem Interface
  /// Should be faster than the ConcurrentDictionary
  /// supporting many Reads and not so many Writes in Multithreaded Environment
  /// 
  /// Reads are performed using a string as Key to retrieve an Item
  /// 
  /// https://learn.microsoft.com/en-us/dotnet/api/system.threading.readerwriterlockslim?view=netframework-4.7.2
  /// 
  /// </summary>
  internal class CacheV2
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );


    // the cache
    private const int c_MaxNumberEntries = 800; // about 12 Matrices a 8*8 tiles  (about 25kB per Tile => 20MB)
    private readonly int c_WaterMark = (int)(c_MaxNumberEntries * 0.8); // watermark is 80%
    // Define the shared resource protected by the ReaderWriterLock.
    static Dictionary<string, ICacheItem> _cache = new Dictionary<string, ICacheItem>( );
    // access control
    static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim( );


#if DEBUG
    // Statistics in dev mode
    static int readerTimeouts = 0;
    static int writerTimeouts = 0;
    static int reads = 0;
    static int writes = 0;
#endif

    /// <summary>
    /// Try to add an item (with optional timeout)
    /// </summary>
    /// <param name="key">A Key</param>
    /// <param name="item">The Item to cache</param>
    /// <param name="timeout_ms">Timeout ms (default is wait)</param>
    public void TryAdd( string key, ICacheItem item, int timeout_ms = 0 )
    {
      if (rwl.TryEnterUpgradeableReadLock( timeout_ms )) {
        try {
          if (!_cache.ContainsKey( key )) {
            rwl.EnterWriteLock( );
            try {
              item.TimeStamp = DateTime.Now;
              _cache.Add( key, item );
#if DEBUG
              Interlocked.Increment( ref writes );
#endif
            }
            finally {
              rwl.ExitWriteLock( );
            }
            return;
          }
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitUpgradeableReadLock( );
        }
      }
      else {
#if DEBUG
        Interlocked.Increment( ref writerTimeouts );
#endif
      }
    }

    /// <summary>
    /// Returns true if the cache contains a key
    /// </summary>
    /// <param name="key">A Key</param>
    /// <param name="timeout_ms">Timeout ms (default is wait)</param>
    /// <returns>True if it contains the key</returns>
    public bool Contains( string key, int timeout_ms = 0 )
    {
      bool ret = false;
      if (rwl.TryEnterReadLock( timeout_ms )) {
        try {
          ret = _cache.ContainsKey( key );
#if DEBUG
          Interlocked.Increment( ref writes );
#endif
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitReadLock( );
        }
      }
      else {
#if DEBUG
        Interlocked.Increment( ref readerTimeouts );
#endif
      }
      // exit only here..
      return ret;
    }

    /// <summary>
    /// Try to get an item from the cache
    /// </summary>
    /// <param name="key">A Key</param>
    /// <param name="item">Out: The Item from cache</param>
    /// <param name="timeout_ms">Timeout ms (default is wait)</param>
    /// <returns>True if the item could be retrieved</returns>
    public bool TryGetValue( string key, out ICacheItem item, int timeout_ms = 0 )
    {
      bool ret = false;
      item = null;
      if (rwl.TryEnterReadLock( timeout_ms )) {
        try {
          if (_cache.TryGetValue( key, out ICacheItem value )) {
            item = value;
            ret = true;
          }
#if DEBUG
          Interlocked.Increment( ref reads );
#endif
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitReadLock( );
        }
      }
      else {
#if DEBUG
        Interlocked.Increment( ref readerTimeouts );
#endif
      }
      // exit only here..
      return ret;
    }

    /// <summary>
    /// Remove all items from the Cache
    /// </summary>
    public void Remove_All( int timeout_ms = 0 )
    {
      if (rwl.TryEnterWriteLock( timeout_ms )) {
        try {
          foreach (var entry in _cache) {
            if (_cache.TryGetValue( entry.Key, out ICacheItem _i )) {
              _i.Dispose( );
              _cache.Remove( entry.Key );
            }
          }
#if DEBUG
          Interlocked.Increment( ref writes );
#endif
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitWriteLock( );
        }
      }
      else {
        // The writer lock request timed out.
#if DEBUG
        Interlocked.Increment( ref writerTimeouts );
#endif
      }
    }


    /// <summary>
    /// Remove all items older than the argument from the Cache
    /// </summary>
    public void Remove_OlderThan( DateTime dateTime, int timeout_ms = 0 )
    {
      if (rwl.TryEnterUpgradeableReadLock( timeout_ms )) {
        try {
          var items = _cache.Where( x => x.Value.TimeStamp < dateTime );
          if (items.Count( ) > 0) {
            rwl.EnterWriteLock( );
            try {
              foreach (var entry in items) {
                if (_cache.TryGetValue( entry.Key, out ICacheItem _i )) {
                  _i.Dispose( );
                  _cache.Remove( entry.Key );
                }
              }
#if DEBUG
              Interlocked.Increment( ref writes );
#endif
            }
            finally {
              rwl.ExitWriteLock( );
            }
            return;
          }
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitUpgradeableReadLock( );
        }
      }
      else {
        // The writer lock request timed out.
#if DEBUG
        Interlocked.Increment( ref writerTimeouts );
#endif
      }

    }

    /// <summary>
    /// Removes the oldest items to maintain a max level for the cache
    /// </summary>
    public void MaintainCacheSize( int timeout_ms = 0 )
    {
      if (rwl.TryEnterUpgradeableReadLock( timeout_ms )) {
        try {
          if (_cache.Count > c_MaxNumberEntries) {
            // get below watermark
            int itemsToRemove = _cache.Count - c_WaterMark;
            var rItems = _cache.OrderBy( x => x.Value.TimeStamp );
            LOG.Info( "MemCacheItemCat.MaintainCacheSize",$"Must remove {itemsToRemove} items" );
            if (rItems.Count( ) > 0) {
              rwl.EnterWriteLock( );
              try {
                foreach (var entry in rItems) {
                  if (_cache.TryGetValue( entry.Key, out ICacheItem _i )) {
                    _i.Dispose( );
                    _cache.Remove( entry.Key );
                    itemsToRemove--;
                  }
                  if (itemsToRemove <= 0) break;
                }
#if DEBUG
                Interlocked.Increment( ref writes );
#endif
              }
              finally {
                rwl.ExitWriteLock( );
              }
              return;
            }
          }
        }
        finally {
          // Ensure that the lock is released.
          rwl.ExitUpgradeableReadLock( );
        }
      }
      else {
        // The writer lock request timed out.
#if DEBUG
        Interlocked.Increment( ref writerTimeouts );
#endif
      }
    }

  }
}
