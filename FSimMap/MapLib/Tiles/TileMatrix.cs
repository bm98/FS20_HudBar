using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XMath;
using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.LLShapes;
using System.ComponentModel;
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
    // 2d Array of MapTiles [X]|[Y] Root = Left/Top orientation
    private MapTile[,] _mapTiles;
    // track scheduled Tiles
    private TrackingCat _tileTrackingList = new TrackingCat( );
    // lock while updating many
    private object _tileLock = new object( );
    // tracks the extension to apply a unique tile ID
    private int _extendVersion = 0;
    // A tile server
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
    // a non empty Key triggers  MatComplete=false => a single tile was loaded (or failed)
    // Failed is required to be set
    private void OnLoadComplete( string key, bool failed )
    {
      //     Debug.WriteLine( $"{DateTime.Now.Ticks} TileMatrix.OnLoadComplete- Key: <{key}> LoadFailed: {failed} MatComplete: {string.IsNullOrEmpty( key )}" );

      if (LoadComplete == null) {
        Debug.WriteLine( $"TileMatrix.OnLoadComplete: NO EVENT RECEIVERS HAVE REGISTERED" );
      }
      LoadComplete?.Invoke( this, new LoadCompleteEventArgs( key, "dummy", failed, string.IsNullOrEmpty( key ) ) );
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
    public ImageLoadingStatus LoadingStatus { get; private set; } = ImageLoadingStatus.Unknown;

    /// <summary>
    /// True if there are failed Tiles in the Matrix
    /// </summary>
    public bool HasFailedTiles {
      get {
        bool failed = false;
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            failed |= _mapTiles[x, y].LoadingStatus != ImageLoadingStatus.LoadComplete;
          }
        }
        return failed;
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
    public int MatrixWidth_pixel => _mapTiles[0, 0].TileSize_pixel.Width * (int)Width;
    /// <summary>
    /// Returns the Screen Pixel Height of the Matrix
    /// </summary>
    public int MatrixHeight_pixel => _mapTiles[0, 0].TileSize_pixel.Width * (int)Height;
    /// <summary>
    /// Returns the Screen Pixel Dimension of the Matrix
    /// </summary>
    public Size MatrixSize_pixel => new Size( MatrixWidth_pixel, MatrixHeight_pixel );

    /// <summary>
    /// Returns the Screen Pixel Width of a Tile
    /// </summary>
    public int TileWidth_pixel => _mapTiles[0, 0].TileSize_pixel.Width;
    /// <summary>
    /// Returns the Screen Pixel Height of a Tile
    /// </summary>
    public int TileHeight_pixel => _mapTiles[0, 0].TileSize_pixel.Height;
    /// <summary>
    /// Returns the Screen Pixel Dimension of a Tile
    /// </summary>
    public Size TileSize_pixel => _mapTiles[0, 0].TileSize_pixel;


    /// <summary>
    /// Get: Horizontal length of one Tile Pixel in meters
    /// </summary>
    public float HorPixelMeasure_m => _mapTiles[0, 0].HorPixelMeasure_m;
    /// <summary>
    /// Get: Vertical length of one Tile Pixel in meters
    /// </summary>
    public float VertPixelMeasure_m => _mapTiles[0, 0].VertPixelMeasure_m;
    /// <summary>
    /// Get: Dimenstion of one Tile Pixel in meters
    /// </summary>
    public SizeF TilePixelMeasure_m => _mapTiles[0, 0].TilePixelMeasure_m;


    /// <summary>
    /// Get: Horizontal length of one Tile in meters
    /// </summary>
    public float HorTileMeasure_m => _mapTiles[0, 0].HorTileMeasure_m;
    /// <summary>
    /// Get: Vertical length of one Tile in meters
    /// </summary>
    public float VertTileMeasure_m => _mapTiles[0, 0].VertTileMeasure_m;
    /// <summary>
    /// Get: Dimenstion of the Tile in meters
    /// </summary>
    public SizeF TileMeasure_m => _mapTiles[0, 0].TileMeasure_m;


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
    public MapPixel LeftTop_mapPixel => _mapTiles[0, 0].TileXY.LeftTopMapPixel;
    /// <summary>
    /// Get: Returns the MapPixel of the right top Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel RightTop_mapPixel => _mapTiles[Width - 1, 0].TileXY.RightTopMapPixel;
    /// <summary>
    /// Get: Returns the MapPixel of the left bottom Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel LeftBottom_mapPixel => _mapTiles[0, Height - 1].TileXY.LeftBottomMapPixel;
    /// <summary>
    /// Get: Returns the MapPixel of the right bottom Matrix pixel
    /// (or -1/-1 if the Projection is not yet available)
    /// </summary>
    public MapPixel RightBottom_mapPixel => _mapTiles[Width - 1, Height - 1].TileXY.RightBottomMapPixel;

    /// <summary>
    /// Get: Returns the coordinate of the top left Matrix pixel
    /// </summary>
    public LatLon LeftTop_coord => _mapTiles[0, 0].LeftTop_coord;
    /// <summary>
    /// Get: Returns the coordinate of the top right Matrix pixel
    /// </summary>
    public LatLon RightTop_coord => _mapTiles[Width - 1, 0].RightTop_coord;
    /// <summary>
    /// Get: Returns the coordinate of the bottom left Matrix pixel
    /// </summary>
    public LatLon LeftBottom_coord => _mapTiles[0, Height - 1].LeftBottom_coord;
    /// <summary>
    /// Get: Returns the coordinate of the bottom right Matrix pixel
    /// </summary>
    public LatLon RightBottom_coord => _mapTiles[Width - 1, Height - 1].RightBottom_coord;

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

      _mapTiles = new MapTile[width, height];
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          _mapTiles[x, y] = _tileServer.GetTile( ); // these are internal x/y s not TileXY !!
        }
      }
      LoadingStatus = ImageLoadingStatus.Idle; // ready to load something
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

      return _mapTiles[matrixXY.X, matrixXY.Y];
    }

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
            var drawTile = this.GetTile( new Point( x, y ) );
            if (drawTile != null) {
              drawTile.Draw( g, drawXy, drawTileBorder );
            }
            else {
              // leave empty
            }
          }
        }
      }
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
            var drawTile = this.GetTile( new Point( x, y ) );
            if (drawTile != null) {
              drawTile.DrawToSurface( bitmap, drawXy, drawTileBorder );
            }
            else {
              // leave empty
            }
          }
        }
      }
      return bitmap;
    }

    /// <summary>
    /// Start Loading the Matrix with Tiles around the Center Coordinate
    /// 
    /// </summary>
    /// <param name="coordOnCenterTile"></param>
    /// <param name="zoomLevel">The desired Map Zoom level</param>
    /// <param name="provider">The Provider to get the Map from</param>
    public void LoadMatrix( LatLon coordOnCenterTile, ushort zoomLevel, MapProvider provider )
    {
      if (provider == MapProvider.DummyProvider) return; // nope..
      if (LoadingStatus == ImageLoadingStatus.Loading) return; // TODO Cancel Loading if already loading

      ClearMatrix( );
      LoadingStatus = ImageLoadingStatus.Idle;
      ZoomLevel = zoomLevel;
      MapProvider = provider;
      MapPixelBounds = new Rectangle( new Point( 0, 0 ), Projection.MapPixelSize( zoomLevel ) );

      // as we have one tile overflow to move the matrix the center will be offset to the left top
      // i.e. assume the matrix is one element less in size
      var dWidth = Width - 1;
      var dHeight = Height - 1;

      // start with the 'center' tile - it will be offset to get the TopLeft tile to start loading
      // the quadrant dictates to which side to extend the tiles more than the other for Even Dimensions
      TileXY tlTileXY = TileXY.LatLonToTileXY( coordOnCenterTile, ZoomLevel );
      var quadrant = TileXY.QuadrantFromLatLon( coordOnCenterTile, ZoomLevel );

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
        Debug.WriteLine( $"LoadMatrix: Error - invalid start TileXY ({tlTileXY})" );
        throw new ArgumentOutOfRangeException( $"Input creates invalid start TileXY ({tlTileXY})" );
      }
      // Start Loading
      for (int x = 0, tx = tlTileXY.X; x < Width; x++, tx++) { // left to right
        for (int y = 0, ty = tlTileXY.Y; y < Height; y++, ty++) { // top to bottom
          var tileXY = new TileXY( tx, ty );
          tileXY.Wrap( zoomLevel ); // in case we are at the edge of the map
          // start loading
          //         Debug.WriteLine( $"LoadMatrix: Load triggered for: {Tools.ToFullKey( new MapImageID( tileXY, ZoomLevel, MapProvider ) )}" );
          _mapTiles[x, y].Configure( zoomLevel, MapProvider, 0, TileMatrix_LoadTileComplete );
          _mapTiles[x, y].LoadTile( tileXY, _tileTrackingList );

        }
      }
      LoadingStatus = ImageLoadingStatus.Loading;
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
        _extendVersion++; // new Extension Version

        if ((matrixSide & TileMatrixSide.Left) > 0) {
          // shift each row to the right and add new content on the left x=0 side
          for (int y = 0; y < Height; y++) {
            var tmp = _mapTiles[Width - 1, y]; // save right element
            for (int x = (int)Width - 1; x > 0; x--) {
              _mapTiles[x, y] = _mapTiles[x - 1, y]; // shift right
            }
            HandleObsoleteTiles( tmp );
            // add a new Tile for this extension
            _mapTiles[0, y] = _tileServer.GetTile( );
            _mapTiles[0, y].Configure( ZoomLevel, MapProvider, _extendVersion, TileMatrix_LoadTileComplete );
            _mapTiles[0, y].TileXYUpdate = Tools.TileXY_DecX( _mapTiles[1, y].TileXY, ZoomLevel );
          }
        }

        if ((matrixSide & TileMatrixSide.Right) > 0) {
          // shift each row to the left and add new content on the right x=Width-1 side
          for (int y = 0; y < Height; y++) {
            var tmp = _mapTiles[0, y];  // save left element
            for (int x = 0; x < Width - 1; x++) {
              _mapTiles[x, y] = _mapTiles[x + 1, y]; // shift left
            }
            HandleObsoleteTiles( tmp );
            // add a new Tile for this extension
            _mapTiles[Width - 1, y] = _tileServer.GetTile( );
            _mapTiles[Width - 1, y].Configure( ZoomLevel, MapProvider, _extendVersion, TileMatrix_LoadTileComplete );
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
            }
            HandleObsoleteTiles( tmp );
            // add a new Tile for this extension
            _mapTiles[x, 0] = _tileServer.GetTile( );
            _mapTiles[x, 0].Configure( ZoomLevel, MapProvider, _extendVersion, TileMatrix_LoadTileComplete );
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
            }
            HandleObsoleteTiles( tmp );
            // add a new Tile for this extension
            _mapTiles[x, Height - 1] = _tileServer.GetTile( );
            _mapTiles[x, Height - 1].Configure( ZoomLevel, MapProvider, _extendVersion, TileMatrix_LoadTileComplete );
            _mapTiles[x, Height - 1].TileXYUpdate = _mapTiles[x, Height - 2].NeedsUpdate ? Tools.TileXY_IncY( _mapTiles[x, Height - 2].TileXYUpdate, ZoomLevel )
                                                                                         : Tools.TileXY_IncY( _mapTiles[x, Height - 2].TileXY, ZoomLevel );
          }
        }

        // create updates
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            if (_mapTiles[x, y].UpdateTile( _tileTrackingList )) {
              // Debug.WriteLine( $"UPDATING TILE:[{x}|{y}] with {_mapTiles[x, y].TileXY}" );
              LoadingStatus = ImageLoadingStatus.Loading;
            }
          }
        }
      }//lock
    }


    // clean non loading obsoletes, loading ones will be removed after Loaded
    private void HandleObsoleteTiles( MapTile mapTile )
    {
      // sanity
      if (mapTile == null) return;

      if ((mapTile.Version < _extendVersion) && (mapTile.LoadingStatus == ImageLoadingStatus.Loading)) {
        // is from a previous extension and in the process of updating but still obsolete by now..
        // needs to stay alive until loading has finished and the callback arrived
        // it is no longer part of the tracked image tiles
        _tileTrackingList.TryRemove( mapTile.TrackKey, out var _ ); // obsolete - remove from tracker
        _tileServer.ReturnObsoleteTile( mapTile );
      }
      else {
        // from this extension or not longer loading - it will be just disposed
        _tileServer.ReturnTile( mapTile );
      }
    }

    /// <summary>
    /// Try again to load failed tiles
    /// </summary>
    public void LoadFailedTiles( )
    {
      Debug.WriteLine( $"TileMatrix.LoadFailedTiles: LoadingStatus= {LoadingStatus}" );
      // sanity
      if (LoadingStatus == ImageLoadingStatus.Loading) return;

      lock (_tileLock) {
        // create updates
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            if (_mapTiles[x, y].LoadingStatus != ImageLoadingStatus.LoadComplete) {
              if (_mapTiles[x, y].MapImage.ShouldRetry) {
                Debug.WriteLine( $"TileMatrix.LoadFailedTiles: Reloading {_mapTiles[x, y].FullKey}" );
                _mapTiles[x, y].TileXYUpdate = _mapTiles[x, y].TileXY; // re-schedule the key
                if (_mapTiles[x, y].UpdateTile( _tileTrackingList )) {
                  LoadingStatus = ImageLoadingStatus.Loading;
                }
              }
            }
          }
        }
      }
    }

    // called when loading of ONE Tile is fired
    private void TileMatrix_LoadTileComplete( object sender, LoadCompleteEventArgs e )
    {
      if (_tileTrackingList.ContainsKey( e.TrackKey )) {

        // signal only for tracked tiles
        if (e.LoadFailed) {
          Debug.WriteLine( $"TileMatrix_LoadComplete: ERROR - LoadFailed for Tile {e.TrackKey}" );
          LoadingStatus = ImageLoadingStatus.LoadFailed;
          OnLoadComplete( e.TileKey, true ); // report about a failed Tile
        }
        else {
          OnLoadComplete( e.TileKey, false ); // report about a loaded tile
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

        // check if all tiles are done
        if (_tileTrackingList.IsEmpty) {
          // Debug.WriteLine( $"TileMatrix_LoadComplete (last tile: {e.TileKey})" );
          LoadingStatus = ImageLoadingStatus.LoadComplete;
          OnLoadComplete( "", false ); // finished and success
        }
      }

      // handle obsoletes after the the loading cycle
      _tileServer.RemoveObsoleteTile( e.TrackKey );
    }


    // Clears and resets the Tile Contents - don't call while Loading...
    private void ClearMatrix( )
    {
      // remove all from tracking and clear the obsoletes
      lock (_tileLock) {
        for (int x = 0; x < Width; x++) {
          for (int y = 0; y < Height; y++) {
            _mapTiles[x, y].ClearTileContent( );
          }
        }
        LoadingStatus = ImageLoadingStatus.Idle;
        _tileTrackingList.Clear( );
      }
      _tileServer.ClearObsoleteTiles( );
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
