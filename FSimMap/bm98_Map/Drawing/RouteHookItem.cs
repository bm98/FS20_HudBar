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
  /// A ManagedHook containing a Route Segment Manager
  /// </summary>
  internal class RouteHookItem : ManagedHookItem
  {
    private RouteSegmentMgr _segmentMgr = new RouteSegmentMgr();

    /// <summary>
    /// A Route Segment Manager
    /// </summary>
    public RouteSegmentMgr SegmentMgr => _segmentMgr;

    /// <summary>
    /// cTor: 
    /// </summary>
    public RouteHookItem( )
      : base( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public RouteHookItem( RouteHookItem other )
      : base( other )
    {
    }


    /// <summary>
    /// Draw all of this item if Active
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="MapToPixel">Function which converts Coordinates to canvas pixels</param>
    public override void Paint( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn
      // reset the segment part
      _segmentMgr.Reset();
      SubItemList.Paint( g, MapToPixel ); // paint all below this item
      // can only paint after the sublist has been painted and segments have been collected
      PaintThis( g, MapToPixel ); // paint this item
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      _segmentMgr.DrawSegments( g );
    }

  }
}
