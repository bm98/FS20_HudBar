using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Generic Route Provider
  /// incl. Route Interpreter from Route Descriptions like SimBrief, LNM and other sources
  /// </summary>
  public class GenRoute
  {
    /// <summary>
    /// True when valid
    /// </summary>
    public bool IsValid { get; internal set; } = false;

    /// <summary>
    /// Departure Airport
    /// </summary>
    public string DepAirport_ICAO { get; set; }
    /// <summary>
    /// Departure Airport Runway ID (e.g. RW02C) if available
    /// </summary>
    public string DepRunwayID { get; set; }
    /// <summary>
    /// Departure Airport Est Time 
    /// </summary>
    public string DepEstTime { get; set; }

    /// <summary>
    /// Initial Cruise Speed and Altitude (IsValid = false if not set)
    /// </summary>
    public SpeedAltRemark CruiseSpeedAlt { get; set; }

    /// <summary>
    /// SID Ident if used
    /// </summary>
    public string SID { get; set; }

    /// <summary>
    /// Waypoints of the route
    /// </summary>
    public List<GenRouteWaypoint> Waypoints { get; set; } = new List<GenRouteWaypoint>( );

    /// <summary>
    /// STAR Ident if used
    /// </summary>
    public string STAR { get; set; }

    /// <summary>
    /// Arrival Airport
    /// </summary>
    public string ArrAirport_ICAO { get; set; }
    /// <summary>
    /// Arrival Airport Runway ID (e.g. RW02C) if available
    /// </summary>
    public string ArrRunwayID { get; set; }

    /// <summary>
    /// Arrival Airport Est Time 
    /// </summary>
    public string ArrEstTime { get; set; }

  }
}
