using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.Extensions;

using SC = SimConnectClient;
using bm98_Map;
using FSimFacilityIF;

namespace FShelf
{
  public partial class frmShelf : Form
  {
    // airport which was requested by the user
    private FSimFacilityIF.IAirport _airport = null;
    // registered obs ID for the aircraft update subscription
    private int _observerID = -1;
    private string _observerName = "SHELF_FORM";

    // flags a missing Facility database
    private bool _dbMissing = false;

    // data update tracker to allow to pace down the updates towards the user control
    private int _updates;
    // comm item with the Mapping user control
    private bm98_Map.Data.TrackedAircraftCls _tAircraft = new bm98_Map.Data.TrackedAircraftCls( );

    // default Airport Entry fore color
    private Color _txForeColorDefault;

    // METAR Provider
    private MetarLib.Metar _metar;

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;
    private Size _lastLiveSize;

    private PerfTracker _perfTracker = new PerfTracker();

    /// <summary>
    /// Set true to run in standalone mode
    /// </summary>
    public bool Standalone { get; private set; } = false;

    /// <summary>
    /// Departure Airport ICAO
    /// </summary>
    public string DEP_Airport {
      get => _dep_Airport;
      set {
        _dep_Airport = value;
        lblMetDep.Text = value; // maintain in METAR
        lblDEP.Text = value; // maintain in MAP
      }
    }
    private string _dep_Airport = "n.a.";
    /// <summary>
    /// Arrival Airport ICAO
    /// </summary>
    public string ARR_Airport {
      get => _arr_Airport;
      set {
        _arr_Airport = value;
        lblMetArr.Text = value; // maintain in METAR
        lblARR.Text = value; // maintain in MAP
      }
    }
    private string _arr_Airport = "n.a.";

    #region AppSettingUpdate

    // Needed only once to update the AppSettings concept
    public void UpdateSettings( string shelfSettings )
    {
      if (shelfSettings.Length > 0) if (AppSettings.Instance.ShelfFolder == "") AppSettings.Instance.ShelfFolder = shelfSettings;
      AppSettings.Instance.Save( );
    }

    #endregion

    /// <summary>
    /// Set the Shelf Folder to use
    /// </summary>
    /// <param name="folderName">A directory name</param>
    /// <returns>True if successful</returns>
    private bool SetShelfFolder( string folderName )
    {
      try {
        aShelf.SetShelfFolder( folderName );
        return true;
      }
      catch (Exception) {
        return false;
      }
    }

    /// <summary>
    /// Checks if a Point is visible on any screen
    /// </summary>
    /// <param name="point">The Location to check</param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Point point )
    {
      Screen[] screens = Screen.AllScreens;
      foreach (Screen screen in screens) {
        if (screen.WorkingArea.Contains( point )) {
          return true;
        }
      }
      return false;
    }

    private class RwyLenItem
    {
      public string Item { get; private set; }
      public float Length_m { get; private set; }
      public RwyLenItem( string item, float len_m )
      {
        Item = item;
        Length_m = len_m;
      }
      public override string ToString( )
      {
        return Item;
      }
    }

    private void InitRunwayCombo( ComboBox cx )
    {
      cx.Items.Clear( );

      cx.Items.Add( new RwyLenItem( "Any length", 0f ) );
      cx.Items.Add( new RwyLenItem( "  300 m ( 1 000 ft)", 300f ) );
      cx.Items.Add( new RwyLenItem( "  600 m ( 2 000 ft)", 600f ) );
      cx.Items.Add( new RwyLenItem( "  900 m ( 3 000 ft)", 900f ) );
      cx.Items.Add( new RwyLenItem( "1 200 m ( 4 000 ft)", 1200f ) );
      cx.Items.Add( new RwyLenItem( "1 500 m ( 5 000 ft)", 1500f ) );
      cx.Items.Add( new RwyLenItem( "1 800 m ( 6 000 ft)", 1800f ) );
      cx.Items.Add( new RwyLenItem( "2 100 m ( 7 000 ft)", 2100f ) );
      cx.Items.Add( new RwyLenItem( "2 400 m ( 8 000 ft)", 2400f ) );
      cx.Items.Add( new RwyLenItem( "3 000 m (10 000 ft)", 3000f ) );
      cx.Items.Add( new RwyLenItem( "4 000 m (13 000 ft)", 4000f ) );
    }

    private readonly string c_facDBmsg = "The Facility Database could not be found!\n\nPlease visit the QuickGuide, head for 'DataLoader' and proceed accordingly";
    private void CheckFacilityDB( )
    {
      if (_dbMissing) {
        _ = MessageBox.Show( c_facDBmsg, "Facility Database Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
      }
    }

    #region Form

    // FORM
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="instance">An instance name (use "" as default)</param>
    /// <param name="standalone">Standalone flag (defaults to false)</param>
    public frmShelf( string instance, bool standalone = false )
    {
      // the first thing to do
      Standalone = standalone;
      AppSettings.InitInstance( Folders.SettingsFile, instance );
      _dbMissing = !File.Exists( Folders.GenAptDBFile ); // facilities DB missing
      MapLib.MapManager.Instance.InitMapLib( Folders.UserFilePath ); // Init before anything else
      MapLib.MapManager.Instance.SetDiskCacheLocation( Folders.CachePath ); // Map cache location
      // ---------------

      InitializeComponent( );

      tab.Dock = DockStyle.Fill;
      aShelf.Dock = DockStyle.Fill;
      aMap.Dock = DockStyle.Fill;

      lblFacDBMissing.Visible = _dbMissing;

      _metar = new MetarLib.Metar( );
      _metar.MetarDataEvent += _metar_MetarDataEvent;
      // handle some Map Events
      aMap.MapCenterChanged += AMap_MapCenterChanged;
      aMap.MapRangeChanged += AMap_MapRangeChanged;

      InitRunwayCombo( comboCfgRunwayLength );

      // use another WindowFrame in standalone
      if (Standalone) {
        this.Text = "MSFS FlightBag";
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        // add datahook
        SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      }

    }

    // form is loaded to get visible
    private void frmShelf_Load( object sender, EventArgs e )
    {
      // Init GUI
      this.Size = AppSettings.Instance.ShelfSize;
      this.Location = AppSettings.Instance.ShelfLocation;
      if (!IsOnScreen( Location )) {
        Location = new Point( 20, 20 );
      }
      _lastLiveSize = Size;
      _lastLiveLocation = Location;

      cbxCfgPrettyMetar.Checked = AppSettings.Instance.PrettyMetar;
      rbKg.Checked = true;
      rbKLbs.Checked = AppSettings.Instance.WeightLbs; // will toggle if needed
      rtbNotes.Text = AppSettings.Instance.NotePadText;

      // set prev. used airports
      DEP_Airport = AppSettings.Instance.DepICAO;
      ARR_Airport = AppSettings.Instance.ArrICAO;
      // defaults in config from prev use as well (not updated through the Property above)
      txCfgDep.Text = DEP_Airport;
      txCfgArr.Text = ARR_Airport;

      _txForeColorDefault = txEntry.ForeColor;

      aMap.ShowMapGrid = AppSettings.Instance.MapGrid;
      aMap.ShowAirportRange = AppSettings.Instance.AirportRings;
      aMap.ShowNavaids = AppSettings.Instance.VorNdb;
      cbxIFRwaypoints.Checked = AppSettings.Instance.IFRwaypoints;
      cbxAcftTrack.Checked = AppSettings.Instance.AcftRange;
      cbxAcftRange.Checked = AppSettings.Instance.AcftTrack;
      aMap.ShowVFRMarks = AppSettings.Instance.VFRmarks;
      aMap.ShowAptMarks = AppSettings.Instance.AptMarks;
      aMap.ShowTrackedAircraft = AppSettings.Instance.AcftMark;

      try {// don't ever fail
        comboCfgRunwayLength.SelectedIndex = AppSettings.Instance.MinRwyLengthCombo;
      }
      catch { comboCfgRunwayLength.SelectedIndex = 0; }

      // standalone connection status line
      lblSimConnectedMap.BackColor = Color.Transparent;
      lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;

      // set shelffolder default Path (in config text and for real)
      if (string.IsNullOrWhiteSpace( AppSettings.Instance.ShelfFolder )) {
        txCfgShelfFolder.Text = @".\DemoBag";
      }
      else {
        txCfgShelfFolder.Text = AppSettings.Instance.ShelfFolder;
      }
      SetShelfFolder( txCfgShelfFolder.Text );

      // init Map Aircraft if connected
      _updates = 0;
      LatLon loc = new LatLon( 0, 0, 0 );
      if (SC.SimConnectClient.Instance.IsConnected) {
        loc.Lat = SC.SimConnectClient.Instance.AircraftTrackingModule.Lat;
        loc.Lon = SC.SimConnectClient.Instance.AircraftTrackingModule.Lon;
        loc.Altitude = SC.SimConnectClient.Instance.AircraftTrackingModule.AltMsl_ft;
      }

      // create the initial 'Airport' to have something to start with
      aMap.MapCreator.Reset( );
      aMap.MapCreator.SetAirport( aMap.MapCreator.DummyAirport( loc ) );
      aMap.MapCreator.Commit( );

      _perfTracker.Reset( );

      if (Standalone) {
        CheckFacilityDB( );
      }

      // Pacer interval 
      timer1.Interval = 1000;
      timer1.Enabled = true;
    }

    // form got attention
    private void frmShelf_Activated( object sender, EventArgs e )
    {
      this.TopMost = true;
      this.timer1.Enabled = true;
      // register DataUpdates if in shared mode and if not yet done 
      if (!Standalone && SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
        _observerID = SC.SimConnectClient.Instance.AircraftTrackingModule.AddObserver( _observerName, OnDataArrival );
      }
    }

    // track last known location while visible
    private void frmShelf_LocationChanged( object sender, EventArgs e )
    {
      if (this.Visible)
        _lastLiveLocation = this.Location;
    }

    // track last known size while visible
    private void tabShelf_SizeChanged( object sender, EventArgs e )
    {
      if (this.Visible)
        _lastLiveSize = this.Size;
    }

    // about to close the form
    private void frmShelf_FormClosing( object sender, FormClosingEventArgs e )
    {
      this.TopMost = false;
      timer1.Enabled = false;

      // UnRegister DataUpdates
      if (SC.SimConnectClient.Instance.IsConnected && (_observerID >= 0))
        SC.SimConnectClient.Instance.AircraftTrackingModule.RemoveObserver( _observerID );
      _observerID = -1;

      // save last known good form location and size
      if (this.Visible && this.WindowState == FormWindowState.Normal) {
        AppSettings.Instance.ShelfLocation = this.Location;
        AppSettings.Instance.ShelfSize = this.Size;
      }
      else {
        AppSettings.Instance.ShelfLocation = _lastLiveLocation;
        AppSettings.Instance.ShelfSize = _lastLiveSize;
      }
      // save last known config and Airport settings for the next start
      AppSettings.Instance.PrettyMetar = cbxCfgPrettyMetar.Checked;
      AppSettings.Instance.WeightLbs = rbKLbs.Checked;
      AppSettings.Instance.NotePadText = rtbNotes.Text;
      AppSettings.Instance.IFRwaypoints = cbxIFRwaypoints.Checked;
      AppSettings.Instance.MinRwyLengthCombo = comboCfgRunwayLength.SelectedIndex;
      AppSettings.Instance.DepICAO = DEP_Airport;
      AppSettings.Instance.ArrICAO = ARR_Airport;
      AppSettings.Instance.MapGrid = aMap.ShowMapGrid;
      AppSettings.Instance.AirportRings = aMap.ShowAirportRange;
      AppSettings.Instance.VorNdb = aMap.ShowNavaids;
      AppSettings.Instance.VFRmarks = aMap.ShowVFRMarks;
      AppSettings.Instance.AptMarks = aMap.ShowAptMarks;
      AppSettings.Instance.AcftMark = aMap.ShowTrackedAircraft;
      AppSettings.Instance.AcftRange = cbxAcftRange.Checked;
      AppSettings.Instance.AcftTrack = cbxAcftTrack.Checked;
      //--
      AppSettings.Instance.Save( );

      if (Standalone) {
        // don't cancel if standalone (else how to close it..)
        this.WindowState = FormWindowState.Minimized;
      }
      else {
        if (e.CloseReason == CloseReason.UserClosing) {
          // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
          e.Cancel = true;
          this.Hide( );
        }
      }
    }

    #endregion

    /// <summary>
    /// Timer Event
    /// </summary>
    private void timer1_Tick( object sender, EventArgs e )
    {
      // register DataUpdates if in shared mode and if not yet done 
      if (!Standalone && SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
        _observerID = SC.SimConnectClient.Instance.AircraftTrackingModule.AddObserver( _observerName, OnDataArrival );
      }
      // Call SimConnect if needed and Standalone
      if (Standalone && --_simConnectTrigger <= 0) {
        SimConnectPacer( );
      }
      if (tab.SelectedTab == tabPerf) {
        SetPerfContent( );
      }
    }

    #region Tab Events

    private void tab_SelectedIndexChanged( object sender, EventArgs e )
    {
      // update when the tab switches to MAP (possible changes from Config)
      if (tab.SelectedTab == tabMap) {
        _tAircraft.ShowAircraftRange = cbxAcftRange.Checked;
        _tAircraft.ShowAircraftTrack = cbxAcftTrack.Checked;
        var rwLen = comboCfgRunwayLength.SelectedItem as RwyLenItem;
        // also update Navaids and Alt Airports (we were disconnected for an unknown time)

        // sanity
        if (_dbMissing) {
          ; // cannot get facilities
        }
        else {
          aMap.SetNavaidList( NavaidList( aMap.MapCenter( ) ) );
          aMap.SetAltAirportList( AltAirportList( aMap.MapCenter( ), rwLen.Length_m ) );
        }
        aMap.RenderItems( ); // will update if there is a need for it
      }
      else if (tab.SelectedTab == tabPerf) {
        SetPerfContent( );
      }
    }

    #endregion

    #region Map User Control Handling

    // loads a list of Navaids which are surounding the latLon
    private List<INavaid> NavaidList( LatLon latLon )
    {
      var nList = new List<INavaid>( );
      // VOR / NDB
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile ))
          return nList;

        // get the the Quads around
        var qs = Quad.Around49EX( latLon.AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        nList = _db.DbReader.Navaids_ByQuadList( qs ).ToList( );
      }

      // APT IFR Waypoints if set in Config
      if (_airport != null && cbxIFRwaypoints.Checked) {
        nList.AddRange( _airport.Navaids.Where( x => x.IsWaypoint ) );
      }

      return nList;
    }

    // get alternate airports from DB 
    private List<IAirportDesc> AltAirportList( LatLon latLon, float minRwyLength )
    {
      var aList = new List<IAirportDesc>( );
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile ))
          return aList; // no db available

        // get the the Quads around
        var qs = Quad.Around49EX( latLon.AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        if (minRwyLength <= 1) {
          // short if no length is selected
          aList = _db.DbReader.AirportDescs_ByQuadList( qs ).ToList( );
        }
        else {
          aList = _db.DbReader.AirportDescs_ByQuadList( qs ).ToList( );
          // select the ones asked for in Config (min length)
          aList = aList.Where( x => x.LongestRwyLength_m >= minRwyLength ).ToList( );
        }
      }
      return aList;
    }

    // fires when the Map Center has changed from the Map interaction or airport change
    private void AMap_MapCenterChanged( object sender, MapEventArgs e )
    {
      //Console.WriteLine( $"{e.CenterCoordinate}" );

      // sanity
      if (_dbMissing) return; // cannot get facilities

      aMap.SetNavaidList( NavaidList( e.CenterCoordinate ) );
      var rwLen = comboCfgRunwayLength.SelectedItem as RwyLenItem;
      aMap.SetAltAirportList( AltAirportList( e.CenterCoordinate, rwLen.Length_m ) );
    }

    // fires when the Map Range has changed
    private void AMap_MapRangeChanged( object sender, MapEventArgs e )
    {
      // no action (so far)
    }

    // retrieve an airport from the DB
    private void SetAirport( string aptICAO )
    {
      // sanity
      if (_dbMissing) {
        _airport = null; // cannot get facilities
        return;
      }

      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile )) {
          txEntry.ForeColor = Color.Red; // clear the one not available
          txEntry.Text = "No DB";
          _airport = null; // no db available
        }
        _airport = _db.DbReader.GetAirport( aptICAO ); ;
      }
    }

    // try to load an airport from the Database into the Map
    private void LoadAirport( string aptICAO )
    {
      SetAirport( aptICAO );
      if (_airport != null) {
        txEntry.ForeColor = _txForeColorDefault; // clear the one not available
        aMap.MapCreator.Reset( );
        aMap.MapCreator.SetAirport( _airport );
        aMap.MapCreator.Commit( );
      }
      else {
        txEntry.ForeColor = Color.Red; // clear the one not available
      }
    }

    // Departure Label clicked
    private void lblDEP_Click( object sender, EventArgs e )
    {
      LoadAirport( lblDEP.Text );
    }

    // Arrival Label clicked
    private void lblARR_Click( object sender, EventArgs e )
    {
      LoadAirport( lblARR.Text );
    }

    // Airport entry button clicked
    private void btGetAirport_Click( object sender, EventArgs e )
    {
      LoadAirport( txEntry.Text.Trim( ) );
    }

    // shortcut on Enter
    private void txEntry_KeyPress( object sender, KeyPressEventArgs e )
    {
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        btGetAirport_Click( sender, e );
      }
    }

    /// <summary>
    /// Handle Data Arrival from Sim
    /// </summary>
    /// <param name="refName">Data Reference Name that called the update</param>
    public void OnDataArrival( string refName )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return; // cannot..

      // track landing performance
      _perfTracker.Update( );

      var simData = SC.SimConnectClient.Instance.AircraftTrackingModule;
      // TODO: the next two selectors will cause loosing tracking of the Acft, may be update it anyway
      if (!this.Visible) return;  // no need to update while the shelf is not visible ???? TODO decide if or if not cut reporting ????
      //if (!(tab.SelectedTab == tabMap)) return;  //don't update while not showing the MapTab - this causes track disruptions when in METAR etc

      // update pace slowed down to an acceptable CPU load - (native is 200ms)
      if ((_updates++ % 3) == 0) { // every third.. 600ms pace - slow enough ?? performance penalty...
        _tAircraft.OnGround = simData.Sim_OnGround;
        _tAircraft.TrueHeading = simData.HDG_true_deg;
        _tAircraft.Position = new LatLon( simData.Lat, simData.Lon );
        _tAircraft.Altitude_ft = simData.AltMsl_ft;
        _tAircraft.RadioAlt_ft = simData.Sim_OnGround ? float.NaN
                                  : (simData.AltAoG_ft <= 1500) ? simData.AltAoG_ft : float.NaN; // limit RA visible to 1500 ft if not on ground
        _tAircraft.Ias_kt = simData.IAS_kt;
        _tAircraft.Vs_fpm = (int)(simData.VS_ftPmin / 20) * 20; // 20 fpm steps only
        _tAircraft.Gs_kt = simData.GS;
        // GPS does not provide meaningful track values when not moving
        _tAircraft.Trk_deg = simData.Sim_OnGround ? float.NaN : simData.GTRK;
        _tAircraft.TrueTrk_deg = simData.Sim_OnGround ? float.NaN : simData.GTRK_true;
        // update the map
        aMap.UpdateAircraft( _tAircraft );
      }
    }

    #endregion

    #region Metar Events

    private void _metar_MetarDataEvent( object sender, MetarLib.MetarTafDataEventArgs e )
    {
      if (!e.MetarTafData.Valid) {
        rtbMetar.Text += "No Report available\n";
        return;
      }
      var metar = e.MetarTafData.FirstOrDefault( );

      if (metar != null) {
        rtbMetar.Text += e.MetarTafData.FirstOrDefault( ).RAW + "\n";
        rtbMetar.Text += (cbxCfgPrettyMetar.Checked ? e.MetarTafData.FirstOrDefault( ).Pretty + "\n" : "");
      }
    }

    // clear the text area
    private void btMetClear_Click( object sender, EventArgs e )
    {
      rtbMetar.Text = "";
    }

    // Dept airport request
    private void btMetDep_Click( object sender, EventArgs e )
    {
      rtbMetar.Text += $"Request - {lblMetDep.Text}:\n";
      _metar.PostMETAR_Request( lblMetDep.Text );
    }

    // Arr airport request
    private void btMetArr_Click( object sender, EventArgs e )
    {
      rtbMetar.Text += $"Request - {lblMetArr.Text}:\n";
      _metar.PostMETAR_Request( lblMetArr.Text );
    }

    // other airport request
    private void btMetApt_Click( object sender, EventArgs e )
    {
      rtbMetar.Text += $"Request - {txMetApt.Text}:\n";
      _metar.PostMETAR_Request( txMetApt.Text );
    }
    private void txMetApt_KeyPress( object sender, KeyPressEventArgs e )
    {
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        btMetApt_Click( sender, e );
      }
    }

    // aircraft position request
    private void btMetAcft_Click( object sender, EventArgs e )
    {
      rtbMetar.Text += $"Request - {_tAircraft.Position}:\n";
      _metar.PostMETAR_Request( _tAircraft.Position, _tAircraft.TrueHeading );
    }

    #endregion

    #region Performance Setup
    // fill the Perf Tab
    private void SetPerfContent( )
    {
      rtbPerf.Clear( );
      if (!SC.SimConnectClient.Instance.IsConnected) {
        rtbPerf.Text += $"Not connected - no data available";
        return;
      }
      var ds = SC.SimConnectClient.Instance.AircraftTrackingModule;
      bool lbs = rbKLbs.Checked;
      string unit = lbs ? "lbs x 1000" : "kg x 1000";
      float value;
      var RTF = new RTFformatter( );
      RTF.SetTab( 4000 ); RTF.SetTab( 5500 );
      RTF.RBold = true;
      RTF.RColor = RTFformatter.ERColor.ERC_Blue;
      RTF.FontSize( 15 );
      RTF.WriteLn( $"{ds.AcftID}  -  {ds.AcftConfigFile}" );

      RTF.RColor = RTFformatter.ERColor.ERC_Black;
      RTF.FontSize( 14 );
      RTF.WriteLn( );
      RTF.Write( $"Weight and Balance:" ); RTF.WriteTab( $"{unit}" ); RTF.WriteLn( );
      RTF.RBold = false;
      value = lbs ? ds.TotalAcftWeight_lbs / 1_000f : Conversions.Kg_From_Lbs( ds.TotalAcftWeight_lbs ) / 1_000f;
      RTF.Write( $"TOTAL Weight" ); RTF.WriteTab( $"{value:##0.000}\n" ); RTF.WriteLn( );
      value = lbs ? ds.FuelQuantityTotal_lb / 1_000f : Conversions.Kg_From_Lbs( ds.FuelQuantityTotal_lb ) / 1_000f;
      RTF.Write( $"FUEL Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteTab( $"{ds.FuelQuantityTotal_gal:##0.0} gal" ); RTF.WriteLn( );
      value = lbs ? ds.AcftPLS_weight_lbs / 1_000f : Conversions.Kg_From_Lbs( ds.AcftPLS_weight_lbs ) / 1_000f;
      RTF.Write( $"PAYLOAD Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      value = lbs ? (ds.TotalAcftWeight_lbs - ds.FuelQuantityTotal_lb) / 1_000f : Conversions.Kg_From_Lbs( ds.TotalAcftWeight_lbs - ds.FuelQuantityTotal_lb ) / 1_000f;
      RTF.Write( $"ZF Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      RTF.Write( $"CG lon/lat" ); RTF.WriteTab( $"{ds.AcftCGlong_perc * 100f:#0.00} %  /  {ds.AcftCGlat_perc * 100f:#0.00} %" ); RTF.WriteLn( );
      value = lbs ? ds.EmptyAcftWeight_lbs / 1_000f : Conversions.Kg_From_Lbs( ds.EmptyAcftWeight_lbs ) / 1_000f;
      RTF.Write( $"Empty Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      value = lbs ? ds.MaxAcftWeight_lbs / 1_000f : Conversions.Kg_From_Lbs( ds.MaxAcftWeight_lbs ) / 1_000f;
      RTF.Write( $"MAX Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );

      RTF.WriteLn( );
      RTF.RBold = true;
      RTF.Write( $"Last Touchdown Data:" ); RTF.WriteLn( );
      RTF.RBold = false;
      RTF.Write( $"Vertical" ); RTF.WriteTab( $"{_perfTracker.Rate_fpm:#,##0} fpm" ); RTF.WriteLn( );
      RTF.Write( $"Pitch" ); RTF.WriteTab( $"{_perfTracker.Pitch_deg:##0.0}°" ); RTF.WriteLn( );
      RTF.Write( $"Bank" ); RTF.WriteTab( $"{_perfTracker.Bank_deg:##0.0}°" ); RTF.WriteLn( );

      RTF.WriteLn( );
      RTF.RBold = true;
      RTF.Write( $"Design Data:" ); RTF.WriteLn( );
      RTF.RBold = false;
      RTF.Write( $"Cruise Altitude" ); RTF.WriteTab( $"{ds.DesingCruiseAlt_ft,6:##,##0} ft" ); RTF.WriteLn( );
      RTF.Write( $"Est. Cruise Speed" ); RTF.WriteTab( $"{ds.CruiseSpeedEst_kt,8:##,##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vc  Cruise Speed" ); RTF.WriteTab( $"{ds.DesingSpeedVC_kt,8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vy  Climb Speed" ); RTF.WriteTab( $"{ds.DesingSpeedClimb_kt,8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vmu Takeoff Speed" ); RTF.WriteTab( $"{ds.DesingSpeedTakeoff_kt,8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vr  Min Rotation" ); RTF.WriteTab( $"{ds.DesingSpeedMinRotation_kt,8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vs1 Stall Speed" ); RTF.WriteTab( $"{ds.DesingSpeedVS1_kt,8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vs0 Stall Speed" ); RTF.WriteTab( $"{ds.DesingSpeedVS0_kt,8:##0} kt" ); RTF.WriteLn( );

      rtbPerf.Rtf = RTF.RTFtext;
    }

    private void btPerfRefresh_Click( object sender, EventArgs e )
    {
      SetPerfContent( );
    }

    private void rbKg_CheckedChanged( object sender, EventArgs e )
    {
      SetPerfContent( );
    }

    private void rbKLbs_CheckedChanged( object sender, EventArgs e )
    {
      SetPerfContent( );
    }

    #endregion

    #region Notes Events

    private void btNotesClear_Click( object sender, EventArgs e )
    {
      rtbNotes.Clear( );
    }

    #endregion

    #region Config Events

    // DEP Enter Pressed
    private void txCfgDep_KeyPress( object sender, KeyPressEventArgs e )
    {
      txCfgDep.ForeColor = _txForeColorDefault; // clear the one not available
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        SetAirport( txCfgDep.Text );
        if (_airport == null) {
          txCfgDep.ForeColor = Color.Red;
          this.DEP_Airport = "n.a.";
          lblCfgDep.Text = this.DEP_Airport;
        }
        else {
          txCfgDep.ForeColor = Color.GreenYellow;
          this.DEP_Airport = txCfgDep.Text;
          lblCfgDep.Text = _airport.Name;
        }
      }
    }

    // ARR Enter Pressed
    private void txCfgArr_KeyPress( object sender, KeyPressEventArgs e )
    {
      txCfgArr.ForeColor = _txForeColorDefault; // clear the one not available
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        SetAirport( txCfgArr.Text );
        if (_airport == null) {
          txCfgArr.ForeColor = Color.Red;
          this.ARR_Airport = "n.a.";
          lblCfgArr.Text = this.ARR_Airport;
        }
        else {
          txCfgArr.ForeColor = Color.GreenYellow;
          this.ARR_Airport = txCfgArr.Text;
          lblCfgArr.Text = _airport.Name;
        }
      }
    }

    // Folder Button pressed
    private void btCfgSelFolder_Click( object sender, EventArgs e )
    {
      if (Directory.Exists( txCfgShelfFolder.Text )) {
        FBD.SelectedPath = Path.GetFullPath( txCfgShelfFolder.Text );
      }
      else {
        FBD.SelectedPath = "";
        FBD.RootFolder = Environment.SpecialFolder.Desktop;
      }

      if (FBD.ShowDialog( this ) == DialogResult.OK) {
        // save in AppSettings and make it live
        txCfgShelfFolder.Text = FBD.SelectedPath;
        AppSettings.Instance.ShelfFolder = txCfgShelfFolder.Text;
        AppSettings.Instance.Save( );
        SetShelfFolder( txCfgShelfFolder.Text );
      }
    }

    #endregion

    #region SimConnectClient chores

    // Monitor the Sim Event Handler after Connection
    private bool m_awaitingEvent = true; // cleared in the Sim Event Handler
    private int m_scGracePeriod = -1;    // grace period count down
    private int _simConnectTrigger = 0; //  count down to call the SimConnect Pacer

    /// <summary>
    /// fired from Sim for new Data
    /// Callback from SimConnect client signalling data arrival
    ///  Appart from subscriptions this is calles on a regular pace 
    /// </summary>
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      m_awaitingEvent = false; // confirm we've got data events
    }

    /// <summary>
    /// Toggle the connection
    /// if not connected: Try to connect and setup facilities
    /// if connected:     Disconnect facilities and shut 
    /// </summary>
    private void SimConnect( )
    {
      // only needed in a standalone environment
      if (!Standalone) return;

      //LOG.Log( $"SimConnect: Start" );
      lblSimConnectedMap.BackColor = Color.Transparent;
      lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
      if (SC.SimConnectClient.Instance.IsConnected) {
        // Disconnect from Input and SimConnect
        //        SetupInGameHook( false );

        // Unregister DataUpdates if not done 
        if (_observerID >= 0) {
          SC.SimConnectClient.Instance.AircraftTrackingModule.RemoveObserver( _observerID );
          _observerID = -1;
        }
        SC.SimConnectClient.Instance.Disconnect( );
        lblSimConnectedMap.BackColor = Color.DarkRed;
        lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
        //        LOG.Log( $"SimConnect: Disconnected now" );
      }

      else {
        // setup the event monitor before connecting (will be handled in the Timer Event)
        m_awaitingEvent = true;
        m_scGracePeriod = 3; // about 3*5 secs to get an event

        // try to connect
        if (SC.SimConnectClient.Instance.Connect( false )) {

          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.AircraftTrackingModule.SimRate_rate;
          lblSimConnectedMap.BackColor = Color.MediumPurple;
          lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
        }
        else {
          // connect failed - will be retried through the pacer
          lblSimConnectedMap.BackColor = Color.DarkRed;
          lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
        }
      }

    }

    /// <summary>
    /// SimConnect chores on a timer, mostly reconnecting and monitoring the connection status
    /// Intender to be called about every 5 seconds
    /// </summary>
    private void SimConnectPacer( )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        // handle the situation where Sim is connected but could not hookup to events
        // Happens when HudBar is running when the Sim is starting only.
        // Sometimes the Connection is made but was not hooking up to the event handling
        // Disconnect and try to reconnect 
        if (m_awaitingEvent || SC.SimConnectClient.Instance.AircraftTrackingModule.SimRate_rate <= 0) {
          // No events seen so far
          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.AircraftTrackingModule.SimRate_rate;

          if (m_scGracePeriod <= 0) {
            // grace period is expired !
            //            LOG.Log( "SimConnectPacer: Did not receive an Event for 5sec - Restarting Connection" );
            SimConnect( ); // Disconnect if we don't receive Events even the Sim is connected
          }
          m_scGracePeriod--;
        }
        else {
          lblSimConnectedMap.BackColor = Color.DarkGreen;
          lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
          // register DataUpdates if not done 
          if (SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
            _observerID = SC.SimConnectClient.Instance.AircraftTrackingModule.AddObserver( _observerName, OnDataArrival );
          }
        }
      }
      else {
        // If not connected try again
        SimConnect( );
      }

      // reset calling interval
      _simConnectTrigger = 5000 / timer1.Interval;
    }



    #endregion

  }
}
