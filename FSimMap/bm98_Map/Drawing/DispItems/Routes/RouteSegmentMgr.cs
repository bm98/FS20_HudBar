using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XPoint;

namespace bm98_Map.Drawing.DispItems.Routes
{
  /// <summary>
  /// Manages Route Segments
  ///  Adding a point with a Pen will add to the current segment if the Pen is the same
  ///  A different Pen triggers a new segment
  /// </summary>
  internal class RouteSegmentMgr
  {
    private readonly float c_maxDist = 75f;    // max dist between points

    private readonly float _tension = 0.40f; // Curve tension to draw
    private List<RouteSegment> _segments = new List<RouteSegment>( );

    private RouteSegment _currSegment = null;
    private Pen _currPenRef = null;
    private Point _prevPoint = new Point( 0, 0 );

    /// <summary>
    /// Add a Point to draw
    /// </summary>
    /// <param name="pen">A Pen</param>
    /// <param name="point">A Point</param>
    public void AddPoint( Pen pen, Point point )
    {
      if (!pen.Equals( _currPenRef )) {
        // Pen change = new segment
        _currPenRef = pen;
        _currSegment = new RouteSegment( _currPenRef, _tension );
        _prevPoint = point;
        _segments.Add( _currSegment );
      }
      // we want point distances shorter then N pix
      float dist = _prevPoint.Distance( point );
      if (dist > c_maxDist) {
        Size v = _prevPoint.ToSize( point ); // vector from prev to point
        int iter = (int)Math.Ceiling( dist / c_maxDist ); // number of steps
        int dx = v.Width / iter;
        int dy = v.Height / iter;
        // add all but the last one
        for (int i = 1; i < iter; i++) {
          _currSegment.AddPoint( _prevPoint.Add( dx * i, dy * i ) );
        }
      }

      _currSegment.AddPoint( point );
      _prevPoint = point;
    }

    /// <summary>
    /// Reset the Manager 
    /// </summary>
    public void Reset( )
    {
      _segments.Clear( );
      _currSegment = null;
      _currPenRef = null;
    }

    /// <summary>
    /// Draw the collected segments
    /// </summary>
    /// <param name="g">A Graphics context</param>
    public void DrawSegments( Graphics g )
    {
      // sanity
      if (_segments.Count == 0) return;

      foreach (var item in _segments) {
        item.DrawSegment( g );
      }
    }

  }
}
