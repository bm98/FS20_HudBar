using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  sealed class AppSettings : ApplicationSettingsBase
  {

    // Singleton
    private static readonly Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( () => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }

    private AppSettings( )
    {
      if ( this.FirstRun ) {
        // migrate the settings to the new version if the app runs the first time
        try {
          this.Upgrade( );
          // V 0.18 TO V 0.2
          // upgrade from prev to add the Fuel GPH setting (hide the new item)
          string p = (string)this.GetPreviousVersion( "Profile_1" );
          if ( p.Length == 78 ) {
            p = p.Insert( 19 * 2, "0;" ); // insert gph at GPS_WYP Pos and shift all one item
            this.Profile_1 = p;
          }
          p = (string)this.GetPreviousVersion( "Profile_2" );
          if ( p.Length == 78 ) {
            p = p.Insert( 19 * 2, "0;" );
            this.Profile_2 = p;
          }
          p = (string)this.GetPreviousVersion( "Profile_3" );
          if ( p.Length == 78 ) {
            p = p.Insert( 19 * 2, "0;" );
            this.Profile_3 = p;
          }
          p = (string)this.GetPreviousVersion( "Profile_4" );
          if ( p.Length == 78 ) {
            p = p.Insert( 19 * 2, "0;" );
            this.Profile_4 = p;
          }
          p = (string)this.GetPreviousVersion( "Profile_5" );
          if ( p.Length == 78 ) {
            p = p.Insert( 19 * 2, "0;" );
            this.Profile_5 = p;
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

    // User Config Settings

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool ShowUnits {
      get { return (bool)this["ShowUnits"]; }
      set { this["ShowUnits"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool Opaque {
      get { return (bool)this["Opaque"]; }
      set { this["Opaque"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "10" )]
    public int SplitDistance {
      get { return (int)this["SplitDistance"]; }
      set { this["SplitDistance"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "0" )]
    public int SelProfile {
      get { return (int)this["SelProfile"]; }
      set { this["SelProfile"] = value; }
    }


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
    [DefaultSettingValue( "0, 0" )]
    public Point Profile_1_Location {
      get { return (Point)this["Profile_1_Location"]; }
      set { this["Profile_1_Location"] = value; }
    }

    [UserScopedSetting( )]
    [DefaultSettingValue( "False" )]
    public bool  Profile_1_Condensed {
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
