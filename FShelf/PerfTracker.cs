using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

namespace FShelf
{
  /// <summary>
  /// Performance tracker 
  /// Writes the Touchdown Data to a file
  /// </summary>
  internal class PerfTracker
  {
    private readonly string c_PerfFile = "TouchDownLog.csv";

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
    public float Rate_fpm => Conversions.Fpm_From_Fps( _tdRate_fps );
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
      _tdRate_fps = SC.SimConnectClient.Instance.AircraftTrackingModule.TouchDownVS_fps; // set current
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
      if (string.IsNullOrEmpty( SC.SimConnectClient.Instance.AircraftTrackingModule.AcftConfigFile )) {
        // no aircraft in use - usually at init or while changing flights etc.
        Reset( );
        return;
      }

      var simData = SC.SimConnectClient.Instance.AircraftTrackingModule;

      // track while in air
      if (!simData.Sim_OnGround) {
        _tdRwyLatDev_ft = simData.AtcRunwaySelected ? simData.AtcRunway_Displacement_ft : float.NaN;
        _tdRwyLonDev_ft = simData.AtcRunwaySelected ? simData.AtcRunway_Distance_ft : float.NaN;
      }

      if (_tdRate_fps != simData.TouchDownVS_fps) {
        // changed
        // try to avoid bouncing landings, get the first one i.e. wait 30 sec before registering the next one
        // and then only when moving faster than usual taxiing
        if ((simData.GS > 30) && (DateTime.Now > (_tdCapture + TimeSpan.FromSeconds( 30 )))) {
          _tdRate_fps = simData.TouchDownVS_fps;
          _tdPitch_deg = simData.TouchDownPitch_deg;
          _tdBank_deg = simData.TouchDownBank_deg;
          _tdHdg_degm = simData.TouchDownHdg_degm;
          _tdCapture = DateTime.Now;

          WriteTouchDown( );
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
      var simData = SC.SimConnectClient.Instance.AircraftTrackingModule;
      // append
      using (StreamWriter sw = new StreamWriter( tdFile, true )) {
        string log = $"{simData.AcftConfigFile};{simData.AcftID};{_tdCapture.ToString( "s" )}"
                  + $";{Rate_fpm:###0.0};{Pitch_deg:#0.0};{Bank_deg:#0.0};{Hdg_degm:000};{RwyLatDev:##0.0};{RwyLonDev:####0.0}";
        sw.WriteLine( log );
      }
    }


  }
}
