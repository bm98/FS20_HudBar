using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

using FlightplanLib.Flightplan;

using bm98_hbFolders;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// An MSFS GPX FlightPlan Element
  /// </summary>
  [XmlRoot( "FlightPlan.FlightPlan", Namespace = "", IsNullable = false )]
  public class X_FlightPlan
  {
    // Elements
    /// <summary>
    /// The Title element
    /// </summary>
    [XmlElement( ElementName = "Title" )]
    public string Title { get; set; } = "";

    /// <summary>
    /// The Descr element
    /// </summary>
    [XmlElement( ElementName = "Descr" )]
    public string Description { get; set; } = ""; // description 

    /// <summary>
    /// The FPType element
    /// </summary>
    [XmlElement( ElementName = "FPType" )]
    public string PlanType_S { get; set; } = ""; // IFR, VFR, VOR ??

    /// <summary>
    /// The RouteType element
    /// </summary>
    [XmlElement( ElementName = "RouteType" )]
    public string RouteType_S { get; set; } = ""; // HighAlt, LowAlt, VFR, VOR ??

    /// <summary>
    /// The CruisingAlt element
    /// </summary>
    [XmlElement( ElementName = "CruisingAlt" )]
    public string CruisingAlt_S { get; set; } = ""; // a number


    /// <summary>
    /// The DepartureID element
    /// </summary>
    [XmlElement( ElementName = "DepartureID" )]
    public string DepartureICAO { get; set; } = ""; // an ICAO airport code

    /// <summary>
    /// The DepartureLLA element
    /// </summary>
    [XmlElement( ElementName = "DepartureLLA" )]
    public string DepartureLLA { get; set; } = ""; // an LLA

    /// <summary>
    /// The DepartureName element
    /// </summary>
    [XmlElement( ElementName = "DepartureName" )]
    public string DepartureName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// The DeparturePosition element
    /// </summary>
    [XmlElement( ElementName = "DeparturePosition" )]
    public string DeparturePosition { get; set; } = ""; // runway or parking spot e.g. 11, N PARKING 2, 


    /// <summary>
    /// The DestinationID element
    /// </summary>
    [XmlElement( ElementName = "DestinationID" )]
    public string ArrivalCAO { get; set; } = ""; // an ICAO airport code

    /// <summary>
    /// The DestinationLLA element
    /// </summary>
    [XmlElement( ElementName = "DestinationLLA" )]
    public string ArrivalLLA { get; set; } = ""; // an LLA

    /// <summary>
    /// The DestinationName element
    /// </summary>
    [XmlElement( ElementName = "DestinationName" )]
    public string ArrivalName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// Catalog of WaypointCat
    /// </summary>
    [XmlElement( ElementName = "ATCWaypoint" )]
    public List<X_ATCWaypoint> WaypointCat { get; set; } = new List<X_ATCWaypoint>( );


    /// <summary>
    /// The AppVersion element
    /// </summary>
    [XmlElement( ElementName = "AppVersion" )]
    public X_AppVersion AppVersion { get; set; } = new X_AppVersion( );


    // Non XML

    /// <summary>
    /// Departure LatLonAlt (derived field)
    /// </summary>
    [XmlIgnore]
    public LatLon DEP_LatLon => Formatter.ToLatLon( DepartureLLA );
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
    /// Departure Runway Ident if found
    /// </summary>
    [XmlIgnore]
    public string DEP_RwIdent { get; set; } = "";

    /// <summary>
    /// SID Ident if found
    /// </summary>
    [XmlIgnore]
    public string DEP_SID_Ident { get; set; } = "";

    /// <summary>
    /// Destination LatLonAlt (derived field)
    /// </summary>
    [XmlIgnore]
    public LatLon DST_LatLon => Formatter.ToLatLon( ArrivalLLA );
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
    /// Destination Runway Ident if found
    /// </summary>
    [XmlIgnore]
    public string DST_RwIdent { get; set; } = "";

    /// <summary>
    /// Cruising Altitude ft number (derived field)
    /// </summary>
    [XmlIgnore]
    public float CruisingAlt_ft => (float)Formatter.GetValue( CruisingAlt_S );
    /// <summary>
    /// STAR Ident if found
    /// </summary>
    [XmlIgnore]
    public string DST_STAR_Ident { get; set; } = "";
    /// <summary>
    /// APR ProcRef if found
    /// </summary>
    [XmlIgnore]
    public string DST_APR_ProcRef { get; set; } = "";
    /// <summary>
    /// Flight plan type as enum (derived field)
    /// </summary>
    [XmlIgnore]
    public TypeOfFlightplan FlightPlanType => ToTypeOfFP( PlanType_S );
    /// <summary>
    /// Route type as enum (derived field)
    /// </summary>
    [XmlIgnore]
    public TypeOfRoute RouteType => ToTypeOfRoute( RouteType_S );


    private static TypeOfFlightplan ToTypeOfFP( string fpType )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "IFR": return TypeOfFlightplan.IFR;
        case "VFR": return TypeOfFlightplan.VFR;
        case "VOR": return TypeOfFlightplan.VOR;
        default: return TypeOfFlightplan.VFR;
      }
    }

    private static TypeOfRoute ToTypeOfRoute( string rtType )
    {
      switch (rtType.ToUpperInvariant( )) {
        case "LOWALT": return TypeOfRoute.LowAlt;
        case "HIGHALT": return TypeOfRoute.HighAlt;
        case "VOR": return TypeOfRoute.VOR;
        case "VFR": return TypeOfRoute.VFR;
        default: return TypeOfRoute.VFR;
      }
    }

    #region Post Processing

    private bool _ppDone = false;
    private bool _procDone = false;

    // Post Processing to eval the derived values in one go
    internal void PostProc( )
    {
      X_ATCWaypoint _depApt = null;
      string _depRwyIdent = null;
      X_ATCWaypoint _arrApt = null;
      string _arrRwyIdent = null;


      if (_ppDone) return;

      // Assuming they arrive in an order
      foreach (var wyp in WaypointCat) {
        if (!wyp.IsValid) continue; // ??
        if (!(wyp.WaypointType == WaypointTyp.APT || wyp.WaypointType == WaypointTyp.RWY)) continue;

        if ((_depApt == null) && (wyp.WaypointType == WaypointTyp.APT)) {
          // need Departure Airport
          _depApt = wyp;
        }

        else if ((_depRwyIdent == null) && (wyp.WaypointType == WaypointTyp.RWY)) {
          // need Departure Runway
          // RWY close enough?? <8 nm from Apt ??
          if (wyp.LatLonElev_ft.DistanceTo( _depApt.LatLonElev_ft, ConvConsts.EarthRadiusNm ) < 8) {
            _depRwyIdent = wyp.RunwayIdent;
          }
          else {
            // could already be the Arrival Runway then
            _depRwyIdent = null;
            _arrRwyIdent = wyp.RunwayIdent; // add as Arr
          }
        }

        /* Take the Arrival Runway always from the Airport - there are too many Dept RW records... and usually not an Arrival one anyway
        else if ((_arrRwyIdent == null) && (wyp.WaypointType == WaypointTyp.RWY)) {
          // need Arrival Runway
          if (wyp.WaypointType == WaypointTyp.RWY) {
            _arrRwyIdent = wyp.RunwayIdent;
          }
        }
        */
        else if ((_arrApt == null) && (wyp.WaypointType == WaypointTyp.APT)) {
          // need Arrival Airport
          if (wyp.WaypointType == WaypointTyp.APT) {
            _arrApt = wyp;
          }
        }
      }
      // clean the ones not found
      if (_depApt == null) _depApt = new X_ATCWaypoint( );
      if (_depRwyIdent == null) _depRwyIdent = RwALLIdent;
      if (_arrApt == null) _arrApt = new X_ATCWaypoint( );
      // some PLNs have no Arrival Runway but we may create the information
      if (_arrRwyIdent == null) _arrRwyIdent = _arrApt.RunwayIdent;

      DEP_RwIdent = _depRwyIdent;
      DST_RwIdent = _arrRwyIdent;

      _ppDone = true;
    }

    // attempts to derive the procedure names and fix info from the given waypoints
    internal void InsertProcedures( )
    {
      if (_procDone) return;

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
          depAirport = _db.DbReader.GetAirport( DepartureICAO );
          arrAirport = _db.DbReader.GetAirport( ArrivalCAO );
        }
      }

      // Runways
      var depRwy = depAirport?.Runways.FirstOrDefault( rwy => rwy.Ident == DEP_RwIdent );
      var arrRwy = arrAirport?.Runways.FirstOrDefault( rwy => rwy.Ident == DST_RwIdent );

      // SID WYPs if there are any
      if (depAirport != null) {
        // Check Wyps for a SID 
        var wy = WaypointCat.FirstOrDefault( wyp => wyp.IsSID );
        if (wy != null) { selectedSID = wy.SID_Ident; }

        IProcedure refProc = depRwy?.SIDs.FirstOrDefault( proc => proc.Ident == selectedSID )
                          ?? depAirport?.SIDs( ).FirstOrDefault( sid => sid.Ident == selectedSID );
        if (refProc != null) {
          DEP_SID_Ident = selectedSID;
          foreach (var wyp in WaypointCat.Where( wyp => wyp.IsSID )) {
            // if a wyp appears in the selected proc - tag it
            if (refProc.HasFixIdent( wyp.Wyp_Ident )) {
              var fix = refProc.FixWithIdent( wyp.Wyp_Ident );
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
      }
      // STAR WYPs if there are any
      if (arrAirport != null) {
        // Check Wyps for a STAR 
        var wy = WaypointCat.FirstOrDefault( wyp => wyp.IsSTAR );
        if (wy != null) { selectedSTAR = wy.STAR_Ident; }

        IProcedure refProc = arrRwy?.STARs.FirstOrDefault( proc => proc.Ident == selectedSTAR )
                          ?? arrAirport?.STARs( ).FirstOrDefault( star => star.Ident == selectedSTAR );
        if (refProc != null) {
          DST_STAR_Ident = selectedSTAR;
          foreach (var wyp in WaypointCat.Where(wyp=>wyp.IsSTAR)) {
            // if a wyp appears in the selected proc - tag it
            if (refProc.HasFixIdent( wyp.Wyp_Ident )) {
              var fix = refProc.FixWithIdent( wyp.Wyp_Ident );
              if (fix != null) {
                wyp.AltLo_ft = fix.AltitudeLo_ft;
                wyp.AltHi_ft = fix.AltitudeHi_ft;
                wyp.SpeedLimit_kt = fix.SpeedLimit_kt;
              }
              // Add proc if the Wyp is part of the procedure
              wyp.STAR_Ident = selectedSTAR; // placeholder
              wyp.RunwayNumber_S = DST_RwIdent.RwNumberOf( );
              wyp.RunwayDesignation = DST_RwIdent.RwDesignationOf( );
            }
          }
        }
      }
      // APROACH WYPs if there are any
      if (arrAirport != null) {
        // Check Wyps for a APR 
        var wy = WaypointCat.FirstOrDefault( wyp => wyp.IsAPR );
        if (wy != null) { selectedAPR = AsProcRef( wy.ApproachTypeS, wy.ApproachSuffix ); }

        IProcedure refProc = arrRwy?.APRs.FirstOrDefault( proc => proc.ProcRef == selectedAPR )
                          ?? arrAirport.APRs( ).FirstOrDefault( apr => apr.ProcRef == selectedAPR );
        if (refProc != null) {
          DST_APR_ProcRef = selectedAPR;
          int seqNo = 1;
          foreach (var wyp in WaypointCat.Where( wyp => wyp.IsAPR )) {
            // if a wyp appears in the selected proc - tag it
            if (refProc.HasFixIdent( wyp.Wyp_Ident )) {
              var fix = refProc.FixWithIdent( wyp.Wyp_Ident );
              if (fix != null) {
                wyp.AltLo_ft = fix.AltitudeLo_ft;
                wyp.AltHi_ft = fix.AltitudeHi_ft;
                wyp.SpeedLimit_kt = fix.SpeedLimit_kt;
              }
              // Add proc if the Wyp is part of the procedure
              wyp.ApproachProcRef = selectedAPR;
              wyp.ApproachSequ = seqNo++; // must sequence Apt waypoints
              wyp.RunwayNumber_S = DST_RwIdent.RwNumberOf( );
              wyp.RunwayDesignation = DST_RwIdent.RwDesignationOf( );
            }
          }
        }
      }

      _procDone = true;
    }

    #endregion

  }
}
