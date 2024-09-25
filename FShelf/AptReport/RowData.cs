using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static FShelf.AptReport.RowData;

namespace FShelf.AptReport
{
  ///<remarks> All RowData Structs</remarks>

  internal static class RowData
  {
    /// <summary>
    /// String replacement mark
    /// </summary>
    public const string c_MARK = "$$$+!+$$$"; // unlikely to be used somewhere for content
  }


  #region Runway and Approaches

  // Runways:
  //  Runway Row
  //    [Approach Rows]
  //  Runway Row
  //    [Approach Rows]
  //  ...

  /* 
   * Runway Row: 6 Columns
    <tr>
      <td>Runway ID</td>
      <td>Runway Heading degM</td>
      <td>Dimension string [ft]</td>
      <td>Dimension string [m]</td>
      <td>Surface type</td>
      <td>_empty_</td>
    </tr>
   */

  /*
   * Approach Row: 6 Columns
  <tr>
    <td>_empty_</td>
    <td>Approach Type</td>
    <td>ILS ICAO + GS deg</td>
    <td>WYP ICAO</td>
    <td>Missed WYP</td>
    <td>Navaid Source (MS or NG)</td>
  </tr>
 */

  /// <summary>
  /// A Runway Row
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct RwyRowData
  {
    /// <summary>
    /// Format the Table Header as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public static string HeaderRow_html( )
    {
      string cols = "";
      string t_th_1 = $"<th class=\"frame wRwyCol_1\">{c_MARK}</th>\n"; // first Col fixed width for all below
      string t_th = $"<th class=\"frame\">{c_MARK}</th>\n";
      string t_th_s = $"<th class=\"frame wSmall\">{c_MARK}</th>\n"; // small col for the last one

      cols += t_th_1.Replace( c_MARK, "RWY" ); 
      cols += t_th.Replace( c_MARK, "HDG" );
      //    cols += t_td.Replace( c_MARK, "ILS / FIX" );
      //    cols += t_td.Replace( c_MARK, "Freq" );
      //    cols += t_td.Replace( c_MARK, "GS (Range)" );
      cols += t_th.Replace( c_MARK, "Dim [ft]" );
      cols += t_th.Replace( c_MARK, "Dim [m]" );
      cols += t_th.Replace( c_MARK, "Surface" );
      cols += t_th_s.Replace( c_MARK, " " ); // small column

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

    // Runway properties
    /// <summary>
    /// Runway ID (RW01C)
    /// </summary>
    public string Rwy { get; set; }
    /// <summary>
    /// Runway Heading deg (True)
    /// </summary>
    public int Hdg_deg { get; set; }
    /// <summary>
    /// Runway ILS ID or empty
    /// </summary>
    public string IlsID { get; set; }
    //public float IlsFreq_mhz;
    //public float IlsGsSlope_deg;
    //public float IlsGsRange_nm;

    /// <summary>
    /// Dimension string [ft] (LLLxWWW)
    /// </summary>
    public string Dim_ft { get; set; }
    /// <summary>
    /// Dimension string [m] (LLLxWWW)
    /// </summary>
    public string Dim_m { get; set; }
    /// <summary>
    /// Surface Type
    /// </summary>
    public string Surface { get; set; }

    /// <summary>
    /// Format a Runway entry as Table row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      string cols = "";
      string t_td = $"<td class=\"frame fBold\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, using TD
      cols += t_td.Replace( c_MARK, $"{Rwy}" );
      cols += t_td.Replace( c_MARK, $"{Hdg_deg:000}°" );
      cols += t_td.Replace( c_MARK, $"{Dim_ft}" );
      cols += t_td.Replace( c_MARK, $"{Dim_m}" );
      cols += t_td.Replace( c_MARK, $"{Surface}" );
      cols += t_td.Replace( c_MARK, $" " );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }
  }


  #region Approach

  /// <summary>
  /// A Runway Approach Row
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct AprRowData
  {
    // Has no independent Header as it is a sub entry of a runway

    /// <summary>
    /// Approach Type
    /// </summary>
    public string AprType { get; set; }
    /// <summary>
    /// Nav ID (ILS, VOR..)
    /// </summary>
    public string Nav { get; set; }
    /// <summary>
    /// Frequ String - formatted string: 123.12 / 1121.2
    /// </summary>
    public string FreqS { get; set; }
    /// <summary>
    /// ILS GS Slope deg or 0
    /// </summary>
    public float IlsGsSlope_deg { get; set; }
    /// <summary>
    /// ILS GS Range nm or 0
    /// </summary>
    public float IlsGsRange_nm { get; set; }
    /// <summary>
    /// Approach Fix WYP ICAO
    /// </summary>
    public string WaypointID { get; set; }
    /// <summary>
    /// Approach Fix Type (FAF, IAF, IF)
    /// </summary>
    public string Fix { get; set; }
    /// <summary>
    /// Alt Limit Hi or 0
    /// </summary>
    public int Alt_Hi { get; set; }
    /// <summary>
    /// Alt Limit Lo or 0
    /// </summary>
    public int Alt_Lo { get; set; }
    /// <summary>
    /// Missed Alt
    /// </summary>
    public int Alt_Missed { get; set; }
    /// <summary>
    /// Missed WYP ICAO
    /// </summary>
    public string Missed_WypID { get; set; }
    /// <summary>
    /// Missed HOLD
    /// </summary>
    public string Missed_Hold { get; set; }
    /// <summary>
    /// Proc Source (ng / ms)
    /// </summary>
    public string SourceID { get; set; }

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      string cols = "";
      string t_td = $"<td class=\"frame bgGrey\">{c_MARK}</td>\n";
      cols += t_td.Replace( c_MARK, "" ); // first col is empty
      cols += t_td.Replace( c_MARK, $"{AprType}" );
      string t = (IlsGsSlope_deg > 0) ? $" {IlsGsSlope_deg:0.0}°)" : ") ";
      if (!string.IsNullOrEmpty( Nav )) {
        cols += t_td.Replace( c_MARK, $"{Nav} ({FreqS}{t}" );
      }
      else {
        cols += t_td.Replace( c_MARK, $" " );
      }
      // alt decoder, may have hi or lo or both - or none
      string altS = "";
      if ((Alt_Hi > 0) && (Alt_Lo > 0)) {
        if (Alt_Hi == Alt_Lo) {
          altS = $"{Alt_Hi:##,##0}";
        }
        else {
          altS = $"{Alt_Lo:##,##0} / {Alt_Hi:##,##0}";
        }
      }
      else if (Alt_Hi > 0) {
        altS = $"{Alt_Hi:##,##0}";
      }
      else if (Alt_Lo > 0) {
        altS = $"{Alt_Lo:##,##0}";
      }
      cols += t_td.Replace( c_MARK, $"{Fix} {WaypointID} @ {altS} ft" );
      cols += t_td.Replace( c_MARK, $"map {Missed_WypID} @ {Alt_Missed:##,##0} ft {Missed_Hold}" );
      cols += t_td.Replace( c_MARK, $"{SourceID}" );
      // merge into tr
      string row = $"<tr>\n{cols}</tr>";
      return row;
    }
  }

  #endregion // Approach

  #endregion // Runway

  #region Runways and their Procedures

  /*
   * Runway Row: 4 Columns
  <tr>
    <td>RwyS</td>
    <td>RwyProcType</td>
    <td>Fix</td>
    <td>Transitions</td>
  </tr>
 */

  /*
   * Runway Procedure: 4 Columns
    <tr>
      <td>_empty_></td>
      <td>ProcName</td>
      <td>ProcFix</td>
      <td>Transitions</td>
    </tr>
   */

  // Procedures:
  //                 Name         ; Fix    ; Transitions
  //  Runway Row   | ↑ RW16       ;        ;
  //  SID Row      | VEBI4S (RNAV); ↑ VEBIT; ↑ RABIT, ↑ FOLIG, ↑ RURAL
  //  SID Row      | GERS2S (RNAV); ↑ GERSA,
  //  Runway Row   | RW10  , SE   , 
  //  SID Row      | VEBI3E (RNAV)  VEBIT,
  //  SID Row
  //  ...
  //  STAR Row     | KELI3G (RNAV)  KELIP GIPOL RW00
  //  STAR Row     | NEGR2A (RNAV)  NEGRA AMIKI RW00


  //  STAR Row     | VEBI4S (RNAV)  VEBIT
  //    Runway Row |   RW16         VEBIT
  //    Runway Row |   RW14         GALOP
  //  STAR Row
  //    ...
  //
  // 

  /// <summary>
  /// A Runway Row for Procedures
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct RwyProcRowData
  {

    /// <summary>
    /// Format the Table Header as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public static string HeaderRow_html( string procType )
    {
      /* NO HEADER
      string cols = "";
      // background style for TR does not work with this renderer, using TH with background-color
      string t_th = $"<th class=\"frame\">{c_MARK}</th>\n";

      cols += t_th.Replace( c_MARK, $"RWY" );
      cols += t_th.Replace( c_MARK, $"{procType}" );
      cols += t_th.Replace( c_MARK, "Fix" );
      cols += t_th.Replace( c_MARK, "Transitions" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
      */
      return "";
    }

    /// <summary>
    /// Runway String e.g. "↑ RW01C"
    /// </summary>
    public string RwyS { get; set; }
    /// <summary>
    /// Runway Procedure Type (SID / STAR)
    /// </summary>
    public string RwyProcType { get; set; }

    /// <summary>
    /// Format a Runway entry as Table row
    ///  Acts as Main row in Runways and Procedures
    /// </summary>
    /// <returns>A formatted row</returns>
    public string RwyRow_html( int lineNum )
    {
      string cols = "";
      string t_td_1 = $"<td class=\"frame bgIce fBold wRwyCol_1\">{c_MARK}</td>\n"; // first Column- fixed width
      string t_td = $"<td class=\"frame bgIce fBold\">{c_MARK}</td>\n";

      cols += t_td_1.Replace( c_MARK, $"{RwyS}" );
      cols += t_td.Replace( c_MARK, $"{RwyProcType}" );
      cols += t_td.Replace( c_MARK, $"Fix" );
      cols += t_td.Replace( c_MARK, $"Transitions" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

    /// <summary>
    /// Procedure Name
    /// </summary>
    public string ProcName { get; set; }  // name of the proc
    /// <summary>
    /// Procedure Fix
    /// </summary>
    public string ProcFix { get; set; }    // Fix of the proc
    /// <summary>
    /// List of Transitions
    /// </summary>
    public List<string> ProcTransitions { get; set; } // list of transitions

    /// <summary>
    /// Format a Runway Proc entry as Table row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string ProcRow_html( int lineNum )
    {
      // create transition entry if there are
      string txString = "";
      if ((ProcTransitions != null) && (ProcTransitions.Count > 0)) {
        foreach (string t in ProcTransitions) {
          txString += t + ", ";
        }
      }
      txString = txString.TrimEnd( new char[] { ' ', ',' } );

      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // odd / even
      string t_td = $"<td class=\"frame {bg}\">{c_MARK}</td>\n";
      cols += t_td.Replace( c_MARK, $" " ); // empty col
      cols += t_td.Replace( c_MARK, $"{ProcName}" );
      cols += t_td.Replace( c_MARK, $"{ProcFix}" );
      cols += t_td.Replace( c_MARK, $"{txString}" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

  }

  #endregion

  #region Communication

  /*
    <tr>
      <td>Type</td>
      <td>Freq</td>
      <td>Location</td>
    </tr>
   */

  /// <summary>
  /// A Communication Row
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct CommRowData
  {
    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public static string HeaderRow_html( )
    {
      string cols = "";
      string t_th = $"<th class=\"frame\">{c_MARK}</th>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_th.Replace( c_MARK, "Type" );
      cols += t_th.Replace( c_MARK, "Freq" );
      cols += t_th.Replace( c_MARK, "Name" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

    /// <summary>
    /// Comm Type
    /// </summary>
    public string CommType { get; set; }
    /// <summary>
    /// Comm Frequ [MHz]
    /// </summary>
    public float CommFreq_mhz { get; set; }
    /// <summary>
    /// Comm Location Name
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // even / odd
      string t_td = $"<td class=\"frame {bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, $"{CommType}" );
      cols += t_td.Replace( c_MARK, $"{CommFreq_mhz:000.000} MHz" );
      cols += t_td.Replace( c_MARK, $"{LocationName}" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }
  }

  #endregion

  #region Navaids

  /*
    <tr>
      <td>ICAO</td>
      <td>NavaidType</td>
      <td>Freq</td>
      <td>Range/RSI</td>
      <td>NavaidName</td>
    </tr>
  */

  /// <summary>
  /// A Navaid Row
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct NavaidRowData
  {
    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public static string HeaderRow_html( )
    {
      string cols = "";
      string t_th = $"<th class=\"frame\">{c_MARK}</th>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_th.Replace( c_MARK, "ID" );
      cols += t_th.Replace( c_MARK, "Type" );
      cols += t_th.Replace( c_MARK, "Freq" );
      cols += t_th.Replace( c_MARK, "Distance (RSI)" );
      cols += t_th.Replace( c_MARK, "Name" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }

    /// <summary>
    /// Type of Navaid (NDB, VOR, VOR DME..)
    /// </summary>
    public string NavaidType { get; set; }
    /// <summary>
    /// ICAO Name
    /// </summary>
    public string ICAO { get; set; }
    /// <summary>
    /// Frequ String
    /// </summary>
    public string FreqS { get; set; }
    /// <summary>
    /// Range String
    /// </summary>
    public string RangeS { get; set; }
    /// <summary>
    /// Common Name
    /// </summary>
    public string NavaidName { get; set; }

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // even / odd
      string t_td = $"<td class=\"frame {bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD

      cols += t_td.Replace( c_MARK, $"{ICAO}" );
      cols += t_td.Replace( c_MARK, $"{NavaidType}" );
      cols += t_td.Replace( c_MARK, $"{FreqS}" );
      cols += t_td.Replace( c_MARK, $"{RangeS}" );
      cols += t_td.Replace( c_MARK, $"{NavaidName}" );

      string row = $"<tr>\n{cols}</tr>";
      return row;
    }
  }

  #endregion

}
