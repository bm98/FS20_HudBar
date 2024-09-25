using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FlightplanLib.Flightplan.RowData;

namespace FlightplanLib.Flightplan
{
  ///<remarks> All RowData Structs</remarks>

  internal static class RowData
  {
    /// <summary>
    /// String replacement mark
    /// </summary>
    public const string c_MARK = "$$$+!+$$$"; // unlikely to be used somewhere for content
  }

  /// <summary>
  /// Formatter for one FP Table Row
  ///   Set properties and use it to add a row
  /// </summary>
  internal struct FPlanRowData
  {
    /// <summary>
    /// Format Column Headers as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public static string HeaderRow_html( )
    {
      string cols = "";
      // th template
      string t_th = $"<th>{c_MARK}</th>\n";

      cols += t_th.Replace( c_MARK, "Ident" );
      cols += t_th.Replace( c_MARK, "Alt ft" );
      cols += t_th.Replace( c_MARK, "Type" );
      cols += t_th.Replace( c_MARK, "Proc/Info" );
      cols += t_th.Replace( c_MARK, "Trk °M" );
      cols += t_th.Replace( c_MARK, "Dist nm" );
      cols += t_th.Replace( c_MARK, "Remaining nm" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

    // Table Columns
    /// <summary>
    /// Col 1 - WYP Ident
    /// </summary>
    public string Ident { get; set; }
    /// <summary>
    /// Col 2 - Target Alt (use if >0)
    /// </summary>
    public int AltTarget_ft { get; set; }
    /// <summary>
    /// Col 2 - Limit Lo
    /// </summary>
    public int AltLo_ft { get; set; }
    /// <summary>
    /// Col 2 - Limit Hi
    /// </summary>
    public int AltHi_ft { get; set; }
    /// <summary>
    /// Col 3 - WYP Type
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Col 4 - Proc/Info
    /// </summary>
    public string Proc { get; set; }
    /// <summary>
    /// Col 5 - Inbound Track [degM]
    /// </summary>
    public int InbTRK_degm { get; set; }
    /// <summary>
    /// Col 6 - Segment distance [nm]
    /// </summary>
    public double Dist_nm { get; set; }
    /// <summary>
    /// Col 7 - Remaining distance [nm]
    /// </summary>
    public double DistRemaining_nm { get; set; }

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    internal string Row_html( int lineNum )
    {
      /*
      <tr>
        <td>Indent</td>
        <td>Alt</td>
        <td>Type</td>
        <td>Proc/Info</td>
        <td>TrkDeg (inbound)</td>
        <td>Dist</td>
        <td>Remaining</td>
      </tr>
       */
      string cols = "";
      string tdClass = (lineNum % 2 == 0) ? "even" : "odd"; // even / odd rows
                                                            // do some fancy row background for APT and RWY
      tdClass = Type.StartsWith( "AIRPORT" ) ? "airport" : tdClass; // alice blue
      tdClass = Type.StartsWith( "RUNWAY" ) ? "runway" : tdClass;  // bisque
      tdClass = Proc.StartsWith( FSimFacilityIF.UsageTyp.MAPR.ToString( ) ) ? "mapr" : tdClass;  // lavender blush
                                                                                                 // td template for this row
      string t_td = $"<td class=\"{tdClass}\">{c_MARK}</td>\n";

      cols += t_td.Replace( c_MARK, Ident );
      string a;
      if (AltTarget_ft > 0) {
        a = $"{AltTarget_ft:##,##0}";
      }
      else if (AltHi_ft == AltLo_ft) {
        a = (AltLo_ft < 1) ? "" : $"={AltLo_ft:##,##0}";
      }
      else {
        a = (AltLo_ft < 1) ? "" : $"{AltLo_ft:##,##0}";
        a = ((AltHi_ft < 1) ? "" : $"{AltHi_ft:##,##0}") + "/" + a;
      }
      cols += t_td.Replace( c_MARK, a ); // don't print NaNs and Zeros
      cols += t_td.Replace( c_MARK, $"{Type}" );
      cols += t_td.Replace( c_MARK, Proc );
      cols += t_td.Replace( c_MARK, $"{InbTRK_degm:000}" );
      cols += t_td.Replace( c_MARK, double.IsNaN( Dist_nm ) ? "" : $"{Dist_nm:##0.0}" );
      cols += t_td.Replace( c_MARK, (DistRemaining_nm >= 0) ? $"{DistRemaining_nm:##0.0}" : "." );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }
  }
}
