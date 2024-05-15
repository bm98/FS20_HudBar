using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCamControl
{
  internal class FlyByControllerEventArgs : EventArgs
  {
    /// <summary>
    /// True when Ready to fire flyby
    /// </summary>
    public bool Ready { get; set; }
    /// <summary>
    /// Remaining wait time until ready
    /// </summary>
    public int Remaining_ms { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public FlyByControllerEventArgs( bool ready, int remaining_ms )
    {
      Ready = ready;
      Remaining_ms = remaining_ms;
    }

  }
}
