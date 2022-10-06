using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  class EventProcInteger : EventProc
  {
    protected TriggerBandI m_band;

    /// <summary>
    /// The Int state to trigger the callback
    /// </summary>
    public override TriggerBandI TriggerStateI { get => m_band; set => m_band = value; }

  }

}
