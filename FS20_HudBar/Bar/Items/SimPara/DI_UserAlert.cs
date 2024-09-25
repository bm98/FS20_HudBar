using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using dNetBm98;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;
using static FSimClientIF.Sim;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.Triggers;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// Shows and handles User alerts
  ///  LED shows state
  ///    OFF - armed; will change to below based on value
  ///    WARN - stays warned as long as the value remains within limit
  ///    ALERT - stays alerted until target changed or canceled
  ///    
  ///  Click Label cycles Alert type (OFF, ALT, VS, SPD, DIST, TIME)
  ///   - a new alert type starts with default init values
  ///   
  ///  MouseWheel changes target value
  ///   - changing a target value resets and arms the alert
  ///   
  ///  Click LED (Cancel)
  ///   when alerted: cancels the Alert LED and arms the Alert again -> LED OFF
  ///   when warned: cancels the Warning -> if the value is within limits it will warn again
  ///   when armed: NOP
  ///   TIMER: LED click restarts
  ///   
  /// Types:
  ///  - OFF does not do anything
  ///      Disp: _
  ///  - ALT set altitude which warns within limits and triggers when passing the target
  ///      Disp: target (set) value
  ///  - AOG set RA altitude which warns within limits and triggers when passing the target
  ///      Disp: target (set) value
  ///  - VS set v-rate which warns within limits and triggers when passing the target
  ///      Disp: target (set) value
  ///  - SPD set IAS which warns within limits and triggers when passing the target
  ///      Disp: target (set) value
  ///  - DIST set a target distance and warn before and trigger when passing 0 (actually 0.001) - count down by elapsed flown distance
  ///      Disp: remaining distance
  ///  - TIME set a target time span and warn before and trigger when passing 0 (actually 0.001) - count down by Sim Sec
  ///      Disp: remaining time (minutes)
  ///  
  /// </summary>
  abstract class DI_UserAlert : DispItem
  {
    private object _settingLock = new object( );


    // need to implement an InsiteLimitDetector with abilities...
    private class ILDet : InsideLimitDetector<float>
    {
      private float _targetValue;

      public ILDet( float lowerLimit, float higherLimit, float targetValue, float value = default, Action<float> limitAction = null )
        : base( lowerLimit, higherLimit, value, limitAction )
      {
        _targetValue = targetValue;
      }

      /// <summary>
      /// Returns how close to the target value the current value is 0..100 [%]
      /// </summary>
      public int TargetPrct {
        get {
          if (_currentValue < _limitLow) return 0;// below lower limit 
          if (_currentValue > _limitHigh) return 0;// above upper limit 
          if (_currentValue == _targetValue) return 100; // at target

          // above lower limit and below upper limit
          if (_currentValue < _targetValue) {
            // below target
            var d = _targetValue - _limitLow;
            if (d == 0) return 0; // not possible with the above..., catch it anyway...
            float p = (_currentValue - _limitLow) / d;
            return (int)Math.Floor( p * 100.0 ); // dont return 100
          }
          if (_currentValue > _targetValue) {
            // above target
            var d = _limitHigh - _targetValue;
            if (d == 0) return 0; // not possible with the above..., catch it anyway...
            float p = (_limitHigh - _currentValue) / d;
            return (int)Math.Floor( p * 100.0 ); // dont return 100
          }
          return 0;
        }
      }

      /// <summary>
      /// Update the Limits without triggering an event
      /// </summary>
      public void SetLimitsAndTarget( float lowerLimit, float higherLimit, float target )
      {
        base.SetLimits( lowerLimit, higherLimit );
        _targetValue = target;
      }

    }


    // Meters live throughout the application 
    protected readonly CPointMeter _cpMeter = new CPointMeter( );
    protected readonly TimeMeter _timeMeter = new TimeMeter( );

    // the alert set value
    protected float _setValue = 0;
    protected float _lastUserDist = 10;
    protected float _lastUserTime = 10;
    protected bool _targetTriggered = false;

    private readonly SlopeDetector<float> _targetLimit = new SlopeDetector<float>( Slope.BiDirectional, 0, 0, null );
    private readonly ILDet _closeLimit = new ILDet( -100000, -10000, 0, 0, null ); // init with unreachable limits
    // chime
    private PingLib.SoundBite _soundBite = new PingLib.SoundBite( HudBar.ChimeSound );

    // true when chime/callout is enabled
    private bool _canChime = false;

    // trigger callout
    private void CallOut( )
    {
      var aTrig = HudBar.VoicePack.TriggerCat[Callouts.Alerts] as T_Alert;
      aTrig.IssueState( _alert.AlertValueType );
    }

    // alert chime and silence further ones
    private void Chime( )
    {
      if (_canChime) {
        HudBar.PingSound.PlayAsync( _soundBite );
        CallOut( );
        _canChime = false; // disable further chimes until cleared
      }
    }


    // clear all triggers
    private void ClearTriggers( )
    {
      _closeLimit.Read( );
      _targetLimit.Read( );
      _targetTriggered = false;

      // clear triggered colors
      _alert.ItemBackColor = cValBG; // reset if it was changed by trigger
      _alert.ItemForeColor = cTxSet; // reset if it was changed by trigger
      _led.ClearMax( );

      _canChime = true; // reset chime
    }

    // using our own detection, generic triggers are limiting (for now)
    // returns true when an alert is triggered
    private bool CheckAlertClose( float value )
    {
      _closeLimit.Update( value );
      return _closeLimit.Read( );
    }

    // update the targetLimit and return if triggered
    private bool CheckAlertTarget( float value )
    {
      _targetLimit.Update( value );
      return _targetLimit.Read( );
    }

    // return the new set value based on the AlertType
    private float GetAlertValue( float oldValue, bool inc, bool largeChange )
    {
      float v = oldValue;
      switch (_alert.AlertValueType) {
        case AlertType.ALT:
          v += (inc ? 1f : -1f) * (largeChange ? 1000f : 100f);
          v = (v < 0) ? 0 : v; // not below zero
          v = (v > 60_000) ? 60_000 : v; // not above FL 600
          return v;

        case AlertType.AOG:
          v += (inc ? 1f : -1f) * (largeChange ? 200f : 10f);
          v = (v < 0) ? 0 : v; // not below zero
          v = (v > 2_500) ? 2_500 : v; // not above 2500
          return v;

        case AlertType.VS:
          v += (inc ? 1f : -1f) * (largeChange ? 500f : 100f);
          v = (v < -9000) ? -9000f : v; // not below -9000
          v = (v > 9000) ? 9000 : v; // not above 9000
          return v;

        case AlertType.SPD:
          v += (inc ? 1f : -1f) * (largeChange ? 10f : 1f);
          v = (v < 0) ? 0 : v; // not below zero
          v = (v > 600) ? 600 : v; // not above 600
          return v;

        case AlertType.DIST:
          v += (inc ? 1f : -1f) * (largeChange ? 10f : 1f);
          v = (v < 1) ? 1 : v; // not below 1
          v = (v > 1000) ? 1000 : v; // not above 1000
          v = (float)Math.Ceiling( v );
          return v;

        case AlertType.TIME:
          v += (inc ? 1f : -1f) * (largeChange ? 10f : 1f);
          v = (v < 1) ? 1 : v; // not below 1
          v = (v > 1000) ? 1000 : v; // not above 1000
          v = (float)Math.Ceiling( v );
          return v;

        default:
          return 0;
      }
    }

    // set limits for the given set value
    private void SetLimits( float setValue )
    {
      // set new target values
      switch (_alert.AlertValueType) {
        case AlertType.ALT:
          _targetLimit.SetSlope( Slope.BiDirectional );
          _targetLimit.SetTarget( setValue );
          _closeLimit.SetLimitsAndTarget( setValue - 500f, setValue + 500f, setValue );
          break;

        case AlertType.AOG:
          _targetLimit.SetSlope( Slope.BiDirectional );
          _targetLimit.SetTarget( setValue );
          _closeLimit.SetLimitsAndTarget( setValue - 200f, setValue + 200f, setValue );
          break;

        case AlertType.VS:
          _targetLimit.SetSlope( Slope.BiDirectional );
          _targetLimit.SetTarget( setValue );
          _closeLimit.SetLimitsAndTarget( setValue - 300f, setValue + 300f, setValue );
          break;

        case AlertType.SPD:
          _targetLimit.SetSlope( Slope.BiDirectional );
          _targetLimit.SetTarget( setValue );
          _closeLimit.SetLimitsAndTarget( setValue - 20f, setValue + 20f, setValue );
          break;

        case AlertType.DIST:
          // dist decreases only to 0 i.e. has an upper limit for closing in and 0 as target
          _targetLimit.SetSlope( Slope.FromAbove );
          _targetLimit.SetTarget( 0.001f ); // cannot be 0 else it will trigger immediately after changing to DIST
          _closeLimit.SetLimitsAndTarget( 0f, 5f, 0.0001f ); // 5nm

          // restart the distance counter
          _cpMeter.Stop( );
          _cpMeter.Start( new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) ),
                      SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
          // _setValue - current will be shown
          _lastUserDist = setValue;
          break;

        case AlertType.TIME:
          _timeMeter.Stop( );
          // time decreases only to 0 i.e. has an upper limit for closing in and 0 as target
          _targetLimit.SetSlope( Slope.FromAbove );
          _targetLimit.SetTarget( 0.001f ); // cannot be 0 else it will trigger immediately after changing to TIME
          _closeLimit.SetLimitsAndTarget( 0f, 1.5f, 0.001f ); // limit: 0..90 sec

          // restart the time counter
          _timeMeter.Start( SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
          // _setValue - current will be shown
          _lastUserTime = setValue;
          break;

        default:
          break;
      }
      // reset after change
      ClearTriggers( );
    }

    // return the name of the Alert
    private string AlertButtonString( AlertType alertType )
    {
      switch (alertType) {
        case AlertType.ALT:
          _led.BarElements = 11; // 10 value elements
          return "! ALT  ";
        case AlertType.AOG:
          _led.BarElements = 11; // 10 value elements
          return "! AOG  ";
        case AlertType.VS:
          _led.BarElements = 7; // 6 value elements
          return "! V/S  ";
        case AlertType.SPD:
          _led.BarElements = 11; // 10 value elements
          return "! SPD  ";
        case AlertType.DIST:
          _led.BarElements = 6; // 5 value elements
          return "! DIST ";
        case AlertType.TIME:
          _led.BarElements = 10; // 9 value elements
          return "! TIME ";

        default:
          _led.BarElements = 6; // 6 value elements
          return "! OFF  ";
      }
    }

    // rotate through alerttypes
    private AlertType NextAlertType( AlertType thisAlertType )
    {
      var a = thisAlertType;
      a++;
      if (!Enum.IsDefined( typeof( AlertType ), a )) {
        return AlertType.OFF;
      }
      else {
        return a;
      }
    }

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.USR_ALERT_1;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ALRT 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Alert 1";

    protected B_Base _label;
    protected V_Base _value1;
    protected A_LEDbar _led;
    protected V_AlertValue _alert;

    // not available outside
    protected DI_UserAlert( ) { }

    /* Must be implemented by the derived class
    public DI_UserAlert( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
    }
    */

    // handles value change of the target value
    protected void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 1/2 - 1/2  dectection for Digits
      var largeChange = e.Location.X < (_value1.Width / 2);

      if (e.Delta > 0) {
        // Up
        _setValue = GetAlertValue( _setValue, true, largeChange );
      }
      else if (e.Delta < 0) {
        // Down
        _setValue = GetAlertValue( _setValue, false, largeChange );
      }

      lock (_targetLimit) {
        // set new target values
        SetLimits( _setValue );
      }//lock
    }

    // switch AlertType
    protected void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      lock (_targetLimit) {
        AlertType next = NextAlertType( _alert.AlertValueType );
        _label.Text = AlertButtonString( next );
        _alert.AlertValueType = next;
        _setValue = 0f; // start with 0 on change
        if (_alert.AlertValueType == AlertType.DIST) {
          _setValue = 10; // set dist else it is going off right away
          _targetLimit.OverrideValue( 1e5f );
        }
        else if (_alert.AlertValueType == AlertType.TIME) {
          _setValue = 10; // set time else it is going off right away
          _targetLimit.OverrideValue( 1e5f );
        }
        _alert.Value = _setValue;

        // must set default limits after a change
        // set new target values
        SetLimits( _setValue );

        _canChime = false; // not when switched, will be reset when changing values
      }
    }

    // clear triggers when clicked
    protected void _led_Click( object sender, EventArgs e )
    {
      lock (_targetLimit) {
        if (_alert.AlertValueType == AlertType.TIME) {
          // reset timer 
          _timeMeter.Stop( );
          _timeMeter.Start( SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
          _targetLimit.OverrideValue( 1e5f ); // set a high to not trigger pass through zero from below
        }
        ClearTriggers( );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    protected void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {

        lock (_targetLimit) {
          float checkValue = 0;
          float dispValue = _setValue;

          switch (_alert.AlertValueType) {
            case AlertType.ALT:
              checkValue = SV.Get<float>( SItem.fG_Acft_Altimeter_ft );
              break;

            case AlertType.AOG:
              checkValue = SV.Get<float>( SItem.fGS_Acft_AltAoG_ft );
              break;

            case AlertType.VS:
              checkValue = SV.Get<float>( SItem.fG_Acft_VS_ftPmin );
              break;

            case AlertType.SPD:
              checkValue = SV.Get<float>( SItem.fG_Acft_IAS_kt );
              break;

            case AlertType.DIST:
              // collect distance flown
              var latLon = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) );
              _cpMeter.Lapse( latLon, SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
              // remaining distance
              checkValue = _setValue - (float)_cpMeter.Distance;
              checkValue = (checkValue < 0) ? 0 : checkValue; // dont use below zero for the Dist
              dispValue = checkValue; // shows the checkValue
              break;

            case AlertType.TIME:
              // collect time elapsed
              _timeMeter.Lapse( SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
              // remaining time
              checkValue = _setValue - (float)_timeMeter.Duration / 60f; // Minutes
              checkValue = (checkValue < 0) ? 0 : checkValue; // dont use below zero for the Time
              dispValue = checkValue; // shows the checkValue
              break;

            default:
              dispValue = float.NaN; // OFF
              _led.ItemForeColor = cInfo; // OFF
              break;
          }

          // check when values are not NaN
          // - check aircraft values only when not on ground
          // - check Timer in any case
          bool mustCheck = !(float.IsNaN( checkValue ) || float.IsNaN( dispValue ))
                          && (!SV.Get<bool>( SItem.bG_Sim_OnGround ) || (_alert.AlertValueType == AlertType.TIME));

          if (mustCheck &&
            (CheckAlertTarget( checkValue ) || _targetTriggered)) {
            // stays on as long as not cleared
            // _targetTriggered = true;  // HOLD IS MAINTAINED IN THE LEDBAR
            _led.IntValue = 100; // % bar indicator
            Chime( );
          }
          else if (mustCheck
            && CheckAlertClose( checkValue )) {
            // may go away if target was not reached
            _led.IntValue = _closeLimit.TargetPrct; // % bar indicator
          }
          else {
            _led.IntValue = 0; // % bar indicator
          }

          // set display text field
          _alert.Value = dispValue;
        }//lock

      }// visible
    }


  }
}
