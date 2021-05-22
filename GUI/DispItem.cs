using System;
using System.Collections.Generic;
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
