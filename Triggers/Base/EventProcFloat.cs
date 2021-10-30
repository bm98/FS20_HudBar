using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  class EventProcFloat : EventProc
  {
    protected TriggerBandF m_band;

    /// <summary>
    /// The Float state to trigger the callback
    /// </summary>
    public override TriggerBandF TriggerStateF { get => m_band; set => m_band = value; }
  }

}
