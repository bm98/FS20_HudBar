using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Defines a binary event with it's callback
  /// </summary>
  class EventProcBinary : EventProc
  {
    // maintain the current state of the object
    protected bool m_state = false;

    /// <summary>
    /// The Boolean state to trigger the callback
    /// </summary>
    public override bool TriggerState { get => m_state; set => m_state = value; }

  }
}
