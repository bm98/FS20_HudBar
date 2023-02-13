using CoordLib;
using FlightplanLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      var prevLatLon = _route.First( ).LatLon;
      _route[0].SetIbTrack( 0 );

      // ob tracks have the next waypoints Values as Outbound items
      // set items for the route that are not in the Plan
      for (int i = 0; i < _route.Count - 1; i++) {
        RoutePoint pt = _route[i];
        RoutePoint pt1 = _route[i + 1];

        pt.SetObLatLon( pt1.LatLon );
        pt.SetObSid( pt1.IsSidOrStar ); // if the path goes to a SID/STAR Wyp it is an OB
        pt.SetObApt( pt1.PointType == RoutePointType.Apt );
      }
      // fix the last one (omitted above)
      if (_route.Count > 1) {
        RoutePoint pt = _route.Last( );
        pt.SetObTrack( 0 );
        pt.SetObLatLon( LatLon.Empty );// last has no further OB Point
        pt.SetObSid( false );
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
    public string ID { get; private set; }
    /// <summary>
    /// The Type of this Point
    /// </summary>
    public RoutePointType PointType { get; private set; }

    /// <summary>
    /// True if the Wyp is part of the SID/STAR
    /// </summary>
    public bool IsSidOrStar { get; private set; }

    /// <summary>
    /// The Inbound true Track into this Point [deg]
    ///  0..360
    /// </summary>
    public int InboundTrueTrack { get; private set; }
    /// <summary>
    /// Coordinate of this point (lat,lon,alt_m)
    /// </summary>
    public LatLon LatLon { get; private set; }

    /// <summary>
    /// Coordinate of the next point (lat,lon,alt_m)
    /// </summary>
    public LatLon OutboundLatLon { get; private set; }

    /// <summary>
    /// The Outbound true Track from this Point [deg]
    ///  0..360
    /// </summary>
    public int OutboundTrueTrack { get; private set; }
    /// <summary>
    /// True if the Outbound route is part of the SID/STAR
    /// </summary>
    public bool OutboundSidOrStar { get; private set; }
    /// <summary>
    /// True if the Outbound route goes to an Airport/Runway
    /// </summary>
    public bool OutboundApt { get; private set; }

    /// <summary>
    /// Set a new Inbound Track from this item
    /// </summary>
    /// <param name="ibt">Inbound True Track [deg]</param>
    public void SetIbTrack( int ibt ) => InboundTrueTrack = ibt;

    /// <summary>
    /// Set a new Outbound Track for this item
    /// </summary>
    /// <param name="obt">Outbound True Track [deg]</param>
    public void SetObTrack( int obt ) => OutboundTrueTrack = obt;

    /// <summary>
    /// Set a new Outbound Point for this item
    /// </summary>
    /// <param name="obt">Outbound LatLon</param>
    public void SetObLatLon( LatLon obt ) => OutboundLatLon = obt;

    /// <summary>
    /// Set a new Outbound SidOrStar for this item
    /// </summary>
    /// <param name="obs">Outbound SidOrStar</param>
    public void SetObSid( bool obs ) => OutboundSidOrStar = obs;

    /// <summary>
    /// Set a new Outbound Airport/Runway for this item
    /// </summary>
    /// <param name="oba">Outbound Airport or Runway</param>
    public void SetObApt( bool oba ) => OutboundApt = oba;

    /// <summary>
    /// cTor: create a route point from args
    ///  set In or OutBound Track to -1 when not known
    /// </summary>
    public RoutePoint( string id, LatLon latLonAlt, TypeOfWaypoint type, int inbTrack, int outbTrack, bool sid )
    {
      this.ID = id;
      this.LatLon = latLonAlt;
      this.InboundTrueTrack = inbTrack;
      this.OutboundTrueTrack = outbTrack;
      this.IsSidOrStar = sid;
      this.OutboundSidOrStar = false;
      this.OutboundApt = false;
      switch (type) {
        case TypeOfWaypoint.Waypoint: this.PointType = RoutePointType.Wyp; break;
        case TypeOfWaypoint.VOR: this.PointType = RoutePointType.Vor; break;
        case TypeOfWaypoint.NDB: this.PointType = RoutePointType.Ndb; break;
        case TypeOfWaypoint.Airport: this.PointType = RoutePointType.Apt; break;
        case TypeOfWaypoint.Runway: this.PointType = RoutePointType.Apt; break; // is also Apt
        case TypeOfWaypoint.User: this.PointType = RoutePointType.User; break;
        default: this.PointType = RoutePointType.Other; break;
          /*
                case "ATC": return TypeOfWaypoint.ATC;
                default: return TypeOfWaypoint.Other;
           */
      }
    }

  }

}
