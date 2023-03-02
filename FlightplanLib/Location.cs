using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;

namespace FlightplanLib
{
  /// <summary>
  /// Generic Start/End Location, usually an Airport
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// </summary>
  public class Location
  {
    /// <summary>
    /// The ICAO indent of this location
    /// </summary>
    public IcaoRec Icao_Ident { get; internal set; } = new IcaoRec( );

    /// <summary>
    /// Optional:
    /// The IATA Ident of this location
    /// </summary>
    public string Iata_Ident { get; internal set; } = "";

    /// <summary>
    /// Optional:
    /// The Common Name of this location
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// The Lat, Lon, Elevation [ft] of this location
    /// </summary>
    public LatLon LatLonAlt_ft { get; internal set; } = LatLon.Empty;

    /// <summary>
    /// The Magnetic Variation at this location
    /// </summary>
    public double MagVar_deg => LatLonAlt_ft.MagVarLookup_deg( );

    /// <summary>
    /// Optional:
    /// The Runway number as string
    /// </summary>
    internal string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...
    /// <summary>
    /// Optional:
    /// The Runway designation as string as provided by the plan
    /// </summary>
    internal string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    /// <summary>
    /// Returns a Runway ident like 22 or 12R etc.
    /// </summary>
    public string Runway_Ident => FlightPlan.ToRunwayID( RunwayNumber_S, RunwayDesignation );

    // Tools

    /// <summary>
    /// True if the Location is valid (has a valid LatLon)
    /// </summary>
    public bool IsValid => !LatLonAlt_ft.IsEmpty;

  }
}
