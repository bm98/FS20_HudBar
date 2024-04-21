using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// One tracking point of an aircraft
  /// </summary>
  internal class AcftTrackItem : DisplayItem
  {

    /// <summary>
    /// Flag to indicate the Acft is on ground
    /// </summary>
    public bool OnGround = false;

    /// <summary>
    /// cTor: 
    /// </summary>
    public AcftTrackItem( )
      : base( )
    {
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public AcftTrackItem( AcftTrackItem other )
      : base( other )
    {
    }

    /// <summary>
    /// Draw a tracking point
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          Brush brush = new SolidBrush( ColorScale.AltitudeColor( CoordPoint.Altitude, OnGround ) );
          g.FillEllipse( brush, mp.X - 3, mp.Y - 3, 6, 6 );
          brush.Dispose( );
        }
        else {
          // nothing to draw here
        }
      }
      g.EndContainer( save );
    }


  }
}
