using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar
{
  static class Program
  {
    internal static string Instance = ""; // default


    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main( )
    {
      // get a command line arg (if there is)
      var cl = Environment.GetCommandLineArgs();
      if ( cl.Length>1 ) {
        Instance = cl[1];
      }
      // init an AppSettings instance based on the command line argument
      AppSettings.InitInstance( Instance );
      var _ = AppSettings.Instance.Profile_1;

      // Standard Init follows here
      Application.EnableVisualStyles( );
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new frmMain( ) );
    }
  }
}
