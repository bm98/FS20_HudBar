using System;
using System.IO;
using System.Text;

using CoordLib;

using SC = SimConnectClient;

using FSimClientIF.Modules;
using static FSimClientIF.Sim;

using FSimFS20Folders;

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
  internal sealed class FpWrapper : FSimFlightPlanLib.Wrapper.FpWrapper
  {
    // - inherits the generic FpWrapper from the library

    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // SimVar Observer items
    private int _observerID = -1;
    private const string _observerName = "FP_WRAPPER";

    /// <summary>
    /// Fired when the Next Waypoint has changed
    /// </summary>
    public event EventHandler<WaypointNextChangedEventArgs> WaypointNextChanged;
    private void OnWaypointNextChanged( string nextWypID )
      => WaypointNextChanged?.Invoke( this, new WaypointNextChangedEventArgs( nextWypID ) );


    private readonly FS20_Folders _fs20FoldersRef;


    /// <summary>
    /// cTor:
    /// </summary>
    public FpWrapper( FS20_Folders fs20Folders )
      : base( fs20Folders.GenAptDBFile )
    {
      _fs20FoldersRef = fs20Folders;

      // register DataUpdates if in HudBar mode and if not yet done 
      if (_observerID < 0) {
        LOG.Trace( "FpWrapper.cTor", "Start Observing" );
        _observerID = SV.AddObserver( _observerName, 10, OnDataArrival, null ); // 1/sec
      }
    }

    /// <summary>
    /// Load SB Doc from Net
    /// </summary>
    /// <param name="sbUserID"></param>
    /// <returns></returns>
    public bool RequestSBDownload( string sbUserID )
    {
      return base.RequestPlanFromSBDownload( sbUserID, Path.Combine( AppSettings.Instance.ShelfFolder, FS20_Folders.FPlanPDF_FileName ) );
    }

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



    // save the last loaded plan to the MyDocuments Folder of HudBar
    protected override void DebSaveRouteString( string content, string ext )
    {
      var fName = $".\\LastPlanDownload.{ext}";
#if DEBUG
      fName = $".\\LastPlanDownload_{DateTime.Now:s}.{ext}".Replace( ":", "_" );
#endif
      // shall never fail...
      try {
        // save to current Dir while in debug
        var fname = Path.Combine( _fs20FoldersRef.HudBarUserFilePath, fName );
        // Write UTF8 with BOM
        using (var sw = new StreamWriter( fname, false, new UTF8Encoding( true ) )) {
          sw.WriteLine( content );
        }
      }
      catch { }
    }

  }
}
