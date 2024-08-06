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
using static FlightplanLib.Formatter;

namespace FlightplanLib.Flightplan
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
    // our Wyp List
    private WaypointList _waypoints = new WaypointList( );

    // to extend the plan with a selected approach
    private string _selRunwayID = "";
    private string _selApproachID = "";

    /// <summary>
    /// True if the plan is Valid
    /// Must have an Origin, Destination and at least one Waypoint
    /// </summary>
    public bool IsValid => (Origin != null) && (Destination != null) && (_waypoints.Count > 0);
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
    public IEnumerable<Waypoint> Waypoints => _waypoints;

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
    public string ApproachSuffix => (Destination != null) ? Destination.ApproachSuffix : "";
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
    /// Can be empty but not null
    /// </summary>
    public string HTMLdocument { get; internal set; } = "";
    /// <summary>
    /// Links for Documents to retrieve via Plan
    /// Can be empty but not null
    /// </summary>
    public List<FileLink> DocLinks { get; internal set; } = new List<FileLink>( );
    /// <summary>
    /// Links for Images to retrieve via Plan
    /// Can be empty but not null
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



    /// <summary>
    /// Get the Waypoint with Index
    /// </summary>
    /// <param name="index">Point Index 0..N</param>
    /// <returns>A RoutePoint or Empty if not available</returns>
    public Waypoint GetWaypoint( uint index )
    {
      // sanity
      if (index >= _waypoints.Count) return Waypoint.Empty;

      return _waypoints[(int)index];
    }

    /// <summary>
    /// Set the Runway and Approach ID (set empty if not defined)
    ///  Note: This will always clear the extension points
    /// </summary>
    /// <param name="runwayID">Runway ID or an empty string</param>
    /// <param name="apprID">Approach ID or an empty string</param>
    public void SetSelectedRunwayApproachID( string runwayID, string apprID )
    {
      _selRunwayID = runwayID;
      _selApproachID = apprID;
      ClearExtension( );
    }

    /// <summary>
    /// Extend the Plan with Route waypoints addedd from fixes based on the currently selected ApproachID
    /// </summary>
    /// <param name="fixes">List of Fixes to select from</param>
    public void ExtendWithApproach( IList<IFix> fixes )
    {
      // sanity
      if (fixes == null) return;
      if (string.IsNullOrEmpty( SelectedRunwayID )) return;
      if (string.IsNullOrEmpty( SelectedApproachID )) return;


      // APPROACH FIXes
      WaypointList aprWaypoints = new WaypointList( );
      foreach (var fix in fixes) {
        if (fix.WaypointUsage == UsageTyp.Unknown) continue; // skip those

        // select if Approach and matches our designated Runway and Approach type
        var selected = fix.IsApproach
                      && (fix.RwyIdent == SelectedRunwayID)
                      && (fix.ProcRef == SelectedApproachID);

        if (!selected) continue; // try next

        // add needed properties
        var wyp = new Waypoint( ) {
          WaypointType = WaypointTyp.WYP,
          WaypointUsage = UsageTyp.APR,
          SourceIdent = fix.IdentOf,
          LatLonAlt_ft = fix.WYP.Coordinate,
          OriginalPoint = false,  // is extension
          OnDeparture = false,    // never on Departure
          HiddenInMap = true, // Map GUI (does it belong here?)
          ApproachTypeS = fix.ProcRef.ProcOf( ),
          ApproachSuffix = fix.ProcRef.SuffixOf( ),
          ApproachSequence = fix.SequenceNumber,
          RunwayNumber_S = fix.RwyIdent.RwNumberOf( ),
          RunwayDesignation = fix.RwyIdent.RwDesignationOf( ),
        };
        // set a target altitude if one is found
        wyp.SetAltitude_ft( AltTarget_ft( fix ) );

        aprWaypoints.Add( wyp );
      }

      if (aprWaypoints.Count > 0) {
        // extend before arrival airport/runway and merge if needed
        var aptWyp = _waypoints.FirstOrDefault( wyp => wyp.RoutePointType == RoutePointType.AptArrival ); // first apt Wyp
        _waypoints = _waypoints.AppendWithMerge( aprWaypoints, aptWyp );
        // recalc stuff if needed
        PostProcess( );
      }
    }

    /// <summary>
    /// Clear the extension points from the route
    /// </summary>
    public void ClearExtension( )
    {
      var extPoints = _waypoints.Where( pt => pt.OriginalPoint == false ).ToList( );
      foreach (var extPoint in extPoints) {
        _waypoints.Remove( extPoint );
      }
    }

    /// <summary>
    /// Add a Waypoint to the Flightplan
    /// 
    ///   Restricted for library internal use
    /// </summary>
    /// <param name="waypoint">A Waypoint</param>
    internal void AddWaypoint( Waypoint waypoint )
    {
      // set the first one as Previous Point while adding
      if (_waypoints.Count == 0) {
        _prevRoutePointRef = waypoint;
      }
      else {
        // some sanity.. avoid doubles - should not longer happen...
        var prevWyp = GetWaypoint( (uint)(_waypoints.Count - 1) );
        if (waypoint.Equals( prevWyp )) {
          // use the newly submitted one
          _waypoints.Remove( prevWyp );
        }
      }
      // add the new one
      waypoint.Index = (uint)_waypoints.Count; // index the added points
      waypoint.HiddenInMap = HideInMap( waypoint ); // set while adding

      // add to list
      _waypoints.Add( waypoint );
    }

    /// <summary>
    /// Add a range of Waypoints to the Flightplan
    /// 
    ///   Restricted for library internal use
    /// </summary>
    /// <param name="waypoints"></param>
    internal void AddWaypointRange( IEnumerable<Waypoint> waypoints )
    {
      foreach (var waypoint in waypoints) {
        AddWaypoint( waypoint );
      }
    }


    #region ROUTE TRACKING

    // tracks the progress with this flightplan

    private Waypoint _prevRoutePointRef = Waypoint.Empty; // shall never be null
    private Waypoint _nextRoutePointRef = Waypoint.Empty; // shall never be null
    private double _distTraveled_nm = 0f;


    /// <summary>
    /// Returns the previous Waypoint when using UpdateRoute(currentCoord)
    ///   may return an Empty Waypoint if the prev is not yet defined
    ///   Shall never return null
    /// </summary>
    public Waypoint PrevRoutePoint => _prevRoutePointRef;
    /// <summary>
    /// Returns the next Waypoint when using UpdateRoute(currentCoord)
    ///   may return an Empty Waypoint if there is no next point 
    ///   Shall never return null
    /// </summary>
    public Waypoint NextRoutePoint => _nextRoutePointRef;

    /// <summary>
    /// Distance traveled from Prev To Next Wyp
    /// </summary>
    public double DistTraveled_nm => _distTraveled_nm;

    /// <summary>
    /// The currently selected Runway ID
    /// </summary>
    public string SelectedRunwayID => _selRunwayID;

    /// <summary>
    /// The currently selected Approach ID
    /// </summary>
    public string SelectedApproachID => _selApproachID;


    /// <summary>
    /// Update the route with the current aircraft coordinate
    /// </summary>
    /// <param name="currentAcftCoord">Current Aircraft coordinate</param>
    public void TrackAircraft( LatLon currentAcftCoord )
    {
      // get previous point if available
      uint prevIndex = 0;
      if (_prevRoutePointRef.IsValid) {
        prevIndex = _prevRoutePointRef.Index;
      }
      // eval the next one
      var nextRp = GetNextRoutePoint( currentAcftCoord, 0 );
      if (nextRp != null) {
        // Next returned a result
        if (nextRp.IsValid) {
          // valid next pt
          _prevRoutePointRef = GetWaypoint( nextRp.Index - 1 ); // set prev
          _nextRoutePointRef = nextRp; // and next
          _distTraveled_nm =
            currentAcftCoord.AlongTrackDistanceTo( _prevRoutePointRef.LatLonAlt_ft, _nextRoutePointRef.LatLonAlt_ft, ConvConsts.EarthRadiusNm );
        }
        else {
          // next is empty - i.e. there is no Next anymore
          _prevRoutePointRef = GetWaypoint( (uint)_waypoints.Count - 1 ); // set prev = last
          _nextRoutePointRef = Waypoint.Empty;
          _distTraveled_nm = 0;
        }
      }
      else {
        // next is not avail (i.e. cannot evaluate..)
        // don't change anything - and try again with a new coordinate
      }
    }

    #endregion




    /// <inheritdoc/>
    public override string ToString( )
    {
      StringBuilder sb = new StringBuilder( );
      sb.AppendLine( $"Route Dump:" );

      foreach (Waypoint pt in _waypoints) {
        sb.AppendLine( pt.ToString( ) );
      }
      return sb.ToString( );
    }



    // Tools

    /// <summary>
    /// Try to get the next Waypoint of the current route
    ///  Returns an Empty if there is no next anymore
    ///  Returns null when the next cannot be evaluated
    /// </summary>
    /// <param name="coord">Current aircraft coordinate</param>
    /// <param name="prevRpIndex">The index of the (assumed) previous RoutePoint</param>
    /// <returns>A RoutePoint or Null if the Next cannot be evaluated</returns>
    private Waypoint GetNextRoutePoint( LatLon coord, uint prevRpIndex = 0 )
    {
      // sanity
      if (coord.IsEmpty) return null; // cannot evaluate 
      if (_waypoints.Count < 2) return Waypoint.Empty; // having only one point - there is no next

      // track candidates for Next
      int candidateNext = -1;
      double candidateXTRKabs = 0; // absolute XTK dist 

      // scan requested points / Segment is current to next; we catch if there is no segment left
      for (int ptCurrent = (int)prevRpIndex; ptCurrent < _waypoints.Count; ptCurrent++) {
        int ptNext = ptCurrent + 1;
        if (ptNext >= _waypoints.Count) {
          // do we reached the end of list - Next is empty
          candidateNext = candidateNext > 0 ? candidateNext : int.MaxValue;
        }
        else {
          // check if we are within a segment
          // AlongTrackDist will evaluate if the acfts projected position is on a track of a segment
          // if along, use XTK to determine the closest from the candidates
          var atDist_nm = coord.AlongTrackDistanceTo( _waypoints[ptCurrent].LatLonAlt_ft, _waypoints[ptNext].LatLonAlt_ft, ConvConsts.EarthRadiusNm );
          if ((atDist_nm > 0) && (atDist_nm < _waypoints[ptNext].InboundDistance_nm)) {
            // along track dist is from Start to Current is positive but less than Start to End 
            // check the deviation from the track
            double xtkabs = Math.Abs( coord.CrossTrackDistanceTo( _waypoints[ptCurrent].LatLonAlt_ft, _waypoints[ptNext].LatLonAlt_ft, ConvConsts.EarthRadiusNm ) );
            if (candidateNext < 0) {
              // first; just take it
              candidateNext = ptNext;
              candidateXTRKabs = xtkabs;
            }
            else {
              // having already one..
              if (xtkabs < candidateXTRKabs) {
                // we are closer to the expected track; take the new one
                candidateNext = ptNext;
                candidateXTRKabs = xtkabs;
              }
            }
          }
          else {
            // dist negative or > segment length - try next segment
          }
        }
      }

      // evaluate and return the outcome
      if (candidateNext > 0) {
        // a candidate has been found, return if possible else Empty
        return (candidateNext < _waypoints.Count) ? _waypoints[candidateNext] : Waypoint.Empty;
      }
      else {
        // cannot evaluate 
        return null;
      }
    }

    /// <summary>
    /// Does all shared Post Processing on a FlightPlan
    /// </summary>
    internal void PostProcess( )
    {
      WaypointPrune( );
      EvalStepping( );
      CalculateTrack( );
      LoadNavFrequencies( );
    }

    /// <summary>
    /// Remove invalid Waypoints if there are
    /// And reapply Indexes
    /// </summary>
    internal void WaypointPrune( )
    {
      var invList = _waypoints.Where( pt => !pt.IsValid ).ToList( );
      foreach (var pt in invList) { this._waypoints.Remove( pt ); }

      // renumber
      uint index = 1; // starts with 1
      foreach (var pt in _waypoints) { pt.Index = index++; }
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
          item.AltitudeLimitLo_ft = (int)item.LatLonAlt_ft.Altitude;
          item.AltitudeLimitHi_ft = -1;
        }
        if (item.WaypointType == WaypointTyp.RWY) {
          // Limits must match the Runway Altitude AT
          item.AltitudeLimitLo_ft = (int)item.LatLonAlt_ft.Altitude;
          item.AltitudeLimitHi_ft = (int)item.LatLonAlt_ft.Altitude;
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
    /// calculate inbound and outbound track and distances
    /// </summary>
    private void CalculateTrack( )
    {
      // calculate inbound track and distances
      double total_dist = 0;

      // ib tracks have the prev points direction to this pt value as IB
      bool addDist = true;
      for (int i = 0; i < _waypoints.Count; i++) {
        Waypoint pt = _waypoints[i];
        if (i == 0) {
          // first
          pt.InboundTrueTrk = -1; // first has no inbound track
          pt.InboundDistance_nm = 0; // dist is 0
        }
        else {
          // all but first
          Waypoint prevPt = _waypoints[i - 1];
          // from prev to this 
          pt.InboundTrueTrk = (int)prevPt.LatLonAlt_ft.BearingTo( pt.LatLonAlt_ft );
          pt.InboundDistance_nm = prevPt.LatLonAlt_ft.DistanceTo( pt.LatLonAlt_ft, ConvConsts.EarthRadiusNm );
          // add distances but don't add distances for MAPR but RUNWAY (is MAPR)
          total_dist += addDist ? pt.InboundDistance_nm : 0;
          if (pt.RoutePointType == RoutePointType.AptArrival) {
            // no longer add after arrival airport
            addDist = false;
          }
        }
      }
      // setting the sum for all legs
      Distance_Total_nm = total_dist;

      // Outbound track
      for (int i = 0; i < _waypoints.Count; i++) {
        Waypoint pt = _waypoints[i];
        if (i < (_waypoints.Count - 1)) {
          // all but last
          Waypoint nextPt = _waypoints[i + 1];
          // from this to next
          pt.OutboundLatLonAlt = nextPt.LatLonAlt_ft;
          pt.OutboundTrueTrk = (int)pt.LatLonAlt_ft.BearingTo( nextPt.LatLonAlt_ft );
          // OB track features (line between Wyps)
          pt.OutboundDistance_nm = nextPt.InboundDistance_nm;
          pt.OutboundIsSID = pt.IsSID && nextPt.IsSID;         // if the path leads from and to a SID Wyp it is an OB SID track
          pt.OutboundIsAirway = /*pt.IsAirway &&*/ nextPt.IsAirway; // if the path leads from and to a AWY Wyp it is an OB AWY track
          pt.OutboundIsSTAR = pt.IsSTAR && nextPt.IsSTAR;     // if the path leads from and to a STAR Wyp it is an OB STAR track
          pt.OutboundIsApproach = pt.IsAPR && nextPt.IsAPR;   // if the path leads from and to a APR Wyp it is an OB APR track
          pt.OutboundIsApt = nextPt.RoutePointType == RoutePointType.AptArrival; // Apt and/or runway on the Arrival
        }
        else {
          // last one
          pt.OutboundLatLonAlt = LatLon.Empty;
          pt.OutboundTrueTrk = -1; // has no outbound track
          pt.OutboundDistance_nm = 0; // dist is 0
          pt.OutboundIsApt = false; // set the last as destination airport if we did not had an airport before
        }
      }
    }


    /// <summary>
    /// Returns the Target altitude for a Fix from Facilities
    /// </summary>
    /// <param name="_w">A IFix</param>
    /// <returns>An Altitude or 0</returns>
    private static double AltTarget_ft( IFix _w )
    {
      // defaults to a potential Fix Altitude
      double altTarget_ft = ValidAlt( _w.WYP.Coordinate.Altitude ) ? _w.WYP.Coordinate.Altitude : 0;

      if (InvalidAlt( _w.AltitudeLo_ft ) && InvalidAlt( _w.AltitudeHi_ft )) {
        // both undef, remains default
      }
      else if (ValidAlt( _w.AltitudeLo_ft ) && ValidAlt( _w.AltitudeHi_ft )) {
        // both defined
        altTarget_ft = (_w.AltitudeLo_ft + _w.AltitudeHi_ft) / 2.0; // between
      }
      else if (ValidAlt( _w.AltitudeLo_ft )) {
        altTarget_ft = _w.AltitudeLo_ft;
      }
      else if (ValidAlt( _w.AltitudeHi_ft )) {
        altTarget_ft = _w.AltitudeHi_ft;
      }

      return altTarget_ft;
    }


    /// <summary>
    /// True when this item should be shown in the Map
    /// </summary>
    private static bool ShowInMap( Waypoint _w ) => !HideInMap( _w );


    /// <summary>
    /// True when this item should be hidden in the Map
    /// </summary>
    private static bool HideInMap( Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.APT) return true; // kill Airports
                                                           // if (_w.WaypointUsage == UsageTyp.HOLD) return true; // kill Holds
      if ((_w.WaypointUsage == UsageTyp.MAPR) && (_w.WaypointType != WaypointTyp.RWY)) return true; // kill MAPR but not Runway
      if (_w.ApproachSequence > 1) return true; // only show the first Approach Wyp

      return false;
    }

    /// <summary>
    /// True when this item should be used for the Route
    /// </summary>
    private static bool UseForRoute( Waypoint _w )
    {
      // selector is here rather than in the Display Section
      if (_w.WaypointType == WaypointTyp.APT) return false; // kill Airports
      if (_w.WaypointUsage == UsageTyp.HOLD) return false; // kill Holds
      if ((_w.WaypointUsage == UsageTyp.MAPR) && (_w.WaypointType != WaypointTyp.RWY)) return false; // kill MAPR but not Runway

      return true;
    }

  }
}
