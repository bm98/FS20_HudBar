using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.MercatorTiles;
using FSimFacilityIF;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Item to capture all aspects of a route waypoint while decoding
  /// </summary>
  public class RouteWaypointCapture
  {
    /// <summary>
    /// The Type of this Capture
    /// </summary>
    public WaypointTyp WaypointType { get; set; } = WaypointTyp.Unknown;
    /// <summary>
    /// The Ident of the Enroute
    /// </summary>
    public string AirwayIdent { get; set; } = "";
    /// <summary>
    /// Enroute to continue on or null if there is no Enroute to follow
    /// </summary>
    public IAirway Airway { get; set; } = null;

    /// <summary>
    /// Waypoint Ident if empty a Coordinate must exist
    /// NAMED WAYPONT or VOR or NDB
    /// if Enroute is given it is the EXIT point of this airway
    /// </summary>
    public string WaypointIdent { get; set; } = "";

    /// <summary>
    /// Coordinate Entry if LatLon.Empty a Waypoint must exits
    ///   Degrees, minutes and [seconds] (11 or 15 characters): '481200N0112842E'
    ///   DDMM[SS]{NS}DDDMM[SS]{EW}
    /// </summary>
    public LatLon Coord { get; set; } = LatLon.Empty;
    /// <summary>
    /// The Quad of the Waypoint
    /// </summary>
    public Quad Quad { get; set; } = Quad.Empty;

    /// <summary>
    /// A Speed and Alt change remark (SpeedAltRemark.IsValid = false if not set)
    /// </summary>
    public SpeedAltRemark SpeedAlt { get; set; } = SpeedAltRemark.Empty;

  }
}
