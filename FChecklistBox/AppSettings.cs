using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SettingsLib;

namespace FChecklistBox
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
    public Point ChecklistBoxLocation {
      get { return (Point)this["ChecklistBoxLocation"]; }
      set { this["ChecklistBoxLocation"] = value; }
    }

    // Control bound settings
    [DefaultSettingValue( "10, 10" )]
    public Point ConfigLocation {
      get { return (Point)this["ConfigLocation"]; }
      set { this["ConfigLocation"] = value; }
    }

    // Control bound settings
    [DefaultSettingValue( "0, 0" )]
    public Size ConfigSize {
      get { return (Size)this["ConfigSize"]; }
      set { this["ConfigSize"] = value; }
    }


    #endregion

  }
}
