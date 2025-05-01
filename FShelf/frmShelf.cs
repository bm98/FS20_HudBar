using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using Point = System.Drawing.Point;
using static FSimClientIF.Sim;
using static dNetBm98.Units;

using CoordLib;
using CoordLib.MercatorTiles;
using CoordLib.Extensions;

using dNetBm98;
using DbgLib;
using FSFData;

using FSimClientIF.Modules;
using FSimClientIF;
using FSimFacilityIF;

using SC = SimConnectClient;
using SimConnectClientAdapter;

using bm98_hbFolders;
using bm98_Map;
using bm98_Map.Data;

using FSimFlightPlans.SimBrief;
using FSimFlightPlans.MSFSPln;

using FShelf.Profiles;
using FShelf.FPlans;
using FShelf.LandPerf;
using dNetBm98.Job;
using System.Threading;

namespace FShelf
{
  /// <summary>
  /// FlightBag (Shelf) Form
  ///   Provides an API
  /// </summary>
  public partial class frmShelf : Form, IFlightBagAPI
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // RA display limits (Should match the ones in HudBar Calculator - but there is no sensible shared library to place them)
    private const float c_raDefault_ft = 1500;
    private const float c_raAirliner_ft = 2500;
    // ICAO display for airport not available
    private const string c_airportNA = "n.a.";

    // SimConnect Client Adapter
    // (!! used only to establish the connection and handle the Online color label !!)
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
    private int _updatesAI;

    // comm item with the Mapping user control
    private readonly TrackedAircraftCls _tAircraft = new TrackedAircraftCls( );

    // default Airport Entry fore color
    private Color _txForeColorDefault;

    // METAR Provider
    private readonly MetarLib.Metar _metar;

    // Plan Handler
    private readonly FpWrapper _flightPlanHandler = new FpWrapper( ); // only one instance, don't null it !!
    private JobRunner _jobRunner;

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;
    private Size _lastLiveSize;

    // Profiles
    private readonly ProfileCat _profileCat = new ProfileCat( );
    private readonly BindingSource _profileBinding = new BindingSource( );
    private readonly BindingSource _vsRateBinding = new BindingSource( );
    private readonly BindingSource _altBinding = new BindingSource( );
    private static readonly DataGridViewCellStyle _vCellStyle
      = new DataGridViewCellStyle( ) { Alignment = DataGridViewContentAlignment.MiddleRight, BackColor = Color.Gainsboro, SelectionBackColor = Color.CornflowerBlue };
    private static readonly DataGridViewCellStyle _vCellStyleMarked
      = new DataGridViewCellStyle( _vCellStyle ) { BackColor = Color.MediumSpringGreen, SelectionBackColor = Color.CadetBlue };

    // remove the obsolete @.FlightTable.png / @.FlightPlan.png file 
    private void CleanupV07( string shelfFolder )
    {
      string delFile = Path.Combine( shelfFolder, Folders.FTablePNG_FileName ); // "@.FlightTable.png"
      if (File.Exists( delFile )) {
        try {
          File.Delete( delFile );
        }
        catch { }
      }
      delFile = Path.Combine( shelfFolder, Folders.FPlanPNG_FileName ); // "@.FlightPlan.png"
      if (File.Exists( delFile )) {
        try {
          File.Delete( delFile );
        }
        catch { }
      }
    }

    /// <summary>
    /// Set true to run in standalone mode
    /// </summary>
    public bool Standalone { get; private set; } = false;

    /// <summary>
    /// Fired when the user loaded a valid flightplan
    /// </summary>
    public event EventHandler<EventArgs> FlightPlanLoadedByUser;
    private void OnFlightPlanLoadedByUser( )
    {
      FlightPlanLoadedByUser?.Invoke( this, new EventArgs( ) );
      aMap?.RenderItems( ); // will update if there is a need for it
    }


    /// <summary>
    /// Ref to the currently loaded Flightplan, can be empty one (check IsValid)
    /// NOTE: don't mess with it and update when the load event is fired... 
    /// TODO: provide a tamper safe interface...
    /// </summary>
    public FSimFlightPlans.FlightPlan FlightPlanRef => _flightPlanHandler.FlightPlan;

    /// <summary>
    /// Departure Airport ICAO ID
    /// </summary>
    public string DEP_Airport {
      get => _dep_Airport;
      set {
        _dep_Airport = value;
        lblMetDep.Text = value; // maintain in METAR
        lblDEP.Text = value; // maintain in MAP
      }
    }
    private string _dep_Airport = c_airportNA;
    /// <summary>
    /// Arrival Airport ICAO ID
    /// </summary>
    public string ARR_Airport {
      get => _arr_Airport;
      set {
        _arr_Airport = value;
        lblMetArr.Text = value; // maintain in METAR
        lblARR.Text = value; // maintain in MAP
      }
    }
    private string _arr_Airport = c_airportNA;

    /// <summary>
    /// The active SimBrief Pilot ID
    /// </summary>
    public string SimBriefID => AppSettings.Instance.SbPilotID;

    /// <summary>
    /// Load the active FP from SimBrief - reports via FlightPlanLoadedByUser Event
    /// using the Setting ID
    /// </summary>
    /// <returns>True if loading</returns>
    public bool LoadFromSimBrief( )
    {
      if (SimBriefHandler.IsSimBriefUserID( SimBriefID )) {
        _flightPlanHandler.RequestSBDownload( SimBriefID );
        // will report in the Event
        return true;
      }
      return false;
    }

    /// <summary>
    /// Open a Dialog to load a FLightPlan from File - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    public bool LoadFlightPlanFile( )
    {
      // override GUI
      OFD.Filter = "MSFS Flightplans|*.pln;*.flt|LittleNavMap Plan|*.lnmpln|Garmin FPL file|*.fpl|GPX File|*.gpx|Route String|*.rte|All files|*.*";
      OFD.Title = "Select and Load a Flightplan";
      // usually it is the last selected one
      var path = AppSettings.Instance.LastMsfsPlan;
      path = string.IsNullOrWhiteSpace( path ) ? "DUMMY" : path; // cannot handle empty strings..
      if (Directory.Exists( Path.GetDirectoryName( path ) )) {
        // path exists - use it
        OFD.FileName = Path.GetFileName( path );
        OFD.InitialDirectory = Path.GetDirectoryName( path );
        OFD.FilterIndex = (Path.GetExtension( path ).ToLowerInvariant( ) == ".rte") ? 5
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".gpx") ? 4
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".fpl") ? 3
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".lnmpln") ? 2
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".pln") ? 1
          : (Path.GetExtension( path ).ToLowerInvariant( ) == ".flt") ? 1
          : 6;
      }
      else {
        // set a default path if the last one does not longer exists
        OFD.FileName = "CustomFlight.pln";
        OFD.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
        OFD.FilterIndex = 1;
      }

      if (OFD.ShowDialog( this ) == DialogResult.OK) {
        //selected
        lblCfgPlanMessage.Text = "loading...";
        if (_flightPlanHandler.RequestPlanFile( OFD.FileName )) {
          return true;
          // will report in the Event
        }
      }
      return false;
    }

    /// <summary>
    /// Loads the default PLN file - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    public bool LoadDefaultPLN( )
    {
      lblCfgPlanMessage.Text = "loading...";
      // FS2020 call for a XML PLN, FS2024 call for EFB Planned Route
      var fsVersion = SV.Get<FSimVersion>( SItem.fv_Sim_FSVersion );
      if (fsVersion == FSimVersion.MSFS2020) {
        if (_flightPlanHandler.RequestPlanFile( MSFSPlnHandler.CustomFlightPlan_filename )) {
          return true;
          // will report in the Event
        }
        ;
      }
      else if (fsVersion == FSimVersion.MSFS2024) {
        // trigger download
        SV.Set( SItem.sGS_Gps_EFB_route, "" );
        // delayed job... to get the EFB file and issue a load request
        _jobRunner.AddJob( new JobObj( ( ) => {
          // max 10 tries, else give up
          string efbS = "";
          for (int i = 0; i < 10; i++) {
            Thread.Sleep( 1000 ); // Wait until retrieved
            efbS = SV.Get<string>( SItem.sGS_Gps_EFB_route, "" ); // get string
            if (!string.IsNullOrWhiteSpace( efbS )) { break; }
          }
          if (!string.IsNullOrWhiteSpace( efbS )) {
            _flightPlanHandler.RequestPlanData( efbS, FSimFlightPlans.SourceOfFlightPlan.MS_Efb24 );
            // will report in event later
          }
        }, "Get EFB24 Route" ) );
        Thread.Yield( );
        return true;
      }

      return false;
    }

    /// <summary>
    /// Load a Route String - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    public bool LoadRouteString( string routeString )
    {
      // never fail
      try {
        lblCfgPlanMessage.Text = "loading...";
        return _flightPlanHandler?.RequestPlanData( routeString, FSimFlightPlans.SourceOfFlightPlan.GEN_Rte ) ?? false;
        // will report in the Event
      }
      catch (Exception ex) {
        LOG.Error( ex, "LoadRouteString: failed with exception" );
      }

      return false;
    }

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
            LOG.Info( "frmShelf.SetShelfFolder", "created AirportReport folder" );
          }
          catch (Exception ex) {
            LOG.Error( "frmShelf.SetShelfFolder", ex, "Create AirportReport folder failed" );
          }
        }
        // we may get along even if the Airport folder could not be created...
        return true;
      }
      catch (Exception ex) {
        LOG.Error( "frmShelf.SetShelfFolder", ex, $"Failed for <{folderName}> with" );
        return false;
      }
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

      cx.Items.Add( new RwyLenItem( "Runway any length", 0f ) );
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
      cx.Items.Add( new RwyLenItem( "Helipad only", float.MinValue ) ); // add for Helipads
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

      // Init the FlightPlan Module with our locations
      FSimFlightPlans.FlightPlan.Setup( Folders.GenAptDBFile, Folders.UserTempPath );

      // Init Settings
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

      // Flightplan Handler
      _flightPlanHandler.FlightPlanArrived += _flightPlanHandler_FlightPlanArrived;
      _jobRunner = new JobRunner( );

      // handle some Map Events
      aMap.MapCenterChanged += AMap_MapCenterChanged;
      aMap.MapRangeChanged += AMap_MapRangeChanged;
      aMap.TeleportAircraft += AMap_TeleportAircraft;

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
        SC.SimConnectClient.Instance.FltFileModule.Enabled = false;
        SC.SimConnectClient.Instance.FltFileModule.ModuleMode = FSimClientIF.FltFileModuleMode.Disabled;

        // Connection to SimConnect Client
        SCAdapter = new SCClient( );
        SCAdapter.Connected += SCAdapter_Connected;
        SCAdapter.Establishing += SCAdapter_Establishing;
        SCAdapter.Disconnected += SCAdapter_Disconnected;
        SCAdapter.SC_Label = lblSimConnectedMap;
      }

      // Pacer Interval is 1 sec
      timer1.Interval = 1000;

      // In HudBar mode, enable the Pacer already in cTor
      // should capture and process data arrival without having to show the Window.
      if (!Standalone) {
        timer1.Enabled = true;
      }
    }

    // form is loaded to get visible
    // do only GUI tasks here as the Form is not loaded until the user asks for it
    private void frmShelf_Load( object sender, EventArgs e )
    {
      // Init GUI
#if DEBUG
#else
      tab.TabPages.Remove( tabEnergy ); // not yet productive
#endif
      this.Size = AppSettings.Instance.ShelfSize;
      Location = new Point( 20, 20 );
      // init with the proposed location from profile (check within a virtual box)
      if (dNetBm98.Utilities.IsOnScreen( AppSettings.Instance.ShelfLocation, new Size( 100, 100 ) )) {
        this.Location = AppSettings.Instance.ShelfLocation;
      }
      _lastLiveSize = Size;
      _lastLiveLocation = Location;

      cbxCfgPrettyMetar.Checked = AppSettings.Instance.PrettyMetar;
      rbKg.Checked = true;
      rbKLbs.Checked = AppSettings.Instance.WeightLbs; // will toggle if needed
      rtbNotes.Text = AppSettings.Instance.NotePadText;

      // set prev. used airports if not already set
      if (DEP_Airport == c_airportNA) {
        DEP_Airport = AppSettings.Instance.DepICAO;
      }
      if (ARR_Airport == c_airportNA) {
        ARR_Airport = AppSettings.Instance.ArrICAO;
      }
      // defaults in config from prev use as well (not updated through the Property above)
      txCfgDep.Text = DEP_Airport;
      txCfgArr.Text = ARR_Airport;

      _txForeColorDefault = txEntry.ForeColor;

      // map settings
      aMap.MapRange = MapRange.Mid;
      aMap.ShowMapGrid = AppSettings.Instance.MapGrid;
      aMap.ShowAirportRange = AppSettings.Instance.AirportRings;
      aMap.ShowRoute = AppSettings.Instance.FlightplanRoute;
      aMap.ShowNavaids = AppSettings.Instance.VorNdb;
      aMap.ShowVFRMarks = AppSettings.Instance.VFRmarks;
      aMap.ShowAptMarks = AppSettings.Instance.AptMarks;
      aMap.ShowTrackedAircraft = AppSettings.Instance.AcftMark;
      aMap.AutoRange = AppSettings.Instance.AutoRange;
      aMap.ShowOtherAircraftsEnabled = AppSettings.Instance.AcftAIChecked;
      // make sure we get a valid Enum
      if (Enum.IsDefined( typeof( AcftAiDisplayMode ), AppSettings.Instance.AcftAI )) {
        aMap.ShowOtherAircrafts = AppSettings.Instance.AcftAIChecked // must be checked, else set to None
          ? (AcftAiDisplayMode)AppSettings.Instance.AcftAI : AcftAiDisplayMode.None;
      }
      else {
        aMap.ShowOtherAircrafts = AcftAiDisplayMode.None; // invalid, set default
      }

      // config settings
      cbxCfgAcftRange.Checked = AppSettings.Instance.AcftRange;
      cbxCfgAcftWind.Checked = AppSettings.Instance.AcftWind;
      cbxCfgAcftTrack.Checked = AppSettings.Instance.AcftTrack;
      cbxCfgShowOtherAcft.Checked = AppSettings.Instance.AcftAIChecked;
      txAiFilter.Text = AppSettings.Instance.AcftAIFilter;
      txCfgSbPilotID.Text = AppSettings.Instance.SbPilotID;

      try {// don't ever fail
        comboCfgRunwayLength.SelectedIndex = (AppSettings.Instance.MinRwyLengthCombo >= 0) ? AppSettings.Instance.MinRwyLengthCombo : 0;
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
        loc.Lat = SV.Get<double>( SItem.dGS_Acft_Lat );
        loc.Lon = SV.Get<double>( SItem.dGS_Acft_Lon );
        loc.Altitude = SV.Get<float>( SItem.fGS_Acft_AltMsl_ft );
      }

      // create the initial 'Airport' to have something to start with
      aMap.MapCreator.Reset( );
      aMap.MapCreator.SetAirport( aMap.MapCreator.DummyAirport( loc ) );
      aMap.MapCreator.Commit( );

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

      // cleanup from prev versions
      CleanupV07( aShelf.ShelfFolder );

      // will enable the Pacer in Standalone mode, after 
      timer1.Enabled = true;
    }

    // form got attention
    private void frmShelf_Activated( object sender, EventArgs e )
    {
      this.TopMost = true;
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
      LOG.Info( "ShelfForm about to close" );

      this.TopMost = false;

      // Check if Form was not initialized
      if (!this.Created || (comboCfgRunwayLength.SelectedIndex < 0)) {
        // happens when flightplan processing is active but the Form was never called by the user
        // DON'T SAVE SETTINGS, 
        LOG.Info( "ShelfForm was not yet created - omit save of Settings" );
        ; // DEBUG STOP
      }
      else {
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
        AppSettings.Instance.AcftAI = cbxCfgShowOtherAcft.Checked // must be checked, else set None
          ? (int)aMap.ShowOtherAircrafts
          : (int)AcftAiDisplayMode.None;

        // config settings
        AppSettings.Instance.PrettyMetar = cbxCfgPrettyMetar.Checked;
        AppSettings.Instance.AcftRange = cbxCfgAcftRange.Checked;
        AppSettings.Instance.AcftWind = cbxCfgAcftWind.Checked;
        AppSettings.Instance.AcftTrack = cbxCfgAcftTrack.Checked;
        AppSettings.Instance.AcftAIChecked = cbxCfgShowOtherAcft.Checked;
        AppSettings.Instance.AcftAIFilter = txAiFilter.Text.Trim( );
        AppSettings.Instance.SbPilotID = txCfgSbPilotID.Text.Trim( );
        //--
        AppSettings.Instance.Save( );
      }

      if (Standalone) {
        timer1.Enabled = false;
        // UnRegister DataUpdates
        if (SC.SimConnectClient.Instance.IsConnected && (_observerID >= 0)) {
          SV.RemoveObserver( _observerID );
        }
        _observerID = -1;

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

    #region TImer Event

    // Timer has fired
    private void timer1_Tick( object sender, EventArgs e )
    {
      // register DataUpdates if in HudBar mode and if not yet done 
      if (!Standalone && SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival, this );
      }

      // Update the Performance Tab content 
      if (tab.SelectedTab == tabPerf) {
        SetPerfContent( );
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

        // AI aircrafts
        var filterListAI = new List<string>( );
        if (cbxCfgShowOtherAcft.Checked) {
          SC.SimConnectClient.Instance.AiAcftPoolModule.Enabled = true;
          aMap.ShowOtherAircraftsEnabled = true;
          // update the AI acft filter
          string[] items = txAiFilter.Text.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
          foreach (var item in items) {
            filterListAI.Add( item.Trim( ).ToUpperInvariant( ) );
          }
        }
        else {
          SC.SimConnectClient.Instance.AiAcftPoolModule.Enabled = false;
          aMap.ShowOtherAircrafts = AcftAiDisplayMode.None;
          aMap.ShowOtherAircraftsEnabled = false;
        }
        aMap.SetOtherAircraftFilter( filterListAI );

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
        SC.SimConnectClient.Instance.AiAcftPoolModule.Enabled = false; // disable AI tracking if not in Map mode
      }
      else {
        SC.SimConnectClient.Instance.AiAcftPoolModule.Enabled = false; // disable AI tracking if not in Map mode
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
        aList = _db.DbReader.AirportDescList_ByQuadList( qs ).ToList( );
        // select the ones asked for in Config (min length or helipads)
        if (minRwyLength >= 0) {
          aList = aList.Where( x => x.HasRunways && x.LongestRwyLength_m >= minRwyLength ).ToList( );
        }
        else {
          // All Airports with Helipad 
          aList = aList.Where( x => x.HasHelipads ).ToList( );
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
      aMap.SetFlightplan( _flightPlanHandler.FlightPlan );
    }

    // fires when the Map Range has changed
    private void AMap_MapRangeChanged( object sender, MapEventArgs e )
    {
      // no action (so far)
    }

    // fires when the user want to teleport the acft
    private void AMap_TeleportAircraft( object sender, TeleportEventArgs e )
    {
      // go to a safe altitude first
      float msa = MSALib.MSA.Msa_ft( e.Lat, e.Lon, true );
      float thisMsa = MSALib.MSA.Msa_ft( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ), true );
      float gotoAlt = Math.Max( msa, thisMsa ); // clear terrain here and at destination
      if (e.AltIsMSL) {
        gotoAlt = Math.Max( gotoAlt, e.Altitude_ft ); // if MSL is asked max again
      }
      SV.Set<float>( SItem.fGS_Acft_AltMsl_ft, gotoAlt ); // should be at least clear of this and destination terrain

      // only then move
      SV.Set<double>( SItem.dGS_Acft_Lat, e.Lat );
      SV.Set<double>( SItem.dGS_Acft_Lon, e.Lon );
      // got to asked altitude now
      if (e.AltIsMSL) {
        if (e.Altitude_ft != gotoAlt) {
          // if not there yet
          SV.Set<float>( SItem.fGS_Acft_AltMsl_ft, e.Altitude_ft );
        }
      }
      else {
        SV.Set<float>( SItem.fGS_Acft_AltAoG_ft, e.Altitude_ft );
      }
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
        txEntry.Text = c_airportNA;
        // don't change the _airport when nothing is found
      }
      else {
        txEntry.ForeColor = _txForeColorDefault; // clear the one not available
        _airport = apt;
        LoadAirport( );

        string dbSource = Folders.FS2024Used ? "MSFS2024" : "MSFS2020";
        var aptReport = new AptReport.AptReportTable( );
        aptReport.SaveDocument( _airport, _navaidList, _fixList, dbSource,
                            Path.Combine( AppSettings.Instance.ShelfFolder, Folders.AptReportSubfolder ) );
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


    #endregion

    #region Data Arrival Callback

    /// <summary>
    /// Handle Data Arrival from Sim
    /// </summary>
    /// <param name="refName">Data Reference Name that called the update</param>
    public void OnDataArrival( string refName )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return; // cannot..

      // Map update pace slowed down to an acceptable CPU load - (native is 100ms)
      if ((_updates++ % 5) == 0) { // 500ms pace - slow enough ?? performance penalty...
                                   // track waypoints on data arrival
        _tAircraft.AircraftID = SV.Get<string>( SItem.sG_Cfg_AircraftID );
        _tAircraft.OnGround = SV.Get<bool>( SItem.bG_Sim_OnGround );
        _tAircraft.TrueHeading_deg = SV.Get<float>( SItem.fG_Nav_HDG_true_deg );
        _tAircraft.Heading_degm = SV.Get<float>( SItem.fG_Nav_HDG_mag_degm );
        _tAircraft.Position = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ), SV.Get<float>( SItem.fGS_Acft_AltMsl_ft ) );
        _tAircraft.AltitudeMsl_ft = SV.Get<float>( SItem.fGS_Acft_AltMsl_ft );
        _tAircraft.AltitudeIndicated_ft = SV.Get<float>( SItem.fG_Acft_Altimeter_ft );

        // limit RA visible to an altitude and if not on ground
        var _raLimit_ft = ((SV.Get<EngineType>( SItem.etG_Cfg_EngineType ) == EngineType.Jet)
                        || (SV.Get<EngineType>( SItem.etG_Cfg_EngineType ) == EngineType.Turboprop)) ? c_raAirliner_ft : c_raDefault_ft;
        var ra_ft = SV.Get<float>( SItem.fGS_Acft_AltAoG_ft );
        _tAircraft.RadioAlt_ft = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN
                                                                       : ra_ft <= _raLimit_ft ? ra_ft
                                                                                              : float.NaN;

        _tAircraft.Ias_kt = SV.Get<float>( SItem.fG_Acft_IAS_kt );
        _tAircraft.Tas_kt = SV.Get<float>( SItem.fG_Acft_TAS_kt );
        _tAircraft.Gs_kt = SV.Get<float>( SItem.fG_Acft_GS_kt );
        _tAircraft.Vs_fpm = (int)(SV.Get<float>( SItem.fG_Acft_VS_ftPmin ) / 20) * 20; // 20 fpm steps only
        _tAircraft.Fpa_deg = SV.Get<float>( SItem.fG_Acft_FlightPathAngle_deg );
        // GPS does not provide meaningful track values when not moving
        _tAircraft.Trk_degm = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN : SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm );
        _tAircraft.TrueTrk_deg = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? float.NaN : SV.Get<float>( SItem.fG_Gps_GTRK_true_deg );

        _tAircraft.DistanceToTOD_nm = SV.Get<float>( SItem.fG_Gps_TOD_dist_nm );
        _tAircraft.WindSpeed_kt = SV.Get<float>( SItem.fG_Acft_WindSpeed_kt );
        _tAircraft.WindDirection_deg = SV.Get<float>( SItem.fG_Acft_WindDirection_deg );

        // update the map
        aMap.UpdateAircraft( _tAircraft );
        UpdateAiAircrafts( );
        UpdateProfileData( );
        UpdateEnergyData( );
      }
    }

    // AI update
    private void UpdateAiAircrafts( )
    {
      // Handle AI aircrafts
      var SCI = SC.SimConnectClient.Instance;
      var aiList = new List<ITrackedAircraft>( );
      if (SCI.AiAcftPoolModule.Enabled) {
        if ((_updatesAI++ % 1) == 0) { // 1000ms pace - slow enough ?? performance penalty...
          foreach (var aiAcftID in SCI.AiAcftPoolModule.AiAircrafts) {
            var srcItem = SCI.AiAcftPoolModule.AiAircraftProps( aiAcftID );
            if (!srcItem.Valid) continue; // not a valid item

            LatLon lla = new LatLon( srcItem.Lat, srcItem.Lon, srcItem.AltMsl_ft );
            TcasFlag tc = (srcItem.AltMsl_ft > (_tAircraft.AltitudeMsl_ft + 1000f)) ? TcasFlag.Above
                  : (srcItem.AltMsl_ft < (_tAircraft.AltitudeMsl_ft - 1000f)) ? TcasFlag.Below
                  : TcasFlag.Level;
            if (tc == TcasFlag.Level) {
              if (lla.DistanceTo( _tAircraft.Position, CoordLib.ConvConsts.EarthRadiusNm ) < 15) {
                tc = TcasFlag.ProximityLevel; // within 1000ft and 15nm
              }
            }
            // finally add to tracking list
            var dstItem = new TrackedAircraftCls( ) {
              AircraftID = srcItem.Atc_ID,
              IsHeli = srcItem.IsHeli,
              TrueHeading_deg = srcItem.HDG_deg,
              Heading_degm = srcItem.HDG_degm,
              Position = lla,
              AltitudeMsl_ft = srcItem.AltMsl_ft,
              RadioAlt_ft = srcItem.AltRA_ft,
              Gs_kt = srcItem.GS_kt,
              Vs_fpm = srcItem.VS_fpm,
              TCAS = tc,
              OnGround = srcItem.OnGround,
            };
            aiList.Add( dstItem );
          }
          aMap.UpdateAircraftsAI( aiList ); // submit
        }
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

    #region FlighPlan Data Events

    // FP Handler signals arrival of a Flightplan
    private void _flightPlanHandler_FlightPlanArrived( object sender, FlightPlanArrivedEventArgs e )
    {
      lblCfgPlanMessage.Text = _flightPlanHandler.LoadMessage;

      // sanity when failed
      if (!e.Success) return;

      // preselect airports
      txCfgDep.Text = _flightPlanHandler.FlightPlan.Origin.Icao_Ident;
      var apt = GetAirport( txCfgDep.Text );
      if (apt == null) {
        txCfgDep.ForeColor = Color.Red;
        this.DEP_Airport = c_airportNA;
        lblCfgDep.Text = this.DEP_Airport;
      }
      else {
        txCfgDep.ForeColor = Color.GreenYellow;
        this.DEP_Airport = txCfgDep.Text;
        lblCfgDep.Text = apt.Name;
      }

      txCfgArr.Text = _flightPlanHandler.FlightPlan.Destination.Icao_Ident;
      apt = GetAirport( txCfgArr.Text );
      if (apt == null) {
        txCfgArr.ForeColor = Color.Red;
        this.ARR_Airport = c_airportNA;
        lblCfgArr.Text = this.ARR_Airport;
      }
      else {
        txCfgArr.ForeColor = Color.GreenYellow;
        this.ARR_Airport = txCfgArr.Text;
        lblCfgArr.Text = apt.Name;
      }

      // Set Map Route
      aMap.SetFlightplan( _flightPlanHandler.FlightPlan );
      // Announce plan loading
      OnFlightPlanLoadedByUser( );

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
      var tDdata = PerfTracker.Instance.InitialTD;

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
      RTF.WriteTab( $"{SV.Get<float>( SItem.fG_Acft_AcftCGlong_perc ):#0.00} %  /  {SV.Get<float>( SItem.fG_Acft_AcftCGlat_perc ):#0.00} %" );
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
      RTF.Write( $"Vertical" ); RTF.WriteTab( $"{tDdata.LandingPerf.TdRate_fpm:#,##0} fpm" ); RTF.WriteLn( );
      RTF.Write( $"G's" ); RTF.WriteTab( $"{tDdata.LandingPerf.TdGValue:0.00} G" ); RTF.WriteLn( );
      RTF.Write( $"Pitch" ); RTF.WriteTab( $"{tDdata.LandingPerf.TdPitch_deg:##0.0}°" ); RTF.WriteLn( );
      RTF.Write( $"Bank" ); RTF.WriteTab( $"{tDdata.LandingPerf.TdBank_deg:##0.0}°" ); RTF.WriteLn( );

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
      lblAlt_P.Text = $"{_tAircraft.AltitudeMsl_ft:##,##0}";
      lblIAS_P.Text = $"{_tAircraft.Ias_kt:##0}";
      lblTAS_P.Text = $"{_tAircraft.Tas_kt:##0}";
      lblFPA_P.Text = $"{_tAircraft.Fpa_deg:#0.0}";
      // mark the Row entries
      MarkGS( _tAircraft.Gs_kt );
      MarkAlt( _tAircraft.AltitudeMsl_ft );
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
        SV.Get<float>( SItem.fGS_Acft_AltMsl_ft ),
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
      lblAlt_E.Text = $"{_tAircraft.AltitudeMsl_ft:##,##0}";
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
          this.DEP_Airport = c_airportNA;
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
          this.ARR_Airport = c_airportNA;
          lblCfgArr.Text = this.ARR_Airport;
        }
        else {
          txCfgArr.ForeColor = Color.GreenYellow;
          this.ARR_Airport = txCfgArr.Text;
          lblCfgArr.Text = apt.Name;
        }
      }
    }

    // Clear AI filter clicked
    private void btAiFilterClear_Click( object sender, EventArgs e )
    {
      txAiFilter.Text = "";
    }

    // Pilot ID KeyPress
    private void txCfgSbPilotID_KeyPress( object sender, KeyPressEventArgs e )
    {
      txCfgSbPilotID.ForeColor = _txForeColorDefault; // clear the one not available
      if (e.KeyChar == (char)Keys.Return) {
        e.Handled = true;
        if (SimBriefHandler.IsSimBriefUserID( txCfgSbPilotID.Text )) {
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
      lblCfgPlanMessage.Text = "...";
      if (SimBriefHandler.IsSimBriefUserID( txCfgSbPilotID.Text )) {
        lblCfgPlanMessage.Text = "loading...";
        _flightPlanHandler.RequestSBDownload( txCfgSbPilotID.Text );
        // will report in the Event
      }
      else {
        lblCfgPlanMessage.Text = "invalid Pilot ID format (->2..7 digit)";
      }
    }

    // Select and load a plan
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
        lblCfgPlanMessage.Text = "loading...";
        if (!_flightPlanHandler.RequestPlanFile( OFD.FileName )) {
          lblCfgPlanMessage.Text = "unknown file, aborted";
        }
        // will report in the Event
      }
    }

    // MSFS LoadDefaultPlan pressed
    private void btCfgMsLoadPlan_Click( object sender, EventArgs e )
    {
      lblCfgPlanMessage.Text = "loading...";
      // call for a XML OFP
      if (!_flightPlanHandler.RequestPlanFile( MSFSPlnHandler.CustomFlightPlan_filename )) {
        lblCfgPlanMessage.Text = "unknown file, aborted";
      }
      ;
      // will report in the Event
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

    #region SimConnectClient chores (Standalone Mode)

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
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival, this );
      }
      // Init Landing Performance Tracker to add an observer
      _ = FShelf.LandPerf.PerfTracker.Instance;
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
