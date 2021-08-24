using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using CoordLib;

namespace FS20_HudBar.Bar.FltLib
{
  /// <summary>
  /// The ATC Clearance
  /// </summary>
  public enum ATC_Clearance
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    Own_Navigation,
    Vectors_Icpt_Left,
    Vectors_Icpt_Right,
    Vectors_Route,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// The ATC assigned (assumed...) Landing Sequence
  /// </summary>
  public enum ATC_Landing
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    IFR_Expecting_Approach,
    IFR_Cleared_Approach,
    IFR_Cleared_To_Land,

    VFR_Landing_Request,
    VFR_Landing_Pattern,
    VFR_Cleared_To_Land,

    VFR_TG_Request,
    VFR_TG_Pattern,
    VFR_TG_Cleared_To_Land,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// A trivial version of a flight plan in MSFS
  /// </summary>
  public class FlightPlan
  {
    /// <summary>
    /// True if the FLT file reports an active Flightplan
    /// </summary>
    public bool HasFlightPlan { get; private set; } = false;

    /// <summary>
    /// The Departure airport ICAO code
    /// </summary>
    public string Departure { get; private set; } = "";
    /// <summary>
    /// The Destination airport ICAO code
    /// </summary>
    public string Destination { get; private set; } = "";
    /// <summary>
    /// The Waypoints found for this Flight
    /// </summary>
    public Waypoints Waypoints { get; private set; } = new Waypoints( );

    /// <summary>
    /// The next Waypoint Number from the FLT file
    /// Defaults to the second WYP i.e. the first Next
    /// </summary>
    public int NextWyp { get; private set; } = 1;

    /// <summary>
    /// The next Waypoint Name from the FLT file (or empty if not found)
    /// </summary>
    public string NextWypIdent =>
      ( NextWyp < Waypoints.Count ) ? Waypoints[NextWyp].Ident : "";

    /// <summary>
    /// The ATC cleared altitude
    /// </summary>
    public float AssignedAlt { get; private set; } = float.NaN;
    /// <summary>
    /// The ATC assigned heading
    /// </summary>
    public float AssignedHdg { get; private set; } = float.NaN;

    /// <summary>
    /// Current ATC Clearance
    /// </summary>
    public ATC_Clearance Clearance { get; private set; } = ATC_Clearance.None;
    /// <summary>
    /// Current ATC assigned/assumed Landing Sequence
    /// </summary>
    public ATC_Landing LandingSequ { get; private set; } = ATC_Landing.None;

    /// <summary>
    /// Hash of the Pretty String to decide if something has changed in the Plan
    /// </summary>
    public int Hash { get; private set; } = 0;


    /// <summary>
    /// Returns a CRLF separated string containing the full stored FlighPlan
    /// or an empty string if there is no FP
    /// </summary>
    public string Pretty {
      get {
        if ( !HasFlightPlan ) return "No active Flightplan";

        string ret =  PrettyLeader;
        ret += "Waypoints:\n";
        for ( int i = 0; i < Waypoints.Count; i++ ) {
          ret += $"{Waypoints[i].Pretty}\n";
        }
        return ret;
      }
    }

    // The Leader part or empty if no FP is available
    private string PrettyLeader {
      get {
        if ( !HasFlightPlan ) return "";

        string ret = $"Flightplan: {Departure}-{Destination}\n";
        ret += !string.IsNullOrEmpty( NextWypIdent ) ? $" ATC Next Wyp : {NextWypIdent}\n" : "";
        ret += ( !float.IsNaN( AssignedAlt ) ) ? $" ATC Altitude : {AssignedAlt:##,##0} ft\n" : "";
        ret += ( !float.IsNaN( AssignedHdg ) ) ? $" ATC Heading  : {AssignedHdg:000}°\n" : "";
        ret += ( Clearance != ATC_Clearance.None ) ? $" ATC Clearance: {Clearance.ToString( ).Replace( "_", " " )}\n" : "";
        ret += ( LandingSequ != ATC_Landing.None ) ? $" ATC Landing  : {LandingSequ.ToString( ).Replace( "_", " " )}\n" : "";
        return ret;
      }
    }


    // Find the Index of the WYP given
    //  if the name is not found we use the reported one from the last FLT file
    private int EvalNextWypIndex( string nextWp )
    {
      if ( !HasFlightPlan ) return 0;

      int next = Waypoints.FindIndex (x=> x.Ident==nextWp);
      if ( next < 0 ) {
        // cannot find the WYP by name - obscure FLT naming by MSFS e.g. (POI1 is POI in the GPS ???)
        next = NextWyp; // take the NextWyp reported in the last FLT file
      }
      return next;
    }

    /// <summary>
    /// Calculate the remaining distance given the next WP and the distance to it
    /// (if the next WP is not known, the Destination is assumed)
    /// </summary>
    /// <param name="nextWp">The next WP</param>
    /// <param name="dToWp">Distance to the WP</param>
    /// <returns>Remaining distance in nm</returns>
    public float RemainingDist_nm( string nextWp, float dToWp )
    {
      if ( !HasFlightPlan ) return 0;

      int next = EvalNextWypIndex(nextWp);
      if ( next < Waypoints.Count ) {
        return Waypoints[next].RemainingDist_nm + dToWp;
      }
      else {
        return dToWp; // just return the remaining one submitted (we don't know anything else here)
      }
    }


    /// <summary>
    /// Returns the remaining FlightPlan as CRLF separated line of text
    /// </summary>
    /// <param name="nextWp"></param>
    /// <returns></returns>
    public string RemainingPlan( string nextWp )
    {
      if ( !HasFlightPlan ) return "";

      int next = EvalNextWypIndex(nextWp);
      var ret = PrettyLeader;
      ret += "Remaining Waypoints:\n"; 
      for ( int i = next; i < Waypoints.Count; i++ ) {
        ret += $"{Waypoints[i].Pretty}\n";
      }

      return ret;
    }


    /// <summary>
    /// Returns a waypoint given an Name
    /// Note: GPS Waypoint names and the ones from FLT file sometimes don't match (either GPS or other issues in the Sim)
    ///       Seen in Missions wher the GPS names POI and the real ones are POI1..POIn
    /// </summary>
    /// <param name="wypName">A Waypoint name</param>
    /// <returns>The Waypoint or a default one</returns>
    public Waypoint WaypointByName( string wypName )
    {
      if ( !HasFlightPlan ) return new Waypoint( );

      wypName = wypName.Trim( ).ToUpperInvariant( );
      // Console.WriteLine( $"WAYPOINT sought:¦{wypName}¦" );
      var wyp = Waypoints.Find(x=> x.Ident == wypName );
      if ( wyp != null )
        return wyp;
      else
        return new Waypoint( ); // not found
    }


    // Decode Part
    /// <summary>
    /// Decode the Flight Plan Parts
    /// </summary>
    /// <param name="fltFileContent">Content of the Flt file</param>
    /// <returns>A FlightPlan</returns>
    internal static FlightPlan Decode( string fltFileContent )
    {
      var ret = new FlightPlan();

      var lines = FltReader.GetPart( "ATC_Aircraft.0", fltFileContent );
      var item = FltReader.ItemContent(lines, "ActiveFlightPlan");
      if ( Tooling.ToBool( item ) == false )
        return ret; // no active FP, return an empty FP

      // OK continue
      ret.HasFlightPlan = true;
      // Get expected Alt
      item = FltReader.ItemContent( lines, "AltCleared" );  // feet or -1 if not assigned
      if ( float.TryParse( item, out float f ) )
        ret.AssignedAlt = ( f == -1 ) ? float.NaN : f;
      // Get expected Hdg
      item = FltReader.ItemContent( lines, "HdgAssigned" ); // radians or -1 if not assigned
      if ( float.TryParse( item, out f ) )
        ret.AssignedHdg = ( f == -1 ) ? float.NaN : (float)ConvConsts.ToDegrees( f );
      // Current ATC clearance
      item = FltReader.ItemContent( lines, "CtCur" ); // string
      switch ( item ) {
        case "CLEARANCE_NONE": ret.Clearance = ATC_Clearance.None; break;
        case "CLEARANCE_OWNNAV": ret.Clearance = ATC_Clearance.Own_Navigation; break;
        case "CLEARANCE_VECTORS_INTERCEPT_LEFT": ret.Clearance = ATC_Clearance.Vectors_Icpt_Left; break;
        case "CLEARANCE_VECTORS_INTERCEPT_RIGHT": ret.Clearance = ATC_Clearance.Vectors_Icpt_Right; break;
        case "CLEARANCE_VECTORS_ROUTE": ret.Clearance = ATC_Clearance.Vectors_Route; break;
        default: ret.Clearance = ATC_Clearance.None; break;
      }
      // Current ATC landing sequ.
      item = FltReader.ItemContent( lines, "LandingSequence" ); // string
      switch ( item ) {
        case "LANDING_NONE": ret.LandingSequ = ATC_Landing.None; break;
        case "LANDING_IFR_EXPECTING_APPROACH": ret.LandingSequ = ATC_Landing.IFR_Expecting_Approach; break;
        case "LANDING_IFR_CLEARED_FOR_APPROACH": ret.LandingSequ = ATC_Landing.IFR_Cleared_Approach; break;
        case "LANDING_IFR_CLEARED_TO_LAND": ret.LandingSequ = ATC_Landing.IFR_Cleared_To_Land; break;
        case "LANDING_VFR_FULL_STOP_REQUEST": ret.LandingSequ = ATC_Landing.VFR_Landing_Request; break;
        case "LANDING_VFR_FULL_STOP_FLYING_PATTERN": ret.LandingSequ = ATC_Landing.VFR_Landing_Pattern; break;
        case "LANDING_VFR_FULL_STOP_CLEARED_TO_LAND": ret.LandingSequ = ATC_Landing.VFR_Cleared_To_Land; break;
        case "LANDING_VFR_TOUCH_AND_GO_REQUEST": ret.LandingSequ = ATC_Landing.VFR_TG_Request; break;
        case "LANDING_VFR_TOUCH_AND_GO_FLYING_PATTERN": ret.LandingSequ = ATC_Landing.VFR_TG_Pattern; break;
        case "LANDING_VFR_TOUCH_AND_GO_CLEARED_TO_LAND": ret.LandingSequ = ATC_Landing.VFR_TG_Cleared_To_Land; break;
        default: ret.LandingSequ = ATC_Landing.None; break;
      }

      // Get Waypoints
      item = FltReader.ItemContent( lines, "NumberofWaypoints" );
      if ( int.TryParse( item, out int nWyp ) ) {
        for ( int i = 0; i < nWyp; i++ ) {
          item = FltReader.ItemContent( lines, $"Waypoint.{i}" );
          var wyp = Waypoint.Decode(item);
          if ( wyp != null )
            ret.Waypoints.Add( wyp );
        }
        // find first and last Airport (if there are..)
        var x = ret.Waypoints.Find(d=>d.WaypointType== WypType.Airport);
        if ( x != null ) ret.Departure = x.Ident;
        x = ret.Waypoints.FindLast( d => d.WaypointType == WypType.Airport );
        if ( x != null ) ret.Destination = x.Ident;
        // calc distances
        CalcLegDist( ret.Waypoints );
      }
      // next WYP from file
      item = FltReader.ItemContent( lines, "WaypointNext" );
      if ( int.TryParse( item, out int nextWyp ) ) {
        ret.NextWyp = nextWyp - 1;
        if ( ret.NextWyp < 0 ) ret.NextWyp = 0;
      }

      // Finalize 
      ret.Hash = ret.Pretty.GetHashCode( );
      return ret;
    }

    /// <summary>
    /// Calc Legs and Distances towards the Destination
    /// </summary>
    /// <param name="waypoints">A list of waypoints</param>
    internal static void CalcLegDist( Waypoints waypoints )
    {
      waypoints.Reverse( ); // calc from the last to the first
      Waypoint next = null;
      Waypoint lastApt = null;
      foreach ( var item in waypoints ) {
        if ( next == null ) {
          // first (actually last) is end of flight
          item.RemainingDist_nm = 0f;
          item.LegDist_nm = 0;
        }

        if ( item.WaypointType == WypType.Airport ) {
          // for some FPs if there are remaining (old) Destinations in we have more than one Airport 
          // Does not make real sense but happens, so we reset the distance counter at every Apt
          item.RemainingDist_nm = 0f;
          item.LegDist_nm = 0;
          lastApt = item; // need this one for the Runway WYP fixup later
        }
        else {
          // fixup the Runway Waypoint seems to be RWNNx - if there is a APT decoded before
          if ( item.Name == "Runway" ) {
            item.Ident = ( lastApt != null ) ? lastApt.Runway : item.Ident;
            item.Altitude = ( lastApt != null ) ? lastApt.Altitude : item.Altitude; // seems the Runway is always 0000 (MSFS BUG??) - so use the Apt Elevation
          }
          // usually one would expect an APT as the first and last WYP in the list BUT if not there is not yet a Next one valid
          if ( next != null ) {
            // calculate the legs from this to next (next as we have reversed the list)
            item.LegDist_nm = (float)item.LatLon.DistanceTo( next.LatLon, ConvConsts.EarthRadiusNm );
            item.RemainingDist_nm = next.RemainingDist_nm + item.LegDist_nm;
          }
        }
        next = item;
      }
      waypoints.Reverse( ); // back to original order
    }

  }



}
