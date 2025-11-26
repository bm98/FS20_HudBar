using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoordLib;
using MetarLib.Provider;

namespace MetarLib
{
  /// <summary>
  /// Provides Access to TAF Data 
  /// 
  /// </summary>
  public class Taf
  {
    // Ranges we will scan while no results are returned
    private static readonly List<int> c_MaxRangeSM = new List<int> { 50, 100, 250, 500, 750 };

    /// <summary>
    /// The TAF Message provider 
    /// </summary>
    public Providers TafProvider { get; set; } = Providers.AviationWeatherDotGov;

    /// <summary>
    /// Event Handler for TAF data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void MetarTafDataEventHandler( object sender, MetarTafDataEventArgs e );

    /// <summary>
    /// Event triggered on TAF data arrival
    /// </summary>
    public event MetarTafDataEventHandler TafDataEvent;

    // Signal the user that and what data has arrived
    private void OnTafDataEvent( MetarTafDataList data )
    {
      TafDataEvent?.Invoke( this, new MetarTafDataEventArgs( data ) );
    }

    /// <summary>
    /// Post a TAF request for a station (ICAO code)
    /// The caller received an TAF Event when finished
    /// If there is no record received it will try the lat, lon provided
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="station">The ICAO Station Name</param>
    public void PostTAF_Request( string station )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( station )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( station );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a TAF request for a Position (lat/lon) with range (Statute Miles)
    /// The caller received an TAF Event when finished
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    public void PostTAF_Request( LatLon latLon )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( latLon );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a TAF request for a position and bearing with range (Statute Miles)
    /// The caller received an TAF Event when finished
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    /// <param name="bearing">The bearing to fly to</param>
    public void PostTAF_Request( LatLon latLon, float bearing )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( latLon, bearing );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    #region Asynch Request methods

    /// <summary>
    /// Retrieve most current data for a Station
    /// </summary>
    private async Task GetData( string station )
    {
      var response = new MetarTafDataList( );
      switch (TafProvider) {
        case Providers.AviationWeatherDotGov:
          response = await Provider.AviationWeatherDotGov.TafRequest.GetTaf( station );
          break;
        default: break;
      }
      // signal response
      OnTafDataEvent( response );
    }

    /// <summary>
    /// Retrieve most current data for a Location
    /// Try a number of ranges to retrieve data to not overlaod the server
    /// </summary>
    private async Task GetData( LatLon latLon )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {
        switch (TafProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.TafRequest.GetTaf( latLon, range );
            break;
          default: break;
        }
        if (response.Count > 0 && response.Valid)
          break; // We have found an entry.. Return
      }
      // signal response
      OnTafDataEvent( response );
    }

    /// <summary>
    /// Retrieve most current data for a Location and Destination
    /// Try a number of ranges to retrieve data to not overlaod the server
    /// </summary>
    private async Task GetData( LatLon latLon, float bearing )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;
      if (float.IsNaN( bearing )) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {

        var dest = latLon.DestinationPoint( range * 2, bearing, ConvConsts.EarthRadiusSM ); // pt at end of range*2

        switch (TafProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.TafRequest.GetTaf( latLon, dest, range );
            break;
          default: break;
        }
        if (response.Count > 0 && response.Valid)
          break; // We have found an entry.. Return
      }
      // signal response
      OnTafDataEvent( response );
    }

    #endregion

  }
}
