using System;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// TCAS indicator
  /// </summary>
  public enum TcasFlag
  {
    /// <summary>
    /// Within limits on the same ref alt
    /// AND within 10nm range
    /// </summary>
    ProximityLevel = 0,
    /// <summary>
    /// Within limits on the same ref alt
    /// AND outside 10nm range
    /// </summary>
    Level,
    /// <summary>
    /// Above current ref alt
    /// </summary>    
    Above,
    /// <summary>
    /// Below current ref alt
    /// </summary>    
    Below,
  }

  /// <summary>
  /// An Interface an TrackedAircraft must expose
  /// </summary>
  public interface ITrackedAircraft
  {
    /// <summary>
    /// The ID of the tracked Aircraft
    /// </summary>
    string AircraftID { get; }
    /// <summary>
    /// True if it is a Helicopter
    /// </summary>
    bool IsHeli { get; }
    /// <summary>
    /// The LatLon position  (not using the Alt from this element for now)
    /// </summary>
    LatLon Position { get; }
    /// <summary>
    /// True Heading in degree
    /// </summary>
    float TrueHeading_deg { get; }
    /// <summary>
    /// Mag Heading in degree
    /// </summary>
    float Heading_degm { get; }
    /// <summary>
    /// Altitude in feet above mean sea level
    ///  set to float.NaN to hide from display
    /// (used also for range calc, if switched off...)
    /// </summary>
    float AltitudeMsl_ft { get; }
    /// <summary>
    /// Indicated Altitude
    /// </summary>
    float AltitudeIndicated_ft { get; }
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
    float Trk_degm { get; }
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
    /// FlightPath angle degree
    /// </summary>
    float Fpa_deg { get; }
    /// <summary>
    /// Wind Speed in kt
    /// </summary>
    float WindSpeed_kt { get; }
    /// <summary>
    /// Wind Direction in deg (true, chart direction)
    /// </summary>
    float WindDirection_deg { get; }

    /// <summary>
    /// Flag if this aircraft is above, below or level with a ref altitude
    /// ref altitude is usually the users aircraft altitude
    /// </summary>
    TcasFlag TCAS { get; }

    /// <summary>
    /// Distance to T/D if available, else set negative or NaN
    /// </summary>
    float DistanceToTOD_nm { get; }

    /// <summary>
    /// Flag to indicate the Acft is on Ground
    /// </summary>
    bool OnGround { get; }

    /// <summary>
    /// Flag to show or hide the range indicator of the icon
    /// </summary>
    bool ShowAircraftRange { get; }
    /// <summary>
    /// Flag to show or hide the range rings of the icon
    /// </summary>
    bool ShowAircraftRings { get; }
    /// <summary>
    /// Flag to show or hide the wind indicator of the icon
    /// </summary>
    bool ShowAircraftWind { get; }
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
