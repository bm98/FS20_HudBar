using System;
using System.Windows.Forms;

namespace FS20_CamControl
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
      var cl = Environment.GetCommandLineArgs( );
      if (cl.Length > 1) {
        Instance = cl[1];
      }

      Application.EnableVisualStyles( );
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new FCamControl.frmCameraV2( Instance, true ) );
      //
      NLog.LogManager.Shutdown( );

    }
  }
}
