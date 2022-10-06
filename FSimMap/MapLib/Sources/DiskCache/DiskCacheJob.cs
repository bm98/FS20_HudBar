using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.DiskCache
{
  /// <summary>
  /// Disk Cache Job - save a tile
  /// </summary>
  internal class DiskCacheJob
  {
    /// <summary>
    /// The MapImageID to cache
    /// </summary>
    public MapImageID MapImageID { get; private set; }

    /// <summary>
    /// The Cache Provider
    /// </summary>
    public LiteDBCache CacheProvider { get; private set; }
    /// <summary>
    /// The Data to Cache
    /// </summary>
    public byte[] Data { get; private set; }

    public DiskCacheJob( MapImageID mapImageID , LiteDBCache cacheProvider, byte[] data )
    {
      MapImageID = mapImageID;
      CacheProvider = cacheProvider;
      Data = data;
    }

    public override string ToString( )
    {
      return MapImageID.ToString( );
    }

  }
}
