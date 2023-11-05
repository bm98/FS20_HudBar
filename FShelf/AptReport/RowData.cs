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
    // string replacement mark
    public const string c_MARK = "$$$+++$$$";
  }

  #region Runway

  // Runways:
  //  Runway Row
  //    [Approach Rows]
  //  Runway Row
  //    [Approach Rows]
  //  ...


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
      // background style for TR does not work with this renderer, using TH with background-color
      string t_th = $"<th class=\"frame\">{c_MARK}</th>\n";
      string t_th_s = $"<th class=\"frame wSmall\">{c_MARK}</th>\n"; // small col for the last one

      cols += t_th.Replace( c_MARK, "RWY" );
      cols += t_th.Replace( c_MARK, "HDG" );
      //    cols += t_td.Replace( c_MARK, "ILS / FIX" );
      //    cols += t_td.Replace( c_MARK, "Freq" );
      //    cols += t_td.Replace( c_MARK, "GS (Range)" );
      cols += t_th.Replace( c_MARK, "Dim [ft]" );
      cols += t_th.Replace( c_MARK, "Dim [m]" );
      cols += t_th.Replace( c_MARK, "Surface" );
      cols += t_th_s.Replace( c_MARK, " " ); // small column

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }

    // Runway properties
    public string Rwy;      // RW01C ..
    public float Hdg_deg;   // number
    public string IlsID;    //
    //public float IlsFreq_mhz;
    //public float IlsGsSlope_deg;
    //public float IlsGsRange_nm;
    public string Dim_ft;   // LLLxWWW
    public string Dim_m;    // LLLxWWW
    public string Surface;  // text

    /// <summary>
    /// Format a Runway entry as Table row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */
      string cols = "";
      string t_td = $"<td class=\"frame fBold\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, using TD
      cols += t_td.Replace( c_MARK, $"{Rwy}" );
      cols += t_td.Replace( c_MARK, $"{Hdg_deg:000}°" );
      cols += t_td.Replace( c_MARK, $"{Dim_ft}" );
      cols += t_td.Replace( c_MARK, $"{Dim_m}" );
      cols += t_td.Replace( c_MARK, $"{Surface}" );
      cols += t_td.Replace( c_MARK, $" " );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }
  }

  /// <summary>
  /// A Runway Approach Row
  ///  Set unavailable items = 0 or String.Empty
  /// </summary>
  internal struct AprRowData
  {
    // Approach properties
    public string AprType;

    public string Nav;      // XYZ 
    public string Freq;     // formatted string: 123.12 / 1121.2
    public float IlsGsSlope_deg;
    public float IlsGsRange_nm;

    public string WaypointID; // SDJKS
    public string Fix;      // FAF, IAF, IF

    public float Alt_Hi;
    public float Alt_Lo;

    public float Alt_Missed;
    public string Missed_WypID;
    public string Missed_Hold;

    public string SourceID; // ng / ms

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */
      string cols = "";
      string t_td = $"<td class=\"frame bgGrey\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, "" ); // first col is empty
      cols += t_td.Replace( c_MARK, $"{AprType}" );
      string t = (IlsGsSlope_deg > 0) ? $" {IlsGsSlope_deg:0.0}°)" : ") ";
      if (!string.IsNullOrEmpty( Nav )) {
        cols += t_td.Replace( c_MARK, $"{Nav} ({Freq}{t}" );
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
      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }
  }

  #endregion

  #region Procedures

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

    // Runway properties
    public string RwyS;         // ↑ RW01C 
    public string RwyProcType;  // SID / STAR

    /// <summary>
    /// Format a Runway entry as Table row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string RwyRow_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */
      string cols = "";
      string t_td = $"<td class=\"frame bgIce fBold\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, using TD
      cols += t_td.Replace( c_MARK, $"{RwyS}" );
      cols += t_td.Replace( c_MARK, $"{RwyProcType}" );
      cols += t_td.Replace( c_MARK, $"Fix" );
      cols += t_td.Replace( c_MARK, $"Transitions" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }


    public string ProcName;  // name of the proc
    public string ProcFix;    // Fix of the proc
    public List<string> ProcTransitions; // list of transitions

    /// <summary>
    /// Format a Runway Proc entry as Table row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string ProcRow_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */

      string txString = "";
      if (ProcTransitions != null && ProcTransitions.Count > 0) {
        foreach (string t in ProcTransitions) {
          txString += t + ", ";
        }
      }
      txString = txString.TrimEnd( new char[] { ' ', ',' } );

      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // odd / even
      string t_td = $"<td class=\"noframe {bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, using TD
      cols += t_td.Replace( c_MARK, $" " ); // empty col
      cols += t_td.Replace( c_MARK, $"{ProcName}" );
      cols += t_td.Replace( c_MARK, $"{ProcFix}" );
      cols += t_td.Replace( c_MARK, $"{txString}" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }

  }

  #endregion

  #region Communication

  // Communication:
  //   Com Row
  //   Com Row
  //   ...


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
      string t_td = $"<th class=\"frame\">{c_MARK}</th>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, "Type" );
      cols += t_td.Replace( c_MARK, "Freq" );
      cols += t_td.Replace( c_MARK, "Name" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }

    // Runway properties
    public string CommType;
    public float CommFreq_mhz;
    public string LocationName;

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */
      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // even / odd
      string t_td = $"<td class=\"frame {bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, $"{CommType}" );
      cols += t_td.Replace( c_MARK, $"{CommFreq_mhz:000.000} MHz" );
      cols += t_td.Replace( c_MARK, $"{LocationName}" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }
  }

  #endregion

  #region Navaids

  // Navaids:
  //  Navaid Row
  //  Navaid Row
  //  ...


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
      string t_td = $"<th class=\"frame\">{c_MARK}</th>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, "ID" );
      cols += t_td.Replace( c_MARK, "Type" );
      cols += t_td.Replace( c_MARK, "Freq" );
      cols += t_td.Replace( c_MARK, "Distance (RSI)" );
      cols += t_td.Replace( c_MARK, "Name" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }

    // Runway properties
    public string NavaidType; // NDB, VOR, VOR DME
    public string ICAO;
    public string FreqS; // set either of the two and the other 0 !!
    public string RangeS;
    public string NavaidName;

    /// <summary>
    /// Format as HTML row
    /// </summary>
    /// <returns>A formatted row</returns>
    public string Row_html( int lineNum )
    {
      /*
      <tr>
        <td>Col 1</td>
        <td>Col 2</td>
        <td>Col 3</td>
      </tr>
       */
      string cols = "";
      string bg = (lineNum % 2 == 0) ? "bgAlt" : "bgWhite"; // even / odd
      string t_td = $"<td class=\"frame {bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD
      cols += t_td.Replace( c_MARK, $"{ICAO}" );
      cols += t_td.Replace( c_MARK, $"{NavaidType}" );
      cols += t_td.Replace( c_MARK, $"{FreqS}" );
      cols += t_td.Replace( c_MARK, $"{RangeS}" );
      cols += t_td.Replace( c_MARK, $"{NavaidName}" );

      string row = $"<tr>\n{cols}</tr>\n";
      return row;
    }
  }

  #endregion

}
