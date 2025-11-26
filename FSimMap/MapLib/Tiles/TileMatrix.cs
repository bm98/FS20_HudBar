using System;
using System.Drawing;

using static dNetBm98.XMath;
using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.LLShapes;
using DbgLib;
using System.Threading;

namespace MapLib.Tiles
{
  /// <summary>
  /// A Matrix of Tiles
  /// 
  /// Dispose when not longer used (tiles are disposed as well)
  /// 
  /// </summary>
  public class TileMatrix : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    private static readonly Font c_debugFont = new Font( "Arial", 14 );

    private static dNetBm98.Job.JobRunner _eventJobRunner = new dNetBm98.Job.JobRunner( 8 );
    internal static dNetBm98.Job.JobRunner Asynch_JobRunner => _eventJobRunner;


    // 2d Array of MapTiles [X]|[Y] Root = Left/Top orientation
    private readonly MapTile[,] _mapTiles;
    // lock while updating or query _mapTiles[]
    private object _tileLock = new object( );

    // track scheduled Tiles
    private TrackingCat _tileTrackingList = new TrackingCat( );

    // tracks the extension to apply a unique tile ID
    private int _extendVersion = 0;
    // The tile server
    private MapTileServer _tileServer = null;

    /// <summary>
    /// Event triggered on LoadComplete or LoadFailed
    ///  Returns 
    ///     MatrixComplete=true 
    ///     MatrixComplete=false + TileKey + Failed=true  when one Tile issued an error 
    ///  
    /// </summary>
    public event EventHandler<LoadCompleteEventArgs> LoadComplete;

    // Signal the user that data has arrived
    // Failed is required to be set
    private void OnLoadComplete( string key, bool failed, bool matrixComplete )
    {
      //     Debug.WriteLine( $"{DateTime.Now.Ticks} TileMatrix.OnLoadComplete- Key: <{key}> LoadFailed: {failed} MatComplete: {string.IsNullOrEmpty( key )}" );

      if (LoadComplete == null) {
        LOG.Error( "TileMatrix.OnLoadComplete", $"NO EVENT RECEIVERS HAVE REGISTERED" );
      }
      // set Matrix complete when the key is not provided
      LoadComplete?.Invoke( this, new LoadCompleteEventArgs( key, "dummy", loadCancelled: false, loadFailed: failed, matComplete: matrixComplete ) );
    }

    private Rectangle _mapPixelBounds = new Rectangle( );
    /// <summary>
    /// Bounds of the Matrix in MapPixels at current Zoom
    /// </summary>
    public Rectangle MapPixelBounds {
      get => _mapPixelBounds;
      set => _mapPixelBounds = value;
    }


    /// <summary>
    /// Get: Number of Tiles in Longitude direction
    /// </summary>
    public uint Width { get; private set; }
    /// <summary>
    /// Get: Number of Tiles in Latitude direction
    /// </summary>
    public uint Height { get; private set; }
    /// <summary>
    /// Get: The Map ZoomLevel for this Matrix native Tiles
    /// </summary>
    public ushort ZoomLevel { get; private set; }
    /// <summary>
    /// Get: Status of Image Loading
    /// </summary>
    public ImageLoadingStatus MatrixLoadingStatus { get; private set; } = ImageLoadingStatus.Unknown;

    /// <summary>
    /// True if there are pending Tiles in the Matrix
    /// </summary>
    public bool HasPendingTiles {
      get {
        bool pending = false;
        lock (_tileLock) {
          for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
              pending |= _mapTiles[x, y].IsNotFinished;
            }
          }
        } // lock
        return pending;
      }
    }

    /// <summary>
    /// The Extend Version of the Matrix 
    /// i.e. when the image is shifted by tiles
    /// </summary>
    public int Version => _extendVersion;

    /// <summary>
    /// Get: The used Map Provider
    /// </summary>
    public MapProvider MapProvider { get; private set; }

    /// <summary>
    /// Get: The Copyright string of the Map Provider
    /// </summary>
    public string ProviderCopyright => _mapTiles[0, 0].ProviderCopyright;


    #region Calculated props

    // Dimensions

    /// <summary>
    /// Returns the Screen Pixel Width of the Matrix
    /// </summary>
    public int MatrixWidth_pixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileSize_pixel.Width * (int)Width;
        } // lock
      }
    }

    /// <summary>
    /// Returns the Screen Pixel Height of the Matrix
    /// </summary>
    public int MatrixHeight_pixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileSize_pixel.Width * (int)Height;
        } // lock
      }
    }

    /// <summary>
    /// Returns the Screen Pixel Dimension of the Matrix
    /// </summary>
    public Size MatrixSize_pixel => new Size( MatrixWidth_pixel, MatrixHeight_pixel );

    /// <summary>
    /// Returns the Screen Pixel Width of a Tile
    /// </summary>
    public int TileWidth_pixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileSize_pixel.Width;
        } // lock
      }
    }

    /// <summary>
    /// Returns the Screen Pixel Height of a Tile
    /// </summary>
    public int TileHeight_pixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileSize_pixel.Height;
        } // lock
      }
    }

    /// <summary>
    /// Returns the Screen Pixel Dimension of a Tile
    /// </summary>
    public Size TileSize_pixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileSize_pixel;
        } // lock
      }
    }


    /// <summary>
    /// Get: Horizontal length of one Tile Pixel in meters
    /// </summary>
    public float HorPixelMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].HorPixelMeasure_m;
        } // lock
      }
    }

    /// <summary>
    /// Get: Vertical length of one Tile Pixel in meters
    /// </summary>
    public float VertPixelMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].VertPixelMeasure_m;
        } // lock
      }
    }

    /// <summary>
    /// Get: Dimenstion of one Tile Pixel in meters
    /// </summary>
    public SizeF TilePixelMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TilePixelMeasure_m;
        } // lock
      }
    }

    /// <summary>
    /// Get: Horizontal length of one Tile in meters
    /// </summary>
    public float HorTileMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].HorTileMeasure_m;
        } // lock
      }
    }

    /// <summary>
    /// Get: Vertical length of one Tile in meters
    /// </summary>
    public float VertTileMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].VertTileMeasure_m;
        } // lock
      }
    }

    /// <summary>
    /// Get: Dimenstion of the Tile in meters
    /// </summary>
    public SizeF TileMeasure_m {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileMeasure_m;
        } // lock
      }
    }


    /// <summary>
    /// Get: Horizontal length of the Matrix in meters
    /// </summary>
    public float HorMatrixMeasure_m => HorTileMeasure_m * Width;
    /// <summary>
    /// Get: Vertical length of the Matrix in meters
    /// </summary>
    public float VertMatrixMeasure_m => VertTileMeasure_m * Height;
    /// <summary>
    /// Get: Dimenstion of the Matrix in meters
    /// </summary>
    public SizeF MatrixMeasure_m => new SizeF( HorMatrixMeasure_m, VertMatrixMeasure_m );

    // Coords

    /// <summary>
    /// Get: Coordinate of the Matrix Center Point
    /// </summary>
    public LatLon CenterCoord {
      get {
        if (MapProvider == MapProvider.DummyProvider) return LatLon.Empty;
        return Projection.MapPixelToLatLon(
                LeftTop_mapPixel.X + MatrixWidth_pixel / 2,
                LeftTop_mapPixel.Y + MatrixHeight_pixel / 2,
                ZoomLevel
        );
      }
    }

    /// <summary>
    /// Get: Returns the MapPixel of the left top Matrix pixel 
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel LeftTop_mapPixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].TileXY.LeftTopMapPixel;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the MapPixel of the right top Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel RightTop_mapPixel {
      get {
        lock (_tileLock) {
          return _mapTiles[Width - 1, 0].TileXY.RightTopMapPixel;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the MapPixel of the left bottom Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel LeftBottom_mapPixel {
      get {
        lock (_tileLock) {
          return _mapTiles[0, Height - 1].TileXY.LeftBottomMapPixel;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the MapPixel of the right bottom Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel RightBottom_mapPixel {
      get {
        lock (_tileLock) {
          return _mapTiles[Width - 1, Height - 1].TileXY.RightBottomMapPixel;
        } // lock
      }
    }


    /// <summary>
    /// Get: Returns the coordinate of the top left Matrix pixel
    /// </summary>
    public LatLon LeftTop_coord {
      get {
        lock (_tileLock) {
          return _mapTiles[0, 0].LeftTop_coord;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the coordinate of the top right Matrix pixel
    /// </summary>
    public LatLon RightTop_coord {
      get {
        lock (_tileLock) {
          return _mapTiles[Width - 1, 0].RightTop_coord;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the coordinate of the bottom left Matrix pixel
    /// </summary>
    public LatLon LeftBottom_coord {
      get {
        lock (_tileLock) {
          return _mapTiles[0, Height - 1].LeftBottom_coord;
        } // lock
      }
    }

    /// <summary>
    /// Get: Returns the coordinate of the bottom right Matrix pixel
    /// </summary>
    public LatLon RightBottom_coord {
      get {
        lock (_tileLock) {
          return _mapTiles[Width - 1, Height - 1].RightBottom_coord;
        } // lock
      }
    }


    /// <summary>
    /// Get: An LLRectangle of the covered area
    /// </summary>
    public LLRectangle MatrixArea_coord => new LLRectangle( LeftTop_coord.Lat, LeftTop_coord.Lon, RightBottom_coord.Lat, RightBottom_coord.Lon ); // TODO Widht, Height wrong

    #endregion

    #region Calculation Methods exposed

    /// <summary>
    /// Map a LatLon Coordinate to Map Pixels
    /// </summary>
    /// <param name="latLon">LatLon Coordinate</param>
    /// <returns>A MapPixel Point</returns>
    public MapPixel MapToMapPixel( LatLon latLon )
    {
      if (MapProvider == MapProvider.DummyProvider) return MapPixel.Empty;
      return MapPixel.LatLonToMapPixel( latLon, ZoomLevel );
    }

    /// <summary>
    /// Map a LatLon Coordinate to Matrix Pixels, takes care of wrapping at 180°
    /// </summary>
    /// <param name="latLon">LatLon Coordinate</param>
    /// <returns>A MatrixPixel Point (can be out of range..)</returns>
    public Point MapToMatrixPixel( LatLon latLon )
    {
      var mapP = MapToMapPixel( latLon );
      if (LeftTop_coord.Lon > 0 && latLon.Lon < 0) {
        // map starts east of the Dateline (180°) and the point is on the west side (..pixel max|pixel min..)
        mapP.Offset( -(LeftTop_mapPixel.X - Projection.MapPixelSize( ZoomLevel ).Width), -LeftTop_mapPixel.Y ); // subtract our left top - Width
      }
      else {
        // no wrapping
        mapP.Offset( -LeftTop_mapPixel.X, -LeftTop_mapPixel.Y ); // subtract our left top 
      }
      return mapP.AsPoint( );
    }

    /// <summary>
    /// Map a MapPixel to the LatLon coordinate at zoom
    /// </summary>
    /// <param name="mapPixel">A Map Pixel</param>
    /// <returns>A LatLon</returns>
    public LatLon MapPixelToMap( MapPixel mapPixel )
    {
      if (MapProvider == MapProvider.DummyProvider) return LatLon.Empty;
      return mapPixel.ToLatLon( ZoomLevel );
    }

    /// <summary>
    /// Map a MatrixPixel to the LatLon coordinate at zoom
    /// </summary>
    /// <param name="matrixPixel">A Matrix Pixel</param>
    /// <returns>A LatLon</returns>
    public LatLon MatrixPixelToMap( Point matrixPixel )
    {
      var mapPixel = matrixPixel;
      mapPixel.Offset( LeftTop_mapPixel.X, LeftTop_mapPixel.Y );
      if (mapPixel.X > MapPixelBounds.Width) {
        // map starts east of the Dateline (180°) and stretches over the dateline
        mapPixel.Offset( -Projection.MapPixelSize( ZoomLevel ).Width, 0 ); // subtract the width
      }
      return MapPixelToMap( new MapPixel( mapPixel.X, mapPixel.Y ) );
    }

    #endregion

    /// <summary>
    /// cTor: Create a TileMatrix with width and height #tiles
    /// </summary>
    /// <param name="width">Number of Tiles in Longitude direction</param>
    /// <param name="height">Number of Tiles in Latitude direction</param>
    public TileMatrix( uint width, uint height )
    {
      Width = width;
      Height = height;

      _tileServer = new MapTileServer( Width * Height * 2 );

      // no lock needed here..
      _mapTiles = new MapTile[width, height];
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          _mapTiles[x, y] = _tileServer.GetTile( ); // these are internal x/y s not TileXY !!
        }
      }
      MatrixLoadingStatus = ImageLoadingStatus.Idle; // ready to load something

      Service.RequestScheduler.Instance.PingEvent += RequestScheduler_PingEvent;
    }

    /// <summary>
    /// Returns a Tile from an XY point 
    /// where [0,0] defaults to Left-Top
    /// </summary>
    /// <param name="matrixXY">The Tile designator (0..Width-1| 0..Height-1)</param>
    /// <returns>A Tile or Null</returns>
    internal MapTile GetTile( Point matrixXY )
    {
      if ((matrixXY.X < 0) || (matrixXY.X >= Width)) return null; // Nope
      if ((matrixXY.Y < 0) || (matrixXY.Y >= Height)) return null; // Nope

      MapTile tile = null;
      lock (_tileLock) {
        tile = _mapTiles[matrixXY.X, matrixXY.Y];
      } // lock
      return tile;
    }

    #region Drawing Support

    /// <summary>
    /// Combines the Matrix Tiles into one Image and draws it using the Graphics
    /// </summary>
    /// <param name="g">Graphics context to draw to</param>
    /// <param name="drawTileBorder">Will draw a red 1px border around the tiles</param>
    /// <returns>An Image</returns>
    public void DrawMatrixImage( Graphics g, bool drawTileBorder = false )
    {
      // Debug.WriteLine( $"DrawMatrixImage: {this.CenterCoord}" );

      var refTile = this.GetTile( new Point( 0, 0 ) );
      if (refTile == null) return;

      var tileWidth = this.TileWidth_pixel;
      var tileHeight = this.TileHeight_pixel;

      lock (_tileLock) {
        for (int x = 0, tx = 0; x < this.Width; x++, tx += tileWidth) {
          for (int y = 0, ty = 0; y < this.Height; y++, ty += tileHeight) {
            var drawXy = new Point( tx, ty );
            var drawTile = _mapTiles[x, y];
            if (drawTile != null) {
              drawTile.Draw( g, drawXy, drawTileBorder );
#if DEBUG
              /* DRAW TILE INFO FOR DEBUG
              g.DrawString( $"Tile: {x}/{y}", c_debugFont, Brushes.Red, drawXy );
              if (_mapTiles[x, y].MapImage != null) {
                g.DrawString( _mapTiles[x, y].MapImage.MapImageID.ZxyKey, c_debugFont, Brushes.Red, drawXy.X, drawXy.Y + 20 );
              }
              */
#endif
            }
            else {
              // leave empty
            }
          }
        }
      } // lock
    }

    /// <summary>
    /// Combines the Matrix Tiles into one Image
    /// </summary>
    /// <param name="drawTileBorder">Will draw a red 1px border around the tiles</param>
    /// <returns>An Image</returns>
    public Image GetMatrixImage( bool drawTileBorder = false )
    {
      var refTile = this.GetTile( new Point( 0, 0 ) );
      if (refTile == null) return null; // TODO return a placeholder

      var imageSize = this.MatrixSize_pixel;
      var tileWidth = this.TileWidth_pixel;
      var tileHeight = this.TileHeight_pixel;

      var bitmap = refTile.CreateSurface( imageSize );
      if (bitmap == null) {
        // attempting to load one before the RefTile was created
        bitmap = new Bitmap( Properties.Resources.LoadingImage, imageSize ); // try to create from Stock Image
      }

      lock (_tileLock) {
        for (int x = 0, tx = 0; x < this.Width; x++, tx += tileWidth) {
          for (int y = 0, ty = 0; y < this.Height; y++, ty += tileHeight) {
            var drawXy = new Point( tx, ty );
            var drawTile = _mapTiles[x, y];
            if (drawTile != null) {
              drawTile.DrawToSurface( bitmap, drawXy, drawTileBorder );
            }
            else {
              // leave empty
            }
          }
        }
      } // lock
      return bitmap;
    }

    #endregion

    /// <summary>
    /// Start Loading the Matrix with Tiles around the Center Coordinate
    /// - the map is reloaded from scratch
    /// </summary>
    /// <param name="coordOnCenterTile"></param>
    /// <param name="zoomLevel">The desired Map Zoom level</param>
    /// <param name="provider">The Provider to get the Map from</param>
    public void LoadMatrix( LatLon coordOnCenterTile, ushort zoomLevel, MapProvider provider )
    {
      if (provider == MapProvider.DummyProvider) return; // nope..

      if (MatrixLoadingStatus == ImageLoadingStatus.Loading) {
        // @@@@@@@@ This will invalidate all jobs scheduled so far
        //        Service.RequestScheduler.Instance.JobNumberLimit = TileLoaderJobFactory.LastJobNumber;
        //        _tileTrackingList.RemoveObsoleteJobs( TileLoaderJobFactory.LastJobNumber );
        //        MatrixLoadingStatus = ImageLoadingStatus.LoadCancelled;
        return; // TODO Cancel Loading if already loading
      }

      // as we have one tile overflow to move the matrix the center will be offset to the left top
      // i.e. assume the matrix is one element less in size
      var dWidth = Width - 1;
      var dHeight = Height - 1;

      // start with the 'center' tile - it will be offset to get the TopLeft tile to start loading
      // the quadrant dictates to which side to extend the tiles more than the other for Even Dimensions
      TileXY tlTileXY = TileXY.LatLonToTileXY( coordOnCenterTile, zoomLevel );
      var quadrant = TileXY.QuadrantFromLatLon( coordOnCenterTile, zoomLevel );

      if (dWidth.Even( )) {
        if (dHeight.Even( )) {
          // both even - offset X by quadrant, Y by quadrant
          switch (quadrant) {
            case TileQuadrant.LeftTop: tlTileXY.Offset( -(int)dWidth / 2, -(int)dHeight / 2, zoomLevel ); break;
            case TileQuadrant.RightTop: tlTileXY.Offset( -((int)dWidth / 2 - 1), -(int)dHeight / 2, zoomLevel ); break;
            case TileQuadrant.RightBottom: tlTileXY.Offset( -((int)dWidth / 2 - 1), -((int)dHeight / 2 - 1), zoomLevel ); break;
            case TileQuadrant.LeftBottom: tlTileXY.Offset( -(int)dWidth / 2, -((int)dHeight / 2 - 1), zoomLevel ); break;
            default: break; // program error ...
          }
        }
        else {
          // width even, height odd - offset X by quadrant, Y by (height-1)/2
          tlTileXY.Offset( 0, -((int)dHeight - 1) / 2, zoomLevel );
          switch (quadrant) {
            case TileQuadrant.LeftTop: tlTileXY.Offset( -(int)dWidth / 2, 0, zoomLevel ); break;
            case TileQuadrant.RightTop: tlTileXY.Offset( -((int)dWidth / 2 - 1), 0, zoomLevel ); break;
            case TileQuadrant.RightBottom: tlTileXY.Offset( -((int)dWidth / 2 - 1), 0, zoomLevel ); break;
            case TileQuadrant.LeftBottom: tlTileXY.Offset( -(int)dWidth / 2, 0, zoomLevel ); break;
            default: break; // program error ...
          }
        }
      }
      else {
        if (dHeight.Even( )) {
          // width odd, height even - offset X by (width-1)/2, Y by quadrant
          tlTileXY.Offset( -((int)dWidth - 1) / 2, 0, zoomLevel );
          switch (quadrant) {
            case TileQuadrant.LeftTop: tlTileXY.Offset( 0, -(int)dHeight / 2, zoomLevel ); break;
            case TileQuadrant.RightTop: tlTileXY.Offset( 0, -(int)dHeight / 2, zoomLevel ); break;
            case TileQuadrant.RightBottom: tlTileXY.Offset( 0, -((int)dHeight / 2 - 1), zoomLevel ); break;
            case TileQuadrant.LeftBottom: tlTileXY.Offset( 0, -((int)dHeight / 2 - 1), zoomLevel ); break;
            default: break; // program error ...
          }
        }
        else {
          // both odd - offset X by (width-1)/2, Y by (height-1)/2
          tlTileXY.Offset( -((int)dWidth - 1) / 2, -((int)dHeight - 1) / 2, zoomLevel );
        }
      }
      // sanity check
      if ((tlTileXY.X < 0) || (tlTileXY.Y < 0)) {
        LOG.Error( "LoadMatrix", $"Invalid start TileXY ({tlTileXY})" );
        throw new ArgumentOutOfRangeException( $"Input creates invalid start TileXY ({tlTileXY})" );
      }

      // Get new Tiles
      lock (_tileLock) {
        MatrixLoadingStatus = ImageLoadingStatus.Idle;
        MapProvider = provider;
        ZoomLevel = zoomLevel;
        MapPixelBounds = new Rectangle( new Point( 0, 0 ), Projection.MapPixelSize( zoomLevel ) );

        // remove all from current
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            _tileServer.ReturnObsoleteTile( _mapTiles[x, y] );
            // alloc the new one
            _mapTiles[x, y] = _tileServer.GetTile( );
            _mapTiles[x, y].Configure( zoomLevel, MapProvider, 0, new Point( x, y ), TileMatrix_MapTile_LoadComplete );
          }
        }
        _tileTrackingList.Clear( );
        MatrixLoadingStatus = ImageLoadingStatus.Loading;

        // Start Loading
        //ReloadAllTiles( tlTileXY, zoomLevel );
        ReloadAllTiles_EX1( tlTileXY, zoomLevel );
      } // lock

    }

    // issue LoadTile Jobs for the Matrix
    private void ReloadAllTiles( TileXY tlTileXY, ushort zoomLevel )
    {
      // Start Loading
      for (int x = 0, tx = tlTileXY.X; x < Width; x++, tx++) { // left to right
        for (int y = 0, ty = tlTileXY.Y; y < Height; y++, ty++) { // top to bottom
          var tileXY = new TileXY( tx, ty );
          tileXY.Wrap( zoomLevel ); // in case we are at the edge of the map
          _mapTiles[x, y].LoadTile( tileXY, x, y, _tileTrackingList );
        }
      }
    }

    // issue LoadTile Jobs for the Matrix
    private void ReloadAllTiles_EX1( TileXY tlTileXY, ushort zoomLevel )
    {
      // Start Loading
      for (int x = 0, tx = tlTileXY.X; x < Width; x++, tx++) { // left to right
        for (int y = 0, ty = tlTileXY.Y; y < Height; y++, ty++) { // top to bottom
          var tileXY = new TileXY( tx, ty );
          tileXY.Wrap( zoomLevel ); // in case we are at the edge of the ma
          _mapTiles[x, y].LoadTile_EX1( tileXY, x, y );
        }
      }
    }


    /// <summary>
    /// Add new content TOWARDS the given side(s)
    /// i.e. load the Tiles at the given borders and shift the existing ones to the other side
    /// (Cannot load contradictionary sides i.e. left and right, left and top have prio)
    /// </summary>
    /// <param name="matrixSide">Extend towards side(s)</param>
    public void ExtendMatrix( TileMatrixSide matrixSide )
    {
      // Debug.WriteLine( $"ExtendMatrix: {matrixSide}" );

      // sanity..
      matrixSide = (matrixSide & TileMatrixSide.Left) > 0 ? matrixSide & ~TileMatrixSide.Right : matrixSide; // mask right if left is set
      matrixSide = (matrixSide & TileMatrixSide.Top) > 0 ? matrixSide & ~TileMatrixSide.Bottom : matrixSide; // mask bottom if top is set
      // TODO check for 1,1 Matrix

      lock (_tileLock) {
        Interlocked.Increment( ref _extendVersion ); // new Extension Version

        if ((matrixSide & TileMatrixSide.Left) > 0) {
          // shift each row to the right and add new content on the left x=0 side
          for (int y = 0; y < Height; y++) {
            var tmp = _mapTiles[Width - 1, y]; // save right element
            for (int x = (int)Width - 1; x > 0; x--) {
              _mapTiles[x, y] = _mapTiles[x - 1, y]; // shift right
              _mapTiles[x, y].UpdateMatrixPixel( x, y );
            }
            ReturnObsoleteTile( tmp );
            // add a new Tile for this extension
            _mapTiles[0, y] = _tileServer.GetTile( );
            _mapTiles[0, y].Configure( ZoomLevel, MapProvider, _extendVersion, new Point( 0, y ), TileMatrix_MapTile_LoadComplete );
            _mapTiles[0, y].TileXYUpdate = Tools.TileXY_DecX( _mapTiles[1, y].TileXY, ZoomLevel );
          }
        }

        if ((matrixSide & TileMatrixSide.Right) > 0) {
          // shift each row to the left and add new content on the right x=Width-1 side
          for (int y = 0; y < Height; y++) {
            var tmp = _mapTiles[0, y];  // save left element
            for (int x = 0; x < Width - 1; x++) {
              _mapTiles[x, y] = _mapTiles[x + 1, y]; // shift left
              _mapTiles[x, y].UpdateMatrixPixel( x, y );
            }
            ReturnObsoleteTile( tmp );
            // add a new Tile for this extension
            _mapTiles[Width - 1, y] = _tileServer.GetTile( );
            _mapTiles[Width - 1, y].Configure( ZoomLevel, MapProvider, _extendVersion, new Point( (int)Width - 1, y ), TileMatrix_MapTile_LoadComplete );
            _mapTiles[Width - 1, y].TileXYUpdate = Tools.TileXY_IncX( _mapTiles[Width - 2, y].TileXY, ZoomLevel );
          }
        }

        // top (check if we had a side shift before and adjust accordingly)
        if ((matrixSide & TileMatrixSide.Top) > 0) {
          // shift each row to the bottom and add new content on the top y=0 side
          for (int x = 0; x < Width; x++) {
            var tmp = _mapTiles[x, Height - 1]; // save bottom element
            for (int y = (int)Height - 1; y > 0; y--) {
              _mapTiles[x, y] = _mapTiles[x, y - 1]; // shift down
              _mapTiles[x, y].UpdateMatrixPixel( x, y );
            }
            ReturnObsoleteTile( tmp );
            // add a new Tile for this extension
            _mapTiles[x, 0] = _tileServer.GetTile( );
            _mapTiles[x, 0].Configure( ZoomLevel, MapProvider, _extendVersion, new Point( x, 0 ), TileMatrix_MapTile_LoadComplete );
            _mapTiles[x, 0].TileXYUpdate = _mapTiles[x, 1].NeedsUpdate ? Tools.TileXY_DecY( _mapTiles[x, 1].TileXYUpdate, ZoomLevel )
                                                                       : Tools.TileXY_DecY( _mapTiles[x, 1].TileXY, ZoomLevel );
          }
        }

        // bottom (check if we had a side shift before and adjust accordingly)
        if ((matrixSide & TileMatrixSide.Bottom) > 0) {
          // shift each row to the top and add new content on the bottom y=Height-1 side
          for (int x = 0; x < Width; x++) {
            var tmp = _mapTiles[x, 0]; // save top element
            for (int y = 0; y < Height - 1; y++) {
              _mapTiles[x, y] = _mapTiles[x, y + 1]; // shift up
              _mapTiles[x, y].UpdateMatrixPixel( x, y );
            }
            ReturnObsoleteTile( tmp );
            // add a new Tile for this extension
            _mapTiles[x, Height - 1] = _tileServer.GetTile( );
            _mapTiles[x, Height - 1].Configure( ZoomLevel, MapProvider, _extendVersion, new Point( x, (int)Height - 1 ), TileMatrix_MapTile_LoadComplete );
            _mapTiles[x, Height - 1].TileXYUpdate = _mapTiles[x, Height - 2].NeedsUpdate ? Tools.TileXY_IncY( _mapTiles[x, Height - 2].TileXYUpdate, ZoomLevel )
                                                                                         : Tools.TileXY_IncY( _mapTiles[x, Height - 2].TileXY, ZoomLevel );
          }
        }

        // create updates
        //UpdateAllTiles( );
        UpdateAllTiles_EX1( );
      } // lock

    }

    // issue UpdateTile Jobs for the Matrix
    private void UpdateAllTiles( )
    {
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          if (_mapTiles[x, y].UpdateTile( x, y, _tileTrackingList )) {
            // Debug.WriteLine( $"UPDATING TILE:[{x}|{y}] with {_mapTiles[x, y].TileXY}" );
            MatrixLoadingStatus = ImageLoadingStatus.Loading;
          }
        }
      }
    }

    private void UpdateAllTiles_EX1( )
    {
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          if (_mapTiles[x, y].NeedsUpdate) {
            if (_mapTiles[x, y].NeedsUpdate) {
              _mapTiles[x, y].UpdateTile_EX1( x, y );
              // Debug.WriteLine( $"UPDATING TILE:[{x}|{y}] with {_mapTiles[x, y].TileXY}" );
              MatrixLoadingStatus = ImageLoadingStatus.Loading;
            }
          }
        }
      }
    }


    // return to stock as obsolete and remove from tracking (in case..)
    private void ReturnObsoleteTile( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      _tileTrackingList.TryRemove( mapTile.TrackKey, out var _ ); // obsolete - remove from tracker
      _tileServer.ReturnObsoleteTile( mapTile ); // markes as obsolete
    }

    /// <summary>
    /// Try again to load failed tiles
    /// </summary>
    public void ReloadFailedTiles( )
    {
      LOG.Info( $"TileMatrix.LoadFailedTiles", $"LoadingStatus= {MatrixLoadingStatus}" );
      // sanity
      if (MatrixLoadingStatus == ImageLoadingStatus.Loading) return; // should not while still loading

      lock (_tileLock) {
        // create updates
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            if (_mapTiles[x, y].IsNotFinished) {
              if (_mapTiles[x, y].CanRetry) {
                LOG.Info( "TileMatrix.LoadFailedTiles", $"Reloading {_mapTiles[x, y].FullKey}" );
                _mapTiles[x, y].TileXYUpdate = _mapTiles[x, y].TileXY; // re-schedule the key
                if (_mapTiles[x, y].NeedsUpdate) {
                  _mapTiles[x, y].UpdateTile( x, y, _tileTrackingList );
                  MatrixLoadingStatus = ImageLoadingStatus.Loading;
                }
              }
            }
          }
        }
      } // lock
    }


    /// <summary>
    /// Try again to load failed tiles
    /// </summary>
    public void ReloadFailedTiles_EX1( )
    {

      // sanity
      if (MatrixLoadingStatus == ImageLoadingStatus.Loading) return; // should not while still loading

      LOG.Info( $"TileMatrix.ReloadFailedTiles_EX1", $"LoadingStatus= {MatrixLoadingStatus}" );
      // create updates
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          if (_mapTiles[x, y].IsNotFinished && _mapTiles[x, y].CanRetry) {
            LOG.Info( "TileMatrix.LoadFailedTiles", $"Reloading {_mapTiles[x, y].FullKey}" );

            _mapTiles[x, y].TileXYUpdate = _mapTiles[x, y].TileXY; // re-schedule the key
            if (_mapTiles[x, y].NeedsUpdate) {
              _mapTiles[x, y].UpdateTile_EX1( x, y );
              MatrixLoadingStatus = ImageLoadingStatus.Loading;
            }
          }
        }
      }
    }

    // called when loading of ONE Tile is fired
    // Failed is true with or without retry recommendation
    // cancelled ones should remove the tracker indpendent of its loading state
    private void TileMatrix_MapTile_LoadComplete( object sender, LoadCompleteEventArgs e )
    {
      if (e.LoadCancelled) {
        // just remove the tracker
        _tileTrackingList.TryRemove( e.TrackKey, out int _ );
      }

      else if (_tileTrackingList.ContainsKey( e.TrackKey )) {
        // handle tracked tiles
        if (e.LoadFailed) {
          LOG.Error( "TileMatrix_LoadComplete", $"LoadFailed for Tile {e.TrackKey}" );
          MatrixLoadingStatus = ImageLoadingStatus.LoadFailed; // temp for this MapTile
          OnLoadComplete( e.TileKey, failed: true, matrixComplete: false ); // report about a failed Tile
        }
        else {
          OnLoadComplete( e.TileKey, failed: false, matrixComplete: false ); // report about a loaded tile
        }

        // manage the tile tracking
        if (_tileTrackingList.TryRemove( e.TrackKey, out int _ )) {
          // Debug.WriteLine( $"TileMatrix_LoadComplete - got Tile {e.TileKey}" );
          ; // Debug Stop
        }
        else {
          // Debug.WriteLine( $"TileMatrix_LoadComplete: Tile {e.TrackKey} was not found (already removed?)" );
          ; // Debug Stop
        }

      }
      else {
        // not cancelled and not tracked - should not ..
        ;
      }

      // check if all tiles are done for the first time
      if ((MatrixLoadingStatus != ImageLoadingStatus.LoadComplete)
        && HasPendingTiles == false) {
        // Debug.WriteLine( $"TileMatrix_LoadComplete (last tile: {e.TileKey})" );
        MatrixLoadingStatus = ImageLoadingStatus.LoadComplete;
        OnLoadComplete( "", failed: false, matrixComplete: true ); // finished and success
#if DEBUG
        // check the image list - should be empty by now
        int count = Service.RequestScheduler.Instance.TileWorkflowCatalog.Count;
        if (count > 0) {
          LOG.Error( "TileMatrix_LoadComplete", $"TileWorkflowCatalog is not empty <{count}>" );
        }
#endif
      }
    }

    // called at an intervall
    private void RequestScheduler_PingEvent( object sender, EventArgs e )
    {
      // check if all tiles are done for the first time
      //if (!HasPendingTiles) {
      if (true) {
        MatrixLoadingStatus = ImageLoadingStatus.LoadComplete;
        OnLoadComplete( "", failed: false, matrixComplete: !HasPendingTiles ); // finished and success
#if DEBUG
        // check the image list - should be empty by now
        int count = Service.RequestScheduler.Instance.TileWorkflowCatalog.Count;
        if (count > 0) {
          LOG.Error( "TileMatrix_LoadComplete", $"TileWorkflowCatalog is not empty <{count}>" );
        }
#endif
      }
    }


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
          Service.RequestScheduler.Instance.PingEvent -= RequestScheduler_PingEvent;

          _tileTrackingList.Clear( );
          for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
              _mapTiles[x, y]?.Dispose( );
            }
          }
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~TileMatrix()
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
