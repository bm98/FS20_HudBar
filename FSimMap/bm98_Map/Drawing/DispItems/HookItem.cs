﻿using System.Drawing;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// An invisible Hook for a substream of items
  /// </summary>
  internal class HookItem : DisplayItem
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
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g,IVPortPaint vpRef )
    {
      return; // the Hook shall not be drawn
    }



  }
}
