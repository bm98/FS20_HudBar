using System;

using SC = SimConnectClient;
using FS20_HudBar.GUI;
using static FSimClientIF.Sim;

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
  internal sealed class TouchDownTrigger : TriggerBase<bool>
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
    private const int c_delayCount = 25; // ~ 0.2 sec per tic
    // trigger action when <=0
    private int _delay = 0;

    public TouchDownTrigger( GUI_Speech speaker )
      : base( speaker )
    {
      _state = TState.Unknown;
    }

    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, 2, OnDataArrival );  // update 5/sec 
    }

    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SV ); // use generic
    }

    protected override void OnDataArrival( string dataRefName )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) {
        _state = TState.Unknown;
        return;
      }

      bool onGround = SV.Get<bool>( SItem.bG_Sim_OnGround );
      switch (_state) {
        case TState.Unknown:
          _state = onGround ? TState.OnGround : TState.InAir;
          break;

        case TState.OnGround:
          if (onGround) {
            ; // remains on ground
          }
          else {
            // wait to decide to stay on ground or going in air / avoid bouncing acft callouts
            _delay = c_delayCount;
            _state = TState.Delay;
          }
          break;

        case TState.InAir:
          if (onGround) {
            // only when InAir -> OnGround triggers once a TouchDown
            _state = TState.TouchDown;
            if (_enabled) Say( "Touchdown" );
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
          if (_delay-- > 0) return;  // wait for next round

          // after a wait time decide on the next state
          if (onGround) {
            _state = TState.OnGround;
          }
          else {
            _state = TState.InAir;
          }
          break;

        default: return;
      }
    }

    public override void ResetTrigger( )
    {
      _state = TState.Unknown;
    }

    // Procs are not used here
    public override void AddProc( EventProc<bool> callback )
    {
      throw new NotImplementedException( );
    }

    public override void ClearProcs( )
    {
      throw new NotImplementedException( );
    }

    protected override bool DetectStateChange( bool level )
    {
      throw new NotImplementedException( );
    }

    public override void SetLevel( bool level, int itemIndex )
    {
      throw new NotImplementedException( );
    }
  }
}
