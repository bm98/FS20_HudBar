using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Bar.Items.Base
{
  /// <summary>
  /// Display Panel - A FlowPanel which contains all DispItems
  /// </summary>
  class DispPanel : FlowLayoutPanel
  {
    /// <summary>
    /// cTor: 
    /// </summary>
    public DispPanel( )
    {
      // Init
      this.CausesValidation = false;
      this.FlowDirection = FlowDirection.LeftToRight;
      this.Size = new Size( 5, 5 );
      this.Location = new Point( 0, 0 );
      this.WrapContents = true;
      this.AutoSizeMode = AutoSizeMode.GrowOnly;
      this.AutoSize = true;
      this.TabStop = false;
    }

    /// <summary>
    /// Currently handled Number of engines
    /// </summary>
    public int NumEngines => _numEngines;

    /// <summary>
    /// Empty the Panel
    /// </summary>
    public void ClearPanel( )
    {
      // reset management vars when reloaded
      _numEngines = -1;

      // clear the controls with Dispose
      this.Controls.Clear( true );
    }

    #region Manage Engine Visibility

    // track this number - can be 0..4 or -1 when initialized or reset
    private int _numEngines = -1;


    /// <summary>
    /// Setting the Engines in one Go to avoid all the flicker when set by each DispItem
    /// </summary>
    /// <param name="numEngines">Num Engines to show, <0 to reset the manager</param>
    public void SetEnginesVisible( int numEngines )
    {
      if (numEngines == _numEngines) return; // we are already there..
      // check for a forced Reset 
      if (numEngines < 0) {
        _numEngines = -1;
        return;
      }

      // Engine dependent items are Tagged with 'EngineItem' and will contain 4 Value fields (Guidance!!)
      this.SuspendLayout( );
      foreach (var di in this.Controls) {
        if (!(di is DispItem)) continue; // can only handle DispItems
        var dix = di as DispItem;

        if (dix.IsDisposed) continue; // Sanity, would be a program error
        if (!dix.IsEngineItem) continue; // not to be handled

        dix.SetValuesVisible( numEngines );
      }
      // remember
      _numEngines = numEngines;

      this.ResumeLayout( );
    }

    #endregion

  }

}
