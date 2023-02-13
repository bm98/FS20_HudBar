using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// Our Implementation of the tracked aircraft
  /// with some calculations added
  /// </summary>
  internal class TrackedAircraft : TrackedAircraftCls
  {
    // calculations and shortcuts

    private AvgModule_Rolling _vsAvg = new AvgModule_Rolling( 5, 5 );

    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowMTRK => !float.IsNaN( Trk_degm );
    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowAlt => !float.IsNaN( Altitude_ft );
    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowRA => !float.IsNaN( RadioAlt_ft );
    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowIas => !float.IsNaN( Ias_kt );
    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowGs => !float.IsNaN( Gs_kt );
    /// <summary>
    /// True to Show
    /// </summary>
    public bool ShowVs => !float.IsNaN( Vs_fpm );

    /// <summary>
    /// A Target Altitude for Distance Calc, 
    /// Usually the Airport elevation
    /// </summary>
    public double TargetAltitude_ft { get; set; } = 0;

    /// <summary>
    /// Windspeed string (Unit setting dependent)
    /// </summary>
    public string WindSpeedS { get; set; } = "";

    /// <summary>
    /// True to Show Target Range instead of distance arcs
    ///  - must be less than 1500 ft to target elevation
    ///  - must be less than -120 fpm on the average
    ///  - must be less than 8nm to go
    /// </summary>
    public bool ShowTargetRange {
      get {
        //Console.WriteLine( $"Vs Avg: {_vsAvg.Avg} Alt {Altitude_ft:##0} TAlt {TargetAltitude_ft:##0} Dist {DistanceToTargetAlt( )}" );
        if (_vsAvg.Avg >= -120f) return false;
        if ((Altitude_ft - TargetAltitude_ft) >= 1500f) return false;
        if (DistanceToTargetAlt( ) >= 8) return false;
        return true;
      }
    }

    /// <summary>
    /// Calculates the distance where the target Alt is reached
    /// </summary>
    /// <param name="targetAlt_ft">Target Altitude in ft</param>
    /// <returns>Distance in nautical miles - -1 if not reachable or cannot calculate</returns>
    public double DistanceToTargetAlt( double targetAlt_ft )
    {
      if (!(ShowAlt || ShowGs || ShowVs)) return -1f;

      // dist = kts * 60 * (( tAlt - currAlt ) / vs)
      // if dist<0 the target will not be reached anymore (having the wrong sign on the VS)
      var dist = Gs_kt / 60.0 * ((targetAlt_ft - Altitude_ft) / _vsAvg.Avg);

      return (dist > 0) ? (float)dist : -1f;
    }

    /// <summary>
    /// Calculates the distance where the target Alt is reached
    /// Target is taken from the Property TargetAltitude_ft
    /// </summary>
    /// <returns>Distance in nautical miles - -1 if not reachable or cannot calculate</returns>
    public double DistanceToTargetAlt( ) => DistanceToTargetAlt( TargetAltitude_ft );


    /// <summary>
    /// Update from the Source
    /// </summary>
    /// <param name="aircraft">An aircraft supporting ITrackedAircraft</param>
    public void Update( ITrackedAircraft aircraft )
    {
      Position = new LatLon( aircraft.Position.Lat, aircraft.Position.Lon,
                           float.IsNaN( aircraft.Altitude_ft ) ? double.NaN : aircraft.Altitude_ft );// maintain with all 3 data items
      Heading_degm = aircraft.Heading_degm;
      TrueHeading_deg = aircraft.TrueHeading_deg;
      Trk_degm = aircraft.Trk_degm;
      TrueTrk_deg = aircraft.TrueTrk_deg;
      Altitude_ft = aircraft.Altitude_ft;
      RadioAlt_ft = aircraft.RadioAlt_ft;
      Ias_kt = aircraft.Ias_kt;
      Gs_kt = aircraft.Gs_kt;
      Vs_fpm = aircraft.Vs_fpm;
      OnGround = aircraft.OnGround;

      WindDirection_deg = (aircraft.WindSpeed_kt < 1) ? float.NaN : aircraft.WindDirection_deg; // avoid speeds <1
      WindSpeed_kt = aircraft.WindSpeed_kt;
      WindSpeedS = $"{aircraft.WindSpeed_kt:##0}kt"; // default

      ShowAircraftRange = aircraft.ShowAircraftRange;
      ShowAircraftWind = aircraft.ShowAircraftWind;
      ShowAircraftTrack = aircraft.ShowAircraftTrack;
      ClearAircraftTrack = aircraft.ClearAircraftTrack;

      _vsAvg.Sample( Vs_fpm );
    }
  }
}
