using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// The Trigger types supported
  /// </summary>
  enum TriggerType
  {
    /// <summary>
    /// Undef Initial Value
    /// </summary>
    Undef=-1,
    /// <summary>
    /// A binary trigger
    /// </summary>
    Binary=0,
    /// <summary>
    /// A level based trigger (float)
    ///  Requires a TriggerBand to be defined for each level
    /// </summary>
    LevelChainF,
    /// <summary>
    /// A level based trigger (int)
    /// </summary>
    LevelChainI,

  }

  /// <summary>
  /// Defines an event with it's callback
  /// </summary>
  abstract class EventProc
  {
    /// <summary>
    /// The stored TriggerType
    /// </summary>
    protected TriggerType m_triggerType = TriggerType.Undef;

    /// <summary>
    /// The Boolean state to trigger the callback
    /// </summary>
    public virtual bool TriggerState { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }
    /// <summary>
    /// The Float state to trigger the callback
    /// </summary>
    public virtual TriggerBandF TriggerStateF { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }
    /// <summary>
    /// The Int state to trigger the callback
    /// </summary>
    public virtual TriggerBandI TriggerStateI { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }

    /// <summary>
    /// Defines the action with one string parameter to execute
    /// </summary>
    public Action<string> Callback { get; set; }
    /// <summary>
    /// The Text to say for this Event
    /// </summary>
    public string Text { get; set; }
  }


}
