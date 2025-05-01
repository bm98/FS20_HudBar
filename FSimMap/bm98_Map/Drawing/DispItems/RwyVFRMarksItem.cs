using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using static dNetBm98.XPoint;
using CoordLib;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Draws some VFR Markings around the airport/runway
  /// </summary>
  internal class RwyVFRMarksItem : RunwayItem
  {
    /// <summary>
    /// Main (Start) runway bearing (deg Mag) for number display
    /// </summary>
    public float StartHeading_degm { get; set; } = 0; // default - must be set 
    public string StartRunwayIdent { get; set; } = ""; // default - must be set 

    /// <summary>
    /// Other (End) runway bearing (deg Mag) for number display
    /// </summary>
    public float EndHeading_degm { get; set; } = 0; // default - must be set 
    public string EndRunwayIdent { get; set; } = ""; // default - must be set 

    /// <summary>
    /// Ring Range font
    /// </summary>
    public Font RangeFont { get; set; } = FontsAndColors.FtSmall; // default - must be set 
    /// <summary>
    /// Heading Pen for incoming lines when no VFR deco is shown (ladder)
    /// </summary>
    public Pen NoDecoPen { get; set; } = Pens.Pink; // default - must be set 
    /// <summary>
    /// Main Heading Pen for incoming lines and Left hand pattern
    /// </summary>
    public Pen VfrPenMain { get; set; } = Pens.Pink; // default - must be set 
    /// <summary>
    /// Alternate Heading Pen for outgoing lines and Right hand pattern
    /// </summary>
    public Pen VfrPenAlt { get; set; } = Pens.Pink; // default - must be set 

    /// <summary>
    /// True to show the full VFR painting, False to have just the selected RWY indicated
    /// </summary>
    public bool ShowFullDecoration { get; set; } = false;

    /// <summary>
    /// cTor: create sprite
    /// </summary>
    public RwyVFRMarksItem( )
    {
      StringFormat.Alignment = StringAlignment.Center;
      StringFormat.LineAlignment = StringAlignment.Far;
      RangeFont = base.Font;
      VfrPenMain = base.Pen;
      VfrPenAlt = base.Pen;
    }

    private void PaintRwyItems( Graphics g, int dist, float rwyLen_px, float bearing, string rwIdent, GraphicsPath pattern, bool mainRwy )
    {
      // need to dispose the pen and brushes at the end
      var pendash = new Pen( Color.Black, 1f ) { DashStyle = DashStyle.Dot };
      var brushPatternL = new SolidBrush( Color.FromArgb( 127, FontsAndColors.ColVfrMain ) );
      var brushPatternR = new SolidBrush( Color.FromArgb( 127, FontsAndColors.ColVfrAlt ) );
      var brushStraightIn = new SolidBrush( Color.FromArgb( 100, mainRwy ? Color.Green : Color.DarkGray ) );

      Rectangle hdgBox = new Rectangle( new Point( -100, dist ), new Size( 200, 100 ) );// needs to be large enough to hold the text '000°'
      Rectangle straightIn = new Rectangle( new Point( (int)(-dist * 5), (int)(-dist * 5) ), new Size( dist * 10, dist * 10 ) );// 5nm tall

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        // traffic pattern 
        g.FillPath( mainRwy ? brushPatternR : brushPatternL, pattern ); // Right hand pattern
        g.ScaleTransform( -1, 1 ); // invert X
        g.FillPath( mainRwy ? brushPatternL : brushPatternR, pattern ); // Left hand pattern
        g.ScaleTransform( -1, 1 ); // invert X again to normal

        var saveRwyTx = g.Transform;
        {
          // draw enty points at 5nm
          g.RotateTransform( -45 ); // rotate to -45
          g.FillEllipse( FillBrush, new Rectangle( 0, dist * 5, 30, 30 ) );
          g.RotateTransform( 90 ); // rotate to +45
          g.FillEllipse( FillBrush, new Rectangle( 0, dist * 5, 30, 30 ) );
          g.RotateTransform( -45 ); // rotate to +0
          // center Start of runway in Y direction to 0/0
          g.TranslateTransform( 0, rwyLen_px / 2 );

          // +-30° segment / arc goes in X dir for 0 deg - we rotated to have N up - so draw is -90° to have it on Y up
          g.FillPie( brushStraightIn, straightIn, 60f, 60f );
          // mag heading number
          g.DrawString( $"{rwIdent}\n{bearing:000}°", Font, TextBrush, hdgBox, StringFormat );
        }
        g.Transform = saveRwyTx;

        // we are back at the center of the runway at 0/0
        // drawing the left, then right hand entries
        saveRwyTx = g.Transform;
        {
          g.TranslateTransform( -dist, 0 );
          g.RotateTransform( 90 + 45 );
          g.DrawLine( pendash, new Point( 0, 0 ), new Point( 0, dist * 4 ) ); // fine line up to 5nm out
          g.DrawLine( VfrPenMain, new Point( 0, 0 ), new Point( 0, dist ) ); // main entry Left
          g.FillEllipse( FillBrush, new Rectangle( -8, -8, 16, 16 ) );
        }
        g.Transform = saveRwyTx;
        // border of pattern 1nm right 45° dashed line down
        {
          g.TranslateTransform( dist, 0 );
          g.RotateTransform( -90 - 45 );
          g.DrawLine( pendash, new Point( 0, 0 ), new Point( 0, dist * 4 ) ); // fine line up to 5nm out
          g.DrawLine( VfrPenAlt, new Point( 0, 0 ), new Point( 0, dist ) ); // main entry Right
          g.FillEllipse( FillBrush, new Rectangle( -8, -8, 16, 16 ) );
        }
      }
      g.EndContainer( save );

      pendash.Dispose( );
      brushPatternL.Dispose( );
      brushPatternR.Dispose( );
      brushStraightIn.Dispose( );
    }


    /// <summary>
    /// Draw VFR Marks (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g,IVPortPaint vpRef )
    {
      if (Lenght == 0) return; // NOT SET, avoid Div0
      if (!Active) return; // shall not be drawn

      Point start_px = vpRef.MapToCanvasPixel( Start );
      Point end_px = vpRef.MapToCanvasPixel( End );
      float rwyLen_px = start_px.Distance( end_px ); // pix length of the Runway
      // SQRT of Length() to get NaN ?? still happens ??
      if (float.IsNaN( rwyLen_px )) {
        ; // TODO - odd case to handle
        return;
      }
      rwyLen_px = (rwyLen_px < 2f) ? 2f : rwyLen_px;

      // calculate the drawing angle of the runway endpoint to place the Numbers (Headings are not matching the drawn items due to mag var)
      // angle between two vectors (where (x2|y2) is set as unity vector pointing North)
      //var x2 = 0;
      //var y2 = 1;
      //dot = x1*x2 + y1*y2  
      //det = x1*y2 - y1*x2
      //angle = atan2(det, dot) // radians
      var startAngle = (Math.Atan2( end_px.X - start_px.X, -(end_px.Y - start_px.Y) ) / Math.PI * 180.0); // drawing coords are Y-upside down

      RectangleF textRect = new RectangleF {
        Location = new PointF( -20, rwyLen_px / 2.0f ), // X=> 1/2 Box Width (set below)
        Size = new SizeF( 40, 50 ) // box size
      };
      // Runway center coord
      var rMid = new Point( start_px.X + (int)((end_px.X - start_px.X) / 2.0), start_px.Y + (int)((end_px.Y - start_px.Y) / 2.0) );
      // calculates a point 1 nm in direction of North to calibrate the length
      var out1nm = vpRef.MapToCanvasPixel( Start.DestinationPoint( 1, 0, ConvConsts.EarthRadiusNm ) );
      int dist1nm = (int)start_px.Distance( out1nm ); // 1 nm vector length in pixels
      // fine dash for the accross runway center
      var pendash = new Pen( Color.Black, 1f ) { DashStyle = DashStyle.Dot };

      // traffic pattern shading 0.5..1nm
      GraphicsPath pattern = new GraphicsPath( );
      pattern.AddLine( new PointF( dist1nm / 2, 0 ), new PointF( dist1nm, 0 ) );
      pattern.AddLine( new PointF( dist1nm, 0 ), new PointF( dist1nm, rwyLen_px / 2f + dist1nm ) );
      pattern.AddLine( new PointF( dist1nm, rwyLen_px / 2f + dist1nm ), new PointF( dist1nm / 2f, rwyLen_px / 2f + dist1nm / 2f ) );
      pattern.AddLine( new PointF( dist1nm / 2f, rwyLen_px / 2f + dist1nm / 2f ), new PointF( dist1nm / 2, 0 ) );
      // 2nm circle box
      Rectangle rect4 = new Rectangle( new Point( -dist1nm * 4 / 2, -dist1nm * 4 / 2 ), new Size( dist1nm * 4, dist1nm * 4 ) );

      var saveDrawing = g.BeginContainer( );
      {
        // prep orientation
        g.TranslateTransform( rMid.X, rMid.Y ); // center runway at 0|0

        // For both runway ends
        var saveRwy = g.BeginContainer( );
        {
          // rotate so the rwy point upwards for the incomming flight
          g.RotateTransform( (float)startAngle );
          // Runway straight dashed lines
          if (ShowFullDecoration) {
            var psave = VfrPenMain.DashStyle;
            VfrPenMain.DashStyle = DashStyle.Dot;
            VfrPenAlt.DashStyle = DashStyle.Dot;
//            g.DrawLine( VfrPenMain, new Point( 0, dist1nm * 5 ), new Point( 0, (int)rwyLen_px ) ); // fly in
//            g.DrawPie( VfrPenMain, new Rectangle( new Point( -30, (int)rwyLen_px - 60 ), new Size( 60, 120 ) ), +90 - 30, 60 );// arrow, kind of
            g.DrawLine( VfrPenAlt, new Point( 0, -dist1nm * 5 ), new Point( 0, -(int)rwyLen_px ) ); // fly out
            VfrPenMain.DashStyle = psave;
            VfrPenAlt.DashStyle = psave;
          }

          {
            // Only this part is SHOWN when Full Deco is OFF (an arrow pointing towards the runway)
            float lineStart = rwyLen_px / 2f;
            float lineEnd = lineStart + dist1nm * 16f;
            g.DrawLine( NoDecoPen, new PointF( 0, lineStart ), new PointF( 0, lineEnd ) ); // fly in
            g.DrawLine( NoDecoPen, new PointF( -10, lineStart ), new PointF( 10, lineStart ) ); // step 2nm
            g.DrawLine( NoDecoPen, new PointF( -30, lineStart + 2 * dist1nm ), new PointF( 30, lineStart + 2 * dist1nm ) ); // step 2nm
            g.DrawLine( NoDecoPen, new PointF( -30, lineStart + 4 * dist1nm ), new PointF( 30, lineStart + 4 * dist1nm ) ); // step 4nm
            g.DrawLine( NoDecoPen, new PointF( -30, lineStart + 8 * dist1nm ), new PointF( 30, lineStart + 8 * dist1nm ) ); // step 8nm
            g.DrawLine( NoDecoPen, new PointF( -30, lineEnd ), new PointF( 30, lineEnd ) ); // step end
          }

          if (ShowFullDecoration) {
            // Runway accross dashed lines
            g.DrawLine( pendash, new Point( -dist1nm * 5, 0 ), new Point( -(int)rwyLen_px, 0 ) );
            g.DrawLine( pendash, new Point( (int)rwyLen_px, 0 ), new Point( dist1nm * 5, 0 ) );
            // Runway 2nm circle
            g.DrawEllipse( Pen, rect4 );
            g.RotateTransform( 90 );
            g.DrawString( "2 nm", RangeFont, TextBrush, rect4.Location.Add( new Point( rect4.Width / 2, 0 ) ), StringFormat );
          }
        }
        g.EndContainer( saveRwy );

        if (ShowFullDecoration) {
          // draw 'Start' side
          saveRwy = g.BeginContainer( );
          {
            // rotate so the rwy point upwards for the incomming flight
            g.RotateTransform( (float)startAngle );
            PaintRwyItems( g, dist1nm, rwyLen_px, StartHeading_degm, StartRunwayIdent, pattern, true );
          }
          g.EndContainer( saveRwy );

          // draw 'End' side
          if (!string.IsNullOrEmpty( EndID )) {
            saveRwy = g.BeginContainer( );
            {
              g.RotateTransform( (float)Geo.Wrap360( startAngle + 180 ) );
              PaintRwyItems( g, dist1nm, rwyLen_px, EndHeading_degm, EndRunwayIdent, pattern, false );
            }
            g.EndContainer( saveRwy );
          }
        }
      }
      g.EndContainer( saveDrawing );
      // closure
      pendash.Dispose( );
      pattern.Dispose( );
    }

  }
}
