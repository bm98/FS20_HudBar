using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Album
{
  /// <summary>
  /// View manager for a Zoomable, Moveable image display
  /// </summary>
  internal class ViewportZ
  {

    private Size _imageSize = new Size(0,0);
    private PointF _imageCenter_src = new PointF(0,0); // source Origin (Source Coords)

    private Size _viewport = new Size(0,0);

    // the supported Zoom Magnification factors
    private float[] _zoomLadder = new float[]{0.125f, 0.25f, 0.333f, 0.5f, 0.667f, 1f, 1.5f, 2, 3, 4, 8};
    private const int _zoomNorm = 5; // index to 1.0 in the Array above
    private int _zoom_index = _zoomNorm; // we use and Index into the Zoom Ladder to zoom in and out

    private SizeF _srcSize = new SizeF(0,0);  // current Image Size in Zoomed Units
    private RectangleF _srcRect = new RectangleF(0,0,0,0); // The full Src Rectangle used for DrawImage Ops outside this code

    private Point _dragStart_vp = new Point(0,0); // last DragStarting Point (VP Coords)
    private SizeF _drag_vp = new Size(0,0);    // current Drag Displacement while Draging (VP Coords)


    /// <summary>
    /// The set Image Size (original size in Pixel)
    /// </summary>
    public Size ImageSize => _imageSize;
    /// <summary>
    /// The set Viewport Size
    /// </summary>
    public Size Viewport => _viewport;
    /// <summary>
    /// The current Zoom Level (nominal => 1)
    /// </summary>
    public float ZoomLevel => _zoomLadder[_zoom_index];

    /// <summary>
    /// The Source Rectangle to draw the Image with
    /// </summary>
    public Rectangle SrcRect => Rectangle.Round( _srcRect );


    /// <summary>
    /// Set the Output Viewport Size
    /// </summary>
    /// <param name="size">The Viewport Size</param>
    public void SetViewport( Size size )
    {
      _viewport = size;
      _srcSize = Scale( _viewport, ZoomLevel );
      RecalcSrcRect( );
    }
    /// <summary>
    /// Set the Output Viewport Size
    /// </summary>
    /// <param name="size">The Viewport Size</param>
    public void SetViewport( SizeF size )
    {
      SetViewport( Size.Round( size ) );
    }

    /// <summary>
    /// Set Origial Image Size
    /// </summary>
    /// <param name="size">The Original Image Size</param>
    public void SetImageSize( Size size )
    {
      _imageSize = size; // new image size
      ZoomReset( ); // reset Zoom (does all other calculations)
    }
    /// <summary>
    /// Set Origial Image Size
    /// </summary>
    /// <param name="size">The Original Image Size</param>
    public void SetImageSize( SizeF size )
    {
      SetImageSize( Size.Round( size ) );
    }

    /// <summary>
    /// Zoom one Level Out (shrink shown image)
    /// </summary>
    public void ZoomOut( )
    {
      _zoom_index++;
      _zoom_index = ( _zoom_index < _zoomLadder.Length ) ? _zoom_index : ( _zoomLadder.Length - 1 ); // limit to available levels

      _srcSize = Scale( _viewport, ZoomLevel ); // scale the display area based on the Zoom magnificaion
      RecalcSrcRect( );
    }
    /// <summary>
    /// Zoom one Level In (enlarge shown image)
    /// </summary>
    public void ZoomIn( )
    {
      _zoom_index--;
      _zoom_index = ( _zoom_index < 0 ) ? 0 : _zoom_index; // limit to available levels

      _srcSize = Scale( _viewport, ZoomLevel ); // scale the display area based on the Zoom magnificaion
      RecalcSrcRect( );
    }
    /// <summary>
    /// Reset Zoom and Drag (original Pix per Pix display)
    /// </summary>
    public void ZoomReset( )
    {
      _zoom_index = _zoomNorm; // should get us to ZoomLevel 1.0 ..
      _dragStart_vp = new Point( 0, 0 ); // Reset Drag
      _drag_vp = new Size( 0, 0 ); // Reset Drag Offset

      _imageCenter_src = new PointF( _imageSize.Width / 2f, _imageSize.Height / 2f ); // Reset to Center the image
      _srcSize = Scale( _viewport, ZoomLevel ); // Reset display to 1:1 pixel - ZoomLevel is 1.0 here
      RecalcSrcRect( );
    }

    /// <summary>
    /// Startpoint of a Drag operation 
    /// </summary>
    /// <param name="vpStartPoint">The start point in Viewport coords</param>
    public void DragStart( Point vpStartPoint )
    {
      // save start loc to calc drag offsets while draging the image around
      _dragStart_vp = vpStartPoint; 
    }
    /// <summary>
    /// A new Point while Draging
    /// </summary>
    /// <param name="vpPoint">The point in Viewport coords</param>
    public void Drag( Point vpPoint )
    {
      // Calc the DragOffset (in VP coords)
      _drag_vp = new Size( vpPoint.X - _dragStart_vp.X, vpPoint.Y - _dragStart_vp.Y );

      // calc new image center as displacement from old center and current drag offset (at ZoomLevel in Src Coords)
      //   by subtracting the DragOffset AND the viewport Center Offset
      var srcLoc =  _imageCenter_src - Scale( _drag_vp, ZoomLevel ) - Scale( _viewport, 0.5f * ZoomLevel );
      // modify the Src Location while draging
      _srcRect.Location = srcLoc; 
    }
    /// <summary>
    /// Endpoint of a Drag operation 
    /// </summary>
    /// <param name="vpEndPoint">The end point in Viewport coords</param>
    public void DragStop( Point vpEndPoint )
    {
      // recalc the new image center point with the final drag offset
      _imageCenter_src -= Scale( _drag_vp, ZoomLevel );
      // And reset the Offset
      _drag_vp = new Size( 0, 0 ); 
      // Both ops above should cancel each other out and leave the SrcLocation as is (no need to recalc the SrcRect here)

      // set end as new start (sanity only - will be set to newly used location on DragStart)
      _dragStart_vp = vpEndPoint; 
    }

    // Helpers

    // Recalculate the Source Rectangle based on internal vars 
    // To have it in one place only
    private void RecalcSrcRect( )
    {
      // from (draged) Image Center Point in Zoomed Src coords calc the 0/0 display location
      //   by subtracting the viewport Center Offset (Scaled to Zoom Level)
      // The shown area is _srcSize (which is only set in the Zoom code)
      _srcRect = new RectangleF( _imageCenter_src - Scale( _viewport, 0.5f * ZoomLevel ), _srcSize );
    }

    // Scales both coords of a SizeF item
    private static SizeF Scale( SizeF a, float mult ) => new SizeF( a.Width * mult, a.Height * mult );


  }
}
