using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.GUI;
using FS20_HudBar.Triggers;
using FS20_HudBar.Triggers.Base;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// The Callout Items
  /// </summary>
  public enum Callouts
  {
    /*
     * The Int of this item is used as index and the sequence is maintained in settings
     * Not great but works if not messed with...
     */
    Parkbrake = 0,
    Gear,
    Flaps,
    WaypointETE,
    Glideslope,
    ApAltHold,
    AirTemp,
    FuelWarning,
    Rotate,
    Spoilers,
    PositiveRate,
    Alerts,
    // ADD NEW ITEMS ONLY AT THE END - App Settings is according to this sequence
  }

  /// <summary>
  /// Implements a voice out pack for the HUD
  /// </summary>
  class HudVoice
  {
    #region STATIC

    /// <summary>
    /// The setting as config string
    /// </summary>
    /// <param name="hudVoice">A HudVoice Obj</param>
    /// <returns>A string</returns>
    public static string AsConfigString( HudVoice hudVoice )
    {
      string profile = "";
      // use the enum sequence as Dictionary foreach may not return in sequence
      foreach (Callouts ce in Enum.GetValues( typeof( Callouts ) )) {
        profile += $"{(hudVoice.TriggerCat[ce].Enabled ? 1 : 0)};";
      }
      return profile;
    }

    /// <summary>
    /// The List of flags as config string
    /// </summary>
    /// <param name="config">A list of Flags</param>
    /// <returns>A string</returns>
    public static string AsConfigString( List<bool> config )
    {
      string profile = "";
      foreach (var b in config) {
        profile += $"{(b ? 1 : 0)};";
      }
      return profile;
    }

    /// <summary>
    /// A List with all Callout Flags (false) 
    /// </summary>
    /// <returns>A List of Flags</returns>
    public static List<bool> ConfigList( )
    {
      var l = new List<bool>( );
      foreach (Callouts ce in Enum.GetValues( typeof( Callouts ) )) {
        l.Add( false );
      }
      return l;
    }

    #endregion

    // collect all triggers here (used for batch processing)
    private Dictionary<Callouts, ITriggers> m_triggerCat = new Dictionary<Callouts, ITriggers>( );

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
    private T_Spoilers v_spoilers;
    private T_PositiveRate v_positiveRate;
    private T_Alert v_alerts;

    /// <summary>
    /// Provide the list of installed Voice Triggers
    /// </summary>
    public IDictionary<Callouts, ITriggers> TriggerCat => m_triggerCat;


    /// <summary>
    /// cTor Set the voice output and create the voice out items 
    ///   TODO improve to have the order from an enum rather than from the code order
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    /// <param name="configString">A callout config string</param>
    public HudVoice( GUI_Speech speaker )
    {
      // Create all voice out items and add them to a list for Configuration
      // ADD NEW ITEMS ONLY AT THE END - App Settings is according to this sequence
      v_parkbrake = new T_Brakes( speaker ); m_triggerCat.Add( Callouts.Parkbrake, v_parkbrake );
      v_gear = new T_Gear( speaker ); m_triggerCat.Add( Callouts.Gear, v_gear );
      v_flaps = new T_Flaps( speaker ); m_triggerCat.Add( Callouts.Flaps, v_flaps );
      v_waypointETE = new T_WaypointETE( speaker ); m_triggerCat.Add( Callouts.WaypointETE, v_waypointETE );
      v_glideslope = new T_Glideslope( speaker ); m_triggerCat.Add( Callouts.Glideslope, v_glideslope );
      v_apAltHold = new T_AltHold( speaker ); m_triggerCat.Add( Callouts.ApAltHold, v_apAltHold );
      v_airTemp = new T_OAT( speaker ); m_triggerCat.Add( Callouts.AirTemp, v_airTemp );
      v_warnFuel = new T_WarnFuel( speaker ); m_triggerCat.Add( Callouts.FuelWarning, v_warnFuel );
      v_rotate = new T_IAS_Rotate( speaker ); m_triggerCat.Add( Callouts.Rotate, v_rotate );
      v_spoilers = new T_Spoilers( speaker ); m_triggerCat.Add( Callouts.Spoilers, v_spoilers );
      v_positiveRate = new T_PositiveRate( speaker ); m_triggerCat.Add( Callouts.PositiveRate, v_positiveRate );
      v_alerts = new T_Alert( speaker ); m_triggerCat.Add( Callouts.Alerts, v_alerts );
      // load from settings
      SetFromConfigString( AsConfigString( ConfigList( ) ) ); // all off
    }

    /// <summary>
    /// The current setting as config string
    /// </summary>
    /// <returns>A string</returns>
    public string GetConfigString( ) => AsConfigString( this );

    /// <summary>
    /// Set current from a config string
    /// </summary>
    /// <param name="configString">A config string</param>
    public void SetFromConfigString( string configString )
    {
      string profile = configString;
      string[] e = profile.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      int idx = 0;
      foreach (Callouts ce in Enum.GetValues( typeof( Callouts ) )) {
        bool enabled = false; // default OFF
        if (e.Length > idx) {
          enabled = e[idx] == "1"; // found an element in the string
        }
        idx++; // next item
        TriggerCat[ce].Enabled = enabled;
      }
    }

    /// <summary>
    /// Register Observers to get Data Updates
    /// </summary>
    public void RegisterObservers( )
    {
      // Update all voice out items
      foreach (var trig in m_triggerCat.Values) {
        trig.RegisterObserver( );
      }
    }
    /// <summary>
    /// UnRegister Observers to get Data Updates
    /// </summary>
    public void UnRegisterObservers( )
    {
      // Update all voice out items
      foreach (var trig in m_triggerCat.Values) {
        trig.UnRegisterObserver( );
      }
    }


  }
}
