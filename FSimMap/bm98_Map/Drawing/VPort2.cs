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

using CoordLib;
using MapLib;
using MapLib.Tiles;

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
  /// </summary>
  internal class VPort2
  {
    // Relative Zoom Factor per step (and it's inverse)
    private const float c_zoomInMult = 1.2f;
    private const float c_zoomOutMult = 1 / c_zoomInMult;
    // display zoom range
    private const float c_zoomMax = 20f;
    private const float c_zoomMin = 1.0f;

    // output control
    private readonly Control _view = null;

    // The Map (will have a constant size)
    // odd tile numbers are preferred as we get a center Tile
    private readonly TileMatrix _tileMatrix = new TileMatrix( 8, 8 );

    // The drawing canvas (will need the same size as the TileMatrix)
    private Bitmap _canvasStatic = null; // static map and world
    private object _canvasStaticLock = new object( ); // lock for this item
    private Bitmap _canvas = null; // final canvas, i.e. static + sprites

    // scaling applied to the canvas drawing (ratio of matrix height/with vs. drawn width)
    private readonly float c_drawScale = 1f;
    // the canvas pixels that are drawn to the view (scaled by c_drawScale)
    private readonly Size _canvasDrawSize;

    // the Display List Managers
    private GProc _gProc;
    private GProc _gProcSprite;

    // Zoom and Move 
    private Matrix _toZoomMat = new Matrix( );
    private Matrix _fromZoomMat = new Matrix( ); // Inverse of the _toZoomMat (precalculated)
    private Matrix _moveMat = new Matrix( );
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

    /*
    /// <summary>
    /// Fired when the center of the map has changed
    /// </summary>
    internal event EventHandler MapCenterChanged;
    private void OnMapCenterChanged( )
    {
      MapCenterChanged?.Invoke( this, new EventArgs( ) );
    }
    */

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
        Debug.WriteLine( $"VPort2.OnLoadComplete: NO EVENT RECEIVERS HAVE REGISTERED" );
      LoadComplete?.Invoke( this, eventArgs );
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
    /// Returns the GProc for Sprites
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
      SetZoomFactor( float.NaN ); // reset
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
    /// Zoom Into the Image
    /// </summary>
    public void ZoomIn( )
    {
      ZoomIn_low( _view.ClientSize.Center( ) );
    }
    /// <summary>
    /// Zoom Out of the Image
    /// </summary>
    public void ZoomOut( )
    {
      ZoomOut_low( _view.ClientSize.Center( ) );
    }
    /// <summary>
    /// Zoom to 1:1
    /// </summary>
    public void ZoomNorm( )
    {
      ZoomNorm_low( );
    }

    /// <summary>
    /// Redraw the View Control 
    /// </summary>
    public void Redraw( )
    {
      if (_view.Visible) {
        // paints the Canvas
        _view.Refresh( );
      }
    }


    /// <summary>
    /// Renders the static drawings on the base Canvas
    /// Needs to be done when the Map or decorations changes
    /// </summary>
    public void RenderStatic( )
    {
      //      Debug.WriteLine( $"VPort2.RenderStatic" );
      lock (_canvasStaticLock) {
        using (var g = Graphics.FromImage( _canvasStatic )) {
          GProc.Paint( g, MapToCanvasPixel );
        }
      }
      // update the sprites an complete the canvas
      RenderSprite( );
    }

    /// <summary>
    /// Renders the dynamic drawings on the Canvas
    /// Needs to be done when a sprite is updated
    /// </summary>
    public void RenderSprite( )
    {
      //      Debug.WriteLine( $"VPort2.RenderSprite" );

      lock (_canvas) {
        using (var g = Graphics.FromImage( _canvas )) {
          // base is the static canvas
          lock (_canvasStaticLock) {
            g.DrawImageUnscaled( _canvasStatic, 0, 0 );
          }
          // on top the sprites (aircraft)
          GProcSprite.Paint( g, MapToCanvasPixel );
        }
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
    public VPort2( Control view )
    {
      // prep the control to paint to
      _view = view;
      _view.ClientSizeChanged += _view_ClientSizeChanged;
      _view.Paint += _view_Paint;
      _view.MouseDown += _view_MouseDown;
      _view.MouseUp += _view_MouseUp;
      _view.MouseMove += _view_MouseMove;
      _view.MouseWheel += _view_MouseWheel;

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
      _toZoomMat.Reset( );
      _fromZoomMat.Reset( );
      _moveMat.Reset( );

      // create the Graphic processors
      _gProc = new GProc( );
      _gProcSprite = new GProc( );

      // Set the drawing precalcs for the current VP size
      RecalcViewportScaling( );
    }

    // fired by the matrix when a tile was loaded
    private void _tileMatrix_LoadComplete( object sender, LoadCompleteEventArgs e )
    {
      //    Debug.WriteLine( $"{DateTime.Now.Ticks} VPort2._tileMatrix_LoadComplete- MatComplete: {e.MatrixComplete}  LoadFailed: {e.LoadFailed}" );

      if (e.MatrixComplete) {
        // complete but may have failed tiles
        if (_tileMatrix.HasFailedTiles) {
          Debug.WriteLine( $"VPort2._tileMatrix_LoadComplete:  HasFaileTiles - about to reload failed ones" );
          _tileMatrix.LoadFailedTiles( );
        }
        // need to render the map with new content
        RenderStatic( );
      }
      else if (e.LoadFailed) {
        // some tiles have not been loaded
        Debug.WriteLine( $"VPort2._tileMatrix_LoadComplete:  LoadFailed" );
      }
      // propagate event
      OnLoadComplete( e );
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
      return _tileMatrix.MapToMatrixPixel( coordPoint ); // matrix and ccanvas are the same dimension
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
    /// set the Matrix for Move (ONLY HERE) 
    /// Check, correct and Set the Move (Translation) Bounds
    /// Extends the canvas as needed
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
      zoomBounds.Multiply( _fromZoomMat.Elements[0] - 1f ); // Min LeftTop sp (Move Point)

      TileMatrixSide extending = TileMatrixSide.None;

      // when extending to a side the origin moves one tile to this side
      // 0/0 moves by the pixel width of one tile scaled to the current view
      // bounds check is either 0/0 or the drawn matrix width (which is one tile less than the width)

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

      // set the corrected Translation (absolute Viewport pixels)
      _moveMat.Reset( );
      // _canvas translation (_moveMat will be used by other methods too)
      _moveMat.Translate( txPoint.X, txPoint.Y );
      // combine the Move and Zoom into _transformMat used by Paint
      _transformMat = _moveMat.Clone( );
      _transformMat.Multiply( _toZoomMat, MatrixOrder.Append );

      // Extend if needed
      if (extending != TileMatrixSide.None) {
        Map.ExtendMatrix( extending );
        RenderStatic( );
      }
    }

    /// <summary>
    /// Set the Matrices for Zoom (ONLY HERE)
    /// Reset Zoom with NaN
    /// </summary>
    /// <param name="zFactor">Zoom factor is multiplied with the current Zoom (relative factor)</param>
    private void SetZoomFactor( float zFactor )
    {
      if (float.IsNaN( zFactor )) {
        // reset zoom
        _toZoomMat.Reset( );
        _fromZoomMat.Reset( );
      }
      else {
        _toZoomMat.Scale( zFactor, zFactor, MatrixOrder.Append );
        _fromZoomMat = _toZoomMat.Clone( );
        _fromZoomMat.Invert( );
      }
      // combine the Move and Zoom into _transformMat used by Paint
      _transformMat = _moveMat.Clone( );
      _transformMat.Multiply( _toZoomMat, MatrixOrder.Append );
    }


    /// <summary>
    /// Caclulate the actual center coordinate of the map in the Viewport
    /// </summary>
    /// <returns>Current Viewport center LatLon</returns>
    private LatLon ViewportCenterCoord( )
    {
      // Map left top at zoom
      PointF[] c = new PointF[1] { new PointF( -_moveMat.OffsetX, -_moveMat.OffsetY ) };
      // get the Map left top to zoomed coords
      _toZoomMat.TransformPoints( c );
      c[0].Multiply( 1f / _drawFactor ); // return from VP scaling

      // Viewport center in VDrawPixel
      var refPoint = new Point( _view.ClientRectangle.Width / 2, _view.ClientRectangle.Height / 2 );
      refPoint.Multiply( 1f / _drawFactor ); // return from VP scaling
      // join origin and refPt on the Matrix
      c[0] = PointF.Add( c[0], refPoint.ToSizeF( ) );
      // transform to unzoomed pixels
      _fromZoomMat.TransformPoints( c );

      // convert from a MatrixPoint to LatLon
      return _tileMatrix.MatrixPixelToMap( Point.Round( c[0] ) );
    }

    // map a matrix point to the VPort Pixel Point
    private Point MatrixPixelToVPixel( Point matrixPoint )
    {
      PointF[] c = new PointF[1] { matrixPoint };
      _fromZoomMat.TransformPoints( c ); // get pixels
      return Point.Round( c[0] );
    }

    #endregion

    #region Zoom Ops


    /// <summary>
    /// recalculates the Translation of the canvas in order to maintain the refPoint in the same spot in the View
    /// to reset Zoom set zFactor to NaN, else the Zoom factor is multiplied with the current Zoom (relative factor)
    /// </summary>
    /// <param name="refPoint">The point in the View which must have the same xy after zoom is applied</param>
    /// <param name="zFactor">Zoom factor to apply to the current zoom</param>
    private void CalcNewTranslation( Point refPoint, float zFactor )
    {

      // Map left top at zoom
      PointF[] c = new PointF[1] { new PointF( -_moveMat.OffsetX, -_moveMat.OffsetY ) };
      // get the Map left top to zoomed coords
      _toZoomMat.TransformPoints( c ); // zoomed coords
      // vp scaling is ignored here as it cancels out 
      // join origin and refPt on the Matrix
      c[0] = PointF.Add( c[0], refPoint.ToSizeF( ) );
      // transform to unzoomed pixels
      _fromZoomMat.TransformPoints( c );

      // new Zoom
      SetZoomFactor( zFactor );

      // back to zoomed with new Zoom
      _toZoomMat.TransformPoints( c );
      // find the new map origin by subtracting the refPoint
      c[0] = PointF.Subtract( c[0], refPoint.ToSizeF( ) );
      // transform to unzoomed pixels
      _fromZoomMat.TransformPoints( c );// back to pixel coords
      // apply the shift to the drawing
      CheckAndSetCanvasOrigin( new PointF( -c[0].X, -c[0].Y ) );
    }



    // Zoom Into the Image, maintaining the zoom center at refPoint in the View control
    private void ZoomIn_low( Point refPoint )
    {
      if (_toZoomMat.Elements[0] > c_zoomMax) return;
      CalcNewTranslation( refPoint, c_zoomInMult );
      Redraw( );
    }

    // Zoom Out of the Image, maintaining the zoom center at refPoint in the View control
    private void ZoomOut_low( Point refPoint )
    {
      if (_toZoomMat.Elements[0] <= c_zoomMin) {
        CalcNewTranslation( refPoint, float.NaN ); // reset to exactly 1..
      }
      else {
        CalcNewTranslation( refPoint, c_zoomOutMult ); // apply the zoom out
      }
      // Zooming out may leave black borders - so correct for this
      // CheckAndSetMove( new PointF( _moveMat.OffsetX, _moveMat.OffsetY ) );
      Redraw( );
    }

    // Zoom to 1:1, and center in View 
    private void ZoomNorm_low( )
    {
      // set the translation to maintain the refPoint at zoom 1
      CalcNewTranslation( _view.ClientSize.Center( ), float.NaN );
      // get the center LatLon
      var center = ViewportCenterCoord( );
      if (center.IsEmpty) {
        // matrix may not be initialzed at this stage, set default position and return
        CheckAndSetCanvasOrigin( new PointF( 0, 0 ) );
        Redraw( );
        return;
      }
      // calc zooms and final Transformation at 1
      SetZoomFactor( float.NaN );
      // recalc translation to maintain the center coords
      var matrixPixLL = _tileMatrix.MapToMatrixPixel( center );
      var vpPixLL = MatrixPixelToVPixel( matrixPixLL );
      vpPixLL.Multiply( _drawFactor ); // scale to the current client viewport size
      Point shift = new Point( (int)(_view.ClientRectangle.Width / 2) - vpPixLL.X, (int)(_view.ClientRectangle.Height / 2) - vpPixLL.Y );
      CheckAndSetCanvasOrigin( shift );
      Redraw( );
    }

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

    // Mouse Wheel capture
    private void _view_MouseWheel( object sender, MouseEventArgs e )
    {
      // activate the form if the App is not active so at least the most scroll goes only to the App
      ActivateForm( e );

      if (e.Delta < 0) {
        ZoomOut_low( e.Location );
      }
      else {
        ZoomIn_low( e.Location );
      }
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
        _fromZoomMat.TransformPoints( delta ); // to pixel coords
        // final move, join the move delta and where it started
        CheckAndSetCanvasOrigin( PointF.Add( delta[0], _mapStart.ToSizeF( ) ) );
        // and show it
        Redraw( );
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

    // fired when the control wants to be redrawn
    private void _view_Paint( object sender, PaintEventArgs e )
    {
      // protect the outside from our doing..
      var save = e.Graphics.BeginContainer( );
      {
        // combined Move and Zoom Transformation
        e.Graphics.Transform = _transformMat;
        // Apply scale up for the final Canvas drawing (drawing 7 from 8 tiles to handle partial tiles on borders)
        e.Graphics.ScaleTransform( c_drawScale, c_drawScale, MatrixOrder.Prepend );

        lock (_canvas) {
          // draws the final canvas at zoom and move to the view control
          e.Graphics.DrawImage( _canvas, _view.ClientRectangle.Location.X, _view.ClientRectangle.Location.Y, _drawWidth, _drawWidth );
        }
      }
      e.Graphics.EndContainer( save );
#if DEBUG
      // red center cross
      e.Graphics.DrawLine( Pens.Red, _view.Width / 2 - 10, _view.Height / 2, _view.Width / 2 + 10, _view.Height / 2 );
      e.Graphics.DrawLine( Pens.Red, _view.Width / 2, _view.Height / 2 - 10, _view.Width / 2, _view.Height / 2 + 10 );
#endif
    }

    // fires when the output View size has changed
    private void _view_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcViewportScaling( );
      // changing client size needs recalc and render
      CheckAndSetCanvasOrigin( new PointF( _moveMat.OffsetX, _moveMat.OffsetY ) );
      // show it
      Redraw( );
    }

    #endregion


  }
}
