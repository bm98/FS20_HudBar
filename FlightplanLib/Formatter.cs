using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;

using DbgLib;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;

using FSFData;

using FlightplanLib.Flightplan;
using System.Xml.Linq;

namespace FlightplanLib
{
  /// <summary>
  /// Generic Helpers
  /// 
  ///   Restricted for library internal use
  /// </summary>
  internal class Formatter
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #region STATIC TOOLS

    /// <summary>
    /// true when an alt value is >0
    /// </summary>
    /// <param name="altitude">An altitude as double</param>
    /// <returns>True when >0 </returns>
    public static bool ValidAlt( double altitude ) => !(double.IsNaN( altitude ) || altitude <= 0.0);
    /// <summary>
    /// true when an alt value is invalid
    /// </summary>
    /// <param name="altitude">An altitude as long</param>
    /// <returns>True when invalid </returns>
    public static bool InvalidAlt( double altitude ) => !ValidAlt( altitude );

    /// <summary>
    /// true when an alt value is >0
    /// </summary>
    /// <param name="altitude">An altitude as double</param>
    /// <returns>True when >0 </returns>
    public static bool ValidAlt( long altitude ) => altitude > 0;
    /// <summary>
    /// true when an alt value is invalid
    /// </summary>
    /// <param name="altitude">An altitude as long</param>
    /// <returns>True when invalid </returns>
    public static bool InvalidAlt( long altitude ) => !ValidAlt( altitude );


    /// <summary>
    /// Returns the value in the string or NaN
    /// </summary>
    /// <param name="valueS">A value String</param>
    /// <returns>The value or NaN</returns>
    public static double GetValue( string valueS )
    {
      if (double.TryParse( valueS, out double result )) return result;
      return double.NaN;
    }

    /// <summary>
    /// Convert a WorldPosition (LLA) to a LatLon
    /// </summary>
    /// <param name="worldPosition">An LLA string</param>
    /// <returns>A LatLon</returns>
    public static LatLon ToLatLon( string worldPosition )
    {
      LatLon ll = LatLon.Empty;
      if (LLA.TryParseLLA( worldPosition, out var lat, out var lon, out var alt )) {
        ll = new LatLon( lat, lon, alt );
      }
      else {
        LOG.LogError( "X_ATCWaypoint", $"ToLatLon failed - pattern does not match\n{worldPosition}" );
      }
      return ll;
    }

    /// <summary>
    /// Clean from potential Ident decorations
    /// </summary>
    /// <param name="decoratedName">A potentialy decorated name</param>
    /// <returns></returns>
    public static string CleanName( string decoratedName )
    {
      Match match = c_wpGPX.Match( decoratedName );
      if (match.Success) {
        return CleanNameGPX( decoratedName );
      }
      match = c_wpB21.Match( decoratedName );
      if (match.Success) {
        return CleanB21SoaringName( decoratedName );
      }
      return decoratedName.TrimEnd( ); ; // no matches
    }

    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="decoratedName">Possibly a decorated name</param>
    /// <returns>A string</returns>
    public static string GetDecoration( string decoratedName )
    {
      Match match = c_wpGPX.Match( decoratedName );
      if (match.Success) {
        return GetGPXDecoration( decoratedName );
      }
      match = c_wpB21.Match( decoratedName );
      if (match.Success) {
        return GetB21SoaringDecoration( decoratedName );
      }
      // seems a regular name
      return "";
    }

    #region LNM GPX decorations

    // LNM Has potentially added distances e.g. RW22+3 i.e. RW22 +3nm out
    private readonly static Regex c_wpGPX =
  new Regex( @"^(?<name>RW[^\+]*)(?<dist>(\+)\d{1,3})",
        RegexOptions.Compiled );

    /// <summary>
    /// Clean from potential GPX Ident decorations
    /// </summary>
    /// <param name="gpxName">A potentialy decorated name</param>
    /// <returns></returns>
    private static string CleanNameGPX( string gpxName )
    {
      Match match = c_wpGPX.Match( gpxName );
      if (match.Success) {
        if (match.Groups["name"].Success) {
          return match.Groups["name"].Value.TrimEnd( );
        }
      }
      // seems a regular name
      return gpxName.TrimEnd( );
    }

    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="gpxName">Possibly a LNM GPX Waypoint name</param>
    /// <returns>A string</returns>
    private static string GetGPXDecoration( string gpxName )
    {
      // decoration see above
      Match match = c_wpGPX.Match( gpxName );
      if (match.Success) {
        var deco = "";
        if (match.Groups["dist"].Success) { deco += $"{match.Groups["dist"].Value} nm "; }
        return deco;
      }
      // seems a regular name
      return "";
    }

    #endregion

    #region B21 Soaring Waypoint decorations

    private readonly static Regex c_wpB21 =
      new Regex( @"^(?<start_end>\*)?(?<name>([^\+])*)(?<elevation>(\+|-)\d{1,5})(?<maxAlt>\|\d{1,5})?(?<minAlt>\/\d{1,5})?(?<radius>x\d{1,5})?",
        RegexOptions.Compiled );

    /// <summary>
    /// The B21 Soaring engine uses tagged names for the Task Management
    ///  [*]Name+Elev[|MaxAlt[/MinAlt]][xRadius]
    ///  1st * - Start of Task
    ///  2nd * - End of Task
    ///  Name  - the WP name
    ///  +     - Separator
    ///  Elev  - Waypoint Elevation  [ft}
    ///  |MaxAlt - Max alt of the gate [ft}
    ///  /MinAlt - Min alt of the gate [ft}
    ///  xRadius - Radius of the gate [meters]
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    private static string CleanB21SoaringName( string b21name )
    {
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        if (match.Groups["name"].Success) {
          return match.Groups["name"].Value.TrimEnd( );
        }
      }
      // seems a regular name
      return b21name.TrimEnd( );
    }
    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    private static string GetB21SoaringDecoration( string b21name )
    {
      // decoration see above
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        var deco = "";
        if (match.Groups["start_end"].Success) { deco += "* "; }
        if (match.Groups["elevation"].Success) { deco += $"{match.Groups["elevation"].Value} ft "; }
        if (match.Groups["minAlt"].Success) { deco += $"(min {match.Groups["minAlt"].Value}) "; }
        if (match.Groups["maxAlt"].Success) { deco += $"(max {match.Groups["maxAlt"].Value}) "; }
        if (match.Groups["radius"].Success) { deco += $"(rad {match.Groups["radius"].Value} m)"; }
        return deco;
      }
      // seems a regular name
      return "";
    }
    #endregion

    #endregion

    #region Location Expansion

    /// <summary>
    /// Return the Airport and optional Runway Waypoints from a Location
    /// </summary>
    /// <param name="loc">A location</param>
    /// <param name="addRw">True to add the Runway</param>
    /// <param name="onDeparture">True when on departure, false when on arrival</param>
    /// <returns>List of Waypoints</returns>
    public static WaypointList ExpandLocationAptRw( Location loc, bool addRw, bool onDeparture )
    {
      var wypList = new WaypointList( );
      if (!loc.LatLonAlt_ft.IsEmpty) {
        // add an 'Airport' 
        var wyp = new Flightplan.Waypoint( ) {
          WaypointType = WaypointTyp.APT,
          OnDeparture = onDeparture,
          SourceIdent = loc.Icao_Ident.ICAO,
          CommonName = loc.Name,
          LatLonAlt_ft = loc.LatLonAlt_ft,
          Icao_Ident = loc.Icao_Ident,
          RunwayNumber_S = loc.RunwayNumber_S,
          RunwayDesignation = loc.RunwayDesignation,
          Stage = "TAXI",
        };
        wypList.Add( wyp );
        // add an Airport WYP Runway if we know it
        if (addRw && !loc.RunwayLatLonAlt_ft.IsEmpty) {
          wyp = new Flightplan.Waypoint( ) {
            WaypointType = WaypointTyp.RWY,
            OnDeparture = onDeparture,
            SourceIdent = loc.Runway_Ident,
            CommonName = loc.Runway_Ident,
            LatLonAlt_ft = loc.RunwayLatLonAlt_ft,
            Icao_Ident = new IcaoRec( ) { ICAO = loc.Runway_Ident, Region = loc.Icao_Ident.Region, AirportRef = loc.Icao_Ident.ICAO },
            RunwayNumber_S = loc.RunwayNumber_S,
            RunwayDesignation = loc.RunwayDesignation,
            Stage = "TAKEOFF",
          };
          wypList.Add( wyp );
        }
      }
      return wypList;
    }

    /// <summary>
    /// Return the Runway and then the Airport from a Location
    /// </summary>
    /// <param name="loc">A location</param>
    /// <param name="addRw">True to add the Runway</param>
    /// <param name="aprProcRef">Approach ProcRef or empty</param>
    /// <param name="onDeparture">True when on departure, false when on arrival</param>
    /// <returns>List of Waypoints</returns>
    public static WaypointList ExpandLocationRwApt( Location loc, bool addRw, string aprProcRef, bool onDeparture )
    {
      var wypList = new WaypointList( );
      if (!loc.LatLonAlt_ft.IsEmpty) {
        // add an Airport WYP Runway if we know it
        Flightplan.Waypoint wyp;
        if (addRw && !loc.RunwayLatLonAlt_ft.IsEmpty) {
          wyp = new Flightplan.Waypoint( ) {
            WaypointType = WaypointTyp.RWY,
            OnDeparture = onDeparture,
            SourceIdent = loc.Runway_Ident,
            CommonName = loc.Runway_Ident,
            LatLonAlt_ft = loc.RunwayLatLonAlt_ft,
            Icao_Ident = new IcaoRec( ) { ICAO = loc.Runway_Ident, Region = loc.Icao_Ident.Region, AirportRef = loc.Icao_Ident.ICAO },
            RunwayNumber_S = loc.RunwayNumber_S,
            RunwayDesignation = loc.RunwayDesignation,
            Stage = "LAND",
          };
          wypList.Add( wyp );
        }
        // add an 'Airport' 
        wyp = new Flightplan.Waypoint( ) {
          WaypointType = WaypointTyp.APT,
          OnDeparture = onDeparture,
          SourceIdent = loc.Icao_Ident.ICAO,
          CommonName = loc.Name,
          LatLonAlt_ft = loc.LatLonAlt_ft,
          Icao_Ident = loc.Icao_Ident,
          ApproachTypeS = aprProcRef.ProcOf( ),
          ApproachSuffix = aprProcRef.SuffixOf( ),
          RunwayNumber_S = loc.RunwayNumber_S,
          RunwayDesignation = loc.RunwayDesignation,
          Stage = "TAXI",
        };
        wypList.Add( wyp );
      }
      return wypList;
    }

    /// <summary>
    /// Expands an Airport into a Location
    /// </summary>
    /// <param name="apt">An Airport</param>
    /// <param name="runway">A Runway</param>
    /// <param name="locationType">The Type of location to return</param>
    /// <returns>A Location or null</returns>
    public static Location ExpandAirport( IAirport apt, IRunway runway, LocationTyp locationType )
    {
      if (apt == null) return null;

      var loc = new Location( ) {
        LocationType = locationType,
        Icao_Ident = new IcaoRec( ) { ICAO = apt.Ident, Region = apt.Region },
        Iata_Ident = apt.IATA,
        Name = apt.Name,
        LatLonAlt_ft = apt.Coordinate,
        RunwayNumber_S = (runway != null) ? runway.Ident.RwNumberOf( ) : "",
        RunwayDesignation = (runway != null) ? runway.Ident.RwDesignationOf( ) : "",
        RunwayLatLonAlt_ft = (runway != null) ? runway.StartCoordinate : LatLon.Empty,
      };
      loc.SetAirportDBItem( apt );

      return loc;
    }

    /// <summary>
    /// Expands an Airport into a Location
    /// </summary>
    /// <param name="apt">An Airport</param>
    /// <param name="runwayIdent">A RunwayIdent or empty</param>
    /// <param name="locationType">The Type of location to return</param>
    /// <returns>A Location or null</returns>
    public static Location ExpandAirport( IAirport apt, string runwayIdent, LocationTyp locationType )
    {
      if (apt == null) return null;

      var rw = apt.Runways.FirstOrDefault( rwy => rwy.Ident == runwayIdent );
      return ExpandAirport( apt, rw, locationType );
    }

    /// <summary>
    /// Expands an Airport ICAO into a Location
    /// </summary>
    /// <param name="icao">Airport ICAO code</param>
    /// <param name="runwayIdent">A RunwayIdent or empty</param>
    /// <param name="locationType">The Type of location to return</param>
    /// <returns>A Location or null</returns>
    public static Location ExpandAirport( string icao, string runwayIdent, LocationTyp locationType )
    {
      var apt = DbLookup.GetAirport( icao, bm98_hbFolders.Folders.GenAptDBFile );
      return ExpandAirport( apt, runwayIdent, locationType );
    }

    #endregion

    #region Waypoint Expansion

    /// <summary>
    /// Expand a SID into it's Flighplan Waypoints
    /// </summary>
    /// <param name="sid">The SID</param>
    /// <param name="transition">A Transition or empty</param>
    /// <returns>A Waypoint List</returns>
    public static WaypointList ExpandSID( IProcedure sid, string transition )
    {
      var wypList = new WaypointList( );
      // sanity
      if (sid == null) return wypList;

      var srcWyps = DbLookup.ExpandSIDFixes( sid, transition, bm98_hbFolders.Folders.GenAptDBFile );
      // create Waypoint
      foreach (var sWyp in srcWyps) {
        if (sWyp.WYP == default) continue; // ignore where we did not find a Waypoint for the Fix
        var wyp = new Flightplan.Waypoint( ) {
          WaypointType = sWyp.WYP.WaypointType,
          WaypointUsage = sWyp.WaypointUsage,
          OnDeparture = true,
          SourceIdent = sWyp.WYP.Ident,
          CommonName = sWyp.WYP.Ident,
          LatLonAlt_ft = sWyp.WYP.Coordinate,
          Icao_Ident = new IcaoRec( ) { ICAO = sWyp.WYP.Ident, Region = sWyp.WYP.Region },
          AltitudeLimitLo_ft = sWyp.AltitudeLo_ft,
          AltitudeLimitHi_ft = sWyp.AltitudeHi_ft,
          RunwayNumber_S = sid.RunwayIdent.RwNumberOf( ),
          RunwayDesignation = sid.RunwayIdent.RwDesignationOf( ),
          SID_Ident = ProcS( sid.Ident, transition ),
          Stage = sWyp.WaypointUsage.ToString( ),
        };
        if (sWyp.WYP.HasNAV) {
          var nv = DbLookup.GetNavaid_ByKey( sWyp.WYP.Nav_FKEY, bm98_hbFolders.Folders.GenAptDBFile );
          if (nv != null) { wyp.Frequency = FrequencyS( nv.Frequ_Hz ); }
        }
        wypList.Add( wyp );
      }
      return wypList;
    }

    /// <summary>
    /// Expand a STAR into it's Flighplan Waypoints
    /// </summary>
    /// <param name="star">The STAR</param>
    /// <param name="transition">A Transition or empty</param>
    /// <returns>A Waypoint List</returns>
    public static WaypointList ExpandSTAR( IProcedure star, string transition )
    {
      var wypList = new WaypointList( );
      // sanity
      if (star == null) return wypList;

      var srcWyps = DbLookup.ExpandSTARFixes( star, transition, bm98_hbFolders.Folders.GenAptDBFile );
      // create Waypoint
      foreach (var sWyp in srcWyps) {
        if (sWyp.WYP == default) continue; // ignore where we did not find a Waypoint for the Fix
        var wyp = new Flightplan.Waypoint( ) {
          WaypointType = sWyp.WYP.WaypointType,
          WaypointUsage = sWyp.WaypointUsage,
          OnDeparture = false,
          SourceIdent = sWyp.WYP.Ident,
          CommonName = sWyp.WYP.Ident,
          LatLonAlt_ft = sWyp.WYP.Coordinate,
          Icao_Ident = new IcaoRec( ) { ICAO = sWyp.WYP.Ident, Region = sWyp.WYP.Region },
          AltitudeLimitLo_ft = sWyp.AltitudeLo_ft,
          AltitudeLimitHi_ft = sWyp.AltitudeHi_ft,
          RunwayNumber_S = star.RunwayIdent.RwNumberOf( ),
          RunwayDesignation = star.RunwayIdent.RwDesignationOf( ),
          STAR_Ident = ProcS( star.Ident, transition ),
          Stage = sWyp.WaypointUsage.ToString( ),
        };
        if (sWyp.WYP.HasNAV) {
          var nv = DbLookup.GetNavaid_ByKey( sWyp.WYP.Nav_FKEY, bm98_hbFolders.Folders.GenAptDBFile );
          if (nv != null) { wyp.Frequency = FrequencyS( nv.Frequ_Hz ); }
        }
        wypList.Add( wyp );
      }
      return wypList;
    }

    /// <summary>
    /// Expand a Approach into it's Flighplan Waypoints
    /// </summary>
    /// <param name="loc">The Destination Location</param>
    /// <param name="apr">The Approach</param>
    /// <param name="transition">The Approach Transition</param>
    /// <param name="preferRNAV">True to prefer RNAV transitions</param>
    /// <returns>A Waypoint List</returns>
    public static WaypointList ExpandAPR( Location loc, IProcedure apr, string transition, bool preferRNAV )
    {
      var wypList = new WaypointList( );
      // sanity
      if (apr == null) return wypList;

      // APR Transition
      if (!string.IsNullOrEmpty( transition )) {
        // try to find an approach transition
        KeyValuePair<string, List<IFix>> txWyps;
        if (preferRNAV) {
          txWyps = apr.TransitionCat.Where( kv => kv.Key.StartsWith( transition ) && kv.Key.Contains( "(RNAV)" ) ).FirstOrDefault( ); // take the first..
        }
        else {
          txWyps = apr.TransitionCat.Where( kv => kv.Key.StartsWith( transition ) && kv.Key.Contains( "(RNAV)" ) ).FirstOrDefault( ); // take the first..
        }
        if (string.IsNullOrEmpty( txWyps.Key )) {
          // nothing specific found; try get any
          txWyps = apr.TransitionCat.Where( kv => kv.Key.StartsWith( transition ) ).FirstOrDefault( );
        }
        if (!string.IsNullOrEmpty( txWyps.Key )) {
          // expand and create Waypoints
          DbLookup.ExpandFixList( txWyps.Value, bm98_hbFolders.Folders.GenAptDBFile );

          foreach (var sWyp in txWyps.Value) {
            if (sWyp.WYP == default) continue; // ignore where we did not find a Waypoint for the Fix
            var wyp = new Flightplan.Waypoint( ) {
              WaypointType = sWyp.WYP.WaypointType,
              WaypointUsage = sWyp.WaypointUsage,
              OnDeparture = false,
              SourceIdent = sWyp.WYP.Ident,
              CommonName = sWyp.WYP.Ident,
              LatLonAlt_ft = sWyp.WYP.Coordinate,
              Icao_Ident = new IcaoRec( ) { ICAO = sWyp.WYP.Ident, Region = sWyp.WYP.Region },
              AltitudeLimitLo_ft = sWyp.AltitudeLo_ft,
              AltitudeLimitHi_ft = sWyp.AltitudeHi_ft,
              RunwayNumber_S = apr.RunwayIdent.RwNumberOf( ),
              RunwayDesignation = apr.RunwayIdent.RwDesignationOf( ),
              ApproachTypeS = apr.NavType.ToString( ),
              ApproachSuffix = apr.NavSuffix,
              Stage = sWyp.WaypointUsage.ToString( ),
            };
            if (sWyp.WYP.HasNAV) {
              var nv = DbLookup.GetNavaid_ByKey( sWyp.WYP.Nav_FKEY, bm98_hbFolders.Folders.GenAptDBFile );
              if (nv != null) { wyp.Frequency = FrequencyS( nv.Frequ_Hz ); }
            }
            wypList.Add( wyp );
          }
        }
      } // APR Transition

      // get all Approach fixes from this Approach
      var srcWyps = DbLookup.ExpandAPRFixes( apr, bm98_hbFolders.Folders.GenAptDBFile );
      // create Waypoint
      int sequ = 1; // sequence the Approach to help the mapper to avoid the last Wyps
      foreach (var sWyp in srcWyps) {
        if (sWyp.WYP == default) continue; // ignore where we did not find a Waypoint for the Fix

        // usually the last Wyp in the transition and the first in the Approach are the same
        // use the Approach one ...
        if (wypList.Count > 0 && sWyp.IdentOf == wypList.Last( ).Ident) {
          wypList.Remove( wypList.Last( ) );// remove last
        }
        // Runway?
        if (sWyp.WYP.WaypointType == WaypointTyp.RWY) {
          // create the Runway Wyp
          var wyp = new Flightplan.Waypoint( ) {
            WaypointType = sWyp.WYP.WaypointType,
            WaypointUsage = sWyp.WaypointUsage,
            OnDeparture = false,
            SourceIdent = sWyp.WYP.Ident,
            CommonName = sWyp.WYP.Ident,
            LatLonAlt_ft = loc.RunwayLatLonAlt_ft, // subst with LLA of the runway
            Icao_Ident = new IcaoRec( ) { ICAO = sWyp.WYP.Ident, Region = sWyp.WYP.Region },
            AltitudeLimitLo_ft = (int)loc.RunwayLatLonAlt_ft.Altitude, // subst with RW Alt
            AltitudeLimitHi_ft = (int)loc.RunwayLatLonAlt_ft.Altitude, // subst with RW Alt
            RunwayNumber_S = apr.RunwayIdent.RwNumberOf( ),
            RunwayDesignation = apr.RunwayIdent.RwDesignationOf( ),
            ApproachTypeS = apr.NavType.ToString( ),
            ApproachSuffix = apr.NavSuffix,
            ApproachSequence = sequ++,
            Stage = "LAND",
          };
          wypList.Add( wyp );
        }
        else {
          // create the APR/MAPR Wyp
          var wyp = new Flightplan.Waypoint( ) {
            WaypointType = sWyp.WYP.WaypointType,
            WaypointUsage = sWyp.WaypointUsage,
            OnDeparture = false,
            SourceIdent = sWyp.WYP.Ident,
            CommonName = sWyp.WYP.Ident,
            LatLonAlt_ft = sWyp.WYP.Coordinate,
            Icao_Ident = new IcaoRec( ) { ICAO = sWyp.WYP.Ident, Region = sWyp.WYP.Region },
            AltitudeLimitLo_ft = sWyp.AltitudeLo_ft,
            AltitudeLimitHi_ft = sWyp.AltitudeHi_ft,
            RunwayNumber_S = apr.RunwayIdent.RwNumberOf( ),
            RunwayDesignation = apr.RunwayIdent.RwDesignationOf( ),
            ApproachTypeS = apr.NavType.ToString( ),
            ApproachSuffix = apr.NavSuffix,
            ApproachSequence = sequ++,
            Stage = sWyp.WaypointUsage.ToString( ),
          };
          wypList.Add( wyp );
        }
      }
      return wypList;
    }

    private static string ProcS( string proc, string trans )
    {
      return proc + (string.IsNullOrEmpty( trans ) ? "" : $".{trans}");
    }

    #endregion

#pragma warning disable CS0168 // Variable is declared but never used

    #region XML Serializaion

    /// <summary>
    /// Reads from the open stream one entry
    /// </summary>
    /// <param name="xStream">An open stream at position</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlStream<T>( Stream xStream )
    {
      try {
        var xmlSerializer = new XmlSerializer( typeof( T ) );
        xmlSerializer.UnknownNode += XmlSerializer_UnknownNode;
        xmlSerializer.UnknownAttribute += XmlSerializer_UnknownAttribute;
        xmlSerializer.UnknownElement += XmlSerializer_UnknownElement;
        object objResponse = xmlSerializer.Deserialize( xStream );
        var xmlResults = (T)objResponse;
        xStream.Flush( );
        return xmlResults;
      }
      catch (Exception e) {
        return default( T );
      }
    }

    private static void XmlSerializer_UnknownElement( object sender, XmlElementEventArgs e )
    {
      LOG.LogError( "XmlSerializer_UnknownElement", $"Unknown Element at {e.LineNumber}: {e.Element.Name}" );
    }

    private static void XmlSerializer_UnknownAttribute( object sender, XmlAttributeEventArgs e )
    {
      LOG.LogError( "XmlSerializer_UnknownAttribute", $"Unknown Attribute at {e.LineNumber}: {e.Attr.Name}" );
    }

    private static void XmlSerializer_UnknownNode( object sender, XmlNodeEventArgs e )
    {
      LOG.LogError( "XmlSerializer_UnknownNode", $"Unknown Node at {e.LineNumber}: {e.Name}" );
    }

    /// <summary>
    /// Reads from the supplied string
    /// </summary>
    /// <param name="xmlString">A Xml formatted string</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlString<T>( string xmlString )
    {
      try {
        T xmlResults = default;
        using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( xmlString ) )) {
          xmlResults = FromXmlStream<T>( ms );
        }
        return xmlResults;
      }
      catch (Exception e) {
        return default;
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// Tries to aquire a shared Read Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="xFilename">The Xml Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlFile<T>( string xFilename )
    {
      T retVal = default( T );
      if (!File.Exists( xFilename )) {
        return retVal;
      }

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( xFilename, FileMode.Open, FileAccess.Read, FileShare.Read )) {
            retVal = FromXmlStream<T>( ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }

      return retVal;
    }

    /// <summary>
    /// Write to the open stream one entry
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="xStream">An open stream at position</param>
    /// <returns>True if successfull</returns>
    public static bool ToXmlStream<T>( T data, Stream xStream )
    {
      try {
        var xmlSerializer = new XmlSerializer( typeof( T ) );
        xmlSerializer.Serialize( xStream, data );
        return true;
      }
      catch (Exception e) {
        return false; // bails at data==null or formatting issues
      }
    }

    /// <summary>
    /// Write to a file one entry
    /// Tries to aquire an exclusive Write Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="xFilename">The Xml Filename</param>
    /// <returns>True if successfull</returns>
    public static bool ToXmlFile<T>( T data, string xFilename )
    {
      bool retVal = false;

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( xFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None )) {
            retVal = ToXmlStream( data, ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }
      return retVal;
    }

    #endregion

    #region JSON Serialization

    /// <summary>
    /// Reads from the open stream one entry
    /// </summary>
    /// <param name="jStream">An open stream at position</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonStream<T>( Stream jStream )
    {
      try {
        var jsonSerializer = new DataContractJsonSerializer( typeof( T ) );
        object objResponse = jsonSerializer.ReadObject( jStream );
        var jsonResults = (T)objResponse;
        jStream.Flush( );
        return jsonResults;
      }
      catch (Exception e) {
        return default( T );
      }
    }

    /// <summary>
    /// Reads from the supplied string
    /// </summary>
    /// <param name="jString">A JSON formatted string</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonString<T>( string jString )
    {
      try {
        T jsonResults = default;
        using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( jString ) )) {
          jsonResults = FromJsonStream<T>( ms );
        }
        return jsonResults;
      }
      catch (Exception e) {
        return default;
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// Tries to aquire a shared Read Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonFile<T>( string jFilename )
    {
      T retVal = default( T );
      if (!File.Exists( jFilename )) {
        return retVal;
      }

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( jFilename, FileMode.Open, FileAccess.Read, FileShare.Read )) {
            retVal = FromJsonStream<T>( ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }

      return retVal;
    }

    /// <summary>
    /// Write to the open stream one entry
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="jStream">An open stream at position</param>
    /// <returns>True if successfull</returns>
    public static bool ToJsonStream<T>( T data, Stream jStream )
    {
      try {
        var jsonSerializer = new DataContractJsonSerializer( typeof( T ) );
        jsonSerializer.WriteObject( jStream, data );
        return true;
      }
      catch (Exception e) {
        return false; // bails at data==null or formatting issues
      }
    }

    /// <summary>
    /// Write to a file one entry
    /// Tries to aquire an exclusive Write Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>True if successfull</returns>
    public static bool ToJsonFile<T>( T data, string jFilename )
    {
      bool retVal = false;

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( jFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None )) {
            retVal = ToJsonStream( data, ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }
      return retVal;
    }

    #endregion

#pragma warning restore CS0168 // Variable is declared but never used
  }
}
