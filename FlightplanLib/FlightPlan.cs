using CoordLib;
using FlightplanLib.MSFSPln.PLNDEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// From an MSFS PLN File
    /// </summary>
    MS_Pln,
    /// <summary>
    /// From an MSFS FLT File
    /// </summary>
    MS_Flt,
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
  /// </summary>
  public class FlightPlan
  {
    /// <summary>
    /// True if the Plan is Valid
    /// </summary>
    public bool IsValid => Waypoints.Count > 0;

    /// <summary>
    /// The Source of this Plan
    /// </summary>
    public SourceOfFlightPlan Source { get; internal set; } = SourceOfFlightPlan.Undefined;
    /// <summary>
    /// The Title of this Plan
    /// </summary>
    public string Title { get; internal set; } = "";

    /// <summary>
    /// The Origin of the flightplan
    /// </summary>
    public Location Origin { get; internal set; } = new Location( );

    /// <summary>
    /// The Destination of the flightplan
    /// </summary>
    public Location Destination { get; internal set; } = new Location( );

    /// <summary>
    /// Waypoints of this Plan
    /// </summary>
    public List<Waypoint> Waypoints { get; internal set; } = new List<Waypoint>( );

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
    /// The Type of Flightplan
    /// </summary>
    public TypeOfFlightplan FlightPlanType { get; internal set; } = TypeOfFlightplan.IFR;

    /// <summary>
    /// The Type of the Route
    /// </summary>
    public TypeOfRoute RouteType { get; internal set; } = TypeOfRoute.LowAlt;

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


    // Tools


    /// <summary>
    /// fill the in/outbound items which are not provided in the plan while updating the list
    /// </summary>
    internal void RecalcWaypoints( )
    {
      // this is kind of best guessing....

      string prevIcao = "";
      double stepAlt = CruisingAlt_ft;
      string steps = $"{Origin.Icao_Ident}/{stepAlt / 100:0000}";
      // for all, find SID,STAR based on state
      // assuming the SID_Ident and STAR_Ident are set to "-1" and the Airway_Ident is set
      bool forSID = true; // flag looking for SID now
      string sid = "";
      bool forSTAR = false; // flag looking for STAR now
      string star = "";
      foreach (var item in Waypoints) {
        if (forSID) {
          // looking for SID
          if (item.IsSIDorSTAR) {
            // in SID
            sid = item.Airway_Ident; // it is in Airway Ident
            if (item.SID_Ident == "-1") { item.SID_Ident = sid; }// if not set
            //item.Airway_Ident = ""; // cleanup
          }
          else {
            // if not yet or no longer on SID
            forSID = string.IsNullOrEmpty( sid ) && (item.WaypointType == TypeOfWaypoint.Airport); // not yet : no longer
            forSTAR = !forSID; // toggle if no longer
          }
        }
        else if (forSTAR) {
          // looking for STAR
          if (item.IsSIDorSTAR) {
            // in STAR
            star = item.Airway_Ident; // it is in Airway Ident
            if (item.STAR_Ident == "-1") { item.STAR_Ident = star; } // if not set
            //item.Airway_Ident = "";
          }
          else {
            // if not yet or no longer on STAR
            forSTAR = string.IsNullOrEmpty( star ); // not yet : no longer
          }
        }
        else {
          // done
        }
        // clean up if not set
        if (item.SID_Ident == "-1") { item.SID_Ident = ""; }
        if (item.STAR_Ident == "-1") { item.STAR_Ident = ""; }

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

        total_dist += pt.Distance_nm;
        prevLatLon = pt.LatLonAlt_ft;
      }
      // setting the sum for all legs
      Distance_Total_nm = total_dist;

      // calculate outbound track if not set
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

    /// <summary>
    /// Combines the Runway Ident
    /// </summary>
    /// <param name="rNum"></param>
    /// <param name="rDes"></param>
    /// <returns></returns>
    internal static string ToRunwayID( string rNum, string rDes )
    {
      if (string.IsNullOrWhiteSpace( rDes )) {
        return rNum;
      }
      else {
        return rNum + rDes.Substring( 0, 1 ); // convert from 11 LEFT to 11L
      }
    }

  }
}
