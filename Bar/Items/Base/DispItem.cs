using FS20_HudBar.Bar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items.Base
{



  /// <summary>
  /// A Display Item
  ///  a left to right orientated FlowPanel which carries a label and optionally some value controls
  ///  The DispItem is intended to be loaded into the main Hud Panel
  /// </summary>
  class DispItem : FlowLayoutPanel
  {
    // Implement Each DispItem as derived Control DI_LITEM 
    // In the implementation define the Label, 
    // Define the Value item and add it to the Value Label list to later change properties when data arrives
    // then use the desired formatter label Control (V_xy) set specifics and add it to the display group and access lists
    // for 2 engine items define the second value item and control (same procedure as with the first one)
    //  - we use value2Proto to get a smaller font usually for 2+ values are to be shown for an item
    // Colors are used from default or set here explicitely 

    /// <summary>
    /// The Label ID 
    /// </summary>
    public LItem LabelID { get; protected set; }

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
    /// ToolTip Text 
    /// If set it will be attached to the Label Control of the DisplayItem
    /// </summary>
    public string TText = null;

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
     // this.BackColor = Color.SteelBlue; // DEBUG color
    }

    /// <summary>
    /// Add an Item from Left to Right
    /// </summary>
    /// <param name="control"></param>
    public void AddItem( object control )
    {
      if ( !( control is Control ) ) throw new ArgumentException("Argument must be of type Control"); // sanity
      if ( !( control is IColorType ) ) throw new ArgumentException( "Argument must implement IColorType" ); ; // sanity

      if ( this.Controls.Count == 0 ) {
        m_label = control as Control;
      }

      this.Controls.Add( control as Control );
    }

  }

}
