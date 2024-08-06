using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  /// <summary>
  /// Implements a WypItem (Waypoint Mark and Text)
  /// paints the text and if Active and a Fill is given onto a background
  /// </summary>
  internal class WypItem : DisplayItem
  {
    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

    /// <summary>
    /// Waypoint location (dist, alt) in normalized units 0..1
    /// </summary>
    public PointF WypPoint { get; set; }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public WypItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public WypItem( WypItem other )
      : base( other )
    {
      this.StringFormat = other.StringFormat.Clone( ) as StringFormat;
      WypPoint = other.WypPoint;
    }

    /// <summary>
    /// Get a clone of this TextItem
    /// </summary>
    /// <returns></returns>
    public virtual WypItem Clone( )
    {
      return new WypItem( this );
    }


    private static Point[] _wyp = new Point[] {
      new Point( -5, -5 ),
      new Point( 5, 5 ),
      new Point( 0, 0 ),
      new Point( 0, 0 ),
      new Point( 0, 0 ),
    };

    protected override void PaintThis( Graphics g )
    {

      if (Active == ActiveState.Engaged) {

        var ctxSave = g.Save( );
        //g.SetClip( Rectangle, CombineMode.Replace );

        float Xs = Rectangle.Left + (WypPoint.X * Rectangle.Width);
        float Ys = Rectangle.Top + (WypPoint.Y * Rectangle.Height);

        g.TranslateTransform( Xs, Ys );

        g.FillEllipse( FillBrush, new Rectangle( -5, -5, 10, 10 ) );
        //g.DrawLines( Pen, _wyp );

        var textRect = new Rectangle( -40, 5, 80, 50 ); // currently at Loc WYP
        textRect.Y = WypPoint.Y > 0.7 ? -5 - textRect.Height : textRect.Y; // on top of the label if it is at the bottom 
        textRect.X = (Xs-Rectangle.Left)<textRect.Width/2?textRect.X+textRect.Width/2 :textRect.X; // shift right when clipped left

        // draw label background if active and there is a FillBrush
        if (Active == ActiveState.Alert && BackgBrushAlarm != null) {
          g.FillRectangle( BackgBrushAlarm, textRect );
          if (!string.IsNullOrEmpty( String )) {
            g.DrawString( String, Font, TextBrushAlert, textRect, StringFormat );
          }
        }
        else if (Active == ActiveState.Warn && BackgBrushWarn != null) {
          g.FillRectangle( BackgBrushWarn, Rectangle );
          if (!string.IsNullOrEmpty( String )) {
            g.DrawString( String, Font, TextBrushWarn, textRect, StringFormat );
          }
        }
        // draw label if string is not empty
        else if (!string.IsNullOrEmpty( String )) {
          if ((TextBrushActive != null) && Active == ActiveState.Engaged) {
            g.DrawString( String, Font, TextBrushActive, textRect, StringFormat );
          }
          else if ((TextBrushArmed != null) && Active == ActiveState.Armed) {
            g.DrawString( String, Font, TextBrushArmed, textRect, StringFormat );
          }
        }

        g.Restore( ctxSave );

      }
    }

  }
}
