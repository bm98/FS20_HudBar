using CoordLib.MercatorTiles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Graphic drawing processor
  /// </summary>
  internal class GProc
  {
    // start ID for anonymous DispIDs
    private static int _dispID = 10_000;

    // the DisplayList containing all items to draw in a Paint Event
    private DisplayList _drawList = new DisplayList( );

    // Improved text rendering as per:
    // https://stackoverflow.com/questions/2609520/how-to-make-text-labels-smooth
    private TextRenderingHint _hint = TextRenderingHint.ClearTypeGridFit;
    public TextRenderingHint TextRenderingHint {
      get { return this._hint; }
      set { this._hint = value; }
    }

    #region GProc API

    /// <summary>
    /// Returns an anonymous DispID (store it or dont refer to it..)
    /// </summary>
    /// <returns>A new DisplayItem Key</returns>
    public static int DispID_Anon( ) => _dispID++;

    /// <summary>
    /// This is the one and only Master DisplayList
    /// </summary>
    public DisplayList Drawings => _drawList;

    /// <summary>
    /// Does all paints 
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    public void Paint( Graphics g, IVPortPaint vpRef)
    {
      // protect the incomming GContext from all actions done in our world
      var save = g.BeginContainer( );
      {
        // setting some quality properties here
        g.TextRenderingHint = TextRenderingHint;
        // very expensive and not really needed
        // g.CompositingQuality = CompositingQuality.HighQuality; 
        foreach (var i in _drawList.Values) {
          i.Paint( g, vpRef);
        }
      }
      g.EndContainer( save );
    }

    #endregion
  }
}
