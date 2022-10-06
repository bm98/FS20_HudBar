using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI.Templates.Base
{
  class ToolTip_Base : ToolTip
  {

    /// <summary>
    /// cTor: Define our standard properties
    /// </summary>
    public ToolTip_Base( )
    {
      // Set up the delays for the ToolTip.
      AutoPopDelay = 10_000;
      InitialDelay = 800;
      ReshowDelay = 500;
      // Force the ToolTip text to be displayed whether or not the form is active.
      ShowAlways = true;
    }

  }
}
