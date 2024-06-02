using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;
using bm98_hbFolders;

using SC = SimConnectClient;
using static FSimClientIF.Sim;

using FSimClientIF.Modules;
using FSimFacilityIF;
using System.Diagnostics;
using System.Drawing;
using DbgLib;

namespace FShelf.LandPerf
{
  /// <summary>
  /// Performance tracker 
  /// Writes the Touchdown Data to a file
  /// Singleton
  /// </summary>
  public sealed class PerfTracker
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton var
    private static readonly Lazy<PerfTracker> lazy = new Lazy<PerfTracker>( ( ) => new PerfTracker( ) );

    /// <summary>
    /// The singleton instance of this class
    /// </summary>
    public static PerfTracker Instance { get { return lazy.Value; } }

    /// <summary>
    /// cTor:
    /// </summary>
    private PerfTracker( )
    {
      LPM = SC.SimConnectClient.Instance.LandingPerformanceModule;
      _observerID = LPM.AddObserver( "PerfTracker", 1, OnDataArrival, null ); // get every landing event
    }



    // SimVar access
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;
    // module Ref
    private readonly ILandingPerformance LPM;
    private int _observerID = -1;

    // maintain the local list 
    private List<PerfData> _landingList = new List<PerfData>( );

    /// <summary>
    /// HudBar Performance Data Record
    /// = amended generic LandingPerfData
    /// </summary>
    public struct PerfData
    {
      /// <summary>
      /// Generic Performance record
      /// </summary>
      public LandingPerfData LandingPerf;
      /// <summary>
      /// Touchdown count for this Landing
      /// 1= first, successive are bounces
      /// </summary>
      public int TdCount;
      /// <summary>
      /// Airport Ident or "n.a."
      /// </summary>
      public string AirportIdent;
      /// <summary>
      /// Ruwnway Ident or "n.a."
      /// </summary>
      public string RunwayIdent;
      /// <summary>
      /// Runway true bearing or NaN if no runway is detected
      /// </summary>
      public float RunwayBearing_deg;
      /// <summary>
      /// Runway width [m] or NaN if no runway is detected
      /// </summary>
      public float RunwayWidth_m;
      /// <summary>
      /// Runway length [m] or NaN if no runway is detected
      /// </summary>
      public float RunwayLength_m;
      /// <summary>
      /// TD point from Runway start [m] or NaN if no runway is detected
      /// </summary>
      public float TdDistance_m;
      /// <summary>
      /// TD displacement from Runway centerline [m] or NaN if no runway is detected
      /// </summary>
      public float TdDisplacement_m;
    }

    /// <summary>
    /// Number of TDs for the current landing
    /// </summary>
    public int TDCount => _landingList.Count;

    /// <summary>
    /// List of observed Landing (+ Bounces if there are)
    /// </summary>
    public IEnumerable<PerfData> LandingList => _landingList;

    /// <summary>
    /// The initial touchdown 
    /// </summary>
    public PerfData InitialTD => _landingList.FirstOrDefault( );

    /// <summary>
    /// Write the Landing image of the initial (1st) TD to disk
    /// </summary>
    public bool WriteLandingImage( )
    {
      // sanity
      if (TDCount < 1) return false; // nothing to write

      // using the first
      var lImage = new LandingImage( InitialTD, 0, 0 ); // get native image size
      Image image = lImage.AsImage( );
      if (image != null) {
        // never fail writing to disk
        try {
          string lImageName = $"{InitialTD.LandingPerf.TdTimeStamp.ToString( "s" ).Replace( ":", "_" )}-{InitialTD.AirportIdent}-{InitialTD.RunwayIdent}.jpg";
          image.Save( Path.Combine( Folders.LandingsPath, lImageName ), System.Drawing.Imaging.ImageFormat.Jpeg );
          return true;
        }
        catch { }
        finally {
          image.Dispose( );
        }
      }
      return false;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (LPM.LandingDataCount == 0) return; // ignore Reset Event

      var pd = CreateRecord( LPM.LandingData.LastOrDefault( ) );// get the lastest if there are bounces
      pd.TdCount = LPM.LandingDataCount; // amend record

      if (LPM.LandingDataCount == 1) {
        // reset - first TD
        _landingList.Clear( );
      }
      _landingList.Add( pd );

      WriteTouchDownV3( );
    }


    /* 
     * PREVIOUS VERSIONS
     * 
    private void WriteTouchDown( )
    {
      var tdFile = Path.Combine( Folders.UserFilePath, c_PerfFile );
      if (!File.Exists( tdFile )) {
        // write header when a new file is created
        using (StreamWriter sw = new StreamWriter( tdFile, true )) {
          sw.WriteLine( $"Aircraft;Callsign;Date_Time;VRate_fpm;Pitch_deg;Bank_deg;Hdg_degm;RwyLatDev_ft;RwyLonDev_ft" );
        }
      }
      // append
      using (StreamWriter sw = new StreamWriter( tdFile, true )) {
        string log = $"{SV.Get<string>( SItem.sG_Cfg_AcftConfigFile )};{SV.Get<string>( SItem.sG_Cfg_AircraftID )};{_tdCapture:s}"
                  + $";{VRate_fpm:###0.0};{Pitch_deg:#0.0};{Bank_deg:#0.0};{Hdg_degm:000};{RwyLatDev_ft:##0.0};{RwyLonDev_nm:####0.0}";
        sw.WriteLine( log );
      }
    }

    // V2 includes the Airport and runway if possible
    private void WriteTouchDownV2( )
    {
      var tdFile = Path.Combine( Folders.UserFilePath, c_PerfFile );
      if (!File.Exists( tdFile )) {
        // write header when a new file is created
        using (StreamWriter sw = new StreamWriter( tdFile, true )) {
          sw.WriteLine( $"Aircraft;Callsign;Date_Time;VRate_fpm;Pitch_deg;Bank_deg;Hdg_degm;RwyLatDev_ft;RwyLonDev_ft;Airport" );
        }
      }
      // append
      using (StreamWriter sw = new StreamWriter( tdFile, true )) {
        string log = $"{SV.Get<string>( SItem.sG_Cfg_AcftConfigFile )};{SV.Get<string>( SItem.sG_Cfg_AircraftID )};{_tdCapture:s}"
                  + $";{VRate_fpm:###0.0};{Pitch_deg:#0.0};{Bank_deg:#0.0};{Hdg_degm:000};{RwyLatDev_ft:##0.0};{RwyLonDev_nm:####0.0};{GetAirport( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) )}";
        sw.WriteLine( log );
      }
    }
    */

    // create one new record
    private PerfData CreateRecord( LandingPerfData perfData )
    {
      PerfData pd = new PerfData( ) { LandingPerf = perfData };

      // amend record with local data
      LatLon acftPos = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) );
      // init unknown
      pd.AirportIdent = "n.a.";
      pd.RunwayIdent = "n.a.";
      pd.RunwayBearing_deg = float.NaN;
      pd.RunwayWidth_m = float.NaN;
      pd.RunwayLength_m = float.NaN;
      pd.TdDistance_m = float.NaN;
      pd.TdDisplacement_m = float.NaN;

      IAirport apt = GetAirport( acftPos );
      if (apt != null) {
        pd.AirportIdent = apt.Ident;

        IRunway rwy = GetRunway( apt, acftPos, perfData.TdGTRK_deg );
        if (rwy != null) {
          pd.RunwayIdent = rwy.Ident;
          pd.RunwayBearing_deg = rwy.Bearing_deg;
          pd.RunwayWidth_m = rwy.Width_m;
          pd.RunwayLength_m = rwy.Length_m;

          // distance from RW start (ignoring the small error introduced by displacement from centerline)
          pd.TdDistance_m = (float)rwy.StartCoordinate.DistanceTo( acftPos );

          // displacement from RW center line
          pd.TdDisplacement_m = (float)acftPos.CrossTrackDistanceTo(
            rwy.StartCoordinate.DestinationPoint( -1_000, rwy.Bearing_deg ), // a point behind the rw start
            rwy.StartCoordinate.DestinationPoint( 5_000, rwy.Bearing_deg ) // a point beyond the rw end
          );
        }
      }

      return pd;
    }


    // V3 includes some more items
    private void WriteTouchDownV3( )
    {
      // sanity
      if (_landingList.Count == 0) return;

      var lPerf = _landingList.LastOrDefault( ); // write the latest event

      var tdFile = Folders.LandingsFile;
      // never fail on this sequence
      try {
        if (!File.Exists( tdFile )) {
          // write header when a new file is created
          using (StreamWriter sw = new StreamWriter( tdFile, true )) {
            sw.WriteLine( "Aircraft;Callsign;Date_Time;"
                          + "VRate_fpm;GSpeed_kt;Gs_G;SSlip_deg;"
                          + "Pitch_deg;Bank_deg;Yaw_deg;Hdg_degm;GTrk_degm;IAS_Kt;"
                          + "RwyLatDev_m;RwyLonDev_m;"
                          + "WindDir_deg;WindSpeed_kt;"
                          + "Airport;Runway;TDs" );
          }
        }
        // append to file
        using (StreamWriter sw = new StreamWriter( tdFile, true )) {
          string log = $"{lPerf.LandingPerf.TdAcftTitle};{lPerf.LandingPerf.TdAcftID};{lPerf.LandingPerf.TdTimeStamp:s};"
                    + $"{lPerf.LandingPerf.TdRate_fpm:###0.0};{lPerf.LandingPerf.TdGS_kt:##0};{lPerf.LandingPerf.TdGValue:0.00};{lPerf.LandingPerf.TdSideslipAngle_deg:#0.0};"
                    + $"{lPerf.LandingPerf.TdPitch_deg:#0.0};{lPerf.LandingPerf.TdBank_deg:#0.0};{lPerf.LandingPerf.TdYaw_deg:#0.0};"
                    + $"{lPerf.LandingPerf.TdHdg_degm:000};{lPerf.LandingPerf.TdGTRK_degm:000};{lPerf.LandingPerf.TdIAS_kt:000};"
                    + $"{lPerf.TdDisplacement_m:##0};{lPerf.TdDistance_m:####0};"
                    + $"{lPerf.LandingPerf.TdWindDir_deg:000};{lPerf.LandingPerf.TdWindSpeed_kt:#0};"
                    + $"{lPerf.AirportIdent};{lPerf.RunwayIdent};"
                    + $"{lPerf.TdCount};";

          sw.WriteLine( log );
          LOG.Log( $"PerfTracker.WriteTouchDownV3 - <{log}>" );
        }
      }
      catch { }
    }

    // get the airport by LatLon - the Sim one is from ATC and not always the one we land on...
    // return null if not found
    private IAirport GetAirport( LatLon acftPos )
    {
      using (var _db = new FSFData.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile )) {
          LOG.LogError( $"PerfTracker.GetAirport - No Apt found: DB file not available" );
          return null; // no db available
        }
        var apt = _db.DbReader.GetAirport_ByLatLon( acftPos );
        LOG.Log( $"PerfTracker.GetAirport - EVAL airport as <{((apt == null) ? "not found" : apt.Ident)}>" );
        return apt;
      }
    }

    // return a possible runway or null
    private IRunway GetRunway( IAirport apt, LatLon acftPos, float gtrk_deg )
    {
      // sanity 
      if (apt == null) return null;

      // runways where the bearing of the RW start towards the acft matches about the ground track

      // select runways with bearing of the track (detecting landings across 60m wide to 230+m dist)
      var possibleRwyList = apt.Runways.Where( rw => dNetBm98.XMath.AboutEqual( rw.Bearing_deg, gtrk_deg, 15 ) ); // +- 15 deg
      LOG.Log( $"PerfTracker.GetRunway - EVAL from <{possibleRwyList.Count( )}> possible runways" );
      if (possibleRwyList.Count( ) == 0) {
        LOG.Log( $"  no Runway found for apt <{apt.Ident}> gtrk <{gtrk_deg:000}> " );
        return null; // no match
      }
      if (possibleRwyList.Count( ) == 1) {
        LOG.Log( $"  One Runway found for apt <{apt.Ident}> gtrk <{gtrk_deg:000}> - {possibleRwyList.FirstOrDefault( ).Ident} " );
        return possibleRwyList.FirstOrDefault( ); // one match
      }

      // select from parallel runways the one where rw-start-1000m and acft pos lineup with the runway
      var selectedRwyList = possibleRwyList.Where( rw =>
        dNetBm98.XMath.AboutEqual( rw.StartCoordinate.DestinationPoint( -1000, rw.Bearing_deg ).BearingTo( acftPos ), rw.Bearing_deg, 9 ) ); // +- 9 deg
      LOG.Log( $"PerfTracker.GetRunway - EVAL from <{selectedRwyList.Count( )}> selected runways" );

      if (selectedRwyList.Count( ) == 0) {
        LOG.Log( $"  no Runway matched for apt <{apt.Ident}> gtrk <{gtrk_deg:000}> " );
        return default; // no match
      }
      if (selectedRwyList.Count( ) == 1) {
        LOG.Log( $"  One Runway matched for apt <{apt.Ident}> gtrk <{gtrk_deg:000}> - {selectedRwyList.FirstOrDefault( ).Ident} " );
        return selectedRwyList.FirstOrDefault( ); // one match
      }

      // if still more than one is possible, find the one where the rw start is closest to the acft pos
      IRunway rwy = null;
      double dist = double.MaxValue;
      foreach (var rw in selectedRwyList) {
        var d = acftPos.DistanceTo( rw.StartCoordinate );
        if (d < dist) {
          // this one is closer
          rwy = rw;
          dist = d;
        }
      }
      LOG.Log( $"  Returned closest Runway for apt <{apt.Ident}> gtrk <{gtrk_deg:000}> - {rwy.Ident} " );
      return rwy;
    }



  }
}
