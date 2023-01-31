using CoordLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FShelf.FPlans
{
  /// <summary>
  /// Create a table for the generic FlightPlan
  /// </summary>

  internal class FlightPlanTable
  {
    private FPTableGen _formatter;

    private string AsHTML( FlightplanLib.FlightPlan plan )
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
          ? $"Approach through {item.Approach_Ident} and land on Runway {plan.Destination.Runway_Ident} (elev {plan.Destination.LatLonAlt_ft.Altitude:##,##0} ft)"
          : apr_text;

        // calc some
        string proc =
          item.IsSTAR ? $"STAR {item.STAR_Ident}"
          : item.IsSID ? $"SID {item.SID_Ident}"
          : item.IsAirway ? $"Awy {item.Airway_Ident}"
          : item.IsAPR ? $"APR {item.Approach_Ident}"
          : item.IsDecorated ? $"{item.Wyp_Deco}"
          : "";

        string freq = string.IsNullOrWhiteSpace( item.Frequency ) ? "" : $" ({item.Frequency})";

        // fill the row
        var rowData = new FPTableGen.RowData( ) {
          Ident = item.Wyp_Ident,
          Alt_ft = item.AltitudeRounded_ft,
          Type = $"{item.WaypointType}{freq}",
          Proc = proc,
          InbTRK_degm = item.InboundMagTrk,
          Dist_nm = (float)item.Distance_nm,
        };
        remaining_dist -= rowData.Dist_nm;
        _formatter.AddRow( rowData, remaining_dist );
        //
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
      var html = _formatter.CommitDocument( title, cmt );
      return html;
    }

    /// <summary>
    /// Load a FP doc into the designated folder
    /// </summary>
    /// <param name="plan">A FlightPlan</param>
    /// <param name="targetFolder">The target folder</param>
    /// <returns>True if successfull</returns>
    public bool SaveDocument( FlightplanLib.FlightPlan plan, string targetFolder )
    {
      // sanity
      if (plan == null) return false;
      if (!plan.IsValid) return false;

      var html = AsHTML( plan );

      // protect from inadvertend crashes of the unknown Library...
      Image image;
      try {
        image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage( html );
      }
      catch (Exception ex) {
        Console.WriteLine( $"PLN LoadDocument: Converting to HTML failed:\n{ex}" );
        return false;
      }
      // dest may be locked when viewing
      try {
        image.Save( Path.Combine( targetFolder, "@.FlightTable.png" ), ImageFormat.Png );
      }
      catch (Exception ex) {
        Console.WriteLine( $"PLN LoadDocument: Saving to file failed:\n{ex}" );
        return false;
      }
      return true;

    }

  }
}
