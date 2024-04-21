using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XPoint;
using CoordLib;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Rectangle drawing 
  /// </summary>
  internal class RunwayItem : DisplayItem
  {
    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    /// <summary>
    /// Set true if it is a Water runway
    /// </summary>
    public bool IsWater { get; set; }

    /// <summary>
    /// Rwy Primary End Location
    /// </summary>
    public LatLon Start { get; set; }
    /// <summary>
    /// Ryw Primary End ID (NND or text like SOUTH, SOUTHWEST etc usually for Water)
    /// </summary>
    public string StartID { get; set; }

    /// <summary>
    /// Rwy Secondary End Location
    /// </summary>
    public LatLon End { get; set; }
    /// <summary>
    /// Ryw Secondary End ID (NND or text like SOUTH, SOUTHWEST etc usually for Water)
    /// </summary>
    public string EndID { get; set; }

    /// <summary>
    /// Rwy Length in meter
    /// </summary>
    public float Lenght { get; set; }
    /// <summary>
    /// Rwy Width in meter
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public RunwayItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public RunwayItem( RunwayItem other )
      : base( other )
    {
      this.StringFormat = other.StringFormat.Clone( ) as StringFormat;
      this.Start = other.Start;
      this.StartID = other.StartID;
      this.End = other.End;
      this.EndID = other.EndID;
      this.Lenght = other.Lenght;
      this.Width = other.Width;
      this.IsWater = other.IsWater;
    }

    /// <summary>
    /// Draw a rectangle 
    ///   with fill  (FillBrush set)
    ///   and  frame (Pen set)
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (Lenght == 0) return; // NOT SET, avoid Div0
      if (!Active) return; // shall not be drawn
      Point start_px = vpRef.MapToCanvasPixel( Start );
      Point end_px = vpRef.MapToCanvasPixel( End );
      float rwyLen_px = start_px.Distance( end_px ); // pix length of the Runway
      // SQRT of Length() to get NaN ?? still happens ??
      if (float.IsNaN( rwyLen_px )) {
        return;
      }
      rwyLen_px = (rwyLen_px < 1f) ? 1f : rwyLen_px;
      float rwyWidth_px = rwyLen_px / Lenght * Width;   // pix width of the Runway
      rwyWidth_px = (rwyWidth_px < 1f) ? 1f : rwyWidth_px;
      // SQRT of Length() to get NaN ?? still happens ??
      if (float.IsNaN( rwyWidth_px )) {
        return;
      }

      // write the IDs 

      // calculate the drawing angle of the runway endpoint to place the Numbers (Headings are not matching the drawn items due to mag var)
      // angle between two vectors (where (x2|y2) is set as unity vector pointing North)
      //var x2 = 0;
      //var y2 = 1;
      //dot = x1*x2 + y1*y2  
      //det = x1*y2 - y1*x2
      //angle = atan2(det, dot) // radians
      var startAngle = (Math.Atan2( end_px.X - start_px.X, -(end_px.Y - start_px.Y) ) / Math.PI * 180.0); // drawing coords are Y-upside down

      RectangleF textRect = new RectangleF {
        Location = new PointF( -60, rwyLen_px / 2.0f ), // X=> 1/2 Box Width (set below)
        Size = new SizeF( 120, 50 ) // box size
      };
      // Runway center coord
      var rMid = new Point( start_px.X + (int)((end_px.X - start_px.X) / 2.0), start_px.Y + (int)((end_px.Y - start_px.Y) / 2.0) );

      if (Font != null) {
        var save = g.BeginContainer( );
        {
          // Font is set - draw IDs
          g.TranslateTransform( rMid.X, rMid.Y ); // center runway at 0|0
          g.RotateTransform( (float)startAngle ); // rotate to point upwards
          if (TextRectFillBrush != null)
            g.FillRectangle( TextRectFillBrush, textRect ); // rect contains the text box offset from runway center
                                                            // g.DrawRectangle( TextRectPen, Rectangle.Round(textRect) );
          if (IsWater) {
            g.DrawString( StartID, Font, TextBrush, textRect, StringFormat );
          }
          else {
            // force split NND into NN\nD to stay readable
            g.DrawString( StartID.Insert( 2, "\n" ), Font, TextBrush, textRect, StringFormat );
          }
        }
        g.EndContainer( save );

        save = g.BeginContainer( );
        {
          g.TranslateTransform( rMid.X, rMid.Y );
          g.RotateTransform( (float)Geo.Wrap360( startAngle + 180 ) );
          if (TextRectFillBrush != null)
            g.FillRectangle( TextRectFillBrush, textRect );
          // g.DrawRectangle( TextRectPen, Rectangle.Round( textRect ) ); // back fill for text box
          if (IsWater) {
            g.DrawString( EndID, Font, TextBrush, textRect, StringFormat );
          }
          else {
            // force split NND into NN\nD to stay readable
            g.DrawString( EndID.Insert( 2, "\n" ), Font, TextBrush, textRect, StringFormat );
          }
        }
        g.EndContainer( save );
      }

      var save2 = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw Runway last to overdraw pot ID boxes
        var pen = new Pen( FillBrushAlt, rwyWidth_px + 4 ); // draw with a Runway Width Brush
        g.DrawLine( pen, start_px, end_px );
        pen.Dispose( );
        pen = new Pen( FillBrush, rwyWidth_px ); // draw with a Runway Width Brush
        g.DrawLine( pen, start_px, end_px );
        pen.Dispose( );
        g.EndContainer( save2 );
      }

    }

  }
}
