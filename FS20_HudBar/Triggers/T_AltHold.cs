using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;
using FSimClientIF;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// ALT Hold Trigger: a binary trigger where ALT Hold Active => True, else=> False
  ///  ALT hold can be the Set Alt or the current Alt depending whether it is
  ///   triggered by the ALT capture or manually hitting the ALT hold.
  /// 
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_AltHold : TriggerBinary
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SV ); // use generic
    }

    private string AvNum( int n, bool nine = false )
    {
      switch (n) {
        case 0: return "zero";
        case 1: return "one";
        case 2: return "two";
        case 3: return "three";
        case 4: return "four";
        case 5: return "five";
        case 6: return "six";
        case 7: return "seven";
        case 8: return "eight";
        case 9: return nine ? "nine" : "niner";
        default: return "";
      }
    }

    // Flightlevel text
    //  three four zero 
    //  one niner
    private string AvTextAlt( int alt )
    {
      var s = "";
      int n;
      if (alt >= 10_000) {
        n = alt / 10_000; s += AvNum( n ) + " "; alt -= n * 10_000;
        n = alt / 1_000; s += AvNum( n ) + " "; alt -= n * 1_000;
        s += "thousand ";
        n = alt / 100;
        if (n > 0) {
          s += AvNum( n, true ) + " hundred"; // give nine hundred (not niner hundred)
        }
        return s;
      }
      else {
        n = alt / 1_000; s += AvNum( n, true ) + " "; alt -= n * 1_000;
        s += "thousand ";
        n = alt / 100;
        if (n > 0) {
          s += AvNum( n, true ) + " hundred"; // give nine hundred (not niner hundred)
        }
        return s;
      }
    }

    // Altitude text
    //   one three thousand five hundred
    //   two thousand five hundred
    //   one thousand nine hundred
    //   six thousand
    private string AvTextFL( int fl )
    {
      var s = "";
      int n;
      if (fl >= 100) {
        n = fl / 100; s += AvNum( n ) + " "; fl -= n * 100;
        n = fl / 10; s += AvNum( n ) + " "; fl -= n * 10;
        n = fl; s += AvNum( n ) + " ";
        return s;
      }
      else if (fl >= 10) {
        n = fl / 10; s += AvNum( n ) + " "; fl -= n * 10;
        n = fl; s += AvNum( n ) + " ";
        return s;
      }
      else {
        n = fl; s += AvNum( n ) + " ";
        return s;
      }
    }

    // Returns either feet or flightlevel
    // Transition is assumed when STD BARO is set, latest when at or above 18'000 ft
    private string AltText( float alt, bool stdBaro )
    {
      if (stdBaro || alt > 17_900) {
        return $"Holding Flightlevel {AvTextFL( (int)(alt / 100) )}";
      }
      else {
        return $"Holding {AvTextAlt( ((int)(alt / 100)) * 100 )} feet"; // get it to hundreds else it is talking way to long
      }
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAP_G1000 object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!m_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases
      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) return; // not while on ground

      // if capturing ALT
      if ((SV.Get<APMode>( SItem.apmG_Ap_mode ) == APMode.On) && SV.Get<bool>( SItem.bGS_Ap_ALT_hold )) {
        // find out what ALT we are holding - if at all
        // ALT hold gets active while approaching the target Alt in ALTS mode
        // or holds the current ALT if the pilot hits the ALT hold button.
        // If the Setting is more than 500ft away from the current ALT we assume the pilot pushed the ALT hold button (best guess...)
        float altHolding = SV.Get<float>( SItem.fG_Ap_ALT_holding_ft ); // target ALT
        /*
        if (altHolding > (hs.Altimeter_ft + 500f)) {
          // seems ALT button was pressed on the way UP to SET ALT
          altHolding = (int)Math.Ceiling( hs.Altimeter_ft / 100f ) * 100; // round current ALT UP 
        }
        else if (altHolding < (hs.Altimeter_ft - 500f)) {
          // seems ALT button was pressed on the way DOWN to SET ALT
          altHolding = (int)Math.Floor( hs.Altimeter_ft / 100f ) * 100; // round current ALT DOWN
        }
        */
        m_actions.First( ).Value.Text = AltText( altHolding, SV.Get<bool>( SItem.bGS_Acft_Altimeter1_mode_Std ) );
      }

      // trigger Once and only if AP and ALT Hold is On
      DetectStateChange( (SV.Get<APMode>( SItem.apmG_Ap_mode ) == APMode.On) && SV.Get<bool>( SItem.bGS_Ap_ALT_hold ) );
      if (SV.Get<bool>( SItem.bGS_Ap_ALT_hold ) == false)
        m_lastTriggered = false; // RESET if no longer captured
    }

    // Implements the means to speak out the AP - ALT hold State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_AltHold( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "AP ALT Hold";
      m_test = $"Holding {AvTextAlt( 5500 )} feet";
      m_test = $"Holding Flightlevel {AvTextFL( 190 )}";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      m_lastTriggered = false;
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Holding" } );
    }

  }

}

