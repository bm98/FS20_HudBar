using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;
using static dNetBm98.Units;
using static dNetBm98.XString;
using CoordLib;
using CoordLib.Extensions;
using MapLib;

using bm98_Map.Drawing;
using bm98_Map.Data;
using bm98_Map.UI;
using bm98_VProfile;
using FlightplanLib.Flightplan;

namespace bm98_Map
{
  /// <summary>
  /// Display mode for other aircrafts
  /// </summary>
  public enum AcftAiDisplayMode
  {
    /// <summary>
    /// Display of other aircraft is off
    /// </summary>
    None = 0,
    /// <summary>
    /// Show all aircrafts
    /// </summary>
    All,
    /// <summary>
    /// Shows filtered aircrafts only
    /// </summary>
    Filtered,
  }

  /// <summary>
  /// Behavior Mode
  /// </summary>
  public enum MapBehavior
  {
    /// <summary>
    /// Map Mode 
    ///  Airport centric (standard behavior)
    /// </summary>
    Map = 0,
    /// <summary>
    /// Radar Mode
    ///  Aircraft centric
    /// </summary>
    Radar
  }

  /// <summary>
  /// User Control to draw Maps
  /// Essentially draw items related to Lat, Lon coordinates
  /// The Items available depend on the use case
  /// Here it will be Airport Runways
  /// 
  /// Uses a Background Image for non dynamic stuff
  /// </summary>
  public partial class UC_Map : UserControl
  {
    // WinForms Invoker
    private readonly dNetBm98.Win.WinFormInvoker _eDispatch;


    // The Viewport for this Map
    private readonly VPort2 _viewport;
    // our TT instance
    private readonly ToolTip _toolTip;

    // Map Creator Toolings
    private readonly MapCreator _mapCreator;

    // Behavior Mode of the Map display
    private MapBehavior _mapBehavior = MapBehavior.Map;
    private MapProvider _mapMapProvider = MapProvider.OSM_OpenStreetMap;
    private const MapProvider c_radarMapProvider = MapProvider.CARTO_DarkNL;

    // Holding a ref to the commited airport here
    private Airport _airportRef = Airport.DummyAirport( LatLon.Empty );
    // Holding a ref to the current Navaids
    private IList<INavaid> _navaidRef = new List<INavaid>( ); // empty list instead of null;
    // Holding a ref to the current Fixes
    private IList<IFix> _fixesRef = new List<IFix>( ); // empty list instead of null;
    // Holding a ref to the current Airports
    private IList<IAirportDesc> _airportsRef = new List<IAirportDesc>( ); // empty list instead of null;;
    // Holding a ref to the current Flightplan
    private FlightPlan _flightPlanRef = new FlightPlan( );


    // maintains all the visuals of the Airport
    private readonly DisplayListMgr _airportDisplayMgr;

    // internal Aircraft Data Tracking obj
    private readonly TrackedAircraft _trackedAircraft = new TrackedAircraft( );

    // Center of the Map (airport) when loaded
    // private LatLon _airportCoord = new LatLon( );
    // updated center when Extended to any side
    private LatLon _mapCenterDyn = new LatLon( );
    // need to render the static items
    private bool _renderStaticNeeded = false;

    // master map range
    private readonly MapRangeHandler _mapRangeHandler;

    // Loop to complete when tiles have failed to load
    private const int c_maxFailedLoadingCount = 5; // try max loops to get the complete image

    private readonly Dictionary<MapRange, Button> _mrButtons = new Dictionary<MapRange, Button>( );
    private readonly Color _mrColorOFF;
    private readonly Color _mrColorON = Color.Yellow;

    // button bg color for deco buttons
    private readonly Color _decoBColorOFF;
    private readonly Color _decoBColorON = Color.LimeGreen;
    private readonly Color _decoBColorALT = Color.FromArgb( 215, 215, 0 ); // alternate color darker yellow

    // unit handlers
    private bool _hdgIsTrue = true;
    private bool _trkIsTrue = false;
    // unit toggle is in use
    private bool _unitIsImp = true;
    // individual ones follow the unit toggle in this implementation
    private bool _altIsFeet = true;
    private bool _speedIsKt = true;
    private bool _vsIsFpm = true;

    // Panels
    private readonly StripPanel _pnlRunways;
    private readonly StripPanel _pnlApproaches;
    private readonly StripPanel _pnlNavaids;
    private readonly StripPanel _pnlTower;

    private readonly StripPanel _pnlProviders;

    #region User Control API

    /// <summary>
    /// Fired when the center of the map has changed
    /// </summary>
    [Category( "Map" )]
    [Description( "The Map Center Tile has changed" )]
    public event EventHandler<MapEventArgs> MapCenterChanged;
    private void OnMapCenterChanged( LatLon center )
    {
      MapCenterChanged?.Invoke( this, new MapEventArgs( center, _mapRangeHandler.MapRange, _mapRangeHandler.ZoomLevel ) );
    }

    /// <summary>
    /// Fired when the map range has changed
    /// </summary>
    [Category( "Map" )]
    [Description( "The Map Range has changed" )]
    public event EventHandler<MapEventArgs> MapRangeChanged;
    private void OnMapRangeChanged( )
    {
      MapRangeChanged?.Invoke( this, new MapEventArgs( _viewport.Map.CenterCoord, _mapRangeHandler.MapRange, _mapRangeHandler.ZoomLevel ) );
    }

    /// <summary>
    /// Fired when the used wants to teleport the aircraft
    /// </summary>
    public event EventHandler<TeleportEventArgs> TeleportAircraft;
    private void OnTeleportAircraft( LatLon latLon, bool altIsMsl, int altitude_ft )
    {
      TeleportAircraft?.Invoke( this, new TeleportEventArgs( latLon.Lat, latLon.Lon, altIsMsl, altitude_ft ) );
    }


    /// <summary>
    /// Access the MapCreator of this Map
    /// </summary>
    [Category( "Map" )]
    [Description( "MapCreator to set an airport" )]
    public MapCreator MapCreator => _mapCreator;

    /// <summary>
    /// Get; Set: The native Range of the Map
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: the map range" )]
    public MapRange MapRange {
      get => _mapRangeHandler.MapRange;
      set => _mapRangeHandler.SetMapRange( value ); // this may fail and do nothing if not within current Zoom bounds
    }

    /// <summary>
    /// Get; Set: The Auto Range property of the Map
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: auto range property" )]
    public bool AutoRange {
      get => _viewport.AutoRange;
      set {
        _viewport.AutoRange = value;
        btRangeAuto.ForeColor = value ? _mrColorON : _mrColorOFF;
      }
    }

    /// <summary>
    /// Update the tracked AI aircrafts
    /// </summary>
    /// <param name="aircraftsAi">List of AI aircrafts</param>
    public void UpdateAircraftsAI( IList<ITrackedAircraft> aircraftsAi )
    {
      // sanity
      if (aircraftsAi == null) return;

      // Aircraft Drawing update goes via the AirportDisplayManager object
      _airportDisplayMgr.UpdateAircraftsAI( aircraftsAi );
      // Update the View
      RenderStatic( ); // will only render if needed
      // Aircraft Sprites are updated on every cycle
      _airportDisplayMgr.RenderSprite( );
      _viewport.Redraw( );
    }

    /// <summary>
    /// Update the tracked Aircrafts position and shown properties
    /// submit a NaN if the items shall be hidden
    /// </summary>
    /// <param name="trackedAircraft">The tracked aircraft properties</param>
    public void UpdateAircraft( ITrackedAircraft trackedAircraft )
    {
      // sanity
      if (trackedAircraft == null) return;

      // update our internal _aircraftTracker from the delivered one
      _trackedAircraft.Update( trackedAircraft );

      // Aircraft Labels, hide if the value is float.NaN (ex Heading)
      // data label are 4 chars, number fields 6 chars to align vert. !!
      lblTHdg.Visible = true;
      if (_hdgIsTrue) { lblTHdg.Text = $"THDG: {_trackedAircraft.TrueHeading_deg,6:000}°"; }
      else { lblTHdg.Text = $"HDG : {_trackedAircraft.Heading_degm,6:000}°M"; }

      lblMTrk.Visible = _trackedAircraft.ShowMTRK;
      if (_trkIsTrue) { lblMTrk.Text = $"TTRK: {_trackedAircraft.TrueTrk_deg,6:000}°"; }
      else { lblMTrk.Text = $"TRK : {_trackedAircraft.Trk_degm,6:000}°M"; }

      lblAlt.Visible = _trackedAircraft.ShowAlt;
      if (_altIsFeet) { lblAlt.Text = $"AMSL: {_trackedAircraft.AltitudeMsl_ft,6:##,##0} ft"; }
      else { lblAlt.Text = $"AMSL: {M_From_Ft( _trackedAircraft.AltitudeMsl_ft ),6:##,##0} m"; }

      lblRA.Visible = _trackedAircraft.ShowRA;
      if (_altIsFeet) { lblRA.Text = $"RA  : {_trackedAircraft.RadioAlt_ft,6:##,##0} ft"; }
      else { lblRA.Text = $"RA  : {M_From_Ft( _trackedAircraft.RadioAlt_ft ),6:##,##0} m"; }

      lblIAS.Visible = _trackedAircraft.ShowIas;
      if (_speedIsKt) { lblIAS.Text = $"IAS : {_trackedAircraft.Ias_kt,6:#,##0} kt"; }
      else { lblIAS.Text = $"IAS : {Kmh_From_Kt( _trackedAircraft.Ias_kt ),6:#,##0} km/h"; }

      lblGS.Visible = _trackedAircraft.ShowGs;
      if (_speedIsKt) { lblGS.Text = $"GS  : {_trackedAircraft.Gs_kt,6:#,##0} kt"; }
      else { lblGS.Text = $"GS  : {Kmh_From_Kt( _trackedAircraft.Gs_kt ),6:#,##0} km/h"; }

      lblVS.Visible = _trackedAircraft.ShowVs;
      if (_vsIsFpm) { lblVS.Text = $"V/S : {_trackedAircraft.Vs_fpm,6:+#,##0;-#,##0;---} fpm"; }
      else { lblVS.Text = $"V/S : {Mps_From_Ftpm( _trackedAircraft.Vs_fpm ),6:+#0.0;-#0.0;---} m/s"; }

      // set windspeed string for the Sprite if not default
      if (_speedIsKt) { } // default
      else { _trackedAircraft.WindSpeedS = $"{Mps_From_Kt( _trackedAircraft.WindSpeed_kt ):#0.0}m/s"; }

      // Aircraft Drawing update goes via the AirportDisplayManager object
      _airportDisplayMgr.UpdateAircraft( _trackedAircraft );
      if (_mapBehavior == MapBehavior.Radar) {
        _renderStaticNeeded = true;
      }

      // Update VProfile Props
      if (vpProfile.Visible) {
        // Create a List of Points starting from the NextPoint of the current DispRoute
        var wypList = new List<UC_VProfile.UC_VProfilePropsRoutepoint>( );
        double dist = 0;
        // evaluate where we are within the current DispRoute
        Waypoint nextRp = _flightPlanRef.NextRoutePoint;
        if (nextRp.IsValid) {
          // having a valid NextPoint...
          // subtract distance traveled from Prev to Next Point for the VProfile
          dist = nextRp.InboundDistance_nm - _flightPlanRef.DistTraveled_nm;
          var nextWyp = new UC_VProfile.UC_VProfilePropsRoutepoint( ) {
            Ident = nextRp.Ident.LeftString( 5 ),
            Distance_nm = dist,
            TargetAlt_ft = nextRp.TargetAltitude_ft,
          };
          wypList.Add( nextWyp );

          // capture further Wyps along the DispRoute
          nextRp = _flightPlanRef.GetWaypoint( nextRp.Index + 1 );
          while (nextRp.IsValid) {
            dist += nextRp.InboundDistance_nm;
            nextWyp = new UC_VProfile.UC_VProfilePropsRoutepoint( ) {
              Ident = nextRp.Ident.LeftString( 5 ),
              Distance_nm = dist,
              TargetAlt_ft = nextRp.TargetAltitude_ft,
            };
            wypList.Add( nextWyp );
            // next round
            nextRp = _flightPlanRef.GetWaypoint( nextRp.Index + 1 );
          }
        }
        // create the VProfile Props with current data
        var vpProps = new UC_VProfile.UC_VProfileProps( ) {
          ALT_ft = _trackedAircraft.AltitudeIndicated_ft,
          GS_kt = _trackedAircraft.Gs_kt,
          VS_fpm = _trackedAircraft.Vs_fpm,
          FPA_deg = _trackedAircraft.Fpa_deg,
          WaypointList = wypList,
        };
        // and update the Control
        vpProfile.UpdatePanelProps( vpProps );
      }

      // Update the View
      RenderStatic( ); // will only render if needed
      // Aircraft Sprites are updated on every cycle
      _airportDisplayMgr.RenderSprite( );
      _viewport.Redraw( );
    }

    /// <summary>
    /// To set the shown Navaids
    /// Overwrites the existing list
    /// </summary>
    /// <param name="navaids">List of navaids to show</param>
    /// <param name="fixes">List of Fixes to show</param>
    public void SetNavaidList( List<INavaid> navaids, List<IFix> fixes )
    {
      // sanity
      if (navaids != null) {
        _navaidRef = navaids;
      }
      else {
        _navaidRef = new List<INavaid>( ); // empty list instead of null
      }

      if (fixes != null) {
        _fixesRef = fixes;
      }
      else {
        _fixesRef = new List<IFix>( ); // empty list instead of null;
      }

      PopulateNavaids( _navaidRef );
      _airportDisplayMgr.SetNavaidList( _navaidRef, _fixesRef );
      // Trigger Update the View
      _renderStaticNeeded = true;
    }

    /// <summary>
    /// To set the shown Navaids
    /// Overwrites the existing list
    /// </summary>
    /// <param name="airports">List of airports to show</param>
    public void SetAltAirportList( List<IAirportDesc> airports )
    {
      // sanity
      if (airports != null) {
        _airportsRef = airports;
      }
      else {
        _airportsRef = new List<IAirportDesc>( );
      }

      _airportDisplayMgr.SetAltAirportList( _airportsRef );
      // Trigger Update the View
      _renderStaticNeeded = true;
    }

    /// <summary>
    /// To set the Flightplan plotted on the Map
    /// 
    /// The Route is  part of the various Flightplan types and 
    ///   May or may not include artificial Wyps (from MS or Simbrief etc)
    ///   May or may not include Approach Wyps,
    ///   May or may not include Airport Wyps
    /// </summary>
    /// <param name="flightplan">A Flightplan Obj</param>
    public void SetFlightplan( FlightPlan flightplan )
    {
      // sanity
      if (flightplan == null) return;

      _flightPlanRef = flightplan;

      // init visualization of the route in the map
      _airportDisplayMgr.SetFlightplan( flightplan );

      // Set when already selected and the selected destination
      if (_airportRef.ICAO == flightplan.Destination.Icao_Ident.ICAO) {
        _flightPlanRef?.SetSelectedRunwayApproachID(
          _airportDisplayMgr.GetSelectedNavIdRunway( ),
          _airportDisplayMgr.GetSelectedNavIdRunwayApproach( )
        );
      }
      else {
        _flightPlanRef?.SetSelectedRunwayApproachID( "", "" );
      }
      // Extend route with Approach if one is selected
      _flightPlanRef?.ExtendWithApproach( _fixesRef );
      // init route tracking from the current position
      _flightPlanRef?.TrackAircraft( _trackedAircraft.Position );

      // Trigger Update the View
      _renderStaticNeeded = true;
    }

    /// <summary>
    /// Zoom Into the Image
    /// </summary>
    public void ZoomIn( ) => _viewport.ZoomIn( );
    /// <summary>
    /// Zoom Out of the Image
    /// </summary>
    public void ZoomOut( ) => _viewport.ZoomOut( );
    /// <summary>
    /// Zoom to 1:1
    /// </summary>
    public void ZoomNorm( ) => _viewport.ZoomNorm( );

    /// <summary>
    /// Move the Map to the Center of the View and make it 1:1
    /// </summary>
    public void CenterMap( ) => _viewport.CenterMap( );

    /// <summary>
    /// Back to Original loaded Center (airport)
    /// </summary>
    public void OriginalMap( )
    {
      // reload the original map at zoom 1
      ZoomNorm( );
      UpdateMapCenter( _airportRef.Coordinate ); // reset dynamic center as well
      StartMapLoading( _airportRef.Coordinate ); // load around airport center
    }

    /// <summary>
    /// Renders items if needed
    /// </summary>
    public void RenderItems( )
    {
      RenderStatic( );
    }

    /// <summary>
    /// True to show the map grid, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the tracked aircraft" )]
    public bool ShowTrackedAircraft {
      get => _airportDisplayMgr.ShowTrackedAircraft;
      set {
        _airportDisplayMgr.ShowTrackedAircraft = value; btTogAcftData.BackColor = (value) ? _decoBColorON : _decoBColorOFF;
        pbAltLadder.Visible = value;
        if (value) { pbAltLadder.BringToFront( ); }
        flpAcftData.Visible = value;
      }
    }

    /// <summary>
    /// True to show the map grid, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the route" )]
    public bool ShowRoute {
      get => _airportDisplayMgr.ShowRoute;
      set { _airportDisplayMgr.ShowRoute = value; btTogShowRoute.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the map grid, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the map grid" )]
    public bool ShowMapGrid {
      get => _airportDisplayMgr.ShowMapGrid;
      set { _airportDisplayMgr.ShowMapGrid = value; btTogGrid.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the airport range circles, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the airports range circles" )]
    public bool ShowAirportRange {
      get => _airportDisplayMgr.ShowAiportRange;
      set { _airportDisplayMgr.ShowAiportRange = value; btTogRings.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the navaids, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the navaids (VOR/NDB)" )]
    public bool ShowNavaids {
      get => _airportDisplayMgr.ShowNavaids;
      set { _airportDisplayMgr.ShowNavaids = value; btTogNavaids.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the VFR range circles, false otherwise
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the VFR marks on the selected airport runway" )]
    public bool ShowVFRMarks {
      get => _airportDisplayMgr.ShowVFRMarks;
      set { _airportDisplayMgr.ShowVFRMarks = value; btTogVFR.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the Airport Marks
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing the airports" )]
    public bool ShowAptMarks {
      get => _airportDisplayMgr.ShowAptMarks;
      set { _airportDisplayMgr.ShowAptMarks = value; btTogApt.BackColor = (value) ? _decoBColorON : _decoBColorOFF; }
    }

    /// <summary>
    /// True to show the Airport Marks
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing other aircrafts" )]
    public AcftAiDisplayMode ShowOtherAircrafts {
      get => _airportDisplayMgr.ShowTrackedAircraftAI;
      set {
        _airportDisplayMgr.ShowTrackedAircraftAI = value;
        btTogAcftAi.BackColor = (value == AcftAiDisplayMode.All) ? _decoBColorON
                              : (value == AcftAiDisplayMode.Filtered) ? _decoBColorALT
                              : _decoBColorOFF;
      }
    }

    /// <summary>
    /// True to show the Airport Marks
    /// </summary>
    [Category( "Map" )]
    [Description( "Get;Set: showing other aircrafts Enabled" )]
    public bool ShowOtherAircraftsEnabled {
      get => btTogAcftAi.Visible;
      set { btTogAcftAi.Visible = value; }
    }

    /// <summary>
    /// Load an filter list for other acfts
    /// </summary>
    public void SetOtherAircraftFilter( IList<string> filterList ) => _airportDisplayMgr.SetAcftAiFilter( filterList ); // forward to disp manager


    /// <summary>
    /// Returns the current Map Center
    /// </summary>
    /// <returns>A LatLon</returns>
    public LatLon MapCenter( ) => _mapCenterDyn;

    #endregion

    #region Control Behavior

    // set the visibility of all map controls (inputs)
    private void CtrlVisibility( MapBehavior mode )
    {
      switch (mode) {
        case MapBehavior.Map:
          // hidden ones first
          // still available 
          lblAirport.Visible = true;
          flpAcftData.Visible = true;
          btTogGrid.Visible = true; btTogRings.Visible = true;
          btTogAcftData.Visible = true; btTogShowRoute.Visible = true;
          btTogNavaids.Visible = true; btTogVFR.Visible = true; btTogApt.Visible = true; btTogAcftAi.Visible = true;
          btRunway.Visible = true; btTower.Visible = true; btNavaids.Visible = true;
          btMapProvider.Visible = true;
          btRangeAuto.Visible = true;
          btRangeXFar.Visible = true; btRangeFarFar.Visible = true; btRangeFar.Visible = true;
          btRangeMid.Visible = true; btRangeNear.Visible = true; btRangeClose.Visible = true;
          btCenterApt.Visible = true; btCenterAircraft.Visible = true;
          btZoomNorm.Visible = true; btZoomIn.Visible = true; btZoomOut.Visible = true;
          UpdateCtxMenuText( );
          this.ContextMenuStrip = ctxMenu;
          break;

        case MapBehavior.Radar:
          // hidden ones first
          btTogGrid.Visible = false;
          lblAirport.Visible = false;
          btTogRings.Visible = false;
          btTogAcftData.Visible = false;
          btTogShowRoute.Visible = false;
          btTogVFR.Visible = false;
          btRangeAuto.Visible = false;
          btRunway.Visible = false; btTower.Visible = false; btNavaids.Visible = false;
          btMapProvider.Visible = false;
          btRangeXFar.Visible = false; btRangeClose.Visible = false;
          latLonField.Visible = false;
          teleportField.Visible = false;
          this.ContextMenuStrip = null;
          _pnlTower.Visible = false;
          _pnlNavaids.Visible = false;
          _pnlApproaches.Visible = false;
          _pnlRunways.Visible = false;

          // still available 
          flpAcftData.Visible = true;
          btTogNavaids.Visible = true;
          btTogApt.Visible = true;
          btTogAcftAi.Visible = true;
          btRangeFarFar.Visible = true; btRangeFar.Visible = true;
          btRangeMid.Visible = true; btRangeNear.Visible = true;
          btCenterApt.Visible = true; btCenterAircraft.Visible = true;
          btZoomNorm.Visible = true; btZoomIn.Visible = true; btZoomOut.Visible = true;
          break;
      }
    }

    // changes the map mode 
    private void ChangeBehavior( MapBehavior mode, LatLon mapCenter )
    {
      _mapBehavior = mode;
      CtrlVisibility( mode ); // update controls

      // Trigger Update the View
      _renderStaticNeeded = true;

      switch (mode) {
        case MapBehavior.Map:
          _airportDisplayMgr.SetMapBehavior( _mapBehavior );
          // clear the managed DispItems
          _airportDisplayMgr.ClearDispItems( );
          // create the Airports DrawingList
          _airportDisplayMgr.AddDispItems( _airportRef );
          // some fiddling with airport and extended handling 
          UpdateMapCenter( mapCenter );
          // trigger render - preliminary nothing loaded so far
          _renderStaticNeeded = true;
          // load the Map at the selected airport coord
          MapManager.Instance.SetNewProvider( _mapMapProvider );
          StartMapLoading( mapCenter );
          _viewport.SetMouseAction( true );
          _airportDisplayMgr.UpdateAircraft( _trackedAircraft );

          break;

        case MapBehavior.Radar:
          _airportDisplayMgr.SetMapBehavior( _mapBehavior );
          // clear the managed DispItems
          _airportDisplayMgr.ClearDispItems( );
          // create the Airports DrawingList
          _airportDisplayMgr.AddDispItems( _airportRef );
          // trigger render - preliminary nothing loaded so far
          _renderStaticNeeded = true;
          // load the Map at the aircrafts position
          _mapMapProvider = MapManager.Instance.CurrentProvider; // save current
          MapManager.Instance.SetNewProvider( c_radarMapProvider ); // switch to radar
          _mapRangeHandler.SetMapRange( _mapRangeHandler.MapRange ); // validate if possible
          StartMapLoading( mapCenter );
          _viewport.SetMouseAction( false );
          _airportDisplayMgr.UpdateAircraft( _trackedAircraft );
          break;
      }
      // update if available
      if (_navaidRef != null) _airportDisplayMgr.SetNavaidList( _navaidRef, _fixesRef );
      if (_airportsRef != null) _airportDisplayMgr.SetAltAirportList( _airportsRef );

    }


    #endregion

    #region Airport Panel Updates

    // Load Airport Label
    private void PopulateApt( Airport airport )
    {
      var iata = $"({airport.IATA})";
      lblAirport.Text = $"{airport.ICAO,-4} {iata,-6}   {airport.Name}\n"
        + $"{Dms.ToLat( airport.Coordinate.Lat, "dm", "", 0 )} {Dms.ToLon( airport.Coordinate.Lon, "dm", "", 0 )}   {airport.Elevation_ft:####0} ft ({airport.Elevation_m:###0} m)";
    }

    #region Tower Panel

    // Load Frequencies Panel
    private void PopulateFrequencies( Airport airport )
    {
      _pnlTower.InitUpdate( );
      _pnlTower.ClearItems( );

      foreach (var frq in airport.Comms) {
        _pnlTower.AddItem( frq.CommString( ).PadRight( 63 ), null, false );
      }
      _pnlTower.CommitUpdate( -1, this.ClientRectangle.Bottom - lblCopyright.Height );
    }

    // hide this
    private void _pnlTower_EmptyClicked( object sender, EventArgs e )
    {
      _pnlTower.Visible = false;
    }

    #endregion

    #region Runway Panel

    // fill the approach panel from a runway
    private void PopulateRunwayApproaches( IRunway runway )
    {
      _pnlApproaches.InitUpdate( );
      _pnlApproaches.ClearItems( );
      if (runway != null) {
        // Airport Runways
        _pnlApproaches.Title = $"Runway {runway.Ident} - Approaches";
        var rwyWyps = _fixesRef.Where( x => x.IsApproach && x.RwyIdent == runway.Ident );
        // all runway-Approaches (ILS, RNAV etc)
        var rwyApproaches = rwyWyps.Select( y => y.ProcRef ).Distinct( ).OrderBy( x => x );
        foreach (var appName in rwyApproaches) {
          var fix = _fixesRef.Where(
          x => x.IsApproach
          && x.RwyIdent == runway.Ident
          && x.ProcRef == appName ).FirstOrDefault( );
          if (fix != null) {
            string ngTag = ((fix.FixInfo != null) && fix.WYP.IsNG) ? "'" : " ";
            _pnlApproaches.AddItem( $"{appName,-7} {$"({fix.WYP.Ident})",-8} {runway.Ident}  {ngTag}".PadRight( 30 ), appName, true );
          }
        }
      }
      _pnlApproaches.CommitUpdate( -1, _pnlRunways.Top );
    }

    // Fill the runways panel from an airport
    private void PopulateRunways( Data.Airport airport )
    {
      var padLen = 65;
      _pnlRunways.InitUpdate( );
      _pnlRunways.ClearItems( );

      // all runways
      foreach (var rwy in airport.Runways) {
        // RW00 without Approaches is omitted
        if (rwy.HasAPRs || (rwy.Ident != RwALLIdent)) {
          _pnlRunways.AddItem( rwy.RunwayString( _navaidRef ).PadRight( padLen ), rwy, true ); // make is selectable
        }
      }

      // Airport Navaids - if there are any
      var aptNavaids = new List<INavaid>( );
      aptNavaids = airport.NAV_FKEYs( ).SelectMany( fk => _navaidRef.Where( nav => nav.KEY == fk && (nav.IsVOR || nav.IsNDB) ) ).ToList( );
      // if there are any apt navaids, list them
      if (aptNavaids.Count( ) > 0) {
        _pnlRunways.AddSubTitle( "Airport - Navaids" );
        var itemList = new List<string>( ); // manage doubles
        foreach (var nav in aptNavaids) {
          double distance_nm = _airportRef.Coordinate.DistanceTo( nav.Coordinate, ConvConsts.EarthRadiusNm );
          if (distance_nm > (nav.Range_nm * 1.1)) continue; // range + 10% else cannot be received at the airport
          string rsiS = dNetBm98.Utilities.RSI( distance_nm, nav.Range_nm );
          string dir = Dms.CompassPoint( _airportRef.Coordinate.BearingTo( nav.Coordinate ), 2 );
          var item = nav.VorNdbNameString( dir, distance_nm, rsiS );
          if (!itemList.Contains( item )) {
            _pnlRunways.AddItem( item.PadRight( padLen ), null, false );
            itemList.Add( item );
          }
        }
      }
      _pnlRunways.CommitUpdate( -1, this.ClientRectangle.Bottom - lblCopyright.Height );
      // init approach panel empty and unselected, will populate on runway strip click
      // needs runway panel to be layed out for the placement
      PopulateRunwayApproaches( null );
    }

    // a strip in the approach panel was clicked
    private void ApproachLabel_Click( object sender, EventArgs e )
    {
      // sanity
      if (!(sender is Label)) return;

      var label = sender as Label;
      if (!(label.Tag is string)) {
        // clear selected approach
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( "" ); // show all
        _flightPlanRef?.SetSelectedRunwayApproachID( "", "" ); // clear route extension
      }
      else {
        var approachName = label.Tag as string;
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( approachName );
        // Set Approach extension
        _flightPlanRef?.SetSelectedRunwayApproachID(
          _airportDisplayMgr.GetSelectedNavIdRunway( ),
          _airportDisplayMgr.GetSelectedNavIdRunwayApproach( )
        );
        // Extend route with Approach if one is selected
        _flightPlanRef?.ExtendWithApproach( _fixesRef );
      }
      // render and redraw
      RenderStatic( true );
    }

    // a strip in the runway panel was clicked
    private void RunwayLabel_Click( object sender, EventArgs e )
    {
      // sanity
      if (!(sender is Label)) return;

      // Clear route extension
      _flightPlanRef?.ClearExtension( );

      _pnlApproaches.Visible = false;
      var label = sender as Label;
      if (!(label.Tag is IRunway)) {
        // clear selected runway
        //ShowVFRMarks = false;
        _airportDisplayMgr.SetRunwayVFRDispItems( null ); // unselected
        PopulateRunwayApproaches( null ); // unselected
        _airportDisplayMgr.SetSelectedNavIdRunway( "" );
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( "" );
        _flightPlanRef?.SetSelectedRunwayApproachID( "", "" );
      }
      else {
        // get this pair
        var pair = (label.Tag as IRunway).RunwayPair( _airportRef.Runways );
        _airportDisplayMgr.SetRunwayVFRDispItems( pair );
        // IFR Waypoint
        _airportDisplayMgr.SetSelectedNavIdRunway( pair.First( ).Ident );
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( "" );
        _flightPlanRef?.SetSelectedRunwayApproachID( "", "" );
        // populate approaches
        PopulateRunwayApproaches( pair.First( ) );
      }
      _pnlApproaches.Visible = true;
      // render and redraw
      RenderStatic( true );
    }

    // hide this
    private void _pnlRunways_EmptyClicked( object sender, EventArgs e )
    {
      _pnlApproaches.Visible = false;
      _pnlRunways.Visible = false;
    }
    private void _pnlApproaches_EmptyClicked( object sender, EventArgs e )
    {
      // not used
    }

    #endregion

    #region Navaids Panel 

    // load Navaids Panel
    private void PopulateNavaids( IList<INavaid> navaids )
    {
      _pnlNavaids.InitUpdate( );
      _pnlNavaids.ClearItems( );

      // sanity if called with navaids==null
      if (navaids == null) return;

      var navaidsSorted = navaids.Distinct( ).OrderBy( x => x.Ident ).ToList( );
      foreach (var nav in navaidsSorted) {
        if (nav.IsILS) continue; // skip
        if (string.IsNullOrEmpty( nav.Ident )) continue;  // seen some??

        double distance_nm = _trackedAircraft.Position.DistanceTo( nav.Coordinate, ConvConsts.EarthRadiusNm );
        if (distance_nm > (nav.Range_nm * 1.1)) continue; // range + 10% else cannot be received at the airport
        string rsiS = dNetBm98.Utilities.RSI( distance_nm, nav.Range_nm );
        string dir = Dms.CompassPoint( _trackedAircraft.Position.BearingTo( nav.Coordinate ), 2 );

        _pnlNavaids.AddItem( nav.VorNdbNameString( dir, distance_nm, rsiS ).PadRight( 63 ), null, false );
      }
      _pnlNavaids.CommitUpdate( -1, this.ClientRectangle.Bottom - lblCopyright.Height );
    }

    // hide this
    private void _pnlNavaids_EmptyClicked( object sender, EventArgs e )
    {
      _pnlNavaids.Visible = false;
    }

    #endregion

    #region MapProvider Panel

    // load map provider Panel
    private void PopulateMapProviders( )
    {
      if (DesignMode) return;

      _pnlProviders.InitUpdate( );
      _pnlProviders.ClearItems( );

      foreach (var p in MapManager.Instance.EnabledProviders) {
        _pnlProviders.AddItem( MapManager.Instance.ProviderName( p ).PadRight( 42 ), p, true );
      }
      _pnlProviders.CommitUpdate( btMapProvider.Bottom + 5, -1 );
    }

    // handle provider was clicked
    private void ProviderLabel_Click( object sender, EventArgs e )
    {
      // sanity
      if (!(sender is Label label)) return;
      if (!(label.Tag is MapProvider)) return;

      if (label.Tag is MapProvider mapProvider) {
        if (mapProvider != MapProvider.DummyProvider) {
          // change provider
          MapManager.Instance.SetNewProvider( mapProvider );
          // verify if current zoom is possible - change if needed
          _mapRangeHandler.SetMapRange( _mapRangeHandler.MapRange ); // try set current, will adjust if needed
          StartMapLoading( _mapCenterDyn );
          _pnlProviders.Visible = false;
        }
      }
    }

    #endregion

    #endregion

    #region MapCreator Event handling

    // a new Airport was committed
    private void _mapCreator_Commited( object sender, EventArgs e )
    {
      // Re Init the drawings
      _airportRef = _mapCreator.CommitedAirport;
      // clear the managed DispItems
      _airportDisplayMgr.ClearDispItems( );
      // create the Airports DrawingList
      _airportDisplayMgr.AddDispItems( _airportRef );
      // set a new target alt
      _trackedAircraft.TargetAltitude_ft = _airportRef.Elevation_ft;

      // Whenever an airport is selected switch to Map Mode
      // (else there is a competition between tracking the acft and going to the airport)
      ChangeBehavior( MapBehavior.Map, _airportRef.Coordinate );

      // init from new airport
      PopulateApt( _airportRef );
      PopulateRunways( _airportRef );
      PopulateFrequencies( _airportRef );
      _flightPlanRef?.SetSelectedRunwayApproachID( "", "" ); // clears

      // trigger render - preliminary nothing loaded so far
      _renderStaticNeeded = true;
    }

    #endregion

    #region Map Handling

    // will Render static items and redraw them if needed
    // set forced to override the trigger flag
    private void RenderStatic( bool forced = false )
    {
      if (forced || _renderStaticNeeded) {
        _renderStaticNeeded = false; // reset
        _airportDisplayMgr.RenderStatic( );
        _airportDisplayMgr.Redraw( );
      }
    }

    // Update the mapCenter only via this method !!
    private void UpdateMapCenter( LatLon newCenter )
    {
      // if the center is not in the same Quad_15 as the current one, change
      if (newCenter.AsQuad( 15 ) != _mapCenterDyn.AsQuad( 15 )) {
        _mapCenterDyn = newCenter;
        OnMapCenterChanged( _mapCenterDyn );
        // may be something needs to be rendered
        RenderStatic( );
      }
    }

    // center the map on the aircraft
    private void CenterOnAcft( )
    {
      if (_trackedAircraft.Position.IsEmpty) return;

      UpdateMapCenter( _trackedAircraft.Position );
      StartMapLoading( _mapCenterDyn );
    }

    // start loading of a Map, this will trigger Canvas_LoadComplete events
    private void StartMapLoading( LatLon centerLatLon )
    {
      Debug.WriteLine( $"UC_Map.StartMapLoading- Center: {centerLatLon}" );

      lblLoading.Visible = true;
      lblLoading.BringToFront( );

      _viewport.LoadMap( centerLatLon, _mapRangeHandler.ZoomLevel, MapManager.Instance.CurrentProvider );
      // need to (re)set the current Range
      _airportDisplayMgr.SetMapZoom( _mapRangeHandler.ZoomLevel );
    }

    // to update the View after loading events
    private void UpdateView( )
    {
      // need to set the current Range as it may have changed due to Provider change
      _airportDisplayMgr.SetMapZoom( _mapRangeHandler.ZoomLevel );
      UpdateMapCenter( _viewport.ViewCenterLatLon ); // update if it changed only when the Map was extended
      lblCopyright.Text = _viewport.Map.ProviderCopyright;
      // repaint the UC
      _airportDisplayMgr.Redraw( );
    }

    // Event triggered by the Map once image loading is complete
    private void Canvas_LoadComplete( object sender, LoadCompleteEventArgs e )
    {
      //      Debug.WriteLine( $"UC_Map.Canvas_LoadComplete- MatComplete: {e.MatrixComplete}  LoadFailed: {e.LoadFailed}" );
      if (e.MatrixComplete) {
        // complete - but may have failed tiles
        _eDispatch.HandleEvent( delegate {
          lblLoading.Visible = false; // hide the MapLoading label
        } );
        _eDispatch.HandleEvent( UpdateView );
      }
      else {
        // not yet complete
        if (e.LoadFailed) {
          // tile load failed
        }
        else {
          // tile load success
          _eDispatch.HandleEvent( UpdateView );
        }
      }
    }

    // event triggered when some Map loading starts 
    private void Canvas_MapLoading( object sender, EventArgs e )
    {
      _eDispatch.HandleEvent( delegate {
        lblLoading.Visible = true;
        lblLoading.BringToFront( );
      } );
    }

    private void _viewport_MapMoved( object sender, EventArgs e )
    {
      if (latLonField.Visible) {
        latLonField.Lat = _viewport.ViewCenterLatLon.Lat;
        latLonField.Lon = _viewport.ViewCenterLatLon.Lon;
        latLonField.SetMSA( MSALib.MSA.Msa_ft( _viewport.ViewCenterLatLon, true ) );
      }
    }


    #endregion

    #region MapRange Handling

    // Indicate the Range button which is active
    private void MapRangeButtonUpdate( MapRange mapRange )
    {
      foreach (var kv in _mrButtons) {
        kv.Value.ForeColor = _mrColorOFF;
      }
      _mrButtons[mapRange].ForeColor = _mrColorON;
    }

    // internal handling the the zoomLevel changes, called from MapRangeHandler
    private void SetZoomLevel( ushort zoomLevel )
    {
      MapRangeButtonUpdate( MapRangeHandler.RangeFromZoom( zoomLevel ) );

      // save current center
      var center = _viewport.ViewCenterLatLon;
      if (center.IsEmpty) return; // not a valid center (yet)

      StartMapLoading( center ); // load around dyncamic center
      // UpdateMapCenter( center ); // will render if needed  MMMMMMM
      OnMapRangeChanged( ); // signal to owner
    }

    #endregion

    /// <summary>
    /// cTor: for the control
    /// </summary>
    public UC_Map( )
    {
      InitializeComponent( );

      var stUP = this.GetStyle( ControlStyles.UserPaint );
      var stWMP = this.GetStyle( ControlStyles.AllPaintingInWmPaint );
      var stODB = this.GetStyle( ControlStyles.OptimizedDoubleBuffer );


      lblLoading.Visible = false;

      // a handler
      _eDispatch = new dNetBm98.Win.WinFormInvoker( this );
      // map range handler
      _mapRangeHandler = new MapRangeHandler( MapRange.Mid, SetZoomLevel );

      // load indexed access to MapRange Buttons
      _mrButtons.Add( MapRange.XFar, btRangeXFar );
      _mrButtons.Add( MapRange.FarFar, btRangeFarFar );
      _mrButtons.Add( MapRange.Far, btRangeFar );
      _mrButtons.Add( MapRange.Mid, btRangeMid );
      _mrButtons.Add( MapRange.Near, btRangeNear );
      _mrButtons.Add( MapRange.Close, btRangeClose );
      _mrColorOFF = btRangeFarFar.ForeColor; // default
      MapRangeButtonUpdate( _mapRangeHandler.MapRange );

      // toggle Show buttons 
      _decoBColorOFF = btTogAcftData.BackColor; // default

      // setup flowpanels for info lists
      _pnlRunways = new StripPanel( new Size( 550, 500 ), "Airport - Runways" ) { Location = new Point( 5, 50 ) }; // only X matters
      _pnlRunways.ItemClicked += RunwayLabel_Click;
      _pnlRunways.EmptyClicked += _pnlRunways_EmptyClicked;
      this.Controls.Add( _pnlRunways );
      this.Controls.SetChildIndex( _pnlRunways, this.Controls.Count - 4 );

      _pnlApproaches = new StripPanel( new Size( 290, 200 ), "Runway - Approaches" ) { Location = new Point( 5, 50 ) }; // only X matters
      _pnlApproaches.ItemClicked += ApproachLabel_Click;
      _pnlApproaches.EmptyClicked += _pnlApproaches_EmptyClicked;
      this.Controls.Add( _pnlApproaches );
      this.Controls.SetChildIndex( _pnlApproaches, this.Controls.Count - 4 );

      _pnlTower = new StripPanel( new Size( 550, 500 ), "Airport - Frequencies" ) { Location = new Point( 5, 50 ) }; // only X matters
      _pnlTower.EmptyClicked += _pnlTower_EmptyClicked;
      this.Controls.Add( _pnlTower );
      this.Controls.SetChildIndex( _pnlTower, this.Controls.Count - 4 );

      _pnlNavaids = new StripPanel( new Size( 550, 500 ), "Navaids in Range" ) { Location = new Point( 5, 50 ) };
      _pnlNavaids.EmptyClicked += _pnlNavaids_EmptyClicked;
      this.Controls.Add( _pnlNavaids );
      this.Controls.SetChildIndex( _pnlNavaids, this.Controls.Count - 4 );

      _pnlProviders = new StripPanel( new Size( 290, 360 ), "Map Providers", new Font( this.Font.FontFamily, 10f, FontStyle.Bold ) ) {
        Anchor = AnchorStyles.Top | AnchorStyles.Right,
        Location = new Point( btNavaids.Right - 290, 50 ),  // only X matters
      };
      _pnlProviders.ItemClicked += ProviderLabel_Click;
      this.Controls.Add( _pnlProviders );
      this.Controls.SetChildIndex( _pnlProviders, this.Controls.Count - 4 );

      flpAcftData.Visible = false;
      flpAcftData.Location = new Point( 5, lblAirport.Bottom + 5 );
      flpAcftData.AutoSize = true;

      latLonField.Visible = false;

      teleportField.Visible = false;
      teleportField.Altitude_ft = 2000;
      teleportField.TeleportPressed += TeleportField_TeleportPressed;

      vpProfile.Visible = false;

      _viewport = new VPort2( pbDrawing, _mapRangeHandler );
      _viewport.LoadComplete += Canvas_LoadComplete;
      _viewport.MapLoading += Canvas_MapLoading;
      _viewport.MapMoved += _viewport_MapMoved;

      // create dummies to have them defined
      _airportRef = Data.Airport.DummyAirport( new LatLon( 0, 0, 0 ) );
      _airportDisplayMgr = new DisplayListMgr( _viewport );
      // map manager IF
      _mapCreator = new MapCreator( );
      _mapCreator.Committed += _mapCreator_Commited;

      // synch units in Map Data display
      _altIsFeet = _unitIsImp;
      _speedIsKt = _unitIsImp;
      _vsIsFpm = _unitIsImp;

      // set deco off defaults
      ShowTrackedAircraft = false;
      ShowMapGrid = false;
      ShowAirportRange = false;
      ShowNavaids = false;
      ShowVFRMarks = false;
      ShowAptMarks = false;
      ShowOtherAircrafts = AcftAiDisplayMode.None;

      // all tooltips
      _toolTip = new ToolTip( );
      _toolTip.SetToolTip( btCenterApt, "MAP: Load the original Airport map" );
      _toolTip.SetToolTip( btCenterAircraft, "MAP: Load the map with the aircraft as center location" );
      _toolTip.SetToolTip( btTogBehavior, "MAP: Load the radar map with the aircraft as center location" );

      _toolTip.SetToolTip( btRangeXFar, "RANGE: Load a eXtra Far range map around the current center" );
      _toolTip.SetToolTip( btRangeFarFar, "RANGE: Load a Far, Far range map around the current center" );
      _toolTip.SetToolTip( btRangeFar, "RANGE: Load a Far range map around the current center" );
      _toolTip.SetToolTip( btRangeMid, "RANGE: Load a Medium range map around the current center" );
      _toolTip.SetToolTip( btRangeNear, "RANGE: Load a Near range map around the current center" );
      _toolTip.SetToolTip( btRangeClose, "RANGE: Load a Close range map around the current center" );

      _toolTip.SetToolTip( btZoomIn, "ZOOM: Zoom into the map" );
      _toolTip.SetToolTip( btZoomOut, "ZOOM: Zoom out of the map" );
      _toolTip.SetToolTip( btZoomNorm, "ZOOM: Reset to full size zoom at current range" );

      _toolTip.SetToolTip( btTower, "Toggle the Airport Tower Frequency List" );
      _toolTip.SetToolTip( btRunway, "Toggle the Airport Runway List" );
      _toolTip.SetToolTip( btNavaids, "Toggle the Navaids List" );
      _toolTip.SetToolTip( btMapProvider, "Toggle Map Provider List" );

      _toolTip.SetToolTip( btTogGrid, "Toggle the map Grid" );
      _toolTip.SetToolTip( btTogRings, "Toggle the airport distance rings" );
      _toolTip.SetToolTip( btTogAcftData, "Toggle tracked aircraft display" );
      _toolTip.SetToolTip( btTogNavaids, "Toggle navaids display" );
      _toolTip.SetToolTip( btTogVFR, "Toggle VFR marks display" );
      _toolTip.SetToolTip( btTogApt, "Toggle alternate Airport marks display" );
      _toolTip.SetToolTip( btTogAcftAi, "Toggle other aircraft display" );

    }

    private void UC_Map_Load( object sender, EventArgs e )
    {
      PopulateMapProviders( );
      pbDrawing.Dock = DockStyle.Fill;

      // init empty to have them properly located
      PopulateRunways( Data.Airport.DummyAirport( new LatLon( 0, 0 ) ) );
      PopulateFrequencies( Data.Airport.DummyAirport( new LatLon( 0, 0 ) ) );
      PopulateNavaids( new List<INavaid>( ) );

      UpdateCtxMenuText( );

      UpdateAircraft( new Data.TrackedAircraftCls( ) ); // dummy update
      UpdateAircraftsAI( new List<ITrackedAircraft>( ) { } ); // dummy update

      ChangeBehavior( MapBehavior.Map, _airportRef.Coordinate );
      _viewport.CenterMap( );

      // maintain a defined stack of controls (SendToBack seems not to work properly)
      //this.Controls.SetChildIndex( vpProfile, this.Controls.Count - 1-2 );
      //this.Controls.SetChildIndex( lblCopyright, this.Controls.Count - 1-1 );
      //this.Controls.SetChildIndex( pbDrawing, this.Controls.Count - 1 );
    }

    #region Button EventHandlers

    private void btRunway_Click( object sender, EventArgs e )
    {
      _pnlTower.Visible = false;
      _pnlNavaids.Visible = false;
      _pnlRunways.Visible = !_pnlRunways.Visible; // toggle
      _pnlApproaches.Visible = _pnlRunways.Visible;
    }

    private void btTower_Click( object sender, EventArgs e )
    {
      _pnlRunways.Visible = false;
      _pnlApproaches.Visible = false;
      _pnlNavaids.Visible = false;
      _pnlTower.Visible = !_pnlTower.Visible; // toggle
    }

    private void btNavaids_Click( object sender, EventArgs e )
    {
      _pnlRunways.Visible = false;
      _pnlApproaches.Visible = false;
      _pnlTower.Visible = false;
      if (_pnlNavaids.Visible == false) {
        // will get visible below
        PopulateNavaids( _navaidRef ); // recalc shown Navaids
      }
      _pnlNavaids.Visible = !_pnlNavaids.Visible; // toggle
    }

    private void btMapProvider_Click( object sender, EventArgs e )
    {
      _pnlProviders.Visible = !_pnlProviders.Visible;
      //flpProvider.Visible = !flpProvider.Visible; // toggle
      //if (flpProvider.Visible) { flpProvider.BringToFront( ); }
    }

    private void btRangeAuto_Click( object sender, EventArgs e )
    {
      AutoRange = !AutoRange; // toggle
    }

    private void btRangeXFar_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.XFar;
    }
    private void btRangeFarFar_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.FarFar;
    }

    private void btRangeFar_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.Far;
    }

    private void btRangeMid_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.Mid;
    }

    private void btRangeNear_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.Near;
    }

    private void btRangeClose_Click( object sender, EventArgs e )
    {
      MapRange = MapRange.Close;
    }

    private void btCenterApt_Click( object sender, EventArgs e )
    {
      if (_mapBehavior == MapBehavior.Radar) {
        ChangeBehavior( MapBehavior.Map, _airportRef.Coordinate );
      }
      OriginalMap( );
    }

    private void btCenterAircraft_Click( object sender, EventArgs e )
    {
      if (_mapBehavior == MapBehavior.Radar) {
        ChangeBehavior( MapBehavior.Map, _trackedAircraft.Position );
      }
      CenterOnAcft( );
    }

    private void btZoomNorm_Click( object sender, EventArgs e )
    {
      ZoomNorm( );
    }

    private void btZoomIn_Click( object sender, EventArgs e )
    {
      ZoomIn( );
    }

    private void btZoomOut_Click( object sender, EventArgs e )
    {
      ZoomOut( );
    }

    private void btTogAcftData_Click( object sender, EventArgs e )
    {
      ShowTrackedAircraft = !ShowTrackedAircraft;
    }

    private void btTogShowRoute_Click( object sender, EventArgs e )
    {
      ShowRoute = !ShowRoute;
    }

    private void btTogGrid_Click( object sender, EventArgs e )
    {
      ShowMapGrid = !ShowMapGrid;
    }

    private void btTogRings_Click( object sender, EventArgs e )
    {
      ShowAirportRange = !ShowAirportRange;
    }

    private void btTogNavaids_Click( object sender, EventArgs e )
    {
      ShowNavaids = !ShowNavaids;
    }

    private void btTogVFR_Click( object sender, EventArgs e )
    {
      ShowVFRMarks = !ShowVFRMarks;
    }

    private void btTogApt_Click( object sender, EventArgs e )
    {
      ShowAptMarks = !ShowAptMarks;
    }

    private void btTogAcftAi_Click( object sender, EventArgs e )
    {
      ShowOtherAircrafts = _airportDisplayMgr.NextDisplayMode( ShowOtherAircrafts ); // cycle through the modes
    }

    private void lblTHdg_Click( object sender, EventArgs e )
    {
      _hdgIsTrue = !_hdgIsTrue; // toggle
    }

    private void lblMTrk_Click( object sender, EventArgs e )
    {
      _trkIsTrue = !_trkIsTrue; // toggle
    }

    // toggles Imperial / SI alltogether
    private void lblAlt_Click( object sender, EventArgs e )
    {
      _unitIsImp = !_unitIsImp; // toggle
      _altIsFeet = _unitIsImp;
      _speedIsKt = _unitIsImp;
      _vsIsFpm = _unitIsImp;
    }

    // toggles Imperial / SI alltogether
    private void lblIAS_Click( object sender, EventArgs e )
    {
      _unitIsImp = !_unitIsImp; // toggle
      _altIsFeet = _unitIsImp;
      _speedIsKt = _unitIsImp;
      _vsIsFpm = _unitIsImp;
    }

    // toggles Imperial / SI alltogether
    private void lblGS_Click( object sender, EventArgs e )
    {
      _unitIsImp = !_unitIsImp; // toggle
      _altIsFeet = _unitIsImp;
      _speedIsKt = _unitIsImp;
      _vsIsFpm = _unitIsImp;
    }

    // toggles Imperial / SI alltogether
    private void lblVS_Click( object sender, EventArgs e )
    {
      _unitIsImp = !_unitIsImp; // toggle
      _altIsFeet = _unitIsImp;
      _speedIsKt = _unitIsImp;
      _vsIsFpm = _unitIsImp;
    }

    // toggles the Map / Radar mode
    private void btTogBehavior_Click( object sender, EventArgs e )
    {
      if (_mapBehavior == MapBehavior.Map) {
        ChangeBehavior( MapBehavior.Radar, _trackedAircraft.Position );
      }
    }

    #endregion

    #region Context Menu

    private void mnuCoord_Click( object sender, EventArgs e )
    {
      latLonField.Visible = !latLonField.Visible; // toggle
      if (latLonField.Visible) {
        latLonField.Lat = _viewport.ViewCenterLatLon.Lat;
        latLonField.Lon = _viewport.ViewCenterLatLon.Lon;
        latLonField.SetMSA( MSALib.MSA.Msa_ft( _viewport.ViewCenterLatLon, true ) );
      }
      UpdateCtxMenuText( );
    }

    private void mnuTeleport_Click( object sender, EventArgs e )
    {
      teleportField.Visible = !teleportField.Visible; // toggle
      UpdateCtxMenuText( );
    }

    private void TeleportField_TeleportPressed( object sender, EventArgs e )
    {
      OnTeleportAircraft( _viewport.ViewCenterLatLon, teleportField.AltMSL, teleportField.Altitude_ft );
    }

    private void mnuVProfile_Click( object sender, EventArgs e )
    {
      vpProfile.Visible = !vpProfile.Visible; // toggle
      UpdateCtxMenuText( );
    }

    private void UpdateCtxMenuText( )
    {
      mnuCoord.Checked = latLonField.Visible;
      mnuTeleport.Checked = teleportField.Visible;
      mnuVProfile.Checked = vpProfile.Visible;
    }

    #endregion

  }
}
