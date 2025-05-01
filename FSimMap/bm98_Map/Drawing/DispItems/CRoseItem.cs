using System.Drawing;
using System.Drawing.Drawing2D;


namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// A Compass Rose 
  /// </summary>
  internal class CRoseItem : DisplayItem
  {

    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };


    /// <summary>
    /// Flag to indicate the Acft is on ground
    /// </summary>
    public bool OnGround = false;

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public CRoseItem( Bitmap spriteRef = null )
    {
      if (spriteRef != null) {
        _imageRef = new Bitmap( spriteRef );
      }
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public CRoseItem( CRoseItem other )
      : base( other )
    {
      _imageRef = other._imageRef; // ref image
      StringFormat = other.StringFormat.Clone( ) as StringFormat; ;
    }

    // an aircraft shape pointing upwards, 0/0 is center of the acft- will be scaled later
    private static readonly Point[] c_roseShape = new Point[] {
      new Point(0,-10), // N
      new Point(-2,-8),
      new Point(-1,-8),
      new Point(-1,-1),
      new Point(-8,-1),
      new Point(-8,-2),
      new Point(-10,0), // W
      new Point(-8,2),
      new Point(-8,1),
      new Point(-1,1),
      new Point(-1,8),
      new Point(-2,8),
      new Point(0,10), // S
      new Point(2,8),
      new Point(1,8),
      new Point(1,1),
      new Point(8,1),
      new Point(8,2),
      new Point(10,0), // E
      new Point(8,-2),
      new Point(8,-1),
      new Point(1,-1),
      new Point(1,-8),
      new Point(2,-8),
     // new Point(0,-10), // looks better if the GP closes the curve..
    };
    private float _tension = 0.1f;
    private float _scale = 8f;

    /// <summary>
    /// Draw an aircraft filled with the AltColor Paint (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected void PaintThisShape( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // used in RADAR View
        // the map is centered in the view
        var mp = vpRef.MapToCanvasPixel( vpRef.ViewCenterLatLon );
        // move and rot to have the canvas center at 0/0 and North up
        g.TranslateTransform( mp.X, mp.Y );
        g.RotateTransform( vpRef.MapHeading );

        // the Compass rose shall be visible all the time but not intrusive... below the center
        // size is _scale * 20 (-10..10) pix square
        // place it at 0/nPix above the bottom of the view (leave room for the copyright and the color ladder)
        mp = new Point( 0, vpRef.ViewPortView.Height / 2 - (int)(_scale * 20) - 80 );
        mp = vpRef.VPixelToMatrixPixel( mp ); // matrix pixels

        // set rose center
        g.TranslateTransform( mp.X, mp.Y);
        // as the compass shows the original direction revert the heading 
        g.RotateTransform( -vpRef.MapHeading );

        g.DrawString( "N", Font, TextBrush, new PointF( -5 * _scale, -8 * _scale ), StringFormat );
        g.ScaleTransform( _scale, _scale ); // up to size
        g.DrawClosedCurve( Pen, c_roseShape, _tension, FillMode.Winding );
      }
      g.EndContainer( save );
    }


    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn
      // either of the two will be used finally...
      //PaintThisSprite( g, MapToPixel );
      PaintThisShape( g, vpRef );
    }



  }
}
