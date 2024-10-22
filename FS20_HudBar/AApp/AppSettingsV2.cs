using System;
using System.Drawing;

using SettingsLib;
using FS20_HudBar.Config;
using System.Windows.Forms;
using System.Reflection;

namespace FS20_HudBar
{
  /// <summary>
  /// AppSettings using the SettingsLib
  ///  
  /// It will not maintain App Version specific config files 
  /// i.e. FirstRun is only true when no Settings File was read at all
  /// If settings need to be versioned - it must be done on the App Level
  /// All Instances are maintained in the same Settings File 
  /// 
  /// </summary>
  internal sealed class AppSettingsV2 : AppSettingsBaseV2
  {
    #region Named Setting access Support

    /// <summary>
    /// Get a named Setting
    /// </summary>
    /// <typeparam name="T">Type of the Setting item</typeparam>
    /// <param name="key">Property name of the Setting item</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>Setting item</returns>
    public static T GetSetting<T>( string key, T defaultValue )
    {
      T data = defaultValue;

      try {
        PropertyInfo settingProperty = typeof( AppSettingsV2 ).GetProperty( key );
        data = (T)settingProperty.GetValue( AppSettingsV2.Instance, null );
      }
      catch { }
      return data;
    }

    /// <summary>
    /// Set a named Setting
    /// </summary>
    /// <typeparam name="T">Type of the Setting item</typeparam>
    /// <param name="key">Property name of the Setting item</param>
    /// <param name="value">The Value to set</param>
    public static void SetSetting<T>( string key, T value )
    {
      try {
        PropertyInfo settingProperty = typeof( AppSettingsV2 ).GetProperty( key );
        settingProperty.SetValue( AppSettingsV2.Instance, value );
      }
      catch { }
    }

    #endregion


    // Singleton
    private static Lazy<AppSettingsV2> m_lazy = new Lazy<AppSettingsV2>( ( ) => new AppSettingsV2( ) );
    public static AppSettingsV2 Instance { get => m_lazy.Value; }
    private AppSettingsV2( ) { }

    /// <summary>
    /// Init a new instance to use
    /// </summary>
    /// <param name="settingsFile">An filename -> distinct for each instance used</param>
    /// <param name="instance">An instance name (use "" as default)</param>
    public static void InitInstance( string settingsFile, string instance )
    {
      m_lazy = new Lazy<AppSettingsV2>( ( ) => new AppSettingsV2( settingsFile, instance ) );
    }
    /// <summary>
    /// cTor: create an instance if such a name is given, else use the default one
    /// </summary>
    /// <param name="settingsFile">An filename -> distinct for each instance used</param>
    /// <param name="instance">An Instance String (defaults to empty = default instance)</param>
    private AppSettingsV2( string settingsFile, string instance ) : base( settingsFile, instance )
    {
      // start processing here 
      if (this.FirstRun) {
        // Load default profiles if the app runs the first time
        try {
          // set profiles when no previous setting is available
          var dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_1 );
          this.Profile_1_Name = dprofile.Name;
          this.Profile_1 = dprofile.Profile;
          this.FlowBreak_1 = dprofile.FlowBreak;
          this.Sequence_1 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_2 );
          this.Profile_2_Name = dprofile.Name;
          this.Profile_2 = dprofile.Profile;
          this.FlowBreak_2 = dprofile.FlowBreak;
          this.Sequence_2 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_3 );
          this.Profile_3_Name = dprofile.Name;
          this.Profile_3 = dprofile.Profile;
          this.FlowBreak_3 = dprofile.FlowBreak;
          this.Sequence_3 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_4 );
          this.Profile_4_Name = dprofile.Name;
          this.Profile_4 = dprofile.Profile;
          this.FlowBreak_4 = dprofile.FlowBreak;
          this.Sequence_4 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_5 );
          this.Profile_5_Name = dprofile.Name;
          this.Profile_5 = dprofile.Profile;
          this.FlowBreak_5 = dprofile.FlowBreak;
          this.Sequence_5 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_6 );
          this.Profile_6_Name = dprofile.Name;
          this.Profile_6 = dprofile.Profile;
          this.FlowBreak_6 = dprofile.FlowBreak;
          this.Sequence_6 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_7 );
          this.Profile_7_Name = dprofile.Name;
          this.Profile_7 = dprofile.Profile;
          this.FlowBreak_7 = dprofile.FlowBreak;
          this.Sequence_7 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_8 );
          this.Profile_8_Name = dprofile.Name;
          this.Profile_8 = dprofile.Profile;
          this.FlowBreak_8 = dprofile.FlowBreak;
          this.Sequence_8 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_9 );
          this.Profile_9_Name = dprofile.Name;
          this.Profile_9 = dprofile.Profile;
          this.FlowBreak_9 = dprofile.FlowBreak;
          this.Sequence_9 = dprofile.DispOrder;

          dprofile = DefaultProfiles.GetDefaultProfile( DProfile.Profile_10 );
          this.Profile_10_Name = dprofile.Name;
          this.Profile_10 = dprofile.Profile;
          this.FlowBreak_10 = dprofile.FlowBreak;
          this.Sequence_10 = dprofile.DispOrder;
        }
        catch { }

        this.FirstRun = false;
        this.Save( );
      }
    }

    // manages Upgrade
    [DefaultSettingValue( "True" )]
    public bool FirstRun {
      get { return (bool)this["FirstRun"]; }
      set { this["FirstRun"] = value; }
    }

    #region SETTINGS

    // This must set True once the new Settings are in use
    [DefaultSettingValue( "False" )]
    public bool V2InUse {
      get { return (bool)this["V2InUse"]; }
      set { this["V2InUse"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool OmitDBCheck {
      get { return (bool)this["OmitDBCheck"]; }
      set { this["OmitDBCheck"] = value; }
    }

    // Control bound settings
    [DefaultSettingValue( "10, 10" )]
    public Point FormLocation {
      get { return (Point)this["FormLocation"]; }
      set { this["FormLocation"] = value; }
    }

    // Screen the bar/tile is attached to 
    [DefaultSettingValue( "0" )]
    public int ScreenNumber {
      get { return (int)this["ScreenNumber"]; }
      set { this["ScreenNumber"] = value; }
    }

    // User Display Settings

    [DefaultSettingValue( "..." )]
    public string FreeText {
      get { return (string)this["FreeText"]; }
      set { this["FreeText"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool ShowUnits {
      get { return (bool)this["ShowUnits"]; }
      set { this["ShowUnits"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool Altitude_Metric {
      get { return (bool)this["Altitude_Metric"]; }
      set { this["Altitude_Metric"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool Distance_Metric {
      get { return (bool)this["Distance_Metric"]; }
      set { this["Distance_Metric"] = value; }
    }

    // Color Set in use (Reg,Dim,Inv)
    [DefaultSettingValue( "0" )]  // ref enum GUI_Colors.ColorSet
    public int Appearance {
      get { return (int)this["Appearance"]; }
      set { this["Appearance"] = value; }
    }

    // Color Picker stored Patches
    [DefaultSettingValue( "" )]
    public string ColorPatches {
      get { return (string)this["ColorPatches"]; }
      set { this["ColorPatches"] = value; }
    }


    #region Configuration Settings

    // Config Settings

    // Location of the Config Dialog
    [DefaultSettingValue( "10, 10" )]
    public Point ConfigLocation {
      get { return (Point)this["ConfigLocation"]; }
      set { this["ConfigLocation"] = value; }
    }
    // Size of the Config Dialog
    [DefaultSettingValue( "0, 0" )]
    public Size ConfigSize {
      get { return (Size)this["ConfigSize"]; }
      set { this["ConfigSize"] = value; }
    }

    // Voice Config
    [DefaultSettingValue( "" )]
    public string OutputDeviceName {
      get { return (string)this["OutputDeviceName"]; }
      set { this["OutputDeviceName"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string VoiceName {
      get { return (string)this["VoiceName"]; }
      set { this["VoiceName"] = value; }
    }

    // Voice Out config
    [DefaultSettingValue( "" )] // 0/1 list - HudVoice() order
    public string VoiceCalloutProfile {
      get { return (string)this["VoiceCalloutProfile"]; }
      set { this["VoiceCalloutProfile"] = value; }
    }

    // FRecoder Flag
    [DefaultSettingValue( "False" )]
    public bool FRecorder {
      get { return (bool)this["FRecorder"]; }
      set { this["FRecorder"] = value; }
    }

    // Backup Flag
    [DefaultSettingValue( "0" )] // ref enum FSimClientIF.FlightPlanMode  (0=Disabled, 1=AutoB, 2=AutoB+ATC)
    public int FltAutoSaveATC {
      // BREAKING CHANGE: restrict ATC to Auto
      get {
        int aMode = (int)this["FltAutoSaveATC"];
        aMode = (aMode > 1) ? 1 : aMode;
        return aMode;
      }
      set {
        this["FltAutoSaveATC"] = (value > 1) ? 1 : value;
      }
    }


    // Keyboard HK enabled flag
    [DefaultSettingValue( "False" )]
    public bool KeyboardHook {
      get { return (bool)this["KeyboardHook"]; }
      set { this["KeyboardHook"] = value; }
    }
    // InGame HK enabled flag
    [DefaultSettingValue( "False" )]
    public bool InGameHook {
      get { return (bool)this["InGameHook"]; }
      set { this["InGameHook"] = value; }
    }
    // Show Hide HudBar
    [DefaultSettingValue( "" )]
    public string HKShowHide {
      get { return (string)this["HKShowHide"]; }
      set { this["HKShowHide"] = value; }
    }
    // Sub App Hotkeys
    [DefaultSettingValue( "" )]
    public string HKCamera {
      get { return (string)this["HKCamera"]; }
      set { this["HKCamera"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string HKShelf {
      get { return (string)this["HKShelf"]; }
      set { this["HKShelf"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string HKChecklistBox {
      get { return (string)this["HKChecklistBox"]; }
      set { this["HKChecklistBox"] = value; }
    }

    // User Fonts
    [DefaultSettingValue( "" )]
    public string UserFonts {
      get { return (string)this["UserFonts"]; }
      set { this["UserFonts"] = value; }
    }
    // User Colors
    [DefaultSettingValue( "" )]
    public string UserColorsReg {
      get { return (string)this["UserColorsReg"]; }
      set { this["UserColorsReg"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string UserColorsDim {
      get { return (string)this["UserColorsDim"]; }
      set { this["UserColorsDim"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string UserColorsInv {
      get { return (string)this["UserColorsInv"]; }
      set { this["UserColorsInv"] = value; }
    }

    // selected Profile
    [DefaultSettingValue( "0" )]
    public int SelProfile {
      get { return (int)this["SelProfile"]; }
      set { this["SelProfile"] = value; }
    }

    // PROFILE 1

    [DefaultSettingValue( "Profile 1" )]
    public string Profile_1_Name {
      get { return (string)this["Profile_1_Name"]; }
      set { this["Profile_1_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_1_FontSize {
      get { return (int)this["Profile_1_FontSize"]; }
      set { this["Profile_1_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_1_Placement {
      get { return (int)this["Profile_1_Placement"]; }
      set { this["Profile_1_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_1_Kind {
      get { return (int)this["Profile_1_Kind"]; }
      set { this["Profile_1_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_1_Trans {
      get { return (int)this["Profile_1_Trans"]; }
      set { this["Profile_1_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_1_Location {
      get { return (Point)this["Profile_1_Location"]; }
      set { this["Profile_1_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_1_Condensed {
      get { return (bool)this["Profile_1_Condensed"]; }
      set { this["Profile_1_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_1 {
      get { return (string)this["Profile_1"]; }
      set { this["Profile_1"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_1 {
      get { return (string)this["FlowBreak_1"]; }
      set { this["FlowBreak_1"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_1 {
      get { return (string)this["Sequence_1"]; }
      set { this["Sequence_1"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile1 {
      get { return (string)this["HKProfile1"]; }
      set { this["HKProfile1"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_1 {
      get { return (string)this["BgImageName_1"]; }
      set { this["BgImageName_1"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_1 {
      get { return (Padding)this["BgImageArea_1"]; }
      set { this["BgImageArea_1"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_1 {
      get { return (string)this["ProfileFonts_1"]; }
      set { this["ProfileFonts_1"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_1 {
      get { return (string)this["ProfileColorsReg_1"]; }
      set { this["ProfileColorsReg_1"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_1 {
      get { return (string)this["ProfileColorsDim_1"]; }
      set { this["ProfileColorsDim_1"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_1 {
      get { return (string)this["ProfileColorsInv_1"]; }
      set { this["ProfileColorsInv_1"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_1 {
      get { return (bool)this["ProfileFrameItems_1"]; }
      set { this["ProfileFrameItems_1"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_1 {
      get { return (bool)this["ProfileBoxDivider_1"]; }
      set { this["ProfileBoxDivider_1"] = value; }
    }


    // PROFILE 2

    [DefaultSettingValue( "Profile 2" )]
    public string Profile_2_Name {
      get { return (string)this["Profile_2_Name"]; }
      set { this["Profile_2_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_2_FontSize {
      get { return (int)this["Profile_2_FontSize"]; }
      set { this["Profile_2_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_2_Placement {
      get { return (int)this["Profile_2_Placement"]; }
      set { this["Profile_2_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_2_Kind {
      get { return (int)this["Profile_2_Kind"]; }
      set { this["Profile_2_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_2_Trans {
      get { return (int)this["Profile_2_Trans"]; }
      set { this["Profile_2_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_2_Location {
      get { return (Point)this["Profile_2_Location"]; }
      set { this["Profile_2_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_2_Condensed {
      get { return (bool)this["Profile_2_Condensed"]; }
      set { this["Profile_2_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_2 {
      get { return (string)this["Profile_2"]; }
      set { this["Profile_2"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_2 {
      get { return (string)this["FlowBreak_2"]; }
      set { this["FlowBreak_2"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_2 {
      get { return (string)this["Sequence_2"]; }
      set { this["Sequence_2"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile2 {
      get { return (string)this["HKProfile2"]; }
      set { this["HKProfile2"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_2 {
      get { return (string)this["BgImageName_2"]; }
      set { this["BgImageName_2"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_2 {
      get { return (Padding)this["BgImageArea_2"]; }
      set { this["BgImageArea_2"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_2 {
      get { return (string)this["ProfileFonts_2"]; }
      set { this["ProfileFonts_2"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_2 {
      get { return (string)this["ProfileColorsReg_2"]; }
      set { this["ProfileColorsReg_2"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_2 {
      get { return (string)this["ProfileColorsDim_2"]; }
      set { this["ProfileColorsDim_2"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_2 {
      get { return (string)this["ProfileColorsInv_2"]; }
      set { this["ProfileColorsInv_2"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_2 {
      get { return (bool)this["ProfileFrameItems_2"]; }
      set { this["ProfileFrameItems_2"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_2 {
      get { return (bool)this["ProfileBoxDivider_2"]; }
      set { this["ProfileBoxDivider_2"] = value; }
    }


    // PROFILE 3

    [DefaultSettingValue( "Profile 3" )]
    public string Profile_3_Name {
      get { return (string)this["Profile_3_Name"]; }
      set { this["Profile_3_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_3_FontSize {
      get { return (int)this["Profile_3_FontSize"]; }
      set { this["Profile_3_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_3_Placement {
      get { return (int)this["Profile_3_Placement"]; }
      set { this["Profile_3_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_3_Kind {
      get { return (int)this["Profile_3_Kind"]; }
      set { this["Profile_3_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_3_Trans {
      get { return (int)this["Profile_3_Trans"]; }
      set { this["Profile_3_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_3_Location {
      get { return (Point)this["Profile_3_Location"]; }
      set { this["Profile_3_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_3_Condensed {
      get { return (bool)this["Profile_3_Condensed"]; }
      set { this["Profile_3_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_3 {
      get { return (string)this["Profile_3"]; }
      set { this["Profile_3"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_3 {
      get { return (string)this["FlowBreak_3"]; }
      set { this["FlowBreak_3"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_3 {
      get { return (string)this["Sequence_3"]; }
      set { this["Sequence_3"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile3 {
      get { return (string)this["HKProfile3"]; }
      set { this["HKProfile3"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_3 {
      get { return (string)this["BgImageName_3"]; }
      set { this["BgImageName_3"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_3 {
      get { return (Padding)this["BgImageArea_3"]; }
      set { this["BgImageArea_3"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_3 {
      get { return (string)this["ProfileFonts_3"]; }
      set { this["ProfileFonts_3"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_3 {
      get { return (string)this["ProfileColorsReg_3"]; }
      set { this["ProfileColorsReg_3"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_3 {
      get { return (string)this["ProfileColorsDim_3"]; }
      set { this["ProfileColorsDim_3"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_3 {
      get { return (string)this["ProfileColorsInv_3"]; }
      set { this["ProfileColorsInv_3"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_3 {
      get { return (bool)this["ProfileFrameItems_3"]; }
      set { this["ProfileFrameItems_3"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_3 {
      get { return (bool)this["ProfileBoxDivider_3"]; }
      set { this["ProfileBoxDivider_3"] = value; }
    }


    // PROFILE 4

    [DefaultSettingValue( "Profile 4" )]
    public string Profile_4_Name {
      get { return (string)this["Profile_4_Name"]; }
      set { this["Profile_4_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_4_FontSize {
      get { return (int)this["Profile_4_FontSize"]; }
      set { this["Profile_4_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_4_Placement {
      get { return (int)this["Profile_4_Placement"]; }
      set { this["Profile_4_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_4_Kind {
      get { return (int)this["Profile_4_Kind"]; }
      set { this["Profile_4_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_4_Trans {
      get { return (int)this["Profile_4_Trans"]; }
      set { this["Profile_4_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_4_Location {
      get { return (Point)this["Profile_4_Location"]; }
      set { this["Profile_4_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_4_Condensed {
      get { return (bool)this["Profile_4_Condensed"]; }
      set { this["Profile_4_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_4 {
      get { return (string)this["Profile_4"]; }
      set { this["Profile_4"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_4 {
      get { return (string)this["FlowBreak_4"]; }
      set { this["FlowBreak_4"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_4 {
      get { return (string)this["Sequence_4"]; }
      set { this["Sequence_4"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile4 {
      get { return (string)this["HKProfile4"]; }
      set { this["HKProfile4"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_4 {
      get { return (string)this["BgImageName_4"]; }
      set { this["BgImageName_4"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_4 {
      get { return (Padding)this["BgImageArea_4"]; }
      set { this["BgImageArea_4"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_4 {
      get { return (string)this["ProfileFonts_4"]; }
      set { this["ProfileFonts_4"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_4 {
      get { return (string)this["ProfileColorsReg_4"]; }
      set { this["ProfileColorsReg_4"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_4 {
      get { return (string)this["ProfileColorsDim_4"]; }
      set { this["ProfileColorsDim_4"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_4 {
      get { return (string)this["ProfileColorsInv_4"]; }
      set { this["ProfileColorsInv_4"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_4 {
      get { return (bool)this["ProfileFrameItems_4"]; }
      set { this["ProfileFrameItems_4"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_4 {
      get { return (bool)this["ProfileBoxDivider_4"]; }
      set { this["ProfileBoxDivider_4"] = value; }
    }


    // PROFILE 5

    [DefaultSettingValue( "Profile 5" )]
    public string Profile_5_Name {
      get { return (string)this["Profile_5_Name"]; }
      set { this["Profile_5_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_5_FontSize {
      get { return (int)this["Profile_5_FontSize"]; }
      set { this["Profile_5_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_5_Placement {
      get { return (int)this["Profile_5_Placement"]; }
      set { this["Profile_5_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_5_Kind {
      get { return (int)this["Profile_5_Kind"]; }
      set { this["Profile_5_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_5_Trans {
      get { return (int)this["Profile_5_Trans"]; }
      set { this["Profile_5_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_5_Location {
      get { return (Point)this["Profile_5_Location"]; }
      set { this["Profile_5_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_5_Condensed {
      get { return (bool)this["Profile_5_Condensed"]; }
      set { this["Profile_5_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_5 {
      get { return (string)this["Profile_5"]; }
      set { this["Profile_5"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_5 {
      get { return (string)this["FlowBreak_5"]; }
      set { this["FlowBreak_5"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_5 {
      get { return (string)this["Sequence_5"]; }
      set { this["Sequence_5"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile5 {
      get { return (string)this["HKProfile5"]; }
      set { this["HKProfile5"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_5 {
      get { return (string)this["BgImageName_5"]; }
      set { this["BgImageName_5"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_5 {
      get { return (Padding)this["BgImageArea_5"]; }
      set { this["BgImageArea_5"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_5 {
      get { return (string)this["ProfileFonts_5"]; }
      set { this["ProfileFonts_5"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_5 {
      get { return (string)this["ProfileColorsReg_5"]; }
      set { this["ProfileColorsReg_5"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_5 {
      get { return (string)this["ProfileColorsDim_5"]; }
      set { this["ProfileColorsDim_5"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_5 {
      get { return (string)this["ProfileColorsInv_5"]; }
      set { this["ProfileColorsInv_5"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_5 {
      get { return (bool)this["ProfileFrameItems_5"]; }
      set { this["ProfileFrameItems_5"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_5 {
      get { return (bool)this["ProfileBoxDivider_5"]; }
      set { this["ProfileBoxDivider_5"] = value; }
    }


    // PROFILE 6

    [DefaultSettingValue( "Profile 6" )]
    public string Profile_6_Name {
      get { return (string)this["Profile_6_Name"]; }
      set { this["Profile_6_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_6_FontSize {
      get { return (int)this["Profile_6_FontSize"]; }
      set { this["Profile_6_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_6_Placement {
      get { return (int)this["Profile_6_Placement"]; }
      set { this["Profile_6_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_6_Kind {
      get { return (int)this["Profile_6_Kind"]; }
      set { this["Profile_6_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_6_Trans {
      get { return (int)this["Profile_6_Trans"]; }
      set { this["Profile_6_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_6_Location {
      get { return (Point)this["Profile_6_Location"]; }
      set { this["Profile_6_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_6_Condensed {
      get { return (bool)this["Profile_6_Condensed"]; }
      set { this["Profile_6_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_6 {
      get { return (string)this["Profile_6"]; }
      set { this["Profile_6"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_6 {
      get { return (string)this["FlowBreak_6"]; }
      set { this["FlowBreak_6"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_6 {
      get { return (string)this["Sequence_6"]; }
      set { this["Sequence_6"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile6 {
      get { return (string)this["HKProfile6"]; }
      set { this["HKProfile6"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_6 {
      get { return (string)this["BgImageName_6"]; }
      set { this["BgImageName_6"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_6 {
      get { return (Padding)this["BgImageArea_6"]; }
      set { this["BgImageArea_6"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_6 {
      get { return (string)this["ProfileFonts_6"]; }
      set { this["ProfileFonts_6"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_6 {
      get { return (string)this["ProfileColorsReg_6"]; }
      set { this["ProfileColorsReg_6"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_6 {
      get { return (string)this["ProfileColorsDim_6"]; }
      set { this["ProfileColorsDim_6"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_6 {
      get { return (string)this["ProfileColorsInv_6"]; }
      set { this["ProfileColorsInv_6"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_6 {
      get { return (bool)this["ProfileFrameItems_6"]; }
      set { this["ProfileFrameItems_6"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_6 {
      get { return (bool)this["ProfileBoxDivider_6"]; }
      set { this["ProfileBoxDivider_6"] = value; }
    }


    // PROFILE 7

    [DefaultSettingValue( "Profile 7" )]
    public string Profile_7_Name {
      get { return (string)this["Profile_7_Name"]; }
      set { this["Profile_7_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_7_FontSize {
      get { return (int)this["Profile_7_FontSize"]; }
      set { this["Profile_7_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_7_Placement {
      get { return (int)this["Profile_7_Placement"]; }
      set { this["Profile_7_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_7_Kind {
      get { return (int)this["Profile_7_Kind"]; }
      set { this["Profile_7_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_7_Trans {
      get { return (int)this["Profile_7_Trans"]; }
      set { this["Profile_7_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_7_Location {
      get { return (Point)this["Profile_7_Location"]; }
      set { this["Profile_7_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_7_Condensed {
      get { return (bool)this["Profile_7_Condensed"]; }
      set { this["Profile_7_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_7 {
      get { return (string)this["Profile_7"]; }
      set { this["Profile_7"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_7 {
      get { return (string)this["FlowBreak_7"]; }
      set { this["FlowBreak_7"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_7 {
      get { return (string)this["Sequence_7"]; }
      set { this["Sequence_7"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile7 {
      get { return (string)this["HKProfile7"]; }
      set { this["HKProfile7"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_7 {
      get { return (string)this["BgImageName_7"]; }
      set { this["BgImageName_7"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_7 {
      get { return (Padding)this["BgImageArea_7"]; }
      set { this["BgImageArea_7"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_7 {
      get { return (string)this["ProfileFonts_7"]; }
      set { this["ProfileFonts_7"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_7 {
      get { return (string)this["ProfileColorsReg_7"]; }
      set { this["ProfileColorsReg_7"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_7 {
      get { return (string)this["ProfileColorsDim_7"]; }
      set { this["ProfileColorsDim_7"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_7 {
      get { return (string)this["ProfileColorsInv_7"]; }
      set { this["ProfileColorsInv_7"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_7 {
      get { return (bool)this["ProfileFrameItems_7"]; }
      set { this["ProfileFrameItems_7"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_7 {
      get { return (bool)this["ProfileBoxDivider_7"]; }
      set { this["ProfileBoxDivider_7"] = value; }
    }


    // PROFILE 8

    [DefaultSettingValue( "Profile 8" )]
    public string Profile_8_Name {
      get { return (string)this["Profile_8_Name"]; }
      set { this["Profile_8_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_8_FontSize {
      get { return (int)this["Profile_8_FontSize"]; }
      set { this["Profile_8_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_8_Placement {
      get { return (int)this["Profile_8_Placement"]; }
      set { this["Profile_8_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_8_Kind {
      get { return (int)this["Profile_8_Kind"]; }
      set { this["Profile_8_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_8_Trans {
      get { return (int)this["Profile_8_Trans"]; }
      set { this["Profile_8_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_8_Location {
      get { return (Point)this["Profile_8_Location"]; }
      set { this["Profile_8_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_8_Condensed {
      get { return (bool)this["Profile_8_Condensed"]; }
      set { this["Profile_8_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_8 {
      get { return (string)this["Profile_8"]; }
      set { this["Profile_8"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_8 {
      get { return (string)this["FlowBreak_8"]; }
      set { this["FlowBreak_8"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_8 {
      get { return (string)this["Sequence_8"]; }
      set { this["Sequence_8"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile8 {
      get { return (string)this["HKProfile8"]; }
      set { this["HKProfile8"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_8 {
      get { return (string)this["BgImageName_8"]; }
      set { this["BgImageName_8"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_8 {
      get { return (Padding)this["BgImageArea_8"]; }
      set { this["BgImageArea_8"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_8 {
      get { return (string)this["ProfileFonts_8"]; }
      set { this["ProfileFonts_8"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_8 {
      get { return (string)this["ProfileColorsReg_8"]; }
      set { this["ProfileColorsReg_8"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_8 {
      get { return (string)this["ProfileColorsDim_8"]; }
      set { this["ProfileColorsDim_8"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_8 {
      get { return (string)this["ProfileColorsInv_8"]; }
      set { this["ProfileColorsInv_8"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_8 {
      get { return (bool)this["ProfileFrameItems_8"]; }
      set { this["ProfileFrameItems_8"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_8 {
      get { return (bool)this["ProfileBoxDivider_8"]; }
      set { this["ProfileBoxDivider_8"] = value; }
    }


    // PROFILE 9

    [DefaultSettingValue( "Profile 9" )]
    public string Profile_9_Name {
      get { return (string)this["Profile_9_Name"]; }
      set { this["Profile_9_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_9_FontSize {
      get { return (int)this["Profile_9_FontSize"]; }
      set { this["Profile_9_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_9_Placement {
      get { return (int)this["Profile_9_Placement"]; }
      set { this["Profile_9_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_9_Kind {
      get { return (int)this["Profile_9_Kind"]; }
      set { this["Profile_9_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_9_Trans {
      get { return (int)this["Profile_9_Trans"]; }
      set { this["Profile_9_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_9_Location {
      get { return (Point)this["Profile_9_Location"]; }
      set { this["Profile_9_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_9_Condensed {
      get { return (bool)this["Profile_9_Condensed"]; }
      set { this["Profile_9_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_9 {
      get { return (string)this["Profile_9"]; }
      set { this["Profile_9"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_9 {
      get { return (string)this["FlowBreak_9"]; }
      set { this["FlowBreak_9"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_9 {
      get { return (string)this["Sequence_9"]; }
      set { this["Sequence_9"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile9 {
      get { return (string)this["HKProfile9"]; }
      set { this["HKProfile9"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_9 {
      get { return (string)this["BgImageName_9"]; }
      set { this["BgImageName_9"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_9 {
      get { return (Padding)this["BgImageArea_9"]; }
      set { this["BgImageArea_9"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_9 {
      get { return (string)this["ProfileFonts_9"]; }
      set { this["ProfileFonts_9"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_9 {
      get { return (string)this["ProfileColorsReg_9"]; }
      set { this["ProfileColorsReg_9"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_9 {
      get { return (string)this["ProfileColorsDim_9"]; }
      set { this["ProfileColorsDim_9"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_9 {
      get { return (string)this["ProfileColorsInv_9"]; }
      set { this["ProfileColorsInv_9"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_9 {
      get { return (bool)this["ProfileFrameItems_9"]; }
      set { this["ProfileFrameItems_9"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_9 {
      get { return (bool)this["ProfileBoxDivider_9"]; }
      set { this["ProfileBoxDivider_9"] = value; }
    }


    // PROFILE 10

    [DefaultSettingValue( "Profile 10" )]
    public string Profile_10_Name {
      get { return (string)this["Profile_10_Name"]; }
      set { this["Profile_10_Name"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_10_FontSize {
      get { return (int)this["Profile_10_FontSize"]; }
      set { this["Profile_10_FontSize"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_10_Placement {
      get { return (int)this["Profile_10_Placement"]; }
      set { this["Profile_10_Placement"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_10_Kind {
      get { return (int)this["Profile_10_Kind"]; }
      set { this["Profile_10_Kind"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Profile_10_Trans {
      get { return (int)this["Profile_10_Trans"]; }
      set { this["Profile_10_Trans"] = value; }
    }

    [DefaultSettingValue( "0, 0" )]
    public Point Profile_10_Location {
      get { return (Point)this["Profile_10_Location"]; }
      set { this["Profile_10_Location"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool Profile_10_Condensed {
      get { return (bool)this["Profile_10_Condensed"]; }
      set { this["Profile_10_Condensed"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Profile_10 {
      get { return (string)this["Profile_10"]; }
      set { this["Profile_10"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string FlowBreak_10 {
      get { return (string)this["FlowBreak_10"]; }
      set { this["FlowBreak_10"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string Sequence_10 {
      get { return (string)this["Sequence_10"]; }
      set { this["Sequence_10"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string HKProfile10 {
      get { return (string)this["HKProfile10"]; }
      set { this["HKProfile10"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240224
    public string BgImageName_10 {
      get { return (string)this["BgImageName_10"]; }
      set { this["BgImageName_10"] = value; }
    }
    [DefaultSettingValue( "0, 0, 0, 0" )] // 20240224
    public Padding BgImageArea_10 {
      get { return (Padding)this["BgImageArea_10"]; }
      set { this["BgImageArea_10"] = value; }
    }

    [DefaultSettingValue( "" )] // 20240226
    public string ProfileFonts_10 {
      get { return (string)this["ProfileFonts_10"]; }
      set { this["ProfileFonts_10"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsReg_10 {
      get { return (string)this["ProfileColorsReg_10"]; }
      set { this["ProfileColorsReg_10"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsDim_10 {
      get { return (string)this["ProfileColorsDim_10"]; }
      set { this["ProfileColorsDim_10"] = value; }
    }
    [DefaultSettingValue( "" )] // 20240226
    public string ProfileColorsInv_10 {
      get { return (string)this["ProfileColorsInv_10"]; }
      set { this["ProfileColorsInv_10"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileFrameItems_10 {
      get { return (bool)this["ProfileFrameItems_10"]; }
      set { this["ProfileFrameItems_10"] = value; }
    }
    [DefaultSettingValue( "False" )]
    public bool ProfileBoxDivider_10 {
      get { return (bool)this["ProfileBoxDivider_10"]; }
      set { this["ProfileBoxDivider_10"] = value; }
    }


    #endregion // ConfigSettings

    #endregion // Settings

  }
}
