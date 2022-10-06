using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Drawing
{

  /// <summary>
  /// Managed Hook  if not Active it will prevent the hooked items to be drawn
  /// </summary>
  internal class ManagedHookItem : HookItem
  {
    /// <summary>
    /// cTor: 
    /// </summary>
    public ManagedHookItem( )
      : base( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public ManagedHookItem( HookItem other )
      : base( other )
    {
    }


    /// <summary>
    /// Draw all of this item if Active
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="MapToPixel">Function which converts Coordinates to canvas pixels</param>
    public override void Paint( Graphics g, Func<CoordLib.LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      PaintThis( g, MapToPixel ); // paint this item
      SubItemList.Paint( g, MapToPixel ); // paint all below this item
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      return; // the Hook shall not be drawn
    }

  }
}
