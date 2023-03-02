using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
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
using bm98_Map.Data;
using FSimFacilityIF;
using FShelf.Profiles;

using FlightplanLib;
using FlightplanLib.SimBrief;
using FlightplanLib.MSFSPln;
using FlightplanLib.MSFSFlt;

using Point = System.Drawing.Point;
using Route = bm98_Map.Data.Route;
using FShelf.FPlans;

namespace FShelf
{
  /// <summary>
  /// FlightBag (Shelf) Form
  /// </summary>
  public partial class frmShelf : Form
  {
    // airport which was requested by the user
    private FSimFacilityIF.IAirport _airport = null;
    // registered obs ID for the aircraft update subscription
    private int _observerID = -1;
    private const string _observerName = "SHELF_FORM";

    // flags a missing Facility database
    private readonly bool _dbMissing = false;

    // data update tracker to allow to pace down the updates towards the user control
    private int _updates;
    // comm item with the Mapping user control
    private readonly TrackedAircraftCls _tAircraft = new TrackedAircraftCls( );

    // default Airport Entry fore color
    private Color _txForeColorDefault;

    // METAR Provider
    private readonly MetarLib.Metar _metar;

    // Plan Wrapper
    private readonly FpWrapper _flightPlan = new FpWrapper( ); // only one instance, don't null it !!

    // SimBrief Provider
    private readonly SimBrief _simBrief;

    // MSFS Pln Provider
    private readonly MSFSPln _msfsPln;
    private readonly MSFSFlt _msfsFlt;
    private string _selectedPlanFile = "";
    private bool _awaitingFLTfile = false; // true when FLT is requested

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;
    private Size _lastLiveSize;

    private readonly PerfTracker _perfTracker = new PerfTracker( );

    // Profiles
    private readonly ProfileCat _profileCat = new ProfileCat( );
    private readonly BindingSource _profileBinding = new BindingSource( );
    private readonly BindingSource _vsRateBinding = new BindingSource( );
    private readonly BindingSource _altBinding = new BindingSource( );
    private static readonly DataGridViewCellStyle _vCellStyle
      = new DataGridViewCellStyle( ) { Alignment = DataGridViewContentAlignment.MiddleRight, BackColor = Color.Gainsboro, SelectionBackColor = Color.CornflowerBlue };
    private static readonly DataGridViewCellStyle _vCellStyleMarked
      = new DataGridViewCellStyle( _vCellStyle ) { BackColor = Color.MediumSpringGreen, SelectionBackColor = Color.CadetBlue };

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
    /// <summary>
    /// Settings update from .Net to SettingsLib
    /// </summary>
    /// <param name="shelfSettings">The old shelf settings</param>
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

    // translate the SimBrief OFP to a Route obj for Map Use
    private static Route GetRouteFromPlan( FlightPlan plan )
    {
      var route = new Route( );
      if (!plan.IsValid) return route;

      foreach (var fix in plan.Waypoints) {
        route.AddRoutePoint( new RoutePoint( fix.Wyp_Ident7, fix.LatLonAlt_ft, fix.WaypointType, fix.InboundTrueTrk, fix.OutboundTrueTrk, fix.IsSIDorSTAR ) );
      }
      route.RecalcTrack( ); // as we have no Out Tracks
      return route;
    }

    private static void DebSaveRouteString( string content, string ext )
    {
      var fName = $".\\LastPlanDownload.{ext}";
#if DEBUG
      fName = $".\\LastPlanDownload_{DateTime.Now:s}.{ext}".Replace( ":", "_" );
#endif
      // shall never fail...
      try {
        // save to current Dir while in debug
        var fname = Path.Combine( Folders.UserFilePath, fName );
        // Write UTF8 with BOM
        using (var sw = new StreamWriter( fname, false, new UTF8Encoding( true ) )) {
          sw.WriteLine( content );
        }
      }
      catch { }
    }

    // Helper for Runways
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

    // setup the profile tab's data grids
    private void InitProfileData( )
    {
      DataGridView dgv;
      // profile DataView
      dgv = dgvProfile;
      dgv.AutoGenerateColumns = true;
      dgv.Columns.Clear( );
      _profileBinding.DataSource = _profileCat.ProfileValueSet;
      _profileBinding.DataMember = ProfileCat.ProfileTable;
      dgv.DataSource = _profileBinding;
      for (int i = 0; i < dgv.Columns.Count; i++) {
        dgv.Columns[i].HeaderText = _profileCat.ProfileColumnCaption( ProfileCat.ProfileTable, i ); // DGV uses Col Name not Caption from the data set..
        dgv.Columns[i].DefaultCellStyle = _vCellStyle;
        dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.AutoResizeColumn( i );
      }
      dgv.Columns[0].Visible = false;

      // VS target DataView
      dgv = dgvRate;
      dgv.AutoGenerateColumns = true;
      dgv.Columns.Clear( );
      _vsRateBinding.DataSource = _profileCat.ProfileValueSet;
      _vsRateBinding.DataMember = ProfileCat.TgtVsTable;
      dgv.DataSource = _vsRateBinding;
      for (int i = 0; i < dgv.Columns.Count; i++) {
        dgv.Columns[i].HeaderText = _profileCat.ProfileColumnCaption( ProfileCat.TgtVsTable, i ); // DGV uses Col Name not Caption from the data set..
        dgv.Columns[i].DefaultCellStyle = _vCellStyle;
        dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.AutoResizeColumn( i );
      }
      dgv.Columns[0].Visible = false;

      // Dist for Altitude DataView
      dgv = dgvAlt;
      dgv.AutoGenerateColumns = true;
      dgv.Columns.Clear( );
      _altBinding.DataSource = _profileCat.ProfileValueSet;
      _altBinding.DataMember = ProfileCat.Dist4AltTable;
      dgv.DataSource = _altBinding;
      for (int i = 0; i < dgv.Columns.Count; i++) {
        dgv.Columns[i].HeaderText = _profileCat.ProfileColumnCaption( ProfileCat.Dist4AltTable, i ); // DGV uses Col Name not Caption from the data set..
        dgv.Columns[i].DefaultCellStyle = _vCellStyle;
        dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.AutoResizeColumn( i );
      }
      dgv.Columns[0].Visible = false;
    }


    // marks the given GS entry in the TargetVS Table, clearAll=true will unmark the marked one (independent of GS)
    private int _gsMarkIndex = -1;
    private void MarkGS( float gs, bool clearAll = false )
    {
      int newIndex = _profileCat.GsRowIndex( gs );
      if (clearAll) {
        // clear old mark
        if (_gsMarkIndex >= 0) dgvRate.Rows[_gsMarkIndex].Cells[1].Style = _vCellStyle;
        _gsMarkIndex = -1;
      }
      else if (newIndex != _gsMarkIndex) {
        // clear old mark
        if (_gsMarkIndex >= 0) dgvRate.Rows[_gsMarkIndex].Cells[1].Style = _vCellStyle;
        _gsMarkIndex = newIndex;
        dgvRate.Rows[_gsMarkIndex].Cells[1].Style = _vCellStyleMarked;
      }
    }

    // marks the given Alt entry in the Alt Table, clearAll=true will unmark the marked one (independent of Alt)
    private int _altMarkIndex = -1;
    private void MarkAlt( float alt, bool clearAll = false )
    {
      int newIndex = _profileCat.AltRowIndex( alt );
      if (clearAll) {
        // clear old mark
        if (_altMarkIndex >= 0) dgvAlt.Rows[_altMarkIndex].Cells[1].Style = _vCellStyle;
        _altMarkIndex = -1;
      }
      else if (newIndex != _altMarkIndex) {
        // clear old mark
        if (_altMarkIndex >= 0) dgvAlt.Rows[_altMarkIndex].Cells[1].Style = _vCellStyle;
        _altMarkIndex = newIndex;
        dgvAlt.Rows[_altMarkIndex].Cells[1].Style = _vCellStyleMarked;
      }
    }

    // marks the given Profile Deg entry in the Profile Table, clearAll=true will unmark the marked one (independent of Deg)
    private int _fpaMarkIndex = -1;
    private void MarkFPA( float alt, bool clearAll = false )
    {
      int newIndex = _profileCat.FpaRowIndex( alt );
      if (newIndex < 0) { clearAll = true; } // small deg- remove mark only

      if (clearAll) {
        // clear old mark
        if (_fpaMarkIndex >= 0) dgvProfile.Rows[_fpaMarkIndex].Cells[1].Style = _vCellStyle;
        _fpaMarkIndex = -1;
      }
      else if (newIndex != _fpaMarkIndex) {
        // clear old mark
        if (_fpaMarkIndex >= 0) dgvProfile.Rows[_fpaMarkIndex].Cells[1].Style = _vCellStyle;
        _fpaMarkIndex = newIndex;
        dgvProfile.Rows[_fpaMarkIndex].Cells[1].Style = _vCellStyleMarked;
      }
    }



    // facility check & message
    private readonly string c_facDBmsg = "The Facility Database could not be found!\n\nPlease visit the QuickGuide, head for 'DataLoader' and proceed accordingly";
    private void CheckFacilityDB( )
    {
      if (_dbMissing) {
        _ = MessageBox.Show( c_facDBmsg, "Facility Database Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
      }
    }

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

      _simBrief = new SimBrief( );
      _simBrief.SimBriefDataEvent += _simBrief_SimBriefDataEvent;
      _simBrief.SimBriefDownloadEvent += _simBrief_SimBriefDownloadEvent;

      _msfsPln = new MSFSPln( );
      _msfsPln.MSFSPlnDataEvent += _msfsPln_MSFSPlnDataEvent;
      _msfsFlt = new MSFSFlt( );
      _msfsFlt.MSFSFltDataEvent += _msfsFlt_MSFSFltDataEvent;

      // handle some Map Events
      aMap.MapCenterChanged += AMap_MapCenterChanged;
      aMap.MapRangeChanged += AMap_MapRangeChanged;

      // attach FLT save event
      SC.SimConnectClient.Instance.FltSave += Instance_FltSave;

      InitRunwayCombo( comboCfgRunwayLength );

      // use another WindowFrame in standalone
      if (Standalone) {
        this.Text = "MSFS FlightBag";
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        // start FP Module disabled
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false;
        SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode = FSimClientIF.FlightPlanMode.Disabled;
        // add datahook to receive connection updates
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

      // map settings
      aMap.ShowMapGrid = AppSettings.Instance.MapGrid;
      aMap.ShowAirportRange = AppSettings.Instance.AirportRings;
      aMap.ShowRoute = AppSettings.Instance.FlightplanRoute;
      aMap.ShowNavaids = AppSettings.Instance.VorNdb;
      aMap.ShowVFRMarks = AppSettings.Instance.VFRmarks;
      aMap.ShowAptMarks = AppSettings.Instance.AptMarks;
      aMap.ShowTrackedAircraft = AppSettings.Instance.AcftMark;
      aMap.AutoRange = AppSettings.Instance.AutoRange;
      // config settings
      cbxCfgAcftRange.Checked = AppSettings.Instance.AcftRange;
      cbxCfgAcftWind.Checked = AppSettings.Instance.AcftWind;
      cbxCfgAcftTrack.Checked = AppSettings.Instance.AcftTrack;

      txCfgSbPilotID.Text = AppSettings.Instance.SbPilotID;

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

      // profiles
      InitProfileData( );

      // standalone handling
      if (Standalone) {
        // File Access Check
        if (DbgLib.Dbg.Instance.AccessCheck( Folders.UserFilePath ) != DbgLib.AccessCheckResult.Success) {
          string msg = $"MyDocuments Folder Access Check Failed:\n{DbgLib.Dbg.Instance.AccessCheckResult}\n\n{DbgLib.Dbg.Instance.AccessCheckMessage}";
          MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
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
      AppSettings.Instance.WeightLbs = rbKLbs.Checked;
      AppSettings.Instance.NotePadText = rtbNotes.Text;
      AppSettings.Instance.MinRwyLengthCombo = comboCfgRunwayLength.SelectedIndex;
      AppSettings.Instance.DepICAO = DEP_Airport;
      AppSettings.Instance.ArrICAO = ARR_Airport;
      // map settings
      AppSettings.Instance.MapGrid = aMap.ShowMapGrid;
      AppSettings.Instance.AirportRings = aMap.ShowAirportRange;
      AppSettings.Instance.FlightplanRoute = aMap.ShowRoute;
      AppSettings.Instance.VorNdb = aMap.ShowNavaids;
      AppSettings.Instance.VFRmarks = aMap.ShowVFRMarks;
      AppSettings.Instance.AptMarks = aMap.ShowAptMarks;
      AppSettings.Instance.AcftMark = aMap.ShowTrackedAircraft;
      AppSettings.Instance.AutoRange = aMap.AutoRange;
      // config settings
      AppSettings.Instance.PrettyMetar = cbxCfgPrettyMetar.Checked;
      AppSettings.Instance.AcftRange = cbxCfgAcftRange.Checked;
      AppSettings.Instance.AcftWind = cbxCfgAcftWind.Checked;
      AppSettings.Instance.AcftTrack = cbxCfgAcftTrack.Checked;
      AppSettings.Instance.SbPilotID = txCfgSbPilotID.Text;
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

    #region Tab Events

    private void tab_SelectedIndexChanged( object sender, EventArgs e )
    {
      // update when the tab switches to MAP (possible changes from Config)
      if (tab.SelectedTab == tabMap) {
        _tAircraft.ShowAircraftRange = cbxCfgAcftRange.Checked;
        _tAircraft.ShowAircraftWind = cbxCfgAcftWind.Checked;
        _tAircraft.ShowAircraftTrack = cbxCfgAcftTrack.Checked;
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

      // APT Approach Waypoints, VORs and NDBs if set in Config
      if (_airport != null /* && cbxCfgIFRwaypoints.Checked */) {
        nList.AddRange( _airport.Navaids.Where( x => x.IsApproach ) );
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
    // provde the static map decorations for the new center coordinate
    private void AMap_MapCenterChanged( object sender, MapEventArgs e )
    {
      //Console.WriteLine( $"{e.CenterCoordinate}" );

      // sanity
      if (_dbMissing) return; // cannot get facilities

      aMap.SetNavaidList( NavaidList( e.CenterCoordinate ) );
      var rwLen = comboCfgRunwayLength.SelectedItem as RwyLenItem;
      aMap.SetAltAirportList( AltAirportList( e.CenterCoordinate, rwLen.Length_m ) );

      // (re)set the route from the plan in use
      aMap.SetRoute( GetRouteFromPlan( _flightPlan.FlightPlan ) );
    }

    // fires when the Map Range has changed
    private void AMap_MapRangeChanged( object sender, MapEventArgs e )
    {
      // no action (so far)
    }

    // return an airport from the DB or null
    private IAirport GetAirport( string aptICAO )
    {
      // sanity
      if (_dbMissing) return null;

      IAirport airport = null;
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (_db.Open( Folders.GenAptDBFile )) {
          airport = _db.DbReader.GetAirport( aptICAO ); ;
        }
      }
      return airport;
    }

    // try to load an airport from the Database into the Map
    private void LoadAirport( )
    {
      if (_airport != null) {
        aMap.MapCreator.Reset( );
        aMap.MapCreator.SetAirport( _airport );
        aMap.MapCreator.Commit( );
      }
    }

    // Departure Label clicked
    private void lblDEP_Click( object sender, EventArgs e )
    {
      var apt = GetAirport( lblDEP.Text );
      if (apt != null) {
        _airport = apt;
        LoadAirport( );
      }
    }

    // Arrival Label clicked
    private void lblARR_Click( object sender, EventArgs e )
    {
      var apt = GetAirport( lblARR.Text );
      if (apt != null) {
        _airport = apt;
        LoadAirport( );
      }
    }

    // Airport entry button clicked
    private void btGetAirport_Click( object sender, EventArgs e )
    {
      var apt = GetAirport( txEntry.Text.Trim( ) );
      if (apt == null) {
        txEntry.ForeColor = Color.Red; // clear the one not available
        txEntry.Text = "n.a.";
        // don't change the _airport when nothing is found
      }
      else {
        txEntry.ForeColor = _txForeColorDefault; // clear the one not available
        _airport = apt;
        LoadAirport( );
      }
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
        _tAircraft.TrueHeading_deg = simData.HDG_true_deg;
        _tAircraft.Heading_degm = simData.HDG_mag_degm;
        _tAircraft.Position = new LatLon( simData.Lat, simData.Lon );
        _tAircraft.Altitude_ft = simData.AltMsl_ft;
        _tAircraft.RadioAlt_ft = simData.Sim_OnGround ? float.NaN
                                  : (simData.AltAoG_ft <= 1500) ? simData.AltAoG_ft : float.NaN; // limit RA visible to 1500 ft if not on ground
        _tAircraft.Ias_kt = simData.IAS_kt;
        _tAircraft.Tas_kt = simData.TAS_kt;
        _tAircraft.Gs_kt = simData.GS;
        _tAircraft.Vs_fpm = (int)(simData.VS_ftPmin / 20) * 20; // 20 fpm steps only
        _tAircraft.Fpa_deg = simData.FlightPathAngle_deg;
        // GPS does not provide meaningful track values when not moving
        _tAircraft.Trk_degm = simData.Sim_OnGround ? float.NaN : simData.GTRK;
        _tAircraft.TrueTrk_deg = simData.Sim_OnGround ? float.NaN : simData.GTRK_true;

        _tAircraft.WindSpeed_kt = simData.WindSpeed_kt;
        _tAircraft.WindDirection_deg = simData.WindDirection_deg;

        // update the map
        aMap.UpdateAircraft( _tAircraft );
        // Update Profile page
        UpdateProfileData( );
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
      _metar.PostMETAR_Request( _tAircraft.Position, _tAircraft.TrueHeading_deg );
    }

    #endregion

    #region SimBrief Events

    // triggered on SimBrief data arrival
    private void _simBrief_SimBriefDataEvent( object sender, SimBriefDataEventArgs e )
    {
      DebSaveRouteString( e.SimBriefData, "json" );

      if (!e.Success) {
        lblCfgSbPlanData.Text = "No data received";
        return;
      }

      lblCfgSbPlanData.Text = "OFP data received";
      var js = e.SimBriefData;
      _flightPlan.LoadSbPlan( js );
      if (_flightPlan.IsSbPlan) {
        // populate CFG fields
        lblCfgSbPlanData.Text = $"OFP: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
        lblCfgMsPlanData.Text = "..."; // clear the MS one

        // preselect airports
        txCfgDep.Text = _flightPlan.FlightPlan.Origin.Icao_Ident.ICAO;
        var apt = GetAirport( txCfgDep.Text );
        if (apt == null) {
          txCfgDep.ForeColor = Color.Red;
          this.DEP_Airport = "n.a.";
          lblCfgDep.Text = this.DEP_Airport;
        }
        else {
          txCfgDep.ForeColor = Color.GreenYellow;
          this.DEP_Airport = txCfgDep.Text;
          lblCfgDep.Text = apt.Name;
        }

        txCfgArr.Text = _flightPlan.FlightPlan.Destination.Icao_Ident.ICAO;
        apt = GetAirport( txCfgArr.Text );
        if (apt == null) {
          txCfgArr.ForeColor = Color.Red;
          this.ARR_Airport = "n.a.";
          lblCfgArr.Text = this.ARR_Airport;
        }
        else {
          txCfgArr.ForeColor = Color.GreenYellow;
          this.ARR_Airport = txCfgArr.Text;
          lblCfgArr.Text = apt.Name;
        }

        // Set Map Route
        aMap.SetRoute( GetRouteFromPlan( _flightPlan.FlightPlan ) );
        // Load Shelf Docs
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgSbPlanData.Text = err;

      }
      else {
        lblCfgSbPlanData.Text = "No OFP received";
      }
    }


    // triggered when a DL is completed
    private void _simBrief_SimBriefDownloadEvent( object sender, EventArgs e )
    {
      ; // just a ping
    }

    #endregion

    #region MSFS Pln Events

    private void _msfsPln_MSFSPlnDataEvent( object sender, MSFSPlnDataEventArgs e )
    {
      DebSaveRouteString( e.MSFSPlnData, "PLN" );

      lblCfgMsPlanData.Text = "PLN data received";
      var xs = e.MSFSPlnData;
      _flightPlan.LoadMsFsPLN( xs );
      if (_flightPlan.IsMsFsPLN) {
        // save selected file in settings 
        if (!string.IsNullOrEmpty( _selectedPlanFile )) {
          AppSettings.Instance.LastMsfsPlan = _selectedPlanFile;
        }

        // populate CFG fields
        lblCfgMsPlanData.Text = $"PLN: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
        lblCfgSbPlanData.Text = "..."; // clear the SB one

        // preselect airports
        txCfgDep.Text = _flightPlan.FlightPlan.Origin.Icao_Ident.ICAO;
        var apt = GetAirport( txCfgDep.Text );
        if (apt == null) {
          txCfgDep.ForeColor = Color.Red;
          this.DEP_Airport = "n.a.";
          lblCfgDep.Text = this.DEP_Airport;
        }
        else {
          txCfgDep.ForeColor = Color.GreenYellow;
          this.DEP_Airport = txCfgDep.Text;
          lblCfgDep.Text = apt.Name;
        }

        txCfgArr.Text = _flightPlan.FlightPlan.Destination.Icao_Ident.ICAO;
        apt = GetAirport( txCfgArr.Text );
        if (apt == null) {
          txCfgArr.ForeColor = Color.Red;
          this.ARR_Airport = "n.a.";
          lblCfgArr.Text = this.ARR_Airport;
        }
        else {
          txCfgArr.ForeColor = Color.GreenYellow;
          this.ARR_Airport = txCfgArr.Text;
          lblCfgArr.Text = apt.Name;
        }

        // Set Map Route
        aMap.SetRoute( GetRouteFromPlan( _flightPlan.FlightPlan ) );
        // Load Shelf Docs
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
    }

    #endregion

    #region NSFS Flt Events

    private void _msfsFlt_MSFSFltDataEvent( object sender, MSFSFltDataEventArgs e )
    {
      DebSaveRouteString( e.MSFSFltData, "FLT" );

      lblCfgMsPlanData.Text = "FLT data received";
      var ins = e.MSFSFltData;
      _flightPlan.LoadMsFsFLT( ins );
      if (_flightPlan.IsMsFsPLN) {
        // save selected file in settings 
        if (!string.IsNullOrEmpty( _selectedPlanFile )) {
          AppSettings.Instance.LastMsfsPlan = _selectedPlanFile;
        }

        // populate CFG fields
        lblCfgMsPlanData.Text = $"FLT: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
        lblCfgSbPlanData.Text = "..."; // clear the SB one

        // preselect airports
        if (_flightPlan.FlightPlan.Origin.IsValid) {
          txCfgDep.Text = _flightPlan.FlightPlan.Origin.Icao_Ident.ICAO;
          var apt = GetAirport( txCfgDep.Text );
          if (apt == null) {
            txCfgDep.ForeColor = Color.Red;
            this.DEP_Airport = "n.a.";
            lblCfgDep.Text = this.DEP_Airport;
          }
          else {
            txCfgDep.ForeColor = Color.GreenYellow;
            this.DEP_Airport = txCfgDep.Text;
            lblCfgDep.Text = apt.Name;
          }
        }

        if (_flightPlan.FlightPlan.Destination.IsValid) {
          txCfgArr.Text = _flightPlan.FlightPlan.Destination.Icao_Ident.ICAO;
          var apt = GetAirport( txCfgArr.Text );
          if (apt == null) {
            txCfgArr.ForeColor = Color.Red;
            this.ARR_Airport = "n.a.";
            lblCfgArr.Text = this.ARR_Airport;
          }
          else {
            txCfgArr.ForeColor = Color.GreenYellow;
            this.ARR_Airport = txCfgArr.Text;
            lblCfgArr.Text = apt.Name;
          }
        }

        // Set Map Route
        aMap.SetRoute( GetRouteFromPlan( _flightPlan.FlightPlan ) );
        // Load Shelf Docs
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
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

    #region Profile Events

    // update the profile data
    private void UpdateProfileData( )
    {
      if (tab.SelectedTab != tabProfile) return; // Tab not shown, omit updates

      lblGS.Text = $"{_tAircraft.Gs_kt:##0}";
      lblVS.Text = $"{_tAircraft.Vs_fpm:#,##0}";
      lblAlt.Text = $"{_tAircraft.Altitude_ft:##,##0}";
      lblIAS.Text = $"{_tAircraft.Ias_kt:##0}";
      lblTAS.Text = $"{_tAircraft.Tas_kt:##0}";
      lblFPA.Text = $"{_tAircraft.Fpa_deg:#0.0}";
      // mark the Row entries
      MarkGS( _tAircraft.Gs_kt );
      MarkAlt( _tAircraft.Altitude_ft );
      MarkFPA( _tAircraft.Fpa_deg );
    }

    // profile changed
    private void dgvProfile_SelectionChanged( object sender, EventArgs e )
    {
      // sanity
      if (dgvProfile.SelectedRows.Count <= 0) return;

      var selRow = dgvProfile.SelectedRows[0];
      _profileCat.SetSelectedProfile( selRow );
    }

    // alt1 selection changed
    private void dgvAlt1_SelectionChanged( object sender, EventArgs e )
    {
      // sanity
      if (dgvAlt.SelectedRows.Count <= 0) return;

      _profileCat.SetStartAltitude( dgvAlt.SelectedRows[0] );
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
        var apt = GetAirport( txCfgDep.Text );
        if (apt == null) {
          txCfgDep.ForeColor = Color.Red;
          this.DEP_Airport = "n.a.";
          lblCfgDep.Text = this.DEP_Airport;
        }
        else {
          txCfgDep.ForeColor = Color.GreenYellow;
          this.DEP_Airport = txCfgDep.Text;
          lblCfgDep.Text = apt.Name;
        }
      }
    }

    // ARR Enter Pressed
    private void txCfgArr_KeyPress( object sender, KeyPressEventArgs e )
    {
      txCfgArr.ForeColor = _txForeColorDefault; // clear the one not available
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        var apt = GetAirport( txCfgArr.Text );
        if (apt == null) {
          txCfgArr.ForeColor = Color.Red;
          this.ARR_Airport = "n.a.";
          lblCfgArr.Text = this.ARR_Airport;
        }
        else {
          txCfgArr.ForeColor = Color.GreenYellow;
          this.ARR_Airport = txCfgArr.Text;
          lblCfgArr.Text = apt.Name;
        }
      }
    }

    // Pilot ID KeyPress
    private void txCfgSbPilotID_KeyPress( object sender, KeyPressEventArgs e )
    {
      txCfgSbPilotID.ForeColor = _txForeColorDefault; // clear the one not available
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        if (SimBrief.IsSimBriefUserID( txCfgSbPilotID.Text )) {
          txCfgSbPilotID.ForeColor = Color.GreenYellow;
        }
        else {
          txCfgSbPilotID.ForeColor = Color.Red;
        }
      }
    }

    // SB Load Plan pressed
    private void btCfgSbLoadPlan_Click( object sender, EventArgs e )
    {
      lblCfgSbPlanData.Text = "...";
      if (SimBrief.IsSimBriefUserID( txCfgSbPilotID.Text )) {
        lblCfgSbPlanData.Text = "loading...";
        // call for a JSON OFP
        _simBrief.PostDocument_Request( txCfgSbPilotID.Text, SimBriefDataFormat.JSON );
        // will return in the CallBack
      }
      else {
        lblCfgSbPlanData.Text = "invalid Pilot ID format (->2..7 digit)";
      }
    }

    // MSFS Select and load a plan
    private void btCfgMsSelectPlan_Click( object sender, EventArgs e )
    {
      OFD.Title = "Select and Load a Flightplan";
      // usually it is the last selected one
      var path = AppSettings.Instance.LastMsfsPlan;
      path = string.IsNullOrWhiteSpace( path ) ? "DUMMY" : path; // cannot handle empty strings..
      if (Directory.Exists( Path.GetDirectoryName( path ) )) {
        // path exists - use it
        OFD.FileName = Path.GetFileName( path );
        OFD.InitialDirectory = Path.GetDirectoryName( path );
      }
      else {
        // set a default path if the last one does not longer exists
        OFD.FileName = "CustomFlight.pln";
        OFD.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
      }

      if (OFD.ShowDialog( this ) == DialogResult.OK) {
        //selected
        lblCfgMsPlanData.Text = "loading...";
        _selectedPlanFile = OFD.FileName; // temp store the plan file name
        if (_selectedPlanFile.ToLowerInvariant( ).EndsWith( ".pln" )) {
          _msfsPln.PostDocument_Request( _selectedPlanFile );
        }
        else if (_selectedPlanFile.ToLowerInvariant( ).EndsWith( ".flt" )) {
          _msfsFlt.PostDocument_Request( _selectedPlanFile );
        }
        // will report in the Event
      }
    }

    // Request MSFS FLT Download pressed
    private void btCfgRequestFLT_Click( object sender, EventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        lblCfgMsPlanData.Text = "loading...";
        _awaitingFLTfile = true; // allow to get one
        SC.SimConnectClient.Instance.FlightPlanModule.RequestFlightSave( );
        // should report via Instance_FltSave Event
      }
      else {
        lblCfgMsPlanData.Text = "not connected";
      }
    }

    // MSFS Load Plan pressed
    private void btCfgMsLoadPlan_Click( object sender, EventArgs e )
    {
      lblCfgMsPlanData.Text = "loading...";
      // call for a XML OFP
      _selectedPlanFile = ""; // clear the selected one
      _msfsPln.PostDocument_Request( MSFSPln.CustomFlightPlan_filename );
      // will return in the CallBack
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

    // triggered when a FLT file arrives and it was requested by the user
    // else there are auto saves etc. which are of no interest here
    private void Instance_FltSave( object sender, SC.State.FltSaveEventArgs e )
    {
      if (_awaitingFLTfile) {
        _awaitingFLTfile = false;
        if (e.Filename.ToLowerInvariant( ).EndsWith( ".flt" )) {
          _msfsFlt.PostDocument_Request( e.Filename );
        }
      }
    }

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

    #region DGV Workarounds

    private void dgvProfile_RowsAdded( object sender, DataGridViewRowsAddedEventArgs e )
    {
      // workaround where the column get visible again when clearing and adding rows....
      dgvProfile.Columns[0].Visible = false;
    }

    private void dgvRate_RowsAdded( object sender, DataGridViewRowsAddedEventArgs e )
    {
      // workaround where the column get visible again when clearing and adding rows....
      dgvRate.Columns[0].Visible = false;
    }
    private void dgvAlt1_RowsAdded( object sender, DataGridViewRowsAddedEventArgs e )
    {
      // workaround where the column get visible again when clearing and adding rows....
      dgvAlt.Columns[0].Visible = false;
    }

    #endregion

  }
}
