using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CoordLib;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// A Waypoint of a GenRoute
  /// </summary>
  public class GenRouteWaypoint
  {
    /// <summary>
    /// Enroute to continue on 
    /// Empty if there is no Enroute to follow
    /// </summary>
    public string AirwayIdent { get; set; }

    /// <summary>
    /// Waypoint Ident (ICAO style ident) OR a Coordinate entry
    /// NAMED WAYPONT or VOR or NDB
    /// Coordinate Entry:
    ///   Degrees, minutes and [seconds] (11 or 15 characters): '481200N0112842E'
    ///   DDMM[SS]{NS}DDDMM[SS]{EW}
    /// </summary>
    public string Ident { get; set; }
    /// <summary>
    /// The Ident as LatLon - will be LatLon.Empty if not decodable
    /// </summary>
    public LatLon IdentAsLatLon { get; }

    /// <summary>
    /// A Speed and Alt change remark (IsValid = false if not set)
    /// </summary>
    public SpeedAltRemark SpeedAlt { get; set; }


  }
}
