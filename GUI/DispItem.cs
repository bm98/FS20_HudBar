using FS20_HudBar.Bar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// A Display Item
  ///  a left to right orientated FlowPanel which carries a label and optionally some value controls
  ///  The DispItem is intended to be loaded into the main Hud Panel
  /// </summary>
  class DispItem : FlowLayoutPanel
  {
    // The Label (first item)
    private object m_label = null;

    /// <summary>
    /// Returns the Label of this group (first added control)
    /// </summary>
    public Control Label => (Control)m_label;
    /// <summary>
    /// Returns the Label of this group (first added control)
    /// </summary>
    public IColorType ColorType => (IColorType)m_label;

    /// <summary>
    /// cTor: Create an item
    /// </summary>
    public DispItem( )
    {
      this.FlowDirection = FlowDirection.LeftToRight;
      this.WrapContents = false;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
      this.Dock = DockStyle.Bottom;
      this.Cursor = Cursors.Default; // avoid the movement cross on the item controls
      this.BackColor = GUI_Colors.ItemColor(GUI_Colors.ColorType.cBG); // default (should be transparent - ex. for debugging)
      //this.BackColor = Color.Pink; // DEBUG color
    }

    /// <summary>
    /// Add an Item from Left to Right
    /// </summary>
    /// <param name="control"></param>
    public void AddItem( object control )
    {
      if ( !( control is Control ) ) return; // sanity
      if ( !( control is IColorType ) ) return; // sanity

      if ( this.Controls.Count == 0 ) {
        m_label = control as Control;
      }

      this.Controls.Add( control as Control );
    }

  }

}
