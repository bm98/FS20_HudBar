using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using bm98_Map.Data;

namespace bm98_Map.Drawing.DispItems
{

  /// <summary>
  /// AI Aircraft Icon Drawing
  ///  use String for deco string below the Icon
  /// </summary>
  internal class AircraftAiItem : DisplayItem
  {

    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

    /// <summary>
    /// The AI Aircrafts ID to identify and manage the Item on updates
    /// </summary>
    public string AircraftID { get; set; } = "";
    /// <summary>
    /// Flag used to manage the item while updating
    /// </summary>
    public bool Alive { get; set; } = false;
    /// <summary>
    /// Indicator for the altitude separation display mode
    /// </summary>
    public TcasFlag TCAS { get; set; } = TcasFlag.Level;
    /// <summary>
    /// True if it is a Helicopter
    /// </summary>
    public bool IsHeli { get; set; } = false;
    /// <summary>
    /// The Acft Heading North Up
    /// </summary>
    public float Heading { get; set; } = 0;
    /// <summary>
    /// Flag to indicate the Acft is on ground
    /// </summary>
    public bool OnGround = false;

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public AircraftAiItem( Bitmap spriteRef )
    {
      _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AircraftAiItem( AircraftAiItem other )
      : base( other )
    {
      _imageRef = other._imageRef; // ref image
      Heading = other.Heading;
      StringFormat = other.StringFormat.Clone( ) as StringFormat; ;
    }

    // reference is pointing upwards, 0/0 is center of the acft- will be scaled later
    // an aircraft shape pointing upwards
    private static readonly Point[] c_acftShape = new Point[] {
      new Point(0,-10), // top Y=-10
      new Point(-2,-7),
      new Point(-2,-3),
      new Point(-10,3),
      new Point(-9,4),
      new Point(-2,2),
      new Point(-2,7),
      new Point(-5,9),
      new Point(-4,10),
      new Point(0,9),
      new Point(4,10),
      new Point(5,9),
      new Point(2,7),
      new Point(2,2),
      new Point(9,4),
      new Point(10,3),
      new Point(2,-3),
      new Point(2,-7),
     // new Point(0,-10), // looks better if the GP closes the curve..
    };
    // reference is pointing upwards, 0/0 is center of the acft- will be scaled later
    // pack part of heli shape pointing upwards
    private static readonly Point[] c_heliShape = new Point[] {
      new Point(-1,3),
      new Point(-1,7),
      new Point(-5,9),
      new Point(-4,10),
      new Point(0,9),
      new Point(4,10),
      new Point(5,9),
      new Point(1,7),
      new Point(1,3),
    };
    private float _tension = 0.1f;
    private float _scale = 2f;

    // a triangle (A) shape
    private static readonly Point[] c_aboveShape = new Point[] {
      new Point(0,-4), // Top,Left Y=-4
      new Point(-4,4),
      new Point(4,4),
    };

    // a square shape
    private static readonly Point[] c_levelShape = new Point[] {
      new Point(-4,-4), // Top,Left Y=-4
      new Point(-4,4),
      new Point(4,4),
      new Point(4,-4),
    };

    private void D_Heli( Graphics g, Brush brush )
    {
      g.FillClosedCurve( brush, c_heliShape, FillMode.Winding, _tension );
      g.DrawClosedCurve( Pen, c_heliShape, _tension, FillMode.Winding );
      g.FillEllipse( brush, -3, -9, 6, 13 );
      g.DrawEllipse( Pen, -3, -9, 6, 13 );
      using (Pen pen = new Pen( Pen.Color, 2 )) {
        g.DrawLine( pen, -6, -10, 6, 3 );
        g.DrawLine( pen, 6, -10, -6, 3 );
      }

    }
    private void D_Acft( Graphics g, Brush brush )
    {
      g.FillClosedCurve( brush, c_acftShape, FillMode.Winding, _tension );
      g.DrawClosedCurve( Pen, c_acftShape, _tension, FillMode.Winding );
    }

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
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          var saveAcft = g.BeginContainer( );
          { // container bracket
            // acft pos to 0/0 to rotate, scale and the draw symmetrically
            g.RotateTransform( Heading, MatrixOrder.Append );
            g.TranslateTransform( mp.X, mp.Y, MatrixOrder.Append );

            using (Brush brush = new SolidBrush( ColorScale.AltitudeColor( CoordPoint.Altitude, OnGround ) )) {
              if (OnGround || (TCAS == TcasFlag.ProximityLevel)) {
                // show acft shape
                g.ScaleTransform( _scale, _scale ); // up to size
                if (IsHeli) {
                  D_Heli( g, brush );
                }
                else {
                  D_Acft( g, brush );
                }
              }
              else if (TCAS == TcasFlag.Level) {
                // Level outside uses a box shape
                g.ScaleTransform( _scale, _scale ); // up to size
                g.FillClosedCurve( brush, c_levelShape, FillMode.Winding, _tension );
                g.DrawClosedCurve( Pen, c_levelShape, _tension, FillMode.Winding );
                g.DrawLine( Pen, 0, 4, 0, 18 ); // towards the bottom
              }
              else if (TCAS == TcasFlag.Above) {
                // Above uses a triangle shape
                g.ScaleTransform( _scale, _scale ); // up to size
                g.FillClosedCurve( brush, c_aboveShape, FillMode.Winding, _tension );
                g.DrawClosedCurve( Pen, c_aboveShape, _tension, FillMode.Winding );
                g.DrawLine( Pen, 0, 4, 0, 18 ); // towards the bottom
              }
              else {
                // Below uses a circular shape
                g.ScaleTransform( _scale, _scale ); // up to size
                g.FillEllipse( brush, -4, -4, 8, 8 );
                g.DrawEllipse( Pen, -4, -4, 8, 8 );
                g.DrawLine( Pen, 0, 4, 0, 18 ); // towards the bottom
              }
            }
          }
          g.EndContainer( saveAcft );

          // need to turn for the label in radar mode else the angle is 0 anyway
          TurnAroundPoint( g, mp, vpRef.MapHeading );
          g.TranslateTransform( mp.X, mp.Y + 60 );

          if (TextRectFillBrush != null) {
            var stringSize = g.MeasureString( String, Font );
            var rectangle = new RectangleF( new PointF( -stringSize.Width / 2f, 0 ), stringSize );
            g.FillRectangle( TextRectFillBrush, rectangle );
          }
          g.DrawString( String, Font, TextBrush, 0, 0, StringFormat );
        }
        else {
          // not on map at current zoom
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
                           // either of the two will be used finally...
                           //PaintThisSprite( g, MapToPixel );
      PaintThisShape( g, vpRef );
    }



  }
}
