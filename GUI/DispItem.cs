using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class DispItem : FlowLayoutPanel
  {

    public DispItem( )
    {
      this.FlowDirection = FlowDirection.LeftToRight;
      this.WrapContents = false;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
      this.Dock = DockStyle.Bottom;
      this.Cursor = Cursors.Default; // avoid the movement cross on the item controls
      //this.BackColor = Color.Pink; // DEBUG color
    }

    /// <summary>
    /// Add a Control from Left to Right
    /// </summary>
    /// <param name="control"></param>
    public void AddItem( Control control )
    {
      this.Controls.Add( control );
    }

  }
}
