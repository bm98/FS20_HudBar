using FSimFacilityIF;

using FSimFlightPlans;

namespace bm98_Map
{
  /// <summary>
  /// Extend the Waypoint for Shelf use
  /// </summary>
  internal static class WypExtensions
  {

    /// <summary>
    /// True when this item should be shown in the Map
    ///  inverse of HideInMap
    /// </summary>
    public static bool ShowInMap( this Waypoint _w ) => !_w.HideInMap(  );

    /// <summary>
    /// True when this item should be _hidden_ in the Map
    ///  hidden are: 
    ///    APT
    ///    MAPR waypoints except when a RWY
    ///    APR 2nd to last
    /// </summary>
    public static bool HideInMap( this Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.RWY) return false; // allow Runways

      if (_w.WaypointType == WaypointTyp.APT) return true; // kill Airports
      // if (_w.WaypointUsage == UsageTyp.HOLD) return true; // kill Holds
      if (_w.WaypointUsage == UsageTyp.MAPR) return true; // kill MAPR but not Runway / checked above already
      if (_w.ApproachSequence > 1) return true; // only show the first Approach Wyp

      return false;
    }

    /// <summary>
    /// True when this item should be used for the Route
    ///  allowed are:
    ///    all except APT, HOLD and MAPR except when a RWY
    /// </summary>
    public static bool UseForRoute( this Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.RWY) return true; // allow Runways
      if (_w.WaypointType == WaypointTyp.APT) return false; // kill Airports
      if (_w.WaypointUsage == UsageTyp.HOLD) return false; // kill Holds
      if (_w.WaypointUsage == UsageTyp.MAPR) return false; // kill MAPR but not Runway / checked above already

      return true;
    }

  }
}
