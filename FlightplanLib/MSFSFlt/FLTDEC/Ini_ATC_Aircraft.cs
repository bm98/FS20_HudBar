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
    /// RequestedFlightPlan field
    /// </summary>
    [IniFileKey( "RequestedFlightPlan" )]
    public string RequestedFlightPlan_S { get; internal set; } = ""; // True, False
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
    public double CruisingAltitude_ft { get; internal set; } = 0.0;

    // non Ini file

    /// <summary>
    /// True if ActiveFlightPlan is true
    /// </summary>
    [IniFileIgnore]
    public bool HasActiveFlightPlan => Formatter.ToBool( ActiveFlightPlan_S );
    /// <summary>
    /// True if RequestedFlightPlan is true
    /// </summary>
    [IniFileIgnore]
    public bool HasRequestedFlightPlan => Formatter.ToBool( RequestedFlightPlan_S );

  }
}
