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
  /// Aircraft Icon Drawing
  /// </summary>
  internal class AircraftItem : DisplayItem
  {

    private Bitmap _imageRef;
    private Size _shift;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };


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
    public AircraftItem( Bitmap spriteRef )
    {
      _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AircraftItem( AircraftItem other )
      : base( other )
    {
      _imageRef = other._imageRef; // ref image
      Heading = other.Heading;
      StringFormat = other.StringFormat.Clone( ) as StringFormat; ;
    }

    // an aircraft shape pointing upwards, 0/0 is center of the acft- will be scaled later
    private Point[] _acftShape = new Point[] {
      new Point(0,-10),
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
    private float _tension = 0.1f;
    private float _scale = 3f;

    /// <summary>
    /// Draw an aircraft filled with the AltColor Paint (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected void PaintThisShape( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          // acft pos to 0/0 to rotate, scale and the draw symmetrically
          _shift = new Size( mp.X, mp.Y );
          g.RotateTransform( Heading, MatrixOrder.Append );
          g.TranslateTransform( mp.X, mp.Y, MatrixOrder.Append );
          g.ScaleTransform( _scale, _scale ); // up to size
          Brush brush = new SolidBrush( ColorScale.AltitudeColor( CoordPoint.Altitude, OnGround ) );
          g.FillClosedCurve( brush, _acftShape ,FillMode.Winding, _tension );
          g.DrawClosedCurve( Pen, _acftShape, _tension, FillMode.Winding );
          brush.Dispose( );
        }
        else {
          g.DrawString( "Aircraft out of sight",
            Font, TextBrush,
            g.VisibleClipBounds.X + g.VisibleClipBounds.Width / 2, g.VisibleClipBounds.Y + g.VisibleClipBounds.Height / 2,
            StringFormat
          );
        }
      }
      g.EndContainer( save );
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected void PaintThisSprite( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          var rect = new Rectangle( mp, _imageRef.Size );
          g.TranslateTransform( -_imageRef.Width / 2, -_imageRef.Height / 2 );

          _shift = new Size( mp.X, mp.Y );
          g.TranslateTransform( -_shift.Width, -_shift.Height );
          g.RotateTransform( Heading, MatrixOrder.Append );
          g.TranslateTransform( _shift.Width, _shift.Height, MatrixOrder.Append );
          g.DrawImage( _imageRef, rect );
        }
        else {
          g.DrawString( "Aircraft out of sight",
            Font, TextBrush,
            g.VisibleClipBounds.X + g.VisibleClipBounds.Width / 2, g.VisibleClipBounds.Y + g.VisibleClipBounds.Height / 2,
            StringFormat
          );
        }
      }
      g.EndContainer( save );
    }


    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn
      // either of the two will be used finally...
      //PaintThisSprite( g, MapToPixel );
      PaintThisShape( g, MapToPixel );
    }



  }
}
