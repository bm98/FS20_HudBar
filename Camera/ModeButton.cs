using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF;

namespace FS20_HudBar.Camera
{
  /// <summary>
  /// Represents A View Mode Button (Setting)
  /// Handles the Click of the button
  /// </summary>
  internal class ModeButton
  {

    private Action<CameraSetting, uint> _switchCam;

    public Button Button { get; private set; }
    public CameraSetting Setting { get; private set; }
    public uint ViewIndex { get; set; } = 0;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="setting">The Setting for this Mode</param>
    /// <param name="button">The managed button</param>
    /// <param name="switchCam">Camera Switch Method</param>
    public ModeButton( CameraSetting setting, Button button, Action<CameraSetting, uint> switchCam )
    {
      Setting = setting;
      Button = button;
      button.Click += button_Click;
      _switchCam = switchCam;
    }

    private void button_Click( object sender, EventArgs e )
    {
      _switchCam( Setting, ViewIndex );
    }

  }
}
