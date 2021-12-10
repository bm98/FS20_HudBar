using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FS20_HudBar.GUI;
using FS20_HudBar.Triggers;
using FS20_HudBar.Triggers.Base;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Implements a voice out pack for the HUD
  /// </summary>
  class HudVoice
  {
    // collect all triggers here (used for batch processing)
    private List<ITriggers> m_triggerList = new List<ITriggers>();

    // All voice out items
    private T_Brakes v_parkbrake;
    private T_Gear v_gear;
    private T_Flaps v_flaps;
    private T_WaypointETE v_waypointETE;
    private T_Glideslope v_glideslope;
    private T_AltHold v_apAltHold;
    private T_OAT v_airTemp;
    private T_WarnFuel v_warnFuel;
    private T_IAS_Rotate v_rotate;

    /// <summary>
    /// Provide the list of installed Voice Triggers
    /// </summary>
    public IList<ITriggers> Triggers => m_triggerList;


    /// <summary>
    /// cTor Set the voice output and create the voice out items
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public HudVoice( GUI_Speech speaker )
    {
      // Create all voice out items and add them to a list for Configuration
      // ADD NEW ITEMS ONLY AT THE END - App Settings is according to this sequence
      v_parkbrake = new T_Brakes( speaker ); m_triggerList.Add( v_parkbrake );
      v_gear = new T_Gear( speaker ); m_triggerList.Add( v_gear );
      v_flaps = new T_Flaps( speaker ); m_triggerList.Add( v_flaps );
      v_waypointETE = new T_WaypointETE( speaker ); m_triggerList.Add( v_waypointETE );
      v_glideslope = new T_Glideslope( speaker ); m_triggerList.Add( v_glideslope );
      v_apAltHold = new T_AltHold( speaker ); m_triggerList.Add( v_apAltHold );
      v_airTemp = new T_OAT(speaker); m_triggerList.Add( v_airTemp );
      v_warnFuel = new T_WarnFuel( speaker ); m_triggerList.Add( v_warnFuel );
      v_rotate = new T_IAS_Rotate( speaker ); m_triggerList.Add( v_rotate );

      // load from settings
      string profile = AppSettings.Instance.VoiceCalloutProfile;
      string[] e = profile.Split(new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries );
      int idx = 0;
      foreach ( var vc in m_triggerList ) {
        bool enabled = false; // default OFF
        if ( e.Length > idx ) {
          enabled = e[idx] == "1"; // found an element in the string
        }
        idx++; // next item
        vc.Enabled = enabled;
      }

    }


    /// <summary>
    /// Save the curren VoiceProfile
    /// </summary>
    public void SaveSettings( )
    {
      string profile = "";
      foreach (var vc in Triggers ) {
        profile += $"{(vc.Enabled?1:0)};";
      }
      AppSettings.Instance.VoiceCalloutProfile = profile;
      AppSettings.Instance.Save( );
    }

    /// <summary>
    /// Register Observers to get Data Updates
    /// </summary>
    public void RegisterObservers(  )
    {
      // Update all voice out items
      foreach (var trig in m_triggerList ) {
        trig.RegisterObserver( );
      }
    }


  }
}
