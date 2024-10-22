using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.GUI
{
  internal class ClickedEventArgs : EventArgs
  {
    public VItem Item { get; } // readonly
    public MouseButtons MouseButton { get; }

    public ClickedEventArgs( VItem item, MouseButtons mouseButtons ) { Item = item; MouseButton = mouseButtons; }
  }

}
