using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XPoint;
using CoordLib;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Draws some VFR Markings around the airport
  /// </summary>
  internal class AptVFRMarksItem : AirportRangeItem
  {
    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public AptVFRMarksItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AptVFRMarksItem( AptVFRMarksItem other )
      : base( other )
    {
      StringFormat = other.StringFormat.Clone( ) as StringFormat;
    }

    /// <summary>
    /// Draw range circles (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          // calculates a point 1 nm in direction of Northto calibrate the arc boxes
          var out1nm = MapToPixel( CoordPoint.DestinationPoint( 1, 0, ConvConsts.EarthRadiusNm ) );
          int dist = (int)mp.Distance( out1nm ); // 1 nm vector length in pixels
          if (dist > 5) {
            // 1 nm line should have some pixels to avoid smallest arcs
            Rectangle rect4 = new Rectangle( mp.Subtract( new Point( dist * 4 / 2, dist * 4 / 2 ) ), new Size( dist * 4, dist * 4 ) ); // 2 nm
            Rectangle rect10 = new Rectangle( mp.Subtract( new Point( dist * 10 / 2, dist * 10 / 2 ) ), new Size( dist * 10, dist * 10 ) ); // 5nm

            {
              g.DrawEllipse( Pen, rect10 );
              g.DrawString( "5 nm", Font, TextBrush, rect10.Location.Add( new Point( rect10.Width / 2, 0 ) ), StringFormat );
              g.DrawEllipse( Pen, rect4 );
              g.DrawString( "2 nm", Font, TextBrush, rect4.Location.Add( new Point( rect4.Width / 2, 0 ) ), StringFormat );
            }
          }
        }
      }
      g.EndContainer( save );
    }

  }
}
