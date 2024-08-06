using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;
using FSimFacilityIF;
using static FSimFacilityIF.Extensions;
using FlightplanLib.Flightplan;
using bm98_hbFolders;

namespace FlightplanLib.GPX.GPXDEC
{

  /// <summary>
  /// An GPX rte FlightPlan Element
  /// </summary>
  [XmlRoot( ElementName = "rte", IsNullable = false )]
  public class X_Rte
  {
    // Elements
    /// <summary>
    /// The Header element
    /// </summary>
    [XmlElement( ElementName = "name" )]
    public string Title { get; set; } = "";
    /// <summary>
    /// The SimData element (MSFS, ...)
    /// </summary>
    [XmlElement( ElementName = "desc" )]
    public string Description { get; set; } = "";

    /// <summary>
    /// The DepartureLLA element
    /// </summary>
    [XmlElement( ElementName = "rtept" )]
    public List<X_RtePt> WaypointCat { get; set; } = new List<X_RtePt>( );



    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    [XmlIgnore]
    public bool IsValid {
      get {
        if (WaypointCat == null) return false;
        if (WaypointCat.Count <= 0) return false;
        PostProc( );
        InsertProcedures( );
        return true;
      }
    }


    /// <summary>
    /// The Departure Airport if any
    /// </summary>
    [XmlIgnore]
    public X_RtePt DepartureAirport {
      get {
        PostProc( );
        return _depApt;
      }
    }
    /// <summary>
    /// The Departure Runway if any
    /// </summary>
    [XmlIgnore]
    public X_RtePt DepartureRw {
      get {
        PostProc( );
        return _depRwy;
      }
    }

    /// <summary>
    /// The Arrival Airport if any
    /// </summary>
    [XmlIgnore]
    public X_RtePt ArrivalAirport {
      get {
        PostProc( );
        return _arrApt;
      }
    }
    /// <summary>
    /// The Arrival Runway if any
    /// </summary>
    [XmlIgnore]
    public X_RtePt ArrivalRw {
      get {
        PostProc( );
        return _arrRwy;
      }
    }

    /// <summary>
    /// Departure LatLonAlt (derived field)
    /// </summary>
    [XmlIgnore]
    public LatLon DEP_LatLon => DepartureAirport.LatLonAlt_ft;
    /// <summary>
    /// Departure Latitude (derived field)
    /// </summary>
    [XmlIgnore]
    public double DEP_Lat => DEP_LatLon.Lat;
    /// <summary>
    /// Departure Longitude (derived field)
    /// </summary>
    [XmlIgnore]
    public double DEP_Lon => DEP_LatLon.Lon;
    /// <summary>
    /// Departure Altitude ft (derived field)
    /// </summary>
    [XmlIgnore]
    public float DEP_Altitude_ft => (float)DEP_LatLon.Altitude;


    /// <summary>
    /// Destination LatLonAlt (derived field)
    /// </summary>
    [XmlIgnore]
    public LatLon DST_LatLon => ArrivalAirport.LatLonAlt_ft;
    /// <summary>
    /// Destination Latitude (derived field)
    /// </summary>
    [XmlIgnore]
    public double DST_Lat => DST_LatLon.Lat;
    /// <summary>
    /// Destination Longitude (derived field)
    /// </summary>
    [XmlIgnore]
    public double DST_Lon => DST_LatLon.Lon;
    /// <summary>
    /// Destination Altitude ft (derived field)
    /// </summary>
    [XmlIgnore]
    public float DST_Altitude_ft => (float)DST_LatLon.Altitude;

    /// <summary>
    /// Cruising Altitude ft number (derived field)
    /// </summary>
    [XmlIgnore]
    public float CruisingAlt_ft {
      get {
        PostProc( );
        // already set
        if (!float.IsNaN( _cruiseAlt_ft )) return _cruiseAlt_ft;
        // try to find out... once
        float cAlt = 0.0f;
        foreach (var wyp in WaypointCat) {
          cAlt = (wyp.AltitudeRounded_ft > cAlt) ? wyp.AltitudeRounded_ft : cAlt;
        }
        _cruiseAlt_ft = cAlt;
        return _cruiseAlt_ft;
      }
    }

    /// <summary>
    /// Flight plan type as enum (derived field)
    /// </summary>
    [XmlIgnore]
    public TypeOfFlightplan FlightPlanType {
      get {
        PostProc( );
        return _fpType;
      }
    }

    /// <summary>
    /// Route type as enum (derived field)
    /// </summary>
    [XmlIgnore]
    public TypeOfRoute RouteType => ToTypeOfRoute( FlightPlanType, CruisingAlt_ft );

    private static TypeOfRoute ToTypeOfRoute( TypeOfFlightplan typeOfFlightplan, float cruiseAlt_ft )
    {
      if (typeOfFlightplan == TypeOfFlightplan.VFR) { return TypeOfRoute.VFR; }
      else if (typeOfFlightplan == TypeOfFlightplan.VOR) { return TypeOfRoute.VOR; }
      else {
        // IFR types
        if (cruiseAlt_ft >= 18000f) return TypeOfRoute.HighAlt;
        else return TypeOfRoute.LowAlt;
      }
    }

    #region Post Processing

    private bool _ppDone = false;
    private bool _procDone = false;
    private X_RtePt _depApt = null;
    private X_RtePt _depRwy = null;
    private X_RtePt _arrApt = null;
    private X_RtePt _arrRwy = null;
    private float _cruiseAlt_ft = float.NaN;
    private TypeOfFlightplan _fpType = TypeOfFlightplan.VFR;

    // Post Processing to eval the derived values in one go
    private void PostProc( )
    {
      if (_ppDone) return;

      // Assuming they arrive in an order from LNM
      foreach (var wyp in WaypointCat) {
        if (!wyp.IsValid) continue; // ??

        if (_depApt == null) {
          // need Departure Airport
          if (wyp.WaypointType == WaypointTyp.APT) {
            _depApt = wyp;
          }
        }

        else if (_depRwy == null) {
          // need Departure Runway
          if (wyp.WaypointType == WaypointTyp.RWY) {
            // RWY close enough?? <8 nm from Apt ??
            if (wyp.LatLonAlt_ft.DistanceTo( _depApt.LatLonAlt_ft, ConvConsts.EarthRadiusNm ) < 8) {
              _depRwy = wyp;
            }
            else {
              // could already be the Arrival Runway then
              _depRwy = new X_RtePt( ); // add an empty one to Dep
              _arrRwy = wyp; // add as Arr
            }
          }
        }

        else if (_arrRwy == null) {
          // need Arrival Runway
          if (wyp.WaypointType == WaypointTyp.RWY) {
            _arrRwy = wyp;
          }
        }

        else if (_arrApt == null) {
          // need Arrival Airport
          if (wyp.WaypointType == WaypointTyp.APT) {
            _arrApt = wyp;
            // RWY close enough?? <8 nm ??
            if (_arrRwy.LatLonAlt_ft.DistanceTo( _arrApt.LatLonAlt_ft, ConvConsts.EarthRadiusNm ) > 8)
              _arrRwy = new X_RtePt( ); // add an empty one to Dep
          }
        }
      }
      // clean the ones not found
      if (_depApt == null) _depApt = new X_RtePt( );
      if (_depRwy == null) _depRwy = new X_RtePt( );
      if (_arrApt == null) _arrApt = new X_RtePt( );
      if (_arrRwy == null) _arrRwy = new X_RtePt( );

      DecodeDescription( );
      _ppDone = true;
    }

    // attempts to derive the procedure names and fix info from the given waypoints
    private void InsertProcedures( )
    {
      if (_procDone) return;

      // we collect Proc candidates hence lists below
      List<IProcedure> collectedSid = new List<IProcedure>( );
      List<IProcedure> collectedStar = new List<IProcedure>( );
      List<IProcedure> collectedApr = new List<IProcedure>( );

      // selected ProcRefs
      string selectedSID = "";
      string selectedSTAR = "";
      string selectedAPR = "";

      IAirport depAirport = null;
      IAirport arrAirport = null;

      // using the Database to get the Airports
      using (var _db = new FSFData.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (_db.Open( Folders.GenAptDBFile )) {
          // Airports
          depAirport = _db.DbReader.GetAirport( DepartureAirport.ICAO );
          arrAirport = _db.DbReader.GetAirport( ArrivalAirport.ICAO );
        }
      }

      // Runways
      var depRwy = depAirport?.Runways.FirstOrDefault( rwy => rwy.Ident == DepartureRw.RunwayIdent );
      var arrRwy = arrAirport?.Runways.FirstOrDefault( rwy => rwy.Ident == ArrivalRw.RunwayIdent );

      // SID - find a SID having a Wyp from the list...
      if (depAirport != null) {
        // walk all Wyps and find Proc in Runway Procs
        foreach (var wyp in WaypointCat) {
          var proc = depRwy?.SIDs.FirstOrDefault( s => s.HasFixIdent( wyp.ICAO ) );
          if (proc != null) collectedSid.Add( proc );
        }
        if (collectedSid.Count == 0) {
          // try airport Procs if not found in runways
          foreach (var wyp in WaypointCat) {
            var proc = depAirport.SIDs( ).FirstOrDefault( s => s.HasFixIdent( wyp.ICAO ) );
            if (proc != null) collectedSid.Add( proc );
          }
        }
      }
      // report the Proc(s) having the most wins...
      // thank you.. https://stackoverflow.com/questions/10335223/how-to-build-a-histogram-for-a-list-of-int-in-c-sharp
      if (collectedSid.Count > 0) {
        // List must be non empty... take all groups which have the same count as the one with the most counts
        var groups = collectedSid.GroupBy( proc => proc.Ident ).OrderByDescending( grp => grp.Count( ) );
        var selGroups = groups.Where( grp => grp.Count( ) == groups.First( ).Count( ) ); // take all having max count
        IProcedure refProc = null;
        // having multiples with the same count ... RW ones were preferred above
        // return the first found ...
        selectedSID = selGroups.First( ).Key;
        refProc = selGroups.First( ).FirstOrDefault( ); // use the first of the most counted group, mostly the best..
        foreach (var wyp in WaypointCat) {
          // if a wyp appears in the selected proc - tag it
          if (refProc.HasFixIdent( wyp.ICAO )) {
            var fix = refProc.FixWithIdent( wyp.ICAO );
            if (fix != null) {
              wyp.AltLo_ft = fix.AltitudeLo_ft;
              wyp.AltHi_ft = fix.AltitudeHi_ft;
              wyp.SpeedLimit_kt = fix.SpeedLimit_kt;
            }
            // Add proc if the Wyp is part of the procedure
            wyp.SID_Ident = selectedSID;
          }
        }
      }
      // STAR
      if (arrAirport != null) {
        // walk all Wyps and find Proc in Runway Procs
        foreach (var wyp in WaypointCat) {
          var proc = arrRwy?.STARs.FirstOrDefault( s => s.HasFixIdent( wyp.ICAO ) );
          if (proc != null) collectedStar.Add( proc );
        }
        if (collectedStar.Count == 0) {
          // try airport Procs if not found in runways
          foreach (var wyp in WaypointCat) {
            var proc = arrAirport.STARs( ).FirstOrDefault( s => s.HasFixIdent( wyp.ICAO ) );
            if (proc != null) collectedStar.Add( proc );
          }
        }
        if (collectedStar.Count > 0) {
          // List must be non empty... take all groups which have the same count as the one with the most counts
          var groups = collectedStar.GroupBy( proc => proc.Ident ).OrderByDescending( grp => grp.Count( ) );
          var selGroups = groups.Where( grp => grp.Count( ) == groups.First( ).Count( ) ); // take all having max count
          IProcedure refProc = null;
          // having multiples with the same count ... RW ones were preferred above
          // return the first found ...
          selectedSTAR = selGroups.First( ).Key;
          refProc = selGroups.First( ).FirstOrDefault( ); // use the first of the most counted group, mostly the best..
          foreach (var wyp in WaypointCat) {
            // if a wyp appears in the selected proc - tag it
            if (refProc.HasFixIdent( wyp.ICAO )) {
              var fix = refProc.FixWithIdent( wyp.ICAO );
              if (fix != null) {
                wyp.AltLo_ft = fix.AltitudeLo_ft;
                wyp.AltHi_ft = fix.AltitudeHi_ft;
                wyp.SpeedLimit_kt = fix.SpeedLimit_kt;
              }
              // Add proc if the Wyp is part of the procedure
              wyp.STAR_Ident = selectedSTAR; // placeholder
            }
          }
        }
      }
      // APROACH
      if (arrAirport != null) {
        // walk all Wyps and find Proc in Runway Procs
        foreach (var wyp in WaypointCat) {
          collectedApr.AddRange(
             arrRwy?.APRs.Where( prc => prc.HasFixIdent( wyp.Ident ) && (prc.RunwayIdent == ArrivalRw.Ident || prc.RunwayIdent == RwALLIdent) )
          );
        }
        if (collectedApr.Count == 0) {
          // try airport Procs if not found in runways
          foreach (var wyp in WaypointCat) {
            var proc = arrAirport.APRs( ).FirstOrDefault( prc => prc.RunwayIdent == ArrivalRw.Ident || prc.RunwayIdent == RwALLIdent );
            if (proc != null) collectedApr.Add( proc );
          }
        }
        if (collectedApr.Count > 0) {
          // List must be non empty... take all groups which have the same count as the one with the most counts
          var groups = collectedApr.GroupBy( i => i.ProcRef ).OrderByDescending( grp => grp.Count( ) );
          var selGroups = groups.Where( grp => grp.Count( ) == groups.First( ).Count( ) ).OrderBy( c => c.First( ).NavType ); //take all having max count
          // having multiples with the same count ... RW ones were preferred above
          // return the first found ...
          selectedAPR = selGroups.First( ).Key;
          IProcedure refProc = collectedApr.Where( apr => apr.ProcRef == selectedAPR ).OrderBy( proc => proc.NavType ).FirstOrDefault( ); // use first (ordered by NavType
          int seqNo = 1;
          foreach (var wyp in WaypointCat) {
            // if a wyp appears in the selected proc - tag it
            if (refProc.HasFixIdent( wyp.ICAO )) {
              var fix = refProc.FixWithIdent( wyp.ICAO );
              if (fix != null) {
                wyp.AltLo_ft = fix.AltitudeLo_ft;
                wyp.AltHi_ft = fix.AltitudeHi_ft;
                wyp.SpeedLimit_kt = fix.SpeedLimit_kt;
              }
              // Add proc if the Wyp is part of the procedure
              wyp.ApproachProcRef = selectedAPR;
              wyp.ApproachSequ = seqNo++; // must sequence Apt waypoints
            }
          }
        }
      }

      _procDone = true;
    }


    // Decode the Description line if possible
    // seems to stay in English independent of the GUI language
    // Ex.
    // <desc>Galeao-Antonio C Jobim Intl (SBGL) to Congonhas Intl (SBSP) at 12000 ft, IFR</desc>
    // <desc>Tokyo (Haneda) Intl (RJTT) to Kansai Intl (RJBB) at 24000 ft, IFR</desc>

    Regex _rxDesc = new Regex(
     @"(?<dApN>.*)([\(](?<dApI>[A-Z0-9]{3,4})[\)])\s*to\s*(?<aApN>.*)([\(](?<aApI>[A-Z0-9]{2,4})[\)])\s*at\s*(?<cAlt>\d{1,5})\s*(?<uAlt>\w+)[,\s]*(?<typ>\w+)"
        , RegexOptions.Compiled | RegexOptions.Singleline );

    private void DecodeDescription( )
    {
      Match match = _rxDesc.Match( Description );
      if (match.Success) {
        // Departure Apt
        string item = match.Groups["dApI"].Value;
        if (_depApt.IsValid && _depApt.ICAO == item) _depApt.Name = match.Groups["dApN"].Value;
        // Destination Apt
        item = match.Groups["aApI"].Value;
        if (_arrApt.IsValid && _arrApt.ICAO == item) _arrApt.Name = match.Groups["aApN"].Value;
        // Cruise Alt
        item = match.Groups["cAlt"].Value;
        if (float.TryParse( item, out float value )) {
          // Cruise Alt Unit
          item = match.Groups["uAlt"].Value;
          if (item.Trim( ).ToLowerInvariant( ) == "ft") {
            _cruiseAlt_ft = value;
          }
          else if (item.Trim( ).ToLowerInvariant( ) == "m") {
            _cruiseAlt_ft = (float)dNetBm98.Units.Ft_From_M( value );
          }
        }
        // Plan Type
        item = match.Groups["typ"].Value;
        if (item == "IFR") _fpType = TypeOfFlightplan.IFR;
        else if (item == "VFR") _fpType = TypeOfFlightplan.VFR;
      }
      else {
        // at least try the Type
        if (Description.ToUpperInvariant( ).EndsWith( "IFR" )) _fpType = TypeOfFlightplan.IFR;
        else if (Description.ToUpperInvariant( ).EndsWith( "VFR" )) _fpType = TypeOfFlightplan.VFR;
      }

    }

    #endregion

  }
}
