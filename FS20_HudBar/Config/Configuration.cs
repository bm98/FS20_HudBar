using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.GUI;

using FSimClientIF;
using FSimClientIF.Modules;

using Windows.Globalization.Collation;

using static FS20_HudBar.GUI.GUI_Colors;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// The Configuration of HudBar
  /// </summary>
  internal class Configuration
  {
    #region STATIC

    /// <summary>
    /// Load a Configuration from Settings
    /// </summary>
    /// <returns>A new Configuration</returns>
    public static Configuration GetFromSettings( )
    {
      var AS = AppSettingsV2.Instance;

      var cfg = new Configuration( );

      cfg.OutputDeviceName = AS.OutputDeviceName;
      cfg.VoiceName = AS.VoiceName;
      cfg.VoiceCalloutProfile = AS.VoiceCalloutProfile;
      cfg.FRecorder = AS.FRecorder;
      cfg.FltAutoSaveATC = (FlightPlanMode)AS.FltAutoSaveATC;

      cfg.InGameHook = AS.InGameHook;
      cfg.KeyboardHook = AS.KeyboardHook;
      cfg.HKShowHide = AS.HKShowHide;
      cfg.HKCamera = AS.HKCamera;
      cfg.HKShelf = AS.HKShelf;
      cfg.HKChecklistBox = AS.HKChecklistBox;

      cfg.UserFonts = AS.UserFonts;
      cfg.UserColorsReg = AS.UserColorsReg;
      cfg.UserColorsDim = AS.UserColorsDim;
      cfg.UserColorsInv = AS.UserColorsInv;

      cfg.CurrentProfile = (DProfile)AS.SelProfile;

      // load all Profiles
      int profileNumber = 1;
      foreach (DProfile profile in Enum.GetValues( typeof( DProfile ) )) {
        cfg.Profiles.Add( profile, CProfile.GetFromSettings( profileNumber++ ) );
      }

      return cfg;
    }

    /// <summary>
    /// Save a Configuration to Settings
    /// </summary>
    public static void SaveToSettings( Configuration config )
    {
      var AS = AppSettingsV2.Instance;

      // General
      AS.OutputDeviceName = config.OutputDeviceName;
      AS.VoiceName = config.VoiceName;
      AS.VoiceCalloutProfile = config.VoiceCalloutProfile;
      AS.FRecorder = config.FRecorder;
      AS.FltAutoSaveATC = (int)config.FltAutoSaveATC;

      AS.InGameHook = config.InGameHook;
      AS.KeyboardHook = config.KeyboardHook;
      AS.HKShowHide = config.HKShowHide;
      AS.HKCamera = config.HKCamera;
      AS.HKShelf = config.HKShelf;
      AS.HKChecklistBox = config.HKChecklistBox;

      AS.UserFonts = config.UserFonts;
      AS.UserColorsReg = config.UserColorsReg;
      AS.UserColorsDim = config.UserColorsDim;
      AS.UserColorsInv = config.UserColorsInv;

      AS.SelProfile = config.CurrentProfileIndex;

      // save all Profiles
      foreach (DProfile profile in Enum.GetValues( typeof( DProfile ) )) {
        CProfile.SaveToSettings( config.Profiles[profile] );
      }

      // Finally Save
      AS.Save( );
    }

    /// <summary>
    /// Returns a deep copy of the given Configuration
    /// </summary>
    /// <param name="other">The Configuration to copy from</param>
    /// <returns>A new Configuration</returns>
    public static Configuration AsCopy( Configuration other )
    {
      Configuration cfg = (Configuration)other.MemberwiseClone( );
      // profiles must be cloned
      cfg._profiles = new Dictionary<DProfile, CProfile>( );
      foreach (var p in other.Profiles) {
        cfg.Profiles.Add( p.Key, new CProfile( p.Value ) );
      }

      return cfg;
    }

    #endregion

    // *** CLASS

    /// <summary>
    /// cTor: is private, use FactoryMethod 'GetFromSettings()'
    /// </summary>
    private Configuration( ) { }

    // profile store (never a null value expected)
    private Dictionary<DProfile, CProfile> _profiles = new Dictionary<DProfile, CProfile>( );

    /// <summary>
    /// Returns a deep copy of this Configuration
    /// </summary>
    /// <returns>A Configuration</returns>
    public Configuration Clone( ) => AsCopy( this );

    /// <summary>
    /// Voice Output device name
    /// </summary>
    public string OutputDeviceName { get; protected set; } = "";
    /// <summary>
    /// Voice Name
    /// </summary>
    public string VoiceName { get; protected set; } = "";
    /// <summary>
    /// VoiceOut Profile (; list)
    /// </summary>
    public string VoiceCalloutProfile { get; protected set; } = "";
    /// <summary>
    /// Use Flight Recorder Flag
    /// </summary>
    public bool FRecorder { get; protected set; } = false;
    /// <summary>
    /// Backup Mode
    /// </summary>
    public FlightPlanMode FltAutoSaveATC { get; protected set; } = FlightPlanMode.Disabled;

    /// <summary>
    /// InGame Hotkeys enabled flag
    /// </summary>
    public bool InGameHook { get; protected set; } = false;

    /// <summary>
    /// Keyboard Hotkeys enabled flag
    /// </summary>
    public bool KeyboardHook { get; protected set; } = false;
    /// <summary>
    /// Hudbar show/hide hotkey string
    /// </summary>
    public string HKShowHide { get; protected set; } = "";
    /// <summary>
    /// CameraControl App show/hide hotkey string
    /// </summary>
    public string HKCamera { get; protected set; } = "";
    /// <summary>
    /// Flightbag App show/hide hotkey string
    /// </summary>
    public string HKShelf { get; protected set; } = "";
    /// <summary>
    /// CheckistBox App show/hide hotkey string
    /// </summary>
    public string HKChecklistBox { get; protected set; } = "";

    /// <summary>
    /// True when using Default (App) Fonts
    /// </summary>
    public bool UsingDefaultFonts => string.IsNullOrEmpty( UserFonts );
    /// <summary>
    /// General User Fonts string
    /// </summary>
    public string UserFonts { get; protected set; } = "";

    /// <summary>
    /// True when using Default (App) Colors
    /// </summary>
    public bool UsingDefaultColors => string.IsNullOrEmpty( UserColorsReg );
    /// <summary>
    /// General Regular User Colors string
    /// </summary>
    public string UserColorsReg { get; protected set; } = "";
    /// <summary>
    /// General Dimmed User Colors string
    /// </summary>
    public string UserColorsDim { get; protected set; } = "";
    /// <summary>
    /// General Inverse User Colors string
    /// </summary>
    public string UserColorsInv { get; protected set; } = "";


    /// <summary>
    /// Profile store
    /// </summary>
    public IDictionary<DProfile, CProfile> Profiles => _profiles;

    /// <summary>
    /// The named Profile 
    /// </summary>
    /// <param name="dProfile">Profile Enum</param>
    /// <returns>A Profile</returns>
    public CProfile Profile( DProfile dProfile ) => _profiles[dProfile];

    /// <summary>
    /// The Profile with index
    /// </summary>
    /// <param name="index">Profile Index</param>
    /// <returns>A Profile</returns>
    public CProfile ProfileAt( int index ) => Profile( (DProfile)index );


    /// <summary>
    /// Save this configuration to Settings
    /// </summary>
    public void SaveToSettings( ) => SaveToSettings( this );

    /// <summary>
    /// Set the actual profile
    /// </summary>
    /// <param name="profileEnum">A profile Enum</param>
    public void SetProfile( DProfile profileEnum ) => CurrentProfile = profileEnum;
    /// <summary>
    /// Set the actual profile
    /// </summary>
    /// <param name="profileIndex">A profile Index</param>
    public void SetProfile( int profileIndex ) => SetProfile( (DProfile)profileIndex );

    /// <summary>
    /// Set the current InGame Hook flag
    /// </summary>
    /// <param name="inGameHook">Enabled Flag</param>
    public void SetInGameHook( bool inGameHook ) => InGameHook = inGameHook;
    /// <summary>
    /// Set the current keyboardHook flag
    /// </summary>
    /// <param name="keyboardHook">Enabled Flag</param>
    public void SetKeyboardHook( bool keyboardHook ) => KeyboardHook = keyboardHook;

    /// <summary>
    /// The Hotkey config string of a HK
    /// </summary>
    /// <param name="hotkey">A hotkey</param>
    /// <returns>The config string</returns>
    public string HKStringOf( Hotkeys hotkey )
    {
      switch (hotkey) {
        case Hotkeys.Profile_1: return Profiles[DProfile.Profile_1].HKProfile;
        case Hotkeys.Profile_2: return Profiles[DProfile.Profile_2].HKProfile;
        case Hotkeys.Profile_3: return Profiles[DProfile.Profile_3].HKProfile;
        case Hotkeys.Profile_4: return Profiles[DProfile.Profile_4].HKProfile;
        case Hotkeys.Profile_5: return Profiles[DProfile.Profile_5].HKProfile;
        case Hotkeys.Profile_6: return Profiles[DProfile.Profile_6].HKProfile;
        case Hotkeys.Profile_7: return Profiles[DProfile.Profile_7].HKProfile;
        case Hotkeys.Profile_8: return Profiles[DProfile.Profile_8].HKProfile;
        case Hotkeys.Profile_9: return Profiles[DProfile.Profile_9].HKProfile;
        case Hotkeys.Profile_10: return Profiles[DProfile.Profile_10].HKProfile;
        case Hotkeys.Show_Hide: return HKShowHide;
        case Hotkeys.FlightBag: return HKShelf;
        case Hotkeys.Camera: return HKCamera;
        case Hotkeys.ChecklistBox: return HKChecklistBox;

        case Hotkeys.MoveBarToOtherWindow: return ""; // hardcoded; not in config
        default: return "";
      }
    }

    /// <summary>
    /// Update the Hotkey config string of a HK
    /// </summary>
    /// <param name="hotkey">A hotkey</param>
    /// <param name="hk">The config string</param>
    public void SetHKStringOf( Hotkeys hotkey, string hk )
    {
      switch (hotkey) {
        case Hotkeys.Profile_1: Profiles[DProfile.Profile_1].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_2: Profiles[DProfile.Profile_2].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_3: Profiles[DProfile.Profile_3].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_4: Profiles[DProfile.Profile_4].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_5: Profiles[DProfile.Profile_5].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_6: Profiles[DProfile.Profile_6].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_7: Profiles[DProfile.Profile_7].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_8: Profiles[DProfile.Profile_8].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_9: Profiles[DProfile.Profile_9].SetHKfromValue( hk ); break;
        case Hotkeys.Profile_10: Profiles[DProfile.Profile_10].SetHKfromValue( hk ); break;
        case Hotkeys.Show_Hide: HKShowHide = hk; break;
        case Hotkeys.FlightBag: HKShelf = hk; break;
        case Hotkeys.Camera: HKCamera = hk; break;
        case Hotkeys.ChecklistBox: HKChecklistBox = hk; break;

        case Hotkeys.MoveBarToOtherWindow: break; // hardcoded; not in config
        default: break;
      }
    }



    /// <summary>
    /// Set the Flight Recorder enabled flag
    /// </summary>
    /// <param name="fRec">Enabled Flag</param>
    public void SetFlightRecorder( bool fRec ) => FRecorder = fRec;
    /// <summary>
    /// Set the current FltAutoSave
    /// </summary>
    /// <param name="autoSave">Backup enum</param>
    public void SetFltAutoSave( FlightPlanMode autoSave ) => FltAutoSaveATC = autoSave;
    /// <summary>
    /// Set the current VoiceName
    /// </summary>
    /// <param name="voiceName">Voice Name</param>
    public void SetVoiceName( string voiceName ) => VoiceName = voiceName;
    /// <summary>
    /// Set the current outputDeviceName
    /// </summary>
    /// <param name="outputDeviceName">Output Device Name</param>
    public void SetOutputDeviceName( string outputDeviceName ) => OutputDeviceName = outputDeviceName;
    /// <summary>
    /// Set the Voice Callout Configuration
    /// </summary>
    /// <param name="voiceConfigString">Voice Callouts config string</param>
    public void SetVoiceCalloutConfigString( string voiceConfigString ) => VoiceCalloutProfile = voiceConfigString;

    /// <summary>
    /// Set the current UserFonts
    /// </summary>
    /// <param name="configString">Fonts config string</param>
    public void SetUserFontsConfigString( string configString )
    {
      UserFonts = configString;
    }

    /// <summary>
    /// Set the current UserColors
    /// </summary>
    /// <param name="configStringReg">Regular config string</param>
    /// <param name="configStringDim">Dimmed config string</param>
    /// <param name="configStringInv">Inverse config string</param>
    public void SetUserColorsConfigString( string configStringReg, string configStringDim, string configStringInv )
    {
      // store User Colors
      UserColorsReg = configStringReg;
      UserColorsDim = configStringDim;
      UserColorsInv = configStringInv;
    }

    // update currently used colors
    private void UpdateColorSet( )
    {
      // update Live colors from Used ones
      GUI_Colors.SetColorSet( ColorSet.BrightSet, UsedColorSet( ColorSet.BrightSet ) );
      GUI_Colors.SetColorSet( ColorSet.DimmedSet, UsedColorSet( ColorSet.DimmedSet ) );
      GUI_Colors.SetColorSet( ColorSet.InverseSet, UsedColorSet( ColorSet.InverseSet ) );
    }

    // Update currently used fonts
    private void UpdateFonts( )
    {
      // TODO
      //FontRef.FromConfigString( configString );
    }

    /// <summary>
    /// Update live values after config changes
    /// </summary>
    public void UpdateLiveValues( )
    {
      UpdateColorSet( );
      UpdateFonts( );

    }

    // USED items (depends on current profile)

    /// <summary>
    /// Get: The profile used
    /// </summary>
    public DProfile CurrentProfile { get; private set; } = DProfile.Profile_1;
    /// <summary>
    /// Get: The profile index used
    /// </summary>
    public int CurrentProfileIndex => (int)CurrentProfile;

    /// <summary>
    /// The currently used profile
    /// </summary>
    public CProfile UsedProfile => Profiles[CurrentProfile];
    /// <summary>
    /// Get: The name of the profile used
    /// </summary>
    public string UsedProfileName => UsedProfile.PName;

    /// <summary>
    /// Used Regular User ColorSet
    /// </summary>
    public GUI_ColorSet UsedColorSetReg {
      get {
        if (!UsedProfile.UsingDefaultColors) return GUI_Colors.FromConfigString( UsedProfile.ColorReg );
        if (!UsingDefaultColors) return GUI_Colors.FromConfigString( UserColorsReg );
        return GUI_Colors.GetDefaultColorSet( GUI_Colors.ColorSet.BrightSet );
      }
    }
    /// <summary>
    /// Used Dimmed User ColorSet
    /// </summary>
    public GUI_ColorSet UsedColorSetDim {
      get {
        if (!UsedProfile.UsingDefaultColors) return GUI_Colors.FromConfigString( UsedProfile.ColorDim );
        if (!UsingDefaultColors) return GUI_Colors.FromConfigString( UserColorsDim );
        return GUI_Colors.GetDefaultColorSet( GUI_Colors.ColorSet.DimmedSet );
      }
    }
    /// <summary>
    /// Used Inverse User ColorSet
    /// </summary>
    public GUI_ColorSet UsedColorSetInv {
      get {
        if (!UsedProfile.UsingDefaultColors) return GUI_Colors.FromConfigString( UsedProfile.ColorInv );
        if (!UsingDefaultColors) return GUI_Colors.FromConfigString( UserColorsInv );
        return GUI_Colors.GetDefaultColorSet( GUI_Colors.ColorSet.InverseSet );
      }
    }

    /// <summary>
    /// Used ColorSet of kind
    /// </summary>
    /// <param name="colorSet">Kind of ColorSet</param>
    /// <returns>A ColorSet</returns>
    public GUI_ColorSet UsedColorSet( GUI_Colors.ColorSet colorSet )
    {
      switch (colorSet) {
        case GUI_Colors.ColorSet.BrightSet: return UsedColorSetReg;
        case GUI_Colors.ColorSet.DimmedSet: return UsedColorSetDim;
        case GUI_Colors.ColorSet.InverseSet: return UsedColorSetInv;
        default: return UsedColorSetReg;
      }
    }

    /// <summary>
    /// The Used Fonts config string
    ///  When no user fonts are found it returns an empty string
    /// </summary>
    public string UsedFontsConfigString {
      get {
        if (!UsedProfile.UsingDefaultFonts) return UsedProfile.Fonts;
        if (!UsingDefaultFonts) return UserFonts;
        return ""; // will reset the user fonts 
      }
    }



  }
}
