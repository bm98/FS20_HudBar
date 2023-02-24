using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;
using static dNetBm98.XColor;
using CoordLib;
using bm98_Map.Drawing;
using bm98_Map.Data;
using bm98_Map.UI;
using MapLib;
using FSimFacilityIF;
using System.Diagnostics;
using MapLib.Tiles;

namespace bm98_Map
{


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
    private dNetBm98.WinFormInvoker _eDispatch;

    // The Viewport for this Map
    private VPort2 _viewport;
    // our TT instance
    private ToolTip _toolTip;

    // Map Creator Toolings
    private MapCreator _mapCreator;

    // Holding a ref to the commited airport here
    private Airport _airportRef;
    // maintains all the visuals of the Airport
    private DisplayListMgr _airportDisplayMgr;

    // internal Aircraft Data Tracking obj
    private TrackedAircraft _aircraftTrack = new TrackedAircraft( );

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

    private Dictionary<MapRange, Button> _mrButtons = new Dictionary<MapRange, Button>( );
    private readonly Color _mrColorOFF;
    private readonly Color _mrColorON = Color.Yellow;

    private readonly Color _decoBColorOFF;
    private readonly Color _decoBColorON = Color.LimeGreen;

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
    private StripPanel _pnlRunways;
    private StripPanel _pnlApproaches;
    private StripPanel _pnlNavaids;
    private StripPanel _pnlTower;


    #region User Control API

    /// <summary>
    /// Fired when the center of the map has changed
    /// </summary>
    [Category( "Map" )]
    [Description( "The Map Center Tile has changed" )]
    public event EventHandler<MapEventArgs> MapCenterChanged;
    /// <summary>
    /// Fired when the map range has changed
    /// </summary>
    [Category( "Map" )]
    [Description( "The Map Range has changed" )]
    public event EventHandler<MapEventArgs> MapRangeChanged;

    private void OnMapCenterChanged( LatLon center )
    {
      MapCenterChanged?.Invoke( this, new MapEventArgs( center, _mapRangeHandler.MapRange, _mapRangeHandler.ZoomLevel ) );
    }
    private void OnMapRangeChanged( )
    {
      MapRangeChanged?.Invoke( this, new MapEventArgs( _viewport.Map.CenterCoord, _mapRangeHandler.MapRange, _mapRangeHandler.ZoomLevel ) );
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
      set => _mapRangeHandler.SetMapRange( value );
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
    /// Update the tracked Aircrafts position and shown properties
    /// submit a NaN if the items shall be hidden
    /// </summary>
    /// <param name="trackedAircraft">The tracked aircraft properties</param>
    public void UpdateAircraft( ITrackedAircraft trackedAircraft )
    {
      // update our internal _aircraftTracker from the delivered one
      _aircraftTrack.Update( trackedAircraft );

      // Aircraft Labels, hide if the value is float.NaN (ex Heading)
      // data label are 4 chars, number fields 6 chars to align vert. !!
      lblTHdg.Visible = true;
      if (_hdgIsTrue) { lblTHdg.Text = $"THDG: {_aircraftTrack.TrueHeading_deg,6:000}°"; }
      else { lblTHdg.Text = $"HDG : {_aircraftTrack.Heading_degm,6:000}°M"; }

      lblMTrk.Visible = _aircraftTrack.ShowMTRK;
      if (_trkIsTrue) { lblMTrk.Text = $"TTRK: {_aircraftTrack.TrueTrk_deg,6:000}°"; }
      else { lblMTrk.Text = $"TRK : {_aircraftTrack.Trk_degm,6:000}°M"; }

      lblAlt.Visible = _aircraftTrack.ShowAlt;
      if (_altIsFeet) { lblAlt.Text = $"AMSL: {_aircraftTrack.Altitude_ft,6:##,##0} ft"; }
      else { lblAlt.Text = $"AMSL: {M_From_Ft( _aircraftTrack.Altitude_ft ),6:##,##0} m"; }

      lblRA.Visible = _aircraftTrack.ShowRA;
      if (_altIsFeet) { lblRA.Text = $"RA  : {_aircraftTrack.RadioAlt_ft,6:##,##0} ft"; }
      else { lblRA.Text = $"RA  : {M_From_Ft( _aircraftTrack.RadioAlt_ft ),6:##,##0} m"; }

      lblIAS.Visible = _aircraftTrack.ShowIas;
      if (_speedIsKt) { lblIAS.Text = $"IAS : {_aircraftTrack.Ias_kt,6:#,##0} kt"; }
      else { lblIAS.Text = $"IAS : {Kmh_From_Kt( _aircraftTrack.Ias_kt ),6:#,##0} km/h"; }

      lblGS.Visible = _aircraftTrack.ShowGs;
      if (_speedIsKt) { lblGS.Text = $"GS  : {_aircraftTrack.Gs_kt,6:#,##0} kt"; }
      else { lblGS.Text = $"GS  : {Kmh_From_Kt( _aircraftTrack.Gs_kt ),6:#,##0} km/h"; }

      lblVS.Visible = _aircraftTrack.ShowVs;
      if (_vsIsFpm) { lblVS.Text = $"V/S : {_aircraftTrack.Vs_fpm,6:+#,##0;-#,##0;---} fpm"; }
      else { lblVS.Text = $"V/S : {Mps_From_Ftpm( _aircraftTrack.Vs_fpm ),6:+#0.0;-#0.0;---} m/s"; }

      // set windspeed string for the Sprite if not default
      if (_speedIsKt) { } // default
      else { _aircraftTrack.WindSpeedS = $"{Mps_From_Kt( _aircraftTrack.WindSpeed_kt ):#0.0}m/s"; }

      // Aircraft Drawing update goes via the AirportDisplayManager object
      _airportDisplayMgr.UpdateAircraft( _aircraftTrack );
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
    public void SetNavaidList( List<FSimFacilityIF.INavaid> navaids )
    {
      PopulateNavaids( navaids );
      _airportDisplayMgr.SetNavaidList( navaids );
      // Trigger Update the View
      _renderStaticNeeded = true;
    }

    /// <summary>
    /// To set the shown Navaids
    /// Overwrites the existing list
    /// </summary>
    /// <param name="airports">List of airports to show</param>
    public void SetAltAirportList( List<FSimFacilityIF.IAirportDesc> airports )
    {
      _airportDisplayMgr.SetAltAirportList( airports );
      // Trigger Update the View
      _renderStaticNeeded = true;
    }

    /// <summary>
    /// To set the Route plotted on the Map
    /// </summary>
    /// <param name="route">A route Obj</param>
    public void SetRoute( Route route )
    {
      _airportDisplayMgr.SetRoute( route );
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
    /// Returns the current Map Center
    /// </summary>
    /// <returns>A LatLon</returns>
    public LatLon MapCenter( ) => _mapCenterDyn;

    #endregion

    #region Airport Panel Updates

    // Load Airport Label
    private void PopulateApt( Airport airport )
    {
      var iata = $"({airport.IATA})";
      lblAirport.Text = $"{airport.ICAO,-4} {iata,-6}   {airport.Name}\n"
        + $"{Dms.ToLat( airport.Lat, "dm", 0 )} {Dms.ToLon( airport.Lon, "dm", 0 )}   {airport.Elevation_ft:####0} ft ({airport.Elevation_m:###0} m)";
    }

    #region Tower Panel

    // Load Frequencies Panel
    private void PopulateFrequencies( Airport airport )
    {
      _pnlTower.InitUpdate( );
      _pnlTower.ClearItems( );

      if (airport.HasCommsRelation) {
        foreach (var frq in airport.Comms) {
          _pnlTower.AddItem( frq.CommString( ).PadRight( 63 ), null, false );
        }
      }
      _pnlTower.CommitUpdate( this.ClientRectangle.Bottom - lblCopyright.Height );
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
        var rwyWyps = _airportRef.Navaids.Where( x => x.IsApproach && x.Runway_Ident == $"RW{runway.Ident}" );
        // all runway-Approaches (ILS, RNAV etc)
        var rwyApproaches = rwyWyps.Select( y => y.ApproachName ).Distinct( ).OrderBy( x => x );
        foreach (var appName in rwyApproaches) {
          var fix = _airportRef.Navaids.Where(
          x => x.IsApproach
          && x.Runway_Ident == $"RW{runway.Ident}"
          && x.ApproachName == appName ).FirstOrDefault( );
          string ngTag = ((fix.Fix != null) && fix.Fix.EndsWith( "NG" )) ? "'" : " ";
          _pnlApproaches.AddItem( $"{appName,-7} {$"({fix.ICAO})",-8} RWY {runway.Ident}  {ngTag}".PadRight( 30 ), appName, true );
        }
      }
      _pnlApproaches.CommitUpdate( _pnlRunways.Top );
    }

    // Fill the runways panel from an airport
    private void PopulateRunways( Data.Airport airport )
    {
      var padLen = 65;
      _pnlRunways.InitUpdate( );
      _pnlRunways.ClearItems( );

      // all runways
      if (airport.HasRunwaysRelation) {
        foreach (var rwy in airport.Runways) {
          _pnlRunways.AddItem( rwy.RunwayString( ).PadRight( padLen ), rwy, true ); // make is selectable
        }
      }

      // Airport Navaids - if there are any
      var aptNavaids = new List<INavaid>( );
      if (airport.HasNavaidsRelation) {
        aptNavaids = airport.Navaids.Where( x => x.IsVOR || x.IsNDB ).ToList( );
      }
      // if there are any apt navaids, list them
      if (aptNavaids.Count( ) > 0) {
        _pnlRunways.AddSubTitle( "Airport - Navaids" );
        var itemList = new List<string>( ); // manage doubles
        foreach (var nav in aptNavaids) {
          var item = nav.VorNdbNameString( );
          if (!itemList.Contains( item )) {
            _pnlRunways.AddItem( item.PadRight( padLen ), null, false );
            itemList.Add( item );
          }
        }
      }
      _pnlRunways.CommitUpdate( this.ClientRectangle.Bottom - lblCopyright.Height );
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
      }
      else {
        var approachName = label.Tag as string;
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( approachName );
      }
      // render and redraw
      RenderStatic( true );
    }

    // a strip in the runway panel was clicked
    private void RunwayLabel_Click( object sender, EventArgs e )
    {
      // sanity
      if (!(sender is Label)) return;
      _pnlApproaches.Visible = false;
      var label = sender as Label;
      if (!(label.Tag is IRunway)) {
        // clear selected runway
        //ShowVFRMarks = false;
        _airportDisplayMgr.SetRunwayVFRDispItems( null ); // unselected
        PopulateRunwayApproaches( null ); // unselected
        _airportDisplayMgr.SetSelectedNavIdRunway( "" );
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( "" );
      }
      else {
        // get this pair
        var pair = (label.Tag as IRunway).RunwayPair( _airportRef.Runways );
        _airportDisplayMgr.SetRunwayVFRDispItems( pair );
        // IFR Waypoint
        _airportDisplayMgr.SetSelectedNavIdRunway( pair.First( ).Ident );
        _airportDisplayMgr.SetSelectedNavIdRunwayApproach( "" );
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
    private void PopulateNavaids( List<FSimFacilityIF.INavaid> navaids )
    {
      _pnlNavaids.InitUpdate( );
      _pnlNavaids.ClearItems( );

      navaids.Sort( );
      foreach (var nav in navaids) {
        if (nav.IsWaypoint)
          continue; // skip waypoints
        _pnlNavaids.AddItem( nav.VorNdbNameString( ).PadRight( 63 ), null, false );
      }
      _pnlNavaids.CommitUpdate( this.ClientRectangle.Bottom - lblCopyright.Height );
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

      // clear all controls from the FLP
      while (flpProvider.Controls.Count > 0) {
        var cx = flpProvider.Controls[0];
        flpProvider.Controls.Remove( cx );
        cx.Dispose( );
      }

      var f1 = flpProvider.ForeColor.Dimmed( 60 );
      var f2 = flpProvider.ForeColor;
      int num = 0;
      foreach (var p in MapManager.Instance.EnabledProviders) {
        var label = new Label( ) {
          AutoSize = true,
          Text = MapManager.Instance.ProviderName( p ),
          Tag = p,
          ForeColor = (num++ % 2 == 0) ? f1 : f2,
        };
        label.MouseClick += Provider_MouseClick;
        flpProvider.Controls.Add( label );
      }
      flpProvider.Cursor = Cursors.Hand;
      flpProvider.BringToFront( );

    }

    // handle provider was clicked
    private void Provider_MouseClick( object sender, MouseEventArgs e )
    {
      var lbl = sender as Label;
      MapProvider mapProvider = (MapProvider)lbl.Tag;
      if (mapProvider != MapProvider.DummyProvider) {
        // change provider
        MapManager.Instance.SetNewProvider( mapProvider );
        StartMapLoading( _mapCenterDyn );
        flpProvider.Visible = false;
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
      _aircraftTrack.TargetAltitude_ft = _airportRef.Elevation_ft;

      // some fiddling with airport and extended handling 
      UpdateMapCenter( _airportRef.Coordinate );
      // load the Map
      StartMapLoading( _airportRef.Coordinate );

      PopulateApt( _airportRef );
      PopulateRunways( _airportRef );
      PopulateFrequencies( _airportRef );

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
      if (newCenter != _mapCenterDyn) {
        _mapCenterDyn = newCenter;
        OnMapCenterChanged( _mapCenterDyn );
        // may be something needs to be rendered
        RenderStatic( );
      }
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

    #endregion

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
      _eDispatch = new dNetBm98.WinFormInvoker( this );
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

      _pnlApproaches = new StripPanel( new Size( 290, 150 ), "Runway - Approaches" ) { Location = new Point( 5, 50 ) }; // only X matters
      _pnlApproaches.ItemClicked += ApproachLabel_Click;
      _pnlApproaches.EmptyClicked += _pnlApproaches_EmptyClicked;
      this.Controls.Add( _pnlApproaches );

      _pnlTower = new StripPanel( new Size( 550, 500 ), "Airport - Frequencies" ) { Location = new Point( 5, 50 ) }; // only X matters
      _pnlTower.EmptyClicked += _pnlTower_EmptyClicked;
      this.Controls.Add( _pnlTower );

      _pnlNavaids = new StripPanel( new Size( 550, 500 ), "Area - Navaids" ) { Location = new Point( 5, 50 ) };// only X matters
      _pnlNavaids.EmptyClicked += _pnlNavaids_EmptyClicked;
      this.Controls.Add( _pnlNavaids );

      flpAcftData.Visible = false;
      flpAcftData.Location = new Point( 5, lblAirport.Bottom + 5 );
      flpAcftData.AutoSize = true;

      flpProvider.Visible = false;
      flpProvider.AutoSize = true;
      flpProvider.Top = btMapProvider.Bottom + 5;
      _viewport = new VPort2( pbDrawing, _mapRangeHandler );
      _viewport.LoadComplete += Canvas_LoadComplete;
      _viewport.MapLoading += Canvas_MapLoading;

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

      // all tooltips
      _toolTip = new ToolTip( );
      _toolTip.SetToolTip( btCenterApt, "MAP: Load the original Airport map" );
      _toolTip.SetToolTip( btCenterAircraft, "MAP: Load the map with the aircraft as center location" );

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

    }

    private void UC_Map_Load( object sender, EventArgs e )
    {
      PopulateMapProviders( );
      pbDrawing.Dock = DockStyle.Fill;
      lblCopyright.SendToBack( );
      pbDrawing.SendToBack( );
      // init empty to have them properly located
      PopulateRunways( Data.Airport.DummyAirport( new LatLon( 0, 0 ) ) );
      PopulateFrequencies( Data.Airport.DummyAirport( new LatLon( 0, 0 ) ) );
      PopulateNavaids( new List<INavaid>( ) );

      UpdateAircraft( new Data.TrackedAircraftCls( ) ); // dummy update

      _viewport.CenterMap( );

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
      _pnlNavaids.Visible = !_pnlNavaids.Visible; // toggle
    }

    private void btMapProvider_Click( object sender, EventArgs e )
    {
      flpProvider.Visible = !flpProvider.Visible; // toggle
      if (flpProvider.Visible) { flpProvider.BringToFront( ); }
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
      OriginalMap( );
    }

    private void btCenterAircraft_Click( object sender, EventArgs e )
    {
      if (_aircraftTrack.Position.IsEmpty) return;

      UpdateMapCenter( _aircraftTrack.Position );
      StartMapLoading( _mapCenterDyn );
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

    #endregion

  }
}
