using System.Drawing.Drawing2D;
using System.Drawing;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Draw the Wind Component
  /// </summary>
  internal class AcftWindItem : DisplayItem
  {

    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

    /// <summary>
    /// Wind Direction deg
    /// </summary>
    public float WindDir_deg { get; set; } = 0;

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public AcftWindItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AcftWindItem( AcftWindItem other )
      : base( other )
    {
      WindDir_deg = other.WindDir_deg;
      StringFormat = other.StringFormat.Clone( ) as StringFormat; ;
    }

    private const float _scale = 3f;
    // an Wind Arrow shape sitting below the acft and pointing towards the acft, 0/0 is center of the acft- will be scaled later
    // should be outside the Aircraft shape
    private PointF[] _arrowShape = new PointF[] {
      new PointF(0,6),
      new PointF(-3,3),
      new PointF(3,3),
      new PointF(0,6),
      new PointF(0,-6),
    };

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

        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint ); // should be the aircraft
        if (g.VisibleClipBounds.Contains( mp )) {
          float rot = WindDir_deg;
          // acft pos to 0/0 to rotate, scale and the draw symmetrically
          g.RotateTransform( rot, MatrixOrder.Append );
          g.TranslateTransform( mp.X, mp.Y, MatrixOrder.Append );
          g.ScaleTransform( 3, 3 ); // up to size
          g.TranslateTransform( 0, -24 );
          g.DrawLines( Pen, _arrowShape );
          // text invalidate the prev rot and move text box in place
          g.TranslateTransform( 0, -18 ); // was 18
          g.RotateTransform( -rot );
          // upright to get label level
          g.RotateTransform( vpRef.MapHeading );

          g.DrawString( String, Font, TextBrush, 0, 0, StringFormat );
        }
        else {
        }
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
      if (float.IsNaN( WindDir_deg )) return;

      // either of the two will be used finally...
      //PaintThisSprite( g, MapToPixel );
      PaintThisShape( g, vpRef );
    }



  }
}
