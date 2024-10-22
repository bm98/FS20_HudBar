using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimFlightPlans;

namespace FS20_HudBar
{
  /// <summary>
  /// Clears a Control Collection and disposes the items when asked for
  /// </summary>
  public static class ExtensionMethods
  {

    /// <summary>
    /// Clear a Control Collection with optional Dispose
    /// </summary>
    /// <param name="controls">The ControlCollection</param>
    /// <param name="dispose">Set true to call dispose for each item</param>
    public static void Clear( this Control.ControlCollection controls, bool dispose )
    {
      for (int ix = controls.Count - 1; ix >= 0; --ix) {
        var tmpObj = controls[ix];
        controls.RemoveAt( ix );
        if (dispose && !tmpObj.IsDisposed) tmpObj.Dispose( );
      }
    }

    // *** FlightPlan Extensions

    /// <summary>
    /// Returns a CRLF separated string containing the full stored FlighPlan
    /// or an empty string if there is no FP
    /// </summary>
    public static string Pretty( this FSimFlightPlans.FlightPlan _fp )
    {
      if (!_fp.IsValid) return "No active Flightplan";

      string ret = _fp.PrettyLeader( );
      ret += "Waypoints:\n";
      for (uint i = 0; i < _fp.Waypoints.Count( ); i++) {
        ret += $"{_fp.GetWaypoint( i ).Pretty( )}\n";
      }
      return ret;
    }

    // The Leader part or empty if no FP is available
    private static string PrettyLeader( this FSimFlightPlans.FlightPlan _fp )
    {
      if (!_fp.IsValid) return "";

      string ret = $"Flightplan: {_fp.Origin.Icao_Ident}-{_fp.Destination.Icao_Ident}\n";
      ret += $" Next Wyp : {_fp.Tracker.NextRoutePointID}\n";
      return ret;
    }

    /// <summary>
    /// Returns the remaining FlightPlan as CRLF separated line of text
    ///  empty if no plan is available
    /// </summary>
    /// <returns>A CRLF string list</returns>
    public static string RemainingPlan( this FlightPlan _fp )
    {
      if (!_fp.IsValid) return "No active Flightplan";

      uint next = _fp.Tracker.NextRoutePoint.IsValid ? _fp.Tracker.NextRoutePoint.Index : uint.MaxValue;
      if (next >= _fp.Waypoints.Count( )) return "No active Flightplan";

      var ret = _fp.PrettyLeader( );
      ret += "Remaining Waypoints:\n";
      for (uint i = next; i < _fp.Waypoints.Count( ); i++) {
        ret += $"{_fp.GetWaypoint( i ).Pretty( )}\n";
      }

      return ret;
    }


    // *** FlightPlan.Waypoint Extensions

    /// <summary>
    /// Returns a line of Wyp Information
    /// </summary>
    public static string Pretty( this Waypoint _w )
    {
      if (!_w.IsValid) return $"Waypoint not found";

      string typ = (
        string.IsNullOrEmpty( _w.SID_Ident ) ?
        string.IsNullOrEmpty( _w.STAR_Ident ) ?
        string.IsNullOrEmpty( _w.ApproachProcRef ) ?
      string.IsNullOrEmpty( _w.Airway_Ident ) ?
      _w.WaypointType.ToString( ) : _w.Airway_Ident : _w.ApproachProcRef : _w.STAR_Ident : _w.SID_Ident).PadRight( 8 );
      string limits = _w.AltitudeLimitS;
      limits += (_w.SpeedLimit_kt > 0) ? $" Max. {_w.SpeedLimit_kt:##0} kt" : "";
      string tAlt = (_w.TargetAltitude_ft > 0) ? $"@ {_w.TargetAltitude_ft,6:##,##0} ft" : "";

      return $"{_w.Ident7.PadRight( 7 ),-8}\t({typ,-15}) {_w.InboundDistance_nm,7:#,##0.0} - {_w.InboundDistanceRemaining_nm,7:#,##0.0} nm {tAlt}  {limits}";
    }

    /// <summary>
    /// Returns a block of detailed Wyp Information
    /// </summary>
    public static string PrettyDetailed( this Waypoint _w )
    {
      if (!_w.IsValid) return $"Waypoint not found";

      string typ = (
        string.IsNullOrEmpty( _w.SID_Ident ) ?
        string.IsNullOrEmpty( _w.STAR_Ident ) ?
        string.IsNullOrEmpty( _w.ApproachProcRef ) ?
      string.IsNullOrEmpty( _w.Airway_Ident ) ?
      _w.WaypointType.ToString( ) : _w.Airway_Ident : _w.ApproachProcRef : _w.STAR_Ident : _w.SID_Ident).PadRight( 8 );

      string limits = _w.AltitudeLimitS;
      limits += (_w.SpeedLimit_kt > 0) ? $" Max. {_w.SpeedLimit_kt:##0} kt" : "";
      string tAlt = (_w.TargetAltitude_ft > 0) ? $"@ {_w.TargetAltitude_ft,6:##,##0} ft" : "";

      return $"Waypoint: {_w.Ident7} ({typ}) {tAlt}\n"
        + $"{limits}\n"
        + $"Leg Dist: {_w.InboundDistance_nm:##,##0.0} nm\n"
        + $"Remaining: {_w.InboundDistanceRemaining_nm:##,##0.0} nm";
    }

    // *** Control Extensions

    public static void SetLeftMargin( this Control _c, int value )
    {
      var pad = _c.Margin;
      pad.Left = value;
      _c.Margin = pad;
    }
    public static void SetRightMargin( this Control _c, int value )
    {
      var pad = _c.Margin;
      pad.Right = value;
      _c.Margin = pad;
    }
    public static void SetTopBottomMargin( this Control _c, int value )
    {
      var pad = _c.Margin;
      pad.Top = value;
      pad.Bottom = value;
      _c.Margin = pad;
    }

    public static void SetLeftPadding( this Control _c, int value )
    {
      var pad = _c.Padding;
      pad.Left = value;
      _c.Padding = pad;
    }
    public static void SetRightPadding( this Control _c, int value )
    {
      var pad = _c.Padding;
      pad.Right = value;
      _c.Padding = pad;
    }
    public static void SetTopBottomPadding( this Control _c, int value )
    {
      var pad = _c.Padding;
      pad.Top = value;
      pad.Bottom = value;
      _c.Padding = pad;
    }


  }
}
