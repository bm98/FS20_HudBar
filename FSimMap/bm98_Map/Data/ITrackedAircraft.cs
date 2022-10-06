using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// An Interface an TrackedAircraft must expose
  /// </summary>
  public interface ITrackedAircraft
  {
    /// <summary>
    /// The LatLon position  (not using the Alt from this element for now)
    /// </summary>
    LatLon Position { get; }
    /// <summary>
    /// True Heading in degree
    /// </summary>
    float TrueHeading { get; }
    /// <summary>
    /// Altitude in feet above mean sea level
    ///  set to float.NaN to hide from display
    /// (used also for range calc, if switched off...)
    /// </summary>
    float Altitude_ft { get; }
    /// <summary>
    /// Radio/Radar Altitude in ft above the grouns
    ///  set to float.NaN to hide from display
    /// </summary>
    float RadioAlt_ft { get; }
    /// <summary>
    /// Indicated airspeed in knots
    ///  set to float.NaN to hide from display
    /// </summary>
    float Ias_kt { get; }
    /// <summary>
    /// Ground speed in knots 
    ///  set to float.NaN to hide from display
    /// (used also for range calc, if switched off...)
    /// </summary>
    float Gs_kt { get; }
    /// <summary>
    /// Ground Track in mag deg
    /// </summary>
    float Trk_deg { get; }
    /// <summary>
    /// Ground Track in true deg
    /// (used also for range calc, if switched off...)
    /// </summary>
    float TrueTrk_deg { get; }
    /// <summary>
    /// Vertical rate in feet/minute
    ///  set to float.NaN to hide from display
    /// (used also for range calc, if switched off...)
    /// </summary>
    float Vs_fpm { get; }
    /// <summary>
    /// Flag to indicate the Acft is on Ground
    /// </summary>
    bool OnGround { get; }

    /// <summary>
    /// Flag to show or hide the range indicator of the icon
    /// </summary>
    bool ShowAircraftRange { get; }
    /// <summary>
    /// Flag to show or hide the stored track
    /// </summary>
    bool ShowAircraftTrack { get; }
    /// <summary>
    /// Flag to trigger deletion of stored track
    /// </summary>
    bool ClearAircraftTrack { get; }
  }
}
