using FlightplanLib.MSFSPln.PLNDEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using FlightplanLib.MS;
using FlightplanLib.MSFSPln;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// An MSFS FLT Flight Plan
  /// </summary>
  public class FLT
  {
    // Main Keys

    // ... NONE

    /// <summary>
    /// Sect [Main] 
    /// </summary>
    [IniFileSection( "Main" )]
    public Ini_Main Main { get; internal set; } = new Ini_Main( );

    // Sect [Options] - not used

    // Secct [Sim.0] - not used

    /// <summary>
    /// Sect [Departure] 
    /// </summary>
    [IniFileSection( "Departure" )]
    public Ini_DepArr Departure { get; internal set; } = new Ini_DepArr( );

    /// <summary>
    /// Sect [Arrival]
    /// </summary>
    [IniFileSection( "Arrival" )]
    public Ini_DepArr Arrival { get; internal set; } = new Ini_DepArr( );

    /// <summary>
    /// Sect [ATC_Aircraft.0] active Flight gets changes while in flight and saved
    /// </summary>
    [IniFileSection( "ATC_Aircraft.0" )]
    public Ini_ATC_Aircraft ATC_Aircraft { get; internal set; } = null;

    /// <summary>
    /// Sect [ATC_ActiveFlightPlan.0] seems to be an initial or current FP, never contains ATC intermediate waypoints
    /// </summary>
    [IniFileSection( "ATC_ActiveFlightPlan.0" )]
    public Ini_ATC_ActiveFlightPlan ATC_ActiveFlightPlan { get; internal set; } = null;

    /// <summary>
    /// Sect [ATC_RequestedFlightPlan.0] seems to be an initial or current FP, never contains ATC intermediate waypoints
    /// to be used when if [ATC_Aircraft.0] RequestedFlightPlan=True
    /// </summary>
    [IniFileSection( "ATC_RequestedFlightPlan.0" )]
    public Ini_ATC_ActiveFlightPlan ATC_RequestedFlightPlan { get; internal set; } = null;


    // Sect [OriginalFlightPlan]   (Mission Files)

    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    [IniFileIgnore]
    public bool IsValid => ATC_Aircraft != null;

    /// <summary>
    /// Returns the ATC_FlightPlan property in use
    /// </summary>
    [IniFileIgnore]
    public Ini_ATC_ActiveFlightPlan Used_ATC_FlightPlan =>
        this.ATC_Aircraft.HasActiveFlightPlan
      ? this.ATC_ActiveFlightPlan
      : this.ATC_Aircraft.HasRequestedFlightPlan ? this.ATC_RequestedFlightPlan : new Ini_ATC_ActiveFlightPlan( );

    /// <summary>
    /// Returns the Waypoints property in use
    /// </summary>
    [IniFileIgnore]
    public Dictionary<string, string> Used_Waypoints =>
        this.ATC_Aircraft.HasActiveFlightPlan
      ? this.ATC_Aircraft.Waypoints // use the one in the Aircraft section as they may contain approach pts later
      : this.ATC_Aircraft.HasRequestedFlightPlan ? this.ATC_RequestedFlightPlan.Waypoints : new Dictionary<string, string>( );

    /// <summary>
    /// Get the decoded Waypoint for a Key
    /// </summary>
    /// <param name="wypName">A waypoint Key</param>
    /// <returns>The Waypoint obj</returns>
    public Ini_Waypoint Waypoint( string wypName )
    {
      var ret = new Ini_Waypoint( );
      if (Used_Waypoints.ContainsKey( wypName )) {
        ret = Ini_Waypoint.GetWaypoint( Used_Waypoints[wypName] );
      }
      return ret ?? new Ini_Waypoint( );
    }

  }
}
