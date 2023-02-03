using FlightplanLib.MS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// Section ATC_Aircraft of FLT File
  /// </summary>

  public class Ini_ATC_Aircraft
  {
    /// <summary>
    /// ActiveFlightPlan field
    /// </summary>
    [IniFileKey( "ActiveFlightPlan" )]
    public string ActiveFlightPlan_S { get; internal set; } = ""; // True, False
    /// <summary>
    /// Waypoint. field for enumeration, find Waypoint.N (0.. max)
    /// </summary>
    [IniFileKey( "Waypoint." )] // Waypoint.0 .. .N
    public Dictionary<string, string> Waypoints { get; internal set; } = new Dictionary<string, string>( );
    /// <summary>
    /// NumberofWaypoints field
    /// </summary>
    [IniFileKey( "NumberofWaypoints" )]
    public int NumberofWaypoints { get; internal set; } = 0;
    /// <summary>
    /// CruisingAltitude field (ft)
    /// </summary>
    [IniFileKey( "CruisingAltitude" )]
    public double CruisingAltitude { get; internal set; } = 0.0;

    // non Ini file

    /// <summary>
    /// Get the decoded Waypoint for a Key
    /// </summary>
    /// <param name="wypName">A waypoint Key</param>
    /// <returns>The Waypoint obj</returns>
    public Ini_Waypoint Waypoint( string wypName )
    {
      var ret = new Ini_Waypoint( );
      if (Waypoints.ContainsKey( wypName )) {
        ret = Ini_Waypoint.GetWaypoint( Waypoints[wypName] );
      }
      return ret ?? new Ini_Waypoint( );
    }


  }
}
