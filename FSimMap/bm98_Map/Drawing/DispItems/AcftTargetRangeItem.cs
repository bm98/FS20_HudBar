using System.Drawing;
using System.Drawing.Drawing2D;

using CoordLib;

using static dNetBm98.XPoint;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Draws Range arcs from the Aircrafts position out towards the direction and target Altitude (airport)
  /// </summary>
  internal class AcftTargetRangeItem : DisplayItem
  {
    /// <summary>
    /// The Aircraft Track (Ref only, not managed here)
    /// </summary>
    public Data.TrackedAircraft AircraftTrackRef;

    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public AcftTargetRangeItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AcftTargetRangeItem( AcftTargetRangeItem other )
      : base( other )
    {
      AircraftTrackRef = other.AircraftTrackRef; // ref only
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          var out1nm = vpRef.MapToCanvasPixel( CoordPoint.DestinationPoint( 1,AircraftTrackRef.TrueHeading_deg, ConvConsts.EarthRadiusNm ) );
          int dist = (int)mp.Distance( out1nm );

          Rectangle rect5 = new Rectangle( mp.Subtract( new Point( dist * 5 / 2, dist * 5 / 2 ) ), new Size( dist * 5, dist * 5 ) ); // 2.5 nm
          Rectangle rect10 = new Rectangle( mp.Subtract( new Point( dist * 10 / 2, dist * 10 / 2 ) ), new Size( dist * 10, dist * 10 ) ); // 5nm
          Rectangle rect20 = new Rectangle( mp.Subtract( new Point( dist * 20 / 2, dist * 20 / 2 ) ), new Size( dist * 20, dist * 20 ) ); //10nm

          var shift = new Size( mp.X, mp.Y );
          g.TranslateTransform( -shift.Width, -shift.Height );
          g.RotateTransform( AircraftTrackRef.TrueHeading_deg, MatrixOrder.Append );
          g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );
          g.DrawArc( Pen, rect20, -15 - 90, 30 );
          g.DrawArc( Pen, rect10, -15 - 90, 30 );
          g.DrawArc( Pen, rect5, -15 - 90, 30 );
        }
      }
      g.EndContainer( save );
    }
  }
}

