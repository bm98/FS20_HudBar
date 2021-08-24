using System;
using System.IO;

namespace FS20_HudBar.Bar.FltLib
{
  /// <summary>
  /// MSFS2020 FLT File decoder
  ///  For now it decodes the ATC_Aircraft.0 Flightplan which is the best I can get
  ///  This FP is updated with ATC guidance and Inflight requests to ATC
  ///  - It must not match the GPS one (we've seen rather diverging ATC and GPS plans at the same time..)
  /// </summary>
  public class FS20FltFile
  {

    /// <summary>
    /// True if the file was found valid
    /// </summary>
    public bool Valid { get; private set; } = false;

    /// <summary>
    /// The Flightplan 
    /// </summary>
    public FlightPlan FlightPlan { get; private set; } = new FlightPlan( );



    private string m_content;

    /// <summary>
    /// cTor: create with a filename to load
    /// </summary>
    /// <param name="filename">Filepath and name (FLT file)</param>
    public FS20FltFile( string filename )
    {
      if ( !File.Exists( filename ) ) return; // ERROR

      // shall not fail...
      try {
        m_content = File.ReadAllText( filename );
      }
      catch (Exception e) {
        Console.WriteLine( "FS20FltFile: Cannot Read " + filename );
        Console.WriteLine( e.Message );
        return;
      }
      FlightPlan = FlightPlan.Decode( m_content );
      Valid = true; // we did not encounter errors... but the FP can still be an empty one
    }


  }
}
