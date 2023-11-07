using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dNetBm98.IniLib;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// Departure Arrival Section of FLT
  /// </summary>
  public class Ini_DepArr
  {
    /// <summary>
    /// ICAO field
    /// </summary>
    [IniFileKey( "ICAO" )]
    public string ICAO { get; internal set; } = "";
    /// <summary>
    /// RunwayNumber field
    /// </summary>
    [IniFileKey( "RunwayNumber" )]
    public string RunwayNumber { get; internal set; } = "";
    /// <summary>
    /// RunwayDesignator field
    /// </summary>
    [IniFileKey( "RunwayDesignator" )]
    public string RunwayDesignator { get; internal set; } = ""; // NONE, LEFT, RIGHT, CENTER, ??

  }
}
