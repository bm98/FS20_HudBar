﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;

namespace bm98_Map
{
  /// <summary>
  /// Extend Facility Interface Objs 
  /// </summary>
  internal static class Facility_Extensions
  {
    /// <summary>
    /// Returns the other runway
    /// </summary>
    public static IRunway OtherRunway( this IRunway runway, IEnumerable<IRunway> runways )
    {
      return runways.Where( x => x.Ident == runway.OtherIdent ).FirstOrDefault( );
    }

    /// <summary>
    /// Returns this and the other runway as Pair
    /// The initial runway will be in front
    /// </summary>
    public static IEnumerable<IRunway> RunwayPair( this IRunway runway, IEnumerable<IRunway> runways )
    {
      return new List<IRunway>( ) { runway, runways.FirstOrDefault( x => x.Ident == runway.OtherIdent ) };
    }

    /// <summary>
    /// Returns a string for a VOR or NDB else empty
    /// </summary>
    /// <param name="navaid">A navaid</param>
    /// <returns>A string</returns>
    public static string VorNdbString( this INavaid navaid )
    {
      if (navaid.IsVOR) {
        return $"{navaid.NavaidType.ToString( ).Replace( "_", " " ),-8}    {navaid.Ident,-4} {navaid.Frequ_Hz / 1_000_000f,7:##0.00} MHz ({navaid.Range_nm,3:##0} nm)";
      }
      else if (navaid.IsNDB) {
        return $"{navaid.NavaidType.ToString( ).Replace( "_", " " ),-8}    {navaid.Ident,-4} {navaid.Frequ_Hz / 1000f,6:###0.0}  kHz ({navaid.Range_nm,3:##0} nm)";
      }
      return "";
    }

    /// <summary>
    /// Returns a string for a VOR or NDB else empty
    /// </summary>
    /// <param name="navaid">A navaid</param>
    /// <param name="direction">Direction to the Navaid (1 or 2chars)</param>
    /// <param name="dist_nm">Distance to </param>
    /// <param name="rsi">RSI letter</param>
    /// <returns>A string</returns>
    public static string VorNdbNameString( this INavaid navaid, string direction, double dist_nm, string rsi )
    {
      string nt = navaid.NavaidType.ToString( ).Replace( "_", " " );
      if (navaid.IsVOR) {
        return $"{nt,-8} {navaid.Ident,-4} {navaid.Frequ_Hz / 1_000_000f,7:##0.00} MHz ({direction.PadRight( 2 )} {dist_nm,5:##0.0} nm {rsi}) {navaid.Name}";
      }
      else if (navaid.IsNDB) {
        return $"{nt,-8} {navaid.Ident,-4} {navaid.Frequ_Hz / 1000f,6:###0.0} kHz  ({direction.PadRight( 2 )} {dist_nm,5:##0.0} nm {rsi}) {navaid.Name}";
      }
      // len is:41+NameLen
      return "";
    }

    /// <summary>
    /// Returns a Text Line for the Runway
    /// </summary>
    /// <param name="runway">An IRunway obj</param>
    /// <param name="navaids">List of current Navaids</param>
    /// <returns>A string</returns>
    public static string RunwayString( this IRunway runway, IList<INavaid> navaids )
    {
      if (runway.Surface == "WATER") {
        // "COMPASSDIR (hhh°) lenxwidth material"
        return $"{runway.Ident,-9} ({runway.Bearing_deg:000}°)   {runway.Length_m,4:###0}x{runway.Width_m,-3:##0} m  - {runway.Surface}";
      }
      else {
        // "RRR (hhh°) ILS  lenxwidth material"
        return $"{runway.Ident,-3} ({runway.Bearing_deg:000}°)  {RunwayIlsString( runway, navaids )} {runway.Length_m,4:###0}x{runway.Width_m,-3:##0} m  - {runway.Surface}";
      }
    }

    /// <summary>
    /// Returns a Text Line for the Runway ILS
    /// </summary>
    /// <param name="runway">An IRunway obj</param>
    /// <param name="navaids">List of current Navaids</param>
    /// <returns>A string</returns>
    public static string RunwayIlsString( this IRunway runway, IList<INavaid> navaids )
    {
      // "ICAO fff.ff GS(s.s° rr nm)"  (the ID, Frequ and GS Slope+Range)
      string id = "    ", freq = "      ", slope = "    ", range = "     ";// placeholder spacing
      if (runway.HasNAVs) {
        foreach (var nav in runway.NAV_FKEYs.SelectMany( fk => navaids.Where( nav => nav.KEY == fk && nav.IsILS ) )) {
          id = nav.Ident;
          freq = $"{nav.Frequ_Hz / 1_000_000f:000.00}";
          if (nav.HasGS) {
            slope = $"{nav.GsSlope_deg:0.0}°";
            range = $"{nav.GsRange_nm:00} nm";
          }
        }
      }
      var gsString = $"{slope} {range}";
      gsString = string.IsNullOrWhiteSpace( gsString ) ? $"   {gsString} " : $"GS({gsString})";
      return $"{id,-4}  {freq} {gsString}";
    }

    /// <summary>
    /// Returns a Text Line for the Airport Frequency
    /// </summary>
    /// <param name="comm">An IComm obj</param>
    /// <returns>A string</returns>
    public static string CommString( this IComm comm )
    {
      //  "fff.fff ID - Type"
      return $"{comm.Frequ_Hz / 1_000_000f:000.000} - {comm.Ident,-15} ({comm.CommType})";
    }

  }
}
