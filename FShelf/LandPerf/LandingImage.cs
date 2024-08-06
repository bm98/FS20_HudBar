using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using dNetBm98;

using FSimClientIF.Modules;

namespace FShelf.LandPerf
{
  /// <summary>
  /// Provides a Landing Image 
  /// </summary>
  public class LandingImage
  {
    // Test Data
    private PerfTracker.PerfData _lPerf = new PerfTracker.PerfData( ) {
      TdCount = 1,
      LandingPerf = new LandingPerfData(
      3600,
      200f,
      3f, 0f, 5f,
      90f, 1.2f,
      90f, 95f, 60f, 70f, 3f,
      330f, 10f,
      float.NaN, float.NaN,
      "D_CALL",
      "D_TITLE"
      ),
      AirportIdent = "D_APT",
      RunwayIdent = "D_RWI",
      RunwayBearing_deg = 90,
      TdDisplacement_m = 20,
      TdDistance_m = 300
    };

    // static resources - never dispose them !!!

    private static readonly Bitmap c_acftIcon = Properties.Resources.Aircraft_Icon_blue;
    private static readonly Bitmap c_baseImage = Properties.Resources.Runway_top;
    private static readonly RectangleF c_baseRect = new RectangleF( new Point( 0, 0 ), Properties.Resources.Runway_top.Size );

    // an Wind Arrow shape sitting below the acft and pointing towards the acft, 0/0 is center of the acft- will be scaled later
    // should be outside the Aircraft shape
    private static PointF[] c_arrowShape = new PointF[] {
      new PointF(0,6),
      new PointF(-3,3),
      new PointF(3,3),
      new PointF(0,6),
      new PointF(0,-6),
    };

    private static readonly float c_rwLenDrawn_m = 1500;

    private static readonly StringFormat c_stringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
    private static readonly StringFormat c_stringFormatText = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

    private static readonly Brush c_txtBrush = new SolidBrush( Color.Black );
    private static readonly Pen c_trkPen = new Pen( Color.Green, 4 );
    private static readonly Pen c_acftPen = new Pen( Color.Black, 2 );
    private static readonly Brush c_markerBrush = new SolidBrush( Color.Gainsboro );

    private static readonly Pen c_tdPen = new Pen( Color.BlueViolet, 3 );
    private static readonly Brush c_tdBrush = new SolidBrush( c_tdPen.Color );

    private static readonly Font c_fontSS = new Font( "Segoe UI", 8f, FontStyle.Regular );
    private static readonly Font c_fontS = new Font( "Segoe UI", 14f, FontStyle.Regular );
    private static readonly Font c_font = new Font( "Segoe UI Semibold", 18f, FontStyle.Bold );
    private static readonly Font c_fontL = new Font( "Segoe UI Semibold", 21f, FontStyle.Regular );
    private static readonly Font c_fontM = new Font( "Consolas", 16.0f, FontStyle.Bold );

    // main portion Runway
    private static readonly Rectangle c_rwRect = new Rectangle( new Point( 200, 0 ), new Size( 300, (int)c_baseRect.Height ) );
    private static readonly Point c_rwLeftMid = new Point( c_rwRect.Left, c_rwRect.Height / 2 );
    private static readonly Point c_rwCenterMid = new Point( c_rwRect.Left + c_rwRect.Width / 2, c_rwRect.Height / 2 );


    // *** CLASS 

    private int _width = c_baseImage.Width;
    private int _height = c_baseImage.Height;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="lPerf">A Landing Performance record</param>
    /// <param name="width">Width (set 0 for native size)</param>
    /// <param name="height">Height (set 0 for native size)</param>
    public LandingImage( PerfTracker.PerfData lPerf, int width = 318, int height = 420 )
    {
      _lPerf = lPerf;
      if ((width <= 0) || (height <= 0)) {
        _width = c_baseImage.Width;
        _height = c_baseImage.Height;
      }
      else {
        _width = width;
        _height = height;
      }
    }

    /// <summary>
    /// Returns a Landing Performance Image
    ///  Caller must dispose the Bitmap when done
    /// </summary>
    /// <returns>A Bitmap</returns>
    public Image AsImage( )
    {
      Bitmap bmp = new Bitmap( Properties.Resources.Runway_top, new Size( _width, _height ) );
      Graphics g = Graphics.FromImage( bmp );

      // background image
      g.DrawImage(
        c_baseImage,
        new Rectangle( 0, 0, _width, _height ), // target Size
        c_baseRect, // source Rect
        GraphicsUnit.Pixel );

      // Draw on top
      LandingPaint( g );
      g.Dispose( );

      return bmp;
    }

    /// <summary>
    /// To Paint the LP with a Graphics context
    ///  simple for now, assumes a 420x420 canvas, else scaling is going wrong
    /// </summary>
    /// <param name="g">Graphics context</param>
    private void LandingPaint( Graphics g )
    {
      // sanity
      if (_lPerf.TdCount == 0) return;

      // Drawing is based on original size which is about 530x700
      // and then scaled to the destination size
      /*
       RW Bearing or GS Track is Upwards
       */
      var save = g.BeginContainer( );
      {
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        // to dest size
        g.ScaleTransform( _width / c_baseRect.Width, _height / c_baseRect.Height );

        // ** Origin is top left corner of the drawing area

        // markers on the left side of the RW
        DrawRunwayMarker( g );
        // TD mark and values
        DrawTouchdown( g );
        // Data items on the left side
        DrawDataText( g );
        // Date and acft on the right side
        DrawLegend( g );

        // ** Origin moved to the right to center Runway (still Top of drawing area)
        g.TranslateTransform( c_rwCenterMid.X, 0 );

        // Airport - Runway IDs if available
        if (_lPerf.AirportIdent != "n.a.") {
          g.DrawString( $"{_lPerf.AirportIdent}  {_lPerf.RunwayIdent}", c_fontL, Brushes.Black, 0, 20, c_stringFormat );
        }

        // Prepare for Aircraft track, icon etc.
        // Rotate to have track up (using true degrees for orientation)
        float baseOrientation = _lPerf.LandingPerf.TdGTRK_deg;
        if (!float.IsNaN( _lPerf.RunwayBearing_deg )) {
          // having a runway.. the rwBearing is upwards and
          // track is angled later
          baseOrientation = _lPerf.RunwayBearing_deg;
        }

        // compass rose above aircraft icon
        DrawCompass( g, baseOrientation );

        // ** Origin moved down to mid center of RW
        g.TranslateTransform( 0, c_rwCenterMid.Y );

        // GTRK
        DrawGTrack( g, baseOrientation );
        // acft icon
        DrawAircraft( g, baseOrientation );
        // wind arrow on aircraft
        DrawWindArrow( g, baseOrientation );

      }
      g.EndContainer( save );
    }

    // Draw Runway markings (+- according ICAO Annex 14 Chapter 5)
    private void DrawRunwayMarker( Graphics g )
    {
      // sanity
      if (float.IsNaN( _lPerf.RunwayLength_m ) || float.IsNaN( _lPerf.RunwayWidth_m )) return; // RW not defined

      // Marker position 
      var sMarker = g.BeginContainer( );
      {
        float scaleX = (c_rwRect.Width) / _lPerf.RunwayWidth_m;
        // draw max first 1000 m
        float scaleY = (c_rwRect.Height) / ((_lPerf.RunwayLength_m > c_rwLenDrawn_m) ? c_rwLenDrawn_m : _lPerf.RunwayLength_m);
        g.TranslateTransform( c_rwLeftMid.X, c_baseImage.Height ); // center start of RW
        g.ScaleTransform( 1, -1 ); // invert vert scale

        int x, y, w, h, dx;

        // Threshold
        int nStripes = (_lPerf.RunwayWidth_m >= 60) ? 16
          : (_lPerf.RunwayWidth_m >= 45) ? 12
          : (_lPerf.RunwayWidth_m >= 30) ? 8
          : (_lPerf.RunwayWidth_m >= 23) ? 6
          : 4;
        //nStripes /= 2; // draw only left side..
        h = 10; w = 10;
        x = 1; y = 0; dx = (c_rwRect.Width - w) / (nStripes - 1);
        for (int i = 0; i < nStripes; i++, x += dx) {
          g.FillRectangle( c_markerBrush, x, y, w, h );
        }

        dx = 20;
        if (_lPerf.RunwayLength_m < 900) {
          // aiming point
          x = 5; y = (int)(150 * scaleY);
          h = (int)(45 * scaleY); w = 20;
          g.FillRectangle( c_markerBrush, x, y, w, h );
        }

        else if (_lPerf.RunwayLength_m < 1200) {
          // 150m (500 ft)
          x = 5; y = (int)(150 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;

          // 250m aiming point
          x = 5; y = (int)(250 * scaleY);
          h = (int)(45 * scaleY); w = 20;
          g.FillRectangle( c_markerBrush, x, y, w, h );
        }

        else if (_lPerf.RunwayLength_m < 1500) {
          // 150m (500 ft)
          x = 5; y = (int)(150 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;

          // 250m aiming point
          x = 5; y = (int)(250 * scaleY);
          h = (int)(45 * scaleY); w = 25;
          g.FillRectangle( c_markerBrush, x, y, w, h );

          // 450m (1500 ft)
          x = 5; y = (int)(450 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
        }

        else if (_lPerf.RunwayLength_m < 2400) {
          // 150m (500 ft)
          x = 5; y = (int)(150 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;

          // 300m aiming point 
          x = 5; y = (int)(300 * scaleY);
          h = (int)(60 * scaleY); w = 30;
          g.FillRectangle( c_markerBrush, x, y, w, h );

          // 450m (1500 ft)
          x = 5; y = (int)(450 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          // 600m (2000 ft)
          x = 5; y = (int)(600 * scaleY);
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
        }

        else {
          // 150m (500 ft)
          x = 5; y = (int)(150 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          if (_lPerf.AirportIdent.StartsWith( "K" )
              || _lPerf.AirportIdent.StartsWith( "PA" )
              || _lPerf.AirportIdent.StartsWith( "PH" )) {
            // FAA like...
            // 306m aiming point
            x = 5; y = (int)(306 * scaleY);
            h = (int)(60 * scaleY); w = 40;
            g.FillRectangle( c_markerBrush, x, y, w, h );

            // 450m (1500 ft)
            x = 5; y = (int)(450 * scaleY);
            h = (int)(22.5f * scaleY); w = 8;
            g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
            g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          }
          else {
            // ICAO like...
            // 300m (1000 ft)
            x = 5; y = (int)(300 * scaleY);
            g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
            g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
            g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;

            // 400m aiming point
            x = 5; y = (int)(400 * scaleY);
            h = (int)(60 * scaleY); w = 40;
            g.FillRectangle( c_markerBrush, x, y, w, h );
          }

          // 600m (2000 ft)
          x = 5; y = (int)(600 * scaleY);
          h = (int)(22.5f * scaleY); w = 8;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          // 750m (2500 ft)
          x = 5; y = (int)(750 * scaleY);
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
          // 900m (3000 ft)
          x = 5; y = (int)(900 * scaleY);
          g.FillRectangle( c_markerBrush, x, y, w, h ); x += dx;
        }
      }
      g.EndContainer( sMarker );

    }

    private void DrawTouchdown( Graphics g )
    {
      // sanity
      if (float.IsNaN( _lPerf.RunwayLength_m ) || float.IsNaN( _lPerf.RunwayWidth_m )) return; // RW not defined

      // TD position scaled to the runway
      var sTdown = g.BeginContainer( );
      {
        // draw max first 1500 m
        float rwLenDrawn = (_lPerf.RunwayLength_m > c_rwLenDrawn_m) ? c_rwLenDrawn_m : _lPerf.RunwayLength_m;
        // rw drawn
        int x1 = 100, y = 10, dy = 20;
        g.DrawString( $"{rwLenDrawn:#,##0} m   →", c_fontS, c_txtBrush, x1, y, c_stringFormatText ); y += dy;
        g.DrawString( $"{Units.Ft_From_M( rwLenDrawn ):#,##0} ft", c_fontS, c_txtBrush, x1, y, c_stringFormatText );

        float scaleX = (c_rwRect.Width) / _lPerf.RunwayWidth_m;
        float scaleY = (c_rwRect.Height) / rwLenDrawn;
        // TD point is 0/0
        g.TranslateTransform( c_rwCenterMid.X + _lPerf.TdDisplacement_m * scaleX, c_baseImage.Height - (_lPerf.TdDistance_m * scaleY) );

        g.DrawLine( c_tdPen, -10, -10, 10, 10 );
        g.DrawLine( c_tdPen, -10, 10, 10, -10 );
      }
      g.EndContainer( sTdown );
    }

    // Wall of text on the left side
    private void DrawDataText( Graphics g )
    {
      var sText = g.BeginContainer( );
      {
        int y = 60, x1 = 5, x2 = x1 + 40, dy = 25;
        if (!(float.IsNaN( _lPerf.RunwayLength_m ) || float.IsNaN( _lPerf.RunwayWidth_m ))) {
          // RW defined
          g.DrawString( $"RW:", c_fontS, c_txtBrush, x1, y, c_stringFormatText );
          g.DrawString( $"{_lPerf.RunwayLength_m:##,##0}x{_lPerf.RunwayWidth_m:#0} m", c_fontS, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
          g.DrawString( $"{Units.Ft_From_M( _lPerf.RunwayLength_m ):##,##0}x{Units.Ft_From_M( _lPerf.RunwayWidth_m ):#0} ft", c_fontS, c_txtBrush, x2, y, c_stringFormatText );
        }

        // data portion
        y = 150; x1 = 5; x2 = 75; dy = 35;
        g.DrawString( $"Vert:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdRate_fpm:#,##0} fpm", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"GS:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdGS_kt:##0} kt", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"G's:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdGValue:0.00} G", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"Slip:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdSideslipAngle_deg:#0.0}°", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        // gap
        y += dy;
        g.DrawString( $"TD's:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{PerfTracker.Instance.TDCount:#0}", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        //        g.DrawString( $"{1:#0}", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy; // FOR DOC only TD=1
        // gap
        y += dy;
        g.DrawString( $"Pitch:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdPitch_deg:#0.0}°", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"Bank:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdBank_deg:#0.0}°", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"Yaw:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdYaw_deg:#0.0}°", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"HDG:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdHdg_degm:000}° M", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"TRK:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdGTRK_degm:000}° M", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;
        g.DrawString( $"IAS:", c_font, c_txtBrush, x1, y, c_stringFormatText );
        g.DrawString( $"{_lPerf.LandingPerf.TdIAS_kt:##0} kt", c_font, c_txtBrush, x2, y, c_stringFormatText ); y += dy;

        if (!float.IsNaN( _lPerf.TdDistance_m )) {
          y = c_baseImage.Height - 55; x1 = c_rwCenterMid.X; x2 = x1 + 40; dy = 25;
          g.DrawString( $"↑¦↑", c_fontM, c_tdBrush, x1, y, c_stringFormat );
          g.DrawString( $"{_lPerf.TdDistance_m:#,##0} m", c_font, c_tdBrush, x2, y, c_stringFormatText ); y += dy;
          // gap
          if (_lPerf.TdDisplacement_m > 0.5) {
            g.DrawString( $"_¦→", c_fontM, c_tdBrush, x1, y, c_stringFormat );
          }
          else if (_lPerf.TdDisplacement_m < -0.5) {
            g.DrawString( $"←¦_", c_fontM, c_tdBrush, x1, y, c_stringFormat );
          }
          else {
            g.DrawString( $"-¦-", c_fontM, c_tdBrush, x1, y, c_stringFormat );
          }
          g.DrawString( $"{_lPerf.TdDisplacement_m:##0.0} m", c_font, c_tdBrush, x2, y, c_stringFormatText ); y += dy;

        }
        // gap
        //          y += 5;
        //          g.DrawString( $"UTC:", font, c_txtBrush, x1, y, _stringFormatText );
        //          var ts = TimeSpan.FromSeconds( _lPerf.LandingPerf.TdSimUTC_sec );
        //          g.DrawString( $"{ts:hh\\:mm\\:ss}", font, c_txtBrush, x2, y, _stringFormatText ); y += 15;

      }
      g.EndContainer( sText );
    }

    private void DrawCompass( Graphics g, float baseAngle )
    {
      // compass icon
      var sCPas = g.BeginContainer( );
      {
        int y = 60;// text at Y
        g.DrawString( $"{baseAngle:000}°", c_font, Brushes.SaddleBrown, 0, y, c_stringFormat );
        y = 120;// rose center at Y
        g.TranslateTransform( 0, y );
        g.RotateTransform( -baseAngle );
        int yTop = -30;
        g.DrawLine( Pens.SaddleBrown, 0, -yTop, 0, yTop );
        g.DrawLine( Pens.SaddleBrown, 0, yTop, -9, yTop + 5 );
        g.DrawLine( Pens.SaddleBrown, 0, yTop, +9, yTop + 5 );
        g.ScaleTransform( 1.5f, 1.5f ); // upscale letter N
        g.DrawString( $"N", c_fontL, Brushes.SaddleBrown, 0, 0, c_stringFormat );
      }
      g.EndContainer( sCPas );
    }

    // ground track, assumes the center to be at the acft center
    private void DrawGTrack( Graphics g, float baseAngle )
    {
      var save = g.BeginContainer( );
      {
        // yaw is against GTRK, so get the GTRK vs RunwayBrg first
        g.RotateTransform( _lPerf.LandingPerf.TdGTRK_deg - baseAngle );
        int yTop = -180;
        g.DrawLine( c_trkPen, 0, 250, 0, yTop );
        g.DrawLine( c_trkPen, 0, yTop, -9, yTop + 15 );
        g.DrawLine( c_trkPen, 0, yTop, +9, yTop + 15 );
      }
      g.EndContainer( save );
    }

    // aircraft icon, assumes the center to be at the acft center
    private void DrawAircraft( Graphics g, float baseAngle )
    {
      // acft icon
      var save = g.BeginContainer( );
      {
        // yaw is against GTRK, so get the GTRK vs RunwayBrg first
        g.RotateTransform( _lPerf.LandingPerf.TdGTRK_deg - baseAngle );
        // then yaw
        g.RotateTransform( _lPerf.LandingPerf.TdYaw_deg );
        g.DrawLine( c_acftPen, 0, 100, 0, -100 );
        g.ScaleTransform( 0.15f, 0.15f );
        g.DrawImage( c_acftIcon, -c_acftIcon.Width / 2, -c_acftIcon.Height / 2 );
      }
      g.EndContainer( save );
    }

    // wind on aircraft, assumes the center to be at the acft center
    private void DrawWindArrow( Graphics g, float baseAngle )
    {
      if (_lPerf.LandingPerf.TdWindSpeed_kt >= 1) {
        var save = g.BeginContainer( );
        {
          // with regards to the base angle
          g.RotateTransform( -baseAngle );
          g.RotateTransform( _lPerf.LandingPerf.TdWindDir_deg );
          g.ScaleTransform( 3, 3 ); // wind arrow up to size
          g.TranslateTransform( 0, -22 );
          g.DrawLines( Pens.RoyalBlue, c_arrowShape );
          // text: invalidate the prev rot and move text box in place
          g.TranslateTransform( 0, -17 );
          g.RotateTransform( -_lPerf.LandingPerf.TdWindDir_deg );
          g.RotateTransform( baseAngle );

          // text
          g.ScaleTransform( 0.4f, 0.4f ); // scale wind text down from before
          g.DrawString( $"{_lPerf.LandingPerf.TdWindSpeed_kt:#0} kt", c_font, Brushes.RoyalBlue, 0, 0, c_stringFormat );
        }
        g.EndContainer( save );
      }
    }

    // legend on the right side
    private void DrawLegend( Graphics g )
    {
      // date+acft title
      var save = g.BeginContainer( );
      {
        g.TranslateTransform( c_baseRect.Right - 9, c_baseRect.Bottom - 5 );
        g.RotateTransform( -90 );
        // origin is start of right bar
        g.DrawString( $"Date: {_lPerf.LandingPerf.TdTimeStamp:G} / Acft: {_lPerf.LandingPerf.TdAcftTitle}", c_fontSS, Brushes.WhiteSmoke, 0, 0, c_stringFormatText );
        //g.DrawString( $"", c_fontS, Brushes.Wheat, 0, 0, c_stringFormatText );
      }
      g.EndContainer( save );
    }


  }
}

