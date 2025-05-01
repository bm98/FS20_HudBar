using System;
using System.Windows.Forms;

namespace FS20_ChecklistBox
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
      Application.SetCompatibleTextRenderingDefault( true );
      Application.Run( new FChecklistBox.frmChecklistBox( Instance, true ) ); // standalone

      //
      NLog.LogManager.Shutdown( );

    }
  }
}
