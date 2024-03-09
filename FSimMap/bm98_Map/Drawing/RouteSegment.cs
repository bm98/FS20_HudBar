using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// A Route segment
  ///  uses the same Pen and draws the Points as Curve
  /// </summary>
  internal class RouteSegment
  {
    private readonly float _tension = 0;

    private List<Point> _points = new List<Point>( );

    private Pen _penRef;

    /// <summary>
    /// Create a new RouteSegment with a Pen
    /// Optional tension for the curve is 0 (equals to drawing line segments)
    ///   use 1.0 to draw a smooth line
    /// </summary>
    /// <param name="pen">A Pen to draw the segment with</param>
    /// <param name="tension">A tension factor (defaults to 0)</param>
    public RouteSegment( Pen pen, float tension = 0 )
    {
      _penRef = pen;
      _tension = tension;
    }

    /// <summary>
    /// Add a Point to draw
    /// </summary>
    /// <param name="point">A Point</param>
    public void AddPoint( Point point )
    {
      if ((_points.Count > 0) && point == _points.Last( )) return; // don't add the same points TODO check if .Last() is too expensive

      _points.Add( point );
    }

    /// <summary>
    /// Draw the collected points as Curve
    /// Note: draws point with their given coordinates - setup everything before calling this one
    /// </summary>
    /// <param name="g">A Graphics context</param>
    public void DrawSegment( Graphics g )
    {
      // sanity
      if (_penRef == null) return;
      if (_points.Count < 2) return;

      if (_points.Count < 3) {
        // just a line
        g.DrawLine( _penRef, _points[0], _points[1] );
      }
      else {
        g.DrawCurve( _penRef, _points.ToArray( ), _tension );
      }
    }

  }
}
