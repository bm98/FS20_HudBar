using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using bm98_hbFolders;
using DbgLib;

namespace FS20_HudBar
{
  static class Program
  {
    #region STATIC
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion

    internal static string Instance = ""; // default
    internal static bool AppSettingsV1Available = false; // set true if V1 is initialized

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main( )
    {
      // get a command line arg (if there is)
      var cl = Environment.GetCommandLineArgs( );
      if (cl.Length > 1) {
        Instance = cl[1];
      }

      LOG.Log( $"Program Start with Instance ({Instance})" );
#if DEBUG
      // TODO REMOVE FOR FINAL TEST AND RELEASE
      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "HudBarAppSettings-NEXT.json" );
#else
      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "HudBarAppSettings.json" );
#endif
      // init the V2 AppSettings instance based on the command line argument
      AppSettingsV2.InitInstance( Folders.SettingsFile, Instance );
      var v2Used = AppSettingsV2.Instance.V2InUse;

      // check if never used and Upgrade if needed
      if (v2Used) {
        LOG.Log( $"AppSettings V2 is used for Instance ({Instance})" );
      }
      else {
        // Init V1 settings with Instance
        AppSettings.InitInstance( Instance );
        AppSettingsV1Available = true;

        var testLoc = AppSettings.Instance.FormLocation;
        if (testLoc.X == 10 && testLoc.Y == 10) {
          LOG.Log( $"AppSettings V1 is not used for Instance ({Instance})" );
        }
        // in any case Upgrade - else the V2Used flag will not be set
        LOG.Log( $"Upgrading/Init AppSettings to V2 for Instance ({Instance})" );
        AppSettingsUpgrade.UpgradeSettings( );
      }

      // Standard Init follows here
      Application.EnableVisualStyles( );
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new frmMain( ) );

      //
      NLog.LogManager.Shutdown( );

    }
  }
}
