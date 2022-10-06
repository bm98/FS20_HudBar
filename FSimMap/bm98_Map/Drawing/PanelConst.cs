using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Panel Geometries
  /// </summary>
  internal static class PanelConst
  {
    // Enum is also Key in the Display List - so start them from disctinct bases
    public const int AptBaseEnum = 1000;

    /* FROM OLD DESIGN - if such const are used get them here back again 
     * 
     * 
    /// <summary>
    /// The panel painting Area relative to the UserControl
    /// We have relative sizes based on the original pictures
    /// </summary>

    public readonly static Rectangle PanelArea = new Rectangle( 0, 0, 929, 1000 ); // 

    // useful points
    public readonly static Point PanelAreaTopLeft = PanelArea.Location; // top left of all painting
    public readonly static Point PanelAreaTopRight = new Point( PanelArea.Right, PanelArea.Top );
    public readonly static Point PanelAreaBottomLeft = new Point( PanelArea.Left, PanelArea.Bottom );
    public readonly static Point PanelAreaBottomRight = new Point( PanelArea.Right, PanelArea.Bottom );

    public readonly static Point TurnCenter = new Point( 455, 390 );

    public readonly static Rectangle TxAptInfo = new Rectangle( 2, 5, 500, 30 ); // Airport Info

    // Runway Info
    // Left Column 1 .. 8 (all have the same X of item01, height and width)
    // Right Column 9 .. 16 (all have the same X of item01, height and width)
    private readonly static int txRWWidth = 850;
    private readonly static int txRWHeight = 30;

    private readonly static int txRWRow = TxAptInfo.Y+TxAptInfo.Height;
    private readonly static int txRWCol1 = 2;
    private readonly static int txRWCol2 = 522;
    private readonly static int txRWLine = 24;
    public readonly static Rectangle[] TxRW = new Rectangle[] {
       new Rectangle(  txRWCol1, txRWRow+0*txRWLine, txRWWidth, txRWHeight ), // 1
       new Rectangle(  txRWCol1, txRWRow+1*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+2*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+3*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+4*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+5*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+6*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol1, txRWRow+7*txRWLine, txRWWidth, txRWHeight ), // 8

       new Rectangle(  txRWCol2, txRWRow+0*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+1*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+2*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+3*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+4*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+5*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+6*txRWLine, txRWWidth, txRWHeight ),
       new Rectangle(  txRWCol2, txRWRow+7*txRWLine, txRWWidth, txRWHeight )  // 16
      };

    // Nav Info
    // Left Column 1 .. 8 (all have the same X of item01, height and width)
    // Right Column 9 .. 16 (all have the same X of item01, height and width)
    private readonly static int txNavWidth = 500;
    private readonly static int txNavHeight = 30;

    private readonly static int txNavRow = 520;
    private readonly static int txNavCol1 = 2;
    private readonly static int txNavCol2 = 522;
    private readonly static int txNavLine = 30;
    public readonly static Rectangle[] TxNav = new Rectangle[] {
       new Rectangle(  txNavCol1, txNavRow+0*txNavLine, txNavWidth, txNavHeight ), // 1
       new Rectangle(  txNavCol1, txNavRow+1*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+2*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+3*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+4*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+5*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+6*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol1, txNavRow+7*txNavLine, txNavWidth, txNavHeight ), // 8

       new Rectangle(  txNavCol2, txNavRow+0*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+1*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+2*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+3*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+4*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+5*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+6*txNavLine, txNavWidth, txNavHeight ),
       new Rectangle(  txNavCol2, txNavRow+7*txNavLine, txNavWidth, txNavHeight )  // 16
      };

    */

  }
}
