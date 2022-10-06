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
  /// An invisible Hook for a substream of items
  /// </summary>
  internal class HookItem:DisplayItem
  {

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public HookItem( )
    {
      Active = false;
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public HookItem( HookItem other )
      : base( other )
    {
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
