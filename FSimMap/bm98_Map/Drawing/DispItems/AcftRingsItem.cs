using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XPoint;
using CoordLib;
using System.Diagnostics;
using Windows.Devices.Background;


namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Draws Range circle around the aircraft
  /// Where CoordPoint is the aircrafts coordinate
  /// </summary>
  internal class AcftRingsItem : DisplayItem
  {

    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    // stores the last used 1nm pixel length
    private int _lastDx1nm = int.MaxValue;

    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public AcftRingsItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AcftRingsItem( AcftRingsItem other )
      : base( other )
    {
      StringFormat = other.StringFormat.Clone( ) as StringFormat;
      _lastDx1nm = int.MaxValue; // init, will not be used from the clone but newly established
    }


    /// <summary>
    /// Draw range circles (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          // calculates a point 1 nm in direction of East at this latitude to calibrate the arc boxes
          // this length will change according to the latitude the acft is at
          var out1nm = vpRef.MapToCanvasPixel( CoordPoint.DestinationPoint( 1, 90, ConvConsts.EarthRadiusNm ) );

          // 1 nm vector length in pixels
          // using Ceiling will get a more stable value without rounding up/down events
          int dxy1nm = (int)Math.Ceiling( mp.Distance( out1nm ) );
          // this should maintain a more stable ringsize when moving in areas where the base size changes with each sim update
          // i.e. moving EW but with a slight NS deviation just a the boundary where the line length would change
          // creating annoying 'pumping' rings
          // eventually the ring size will change when going NS - nothing to do about it...
          if (dNetBm98.XMath.AboutEqual( dxy1nm, _lastDx1nm, 2.5f )) {
            // if about the same - use the current one
            dxy1nm = _lastDx1nm;
          }
          else {
            // significant change from current (>2 pixel), use the new one
            _lastDx1nm = dxy1nm;
          }

          if (dxy1nm > 5) {
            // 1 nm line should have some pixels to avoid smallest arcs when drawing

            int dxy2; // delta XY / 2 - will be scaled to ringsize below
            int dxy; // delta XY - will be scaled to ringsize below

            //dxy = dxy1nm * 5; dxy2 = (int)Math.Ceiling(dxy1nm * 2.5);
            //Rectangle rect5 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); // 2.5 nm
            dxy = dxy1nm * 10; dxy2 = dxy1nm * 5;
            Rectangle rect10 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); // 5nm
            //dxy = dxy1nm * 20; dxy2 = dxy1nm * 10;
            //Rectangle rect20 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); //10nm
            dxy = dxy1nm * 30; dxy2 = dxy1nm * 15;
            Rectangle rect30 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); //15nm
            //dxy = dxy1nm * 40; dxy2 = dxy1nm * 20;
            //Rectangle rect40 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); //20nm
            dxy = dxy1nm * 60; dxy2 = dxy1nm * 30;
            Rectangle rect60 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); //30nm
            dxy = dxy1nm * 120; dxy2 = dxy1nm * 60;
            Rectangle rect120 = new Rectangle( mp.Subtract( new Point( dxy2, dxy2 ) ), new Size( dxy, dxy ) ); //60nm

            {
              g.DrawEllipse( Pen, rect120 );
              g.DrawEllipse( Pen, rect60 );
              //              g.DrawEllipse( Pen, rect40 );
              g.DrawEllipse( Pen, rect30 );
              //              g.DrawEllipse( Pen, rect20 );
              g.DrawEllipse( Pen, rect10 );
              // g.DrawEllipse( Pen, rect5 );

              // labels - need to turn the graphics for proper label orientation
              TurnAroundPoint( g, mp, vpRef.MapHeading );

              g.DrawString( "60 nm", Font, TextBrush, rect120.Location.Add( new Point( rect120.Width / 2, 0 ) ), StringFormat );
              g.DrawString( "30 nm", Font, TextBrush, rect60.Location.Add( new Point( rect60.Width / 2, 0 ) ), StringFormat );
              //              g.DrawString( "20 nm", Font, TextBrush, rect40.Location.Add( new Point( rect40.Width / 2, 0 ) ), StringFormat );
              g.DrawString( "15 nm", Font, TextBrush, rect30.Location.Add( new Point( rect30.Width / 2, 0 ) ), StringFormat );
              //              g.DrawString( "10 nm", Font, TextBrush, rect20.Location.Add( new Point( rect20.Width / 2, 0 ) ), StringFormat );
              g.DrawString( "5 nm", Font, TextBrush, rect10.Location.Add( new Point( rect10.Width / 2, 0 ) ), StringFormat );
              // g.DrawString( "2.5 nm", Font, TextBrush, rect5.Location.Add( new Point( rect5.Width / 2, 0 ) ), StringFormat );
            }
          }
        }
      }
      g.EndContainer( save );
    }

  }
}
