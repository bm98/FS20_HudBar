using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;

using FlightplanLib.Flightplan;

namespace FShelf.FPlans
{
  /// <summary>
  /// Extend the Waypoint for Shelf use
  /// </summary>
  internal static class WypExtensions
  {

    /// <summary>
    /// True when this item should be shown in the Map
    /// </summary>
    public static bool ShowInMap( this Waypoint _w ) => !_w.HideInMap( );


    /// <summary>
    /// True when this item should be hidden in the Map
    /// </summary>
    public static bool HideInMap( this Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.APT) return true; // kill Airports
      if (_w.WaypointUsage == UsageTyp.HOLD) return true; // kill Holds
      if ((_w.WaypointUsage == UsageTyp.MAPR) && (_w.WaypointType != WaypointTyp.RWY)) return true; // kill MAPR but not Runway
      if (_w.ApproachSequence > 1) return true; // only show the first Approach Wyp

      return false;
    }

    /// <summary>
    /// True when this item should be used for the Route
    /// </summary>
    public static bool UseForRoute( this Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.APT) return false; // kill Airports
      if (_w.WaypointUsage == UsageTyp.HOLD) return false; // kill Holds
      if ((_w.WaypointUsage == UsageTyp.MAPR) && (_w.WaypointType != WaypointTyp.RWY)) return false; // kill MAPR but not Runway

      return true;
    }

    /*
    /// <summary>
    /// Returns the Wyp Target altitude
    /// </summary>
    /// <param name="_w">A Waypoint</param>
    /// <returns>An Altitude or NaN</returns>
    public static float AltTarget_ft( this Waypoint _w )
    {
      // input alt can be NaN or 0 if not defined or not set - we cannot use both of them
      // make it NaN if it was <=0
      float aLo =
        (float.IsNaN( _w.AltitudeLimitLo_ft ) || (_w.AltitudeLimitLo_ft <= 0)) ? float.NaN : _w.AltitudeLimitLo_ft;
      float aHi =
        (float.IsNaN( _w.AltitudeLimitHi_ft ) || (_w.AltitudeLimitHi_ft <= 0)) ? float.NaN : _w.AltitudeLimitHi_ft;
      float altTarget_ft =
        (double.IsNaN( _w.LatLonAlt_ft.Altitude ) || (_w.LatLonAlt_ft.Altitude <= 0))
        ? float.NaN
        : (float)_w.LatLonAlt_ft.Altitude; // Wyp alt is default when available

      if (float.IsNaN( aLo ) && float.IsNaN( aHi )) {
        // both undef, remains default
      }
      else if (!(float.IsNaN( aLo ) || float.IsNaN( aHi ))) {
        // both defined
        altTarget_ft = (aLo + aHi) / 2f; // between
      }
      else if (!float.IsNaN( aLo )) {
        altTarget_ft = aLo;
      }
      else if (!float.IsNaN( aHi )) {
        altTarget_ft = aHi;
      }

      return altTarget_ft;
    }
    */
  }
}
