using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbgLib;

using FSimFacilityIF;

namespace FlightplanLib.Flightplan
{
  /// <summary>
  /// Create a table for the generic FlightPlan
  /// </summary>

  public class FlightPlanTable
  {
    #region STATIC DEBUG
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion


    private FPTableGen _formatter;

    #region Formatting Utilities

    // return a more readable type for the Table
    private string PrettyType( WaypointTyp typ )
    {
      switch (typ) {
        case WaypointTyp.WYP: return "WAYPOINT";
        case WaypointTyp.APT: return "AIRPORT";
        case WaypointTyp.RWY: return "RUNWAY";
        case WaypointTyp.USR: return "USER";
        case WaypointTyp.COR: return "COORDINATE";
        case WaypointTyp.OTH: return "OTHER";
        default: return typ.ToString( );
      }
    }

    /// <summary>
    /// Descriptive Content for FPTable
    /// </summary>
    public string ProcDescription( Waypoint _w )
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

    private string AsHTML( FlightPlan plan )
    {
      _formatter = new FPTableGen( );

      string sid_text = "";
      string star_text = "";
      string apr_text = "";
      double remaining_dist = plan.Distance_Total_nm;
      foreach (var item in plan.Waypoints) {
        // see if we can fill in some texts (assuming runways are in Origin and Dest)
        // those items remain empty until a trigger is detected, then stay at their content
        sid_text = item.IsSID
          ? $"Depart using SID {item.SID_Ident} from Runway {plan.Origin.Runway_Ident} (elev {plan.Origin.LatLonAlt_ft.Altitude:##,##0} ft)"
          : sid_text;
        star_text = item.IsSTAR
          ? $"Arrive using STAR {item.STAR_Ident}"
          : star_text;
        apr_text = item.IsAPR
          ? $"Approach through {item.ApproachName} and land on Runway {plan.Destination.Runway_Ident} (elev {plan.Destination.LatLonAlt_ft.Altitude:##,##0} ft)"
          : apr_text;

        // calc some
        // re Alt usage - the formatting is lo=hi and >0 show lo; else lo>0 show  /lo; hi>0 show hi/
        // for Procedures show limits i.e. use given lo and hi
        // for everything else use lo=hi= AltitudeRounded (target Altitude)
        int altTgt = item.IsProc ? 0 : item.AltitudeRounded_ft; // use Limits

        string freq = string.IsNullOrWhiteSpace( item.Frequency ) ? "" : $" ({item.Frequency})";
        var rowData = new FPTableGen.RowData( ) {
          Ident = item.Ident,
          AltTarget_ft = altTgt,
          AltLo_ft = item.AltitudeLimitLo_ft,
          AltHi_ft = item.AltitudeLimitHi_ft,
          Type = $"{PrettyType( item.WaypointType )}{freq}",
          Proc = ProcDescription( item ),
          InbTRK_degm = item.InboundMagTrk,
          Dist_nm = (double.IsNaN( item.InboundDistance_nm ) || (item.InboundDistance_nm < 0.001)) ? float.NaN : (float)item.InboundDistance_nm, // omit NaN and 0 (very small..)
        };
        remaining_dist -= item.InboundDistance_nm;
        _formatter.AddRow( rowData, remaining_dist );
      }
      // fill optional APR with something meaningful
      if (string.IsNullOrEmpty( apr_text )) {
        apr_text = $"Approach and land on Runway {plan.Destination.Runway_Ident} (elev {plan.Destination.LatLonAlt_ft.Altitude:##,##0} ft)";
      }
      var cmt = new List<string> {
        $"{plan.FlightPlanType} Plan, routing is {plan.RouteType}, initial cruise altitude: {plan.CruisingAlt_ft:##,##0 ft}"
                + (string.IsNullOrEmpty(plan.StepProfile)?"":$" - steps: {plan.StepProfile}"),
        sid_text,
        star_text,
        apr_text
      };

      string title = $"{plan.FlightPlanType} Flightplan from ({plan.Origin.Icao_Ident}) {plan.Origin.Name} to ({plan.Destination.Icao_Ident}) {plan.Destination.Name}";
      var html = _formatter.CommitDocument( title, cmt, plan.FlightPlanFile );
      return html;
    }

    #endregion

    /// <summary>
    /// Load a FP doc into the designated folder
    /// </summary>
    /// <param name="plan">A FlightPlan</param>
    /// <param name="targetFolder">The target folder</param>
    /// <returns>True if successfull</returns>
    public bool SaveDocument( FlightPlan plan, string targetFolder )
    {
      // sanity
      if (plan == null) return false;
      if (!plan.IsValid) return false;

      var html = AsHTML( plan );

      // protect from inadvertend crashes of the unknown Library...
      //Image image;
      try {
        bm98_Html.HtmlRenderer.Instance.Html2PNG( html, Path.Combine( targetFolder, bm98_hbFolders.Folders.FTablePNG_FileName ) );

        // image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage( html );
      }
      catch (Exception ex) {
        LOG.LogException( "FlightPlanTable.SaveDocument", ex, "Converting to HTML failed" );
        return false;
      }
      /*
      // dest may be locked when viewing
      try {
        image.Save( Path.Combine( targetFolder, c_FTablePNG_DocName ), ImageFormat.Png );

        image.Dispose( );
      }
      catch (Exception ex) {
        LOG.LogException( "FlightPlanTable.SaveDocument", ex, "Saving to file failed" );
        return false;
      }
      */
      return true;

    }

  }
}
