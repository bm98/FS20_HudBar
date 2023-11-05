using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.XPoint;
using static dNetBm98.XSize;
using CoordLib;
using MapLib;
using MapLib.Tiles;
using static System.Windows.Forms.AxHost;
using System.Threading;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Collections.Concurrent;
using DbgLib;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// A Viewport Manager
  ///  Needs:
  ///    an Output Control (View)
  ///    
  ///  Owns:
  ///    The Map (TileMatrix)
  ///    The Graphic Processor (GProc)
  ///    The Paint event for the View
  ///    Mouse Event Handling for the View
  ///    Zoom and Move
  ///    
  ///  All Drawing is on the Canvas which has the size of the Map Image
  ///  in View.Paint the Canvas is drawn to the View with Scaling and Translation (Zoom and Move)
  ///  
  /// </summary>
  internal class VPort2 : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Relative Scale Factor per step (and it's inverse)
    // scaleMult provides a sensible 3 step scaling between the tilesets (*2 scaled)
    private const float c_scaleInMult = 1.202f;// 1.202^1=1.202; 1.202^2=1.444804; 1.202^3=1.736654408; 1.202^4=2.087458598416 
    private const float c_scaleOutMult = 1 / c_scaleInMult;
    // display scale range
    private const float c_scaleMax = 20f;
    // *2 would be the change over to a new tile set
    private const float c_scaleAutoRange = 1;//use MapZoom as step // NOT USED  1.736654408f;// c_scaleInMult^3  as the decorations zoom as well, gives a weird impression..
    private const float c_scaleMin = 1.0f; // below we use Tiles from the Range above the current one
    // output control
    private readonly Control _view = null;

    // The Map (will have a constant size)
    // odd tile numbers are preferred as we get a center Tile
    private readonly TileMatrix _tileMatrix = new TileMatrix( 8, 8 );

    // The drawing canvas (will need the same size as the TileMatrix)
    private readonly Bitmap _canvasStatic = null; // static map and world
    private readonly Bitmap _canvas = null; // final canvas, i.e. static + sprites

    // scaling applied to the canvas drawing (ratio of matrix height/with vs. drawn width)
    private readonly float c_drawScale = 1f;
    // the canvas pixels that are drawn to the view (scaled by c_drawScale)
    private readonly Size _canvasDrawSize;

    // the Display List Managers
    private readonly GProc _gProc;
    private readonly GProc _gProcSprite;

    // Scale and Move 
    private readonly MapRangeHandler _mapRangeHandlerRef; // the RangeHandler for tile zoom changes

    private float _scaleMult = 1; // current scale factor
    private readonly Matrix _toScaleMat = new Matrix( );
    private Matrix _fromScaleMat = new Matrix( ); // Inverse of the _toScaleMat (precalculated)
    private readonly Matrix _moveMat = new Matrix( );
    // the precalculated transformation Matrix (don't calc at every Paint)
    private Matrix _transformMat = new Matrix( );

    // Ratio of the matrix pixels vs the current client size
    //  (as the matrix is always drawn in a way that the larger side of the View will match the drawn matrix)
    // -> any viewport drawing size = Matrix Size * _drawFactor
    private float _drawFactor = 1f;
    // precalc - the drawn Tilesize (matrix tile size * _drawFactor)
    private Size _drawTileSize = new Size( );
    // precalc - the longer side of the client rectangle
    private int _drawWidth = 0;
    // precalc - undrawn parts when aspect changes
    private int _fallOffRight = 0;
    private int _fallOffBottom = 0;

    // Embedding the calculations and Redraw as an entity
    private readonly AutoResetEvent _vpTransaction = new AutoResetEvent( true );

    // a timer to pace the display when loading takes long
    private long _matLoadStart = 0;



    /* NOT USED
    /// <summary>
    /// Fired when the center of the map has changed
    /// </summary>
    internal event EventHandler MapCenterChanged;
    private void OnMapCenterChanged( )
    {
      MapCenterChanged?.Invoke( this, new EventArgs( ) );
    }
    */

    // Viewport Transaction on Calculate and Redraw the Canvas
    private void EnterTx( ) => _vpTransaction.WaitOne( ); // try without timeout..
    private void LeaveTx( ) => _vpTransaction.Set( );


    #region Viewport API

    /// <summary>
    /// Event triggered on LoadComplete or LoadFailed
    ///  Returns 
    ///     MatrixComplete=true 
    ///     MatrixComplete=false + TileKey + Failed=true  when one Tile issued an error 
    ///  
    /// </summary>
    public event EventHandler<LoadCompleteEventArgs> LoadComplete;

    // Signal the user that data has arrived
    private void OnLoadComplete( LoadCompleteEventArgs eventArgs )
    {
      if (LoadComplete == null)
        LOG.LogError( "VPort2.OnLoadComplete", "NO EVENT RECEIVERS HAVE REGISTERED" );
      LoadComplete?.Invoke( this, eventArgs );
    }

    /// <summary>
    /// Fired when the Map will load tiles
    /// </summary>
    internal event EventHandler MapLoading;
    private void OnMapLoading( )
    {
      _matLoadStart = DateTime.Now.Ticks;
      MapLoading?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// The Map 
    /// </summary>
    public TileMatrix Map => _tileMatrix;

    /// <summary>
    /// Returns the GProc
    /// </summary>
    public GProc GProc => _gProc;
    /// <summary>
    /// Returns the GProc for Sprites which is the Tracked aircraft and it's decoration
    /// </summary>
    public GProc GProcSprite => _gProcSprite;

    /// <summary>
    /// The native size of the map in drawing pixels
    /// </summary>
    public Size NativeMapPixelSize => _tileMatrix.MatrixSize_pixel;

    /// <summary>
    /// Start loading of the map
    /// </summary>
    /// <param name="coordOnCenterTile"></param>
    /// <param name="zoomLevel"></param>
    /// <param name="provider"></param>
    public void LoadMap( LatLon coordOnCenterTile, ushort zoomLevel, MapProvider provider )
    {
      // reset viewport zoom and place the map left top
      SetScaleFactor( c_scaleMin ); // reset
      _tileMatrix.LoadMatrix( coordOnCenterTile, zoomLevel, provider );
      // as the Matrix may have different properties - recalc the draw scaling
      RecalcViewportScaling( );

      var matrixPixLL = _tileMatrix.MapToMatrixPixel( coordOnCenterTile );
      var vpPixLL = MatrixPixelToVPixel( matrixPixLL );
      vpPixLL.Multiply( _drawFactor ); // scale to the current client viewport size
      Point shift = new Point( (int)(_view.ClientRectangle.Width / 2) - vpPixLL.X, (int)(_view.ClientRectangle.Height / 2) - vpPixLL.Y );
      //shift.Offset( _tileMatrix.MatrixWidth_pixel / 2, _tileMatrix.MatrixHeight_pixel / 2 );
      CheckAndSetCanvasOrigin( shift );
    }

    /// <summary>
    /// Move the Map to the Center of the View and make it 1:1
    /// </summary>
    public void CenterMap( )
    {
      ZoomNorm_low( );
    }

    /// <summary>
    /// Returns the LatLon coordinate point which is currently at the center of the Viewport
    /// </summary>
    public LatLon ViewCenterLatLon => ViewportCenterCoord( );

    /// <summary>
    /// Get;Set: wether autorange tile loading is applied or not
    /// </summary>
    public bool AutoRange { get; set; } = false;

    /// <summary>
    /// Zoom Into the Image with scaling the canvas
    /// </summary>
    public void ScaleTo( float scale ) => ScaleTo_low( _view.ClientSize.Center( ), scale );

    /// <summary>
    /// Zoom Into the Image
    /// </summary>
    public void ZoomIn( ) => ZoomIn_low( _view.ClientSize.Center( ) );

    /// <summary>
    /// Zoom Out of the Image
    /// </summary>
    public void ZoomOut( ) => ZoomOut_low( _view.ClientSize.Center( ) );

    /// <summary>
    /// Zoom to 1:1
    /// </summary>
    public void ZoomNorm( ) => ZoomNorm_low( );


    /// <summary>
    /// Redraw the View Control for API users
    /// </summary>
    public void Redraw( )
    {
      if (_mouseDown) return; // not while moving (it's drawn anyway)
      Redraw_low( );
    }


    // Renders the static drawings on the base Canvas
    private void RenderStatic_low( )
    {
      lock (_canvas) {
        // render static parts
        using (var g = Graphics.FromImage( _canvasStatic )) {
          GProc.Paint( g, MapToCanvasPixel );
        }
        // update the sprites an complete the canvas
        using (var g = Graphics.FromImage( _canvas )) {
          // base is the static canvas
          g.DrawImageUnscaled( _canvasStatic, 0, 0 );
          // paint dynamic items to _canvas
          GProcSprite.Paint( g, MapToCanvasPixel );
        }
      }
    }

    // Renders the dynamic drawings on the Canvas
    public void RenderSprite_low( )
    {
      lock (_canvas) {
        using (var g = Graphics.FromImage( _canvas )) {
          // base is the static canvas
          g.DrawImageUnscaled( _canvasStatic, 0, 0 );
          // paint dynamic items to _canvas
          GProcSprite.Paint( g, MapToCanvasPixel );
        }
      }
    }

    /// <summary>
    /// Renders the static drawings on the base Canvas
    /// Needs to be done when the Map or decorations changes
    /// </summary>
    public void RenderStatic( bool threaded = true )
    {
      //      Debug.WriteLine( $"VPort2.RenderStatic" );
      if (threaded) {
        Task.Factory.StartNew( RenderStatic_low );
      }
      else {
        RenderStatic_low( );
      }
    }

    /// <summary>
    /// Renders the dynamic drawings on the Canvas
    /// Needs to be done when a sprite is updated
    /// </summary>
    public void RenderSprite( bool threaded = true )
    {
      //      Debug.WriteLine( $"VPort2.RenderSprite" );
      if (threaded) {
        Task.Factory.StartNew( RenderSprite_low );
      }
      else {
        RenderSprite_low( );
      }
    }


    #endregion

    /// <summary>
    /// cTor: Create a Viewport from a Control to draw to
    /// Manages Events:
    ///   Paint
    ///   Mouse Down,Up,Move,Wheel
    /// </summary>
    /// <param name="view">The Output control</param>
    /// <param name="mapRangeHandler">The MapRangeHandler to use</param>
    public VPort2( Control view, MapRangeHandler mapRangeHandler )
    {
      _mapRangeHandlerRef = mapRangeHandler;
      // prep the control to paint to
      _view = view;
      _view.ClientSizeChanged += _view_ClientSizeChanged;
      _view.Paint += _view_Paint;
      _view.MouseDown += _view_MouseDown;
      _view.MouseUp += _view_MouseUp;
      _view.MouseMove += _view_MouseMove;
      _view.MouseWheel += _view_MouseWheel;

      _view.MouseEnter += _view_MouseEnter;
      _view.MouseLeave += _view_MouseLeave;

      // create the drawing canvas for static maps and final Paint
      _canvasStatic = new Bitmap( Properties.Resources.background, _tileMatrix.MatrixSize_pixel );
      _canvas = new Bitmap( Properties.Resources.background, _tileMatrix.MatrixSize_pixel );
      // precalc the drawing - we draw one tile less to the View to manage moving
      c_drawScale = (float)_tileMatrix.Width / (_tileMatrix.Width - 1);
      _canvasDrawSize = _tileMatrix.MatrixSize_pixel.Multiply( 1f / c_drawScale );

      // prep the TileMatrix
      _tileMatrix.LoadComplete += _tileMatrix_LoadComplete;
      _tileMatrix.LoadMatrix( new LatLon( 0, 0 ), (int)MapRange.Far, MapProvider.DummyProvider ); // INIT EMPTY

      // reset Transform Matrices
      _toScaleMat.Reset( );
      _fromScaleMat.Reset( );
      _moveMat.Reset( );

      // create the Graphic processors
      _gProc = new GProc( );
      _gProcSprite = new GProc( );

      // Processing Refreshes
      StartBGRefresher( _view );

      // Set the drawing precalcs for the current VP size
      RecalcViewportScaling( );
    }

    // fired by the matrix when a tile was loaded
    private void _tileMatrix_LoadComplete( object sender, LoadCompleteEventArgs e )
    {
      //   Debug.WriteLine( $"{DateTime.Now.Ticks} VPort2._tileMatrix_LoadComplete- MatComplete: {e.MatrixComplete}  LoadFailed: {e.LoadFailed}" );

      // can be
      //   Complete       - signals mat completion      (failed or not failed)
      //   NOT Complete   - signals a single tile load  (failed or not failed)

      if (e.MatrixComplete) {
        // complete but may have failed tiles
        if (_tileMatrix.HasFailedTiles) {
         LOG.Log( "VPort2._tileMatrix_LoadComplete","HasFaileTiles - about to reload failed ones" );
          _tileMatrix.LoadFailedTiles( );
        }
        // need to render the map with new content
        RenderStatic( );
        OnLoadComplete( e ); // reported, needs screen refresh
      }
      else {
        // not yet complete
        if (e.LoadFailed) {
          // Single tile load failed
          LOG.Log( "VPort2._tileMatrix_LoadComplete","LoadFailed" );
          OnLoadComplete( e ); // is reported
        }
        else {
          // single tile load success
          // need to render the map with new content after every second to have some feedback to the user
          //  a reply for each tile loaded leads to a very slugish map movement
          var secSinceStart = new TimeSpan( DateTime.Now.Ticks - _matLoadStart ).TotalSeconds;
          if (secSinceStart > 1) {
            _matLoadStart = DateTime.Now.Ticks; // restart
            RenderStatic( );
            OnLoadComplete( e ); // reported, needs screen refresh
          }
        }
      }
    }

    /// <summary>
    /// Function submitted for Drawing
    /// 
    /// Transform a MapCoordinate into Drawing Canvase Coordinates
    /// </summary>
    /// <param name="coordPoint">A Map coordinate</param>
    /// <returns>A Drawing Canvas Point</returns>
    public Point MapToCanvasPixel( LatLon coordPoint )
    {
      if (!coordPoint.IsEmpty) {
        return _tileMatrix.MapToMatrixPixel( coordPoint ); // matrix and ccanvas are the same dimension
      }
      else {
        return new Point( -999, -999 );
      }
    }

    /// <summary>
    /// Local redraw the View Control 
    /// </summary>
    private void Redraw_low( )
    {
      AddPaint( );
    }

    /// <summary>
    /// As for painting the View Control 
    /// </summary>
    private void Refresh_low( )
    {
      if (_view.Visible) {
        // paints the Canvas ONLY HERE!!
        _view.Refresh( );
      }
    }

    /// <summary>
    /// recalculate the ratio of the canvas (matrix) and current client viewport size
    ///  (as the matrix is always drawn in a way that the larger side of the View will match the drawn matrix)
    /// </summary>
    private void RecalcViewportScaling( )
    {
      // get the longer side of the client rectangle
      _drawWidth = _view.ClientRectangle.Width;
      _drawWidth = (_view.ClientRectangle.Height > _drawWidth) ? _view.ClientRectangle.Height : _drawWidth;
      // the invisible area either horizontal or vertical for non square viewports (the canvas is square)
      _fallOffRight = _view.ClientRectangle.Width - _drawWidth;
      _fallOffBottom = _view.ClientRectangle.Height - _drawWidth;
      // the scaling of the canvas into the viewport rectangle to fill the area
      _drawFactor = (float)_drawWidth / _canvasDrawSize.Width;
      // precalculated tilesize when drawn at current client size
      _drawTileSize = new Size( (int)((float)_tileMatrix.TileWidth_pixel * _drawFactor),
                                 (int)((float)_tileMatrix.TileWidth_pixel * _drawFactor)
                              );
    }

    #region Matrix Ops 

    /// <summary>
    /// Set the Canvas Origin in the Viewport
    /// used to move Canvas (ONLY HERE!!) 
    /// Check, correct and Set the Move (Translation) Bounds
    /// Extends the Matrix as needed to show all tiles
    /// </summary>
    /// <param name="txPoint">Left/top corner of the drawn Canvas in unzoomed pixels</param>
    private void CheckAndSetCanvasOrigin( PointF txPoint )
    {
      /*
      0/0 -> the left top tile will be placed at the left top corner of the Viewport (visible)

      We calculate the needed transformation of the _canvas to have it shown in the View Control
      It will also update the Mouse Move initial points so the dragging is not interrupted while the 
      map needs extension.

      When a tile boundary is crossed by the move the map will be shifted in order to 
      show the newly revealed area - needs also rendering of the newly appeared decorations
       */
      PointF zoomBounds = _view.ClientSize.ToPointF( );
      zoomBounds.Multiply( _fromScaleMat.Elements[0] - 1f ); // Min LeftTop sp (Move Point)

      // when extending to a side the origin moves one tile to this side
      // 0/0 moves by the pixel width of one tile scaled to the current view
      // bounds check is either 0/0 or the drawn matrix width (which is one tile less than the width)

      TileMatrixSide extending = TileMatrixSide.None;

      // move right, left tile bound check
      if (txPoint.X > 0) {
        extending |= TileMatrixSide.Left;
        _mapStart.X -= _drawTileSize.Width;
        txPoint.X -= _drawTileSize.Width;
      }
      // move left, right tile bound check
      else if (txPoint.X < (zoomBounds.X + _fallOffRight - _drawTileSize.Width)) {
        extending |= TileMatrixSide.Right;
        _mapStart.X += _drawTileSize.Width;
        txPoint.X += _drawTileSize.Width;
      }
      // move down, upper tile bound check
      if (txPoint.Y > 0) {
        extending |= TileMatrixSide.Top;
        _mapStart.Y -= _drawTileSize.Height;
        txPoint.Y -= _drawTileSize.Height;
      }
      // move up, lower tile bound check
      else if (txPoint.Y < (zoomBounds.Y + _fallOffBottom - _drawTileSize.Height)) {
        extending |= TileMatrixSide.Bottom;
        _mapStart.Y += _drawTileSize.Height;
        txPoint.Y += _drawTileSize.Height;
      }

      // Extend if needed
      if (extending != TileMatrixSide.None) {
        Map.ExtendMatrix( extending );
      }

      // set the corrected Translation (absolute Viewport pixels)
      _moveMat.Reset( );
      // _canvas translation (_moveMat will be used by other methods too)
      _moveMat.Translate( txPoint.X, txPoint.Y );
      // combine the Move and Zoom into _transformMat used by Paint
      _transformMat = _moveMat.Clone( );
      _transformMat.Multiply( _toScaleMat, MatrixOrder.Append );

      // Extend if needed
      if (extending != TileMatrixSide.None) {
        OnMapLoading( ); // some loading will happen
        RenderStatic( );
      }
    }

    /// <summary>
    /// Set the Matrices for Scaling the canvas (ONLY HERE)
    /// Reset Scale with c_scaleMin or NaN
    /// </summary>
    /// <param name="sFactor">Scale factor </param>
    private void SetScaleFactor( float sFactor )
    {
      // sanity
      sFactor = dNetBm98.XMath.Clip( sFactor, c_scaleMin, c_scaleMax );

      if (float.IsNaN( sFactor ) || sFactor <= (c_scaleMin + float.Epsilon)) {
        // reset zoom
        _toScaleMat.Reset( );
        _fromScaleMat.Reset( );
        _scaleMult = c_scaleMin;
      }
      else {
        _toScaleMat.Reset( );
        _toScaleMat.Scale( sFactor, sFactor, MatrixOrder.Append );
        _scaleMult = _toScaleMat.Elements[0]; // retrieve the exact applied scale

        _fromScaleMat = _toScaleMat.Clone( );
        _fromScaleMat.Invert( );
      }
      // combine the Move and Zoom into _transformMat used by Paint
      _transformMat = _moveMat.Clone( );
      _transformMat.Multiply( _toScaleMat, MatrixOrder.Append );
    }

    /// <summary>
    /// Caclulate the Map Point of a coordinate in the Viewport
    /// </summary>
    /// <returns>Viewport Point for the LatLon</returns>
    private Point ViewportPoint( LatLon coord )
    {
      // Map left top at zoom
      PointF[] c = new PointF[1] { new PointF( -_moveMat.OffsetX, -_moveMat.OffsetY ) };
      // get the Map left top to zoomed coords
      _toScaleMat.TransformPoints( c );
      c[0].Multiply( 1f / _drawFactor ); // return from VP scaling (stretch of the canvas image)
                                         // Point on the Canvas

      // CoordPoint in MatrixPixel
      PointF[] refPoint = new PointF[1] { _tileMatrix.MapToMatrixPixel( coord ) };
      // transform to zoomed pixels
      _toScaleMat.TransformPoints( refPoint );
      // join origin and refPt on the Canvas
      c[0] = PointF.Subtract( c[0], refPoint[0].ToSizeF( ) );

      c[0].Multiply( _drawFactor ); // to VP scaling

      return Point.Round( c[0] );
    }


    /// <summary>
    /// Caclulate the coordinate of the map Point in the Viewport
    /// </summary>
    /// <returns>Viewport LatLon for the point</returns>
    private LatLon ViewportCoord( Point vpPoint )
    {
      // Map left top at zoom
      PointF[] c = new PointF[1] { new PointF( -_moveMat.OffsetX, -_moveMat.OffsetY ) };
      // get the Map left top to a scaled Point
      _toScaleMat.TransformPoints( c );
      c[0].Multiply( 1f / _drawFactor ); // return from VP scaling (stretch of the canvas image)
                                         // Point on the Canvas

      // Viewport Point in VDrawPixel / true Point on the image shown on screen e.g. mouse pointer
      var refPoint = vpPoint;
      refPoint.Multiply( 1f / _drawFactor ); // return from VP scaling (stretch of the canvas image)
                                             // join origin and refPt on the Canvas
      c[0] = PointF.Add( c[0], refPoint.ToSizeF( ) );
      // transform to unscaled pixels (Matrix Pixels)
      _fromScaleMat.TransformPoints( c );

      // convert from a MatrixPoint to LatLon
      return _tileMatrix.MatrixPixelToMap( Point.Round( c[0] ) );
    }

    /// <summary>
    /// Caclulate the actual center coordinate of the map in the Viewport
    /// </summary>
    /// <returns>Current Viewport center LatLon</returns>
    private LatLon ViewportCenterCoord( )
    {
      return ViewportCoord( new Point( _view.ClientRectangle.Width / 2, _view.ClientRectangle.Height / 2 ) );
    }


    // map a matrix point to the VPort Pixel Point
    private Point MatrixPixelToVPixel( Point matrixPoint )
    {
      PointF[] c = new PointF[1] { matrixPoint };
      _fromScaleMat.TransformPoints( c ); // get pixels
      return Point.Round( c[0] );
    }

    #endregion

    #region Canvas Scale Ops

    /// <summary>
    /// recalculates the Translation of the canvas in order to maintain the refPoint in the same spot in the View
    /// </summary>
    /// <param name="refPoint">The point in the View which must have the same xy after zoom is applied</param>
    /// <param name="zFactor">Zoom factor to apply</param>
    private void CalcNewTranslation( Point refPoint, float zFactor )
    {
      // Map left top at zoom
      PointF[] c = new PointF[1] { new PointF( -_moveMat.OffsetX, -_moveMat.OffsetY ) };
      // get the Map left top to zoomed coords
      _toScaleMat.TransformPoints( c ); // zoomed coords
                                        // vp scaling is ignored here as it cancels out 
                                        // join origin and refPt on the Matrix
      c[0] = PointF.Add( c[0], refPoint.ToSizeF( ) );
      // transform to unzoomed pixels
      _fromScaleMat.TransformPoints( c );
      // c[0] is now the refPoint on the Matrix (ex VP draw scale)

      // new Zoom
      SetScaleFactor( zFactor );

      // back to zoomed with new Zoom
      _toScaleMat.TransformPoints( c );
      // find the new map origin by subtracting the refPoint
      c[0] = PointF.Subtract( c[0], refPoint.ToSizeF( ) );
      // transform to unzoomed pixels
      _fromScaleMat.TransformPoints( c );// back to pixel coords
                                         // c[0] is the new MapOrigin in unzoomed Canvas Pixel coordinates
                                         // where the refPoint is at the same screen location as before 

      // apply the new Matrix Origin
      CheckAndSetCanvasOrigin( new PointF( -c[0].X, -c[0].Y ) );
    }


    // Scale the Image exactly, maintaining the scale center at refPoint in the View control
    private void ScaleTo_low( Point refPoint, float zFactor )
    {
      EnterTx( );
      try {
        CalcNewTranslation( refPoint, zFactor );
        Redraw_low( );
      }
      finally { LeaveTx( ); }
    }

    // Zoom Into the Image with scaling, maintaining the center at refPoint in the View control
    private void ZoomIn_low( Point refPoint )
    {
      if (_scaleMult >= c_scaleMax) {
        // at top scaling end
        return; // cannot scale up further 
      }
      else {
        if (AutoRange && (_scaleMult >= c_scaleAutoRange) && _mapRangeHandlerRef.CanIncZoomLevel) {
          // for autoRange trigger zoomLevel change when a new tile set would be needed
          // refPoint before TileChange
          var rLL = ViewportCoord( refPoint );  // @ scale _scaleMult
          if (_mapRangeHandlerRef.IncZoomLevel( )) {
            try {
              var vpPixLL = MatrixPixelToVPixel( _tileMatrix.MapToMatrixPixel( rLL ) );
              vpPixLL.Multiply( _drawFactor ); // scale to the current client viewport size
              Point shift = refPoint.Subtract( vpPixLL );
              EnterTx( );
              CheckAndSetCanvasOrigin( shift );
              // set scale to autoRange (scaled up at IncDec Limit)
              CalcNewTranslation( refPoint, c_scaleMin );
            }
            finally { LeaveTx( ); }
            // Don't redraw here else it shows the current matrix and jumps later when the tiles are loaded
          }
        }
        else {
          // scale up
          EnterTx( );
          try {
            CalcNewTranslation( refPoint, _scaleMult * c_scaleInMult );
            Redraw_low( );
          }
          finally { LeaveTx( ); }
        }
      }
    }

    // Zoom Out of the Imagewith scaling, maintaining the center at refPoint in the View control
    private void ZoomOut_low( Point refPoint )
    {
      if (_scaleMult <= c_scaleMin) {
        // at lower scaling end (smaller or 1:1)
        if (AutoRange && _mapRangeHandlerRef.CanDecZoomLevel) {
          // for autoRange trigger zoomLevel change when a new tile set would be needed
          // refPoint before TileChange
          var rLL = ViewportCoord( refPoint );
          if (_mapRangeHandlerRef.DecZoomLevel( )) {
            EnterTx( );
            try {
              // get where the refLL is now in the zoomed image
              var vpPixLL = MatrixPixelToVPixel( _tileMatrix.MapToMatrixPixel( rLL ) );
              vpPixLL.Multiply( _drawFactor ); // scale to the current client viewport size
                                               // move map to align the refPoint
              Point shift = refPoint.Subtract( vpPixLL );
              CheckAndSetCanvasOrigin( shift ); // may extend the matrix to have it in view
                                                // set scale to autoRange (scaled up at IncDec Limit)
              CalcNewTranslation( refPoint, c_scaleAutoRange );
            }
            finally { LeaveTx( ); }
            // Don't redraw here else it shows the current matrix and jumps later when the tiles are loaded
          }
        }
        else {
          EnterTx( );
          try {
            CalcNewTranslation( refPoint, c_scaleMin ); // reset to base scale
            Redraw_low( );
          }
          finally { LeaveTx( ); }
        }
      }
      else {
        // scale down
        EnterTx( );
        try {
          CalcNewTranslation( refPoint, _scaleMult * c_scaleOutMult );
          Redraw_low( );
        }
        finally { LeaveTx( ); }
      }
    }

    // Zoom to 1:1, and center in View 
    private void ZoomNorm_low( )
    {
      EnterTx( );
      try {
        // set the translation to maintain the refPoint at zoom 1
        CalcNewTranslation( _view.ClientSize.Center( ), c_scaleMin );
        // get the center LatLon
        var center = ViewportCenterCoord( );
        if (center.IsEmpty) {
          // matrix may not be initialized at this stage, set default position and return
          CheckAndSetCanvasOrigin( new PointF( 0, 0 ) );
        }
        else {
          // calc zooms and final Transformation at 1
          SetScaleFactor( c_scaleMin );
          // recalc translation to maintain the center coords
          var vpPixLL = MatrixPixelToVPixel( _tileMatrix.MapToMatrixPixel( center ) );
          vpPixLL.Multiply( _drawFactor ); // scale to the current client viewport size
          Point shift = new Point( (int)(_view.ClientRectangle.Width / 2) - vpPixLL.X, (int)(_view.ClientRectangle.Height / 2) - vpPixLL.Y );
          CheckAndSetCanvasOrigin( shift );
        }
        Redraw_low( );
      }
      finally { LeaveTx( ); }
    }

    /*
    /// <summary>
    /// Activates the Main Form in order to prevent that further (Mouse) events are sent to the prev. Active Application
    /// This is used to switch to the HudBar for Mouse Wheel scrolling.
    /// MSFS however will still capture the first scroll event as it receives the Event in parallel to the Mouse hovered control of the HudBar
    /// But at least then it will stop getting further scroll events and usually Zoom in/out like crazy...
    ///     basically one would be able to send a reverse scroll to Windows before this but it seems rather complicated to do so....
    /// </summary>
    /// <param name="e">The MouseEvents</param>
    internal void ActivateForm( MouseEventArgs e )
    {
      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      //  NOTE: this will not prevent a single scroll event captured by DirectInput i.e. the Sim however...
      if (Form.ActiveForm == null) {
        _view.FindForm( ).Activate( );
      }
      (e as HandledMouseEventArgs).Handled = true; // don't bubble up the scroll wheel
    }
    */


    // Mouse Wheel capture
    private void _view_MouseWheel( object sender, MouseEventArgs e )
    {
      if (e.Delta < 0) {
        ZoomOut_low( e.Location );
      }
      else {
        ZoomIn_low( e.Location );
      }
    }

    private void _view_MouseLeave( object sender, EventArgs e )
    {
      dNetBm98.WinUser.PopForeground( );
    }

    private void _view_MouseEnter( object sender, EventArgs e )
    {
      dNetBm98.WinUser.PushAndSetForeground( _view );
    }

    #endregion

    #region Move Ops

    // mouse states
    private bool _mouseDown = false;
    private PointF _mouseStart = new PointF( );
    private PointF _mapStart = new PointF( );

    // fires when the mouse button is pressed
    private void _view_MouseDown( object sender, MouseEventArgs e )
    {
      if ((e.Button & MouseButtons.Left) > 0) {
        _mouseStart = e.Location;
        _mapStart = new PointF( _moveMat.OffsetX, _moveMat.OffsetY );

        _mouseDown = true;
        _view.Cursor = Cursors.NoMove2D;
      }
    }

    // fires when moving the mouse
    private void _view_MouseMove( object sender, MouseEventArgs e )
    {
      if (((e.Button & MouseButtons.Left) > 0) && _mouseDown) {
        if (!_view.ClientRectangle.Contains( e.Location )) return;
        // cannot use relative movements at Pixel level due to error addition issues 
        // so we calculate the complete move while the mouse is down and apply this to the values we captured when the move started

        // calc delta since Move started (use an array with one element to apply the transformation later)
        PointF[] delta = new PointF[1] { e.Location };
        delta[0] = PointF.Subtract( delta[0], _mouseStart.ToSizeF( ) );
        // back
        EnterTx( );
        try {
          // to pixel coords
          _fromScaleMat.TransformPoints( delta );
          // final move, join the move delta and where it started
          CheckAndSetCanvasOrigin( PointF.Add( delta[0], _mapStart.ToSizeF( ) ) );
          Redraw_low( );
        }
        finally { LeaveTx( ); }
      }
    }

    // fires when the mouse button was released
    private void _view_MouseUp( object sender, MouseEventArgs e )
    {
      if (((e.Button & MouseButtons.Left) > 0) && _mouseDown) {
        _mouseDown = false;
        _view.Cursor = Cursors.Default;
      }
    }

    #endregion

    #region View Events (ex Mouse)

    #region Blending the Canvas

    #region Refresh Thread

    // blending requires to have a number of paints to make the 
    // image fully opaque, usually this would be the case if things
    // change often and many refreshes are triggered 
    // for cases where e.g. decoration is toggled only one refresh is triggered
    // so adding multiple refreshes to trigger all needed paints is done 
    // via a Repaint algo which adds a number of (internal) refreshes 
    // for each external one
    // internal refreshes will not again load more to eventually get to an end..
    // Repaints are queued and invoked on the main thread via the BG_Refresher thread

    bool _bgContinueProcess = true;
    private Thread _bgRefreshThread;
    private AutoResetEvent _bgSignal;
    // WinForm Invoker
    private dNetBm98.WinFormInvoker _eDispatch;
    // repaint event queue
    private readonly ConcurrentQueue<bool> _paintQueue = new ConcurrentQueue<bool>( );

    // start the blend refesher algo
    private void StartBGRefresher( Control control )
    {
      _imageAttributes.SetColorMatrix( _blendMatrix );
      _bgSignal = new AutoResetEvent( false );
      _eDispatch = new dNetBm98.WinFormInvoker( control );
      _bgRefreshThread = new Thread( BG_Refresher ) { IsBackground = true }; // killed on exit
      _bgContinueProcess = true;
      _bgRefreshThread.Start( );
    }

    // the task routine, issues refreshs while one is queued
    private void BG_Refresher( )
    {
      // can be stopped
      while (_bgContinueProcess) {
        _bgSignal.WaitOne( );
        if (_paintQueue.Count > 0) {
          _eDispatch.HandleEvent( Refresh_low );
          // the item will be removed from the queue in the Paint event
        }
      }
    }

    // add an external paint
    // used when the content changed i.e. Redraw called by API or locally
    private void AddPaint( )
    {
      // empty queue and add a Repaint one
      while (!_paintQueue.IsEmpty) {
        _paintQueue.TryDequeue( out bool _ );
      }
      // add a refresh with repaint request
      _paintQueue.Enqueue( true ); // from the outside, means the image needs blending repaints
      _bgSignal.Set( ); // turn a BG refresh loop when a repaint was added
    }

    #endregion

    // Blending support

    // number of repaints to get the image 99% opaque with regards to the current canvas
    // i.e. end of blending the new one over the current one 
    // the smaller the blending factor the more repaints are needed
    private const int c_numRepaints = 2;
    // main drawn canvas to blend with
    private Bitmap _drawnCanvas = null;
    // just change the alpha
    private readonly ColorMatrix _blendMatrix = new ColorMatrix( new float[][]{
                new float[] {1F, 0, 0, 0, 0},
                new float[] {0, 1F, 0, 0, 0},
                new float[] {0, 0, 1F, 0, 0},
                new float[] {0, 0, 0, 0.85f, 0}, // blending factor
                new float[] {0, 0, 0, 0, 1F}} );
    private readonly ImageAttributes _imageAttributes = new ImageAttributes( );
    // tracks the matrix version i.e. changes of the covered area
    private int _drawnMatVersion = -1;

    // make the drawnCanvas the current one if needed
    private void CheckAndReloadDrawnCanvas( )
    {
      if (_tileMatrix.Version > _drawnMatVersion) {
        // a change in the matrix version means that the previous image is no longer
        // matching the current one and a blend would create a screwed up final image for the user
        _drawnCanvas?.Dispose( );
        _drawnCanvas = (Bitmap)_canvas.Clone( );
        _drawnMatVersion = _tileMatrix.Version;
      }
    }

    // blends the canvas
    private void BlendCanvas( )
    {
      lock (_canvas) {
        CheckAndReloadDrawnCanvas( );
        using (var gPrev = Graphics.FromImage( _drawnCanvas )) {
          gPrev.CompositingMode = CompositingMode.SourceOver;
          gPrev.CompositingQuality = CompositingQuality.HighSpeed;
          gPrev.DrawImage( _canvas,
            new Rectangle( new Point( 0, 0 ), _drawnCanvas.Size ),
            0, 0, _canvas.Width, _canvas.Height,
            GraphicsUnit.Pixel, _imageAttributes ); // add with transparency to the previous canvas / blending
        }
      }
    }

    #endregion


    // fired when the control wants to be redrawn
    private void _view_Paint( object sender, PaintEventArgs e )
    {
      // blend the _canvas onto the _drawnCanvas
      BlendCanvas( );

      // get the repaint order (true is to add repaints, false to just paint)
      var repaint = true;
      if (_paintQueue.TryDequeue( out bool value )) {
        repaint = value;
      }

      // protect the outside from our doing..
      var save = e.Graphics.BeginContainer( );
      {
        // combined Move and Zoom Transformation
        e.Graphics.Transform = _transformMat;
        // Apply scale up for the final Canvas drawing (drawing 7 from 8 tiles to handle partial tiles on borders)
        e.Graphics.ScaleTransform( c_drawScale, c_drawScale, MatrixOrder.Prepend );
        // draws the final canvas at zoom and move to the view control
        e.Graphics.DrawImage( _drawnCanvas, _view.ClientRectangle.Location.X, _view.ClientRectangle.Location.Y, _drawWidth, _drawWidth );
      }
      e.Graphics.EndContainer( save );

      // small red center cross
      e.Graphics.DrawLine( Pens.Red, _view.Width / 2 - 10, _view.Height / 2, _view.Width / 2 + 10, _view.Height / 2 );
      e.Graphics.DrawLine( Pens.Red, _view.Width / 2, _view.Height / 2 - 10, _view.Width / 2, _view.Height / 2 + 10 );

      // need some more Paints to make it fully opaque
      if (repaint) {
        // add while not enough
        while (_paintQueue.Count < c_numRepaints) {
          _paintQueue.Enqueue( false ); // will not trigger more repaints...
        }
      }
      // more paints in the queue - process them
      if (!_paintQueue.IsEmpty) _bgSignal.Set( );
    }

    // fires when the output View size has changed
    private void _view_ClientSizeChanged( object sender, EventArgs e )
    {
      EnterTx( );
      try {
        RecalcViewportScaling( );
        // changing client size needs recalc and render
        CheckAndSetCanvasOrigin( new PointF( _moveMat.OffsetX, _moveMat.OffsetY ) );
        Redraw_low( );
      }
      finally { LeaveTx( ); }
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          _tileMatrix.LoadComplete -= _tileMatrix_LoadComplete;
          _view.Paint -= _view_Paint;
          _view.MouseDown -= _view_MouseDown;
          _view.MouseMove -= _view_MouseMove;
          _view.MouseUp -= _view_MouseUp;
          _view.MouseWheel -= _view_MouseWheel;
          _view.MouseLeave -= _view_MouseLeave;
          _view.MouseEnter -= _view_MouseEnter;

          _gProc.Drawings.Clear( );
          _gProcSprite.Drawings.Clear( );
          // get rid of  the bg thread
          _bgContinueProcess = false;
          _bgSignal?.Set( );
          try {
            _bgRefreshThread?.Abort( );
            _bgRefreshThread?.Join( );
          }
          catch (Exception) { }
          _bgSignal?.Dispose( );

          _tileMatrix?.Dispose( );
          _toScaleMat?.Dispose( );
          _fromScaleMat?.Dispose( );
          _moveMat?.Dispose( );
          _transformMat?.Dispose( );
          _canvasStatic?.Dispose( );
          _canvas?.Dispose( );
          _drawnCanvas?.Dispose( );
          _vpTransaction?.Dispose( );

        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~VPort2()
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
