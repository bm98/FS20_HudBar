using System;
using System.Drawing;

using bm98_Map.Drawing.DispItems.Routes;

namespace bm98_Map.Drawing.DispItems
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
    /// <param name="vpRef">Viewport access for paint events</param>
    public override void Paint( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn
      // reset the segment part
      _segmentMgr.Reset();
      SubItemList.Paint( g, vpRef ); // paint all below this item
      // can only paint after the sublist has been painted and segments have been collected
      PaintThis( g, vpRef); // paint this item
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      _segmentMgr.DrawSegments( g );
    }

  }
}
