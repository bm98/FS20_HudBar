using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbgLib;

using CoordLib;

using SC = SimConnectClient;

using FSimFlightPlans;
using FSimFlightPlans.MSFSPln;
using FSimFlightPlans.MSFSFlt;
using FSimFlightPlans.SimBrief;
using FSimFlightPlans.GPX;
using FSimFlightPlans.RTE;
using FSimFlightPlans.LNM;
using bm98_hbFolders;
using FSimClientIF.Modules;
using static FSimClientIF.Sim;

namespace FShelf.FPlans
{

  /// <summary>
  /// A Wrapper for the different plans
  /// 
  /// Observes the related variables and maintains the tracking
  /// 
  ///  Note: Usually there is only one instance in use for an Application
  ///  
  /// </summary>
  internal sealed class FpWrapper
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );


    // holds the currently loaded plan 
    private FlightPlan _fPlan = new FlightPlan( );

    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // SimVar Observer items
    private int _observerID = -1;
    private const string _observerName = "FP_WRAPPER";

    // SimBrief Provider
    private readonly SimBrief _simBrief;
    // SB Helper
    private SbDocLoader _sbDocLoader = new SbDocLoader( );

    // Other Pln Providers
    private readonly MSFSPln _msfsPln;
    private readonly MSFSFlt _msfsFlt;
    private readonly LNMpln _lnmPln;
    private readonly GPXpln _lnmGpx;
    private readonly RTEpln _lnmRte;

    private string _lastRequestedFile = ""; // debug info 

    /// <summary>
    /// Fired when a Flightplan has arrived
    /// </summary>
    public event EventHandler<FlightPlanArrivedEventArgs> FlightPlanArrived;
    private void OnFlightPlanArrived( bool success, SourceOfFlightPlan source, string loadMessage )
      => FlightPlanArrived?.Invoke( this, new FlightPlanArrivedEventArgs( success, source, loadMessage ) );

    /// <summary>
    /// Fired when the Next Waypoint has changed
    /// </summary>
    public event EventHandler<WaypointNextChangedEventArgs> WaypointNextChanged;
    private void OnWaypointNextChanged( string nextWypID )
      => WaypointNextChanged?.Invoke( this, new WaypointNextChangedEventArgs( nextWypID ) );


    /// <summary>
    /// True if a SimBrief Plan is in use
    /// </summary>
    public bool IsSbPlan => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.SimBrief);

    /// <summary>
    /// True if a MSFS PLN Plan is in use
    /// </summary>
    public bool IsMsFsPLN => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Pln);

    /// <summary>
    /// True if a MSFS FLT Plan is in use
    /// </summary>
    public bool IsMsFsFLT => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Flt);

    /// <summary>
    /// True if a LNM Exported Plan is in use
    /// </summary>
    public bool IsLnmPLN => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.LNM_Pln);
    /// <summary>
    /// True if a LNM GPX Exported Plan is in use
    /// </summary>
    public bool IsLnmGPX => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.LNM_Gpx);

    /// <summary>
    /// True if a Generic Route Plan is in use
    /// </summary>
    public bool IsGenRTE => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.GEN_Rte);


    /// <summary>
    /// Get: the FlightPlan
    /// </summary>
    public FlightPlan FlightPlan => _fPlan;

    /// <summary>
    /// cTor:
    /// </summary>
    public FpWrapper( )
    {
      // Flightplan decoders
      _simBrief = new SimBrief( );
      _simBrief.SimBriefDataEvent += _generic_FlightPlanDataEvent;
      _simBrief.SimBriefDownloadEvent += _simBrief_SimBriefDownloadEvent;

      _msfsPln = new MSFSPln( );
      _msfsPln.MSFSPlnDataEvent += _generic_FlightPlanDataEvent;
      _msfsFlt = new MSFSFlt( );
      _msfsFlt.MSFSFltDataEvent += _generic_FlightPlanDataEvent;

      _lnmPln = new LNMpln( );
      _lnmPln.LNMplnDataEvent += _generic_FlightPlanDataEvent;
      _lnmGpx = new GPXpln( );
      _lnmGpx.GPXplnDataEvent += _generic_FlightPlanDataEvent;

      _lnmRte = new RTEpln( );
      _lnmRte.RTEplnDataEvent += _generic_FlightPlanDataEvent;

      // register DataUpdates if in HudBar mode and if not yet done 
      if (SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
        LOG.Trace( "FpWrapper.cTor", "Start Observing" );
        _observerID = SV.AddObserver( _observerName, 10, OnDataArrival, this ); // 1/sec
      }
    }

    /// <summary>
    /// Request a SimBrief Download
    /// </summary>
    /// <param name="sbPilotID">SB User ID</param>
    public void RequestSBDownload( string sbPilotID )
    {
      LOG.Trace( "RequestSBDownload", "with ID" );

      _simBrief.PostDocument_Request( sbPilotID, FSimFlightPlans.FlightPlanDataFormat.JSON ); // USE HTTP and JSON
    }

    /// <summary>
    /// Request a FlightPlan file to be loadeded based on filename extension
    /// 
    /// PLN: .pln, FLT: .flt, LNM: .lnmpln, GPX: .gpx, RTE: .rte, SB: .json
    /// </summary>
    /// <param name="planFile">FlightPlan file fully qualified</param>
    /// <returns>True when loading started</returns>
    public bool RequestPlan( string planFile )
    {
      LOG.Trace( "RequestSBDownload", $"File: <{planFile}>" );

      _lastRequestedFile = planFile;
      if (planFile.ToLowerInvariant( ).EndsWith( ".pln" )) {
        _msfsPln.PostDocument_Request( planFile );
      }
      else if (planFile.ToLowerInvariant( ).EndsWith( ".flt" )) {
        _msfsFlt.PostDocument_Request( planFile );
      }
      else if (planFile.ToLowerInvariant( ).EndsWith( ".gpx" )) {
        _lnmGpx.PostDocument_Request( planFile );
      }
      else if (planFile.ToLowerInvariant( ).EndsWith( ".rte" )) {
        _lnmRte.PostDocument_Request( planFile );
      }
      else if (planFile.ToLowerInvariant( ).EndsWith( ".lnmpln" )) {
        _lnmPln.PostDocument_Request( planFile );
      }
      else if (planFile.ToLowerInvariant( ).EndsWith( ".json" )) {
        _simBrief.PostDocument_Request( planFile );
      }
      else {
        _lastRequestedFile = ""; // unknown type
        return false;
      }
      return true;
      // will report in the Event
    }


    #region FlighPlan Data Events

    /// <summary>
    /// FP Loading info
    /// </summary>
    public string LoadMessage { get; private set; } = "";

    // triggered on FlightPlan data arrival
    private void _generic_FlightPlanDataEvent( object sender, FSimFlightPlans.FlightPlanDataEventArgs e )
    {
      if (!e.Success) {
        LOG.Trace( "_generic_FlightPlanfDataEvent", $"Received invalid Flightplan Data for <{e.Source}>" );
        LoadMessage = "No data received";
        OnFlightPlanArrived( false, e.Source, LoadMessage );
        return; // EXIT
      }

      bool okFlag = false;
      switch (e.Source) {
        case FSimFlightPlans.SourceOfFlightPlan.SimBrief:
          DebSaveRouteString( e.PlanData, "json" );
          LoadMessage = "OFP data received";
          var sb = FSimFlightPlans.SimBrief.SBDEC.JsonDecoder.FromString( e.PlanData );
          if (sb.IsValid) {
            _fPlan = SimBrief.AsFlightPlan( sb );
            LoadMessage = $"OFP: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        case FSimFlightPlans.SourceOfFlightPlan.MS_Pln:
          DebSaveRouteString( e.PlanData, "PLN" );
          LoadMessage = "PLN data received";
          var pln = FSimFlightPlans.MSFSPln.PLNDEC.PlnDecoder.FromString( e.PlanData );
          if (pln.IsValid) {
            _fPlan = MSFSPln.AsFlightPlan( pln );
            LoadMessage = $"PLN: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        case FSimFlightPlans.SourceOfFlightPlan.MS_Flt:
          DebSaveRouteString( e.PlanData, "FLT" );
          LoadMessage = "FLT file received";
          var flt = FSimFlightPlans.MSFSFlt.FLTDEC.FltDecoder.FromString( e.PlanData );
          if (flt.IsValid) {
            _fPlan = MSFSFlt.AsFlightPlan( flt );
            LoadMessage = $"FLT: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        case FSimFlightPlans.SourceOfFlightPlan.LNM_Gpx:
          DebSaveRouteString( e.PlanData, "GPX" );
          LoadMessage = "GPX data received";
          var gpx = FSimFlightPlans.GPX.GPXDEC.GpxDecoder.FromString( e.PlanData );
          if (gpx.IsValid) {
            _fPlan = GPXpln.AsFlightPlan( gpx );
            LoadMessage = $"GPX: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        case FSimFlightPlans.SourceOfFlightPlan.LNM_Pln:
          DebSaveRouteString( e.PlanData, "LNM" );
          LoadMessage = "LNM data received";
          var lnm = FSimFlightPlans.LNM.LNMDEC.LnmPlnDecoder.FromString( e.PlanData );
          if (lnm.IsValid) {
            _fPlan = LNMpln.AsFlightPlan( lnm );
            LoadMessage = $"LNM: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        case FSimFlightPlans.SourceOfFlightPlan.GEN_Rte:
          DebSaveRouteString( e.PlanData, "RTE" );
          LoadMessage = "RTE data received";
          var rte = FSimFlightPlans.RTE.RTEDEC.RteDecoder.FromString( e.PlanData );
          if (rte.IsValid) {
            _fPlan = RTEpln.AsFlightPlan( rte );
            LoadMessage = $"RTE: {FlightPlan.Origin.Icao_Ident} to {FlightPlan.Destination.Icao_Ident}";
            okFlag = true;
          }
          else {
            LoadMessage = $"No valid data received";
          }
          break;

        default:
          LoadMessage = "No valid data received";
          break;
      }

      // Load Shelf Docs of the plan was OK
      if (okFlag) {
        var err = GetAndSaveDocuments( AppSettings.Instance.ShelfFolder );
        if (!string.IsNullOrWhiteSpace( err )) { LoadMessage += $"\n{err}"; }
      }
      // inform caller
      OnFlightPlanArrived( okFlag, e.Source, LoadMessage );
    }

    // triggered when a Simbrief Document DL is completed
    private void _simBrief_SimBriefDownloadEvent( object sender, EventArgs e )
    {
      LOG.Trace( "_simBrief_SimBriefDownloadEvent", "Ping received" );
    }

    #endregion

    /// <summary>
    /// Handle Data Arrival from Sim
    /// </summary>
    /// <param name="refName">Data Reference Name that called the update</param>
    private void OnDataArrival( string refName )
    {
      // track current position in FlightPlan
      var acftPos = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ), SV.Get<float>( SItem.fGS_Acft_AltMsl_ft ) );
      _fPlan.Tracker.TrackAndMerge(
        SV.Get<double>( SItem.dG_Env_Time_zulu_sec ),
        SV.Get<bool>( SItem.bG_Sim_OnGround ),
        // FP track
        acftPos,
        SV.Get<float>( SItem.fG_Acft_GS_kt ),
        // GPS merge
        SV.Get<bool>( SItem.bG_Gps_FP_tracking ), // if a FP is handled by the GPS the tracker may merge values
        SV.Get<string>( SItem.sG_Gps_WYP_prevID ),
        SV.Get<string>( SItem.sG_Gps_WYP_nextID ),
        SV.Get<float>( SItem.fG_Gps_DEST_dist_nm ), // could be always total dist, not remaining..
        SV.Get<double>( SItem.dG_Gps_DEST_ete_sec ),
        SV.Get<float>( SItem.fG_Gps_WYP_dist_nm ),
        SV.Get<double>( SItem.dG_Gps_WYP_ete_sec ),
        SV.Get<float>( SItem.fG_Gps_WYP_XTRK_nm )
        );

      // announce WYP change
      if (_fPlan.Tracker.HasChangedWYP) {
        OnWaypointNextChanged( _fPlan.Tracker.ReadNextWYP( ) );
      }
    }

    /// <summary>
    /// Load the available Docs and saves them into the destination
    /// (acts on the valid plan)
    /// </summary>
    /// <param name="destLocation">The destination Folder</param>
    /// <returns>An empty string or an error string when not successfull</returns>
    private string GetAndSaveDocuments( string destLocation )
    {
      var sb = new StringBuilder( );
      if (!Directory.Exists( destLocation )) {
        sb.AppendLine( "ERROR: Destination folder does not exist" ); // should not happen or the user removed the Bag Folder
      }
      else {
        if (IsSbPlan) {
          // Load Shelf Docs from Simbrief
          if (!_sbDocLoader.GetFlightPlanPdf( FlightPlan, Path.Combine( destLocation, bm98_hbFolders.Folders.FPlanPDF_FileName ) )) {
            sb.AppendLine( "FP document is currently in use, could not change it" );
          }
        }
        else if (IsMsFsPLN) {
          ; // nothing
        }
        else if (IsMsFsFLT) {
          ; // nothing
        }
        else if (IsLnmGPX) {
          ; // nothing
        }
        else if (IsGenRTE) {
          ; // nothing
        }
        else if (IsLnmPLN) {
          ; // nothing
        }

        // the plan as table for all sources
        var fpTable = new FlightPlanTable( );
        if (!fpTable.SaveDocument( FlightPlan, Path.Combine( destLocation, bm98_hbFolders.Folders.FTablePDF_FileName ) )) {
          sb.AppendLine( "FP table is currently in use, could not change it" );
        }
      }

      LOG.Debug( "GetAndSaveDocuments", "ERR=" + sb.ToString( ) );

      return sb.ToString( );
    }

    // save the last loaded plan to the MyDocuments Folder of HudBar
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

  }
}
