using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Data
{
  /// <summary>
  /// Our Implementation of an AI aircraft
  /// with some calculations added
  /// </summary>
  internal class TrackedAircraftAI : TrackedAircraft
  {
    /// <summary>
    /// For AI always false
    /// </summary>
    public override bool ShowTargetRange => false;

    /// <summary>
    /// For AI always disable
    /// </summary>
    /// <param name="targetAlt_ft">Target Altitude in ft</param>
    /// <returns>Always -1 </returns>
    public override double DistanceToTargetAlt( double targetAlt_ft = -1f ) => -1f;

    /// <summary>
    /// Update from the Source
    /// </summary>
    /// <param name="aircraft">An aircraft supporting ITrackedAircraft</param>
    public override void Update( ITrackedAircraft aircraft )
    {
      AircraftID = aircraft.AircraftID;
      Position = new LatLon( aircraft.Position.Lat, aircraft.Position.Lon,
                           float.IsNaN( aircraft.AltitudeMsl_ft ) ? double.NaN : aircraft.AltitudeMsl_ft );// maintain with all 3 data items
      Heading_degm = aircraft.Heading_degm;
      TrueHeading_deg = aircraft.TrueHeading_deg;
      Trk_degm = aircraft.Trk_degm;
      TrueTrk_deg = aircraft.TrueTrk_deg;
      AltitudeMsl_ft = aircraft.AltitudeMsl_ft;
      AltitudeIndicated_ft = aircraft.AltitudeIndicated_ft;
      RadioAlt_ft = aircraft.RadioAlt_ft;
      Ias_kt = aircraft.Ias_kt;
      Gs_kt = aircraft.Gs_kt;
      Vs_fpm = aircraft.Vs_fpm;
      TCAS = aircraft.TCAS;
      OnGround = aircraft.OnGround;

      // not for AI acft
      DistanceToTOD_nm = float.NaN;
      WindDirection_deg = 0;
      WindSpeed_kt = 0;
      WindSpeedS = $"n.a."; // default


      ShowAircraftRings = false;
      ShowAircraftRange = false;
      ShowAircraftWind = false;
      ShowAircraftTrack = false;
      ClearAircraftTrack = false;

      _vsAvg.Sample( Vs_fpm );
    }
  }
}
