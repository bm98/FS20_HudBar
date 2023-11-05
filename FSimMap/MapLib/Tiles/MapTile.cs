using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.LLShapes;
using DbgLib;

namespace MapLib.Tiles
{
  /// <summary>
  /// Represents one MapTile
  /// </summary>
  internal class MapTile : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // To lock the Img Use
    private readonly object _imageLockObj = new object( );

    private readonly TileXY c_tileNone = TileXY.Empty;

    private int _loadCount = 0;
    // a tile version for ID
    private int _version = 0;
    private EventHandler<LoadCompleteEventArgs> _handler = null;
    // obsolesence support
    private bool _obsolete = false;

    /// <summary>
    /// Event triggered on LoadComplete or LoadFailed
    /// </summary>
    private event EventHandler<LoadCompleteEventArgs> LoadComplete;

    // Signal the user that data has arrived
    private void OnLoadComplete( string tileKey, string trackKey, bool failed )
    {
      //  Debug.WriteLine( $"{DateTime.Now.Ticks} MapTile.OnLoadComplete- Key: <{FullKey}> Failed: {failed}" );
      if (LoadComplete == null)
        LOG.LogError( "MapTile.OnLoadComplete", "NO EVENT RECEIVERS HAVE REGISTERED" );

      LoadComplete?.Invoke( this, new LoadCompleteEventArgs( tileKey, trackKey, failed, false ) ); // Tile cannot make the Matrix complete
    }

    // Inputs while Loading

    /// <summary>
    /// Get: The Map ZoomLevel for this Tile
    /// </summary>
    public ushort ZoomLevel { get; private set; }
    /// <summary>
    /// Get: The used Map Provider
    /// </summary>
    public MapProvider MapProvider { get; private set; } = MapProvider.DummyProvider;


    // must be tracked and updated

    /// <summary>
    /// The MapImage stored here
    /// </summary>
    public MapImageID MapImageID { get; private set; } = new MapImageID( );

    /// <summary>
    /// Status of the Loading process for this Tile
    /// </summary>
    public ImageLoadingStatus LoadingStatus { get; private set; } = ImageLoadingStatus.Unknown;

    /// <summary>
    /// The MapTile Version
    /// </summary>
    public int Version => _version;

    /// <summary>
    /// True if marked as obsolete
    /// </summary>
    public bool IsObsolete => _obsolete;

    // calc from live values so we don't need to track changes

    /// <summary>
    /// The tileXY stored here
    /// Return (-1,-1) if not initialized
    /// </summary>
    public TileXY TileXY => MapImageID.TileXY;

    /// <summary>
    /// Set a TileXY for the next Update 
    /// </summary>
    public TileXY TileXYUpdate { get; set; }

    /// <summary>
    /// True if the Tile will Update
    /// </summary>
    public bool NeedsUpdate => TileXYUpdate != c_tileNone;


    /*
    /// <summary>
    /// A QuadKey of this TileXY (for this ZoomLevel)
    /// </summary>
    public string QuadKey => Tools.ToQuadKey( MapImageID.TileXY );
    */

    /// <summary>
    /// A full Zoom+XY Key for this Tile
    /// </summary>
    public string ZxyKey => Tools.ToZxyKey( MapImageID );

    /// <summary>
    /// A full Key for this Tile
    /// </summary>
    public string FullKey => Tools.ToFullKey( MapImageID );

    /// <summary>
    /// A full Tracking Key for this Tile
    /// </summary>
    public string TrackKey => Tools.ToTrackKey( MapImageID, _version );

    /// <summary>
    /// Get: The Copyright string of the Map Provider
    /// </summary>
    public string ProviderCopyright => (_providerInstance == null) ? "" : _providerInstance.Copyright;



    #region Calculated props

    // Dimensions

    /// <summary>
    /// Returns the Screen Pixel Width of the Tile
    /// </summary>
    public int TileWidth_pixel => TileSize_pixel.Width;
    /// <summary>
    /// Returns the Screen Pixel Height of the Tile
    /// </summary>
    public int TileHeight_pixel => TileSize_pixel.Height;
    /// <summary>
    /// Returns the Screen Pixel Dimension of the Tile
    /// </summary>
    public Size TileSize_pixel => CoordLib.MercatorTiles.Projection.TileSize;

    /// <summary>
    /// Get: Horizontal length of one Tile Pixel in meters
    /// </summary>
    public float HorPixelMeasure_m => (float)CoordLib.MercatorTiles.Projection.MapResolution_mPerPixel( ZoomLevel, CenterCoord.Lat );
    /// <summary>
    /// Get: Vertical length of one Tile Pixel in meters
    /// </summary>
    public float VertPixelMeasure_m => (float)CoordLib.MercatorTiles.Projection.MapResolution_mPerPixel( ZoomLevel, CenterCoord.Lat );
    /// <summary>
    /// Get: Dimenstion of one Tile Pixel in meters
    /// </summary>
    public SizeF TilePixelMeasure_m => new SizeF( HorPixelMeasure_m, VertPixelMeasure_m );

    /// <summary>
    /// Get: Horizontal length of this Tile in meters
    /// </summary>
    public float HorTileMeasure_m => HorPixelMeasure_m * TileWidth_pixel;
    /// <summary>
    /// Get: Vertical length of this Tile in meters
    /// </summary>
    public float VertTileMeasure_m => VertPixelMeasure_m * TileHeight_pixel;
    /// <summary>
    /// Get: Dimenstion of the Tile in meters
    /// </summary>
    public SizeF TileMeasure_m => new SizeF( HorTileMeasure_m, VertTileMeasure_m );

    // Coords

    /// <summary>
    /// Get: Coordinate of the Tile Center Point
    /// </summary>
    public LatLon CenterCoord => TileXY.CenterLatLon( ZoomLevel );

    /// <summary>
    /// Get: Returns the coordinate of the top left tile pixel
    /// (or 0/0 if the Projection is not yet available)
    /// </summary>
    public LatLon LeftTop_coord => TileXY.LeftTopMapPixel.ToLatLon( ZoomLevel );
    /// <summary>
    /// Get: Returns the coordinate of the top right tile pixel
    /// (or 0/0 if the Projection is not yet available)
    /// </summary>
    public LatLon RightTop_coord => TileXY.RightTopMapPixel.ToLatLon( ZoomLevel );
    /// <summary>
    /// Get: Returns the coordinate of the bottom left tile pixel
    /// (or 0/0 if the Projection is not yet available)
    /// </summary>
    public LatLon LeftBottom_coord => TileXY.LeftBottomMapPixel.ToLatLon( ZoomLevel );
    /// <summary>
    /// Get: Returns the coordinate of the bottom right tile pixel
    /// (or 0/0 if the Projection is not yet available)
    /// </summary>
    public LatLon RightBottom_coord => TileXY.RightBottomMapPixel.ToLatLon( ZoomLevel );

    /// <summary>
    /// Get: An LLRectangle of the covered area
    /// </summary>
    public LLRectangle TileArea_coord => new LLRectangle( LeftTop_coord.Lat, LeftTop_coord.Lon, RightBottom_coord.Lat, RightBottom_coord.Lon );

    #endregion

    /// <summary>
    /// Returns a copy of the Image of the Tile
    /// </summary>
    public Image TileImage => this.GetTileImage( );


    /// <summary>
    /// The internal MapImage 
    /// </summary>
    internal MapImage MapImage { get; private set; } = null;

    // ref to the Provider Instance
    private Sources.Providers.MapProviderBase _providerInstance = null;
    // ref to the Projection Instance
    //    private Projections.ProjectionBase _projectionInstance = null;

    /// <summary>
    /// cTor:
    /// </summary>
    public MapTile( )
    {
      TileXYUpdate = c_tileNone;
    }

    /// <summary>
    /// Mark the Tile as obsolete
    /// </summary>
    public void MarkObsolete( ) => _obsolete = true;

    /// <summary>
    /// Set the Tile configuration the tile
    /// </summary>
    /// <param name="zoomLevel">The Zoomlevel</param>
    /// <param name="provider">The Map Provider</param>
    /// <param name="version">The Tile Version</param>
    /// <param name="loadCompleteHandler">The LoadComplete Handler</param>
    public void Configure( ushort zoomLevel, MapProvider provider, int version, EventHandler<LoadCompleteEventArgs> loadCompleteHandler )
    {
      if (provider == MapProvider.DummyProvider) {
        LOG.LogError( "MapTile.Configure", "ERROR Invalid MapProvider" );
        throw new ArgumentException( "Invalid MapProvider" ); // cannot
      }

      ZoomLevel = zoomLevel;
      MapProvider = provider;
      _version = version;
      _handler = loadCompleteHandler;
      this.LoadComplete += loadCompleteHandler;
    }

    /// <summary>
    /// Load this Tile
    /// </summary>
    /// <param name="tileXY">The TileXY</param>
    /// <param name="trackingList">The Tile tracking list</param>
    /// <returns>True if loading</returns>
    public bool LoadTile( TileXY tileXY, TrackingCat trackingList )
    {
      if (LoadingStatus == ImageLoadingStatus.Loading) {
        // the Tile is currently loading from a prev call
        // wait until this one has finished before loading a new one
        LOG.Log( "MapTile.LoadTile", $"Busy Tile {TrackKey}" );
        return false;// cannot load when loading
      }
      _providerInstance = Sources.Providers.MapProviderBase.GetProviderInstance( MapProvider );

      string oldTile = "Initial load";
      if (NeedsUpdate) {
        oldTile = "Update from " + FullKey;
      }

      MapImageID = new MapImageID( tileXY, ZoomLevel, MapProvider );
      TileXYUpdate = c_tileNone; // clear update

      //      Debug.WriteLine( $"MapTile.LoadTile: Loading {FullKey} ({oldTile})" );

      TileLoaderJob tileLoaderJob = new TileLoaderJob( tileXY, ZoomLevel, _providerInstance, OnDone );
      if (trackingList.TryAdd( TrackKey, _loadCount++ )) {
        LoadingStatus = ImageLoadingStatus.Loading;
        Service.RequestScheduler.Instance.Add_TileLoaderJob( tileLoaderJob ); // will eventually get the Image
        return true;
      }
      else {
        LoadingStatus = ImageLoadingStatus.LoadFailed;
        // could not add tracking ??
        if (trackingList.ContainsKey( TrackKey )) {
          LOG.LogError( "MapTile.LoadTile", $"_trackingList.TryAdd FAILED for {TrackKey} - Key exists" );
        }
        else {
          LOG.LogError( "MapTile.LoadTile", $"_trackingList.TryAdd FAILED for {TrackKey} - Locked ??" );
        }

      }
      return false;
    }

    /// <summary>
    /// Load a Tile from a Coordinate anywhere on this Tile
    ///  Tile has to be Configured before loading
    /// </summary>
    /// <param name="coordOnTile">The coord which the Tile should include</param>
    /// <param name="trackingList">The Tile tracking list</param>
    /// <returns>True if loading</returns>
    public bool LoadTile( LatLon coordOnTile, TrackingCat trackingList )
    {
      var tileXY = TileXY.LatLonToTileXY( coordOnTile, ZoomLevel );
      return LoadTile( tileXY, trackingList );
    }

    /// <summary>
    /// Update this Tile if needed, 
    /// returns true if Loading, false if Busy or no update required
    /// </summary>
    public bool UpdateTile( TrackingCat trackingList )
    {
      if (NeedsUpdate) {
        return LoadTile( TileXYUpdate, trackingList );
      }
      return false;
    }

    /// <summary>
    /// Tile Image (as copy)
    /// </summary>
    /// <returns>An Image</returns>
    public Image GetTileImage( )
    {
      var bitmap = this.CreateSurface( );
      this.DrawToSurface( bitmap, new Point( 0, 0 ) );
      return bitmap;
    }

    /// <summary>
    /// Method to be exec if the loading has finished
    /// </summary>
    internal void OnDone( )
    {
      // sanity
      if (MapProvider == MapProvider.DummyProvider) return;

      //   Debug.WriteLine( $"{DateTime.Now.Ticks} MapTile.OnDone: Called for: {FullKey}" );
      if (Service.RequestScheduler.Instance.TileWorkflowCatalog.ContainsKey( FullKey )) {
        // try until the concurrent access is granted
        bool removed;
        do {
          removed = Service.RequestScheduler.Instance.TileWorkflowCatalog.TryRemove( FullKey, out MapImage mapImage );
          if (removed) {
            // removed from the Workflow
            //        Debug.WriteLine( $"MapTile.OnDone: Tile {FullKey} removed from workflow, issuing LoadComplete" );

            // need to exclusively use the Image
            lock (_imageLockObj) {
              MapImage?.Dispose( );
              MapImage = mapImage;
            }
            LoadingStatus = (mapImage.IsFailedImage) ? ImageLoadingStatus.LoadFailed : ImageLoadingStatus.LoadComplete;
            OnLoadComplete( MapImage.MapImageID.FullKey, Tools.ToTrackKey( MapImage.MapImageID, _version ), false );
            // if we had concurrent loading and updating - see if there is an update needed now
            //   UpdateTile( );
            return;
          }
        } while (!removed);
        ;// CANNOT LAND HERE
      }

      // failed case
      LOG.LogError( "MapTile.OnDone", $"Could not get the image: {TrackKey}" );
      LoadingStatus = ImageLoadingStatus.LoadFailed;
      OnLoadComplete( MapImageID.FullKey, Tools.ToTrackKey( MapImage.MapImageID, _version ), true );
    }

    /// <summary>
    /// Clears the contents and resets this tile
    /// </summary>
    public void ClearTileContent( )
    {
      ZoomLevel = 0;
      MapProvider = MapProvider.DummyProvider;
      MapImageID = new MapImageID( TileXY.Empty, 0, MapProvider.DummyProvider );
      _version = 0;
      _obsolete = false;
      _providerInstance = null;
      this.LoadComplete -= _handler;
      // Clear allocated resources

      // need to exclusively use the Image
      lock (_imageLockObj) {
        MapImage?.Dispose( ); MapImage = null;
      }
      LoadingStatus = ImageLoadingStatus.Idle;
    }

    #region Drawing Support

    /// <summary>
    /// Create a compatible Bitmap with the given Size
    /// </summary>
    /// <returns>A bitmap or null</returns>
    public Bitmap CreateSurface( Size newSize )
    {
      if (MapImage == null || MapImage.Img == null) return null;

      Bitmap bitMap;
      // need to exclusively use the Image
      lock (_imageLockObj) {
        bitMap = new Bitmap( MapImage.Img, newSize );
      }
      return bitMap;
    }

    /// <summary>
    /// Create a compatible Bitmap
    /// </summary>
    /// <returns>A bitmap or null</returns>
    public Bitmap CreateSurface( )
    {
      if (MapImage == null || MapImage.Img == null) return null;

      Bitmap bitMap;
      // need to exclusively use the Image
      lock (_imageLockObj) {
        bitMap = new Bitmap( MapImage.Img );
      }
      return bitMap;
    }

    /// <summary>
    /// Draw the contained Image to a given bitmap at location
    /// </summary>
    /// <param name="surface">A bitmap</param>
    /// <param name="tlLocation">A TopLeft point</param>
    /// <param name="drawTileBorder">Will draw a red 1px border around the tile</param>
    public void DrawToSurface( Bitmap surface, Point tlLocation, bool drawTileBorder = false )
    {
      using (var g = Graphics.FromImage( surface )) {
        Draw( g, tlLocation, drawTileBorder );
      }
    }

    /// <summary>
    /// Draw the contained Image using the provided graphics context at location
    /// </summary>
    /// <param name="g">Graphics context to draw to</param>
    /// <param name="tlLocation">A TopLeft point</param>
    /// <param name="drawTileBorder">Will draw a red 1px border around the tile</param>
    public void Draw( Graphics g, Point tlLocation, bool drawTileBorder = false )
    {
      if (MapImage == null || this.LoadingStatus != ImageLoadingStatus.LoadComplete || !MapImage.IsValid) {
        var refImg = Properties.Resources.LoadingImage;
        if (this.LoadingStatus == ImageLoadingStatus.LoadFailed) {
          refImg = Properties.Resources.DummyImage; // not available
        }
        g.DrawImage( refImg, tlLocation ); // send a cannot find an image here
        g.DrawRectangle( Pens.Red, tlLocation.X, tlLocation.Y, refImg.Width, refImg.Height );
        return;
      };

      // need to exclusively use the Image
      lock (_imageLockObj) {
        g.DrawImage( MapImage.Img, tlLocation );
        if (drawTileBorder) {
          g.DrawRectangle( Pens.Red, tlLocation.X, tlLocation.Y, MapImage.Img.Width, MapImage.Img.Height );
        }
      }
    }

    #endregion

    #region DISPOSE 

    private bool disposedValue;

    /// <summary>
    /// Dispose pattern implementation
    /// </summary>
    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          ClearTileContent( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MapTile()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    /// <summary>
    /// Dispose pattern implementation
    /// </summary>
    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}
