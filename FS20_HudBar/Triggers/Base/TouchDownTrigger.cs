using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Triggers a VoiceOut when TouchDown is detected
  /// 
  ///   State Machine:
  ///   
  ///   Init -> Unknown
  ///   
  ///   Unknown:
  ///     SimOnGround -> OnGround, else InAir
  /// 
  ///   OnGround:
  ///     SimOnGround -> OnGround, else Delay
  /// 
  ///   InAir:
  ///     SimOnGround -> TouchDown, else InAir
  /// 
  ///   TouchDown:
  ///     Delay
  /// 
  ///   Delay:
  ///     count>0 -> Delay else {SimOnGround -> OnGround, else InAir}
  ///     
  /// </summary>
  internal sealed class TouchDownTrigger : TriggerBase
  {
    private enum TState
    {
      Unknown = 0,
      OnGround,
      InAir,
      TouchDown,
      Delay, // avoid bouncing detected as touchdown
    }
    // TD state var
    private TState _state = TState.Unknown;

    // count down to zero 
    private const int c_delayCount = 20; // ~ 0.2 sec per tic
    // trigger action when <=0
    private int _delay = 0;

    public TouchDownTrigger( GUI_Speech speaker )
      : base( speaker )
    {
      _state = TState.Unknown;
    }

    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.HudBarModule, OnDataArrival ); // use generic
    }

    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use generic
    }

    protected override void OnDataArrival( string dataRefName )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) {
        _state = TState.Unknown;
        return;
      }

      switch (_state) {
        case TState.Unknown:
          _state = SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround ? TState.OnGround : TState.InAir;
          break;

        case TState.OnGround:
          if (SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround) {
            ; // remains on ground
          }
          else {
            // wait to decide to stay on ground or going in air / avoid bouncing acft callouts
            _delay = c_delayCount;
            _state = TState.Delay;
          }
          break;

        case TState.InAir:
          if (SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround) {
            // only when InAir -> OnGround triggers once a TouchDown
            _state = TState.TouchDown;
            if (m_enabled) Say( "Touchdown" );
          }
          else {
            ; // remains in air
          }
          break;

        case TState.TouchDown:
          // wait to decide to stay on ground or going in air / avoid bouncing acft callouts
          _delay = c_delayCount;
          _state = TState.Delay;
          break;

        case TState.Delay:
          if (_delay > 0) { _delay--; return; }  // wait for next round

          // after a wait time decide on the next state
          if (SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround) {
            _state = TState.OnGround;
          }
          else {
            _state = TState.InAir;
          }
          break;

        default: return;
      }
    }

    public override void Reset( )
    {
      _state = TState.Unknown;
    }

    // Procs are not used here
    public override void AddProc( EventProc callback )
    {
      throw new NotImplementedException( );
    }

    public override void ClearProcs( )
    {
      throw new NotImplementedException( );
    }

  }
}
