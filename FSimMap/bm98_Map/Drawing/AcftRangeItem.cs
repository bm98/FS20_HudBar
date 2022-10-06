using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Draws Range arcs from the Aircrafts position out
  /// </summary>
  internal class AcftRangeItem : DisplayItem
  {
    /// <summary>
    /// The Aircraft Data (Ref only, not managed here)
    /// </summary>
    public Data.TrackedAircraft AircraftTrackRef = null;

    /// <summary>
    /// Pen to draw the Track Arc
    /// </summary>
    public Pen PenTrack { get; set; } = FontsAndColors.PenRange5;

    /// <summary>
    /// Target Range Pen
    /// </summary>
    public Pen TgtPen { get; set; } = FontsAndColors.PenTRange3;

    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public AcftRangeItem( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AcftRangeItem( AcftRangeItem other )
      : base( other )
    {
      AircraftTrackRef = other.AircraftTrackRef; // ref only
    }

    // Paint the distance arcs
    private void PaintRange( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (AircraftTrackRef.OnGround) return; // don't draw ranges on ground
      if (float.IsNaN( AircraftTrackRef.TrueTrk_deg )) return;

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = MapToPixel( AircraftTrackRef.Position );
        if (g.VisibleClipBounds.Contains( mp )) {
          // calulcates a point 1 nm in direction of the true heading to calibrate the arc boxes
          var out1nm = MapToPixel( CoordPoint.DestinationPoint( 1, AircraftTrackRef.TrueHeading, ConvConsts.EarthRadiusNm ) );
          int dist = (int)mp.Distance( out1nm ); // 1 nm vector length in pixels
          if (dist > 5) {
            // 1 nm line should have some pixels to avoid smallest arcs
            Rectangle rect5 = new Rectangle( mp.Subtract( new Point( dist * 5 / 2, dist * 5 / 2 ) ), new Size( dist * 5, dist * 5 ) ); // 2.5 nm
            Rectangle rect10 = new Rectangle( mp.Subtract( new Point( dist * 10 / 2, dist * 10 / 2 ) ), new Size( dist * 10, dist * 10 ) ); // 5nm
            Rectangle rect20 = new Rectangle( mp.Subtract( new Point( dist * 20 / 2, dist * 20 / 2 ) ), new Size( dist * 20, dist * 20 ) ); //10nm

            var shift = new Size( mp.X, mp.Y );
            g.TranslateTransform( -shift.Width, -shift.Height );
            // save before final rotation towards the true heading
            var save2 = g.Transform;
            {
              // Heading Arcs
              g.RotateTransform( AircraftTrackRef.TrueHeading, MatrixOrder.Append );
              g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );
              // arc goes in X dir for 0 deg - we rotated to have N up - so draw is -90° to have it on Y up
              g.DrawArc( Pen, rect20, -15 - 90, 30 );
              g.DrawArc( Pen, rect10, -15 - 90, 30 );
              g.DrawArc( Pen, rect5, -15 - 90, 30 );
            }
            // resume before rotate and do it for the TrueTrack
            g.Transform = save2;
            // Track small arc overlays the heading arc
            g.RotateTransform( AircraftTrackRef.TrueTrk_deg, MatrixOrder.Append );
            g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );
            g.DrawArc( PenTrack, rect20, -3 - 90, 6 );
            g.DrawArc( PenTrack, rect10, -3 - 90, 6 );
            g.DrawArc( PenTrack, rect5, -3 - 90, 6 );
          }
        }
      }
      g.EndContainer( save );
    }

    // Paint the Target Range
    private void PaintTargetRange( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (float.IsNaN( AircraftTrackRef.TrueTrk_deg )) return;

      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        PointF mp = MapToPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          // calulcates a point 1 nm in direction of the true heading to calibrate the arc boxes
          var out1nm = MapToPixel( CoordPoint.DestinationPoint( 1, AircraftTrackRef.TrueTrk_deg, ConvConsts.EarthRadiusNm ) );
          int dist = (int)mp.Distance( out1nm ); // 1 nm vector length in pixels
          if (dist > 15) {
            // 1 nm line should have some pixels to avoid smallest arcs
            var d2 = AircraftTrackRef.DistanceToTargetAlt( ) * 2f; // 2x Distance for the Arc Rectangle
            RectangleF rectT = new RectangleF( mp.Subtract( new PointF( dist * d2 / 2, dist * d2 / 2 ) ), new SizeF( dist * d2, dist * d2 ) ); // 2.5 nm
            if (d2 < 1) {
              rectT = new RectangleF( mp.Subtract( new PointF( dist * d2 / 2, dist * d2 / 2 ) ), new SizeF( 2, 2 ) ); 
            }

            var shift = new SizeF( mp.X, mp.Y );
            g.TranslateTransform( -shift.Width, -shift.Height );
            g.RotateTransform( AircraftTrackRef.TrueTrk_deg, MatrixOrder.Append );
            g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );
            g.DrawArc( TgtPen, rectT, -10 - 90, 20 );
          }
        }
      }
      g.EndContainer( save );
    }

    /// <summary>
    /// Draw range arcs (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn
      if (AircraftTrackRef == null) return; // not yet available

      if (AircraftTrackRef.ShowTargetRange) {
        PaintTargetRange( g, MapToPixel );
      }
      else {
        PaintRange( g, MapToPixel );
      }

    }
  }
}
