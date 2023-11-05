using System;
using System.Collections.Generic;
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
using bm98_hbFolders;
using bm98_Map;
using bm98_Map.Data;
using FSimFacilityIF;
using FShelf.Profiles;

using FlightplanLib;
using FlightplanLib.SimBrief;
using FlightplanLib.MSFSPln;
using FlightplanLib.MSFSFlt;
using FlightplanLib.GPX;
using FlightplanLib.RTE;
using FlightplanLib.LNM;

using Point = System.Drawing.Point;
using Route = bm98_Map.Data.Route;
using FShelf.FPlans;
using FSimClientIF.Modules;
using static FSimClientIF.Sim;
using FSimClientIF;
using SimConnectClientAdapter;
using dNetBm98;
using static dNetBm98.Units;
using DbgLib;
using FSFData;

namespace FShelf
{
  /// <summary>
  /// FlightBag (Shelf) Form
  /// </summary>
  public partial class frmShelf : Form
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );


    // SimConnect Client Adapter (used only to establish the connection and handle the Online color label)
    private SCClient SCAdapter;

    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // airport which was requested by the user
    private IAirport _airport = null;
    private List<INavaid> _navaidList = null;
    private List<IFix> _fixList = null;

    // SimVar Observer items
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
    private readonly GPXpln _lnmGpx;
    private readonly RTEpln _lnmRte;
    private readonly LNMpln _lnmPln;

    private string _selectedPlanFile = "";
    private bool _awaitingFLTfile = false; // true when FLT is requested

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;
    private Size _lastLiveSize;

    // Touchdown Performance tracker
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
    /// Departure Airport IlsID
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
    /// Arrival Airport IlsID
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
        // add our Airport Report folder if it does not exist
        string aptFolder = Path.Combine( AppSettings.Instance.ShelfFolder, Folders.AptReportSubfolder );
        if (!Directory.Exists( aptFolder )) {
          try {
            Directory.CreateDirectory( aptFolder );
            LOG.Log( "frmShelf.SetShelfFolder", "created AirportReport folder" );
          }
          catch (Exception ex) {
            LOG.LogException( "frmShelf.SetShelfFolder", ex, "Create AirportReport folder failed" );
          }
        }
        // we may get along even if the Airport folder could not be created...
        return true;
      }
      catch (Exception ex) {
        LOG.LogException( "frmShelf.SetShelfFolder", ex, $"Failed for <{folderName}> with" );
        return false;
      }
    }


    // translate the FlightPlan to a Route obj for Map Use
    private static Route GetRouteFromPlan( FlightPlan plan )
    {
      var route = new Route( );
      if (!plan.IsValid) return route;

      foreach (var fix in plan.Waypoints) {
        if (fix.HideInMap( )) continue; // not to be shown

        route.AddRoutePoint(
          new RoutePoint( fix.Ident7, fix.LatLonAlt_ft, fix.WaypointType, fix.InboundTrueTrk, fix.OutboundTrueTrk,
                          fix.IsSID, fix.IsSTAR, fix.IsAirway, fix.AltLimitS( ) )
         );
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

    #region Profile Handling 

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

    #endregion


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
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival );
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

      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "FShelfAppSettings.json" );

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

      // Flightplan decoders
      _simBrief = new SimBrief( );
      _simBrief.SimBriefDataEvent += _simBrief_SimBriefDataEvent;
      _simBrief.SimBriefDownloadEvent += _simBrief_SimBriefDownloadEvent;

      _msfsPln = new MSFSPln( );
      _msfsPln.MSFSPlnDataEvent += _msfsPln_MSFSPlnDataEvent;
      _msfsFlt = new MSFSFlt( );
      _msfsFlt.MSFSFltDataEvent += _msfsFlt_MSFSFltDataEvent;
      _lnmGpx = new GPXpln( );
      _lnmGpx.GPXplnDataEvent += _lnmGpx_GPXplnDataEvent;
      _lnmRte = new RTEpln( );
      _lnmRte.RTEplnDataEvent += _lnmRte_RTEplnDataEvent;
      _lnmPln = new LNMpln( );
      _lnmPln.LNMplnDataEvent += _lnmPln_LNMplnDataEvent;

      // handle some Map Events
      aMap.MapCenterChanged += AMap_MapCenterChanged;
      aMap.MapRangeChanged += AMap_MapRangeChanged;

      // attach FLT save event
      SC.SimConnectClient.Instance.FltSave += Instance_FltSave;

      InitRunwayCombo( comboCfgRunwayLength );

      // Handle the Standalone version
      if (Standalone) {
        // use another WindowFrame in standalone
        this.Text = "MSFS FlightBag";
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        // start FP Module disabled
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false;
        SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode = FSimClientIF.FlightPlanMode.Disabled;

        // Connection to SimConnect Client
        SCAdapter = new SCClient( );
        SCAdapter.Connected += SCAdapter_Connected;
        SCAdapter.Establishing += SCAdapter_Establishing;
        SCAdapter.Disconnected += SCAdapter_Disconnected;
        SCAdapter.SC_Label = lblSimConnectedMap;
      }

    }

    // form is loaded to get visible
    private void frmShelf_Load( object sender, EventArgs e )
    {
      // Init GUI
#if DEBUG
#else
      tab.TabPages.Remove( tabEnergy ); // not yet productive
#endif

      this.Size = AppSettings.Instance.ShelfSize;
      this.Location = AppSettings.Instance.ShelfLocation;
      if (!dNetBm98.Utilities.IsOnScreen( Location )) {
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
        loc.Lat = SV.Get<double>( SItem.dG_Acft_Lat );
        loc.Lon = SV.Get<double>( SItem.dG_Acft_Lon );
        loc.Altitude = SV.Get<float>( SItem.fG_Acft_AltMsl_ft );
      }

      // create the initial 'Airport' to have something to start with
      aMap.MapCreator.Reset( );
      aMap.MapCreator.SetAirport( aMap.MapCreator.DummyAirport( loc ) );
      aMap.MapCreator.Commit( );

      _perfTracker.Reset( );

      // profiles
      InitProfileData( );

      // energy
      rb6sec.Checked = true;


      // standalone handling
      if (Standalone) {
        // File Access Check
        if (Dbg.Instance.AccessCheck( Folders.UserFilePath ) != DbgLib.AccessCheckResult.Success) {
          string msg = $"MyDocuments Folder Access Check Failed:\n{Dbg.Instance.AccessCheckResult}\n\n{Dbg.Instance.AccessCheckMessage}";
          MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
        CheckFacilityDB( );
        SCAdapter.Connect( );
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
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival );
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
        SV.RemoveObserver( _observerID );
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
        SCAdapter.Disconnect( );
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
          _navaidList = NavaidList( aMap.MapCenter( ) );
          _fixList = FixList( );
          aMap.SetNavaidList( _navaidList, _fixList );
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
      using (var _db = new FSFData.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile ))
          return nList;

        // get the the Quads around
        var qs = Quad.Around49EX( latLon.AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        nList = _db.DbReader.NavaidList_ByQuadList( qs ).ToList( );
      }

      /*
      // APT Approach WaypointCat, VORs and NDBs if set in Config
      if (_airport != null / * && cbxCfgIFRwaypoints.Checked * /) {
        nList.AddRange( _airport.Navaids.Where( x => x.IsApproach ) );
      }
      */
      return nList;
    }

    // loads a list of Approach Fixes which are related to the Airport
    private List<IFix> FixList( )
    {
      var nList = new List<IFix>( );
      // APT Approach WaypointCat, VORs and NDBs if set in Config
      if (_airport != null) {
        var aprFixes = _airport.APRs( ).SelectMany( proc => proc.CommonFixes ).Distinct( ).ToList( );
        foreach (var apr in _airport.APRs( )) {
          nList.AddRange( DbLookup.ExpandAPRFixes( apr, Folders.GenAptDBFile ) );
        }
        var unresolved = nList.FirstOrDefault( fix => fix.WYP == null );
      }

      return nList;
    }

    // get alternate airports from DB 
    private List<IAirportDesc> AltAirportList( LatLon latLon, float minRwyLength )
    {
      var aList = new List<IAirportDesc>( );
      using (var _db = new FSFData.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile ))
          return aList; // no db available

        // get the the Quads around
        var qs = Quad.Around49EX( latLon.AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        if (minRwyLength <= 1) {
          // short if no length is selected
          aList = _db.DbReader.AirportDescList_ByQuadList( qs ).ToList( );
        }
        else {
          aList = _db.DbReader.AirportDescList_ByQuadList( qs ).ToList( );
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
      _fixList = FixList( );
      _navaidList = NavaidList( e.CenterCoordinate );
      aMap.SetNavaidList( _navaidList, _fixList );
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
      using (var _db = new DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
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

        var aptReport = new AptReport.AptReportTable( );
        aptReport.SaveDocument( _airport, _navaidList, _fixList,
                            Path.Combine( AppSettings.Instance.ShelfFolder, Folders.AptReportSubfolder ),
                            true );
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

      // TODO: the next two selectors will cause loosing tracking of the Acft, may be update it anyway
      if (!this.Visible) return;  // no need to update while the shelf is not visible ???? TODO decide if or if not cut reporting ????
                                  //if (!(tab.SelectedTab == tabMap)) return;  //don't update while not showing the MapTab - this causes track disruptions when in METAR etc

      // Map update pace slowed down to an acceptable CPU load - (native is 100ms)
      if ((_updates++ % 5) == 0) { // 500ms pace - slow enough ?? performance penalty...
        _tAircraft.OnGround = SV.Get<bool>( SItem.bG_Sim_OnGround );
        _tAircraft.TrueHeading_deg = SV.Get<float>( SItem.fG_Nav_HDG_true_deg );
        _tAircraft.Heading_degm = SV.Get<float>( SItem.fG_Nav_HDG_mag_degm );
        _tAircraft.Position = new LatLon( SV.Get<double>( SItem.dG_Acft_Lat ), SV.Get<double>( SItem.dG_Acft_Lon ) );
        _tAircraft.Altitude_ft = SV.Get<float>( SItem.fG_Acft_AltMsl_ft );
        // limit RA visible to 1500 ft if not on ground
        _tAircraft.RadioAlt_ft = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN
                                  : (SV.Get<float>( SItem.fG_Acft_AltAoG_ft ) <= 1500) ? SV.Get<float>( SItem.fG_Acft_AltAoG_ft ) : float.NaN;
        _tAircraft.Ias_kt = SV.Get<float>( SItem.fG_Acft_IAS_kt );
        _tAircraft.Tas_kt = SV.Get<float>( SItem.fG_Acft_TAS_kt );
        _tAircraft.Gs_kt = SV.Get<float>( SItem.fG_Acft_GS_kt );
        _tAircraft.Vs_fpm = (int)(SV.Get<float>( SItem.fG_Acft_VS_ftPmin ) / 20) * 20; // 20 fpm steps only
        _tAircraft.Fpa_deg = SV.Get<float>( SItem.fG_Acft_FlightPathAngle_deg );
        // GPS does not provide meaningful track values when not moving
        _tAircraft.Trk_degm = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN : SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm );
        _tAircraft.TrueTrk_deg = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN : SV.Get<float>( SItem.fG_Gps_GTRK_true_deg );

        _tAircraft.WindSpeed_kt = SV.Get<float>( SItem.fG_Acft_WindSpeed_kt );
        _tAircraft.WindDirection_deg = SV.Get<float>( SItem.fG_Acft_WindDirection_deg );

        // update the map
        aMap.UpdateAircraft( _tAircraft );
        // Update Profile page
        UpdateProfileData( );
        // Update Energy page
        UpdateEnergyData( );
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, true ); // as PDF
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, false );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
    }

    #endregion

    #region NSFS Flt Events

    private void _msfsFlt_MSFSFltDataEvent( object sender, MSFSFltDataEventArgs e )
    {
      DebSaveRouteString( e.MSFSFltData, "FLT" );

      var ins = e.MSFSFltData;
      _flightPlan.LoadMsFsFLT( ins );
      if (_flightPlan.IsMsFsPLN) {
        lblCfgMsPlanData.Text = "FLT file received";
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, false );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
      else {
        lblCfgMsPlanData.Text = "got an invalid file";
      }
    }

    #endregion

    #region LNM Gpx Events
    private void _lnmGpx_GPXplnDataEvent( object sender, GPXplnDataEventArgs e )
    {
      DebSaveRouteString( e.GPXplnData, "GPX" );

      lblCfgMsPlanData.Text = "GPX data received";
      var xs = e.GPXplnData;
      _flightPlan.LoadLnmGPX( xs );
      if (_flightPlan.IsLnmGPX) {
        // save selected file in settings 
        if (!string.IsNullOrEmpty( _selectedPlanFile )) {
          AppSettings.Instance.LastMsfsPlan = _selectedPlanFile;
        }

        // populate CFG fields
        lblCfgMsPlanData.Text = $"GPX: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, false );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
    }
    #endregion

    #region LNM Rte Events
    private void _lnmRte_RTEplnDataEvent( object sender, RTEplnDataEventArgs e )
    {
      DebSaveRouteString( e.RTEplnData, "RTE" );

      lblCfgMsPlanData.Text = "RTE data received";
      var xs = e.RTEplnData;
      _flightPlan.LoadLnmRTE( xs );
      if (_flightPlan.IsLnmRTE) {
        // save selected file in settings 
        if (!string.IsNullOrEmpty( _selectedPlanFile )) {
          AppSettings.Instance.LastMsfsPlan = _selectedPlanFile;
        }

        // populate CFG fields
        lblCfgMsPlanData.Text = $"RTE: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, false );
        if (!string.IsNullOrWhiteSpace( err )) lblCfgMsPlanData.Text = err;
      }
    }
    #endregion

    #region LNM Plan Events
    private void _lnmPln_LNMplnDataEvent( object sender, LNMplnDataEventArgs e )
    {
      DebSaveRouteString( e.LNMplnData, "LNM" );

      lblCfgMsPlanData.Text = "LNM data received";
      var xs = e.LNMplnData;
      _flightPlan.LoadLnmPLN( xs );
      if (_flightPlan.IsLnmPLN) {
        // save selected file in settings 
        if (!string.IsNullOrEmpty( _selectedPlanFile )) {
          AppSettings.Instance.LastMsfsPlan = _selectedPlanFile;
        }

        // populate CFG fields
        lblCfgMsPlanData.Text = $"LNM: {_flightPlan.FlightPlan.Origin.Icao_Ident} to {_flightPlan.FlightPlan.Destination.Icao_Ident}";
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
        var err = _flightPlan.GetAndSaveDocuments( AppSettings.Instance.ShelfFolder, false );
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
      bool lbs = rbKLbs.Checked;
      string unit = lbs ? "lbs x 1000" : "kg x 1000";
      float value;
      var RTF = new RTFformatter( );
      RTF.SetTab( 4000 ); RTF.SetTab( 5500 );
      RTF.RBold = true;
      RTF.RColor = RTFformatter.ERColor.ERC_Blue;
      RTF.FontSize( 15 );
      RTF.WriteLn( $"{SV.Get<string>( SItem.sG_Cfg_AircraftID )}  -  {SV.Get<string>( SItem.sG_Cfg_AcftConfigFile )}" );

      RTF.RColor = RTFformatter.ERColor.ERC_Black;
      RTF.FontSize( 14 );
      RTF.WriteLn( );
      RTF.Write( $"Weight and Balance:" ); RTF.WriteTab( $"{unit}" ); RTF.WriteLn( );
      RTF.RBold = false;
      value = lbs ? SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) ) / 1_000f;
      RTF.Write( $"TOTAL Weight" ); RTF.WriteTab( $"{value:##0.000}\n" ); RTF.WriteLn( );
      value = lbs ? SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) ) / 1_000f;
      RTF.Write( $"FUEL Weight" ); RTF.WriteTab( $"{value:##0.000}" );
      RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Fuel_Quantity_total_gal ):##0.0} gal" ); RTF.WriteLn( );
      value = lbs ? SV.Get<float>( SItem.fG_Acft_PayloadWeight_lbs ) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_PayloadWeight_lbs ) ) / 1_000f;
      RTF.Write( $"PAYLOAD Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      value = lbs ? (SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) - SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb )) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) - SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) ) / 1_000f;
      RTF.Write( $"ZF Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      RTF.Write( $"CG lon/lat" );
      RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Acft_AcftCGlong_perc ) * 100f:#0.00} %  /  {SV.Get<float>( SItem.fG_Acft_AcftCGlat_perc ) * 100f:#0.00} %" );
      RTF.WriteLn( );
      value = lbs ? SV.Get<float>( SItem.fG_Dsg_EmptyAcftWeight_lbs ) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Dsg_EmptyAcftWeight_lbs ) ) / 1_000f;
      RTF.Write( $"Empty Weight" ); RTF.WriteTab( $"{value:##0.000}" ); RTF.WriteLn( );
      value = lbs ? SV.Get<float>( SItem.fG_Dsg_MaxAcftWeight_lbs ) / 1_000f
                  : (float)Kg_From_Lbs( SV.Get<float>( SItem.fG_Dsg_MaxAcftWeight_lbs ) ) / 1_000f;
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
      RTF.Write( $"Cruise Altitude" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_CruiseAlt_ft ),6:##,##0} ft" ); RTF.WriteLn( );
      RTF.Write( $"Est. Cruise Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_CruiseSpeedEst_kt ),8:##,##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vc  Cruise Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedVC_kt ),8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vy  Climb Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedClimb_kt ),8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vmu Takeoff Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedTakeoff_kt ),8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vr  Min Rotation" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedMinRotation_kt ),8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vs1 Stall Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedVS1_kt ),8:##0} kt" ); RTF.WriteLn( );
      RTF.Write( $"Vs0 Stall Speed" ); RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Dsg_SpeedVS0_kt ),8:##0} kt" ); RTF.WriteLn( );

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

      lblGS_P.Text = $"{_tAircraft.Gs_kt:##0}";
      lblVS_P.Text = $"{_tAircraft.Vs_fpm:#,##0}";
      lblAlt_P.Text = $"{_tAircraft.Altitude_ft:##,##0}";
      lblIAS_P.Text = $"{_tAircraft.Ias_kt:##0}";
      lblTAS_P.Text = $"{_tAircraft.Tas_kt:##0}";
      lblFPA_P.Text = $"{_tAircraft.Fpa_deg:#0.0}";
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

    #region EnergyTab Events

    // update the EnergyTab data
    private void UpdateEnergyData( )
    {
      if (tab.SelectedTab != tabEnergy) return; // Tab not shown, omit updates

      //Maintain E-Table values - using the Data API
      uC_ETable1.SetValues(
        SV.Get<float>( SItem.fG_Acft_TAS_kt ),
        SV.Get<float>( SItem.fG_Acft_AltMsl_ft ),
        SV.Get<double>( SItem.dG_Env_Time_absolute_sec )
      );
      // Using Design Constants for Stall Speed
      if (SV.Get<float>( SItem.fG_Flp_Deployment_prct ) > 90) {
        uC_ETable1.StallSpeed_kt = SV.Get<float>( SItem.fG_Dsg_SpeedVS0_kt );
      }
      else {
        uC_ETable1.StallSpeed_kt = SV.Get<float>( SItem.fG_Dsg_SpeedVS1_kt );
      }
      // using the Flight Assistants number for Stall Speed
      /* DOES NOT REPLY - Asobo do better !!!!!
      uC_ETable1.StallSpeed_kt = SV.Get<float>( SItem.fG_Acft_FA_StallSpeed_kt );
      */
      // Elevation of the ground below the aircraft
      uC_ETable1.GroundElevation_ft = (float)Units.Ft_From_M( SV.Get<float>( SItem.fG_Env_GroundAltitude_m ) );

      // Disp Labels on top - use the aircraft obj to pull them from
      lblGS_E.Text = $"{_tAircraft.Gs_kt:##0}";
      lblVS_E.Text = $"{_tAircraft.Vs_fpm:#,##0}";
      lblAlt_E.Text = $"{_tAircraft.Altitude_ft:##,##0}";
      lblIAS_E.Text = $"{_tAircraft.Ias_kt:##0}";
      lblTAS_E.Text = $"{_tAircraft.Tas_kt:##0}";
      lblFPA_E.Text = $"{_tAircraft.Fpa_deg:#0.0}";
    }

    private void rb6sec_CheckedChanged( object sender, EventArgs e )
    {
      if (rb6sec.Checked) {
        uC_ETable1.Est_Time_s = 6f;
      }
    }

    private void rb30sec_CheckedChanged( object sender, EventArgs e )
    {
      if (rb30sec.Checked) {
        uC_ETable1.Est_Time_s = 30f;
      }
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
        _simBrief.PostDocument_Request( txCfgSbPilotID.Text, SimBriefDataFormat.JSON ); // USE HTTP
        // will return in the CallBack
      }
      else {
        lblCfgSbPlanData.Text = "invalid Pilot ID format (->2..7 digit)";
      }
    }

    // MSFS Select and load a plan
    private void btCfgMsSelectPlan_Click( object sender, EventArgs e )
    {
      // override GUI
      OFD.Filter = "MSFS Flightplans|*.pln;*.flt|LittleNavMap Plan|*.lnmpln|GPX File|*.gpx|Route String|*.rte|All files|*.*";

      OFD.Title = "Select and Load a Flightplan";
      // usually it is the last selected one
      var path = AppSettings.Instance.LastMsfsPlan;
      path = string.IsNullOrWhiteSpace( path ) ? "DUMMY" : path; // cannot handle empty strings..
      if (Directory.Exists( Path.GetDirectoryName( path ) )) {
        // path exists - use it
        OFD.FileName = Path.GetFileName( path );
        OFD.InitialDirectory = Path.GetDirectoryName( path );
        OFD.FilterIndex = (Path.GetExtension( path ).ToLowerInvariant( ) == ".rte") ? 4
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".gpx") ? 3
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".lnmpln") ? 2
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".pln") ? 1
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".flt") ? 1
          : 5;
      }
      else {
        // set a default path if the last one does not longer exists
        OFD.FileName = "CustomFlight.pln";
        OFD.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
        OFD.FilterIndex = 1;
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
        else if (_selectedPlanFile.ToLowerInvariant( ).EndsWith( ".gpx" )) {
          _lnmGpx.PostDocument_Request( _selectedPlanFile );
        }
        else if (_selectedPlanFile.ToLowerInvariant( ).EndsWith( ".rte" )) {
          _lnmRte.PostDocument_Request( _selectedPlanFile );
        }
        else if (_selectedPlanFile.ToLowerInvariant( ).EndsWith( ".lnmpln" )) {
          _lnmPln.PostDocument_Request( _selectedPlanFile );
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

    // triggered when a FLT file arrives and it was requested by the user
    // else there are auto saves etc. which are of no interest here
    private void Instance_FltSave( object sender, FltSaveEventArgs e )
    {
      if (_awaitingFLTfile) {
        _awaitingFLTfile = false;
        if (e.FileValid && e.Filename.ToLowerInvariant( ).EndsWith( ".flt" )) {
          _msfsFlt.PostDocument_Request( e.Filename );
        }
      }
    }

    // establishing event
    private void SCAdapter_Establishing( object sender, EventArgs e )
    {
      lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;
    }

    // connect event
    private void SCAdapter_Connected( object sender, EventArgs e )
    {
      lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;

      // register DataUpdates if not done 
      if (SC.SimConnectClient.Instance.IsConnected && _observerID < 0) {
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival );
      }
    }

    // disconnect event
    private void SCAdapter_Disconnected( object sender, EventArgs e )
    {
      lblSimConnectedNotes.BackColor = lblSimConnectedMap.BackColor;

      if (_observerID >= 0) {
        SV.RemoveObserver( _observerID );
        _observerID = -1;
      }
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
