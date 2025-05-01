using System.Drawing;

namespace bm98_Map.Drawing.DispItems
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
    public ManagedHookItem( ManagedHookItem other )
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

      PaintThis( g, vpRef ); // paint this item
      SubItemList.Paint( g, vpRef ); // paint all below this item
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      return; // the Hook shall not be drawn
    }

  }
}
