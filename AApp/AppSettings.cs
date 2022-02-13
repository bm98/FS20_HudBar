﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Config;

namespace FS20_HudBar
{
  sealed class AppSettings : ApplicationSettingsBase
  {
    // Singleton
    private static Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( () => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }

    /// <summary>
    /// Init a new instance to use
    /// </summary>
    /// <param name="instance">An Instance String -> distinct for each instance used</param>
    public static void InitInstance( string instance )
    {
      m_lazy = new Lazy<AppSettings>( ( ) => new AppSettings( instance ) );
    }

    /// <summary>
    /// cTor: create an instance if such a name is given, else use the default one
    /// </summary>
    /// <param name="instance">An Instance String (defaults to empty = default instance)</param>
    private AppSettings( string instance = "" )
    {
      // init an instance if given, else use the default instance
      if ( !string.IsNullOrEmpty( instance ) ) {
        base.SettingsKey = instance;
      }

      // start processing here 
      if ( this.FirstRun ) {
        // migrate the settings to the new version if the app runs the first time
        try {
          this.Upgrade( );
          // set profiles when no previous setting is available
          string p = (string)this.GetPreviousVersion( "Profile_1" );
          if ( p == null ) {
            var dprofile =  DefaultProfiles.GetDefaultProfile(DProfile.Profile_1);
            this.Profile_1_Name = dprofile.Name;
            this.Profile_1 = dprofile.Profile;
            this.FlowBreak_1 = dprofile.FlowBreak;
            this.Sequence_1 = dprofile.DispOrder;
          }
          p = (string)this.GetPreviousVersion( "Profile_2" );
          if ( p == null ) {
            var dprofile =  DefaultProfiles.GetDefaultProfile(DProfile.Profile_2);
            this.Profile_2_Name = dprofile.Name;
            this.Profile_2 = dprofile.Profile;
            this.FlowBreak_2 = dprofile.FlowBreak;
            this.Sequence_2 = dprofile.DispOrder;
          }
          p = (string)this.GetPreviousVersion( "Profile_3" );
          if ( p == null ) {
            var dprofile =  DefaultProfiles.GetDefaultProfile(DProfile.Profile_3);
            this.Profile_3_Name = dprofile.Name;
            this.Profile_3 = dprofile.Profile;
            this.FlowBreak_3 = dprofile.FlowBreak;
            this.Sequence_3 = dprofile.DispOrder;
          }
          p = (string)this.GetPreviousVersion( "Profile_4" );
          if ( p == null ) {
            var dprofile =  DefaultProfiles.GetDefaultProfile(DProfile.Profile_4);
            this.Profile_4_Name = dprofile.Name;
            this.Profile_4 = dprofile.Profile;
            this.FlowBreak_4 = dprofile.FlowBreak;
            this.Sequence_4 = dprofile.DispOrder;
          }
          p = (string)this.GetPreviousVersion( "Profile_5" );
          if ( p == null ) {
            var dprofile =  DefaultProfiles.GetDefaultProfile(DProfile.Profile_5);
            this.Profile_5_Name = dprofile.Name;
            this.Profile_5 = dprofile.Profile;
            this.FlowBreak_5 = dprofile.FlowBreak;
            this.Sequence_5 = dprofile.DispOrder;
          }
          // update for the new AutoSave mode
          try {
            int i = (int)this.GetPreviousVersion("FltAutoSaveATC");
          }
          catch {
            // no old value - try to derive from the Bool Switch if there is one
            bool b = (bool)this.GetPreviousVersion("FltAutoSave"); // if this fails we use the built in default
            this.FltAutoSaveATC = b ? 2 : 0; // ATC save or disabled
          } 

        }
        catch { }
        this.FirstRun = false;
        this.Save( );
      }
    }

    #region Setting Properties

    // manages Upgrade
    [UserScopedSetting( )]
    [DefaultSettingValue( "True" )]
    public bool FirstRun {
      get { return (bool)this["FirstRun"]; }
      set { this["FirstRun"] = value; }
    }


    // Control bound settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point FormLocation {
      get { return (Point)this["FormLocation"]; }
      set { this["FormLocation"] = value; }
    }

    // Shelf Settings
    [UserScopedSetting( )]
    [DefaultSettingValue( "10, 10" )]
    public Point ShelfLocation {
      get { return (Point)this["ShelfLocation"]; }
      set { this["ShelfLocation"] = value; }
    }
    [UserScopedSetting( )]
    [DefaultSettingValue( "450, 280" )]
    public Size ShelfSize {
      get { return (Size)this["ShelfSize"]; }
      set { this["ShelfSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( @".\DemoBag" )]
    public string ShelfFolder {
      get { return (string)this["ShelfFolder"]; }
      set { this["ShelfFolder"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKShelf {
      get { return (string)this["HKShelf"]; }
      set { this["HKShelf"] = value; }
    }

    // User Config Settings

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ShowUnits {
      get { return (bool)this["ShowUnits"]; }
      set { this["ShowUnits"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKShowHide {
      get { return (string)this["HKShowHide"]; }
      set { this["HKShowHide"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKProfile1 {
      get { return (string)this["HKProfile1"]; }
      set { this["HKProfile1"] = value; }
    }
    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKProfile2 {
      get { return (string)this["HKProfile2"]; }
      set { this["HKProfile2"] = value; }
    }
    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKProfile3 {
      get { return (string)this["HKProfile3"]; }
      set { this["HKProfile3"] = value; }
    }
    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKProfile4 {
      get { return (string)this["HKProfile4"]; }
      set { this["HKProfile4"] = value; }
    }
    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string HKProfile5 {
      get { return (string)this["HKProfile5"]; }
      set { this["HKProfile5"] = value; }
    }


    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool KeyboardHook {
      get { return (bool)this["KeyboardHook"]; }
      set { this["KeyboardHook"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool InGameHook {
      get { return (bool)this["InGameHook"]; }
      set { this["InGameHook"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool FRecorder {
      get { return (bool)this["FRecorder"]; }
      set { this["FRecorder"] = value; }
    }

    // Obsolete - no longer used - replaced with FltAutoSaveATC
    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool FltAutoSave {
      get { return (bool)this["FltAutoSave"]; }
      set { this["FltAutoSave"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )] // ref enum FSimClientIF.FlightPlanMode  (0=Disabled, 1=AutoB, 2=AutoB+ATC)
    public int FltAutoSaveATC {
      get { return (int)this["FltAutoSaveATC"]; }
      set { this["FltAutoSaveATC"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string VoiceName {
      get { return (string)this["VoiceName"]; }
      set { this["VoiceName"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string VoiceCalloutProfile {
      get { return (string)this["VoiceCalloutProfile"]; }
      set { this["VoiceCalloutProfile"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int SelProfile {
      get { return (int)this["SelProfile"]; }
      set { this["SelProfile"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Appearance {
      get { return (int)this["Appearance"]; }
      set { this["Appearance"] = value; }
    }


    // PROFILE 1

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 1" )]
    public string Profile_1_Name {
      get { return (string)this["Profile_1_Name"]; }
      set { this["Profile_1_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_1_FontSize {
      get { return (int)this["Profile_1_FontSize"]; }
      set { this["Profile_1_FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_1_Placement {
      get { return (int)this["Profile_1_Placement"]; }
      set { this["Profile_1_Placement"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_1_Kind {
      get { return (int)this["Profile_1_Kind"]; }
      set { this["Profile_1_Kind"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_1_Trans {
      get { return (int)this["Profile_1_Trans"]; }
      set { this["Profile_1_Trans"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_1_Location {
      get { return (Point)this["Profile_1_Location"]; }
      set { this["Profile_1_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Profile_1_Condensed {
      get { return (bool)this["Profile_1_Condensed"]; }
      set { this["Profile_1_Condensed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_1 {
      get { return (string)this["Profile_1"]; }
      set { this["Profile_1"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FlowBreak_1 {
      get { return (string)this["FlowBreak_1"]; }
      set { this["FlowBreak_1"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Sequence_1 {
      get { return (string)this["Sequence_1"]; }
      set { this["Sequence_1"] = value; }
    }

    // PROFILE 2

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 2" )]
    public string Profile_2_Name {
      get { return (string)this["Profile_2_Name"]; }
      set { this["Profile_2_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_2_FontSize {
      get { return (int)this["Profile_2_FontSize"]; }
      set { this["Profile_2_FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_2_Placement {
      get { return (int)this["Profile_2_Placement"]; }
      set { this["Profile_2_Placement"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_2_Kind {
      get { return (int)this["Profile_2_Kind"]; }
      set { this["Profile_2_Kind"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_2_Trans {
      get { return (int)this["Profile_2_Trans"]; }
      set { this["Profile_2_Trans"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_2_Location {
      get { return (Point)this["Profile_2_Location"]; }
      set { this["Profile_2_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Profile_2_Condensed {
      get { return (bool)this["Profile_2_Condensed"]; }
      set { this["Profile_2_Condensed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_2 {
      get { return (string)this["Profile_2"]; }
      set { this["Profile_2"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FlowBreak_2 {
      get { return (string)this["FlowBreak_2"]; }
      set { this["FlowBreak_2"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Sequence_2 {
      get { return (string)this["Sequence_2"]; }
      set { this["Sequence_2"] = value; }
    }

    // PROFILE 3

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 3" )]
    public string Profile_3_Name {
      get { return (string)this["Profile_3_Name"]; }
      set { this["Profile_3_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_3_FontSize {
      get { return (int)this["Profile_3_FontSize"]; }
      set { this["Profile_3_FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_3_Placement {
      get { return (int)this["Profile_3_Placement"]; }
      set { this["Profile_3_Placement"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_3_Kind {
      get { return (int)this["Profile_3_Kind"]; }
      set { this["Profile_3_Kind"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_3_Trans {
      get { return (int)this["Profile_3_Trans"]; }
      set { this["Profile_3_Trans"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_3_Location {
      get { return (Point)this["Profile_3_Location"]; }
      set { this["Profile_3_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Profile_3_Condensed {
      get { return (bool)this["Profile_3_Condensed"]; }
      set { this["Profile_3_Condensed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_3 {
      get { return (string)this["Profile_3"]; }
      set { this["Profile_3"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FlowBreak_3 {
      get { return (string)this["FlowBreak_3"]; }
      set { this["FlowBreak_3"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Sequence_3 {
      get { return (string)this["Sequence_3"]; }
      set { this["Sequence_3"] = value; }
    }

    // PROFILE 4

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 4" )]
    public string Profile_4_Name {
      get { return (string)this["Profile_4_Name"]; }
      set { this["Profile_4_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_4_FontSize {
      get { return (int)this["Profile_4_FontSize"]; }
      set { this["Profile_4_FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_4_Placement {
      get { return (int)this["Profile_4_Placement"]; }
      set { this["Profile_4_Placement"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_4_Kind {
      get { return (int)this["Profile_4_Kind"]; }
      set { this["Profile_4_Kind"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_4_Trans {
      get { return (int)this["Profile_4_Trans"]; }
      set { this["Profile_4_Trans"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_4_Location {
      get { return (Point)this["Profile_4_Location"]; }
      set { this["Profile_4_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Profile_4_Condensed {
      get { return (bool)this["Profile_4_Condensed"]; }
      set { this["Profile_4_Condensed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_4 {
      get { return (string)this["Profile_4"]; }
      set { this["Profile_4"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FlowBreak_4 {
      get { return (string)this["FlowBreak_4"]; }
      set { this["FlowBreak_4"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Sequence_4 {
      get { return (string)this["Sequence_4"]; }
      set { this["Sequence_4"] = value; }
    }

    // PROFILE 5

    [UserScopedSetting( )]
    [DefaultSettingValue( "Profile 5" )]
    public string Profile_5_Name {
      get { return (string)this["Profile_5_Name"]; }
      set { this["Profile_5_Name"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_5_FontSize {
      get { return (int)this["Profile_5_FontSize"]; }
      set { this["Profile_5_FontSize"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_5_Placement {
      get { return (int)this["Profile_5_Placement"]; }
      set { this["Profile_5_Placement"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_5_Kind {
      get { return (int)this["Profile_5_Kind"]; }
      set { this["Profile_5_Kind"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int Profile_5_Trans {
      get { return (int)this["Profile_5_Trans"]; }
      set { this["Profile_5_Trans"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_5_Location {
      get { return (Point)this["Profile_5_Location"]; }
      set { this["Profile_5_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Profile_5_Condensed {
      get { return (bool)this["Profile_5_Condensed"]; }
      set { this["Profile_5_Condensed"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Profile_5 {
      get { return (string)this["Profile_5"]; }
      set { this["Profile_5"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string FlowBreak_5 {
      get { return (string)this["FlowBreak_5"]; }
      set { this["FlowBreak_5"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "" )]
    public string Sequence_5 {
      get { return (string)this["Sequence_5"]; }
      set { this["Sequence_5"] = value; }
    }

    #endregion


  }
}
