using System;
using System.Drawing;
using System.Diagnostics;

using DbgLib;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.LLShapes;
using System.Threading.Tasks;
using MapLib.Service;
using System.Collections.Concurrent;

namespace MapLib.Tiles
{
  /// <summary>
  /// Represents one MapTile
  ///   sealed - not ready to derive from..
  /// </summary>
  internal class MapTile : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    private static readonly TileXY c_tileNone = TileXY.Empty;

    // To lock the Img Use
    private readonly object _imageLockObj = new object( );
    private readonly object _tileLockObj = new object( );

    // a tile version for ID
    private int _version = 0;
    private EventHandler<LoadCompleteEventArgs> _handler = null;
    // obsolesence support
    private bool _obsolete = false;

    // job to load the image
    private TileLoaderJob _loaderJob = default;

    // IDs that have been overwritten 
    private ConcurrentDictionary<MapImageID, int> _obsoleteMapImageIDs = new ConcurrentDictionary<MapImageID, int>( );

    /// <summary>
    /// Event triggered on LoadComplete or LoadFailed
    /// </summary>
    private event EventHandler<LoadCompleteEventArgs> MapTileLoadComplete;

    // Signal the user that data has arrived
    private void OnMapTileLoadComplete( string tileKey, string trackKey, bool failed, bool cancelled )
    {
      if (MapTileLoadComplete == null)
        LOG.Debug( "MapTile.OnLoadComplete", "NO EVENT RECEIVERS HAVE REGISTERED" );

      MapTileLoadComplete?.Invoke( this,
        new LoadCompleteEventArgs( tileKey, trackKey, loadCancelled: cancelled, loadFailed: failed, matComplete: false ) ); // Tile cannot make the Matrix complete
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

    /// <summary>
    /// The Matrix Element of this Tile
    /// </summary>
    public Point MatrixPixel { get; private set; } = new Point( -1, -1 );

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
    /// True if the Tile is not finished (not Complete or Cancelled)
    /// </summary>
    public bool IsNotFinished => !(LoadingStatus == ImageLoadingStatus.LoadComplete) || (LoadingStatus == ImageLoadingStatus.LoadCancelled);

    /// <summary>
    /// True if loading has failed
    /// </summary>
    public bool HasFailed => (LoadingStatus == ImageLoadingStatus.LoadFailed) || (LoadingStatus == ImageLoadingStatus.LoadError);

    /// <summary>
    /// True if loading has failed but might be retried
    /// </summary>
    public bool CanRetry => (LoadingStatus == ImageLoadingStatus.LoadFailed);


    /// <summary>
    /// The MapTile Version
    /// </summary>
    public int Version => _version;

    /// <summary>
    /// True if marked as obsolete i.e. no longer in use for now
    /// </summary>
    public bool IsObsolete => _obsolete;

    /// <summary>
    /// True if a loader job is pending for this Tile
    /// </summary>
    public bool JobPending => _loaderJob != null;
    /// <summary>
    /// The Loading Job Number or 0
    /// </summary>
    public int JobNumber => JobPending ? _loaderJob.JobNumber : 0;

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
    public void MarkObsolete( )
    {
      _obsolete = true;
      if (_loaderJob != null && (_loaderJob.IsCancelled == false)) {
        _loaderJob?.CancelJob( );
        _loaderJob = null;
        LOG.Trace( "MapTile.MarkObsolete", $"Load cancelled: {this.FullKey}" );
      }
    }

    /// <summary>
    /// Update the internal Matrix Element reference
    /// </summary>
    public void UpdateMatrixPixel( int x, int y ) => MatrixPixel = new Point( x, y );

    /// <summary>
    /// Set the Configuration of the tile
    /// </summary>
    /// <param name="zoomLevel">The Zoomlevel</param>
    /// <param name="provider">The Map Provider</param>
    /// <param name="version">The Tile Version</param>
    /// <param name="matPixel">The MatrixPixel of this tile</param>
    /// <param name="mapTileLoadCompleteHandler">The LoadComplete Handler for a Tile</param>
    public void Configure( ushort zoomLevel, MapProvider provider, int version, Point matPixel, EventHandler<LoadCompleteEventArgs> mapTileLoadCompleteHandler )
    {
      // sanity
      if (provider == MapProvider.DummyProvider) {
        LOG.Error( "MapTile.Configure", "ERROR Invalid MapProvider" );
        throw new ArgumentException( "Invalid MapProvider" ); // cannot
      }
      if (this.IsObsolete) {
        return;  // tile is not active
      }

      // cancel anything from before if there is, should not ...
      if (_loaderJob != null && (_loaderJob.IsCancelled == false)) {
        _loaderJob.CancelJob( );
        if (_handler != null) OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: true, failed: false );
        _loaderJob = null;
        LOG.Trace( "MapTile.Configure", $"Previous load cancelled: {this.FullKey}" );
      }

      lock (_tileLockObj) {
        if (_handler != null) {
          this.MapTileLoadComplete -= _handler;
        }
        // add new 
        _handler = mapTileLoadCompleteHandler;
        this.MapTileLoadComplete += _handler;
        // By design: there shall never be more than one handler allocated ..

        // prep for load call
        _loaderJob = null;
        this.LoadingStatus = ImageLoadingStatus.Idle;

        MatrixPixel = matPixel;
        ZoomLevel = zoomLevel;
        MapProvider = provider;
        _providerInstance = Sources.Providers.MapProviderBase.GetProviderInstance( MapProvider );
        _version = version;
      }
    }

    #region OLD Loader

    // Load this Tile
    private bool LoadTile_low( TileXY tileXY, int x, int y, TrackingCat trackingList )
    {
      // in any case, cancel the current load and remove from tracking
      if (_loaderJob != null && (_loaderJob.IsCancelled == false)) {
        _loaderJob.CancelJob( );
        trackingList.TryRemove( this.TrackKey, out _ );
        // no need to signal as we remove the obsolete tracker from the list here
        LOG.Trace( "MapTile.LoadTile_low", $"Previous load cancelled: {this.FullKey}" );
      }

      if (LoadingStatus == ImageLoadingStatus.Loading) {
        // the Tile is currently loading from a prev call
        LOG.Debug( "MapTile.LoadTile_low", $"Busy Tile {TrackKey}" );
      }

#if DEBUG
      string oldTile = "Initial load";
      if (NeedsUpdate) {
        oldTile = "Update from " + FullKey;
        //Debug.WriteLine( $"MapTile.LoadTile: Loading {this.FullKey} ({oldTile})" );
      }
#endif

      MapImageID = new MapImageID( tileXY, ZoomLevel, MapProvider );
      MatrixPixel = new Point( x, y );
      if (MapImageID.IsValid == false) {
        // shall not happen...
        ;
        return false; // EXIT FAILED
      }

      TileXYUpdate = c_tileNone; // clear update

      //      Debug.WriteLine( $"MapTile.LoadTile: Loading {FullKey} ({oldTile})" );

      _loaderJob = TileLoaderJobFactory.CreateJob( tileXY, ZoomLevel, _providerInstance, OnDone );
      if (trackingList.TryAdd( TrackKey, _loaderJob.JobNumber )) {
        LoadingStatus = ImageLoadingStatus.Loading;
        Service.RequestScheduler.Instance.Add_TileLoaderJob( _loaderJob ); // will eventually get the Image
        return true; // EXIT OK
      }

      else {
        LoadingStatus = ImageLoadingStatus.LoadFailed;
        // could not add tracking ??
        if (trackingList.ContainsKey( TrackKey )) {
          LOG.Error( "MapTile.LoadTile", $"_trackingList.TryAdd FAILED for {TrackKey} - Key exists" );
        }
        else {
          LOG.Error( "MapTile.LoadTile", $"_trackingList.TryAdd FAILED for {TrackKey} - Locked ??" );
        }
      }
      return false; // EXIT FAILED
    }

    /// <summary>
    /// Load this Tile
    /// </summary>
    /// <param name="tileXY">The TileXY</param>
    /// <param name="x">MatrixTilePixel X</param>
    /// <param name="y">MatrixTilePixel Y</param>
    /// <param name="trackingList">The Tile tracking list</param>
    /// <returns>True if loading</returns>
    public bool LoadTile( TileXY tileXY, int x, int y, TrackingCat trackingList )
    {
      bool result = LoadTile_low( tileXY, x, y, trackingList );
      return result;
    }

    /// <summary>
    /// Load a Tile from a Coordinate anywhere on this Tile
    ///  Tile has to be Configured before loading
    /// </summary>
    /// <param name="coordOnTile">The coord which the Tile should include</param>
    /// <param name="x">MatrixTilePixel X</param>
    /// <param name="y">MatrixTilePixel Y</param>
    /// <param name="trackingList">The Tile tracking list</param>
    /// <returns>True if loading</returns>
    public bool LoadTile( LatLon coordOnTile, int x, int y, TrackingCat trackingList )
    {
      return LoadTile( TileXY.LatLonToTileXY( coordOnTile, ZoomLevel ), x, y, trackingList );
    }

    /// <summary>
    /// Update this Tile if needed, 
    /// returns true if Loading, false if Busy or no update required
    /// </summary>
    public bool UpdateTile( int x, int y, TrackingCat trackingList )
    {
      if (NeedsUpdate) {
        return LoadTile( TileXYUpdate, x, y, trackingList ); // send the updated Tile Coord
      }
      return false;
    }

    #endregion

    #region NEW LOADER

    /// <summary>
    /// Start a Tile Loading - returns when loaded or erred
    /// </summary>
    /// <param name="tileXY">The TileXY to load</param>
    /// <param name="x">MatrixTilePixel X</param>
    /// <param name="y">MatrixTilePixel Y</param>
    /// <returns>True when an Image was retrieved</returns>
    public bool LoadTile_EX1( TileXY tileXY, int x, int y )
    {
      if (this.IsObsolete) {
        return false; // Tile is no longer active
      }

      if (MatrixPixel != new Point( x, y )) {
        ; // tile mismatch or concurrency issue
      }

      TileLoaderJob tileLoaderJob;

      lock (_tileLockObj) {
        LoadingStatus = ImageLoadingStatus.Idle; // clear below
        var newMapImageID = new MapImageID( tileXY, ZoomLevel, MapProvider );
        // sanity check and exit
        if (!newMapImageID.IsValid) return false;

        //Console.WriteLine( $"ADD NEW: {newMapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );
        // manage obsolete list - concurrency issues
        if (_obsoleteMapImageIDs.ContainsKey( newMapImageID )) {
          // new one is marked obs - remove it
          _obsoleteMapImageIDs.TryRemove( newMapImageID, out _ );
          //Console.WriteLine( $"REDO: {newMapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );
        }
        else if (MapImageID.IsValid) {
          // we have a current one
          if (newMapImageID != MapImageID) {
            // add the current one as obs
            _obsoleteMapImageIDs.TryAdd( MapImageID, _obsoleteMapImageIDs.Count );
            //Console.WriteLine( $"MARK OBS: {MapImageID} for NEW {newMapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );
          }
        }

        // current sought MapImage
        MapImageID = newMapImageID;
        TileXYUpdate = c_tileNone; // clear update

        tileLoaderJob = TileLoaderJobFactory.CreateJob( tileXY, ZoomLevel, _providerInstance, null );
      } //lock

      // return a wrapper with active providers set
      var jobWrapper = RequestScheduler.Instance.GetJobWrapper( tileLoaderJob );

      // Send the tile loading job to the task runner 
      LoadingStatus = ImageLoadingStatus.Loading;
      TileMatrix.Asynch_JobRunner.AddJob(
        new dNetBm98.Job.JobObj<LoaderJobWrapper>( TileLoaderExec, jobWrapper, jobWrapper.MapImageID.ToString( ) )
        );

      return true;
    }

    /// <summary>
    /// Update this Tile if needed, 
    /// returns true if Loading, false if Busy or no update required
    /// </summary>
    /// <param name="x">MatrixTilePixel X</param>
    /// <param name="y">MatrixTilePixel Y</param>
    public bool UpdateTile_EX1( int x, int y )
    {
      if (NeedsUpdate) {
        return LoadTile_EX1( TileXYUpdate, x, y ); // send the updated Tile Coord
      }
      return false;
    }


    // exec the JobWrapper using Asynch Loading
    private void TileLoaderExec( LoaderJobWrapper jobWrapper )
    {
      if (jobWrapper != null) {
        var service = jobWrapper.GetNextSource( );
        if (service != null) {
          LoadingStatus = ImageLoadingStatus.Loading;
          //Console.WriteLine( $"REQUEST: {MapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );
          MapImage mapImage = service.GetTileImage_Asynch( jobWrapper ).GetAwaiter( ).GetResult( );
          if (mapImage != null) {
            //Console.WriteLine( $"GOT: {mapImage.MapImageID} for {MapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );

            // check if the image is not already obs
            if (_obsoleteMapImageIDs.ContainsKey( mapImage.MapImageID )) {
              _obsoleteMapImageIDs.TryRemove( mapImage.MapImageID, out _ );
              //Console.WriteLine( $"OBSOLETE IGNORED: {mapImage.MapImageID} for Tile: {x}|{y} ({MatrixPixel.X}|{MatrixPixel.Y})" );
              return;
            }

#if DEBUG
            // DEBUG CHECK ONLY
            if (mapImage.MapImageID != MapImageID) {
              ; // DEBUG ONLY - shall not happen..
                //              throw new Exception( "MapImageID does not match" );
              return;
            }
#endif

            // need to exclusively use the Image
            // failed ones are reported with a replacement image
            lock (_imageLockObj) {
              this.MapImage?.Dispose( ); // old image
              this.MapImage = mapImage;  // new image
            }
            LoadingStatus = mapImage.IsFailedImage
              ? (mapImage.ShouldRetry ? ImageLoadingStatus.LoadFailed : ImageLoadingStatus.LoadError)
              : ImageLoadingStatus.LoadComplete;
          }
        }

        // final decisions
        if (this.IsObsolete) {
          LOG.Debug( "MapTile.LoadTile_Asynch", $"Obsolete Tile: {TrackKey} not loaded for Tile: ({MatrixPixel.X}|{MatrixPixel.Y})" );
          LoadingStatus = ImageLoadingStatus.LoadCancelled;
          OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: true, failed: false );
        }
        else {
          // report
          OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: false, failed: this.HasFailed );
        }
      }
    }

    #endregion


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
      if (MapProvider == MapProvider.DummyProvider) {
        _loaderJob = null;
        return;
      }

      if (_loaderJob.MapImageID != this.MapImageID) {
        ; // DEBUG ONLY - shall not happen..
#if DEBUG
        throw new Exception( "MapImageID does not match" );
#else
        _loaderJob = null;
        return;
#endif
      }

      //   Debug.WriteLine( $"{DateTime.Now.Ticks} MapTile.OnDone: Called for: {FullKey}" );
      // check if the image is still in the Workflow, and remove it
      if (Service.RequestScheduler.Instance.TileWorkflowCatalog.ContainsKey( this.FullKey )) {
        // try until the concurrent access is granted
        bool removed;
        do {
          removed = Service.RequestScheduler.Instance.TileWorkflowCatalog.TryRemove( this.FullKey, out TileWorkflow workflow );
          if (removed) {
            MapImage mapImage = workflow.MapImage;
            // removed from the Workflow

            // Debug.WriteLine( $"MapTile.OnDone: Tile {FullKey} removed from workflow, issuing LoadComplete" );

            if (this.IsObsolete) break; // the content of this MapTile is no longer in use

            // need to exclusively use the Image
            // failed ones are reported with a replacement image
            lock (_imageLockObj) {
              this.MapImage?.Dispose( ); // old image
              this.MapImage = mapImage;  // new image
            }
            LoadingStatus = mapImage.IsFailedImage
              ? (mapImage.ShouldRetry ? ImageLoadingStatus.LoadFailed : ImageLoadingStatus.LoadError)
              : ImageLoadingStatus.LoadComplete;

#if DEBUG
            // DEBUG CHECK ONLY
            if (MapImage.MapImageID != MapImageID) {
              ; // DEBUG ONLY - shall not happen..
              throw new Exception( "MapImageID does not match" );
            }
#endif
          }
        } while (!removed);
        ;// CANNOT LAND HERE
      }
      else if (!this.IsObsolete) {
        // no not obsolete and no image in workflow ??, 
        LOG.Error( "MapTile.OnDone", $"Could not get the image: {this.TrackKey}" );
        LoadingStatus = ImageLoadingStatus.LoadError;
      }

      // final decisions
      if (this.IsObsolete) {
        LOG.Debug( "MapTile.OnDone", $"Obsolete image: {TrackKey} not loaded" );
        LoadingStatus = ImageLoadingStatus.LoadCancelled;
        OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: true, failed: false );
      }
      else {
        // report
        OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: false, failed: this.HasFailed );
      }

      _loaderJob = null; // no longer waiting for loading
    }

    /// <summary>
    /// Clears the contents and resets this tile
    /// </summary>
    public void ClearTileContent( )
    {
      // cancel anything from before
      if (_loaderJob != null && (_loaderJob.IsCancelled == false)) {
        _loaderJob.CancelJob( );
        OnMapTileLoadComplete( this.FullKey, this.TrackKey, cancelled: true, failed: false );
        _loaderJob = null;
        LOG.Trace( "MapTile.ClearTileContent", $"Previous load cancelled: {this.FullKey}" );
      }

      if (_handler != null) {
        this.MapTileLoadComplete -= _handler;
        _handler = null;
      }

      MatrixPixel = new Point( -1, -1 );
      ZoomLevel = 0;
      MapImageID = new MapImageID( TileXY.Empty, 0, MapProvider.DummyProvider );
      MapProvider = MapProvider.DummyProvider;
      _providerInstance = null;
      _version = 0;
      _obsolete = false;
      _obsoleteMapImageIDs.Clear( );
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

    private readonly static Bitmap c_LoadingImg = Properties.Resources.LoadingImage;
    private readonly static Bitmap c_LoadFailedImg = Properties.Resources.DummyImage;
    private readonly static Bitmap c_LoadErrorImg = Properties.Resources.RefImage;


    /// <summary>
    /// Draw the contained Image using the provided graphics context at location
    /// </summary>
    /// <param name="g">Graphics context to draw to</param>
    /// <param name="tlLocation">A TopLeft point</param>
    /// <param name="drawTileBorder">Will draw a red 1px border around the tile</param>
    public void Draw( Graphics g, Point tlLocation, bool drawTileBorder = false )
    {
      if (MapImage == null || !MapImage.IsValid || this.IsNotFinished) {
        var refImg = c_LoadingImg;
        if (this.LoadingStatus == ImageLoadingStatus.LoadError) {
          refImg = c_LoadErrorImg; // permanentely not available
        }
        else if (this.LoadingStatus == ImageLoadingStatus.LoadFailed) {
          refImg = c_LoadFailedImg; // not available
        }
        g.DrawImage( refImg, tlLocation ); // send a cannot find an image here
        g.DrawRectangle( Pens.Red, tlLocation.X, tlLocation.Y, refImg.Width, refImg.Height );
        return;
      }


      // need to exclusively use the Image
      lock (_imageLockObj) {
        g.DrawImage( MapImage.Img, tlLocation );
        if (drawTileBorder) {
          g.DrawRectangle( Pens.Red, tlLocation.X, tlLocation.Y, MapImage.Img.Width, MapImage.Img.Height );
        }
      }

    }

    #endregion

    /// <inheritdoc/>
    public override string ToString( ) => MapImageID.ToString( );


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
