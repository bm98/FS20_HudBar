using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// Template Class to implement a tracked aircraft
  /// </summary>
  public class TrackedAircraftCls : ITrackedAircraft
  {
    /// <summary>
    /// The LatLon position  (not using the Alt from this element for now)
    /// </summary>
    public LatLon Position { get; set; } = new LatLon( 0, 0, 0 );

    /// <summary>
    /// True Heading in degree
    /// </summary>
    public float TrueHeading { get; set; } = 0;

    /// <summary>
    /// Altitude in feet above mean sea level
    ///  set to float.NaN to hide from display
    /// </summary>
    public float Altitude_ft { get; set; } = float.NaN;

    /// <summary>
    /// Radio/Radar Altitude in ft above the grouns
    ///  set to float.NaN to hide from display
    /// </summary>
    public float RadioAlt_ft { get; set; } = float.NaN;

    /// <summary>
    /// Indicated airspeed in knots
    ///  set to float.NaN to hide from display
    /// </summary>
    public float Ias_kt { get; set; } = float.NaN;

    /// <summary>
    /// True airspeed in knots
    ///  set to float.NaN to hide from display
    /// </summary>
    public float Tas_kt { get; set; } = float.NaN;

    /// <summary>
    /// Ground speed in knots 
    ///  set to float.NaN to hide from display
    /// (used also for range calc, if switched off...)
    /// </summary>
    public float Gs_kt { get; set; } = float.NaN;

    /// <summary>
    /// Ground Track in mag deg
    /// </summary>
    public float Trk_deg { get; set; } = float.NaN;
    /// <summary>
    /// Ground Track in true deg
    /// (used also for range calc, if switched off...)
    /// </summary>
    public float TrueTrk_deg { get; set; } = float.NaN;

    /// <summary>
    /// Vertical rate in feet/minute
    ///  set to float.NaN to hide from display
    /// </summary>
    public float Vs_fpm { get; set; } = float.NaN;

    /// <summary>
    /// Flight Path Angle °
    ///  set to float.NaN to hide from display
    /// </summary>
    public float Fpa_deg { get; set; } = float.NaN;

    /// <summary>
    /// Flag to indicate the Acft is on Ground
    /// </summary>
    public bool OnGround { get; set; } = false;

    /// <summary>
    /// Flag to show or hide the tracked icon
    /// </summary>
    public bool ShowAircraft { get; set; } = false;

    /// <summary>
    /// Flag to show or hide the range indicator of the icon
    /// </summary>
    public bool ShowAircraftRange { get; set; } = false;
    /// <summary>
    /// Flag to show or hide the stored track
    /// </summary>
    public bool ShowAircraftTrack { get; set; } = false;
    /// <summary>
    /// Flag to trigger deletion of stored track
    /// </summary>
    public bool ClearAircraftTrack { get; set; } = false;

  }
}
