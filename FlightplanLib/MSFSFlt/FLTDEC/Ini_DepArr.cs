using FlightplanLib.MS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// Departure Arrival Section of FLT
  /// </summary>
  public class Ini_DepArr
  {
    [IniFileKey( "ICAO" )]
    public string ICAO { get; internal set; } = "";

    [IniFileKey( "RunwayNumber" )]
    public string RunwayNumber { get; internal set; } = "";

    [IniFileKey( "RunwayDesignator" )]
    public string RunwayDesignator { get; internal set; } = ""; // NONE, LEFT, RIGHT, CENTER, ??

  }
}
