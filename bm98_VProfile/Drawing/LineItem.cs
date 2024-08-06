using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  /// <summary>
  /// Implements a LineItem
  /// Draws a line at angle with properties
  /// Use StringFormat.LineAlignment to place the starting Y of the Line (Top[Near], Mid[Center], Bottom[Far]) 
  /// </summary>
  internal class LineItem : DisplayItem
  {
    /// <summary>
    /// Normalized Startpoint (coord values = 0..1)
    /// </summary>
    public PointF StartPoint { get; set; }
    /// <summary>
    /// Normalized Endpoint  (coord values = 0..1)
    /// </summary>
    public PointF EndPoint { get; set; }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public LineItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public LineItem( LineItem other )
      : base( other )
    {
      this.StartPoint = other.StartPoint;
      this.EndPoint = other.EndPoint;
    }

    /// <summary>
    /// Get a clone of this TextItem
    /// </summary>
    /// <returns></returns>
    public virtual LineItem Clone( )
    {
      return new LineItem( this );
    }

    protected override void PaintThis( Graphics g )
    {

      if (Active == ActiveState.Engaged) {

        var ctxSave = g.Save( );
        g.SetClip( Rectangle, CombineMode.Replace );

        float Xs = Rectangle.Left + (StartPoint.X * Rectangle.Width);
        float Ys = Rectangle.Top + (StartPoint.Y * Rectangle.Height);

        float Xe = Rectangle.Left + (EndPoint.X * Rectangle.Width);
        float Ye = Rectangle.Top + (EndPoint.Y * Rectangle.Height);

        g.DrawLine( Pen, Xs, Ys, Xe, Ye );

        g.Restore( ctxSave );
      }
    }


  }
}
