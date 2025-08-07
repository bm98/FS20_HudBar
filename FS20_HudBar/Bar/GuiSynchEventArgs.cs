using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Bar
{
  internal class GuiSynchEventArgs:EventArgs
  {
    public bool Visible { get; set; }

    public GuiSynchEventArgs(bool visible )
    {
      Visible = visible;
    }

  }
}
