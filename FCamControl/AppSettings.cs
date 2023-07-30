using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SettingsLib;

namespace FCamControl
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

    // Camera Settings
    [DefaultSettingValue( "10, 10" )]
    public Point CameraLocation {
      get { return (Point)this["CameraLocation"]; }
      set { this["CameraLocation"] = value; }
    }
    [DefaultSettingValue( "474, 584" )]
    public Size CameraSize {
      get { return (Size)this["CameraSize"]; }
      set { this["CameraSize"] = value; }
    }

    // camera SlotFolders 0..N
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder0 {
      get { return (string)this["CameraSlotFolder0"]; }
      set { this["CameraSlotFolder0"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder1 {
      get { return (string)this["CameraSlotFolder1"]; }
      set { this["CameraSlotFolder1"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder2 {
      get { return (string)this["CameraSlotFolder2"]; }
      set { this["CameraSlotFolder2"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder3 {
      get { return (string)this["CameraSlotFolder3"]; }
      set { this["CameraSlotFolder3"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder4 {
      get { return (string)this["CameraSlotFolder4"]; }
      set { this["CameraSlotFolder4"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder5 {
      get { return (string)this["CameraSlotFolder5"]; }
      set { this["CameraSlotFolder5"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder6 {
      get { return (string)this["CameraSlotFolder6"]; }
      set { this["CameraSlotFolder6"] = value; }
    }
    [DefaultSettingValue( "" )]
    public string CameraSlotFolder7 {
      get { return (string)this["CameraSlotFolder7"]; }
      set { this["CameraSlotFolder7"] = value; }
    }


    #endregion

  }
}
