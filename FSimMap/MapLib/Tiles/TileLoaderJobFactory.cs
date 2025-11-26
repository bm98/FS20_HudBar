using CoordLib.MercatorTiles;
using MapLib.Sources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MapLib.Tiles
{
  /// <summary>
  /// Create an track TileLoaderJobs
  /// </summary>
  internal static class TileLoaderJobFactory
  {
    private static int _jobNumberServed = 0;

    private static int NextJobNumber => Interlocked.Increment( ref _jobNumberServed );

    /// <summary>
    /// The last Job Number that was served
    /// </summary>
    public static int LastJobNumber => _jobNumberServed;

    /// <summary>
    /// Create a Job with Arguments
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <param name="providerRef">Ref to our Manager</param>
    /// <param name="onDone">The Action to be done when successfully retrieved an MapImage</param>
    public static TileLoaderJob CreateJob( MapImageID mapImageID, MapProviderBase providerRef, Action onDone )
    {
      return new TileLoaderJob( mapImageID, providerRef, onDone, NextJobNumber );
    }

    /// <summary>
    /// Create a Job with Arguments
    /// </summary>
    /// <param name="tileXY">The XY Tile Position</param>
    /// <param name="zoom">The Zoomlevel</param>
    /// <param name="providerRef">Ref to our Manager</param>
    /// <param name="onDone">The Action to be done when successfully retrieved an MapImage</param>
    public static TileLoaderJob CreateJob( TileXY tileXY, ushort zoom, MapProviderBase providerRef, Action onDone )
    {
      return new TileLoaderJob( new MapImageID( tileXY, zoom, providerRef.MapProvider ), providerRef, onDone, NextJobNumber );
    }



  }
}
