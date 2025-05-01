using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using static dNetBm98.Units;
using static bm98_Html.HtmlChars;
using DbgLib;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FShelf.AptReport
{
  /// <summary>
  /// Create a table for the generic Airport Report
  /// </summary>

  internal class AptReportTable
  {
    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #endregion

    private AptTableGen _formatter;

    // there are a lot of the same fixes used, so try to minimize DB access and use a lookup table
    private Dictionary<string, string> _fixDecoration = new Dictionary<string, string>( );

    // Translate from a Direction String to a HTML Arrow Char
    private string ToDirectionArrow( string dirS )
    {
      switch (dirS) {
        case "W": return HChar( SpclChar.Arrow_W );
        case "N": return HChar( SpclChar.Arrow_N );
        case "E": return HChar( SpclChar.Arrow_E );
        case "S": return HChar( SpclChar.Arrow_S );
        case "NW": return HChar( SpclChar.Arrow_NW );
        case "NE": return HChar( SpclChar.Arrow_NE );
        case "SE": return HChar( SpclChar.Arrow_SE );
        case "SW": return HChar( SpclChar.Arrow_SW );
        default: return "";
      }
    }


    private string AsHTML( IAirport airport, IList<INavaid> navaidList, IList<IFix> fixList, string dbSource )
    {
      _formatter = new AptTableGen( );
      _fixDecoration.Clear( ); // reset

      // stream Airport Runways + Approach(es) Runways first then Helipads
      var sortedRunways = airport.Runways.Where( rwy => rwy.Ident != RwALLIdent ).OrderByDescending( xx => xx.IsRunway ).ThenBy( x => x.Ident ).ToList( );
      foreach (var runway in sortedRunways) {
        StreamRunway( runway, navaidList );

        // Add Runway Approaches
        foreach (var appProc in runway.APRs.OrderBy( ap => ap.ProcRef )) {
          // take from the Airport FixList (not quering the DB again)
          var fixes = fixList.Where( fix => fix.IsAnyApproach && fix.RwyIdent == runway.Ident && fix.ProcRef == appProc.ProcRef ).ToList( );
          if (fixes.Count > 0) {
            // found Approach fixes
            StreamApproachRow( appProc, fixes, navaidList );
          }
        }
      }

      // stream Airport Procedures per Runway
      sortedRunways = airport.Runways.OrderBy( x => x.Ident ).ToList( ); // include RW00
      foreach (var runway in sortedRunways) {
        if (runway.SIDs.Count( ) > 0) {
          _formatter.ResetZebra( );
          StreamProcRunway( runway, true );
          foreach (var sid in runway.SIDs.OrderBy( s => s.Brg_deg )) {
            StreamSID( sid, airport.Coordinate );
          }
        }
        if (runway.STARs.Count( ) > 0) {
          _formatter.ResetZebra( );
          StreamProcRunway( runway, false );
          foreach (var star in runway.STARs.OrderByDescending( s => s.Brg_deg )) {
            StreamSTAR( star, airport.Coordinate );
          }
        }
      }

      // stream Airport Comms
      _formatter.ResetZebra( );
      foreach (var comm in airport.Comms) {
        StreamComm( comm );
      }

      // stream Navaids in Range
      var navaidsSorted = navaidList.Distinct( ).OrderBy( x => x.Ident ).ToList( );
      _formatter.ResetZebra( );
      foreach (var navaid in navaidsSorted) {
        if (navaid.IsILS) continue; // omit ILSs
        if (string.IsNullOrEmpty( navaid.Ident )) continue; // ?? seen some without Ident

        StreamNavaid( navaid, airport.Coordinate );
      }

      // define header rows (title and list of subtitles)
      var iata = $"{airport.IATA}";
      string title = $"{airport.Ident,-4}  -  {iata,-6}  - {airport.Name}";
      var cmt = new List<string> {
        $"{Dms.ToLat( airport.Coordinate.Lat, "dm",'\0', 0 )} {Dms.ToLon( airport.Coordinate.Lon, "dm", '\0', 0 )}"
        + $" - {airport.Coordinate.Altitude:##,##0} ft ({M_From_Ft(airport.Coordinate.Altitude):##,##0} m)",
      };

      var html = _formatter.CommitDocument( title, cmt, dbSource );
      return html;
    }

    // Detailing

    // stream one SID + transitions
    private void StreamSID( IProcedure proc, LatLon aptCoord )
    {
      if (proc.ProcedureType != UsageTyp.SID) return;

      var identList = proc.ProcedureIdents( ).ToList( );
      var fixIdent = identList.FirstOrDefault( ) ?? "";  // list starts with the common Fix
      if (identList.Count > 0) identList.Remove( fixIdent );

      // get the direction of the fix and tag the procedure with a direction arrow
      var procName = proc.Name + ((proc.NavType == NavTyp.RNAV) ? "  (RNAV)" : "");
      if (!string.IsNullOrEmpty( fixIdent )) {
        procName = ToDirectionArrow( Dms.CompassPoint( proc.Brg_deg, 2 ) ) + " " + procName; // outgoing arrow
      }

      var procData = new RwyProcRowData( ) {
        ProcName = procName,
        ProcFix = fixIdent,
        ProcTransitions = identList,
      };
      _formatter.AddSidRow( procData );
    }

    // stream one STAR + transitions
    private void StreamSTAR( IProcedure proc, LatLon aptCoord )
    {
      if (proc.ProcedureType != UsageTyp.STAR) return;

      var identList = proc.ProcedureIdents( ).ToList( );
      var fixIdent = identList.LastOrDefault( ) ?? "";  // list ends with the common Fix
      if (identList.Count > 0) identList.Remove( fixIdent );

      // get the direction of the fix and tag the procedure with a direction arrow
      var procName = proc.Name + ((proc.NavType == NavTyp.RNAV) ? "  (RNAV)" : "");
      if (!string.IsNullOrEmpty( fixIdent )) {
        procName = ToDirectionArrow( Dms.CompassPoint( proc.Brg_deg + 180, 2 ) ) + " " + procName; // incomming arrow
      }

      var procData = new RwyProcRowData( ) {
        ProcName = procName,
        ProcFix = fixIdent,
        ProcTransitions = identList,
      };
      _formatter.AddStarRow( procData );
    }

    // Runway to collect Procedures
    private void StreamProcRunway( IRunway runway, bool sid )
    {
      var rwString = (runway.Ident == RwALLIdent)
                      ? $"{HChar( SpclChar.BULLSEYE )} ALL RW"
                      : $"{ToDirectionArrow( Dms.CompassPoint( runway.Bearing_deg, 2 ) )} {runway.Ident}"; // outgoing arrow

      var rwProcData = new RwyProcRowData( ) {
        RwyS = rwString,
        RwyProcType = sid ? "SID" : "STAR",
      };
      if (sid)
        _formatter.AddRwySidRow( rwProcData );
      else
        _formatter.AddRwyStarRow( rwProcData );
    }

    private void StreamComm( IComm comm )
    {
      var commData = new CommRowData( ) {
        CommType = comm.CommType.ToString( ),
        CommFreq_mhz = comm.Frequ_Hz / 1_000_000f,
        LocationName = comm.Name,
      };
      _formatter.AddCommRow( commData );
    }

    private void StreamRunway( IRunway runway, IList<INavaid> navaidList )
    {
      string len_ft = " ";
      string len_m = " ";
      string ilsID = " ";
      float ilsFreq = float.NaN;
      float ilsSlope = float.NaN;
      float ilsRange = float.NaN;

      len_m = $"{runway.Length_m:#,##0} x {runway.Width_m:##0} m";
      len_ft = $"{Ft_From_M( runway.Length_m ):##,##0} x {Ft_From_M( runway.Width_m ):##0} ft";
      // find the ILS
      ilsID = String.Empty;
      ilsFreq = float.NaN;
      ilsSlope = float.NaN;
      ilsRange = float.NaN;
      foreach (var nav in runway.NAV_FKEYs.SelectMany( fk => navaidList.Where( nav => nav.ItemKey == fk && nav.IsILS ) )) {
        if (nav.IsILS) {
          ilsID = nav.Ident;
          ilsFreq = nav.Frequ_Hz / 1_000_000f;
        }
        if (nav.HasGS) {
          ilsSlope = nav.GsSlope_deg;
          ilsRange = nav.GsRange_nm;
        }
      }
      string rwString = (runway.Ident == RwALLIdent)
                        ? $"{HChar( SpclChar.BULLSEYE )} ALL"
                        : $"{ToDirectionArrow( Dms.CompassPoint( runway.Bearing_deg, 2 ) )} {runway.Ident}"; // outgoing arrow
      rwString += (runway.IsHelipad ? " HP" : "   ");
      string marks = (runway.IsHelipad && !string.IsNullOrEmpty( runway.HelipadMarks )) ? $" ({runway.HelipadMarks})" : "";
      var rwyData = new RwyRowData( ) {
        Rwy = rwString,
        Hdg_deg = (int)runway.Bearing_deg,
        IlsID = ilsID,
        //IlsFreq_mhz = ilsFreq,
        //IlsGsSlope_deg = ilsSlope,
        //IlsGsRange_nm = ilsRange,
        Dim_ft = len_ft,
        Dim_m = len_m,
        Surface = runway.Surface + marks,
      };
      _formatter.AddRwyRow( rwyData );
    }

    private void StreamNavaid( INavaid navaid, LatLon aptCoord )
    {
      string freqS = FrequencyS( navaid.Frequ_Hz ); // get MHz or kHz
      double distance_nm = aptCoord.DistanceTo( navaid.Coordinate, ConvConsts.EarthRadiusNm );
      if (distance_nm > (navaid.Range_nm * 1.1)) return; // range + 10% else cannot be received at the airport
      string rsiS = dNetBm98.Utilities.RSI( distance_nm, navaid.Range_nm );
      string dir = Dms.CompassPoint( aptCoord.BearingTo( navaid.Coordinate ), 2 );

      var navData = new NavaidRowData( ) {
        NavaidType = $"{navaid.NavaidType}",
        FreqS = freqS,
        ICAO = navaid.Ident,
        NavaidName = navaid.Name,
        //RangeS = $"{navaid.Range_nm:##0} nm",

        RangeS = $"{ToDirectionArrow( dir ),-2} {distance_nm:##0.0} nm ({rsiS})",

      };
      _formatter.AddNavaidRow( navData );
    }

    private void StreamApproachRow( IProcedure appProc, IList<IFix> fixes, IList<INavaid> navaidList )
    {
      float ilsSlope = float.NaN;
      float ilsRange = float.NaN;

      // found Approach fixes
      var firstFix = fixes.FirstOrDefault( );
      // the Runway fix references a Runway Waypoint, has LegAlt as AltLO, has Missed as AltHI, has NavKEY in FixInfo
      // the RunwayWaypoint references a NAV
      var rwyFix = fixes.Where( x => x.WYP.IsRunway ).FirstOrDefault( );
      var navaid = navaidList.FirstOrDefault( n => n.ItemKey == rwyFix.FixInfo );

      var freq = (navaid != null) ? navaid.Frequ_Hz : 0f;
      string freqS = FrequencyS( freq ); // get MHz or kHz
      string nav = (navaid != null) ? navaid.Ident : string.Empty;
      ilsSlope = (navaid != null) ? navaid.GsSlope_deg : 0;
      ilsRange = (navaid != null) ? navaid.GsRange_nm : 0;
      int missed = (rwyFix != null) ? rwyFix.AltitudeHi_ft : 0;
      var lastFix = fixes.Last( );
      string lastFixS = (lastFix != null) ? lastFix.WYP_FKey.IdentOfKEY( ) : string.Empty;
      string holdS = (lastFix.IsHold) ? $"Hold" : string.Empty;
      string ngTag = appProc.IsNG ? "ng" : " "; // "ms";  // NO ms flag else without ng all is ms which is obvious anyway...

      var aprData = new AprRowData( ) {
        AprType = appProc.ProcRef,
        Nav = nav,
        FreqS = freqS,
        IlsGsSlope_deg = ilsSlope,
        IlsGsRange_nm = ilsRange,
        WaypointID = firstFix.WYP_FKey.IdentOfKEY( ),
        Fix = firstFix.FixInfo.ToLowerInvariant( ),
        Alt_Hi = firstFix.AltitudeHi_ft,
        Alt_Lo = firstFix.AltitudeLo_ft,
        Alt_Missed = missed,
        Missed_WypID = lastFixS,
        Missed_Hold = holdS,
        SourceID = ngTag,
      };
      _formatter.AddAprRow( aprData );

    }

    /// <summary>
    /// Load a FP doc into the designated folder
    /// </summary>
    /// <param name="apt">An Airport</param>
    /// <param name="navaidList">A Navaids List</param>
    /// <param name="fixList">A Fix List</param>
    /// <param name="dbSource">Source of the data</param>
    /// <param name="targetFolder">The target folder</param>
    /// <returns>True if successfull</returns>
    public bool SaveDocument( IAirport apt, IList<INavaid> navaidList, IList<IFix> fixList, string dbSource, string targetFolder )
    {
      // sanity
      if (apt == null) return false;
      if (navaidList == null) return false;

      var html = AsHTML( apt, navaidList, fixList, dbSource );

      string fName = $"{apt.Ident}-{apt.IATA}-{apt.Name}.pdf";
      // remove invalid chars in filename
      fName = Path.GetInvalidFileNameChars( ).Aggregate( fName, ( current, c ) => current.Replace( c.ToString( ), string.Empty ) );

      if (bm98_Html.HtmlRenderer.Instance.Add_ToPdf_A4_Portrait(
              html,
              Path.Combine( targetFolder, fName ),
              fName,
              null )// no header
        ) {
        return true;
      }
      else {
        LOG.Info( "AptReportTable.SaveDocument", "Rendering or saving to PDF failed" );
      }
      return false;
    }

  }
}
