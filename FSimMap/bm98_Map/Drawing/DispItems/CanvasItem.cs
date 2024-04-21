using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

using MapLib;
using MapLib.Tiles;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// Canvas drawing 
  /// assuming the Center is the Viewport Center
  /// </summary>
  internal class CanvasItem : DisplayItem
  {
    /// <summary>
    /// The Canvas Image (the matrix is not managed or disposed here)
    /// </summary>
    public TileMatrix MapTilesRef { get; set; }
    // Flag to draw TileBorders
    public bool TileBorders { get; set; } = false;
    public float AcftHeading { get; set; } = 0f;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public CanvasItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public CanvasItem( CanvasItem other )
      : base( other )
    {
      this.MapTilesRef = other.MapTilesRef; // REF ONLY (no copy)
      this.TileBorders = other.TileBorders;
    }

    /// <summary>
    /// Draw the Canvas item
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      // just draw the image full scale to the surface
      var save = g.Save( );
      
      MapTilesRef.DrawMatrixImage( g, TileBorders );

      g.Restore( save );
    }

  }
}
