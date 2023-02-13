﻿using System;
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
  /// Draws Range circle around the airport
  /// </summary>
  internal class AirportRangeItem : DisplayItem
  {

    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public AirportRangeItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AirportRangeItem( AirportRangeItem other )
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
        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          // calculates a point 1 nm in direction of North to calibrate the arc boxes
          var out1nm = MapToPixel( CoordPoint.DestinationPoint( 1, 0, ConvConsts.EarthRadiusNm ) );
          int dist = (int)mp.Distance( out1nm ); // 1 nm vector length in pixels
          if (dist > 5) {
            // 1 nm line should have some pixels to avoid smallest arcs
            Rectangle rect5 = new Rectangle( mp.Subtract( new Point( dist * 5 / 2, dist * 5 / 2 ) ), new Size( dist * 5, dist * 5 ) ); // 2.5 nm
            Rectangle rect10 = new Rectangle( mp.Subtract( new Point( dist * 10 / 2, dist * 10 / 2 ) ), new Size( dist * 10, dist * 10 ) ); // 5nm
            Rectangle rect20 = new Rectangle( mp.Subtract( new Point( dist * 20 / 2, dist * 20 / 2 ) ), new Size( dist * 20, dist * 20 ) ); //10nm
            Rectangle rect30 = new Rectangle( mp.Subtract( new Point( dist * 30 / 2, dist * 30 / 2 ) ), new Size( dist * 30, dist * 30 ) ); //15nm
            Rectangle rect40 = new Rectangle( mp.Subtract( new Point( dist * 40 / 2, dist * 40 / 2 ) ), new Size( dist * 40, dist * 40 ) ); //20nm
            Rectangle rect60 = new Rectangle( mp.Subtract( new Point( dist * 60 / 2, dist * 60 / 2 ) ), new Size( dist * 60, dist * 60 ) ); //30nm
            Rectangle rect120 = new Rectangle( mp.Subtract( new Point( dist * 120 / 2, dist * 120 / 2 ) ), new Size( dist * 120, dist * 120 ) ); //60nm

            {
              g.DrawEllipse( Pen, rect120 );
              g.DrawString( "60 nm", Font, TextBrush, rect120.Location.Add( new Point( rect120.Width / 2, 0 ) ), StringFormat );
              g.DrawEllipse( Pen, rect60 );
              g.DrawString( "30 nm", Font, TextBrush, rect60.Location.Add( new Point( rect60.Width / 2, 0 ) ), StringFormat );
//              g.DrawEllipse( Pen, rect40 );
//              g.DrawString( "20 nm", Font, TextBrush, rect40.Location.Add( new Point( rect40.Width / 2, 0 ) ), StringFormat );
              g.DrawEllipse( Pen, rect30 );
              g.DrawString( "15 nm", Font, TextBrush, rect30.Location.Add( new Point( rect30.Width / 2, 0 ) ), StringFormat );
//              g.DrawEllipse( Pen, rect20 );
//              g.DrawString( "10 nm", Font, TextBrush, rect20.Location.Add( new Point( rect20.Width / 2, 0 ) ), StringFormat );
              g.DrawEllipse( Pen, rect10 );
              g.DrawString( "5 nm", Font, TextBrush, rect10.Location.Add( new Point( rect10.Width / 2, 0 ) ), StringFormat );
             // g.DrawEllipse( Pen, rect5 );
             // g.DrawString( "2.5 nm", Font, TextBrush, rect5.Location.Add( new Point( rect5.Width / 2, 0 ) ), StringFormat );
            }
          }
        }
      }
      g.EndContainer( save );
    }

  }
}
