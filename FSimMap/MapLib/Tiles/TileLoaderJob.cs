using System;

using CoordLib.MercatorTiles;

using MapLib.Sources.Providers;

namespace MapLib.Tiles
{
  /// <summary>
  /// Tile load Job
  /// </summary>
  internal struct TileLoaderJob
  {
#if TIMING_CAPTURE
    private Stopwatch _timer1;
    private Stopwatch _timer2;
#endif

    /// <summary>
    /// The MapImageID to retrieve
    /// </summary>
    public MapImageID MapImageID { get; private set; }

    /// <summary>
    /// The Map Provider asked for
    /// </summary>
    public MapProviderBase ProviderInstance { get; private set; }

    /// <summary>
    /// An Action to be done once the Image is retrieved
    /// </summary>
    public Action OnDone;

    /// <summary>
    /// cTor: create a Job with Arguments
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <param name="providerRef">Ref to our Manager</param>
    /// <param name="onDone">The Action to be done when successfully retrieved an MapImage</param>
    public TileLoaderJob( MapImageID mapImageID, MapProviderBase providerRef, Action onDone )
    {
      MapImageID = mapImageID;
      ProviderInstance = providerRef;
      OnDone = onDone;

#if TIMING_CAPTURE
      _timer1 = new Stopwatch( ); _timer1.Start( );
      _timer2 = new Stopwatch( ); _timer2.Start( );
#endif
    }

    /// <summary>
    /// cTor: create a Job with Arguments
    /// </summary>
    /// <param name="tileXY">The XY Tile Position</param>
    /// <param name="zoom">The Zoomlevel</param>
    /// <param name="providerRef">Ref to our Manager</param>
    /// <param name="onDone">The Action to be done when successfully retrieved an MapImage</param>
    public TileLoaderJob( TileXY tileXY, ushort zoom, MapProviderBase providerRef, Action onDone )
    {
      MapImageID = new MapImageID( tileXY, zoom, providerRef.MapProvider );
      ProviderInstance = providerRef;
      OnDone = onDone;

#if TIMING_CAPTURE
      _timer1 = new Stopwatch( ); _timer1.Start( );
      _timer2 = new Stopwatch( ); _timer2.Start( );
#endif
    }

    /// <summary>
    /// Signal execution has started
    /// </summary>
    public void StartExec( )
    {
#if TIMING_CAPTURE
      _timer1.Stop( );
#endif
    }

    /// <summary>
    /// Worker Interface, called asynch by a Worker thread
    /// </summary>
    public void ProcessTile( bool success, int taskID )
    {
   //   Debug.WriteLine( $"{DateTime.Now.Ticks} TileLoaderJob.ProcessTile[{taskID}]: Key <{MapImageID.FullKey}> Success: {success}" );

      if (success) {
        OnDone(); // call whatever post processing was asked for
      }
#if TIMING_CAPTURE
      _timer2.Stop( );
      Debug.WriteLine( $"Job {MapImageID} finished - ALTTiming  Queued: {_timer1.ElapsedMilliseconds:#,##0} ms, Processing: {_timer2.ElapsedMilliseconds-_timer1.ElapsedMilliseconds:#,##0} ms, Total: {_timer2.ElapsedMilliseconds:#,##0} ms" );
#endif
    }

    public override string ToString( )
    {
      return MapImageID.ToString( );
    }

  }


}
