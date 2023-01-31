using System;
using System.Drawing;

using SettingsLib;

namespace FShelf
{
  internal sealed class AppSettings : AppSettingsBaseV2
  {
    // Singleton
    private static Lazy<AppSettings> m_lazy = new Lazy<AppSettings>( ( ) => new AppSettings( ) );
    public static AppSettings Instance { get => m_lazy.Value; }
    private AppSettings( ) { }


    /// <summary>
    /// Init a new instance to use
    /// </summary>
    /// <param name="settingsFile">An filename -> distinct for each instance used</param>
    /// <param name="instance">An instance name (use "" as default)</param>
    public static void InitInstance( string settingsFile, string instance )
    {
      m_lazy = new Lazy<AppSettings>( ( ) => new AppSettings( settingsFile, instance ) );
    }
    /// <summary>
    /// cTor: create an instance if such a name is given, else use the default one
    /// </summary>
    /// <param name="settingsFile">An filename -> distinct for each instance used</param>
    /// <param name="instance">An Instance String (defaults to empty = default instance)</param>
    private AppSettings( string settingsFile, string instance ) : base( settingsFile, instance )
    {

      // start processing here 
      if (this.FirstRun) {

        // something

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

    // Shelf Settings
    [DefaultSettingValue( "10, 10" )]
    public Point ShelfLocation {
      get { return (Point)this["ShelfLocation"]; }
      set { this["ShelfLocation"] = value; }
    }
    [DefaultSettingValue( "450, 280" )]
    public Size ShelfSize {
      get { return (Size)this["ShelfSize"]; }
      set { this["ShelfSize"] = value; }
    }

    [DefaultSettingValue( @"" )]
    public string ShelfFolder {
      get { return (string)this["ShelfFolder"]; }
      set { this["ShelfFolder"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool PrettyMetar {
      get { return (bool)this["PrettyMetar"]; }
      set { this["PrettyMetar"] = value; }
    }


    [DefaultSettingValue( "" )]
    public string SbPilotID {
      get { return (string)this["SbPilotID"]; }
      set { this["SbPilotID"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string LastMsfsPlan {
      get { return (string)this["LastMsfsPlan"]; }
      set { this["LastMsfsPlan"] = value; }
    }


    [DefaultSettingValue( "True" )]
    public bool WeightLbs {
      get { return (bool)this["WeightLbs"]; }
      set { this["WeightLbs"] = value; }
    }


    [DefaultSettingValue( "Notepad text" )]
    public string NotePadText {
      get { return (string)this["NotePadText"]; }
      set { this["NotePadText"] = value; }
    }

    [DefaultSettingValue( "" )]
    public string DepICAO {
      get { return (string)this["DepICAO"]; }
      set { this["DepICAO"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string ArrICAO {
      get { return (string)this["ArrICAO"]; }
      set { this["ArrICAO"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool MapGrid {
      get { return (bool)this["MapGrid"]; }
      set { this["MapGrid"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AirportRings {
      get { return (bool)this["AirportRings"]; }
      set { this["AirportRings"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool FlightplanRoute {
      get { return (bool)this["FlightplanRoute"]; }
      set { this["FlightplanRoute"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool VorNdb {
      get { return (bool)this["VorNdb"]; }
      set { this["VorNdb"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool IFRwaypoints {
      get { return (bool)this["IFRwaypoints"]; }
      set { this["IFRwaypoints"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool VFRmarks {
      get { return (bool)this["VFRmarks"]; }
      set { this["VFRmarks"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AptMarks {
      get { return (bool)this["AptMarks"]; }
      set { this["AptMarks"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AcftRange {
      get { return (bool)this["AcftRange"]; }
      set { this["AcftRange"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AcftWind {
      get { return (bool)this["AcftWind"]; }
      set { this["AcftWind"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AcftMark {
      get { return (bool)this["AcftMark"]; }
      set { this["AcftMark"] = value; }
    }

    [DefaultSettingValue( "False" )]
    public bool AcftTrack {
      get { return (bool)this["AcftTrack"]; }
      set { this["AcftTrack"] = value; }
    }

    [DefaultSettingValue( "0" )]
    public int MinRwyLengthCombo {
      get { return (int)this["MinRwyLengthCombo"]; }
      set { this["MinRwyLengthCombo"] = value; }
    }


    #endregion

  }
}
