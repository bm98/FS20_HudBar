using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;
using static dNetBm98.Units;
using bm98_hbFolders;

using SC = SimConnectClient;
using static FSimClientIF.Sim;

using FSimClientIF.Modules;

namespace FShelf
{
  /// <summary>
  /// Performance tracker 
  /// Writes the Touchdown Data to a file
  /// </summary>
  internal sealed class PerfTracker
  {
    private readonly string c_PerfFile = "TouchDownLog.csv";

    // SimVar access
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    private float _tdRate_fps = 0;
    private float _tdPitch_deg = 0;
    private float _tdBank_deg = 0;
    private float _tdHdg_degm = 0;
    private float _tdRwyLatDev_ft = float.NaN;
    private float _tdRwyLonDev_ft = float.NaN;

    private DateTime _tdCapture = DateTime.Now - TimeSpan.FromSeconds( 60 ); // capture the current

    /// <summary>
    /// The TD rate in feet per sec
    /// </summary>
    public float Rate_fps => _tdRate_fps;
    /// <summary>
    /// The TD rate in feet per min
    /// </summary>
    public float Rate_fpm => (float)Fpm_From_Fps( _tdRate_fps );
    /// <summary>
    /// Pitch Angle
    /// </summary>
    public float Pitch_deg => _tdPitch_deg;
    /// <summary>
    /// Bank Angle
    /// </summary>
    public float Bank_deg => _tdBank_deg;
    /// <summary>
    /// Mag Heading
    /// </summary>
    public float Hdg_degm => _tdHdg_degm;
    /// <summary>
    /// Lateral Deviation from center in ft
    /// </summary>
    public float RwyLatDev => _tdRwyLatDev_ft;
    /// <summary>
    /// Longitudinal Deviation from TD point in ft
    /// </summary>
    public float RwyLonDev => _tdRwyLonDev_ft;

    /// <summary>
    /// Reset the tracker
    /// </summary>
    public void Reset( )
    {
      _tdRate_fps = SV.Get<float>( SItem.fG_Acft_TouchDown_VS_fps ); // set current
      _tdPitch_deg = 0;
      _tdBank_deg = 0;
      _tdHdg_degm = 0;
      _tdRwyLatDev_ft = float.NaN;
      _tdRwyLonDev_ft = float.NaN;
      _tdCapture = DateTime.Now - TimeSpan.FromSeconds( 60 ); // capture the current
    }

    /// <summary>
    /// Update from Sim data
    /// must be called by the owner 
    /// </summary>
    public void Update( )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (string.IsNullOrEmpty( SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ) )) {
        // no aircraft in use - usually at init or while changing flights etc.
        Reset( );
        return;
      }

      // track while in air
      if (!SV.Get<bool>( SItem.bG_Sim_OnGround )) {
        _tdRwyLatDev_ft = SV.Get<bool>( SItem.bG_Atc_RunwaySelected ) ? SV.Get<float>( SItem.fG_Atc_Runway_Displacement_ft ) : float.NaN;
        _tdRwyLonDev_ft = SV.Get<bool>( SItem.bG_Atc_RunwaySelected ) ? SV.Get<float>( SItem.fG_Atc_Runway_Distance_nm ) : float.NaN;
      }

      if (_tdRate_fps != SV.Get<float>( SItem.fG_Acft_TouchDown_VS_fps )) {
        // changed
        // try to avoid bouncing landings, get the first one i.e. wait 30 sec before registering the next one
        // and then only when moving faster than usual taxiing
        if ((SV.Get<float>( SItem.fG_Acft_GS_kt ) > 30) && (DateTime.Now > (_tdCapture + TimeSpan.FromSeconds( 30 )))) {
          _tdRate_fps = SV.Get<float>( SItem.fG_Acft_TouchDown_VS_fps );
          _tdPitch_deg = SV.Get<float>( SItem.fG_Acft_TouchDown_Pitch_deg );
          _tdBank_deg = SV.Get<float>( SItem.fG_Acft_TouchDown_Bank_deg );
          _tdHdg_degm = SV.Get<float>( SItem.fG_Acft_TouchDown_Heading_degm );

          _tdCapture = DateTime.Now;

          WriteTouchDownV2( );
        }
      }
    }

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
                  + $";{Rate_fpm:###0.0};{Pitch_deg:#0.0};{Bank_deg:#0.0};{Hdg_degm:000};{RwyLatDev:##0.0};{RwyLonDev:####0.0}";
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
                  + $";{Rate_fpm:###0.0};{Pitch_deg:#0.0};{Bank_deg:#0.0};{Hdg_degm:000};{RwyLatDev:##0.0};{RwyLonDev:####0.0};{GetAirport( SV.Get<double>( SItem.dG_Acft_Lat ), SV.Get<double>( SItem.dG_Acft_Lon ) )}";
        sw.WriteLine( log );
      }
    }

    // get the airport by Lat, Lon - the Sim one is from ATC and not always the one we land on...
    private string GetAirport( double lat, double lon )
    {
      return GetAirport( new LatLon( lat, lon ) );
    }

    // get the airport by LatLon - the Sim one is from ATC and not always the one we land on...
    private string GetAirport( LatLon pos )
    {
      using (var _db = new FSFData.DbConnection( ) { ReadOnly = true, SharedAccess = true }) {
        if (!_db.Open( Folders.GenAptDBFile )) return "n.a."; // no db available

        var aptList = _db.DbReader.AirportDescList_ByQuad( pos.AsQuad( 11 ) ).ToList( ); // 11=> 20km^2 field
        if (aptList.Count > 0) return aptList[0].Ident;
        else return "n.a."; // no Apt found
      }

    }


  }
}
