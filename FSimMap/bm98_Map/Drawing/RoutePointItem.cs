using CoordLib;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// To show a Route Point
  /// </summary>
  internal class RoutePointItem : DisplayItem
  {
    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    /// <summary>
    /// Track of the Outbound path
    /// </summary>
    public int OutboundTrack_deg { get; set; } = 0;
    /// <summary>
    /// Coord of the Outbound path
    /// </summary>
    public LatLon OutboundLatLon { get; set; } = new LatLon( 0, 0 );
    /// <summary>
    /// Waypoint Label Rectangle
    /// </summary>
    public Rectangle WypLabelRectangle { get; set; }

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public RoutePointItem( Bitmap spriteRef )
    {
      _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        var omp = MapToPixel( OutboundLatLon );

        if (g.VisibleClipBounds.Contains( mp ) || g.VisibleClipBounds.Contains( omp )) {
          var rect = new Rectangle( new Point( 0, 0 ), _imageRef.Size );
          var textRect = new Rectangle( mp, new Size( 96, 64 ) );

          // draw to if possible
          if (!OutboundLatLon.IsEmpty) {
            g.DrawLine( Pen, mp, omp );
          }
          var rotSave = g.BeginContainer( );
          g.TranslateTransform( -rect.Width / 2, -rect.Height / 2, MatrixOrder.Append );
          g.RotateTransform( (float)OutboundTrack_deg, MatrixOrder.Append );
          g.TranslateTransform( mp.X, mp.Y, MatrixOrder.Append );

          //g.TranslateTransform( rect.Width / 2, rect.Height / 2, MatrixOrder.Append );
          g.DrawImage( _imageRef, rect );
          g.EndContainer( rotSave );

          // 
          g.TranslateTransform( -textRect.Width / 2, rect.Height / 2, MatrixOrder.Append );// below
          g.DrawString( String, Font, TextBrush, textRect, StringFormat );
        }
        else {
          // only the track is visible
          // draw to if possible
          if (!OutboundLatLon.IsEmpty) {
            g.DrawLine( Pen, mp, omp );
          }
        }
      }
      g.EndContainer( save );
    }
  }
}


