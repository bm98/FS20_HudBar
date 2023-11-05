using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib;
using FlightplanLib.LNM.LNMDEC;
using FSimFacilityIF;

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
    /// Descriptive Name of this Item
    /// </summary>
    public static string ProcDescription( this Waypoint _w )
    {
      return _w.IsSTAR ? $"STAR {_w.STAR_Ident}"
            : _w.IsSID ? $"SID {_w.SID_Ident}"
            : _w.IsAirway ? $"via Awy {_w.Airway_Ident}"
            : _w.IsAPR ?
              (_w.WaypointType == WaypointTyp.RWY) ? ""
              : (_w.WaypointUsage == UsageTyp.MAPR) ? $"MAPR"
                  : (_w.WaypointUsage == UsageTyp.HOLD) ? $"MAPR Hold"
                  : $"APR {_w.ApproachName}"
            : _w.IsDecorated ? $"{_w.IdentDecoration}"
            : "";
    }

    /// <summary>
    /// An Alt Limit String, derived from values
    /// </summary>
    public static string AltLimitS( this Waypoint _w )
    {
      string a;
      if (_w.AltitudeHi_ft == _w.AltitudeLo_ft) {
        // =15'000
        a = float.IsNaN( _w.AltitudeLo_ft ) || (_w.AltitudeLo_ft < 1) ? "" : $"{_w.AltitudeLo_ft:##,##0}";
      }
      else {
        // /8'000
        a = float.IsNaN( _w.AltitudeLo_ft ) || (_w.AltitudeLo_ft < 1) ? "" : $"{_w.AltitudeLo_ft:##,##0}";
        // 15'000/  OR 15'000/8'000
        a = (float.IsNaN( _w.AltitudeHi_ft ) || (_w.AltitudeHi_ft < 1) ? "" : $"{_w.AltitudeHi_ft:##,##0}") + "/" + a;
      }
      return a;
    }


  }
}
