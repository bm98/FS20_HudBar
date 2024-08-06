using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  /// <summary>
  /// Graphic drawing processor
  /// </summary>
  internal class GProc
  {

    private DisplayList m_drawList = new DisplayList( );

    #region Class GProc

    /// <summary>
    /// This is the one and only Master DisplayList
    /// </summary>
    public DisplayList Drawings { get => m_drawList; }

    /// <summary>
    /// Does all paints 
    /// </summary>
    /// <param name="g"></param>
    public void Paint( Graphics g )
    {
      foreach ( var i in m_drawList.Values ) {
        i.Paint( g );
      }
    }

    #endregion
  }
}
