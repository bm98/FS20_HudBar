using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

using FSimFacilityIF;

namespace bm98_Map.Data
{
  /// <summary>
  /// Implements a Route to be drawn on the Map
  /// </summary>
  public class Route
  {
    private List<RoutePoint> _route = new List<RoutePoint>( );

    /// <summary>
    /// Fill the outbound items which are not part of the FlightPlan
    /// </summary>
    public void RecalcTrack( )
    {
      // sanity
      if (_route.Count < 1) return; // empty route

      // init
      _route[0].SetIbTrack( 0 );

      // ob tracks have the next waypoints Values as Outbound items
      // set items for the route that are not in the Plan
      bool starTriggered = false;
      for (int i = 0; i < _route.Count - 1; i++) {
        RoutePoint pt = _route[i];
        RoutePoint ptNext = _route[i + 1];
        // lines are drawn from this to next (OB)
        pt.SetObLatLon( ptNext.Coordinate );
        pt.SetObAirway( ptNext.IsAirway ); // if the path goes via an Airway to a Wyp it is an OB
        pt.SetObSID( ptNext.IsSID ); // if the path goes to a SID Wyp it is an OB
        pt.SetObSTAR( starTriggered && ptNext.IsSTAR ); // if the path goes to a STAR Wyp it is an OB BUT not the first one...
        starTriggered = starTriggered || ptNext.IsSTAR; // delays the STAR writing by one Wyp
        pt.SetObApt( ptNext.PointType == RoutePointType.Apt );
      }
      // fix the last one (omitted above)
      if (_route.Count > 1) {
        RoutePoint pt = _route.Last( );
        pt.SetObTrack( 0 );
        pt.SetObLatLon( LatLon.Empty );// last has no further OB Point
        pt.SetObAirway( false );
        pt.SetObSID( false );
        pt.SetObSTAR( false );
        pt.SetObApt( true );
      }
    }

    /// <summary>
    /// Add a point to the route
    /// </summary>
    /// <param name="point">A route point</param>
    public void AddRoutePoint( RoutePoint point )
    {
      _route.Add( point );
      //RecalcTrack( );
    }

    /// <summary>
    /// Returns the RoutePoint Catalog
    /// </summary>
    public List<RoutePoint> RoutePointCat => _route;

  }


  /// <summary>
  /// The Type of Route Point
  /// </summary>
  public enum RoutePointType
  {
    /// <summary>
    /// A Waypoint
    /// </summary>
    Wyp = 0,
    /// <summary>
    /// A VOR
    /// </summary>
    Vor,
    /// <summary>
    /// An NDB
    /// </summary>
    Ndb,
    /// <summary>
    /// An Airport (start or landing)
    /// </summary>
    Apt,
    /// <summary>
    /// Top of Climb or Descend mark
    /// </summary>
    User,
    /// <summary>
    /// Not one of the above
    /// </summary>
    Other,
  }

  /// <summary>
  /// One Point of the Route
  /// </summary>
  public class RoutePoint
  {
    /// <summary>
    /// The ID of this Point
    /// </summary>
    public string ID { get; private set; } = "";
    /// <summary>
    /// The Type of this Point
    /// </summary>
    public RoutePointType PointType { get; private set; } = RoutePointType.Other;

    /// <summary>
    /// True if the Wyp is reached via an Airway
    /// </summary>
    public bool IsAirway { get; private set; } = false;

    /// <summary>
    /// True if the Wyp is part of the SID
    /// </summary>
    public bool IsSID { get; private set; } = false;

    /// <summary>
    /// True if the Wyp is part of the STAR
    /// </summary>
    public bool IsSTAR { get; private set; } = false;
    /// <summary>
    /// True if the Wyp is part of a SID or STAR
    /// </summary>
    public bool IsSIDorSTAR => IsSID || IsSTAR;
    /// <summary>
    /// The Inbound true Track into this Point [deg]
    ///  0..360
    /// </summary>
    public int InboundTrueTrack { get; private set; } = -1;
    /// <summary>
    /// Coordinate of this point (lat,lon,alt_m)
    /// </summary>
    public LatLon Coordinate { get; private set; } = LatLon.Empty;

    /// <summary>
    /// Coordinate of the next point (lat,lon,alt_m)
    /// </summary>
    public LatLon OutboundCoordinate { get; private set; } = LatLon.Empty;


    /// <summary>
    /// The Outbound true Track from this Point [deg]
    ///  0..360
    /// </summary>
    public int OutboundTrueTrack { get; private set; } = -1;
    /// <summary>
    /// True if the Outbound route is part of an Airway
    /// </summary>
    public bool OutboundAirway { get; private set; } = false;
    /// <summary>
    /// True if the Outbound route is part of the SID
    /// </summary>
    public bool OutboundSID { get; private set; } = false;
    /// <summary>
    /// True if the Outbound route is part of the STAR
    /// </summary>
    public bool OutboundSTAR { get; private set; } = false;
    /// <summary>
    /// True if the Outbound route goes to an Airport/Runway
    /// </summary>
    public bool OutboundApt { get; private set; } = false;

    /// <summary>
    /// An AltLimitString if not Empty
    /// </summary>
    public string AltLimitS { get; private set; } = "";
    /// <summary>
    /// Set a new Inbound Track from this item
    /// </summary>
    /// <param name="ibtrk">Inbound True Track [deg]</param>
    public void SetIbTrack( int ibtrk ) => InboundTrueTrack = ibtrk;

    /// <summary>
    /// Set a new Outbound Track for this item
    /// </summary>
    /// <param name="obtrk">Outbound True Track [deg]</param>
    public void SetObTrack( int obtrk ) => OutboundTrueTrack = obtrk;

    /// <summary>
    /// Set a new Outbound Point for this item
    /// </summary>
    /// <param name="obt">Outbound LatLon</param>
    public void SetObLatLon( LatLon obt ) => OutboundCoordinate = obt;

    /// <summary>
    /// Set a new Outbound Airway for this item
    /// </summary>
    /// <param name="obawy">Outbound Airway</param>
    public void SetObAirway( bool obawy ) => OutboundAirway = obawy;

    /// <summary>
    /// Set a new Outbound SID for this item
    /// </summary>
    /// <param name="obs">Outbound SID</param>
    public void SetObSID( bool obs ) => OutboundSID = obs;

    /// <summary>
    /// Set a new Outbound STAR for this item
    /// </summary>
    /// <param name="obs">Outbound STAR</param>
    public void SetObSTAR( bool obs ) => OutboundSTAR = obs;

    /// <summary>
    /// Set a new Outbound Airport/Runway for this item
    /// </summary>
    /// <param name="obapt">Outbound Airport or Runway</param>
    public void SetObApt( bool obapt ) => OutboundApt = obapt;

    /// <summary>
    /// cTor: create a route point from args
    ///  set In or OutBound Track to -1 when not known
    /// </summary>
    public RoutePoint( string id, LatLon latLonAlt, WaypointTyp type, int inbTrack, int outbTrack, bool sid, bool star, bool airway, string altLimit )
    {
      ID = id;
      Coordinate = latLonAlt;
      InboundTrueTrack = inbTrack;
      OutboundTrueTrack = outbTrack;
      IsSID = sid;
      IsSTAR = star;
      IsAirway = airway;
      OutboundSID = false;
      OutboundSTAR = false;
      OutboundApt = false;
      AltLimitS = altLimit;
      switch (type) {
        case WaypointTyp.WYP: PointType = RoutePointType.Wyp; break;
        case WaypointTyp.VOR: PointType = RoutePointType.Vor; break;
        case WaypointTyp.NDB: PointType = RoutePointType.Ndb; break;
        case WaypointTyp.APT: PointType = RoutePointType.Apt; break;
        case WaypointTyp.RWY: PointType = RoutePointType.Apt; break; // is also Apt
        case WaypointTyp.USR: PointType = RoutePointType.User; break;
        default: PointType = RoutePointType.Other; break;
          /*
                case "ATC": return TypeOfWaypoint.ATC;
                default: return TypeOfWaypoint.Other;
           */
      }
    }

  }

}
