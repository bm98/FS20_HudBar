using System;
using System.Drawing;

using SettingsLib;
using FS20_HudBar.Config;

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

    [DefaultSettingValue( "" )]
    public string HKShowHide {
      get { return (string)this["HKShowHide"]; }
      set { this["HKShowHide"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool KeyboardHook {
      get { return (bool)this["KeyboardHook"]; }
      set { this["KeyboardHook"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool InGameHook {
      get { return (bool)this["InGameHook"]; }
      set { this["InGameHook"] = value; }
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


    // User Config Settings
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


    [DefaultSettingValue( "False" )]
    public bool FRecorder {
      get { return (bool)this["FRecorder"]; }
      set { this["FRecorder"] = value; }
    }


    [DefaultSettingValue( "0" )] // ref enum FSimClientIF.FlightPlanMode  (0=Disabled, 1=AutoB, 2=AutoB+ATC)
    public int FltAutoSaveATC {
      get { return (int)this["FltAutoSaveATC"]; }
      set { this["FltAutoSaveATC"] = value; }
    }


    [DefaultSettingValue( "" )]
    public string VoiceName {
      get { return (string)this["VoiceName"]; }
      set { this["VoiceName"] = value; }
    }


    [DefaultSettingValue( "" )]
    public string OutputDeviceName {
      get { return (string)this["OutputDeviceName"]; }
      set { this["OutputDeviceName"] = value; }
    }


    [DefaultSettingValue( "" )]
    public string VoiceCalloutProfile {
      get { return (string)this["VoiceCalloutProfile"]; }
      set { this["VoiceCalloutProfile"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int SelProfile {
      get { return (int)this["SelProfile"]; }
      set { this["SelProfile"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int Appearance {
      get { return (int)this["Appearance"]; }
      set { this["Appearance"] = value; }
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

    #endregion

  }
}
