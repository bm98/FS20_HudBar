using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;

using FSFData;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib
{
  /// <summary>
  /// The Source of a Plan
  /// </summary>
  public enum SourceOfFlightPlan
  {
    /// <summary>
    /// Source is not defined
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// From SimBrief
    /// </summary>
    SimBrief,
    /// <summary>
    /// From an MSFS GPX File
    /// </summary>
    MS_Pln,
    /// <summary>
    /// From an MSFS FLT File
    /// </summary>
    MS_Flt,
    /// <summary>
    /// From an LNM GPX exported File
    /// </summary>
    LNM_Gpx,
    /// <summary>
    /// From an LNM Plam exported File
    /// </summary>
    LNM_Pln,
    /// <summary>
    /// From an Generic Rte string
    /// </summary>
    GEN_Rte,
  }

  /// <summary>
  /// Type of Flightplan
  /// </summary>
  public enum TypeOfFlightplan
  {
    /// <summary>
    /// VFR plan
    /// </summary>
    VFR,
    /// <summary>
    /// IFR plan
    /// </summary>
    IFR,
    /// <summary>
    /// VOR to VOR plan
    /// </summary>
    VOR
  }

  /// <summary>
  /// Type of IFR route
  /// </summary>
  public enum TypeOfRoute
  {
    /// <summary>
    /// High Altitude IFR route
    /// </summary>
    LowAlt,
    /// <summary>
    /// Low Altitude IFR route
    /// </summary>
    HighAlt,
    /// <summary>
    /// VFR route
    /// </summary>
    VFR,
    /// <summary>
    /// VOR route
    /// </summary>
    VOR,
  }


  /// <summary>
  /// Generic Flightplan
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// RULES:..
  ///   Waypoints:
  ///     AltLimits (Lo/Hi) are decoded if >0
  ///     Coord.Altitude is assumed as Target Altitude e.g. enroute Alts
  ///     Procedure Waypoints carry their ProdedureRef (Ident for SID,STAR; NavTyp Suffix for Approaches)
  ///     Airway Waypoints carry the AirwayIdent of this Waypoint, intersections are not marked
  ///     
  /// 
  ///   First WYP - Departure Airport if there is one, else a starting Waypoint
  ///   Optional: add a Runway WYP with StartCoord to help the drwawing
  ///   
  ///   SID:  Add all Waypoints with Alt Limits
  ///   STAR: Add all Waypoints with Alt Limits
  ///   Approaches: Add all Waypoints with Alt Limits
  ///   Approach Waypoints need to be numbered (1...) in order to hide some in the map
  ///   
  ///   Last WYP - Arrival Airport if there is one
  /// </summary>
  public class FlightPlan
  {
    /// <summary>
    /// True if the plan is Valid
    /// Must have an Origin, Destination and at least one Waypoint
    /// </summary>
    public bool IsValid => (Origin != null) && (Destination != null) && (Waypoints.Count > 0);
    /// <summary>
    /// Filename or other source of this plan
    /// </summary>
    public string FlightPlanFile { get; internal set; } = "";
    /// <summary>
    /// The Type of Flightplan
    /// </summary>
    public TypeOfFlightplan FlightPlanType { get; internal set; } = TypeOfFlightplan.IFR;

    /// <summary>
    /// The Type of the Route
    /// </summary>
    public TypeOfRoute RouteType { get; internal set; } = TypeOfRoute.LowAlt;

    /// <summary>
    /// The Source of this plan
    /// </summary>
    public SourceOfFlightPlan Source { get; internal set; } = SourceOfFlightPlan.Undefined;
    /// <summary>
    /// The Title of this Plan
    /// </summary>
    public string Title { get; internal set; } = "";

    /// <summary>
    /// The Origin of the plan
    /// </summary>
    public Location Origin { get; internal set; } = new Location( );

    /// <summary>
    /// The Destination of the plan
    /// </summary>
    public Location Destination { get; internal set; } = new Location( );

    /// <summary>
    /// The Alternate Destination of the plan
    /// </summary>
    public Location Alternate { get; internal set; } = new Location( );

    /// <summary>
    /// WaypointCat of this Plan
    /// to be consistent across plans - add the origin as Waypoint at the beginning
    /// </summary>
    public List<Waypoint> Waypoints { get; internal set; } = new List<Waypoint>( );

    /// <summary>
    /// Optional: Get: The SID Proc Ident (PROC[.TRANSITION] GBAS3, FDESO3.KULSI)
    /// Default: empty
    /// </summary>
    public string SIDProcName => (Origin != null) ? Origin.SIDProcName : "";
    /// <summary>
    /// Optional: Get: The STAR Proc Ident (PROC[.TRANSITION] GBAS3, FDESO3.KULSI)
    /// Default: empty
    /// </summary>
    public string STARProcName => (Destination != null) ? Destination.STARProcName : "";

    /// <summary>
    /// Optional: Get: The Approach Nav Procedure used (ILS, RNAV,...)
    /// Default: None
    /// </summary>
    public string ApproachProc => (Destination != null) ? Destination.ApproachProc : "";
    /// <summary>
    /// Optional: Get: The Approach Nav Procedure Suffix (Y,Z..) a single character
    /// Default: empty
    /// </summary>
    public String ApproachSuffix => (Destination != null) ? Destination.ApproachSuffix : "";
    /// <summary>
    /// Optional: Get: The Approach ProcRef (ILS Z, VOR..)
    /// </summary>
    public string ApproachProcRef => (Destination != null) ? Destination.ApproachProcRef : "";
    /// <summary>
    /// Optional:
    /// A step profile if provided by the plan
    /// </summary>
    public string StepProfile { get; internal set; } = "";

    /// <summary>
    /// The (initial) Cruising Altitude ft
    /// </summary>
    public double CruisingAlt_ft { get; internal set; } = 0;

    /// <summary>
    /// Added Distances of the legs
    /// </summary>
    public double Distance_Total_nm { get; internal set; } = 0;
    /// <summary>
    /// Optional:
    /// A HTML document if provided for this Plan
    /// </summary>
    public string HTMLdocument { get; internal set; } = "";
    /// <summary>
    /// Links for Documents to retrieve via Plan
    /// </summary>
    public List<FileLink> DocLinks { get; internal set; } = new List<FileLink>( );
    /// <summary>
    /// Links for Images to retrieve via Plan
    /// </summary>
    public List<FileLink> ImageLinks { get; internal set; } = new List<FileLink>( );

    /// <summary>
    /// True if the plan has a SID
    /// </summary>
    public bool HasSID => !string.IsNullOrEmpty( SIDProcName );
    /// <summary>
    /// True if the plan has a STAR
    /// </summary>
    public bool HasSTAR => !string.IsNullOrEmpty( STARProcName );
    /// <summary>
    /// True if the plan has an Approach
    /// </summary>
    public bool HasApproach => !string.IsNullOrEmpty( ApproachProcRef );



    // Tools

    /// <summary>
    /// Does all shared Post Processing on a FlightPlan
    /// </summary>
    internal void PostProcess( )
    {
      EvalStepping( );
      CalculateInboundTrack( );
      CalcOutboundTrack( );
      LoadNavFrequencies( );
    }

    /// <summary>
    /// Fill Frequ String if the NAV is found
    /// </summary>
    private void LoadNavFrequencies( )
    {
      foreach (var item in Waypoints) {
        if (item.WaypointType == WaypointTyp.VOR) {
          var nv = DbLookup.NavaidList_ByAreaQuad( item.Ident, item.LatLonAlt_ft.AsQuad( 6 ), bm98_hbFolders.Folders.GenAptDBFile )
                            .Where( nav => nav.IsVOR ).FirstOrDefault( );
          if (nv != null) {
            item.Frequency = FrequencyS( nv.Frequ_Hz );
          }
        }
        if (item.WaypointType == WaypointTyp.NDB) {
          var nv = DbLookup.NavaidList_ByAreaQuad( item.Ident, item.LatLonAlt_ft.AsQuad( 6 ), bm98_hbFolders.Folders.GenAptDBFile )
                            .Where( nav => nav.IsNDB ).FirstOrDefault( );
          if (nv != null) {
            item.Frequency = FrequencyS( nv.Frequ_Hz );
          }
        }
      }
    }


    /// <summary>
    /// fill the in/outbound items which are not provided in the plan while updating the list
    /// </summary>
    private void EvalStepping( )
    {
      // sanity
      if (Origin == null) return;
      // this is kind of best guessing....

      // eval the Stepping profile
      string prevIcao = "";
      double stepAlt = CruisingAlt_ft;
      string steps = $"{Origin.Icao_Ident}/{stepAlt / 100:0000}";
      foreach (var item in Waypoints) {
        // fix some properties...
        if (item.WaypointType == WaypointTyp.APT) {
          // Limits must match the Airport Altitude AT
          item.AltitudeLo_ft = (int)item.LatLonAlt_ft.Altitude;
          item.AltitudeHi_ft = (int)item.LatLonAlt_ft.Altitude;
        }
        if (item.WaypointType == WaypointTyp.RWY) {
          // Limits must match the Runway Altitude AT
          item.AltitudeLo_ft = (int)item.LatLonAlt_ft.Altitude;
          item.AltitudeHi_ft = (int)item.LatLonAlt_ft.Altitude;
        }

        // try to eval the stepping profile
        if (item.LatLonAlt_ft.Altitude > stepAlt) {
          // next alt
          stepAlt = item.LatLonAlt_ft.Altitude;
          steps += $"/{prevIcao}/{stepAlt / 100:0000}";
        }
        // prep next
        prevIcao = item.Icao_Ident.ICAO;
      }// all waypoints
      // set steps if not provided
      if (string.IsNullOrEmpty( StepProfile )) { StepProfile = steps; }
    }


    /// <summary>
    /// calculate inbound track and distances if not set
    /// </summary>
    private void CalculateInboundTrack( )
    {

      // calculate inbound track and distances if not set and 
      var prevLatLon = new LatLon( );
      double total_dist = 0;

      if (Waypoints.Count > 0) {
        prevLatLon = Waypoints.First( ).LatLonAlt_ft;
        Waypoints.First( ).InboundTrueTrk = 0; // first has no inbound track
        Waypoints.First( ).Distance_nm = 0; // first has no inbound distance
      }
      // ib tracks have the prev points direction to this pt value as IB
      for (int i = 1; i < Waypoints.Count; i++) {
        Waypoint pt = Waypoints[i];
        // Calc Distance
        if (!pt.LatLonAlt_ft.IsEmpty) {
          if (pt.InboundTrueTrk < 0) {
            // inbound track is not set
            // from prev to this 
            pt.InboundTrueTrk = (int)prevLatLon.BearingTo( pt.LatLonAlt_ft );

          }
          if (pt.Distance_nm < 0) {
            // inbound Distance is not set
            // from prev to this 
            pt.Distance_nm = prevLatLon.DistanceTo( pt.LatLonAlt_ft, ConvConsts.EarthRadiusNm );
          }
          // don't add MAPR/HOLD but RUNWAY (is MAPR)
          bool addDist = !((pt.WaypointUsage == UsageTyp.MAPR) || (pt.WaypointUsage == UsageTyp.HOLD))
                          || pt.WaypointType == WaypointTyp.RWY;
          total_dist += addDist ? pt.Distance_nm : 0;
          prevLatLon = pt.LatLonAlt_ft;
        }
        else {
          // wyp has no coords ...
          pt.InboundTrueTrk = 0;
          pt.Distance_nm = 0;
        }
      }
      // setting the sum for all legs
      Distance_Total_nm = total_dist;
    }


    /// <summary>
    /// Calculate outbound track if not set
    /// </summary>
    private void CalcOutboundTrack( )
    {
      for (int i = 0; i < Waypoints.Count - 1; i++) {
        Waypoint pt = Waypoints[i];
        Waypoint pt1 = Waypoints[i + 1];

        if (pt.OutboundTrueTrk < 0) {
          // outbound track is not set
          pt.OutboundTrueTrk = pt1.InboundTrueTrk;
        }
      }
      // fix the last one (omitted above)
      if (Waypoints.Count > 1) {
        Waypoint pt = Waypoints[Waypoints.Count - 1];
        if (pt.OutboundTrueTrk < 0) {
          pt.OutboundTrueTrk = 0;
        }
      }
    }
  }
}
