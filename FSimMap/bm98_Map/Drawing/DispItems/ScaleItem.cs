using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Draws A Scale of the Map
  /// Location is right above the copyright (left bottom)
  /// </summary>
  internal class ScaleItem : DisplayItem
  {
    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };


    /// <summary>
    /// The Pixel per Nautical Mile at current range
    /// </summary>
    public float PixelPerNm { get; set; }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public ScaleItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public ScaleItem( ScaleItem other )
      : base( other )
    {
      this.StringFormat = other.StringFormat.Clone( ) as StringFormat;
      this.PixelPerNm = other.PixelPerNm;
    }

    /// <summary>
    /// Draw a Map Scale
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.Save();  

      // the map is centered in the view
      var mp = vpRef.MapToCanvasPixel( vpRef.ViewCenterLatLon );
      // move and rot to have the canvas center at 0/0 and North up
      g.TranslateTransform( mp.X, mp.Y );

      // the Scale shall be visible all the time but not intrusive... below the center
      // size is _scale * 20 (-10..10) pix square
      // place it at 0/nPix above the bottom of the view (leave room for the copyright and the color ladder)
      mp = new Point( -vpRef.ViewPortView.Width/4, vpRef.ViewPortView.Height / 2 - 80 );
      mp = vpRef.VPixelToMatrixPixel( mp ); // matrix pixels

      // set Scale center
      g.TranslateTransform( mp.X, mp.Y );

      // scale location
      //Point loc = new Point( (int)(g.VisibleClipBounds.Width/4f), (int)g.VisibleClipBounds.Bottom - 320 ); // leftish, above the copyright line
      Point loc = new Point( 0, 0 ); // leftish, above the copyright line

      // try to make a scale with 5 segments with a segment width of about 100 pix
      int segment = 100; // proposed segment width in pixels
      float nmPseg = segment / PixelPerNm;
      nmPseg = (nmPseg < 1) ? (int)Math.Ceiling( nmPseg * 5f ) / 5f : (int)Math.Ceiling( nmPseg / 2f ) * 2; // get 0.2 steps below 1 or increments of 2 above 1
      int div = (int)(nmPseg * PixelPerNm);
      // draw 5 segments
      for (int i = 0; i < 5; i++) {
        g.DrawLine( Pen, loc.X + i * div, loc.Y, loc.X + i * div + div, loc.Y );
        g.DrawLine( Pen, loc.X + i * div, loc.Y - 7, loc.X + i * div, loc.Y + 7 ); // scale mark
        g.DrawString( $"{i * nmPseg}", Font, TextBrush, loc.X + i * div, loc.Y - 8, StringFormat );
      }
      // end marker and number
      g.DrawLine( Pen, loc.X + 5 * div, loc.Y - 7, loc.X + 5 * div, loc.Y + 7 ); // scale mark
      g.DrawString( $"{5 * nmPseg} nm", Font, TextBrush, loc.X + 5 * div, loc.Y - 8, StringFormat );

      g.Restore( save );

    }

  }
}