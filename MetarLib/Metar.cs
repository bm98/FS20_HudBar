using System.Collections.Generic;
using System.Threading.Tasks;

using MetarLib.Provider;
using CoordLib;

namespace MetarLib
{
  /// <summary>
  /// Provides Access to METAR Data 
  /// 
  /// </summary>
  public class Metar
  {
    // Ranges we will scan while no results are returned
    private static readonly List<int> c_MaxRangeSM = new List<int> { 50, 100, 250, 500, 750 };

    /// <summary>
    /// The METAR Message provider 
    /// </summary>
    public Providers MetarProvider { get; set; } = Providers.AviationWeatherDotGov;

    /// <summary>
    /// Event Handler for METAR data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void MetarDataEventHandler( object sender, MetarTafDataEventArgs e );

    /// <summary>
    /// Event triggered on METAR data arrival
    /// </summary>
    public event MetarDataEventHandler MetarDataEvent;

    // Signal the user that and what data has arrived
    private void OnMetarDataEvent( MetarTafDataList data )
    {
      MetarDataEvent?.Invoke( this, new MetarTafDataEventArgs( data ) );
    }

    /// <summary>
    /// Post a METAR request for a station (ICAO code)
    /// </summary>
    /// <param name="station">The ICAO Station Name</param>
    public void PostMETAR_Request( string station )
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( station );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }


    /// <summary>
    /// Post a METAR request for a Position (lat/lon) with range (Statute Miles)
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    public void PostMETAR_Request( LatLon latLon )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( latLon );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }


    /// <summary>
    /// Post a METAR request for a position and bearing with range (Statute Miles)
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    /// <param name="bearing">The bearing to fly to</param>
    public void PostMETAR_Request( LatLon latLon, float bearing )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( latLon, bearing );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a METAR request for a position and destination
    /// The caller received an METAR Event when finished
    /// NOTE: This gets all stations found along the full path - can be many !!
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    /// <param name="destLatLon">Destination Coordinate</param>
    public void PostMETAR_Request( LatLon latLon, LatLon destLatLon )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( latLon, destLatLon );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    #region Asynch Request methods

    /// <summary>
    /// Retrieve most current data for a Station
    /// </summary>
    private async Task GetData( string station )
    {
      var response = new MetarTafDataList( );
      switch (MetarProvider) {
        case Providers.AviationWeatherDotGov:
          response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( station );
          break;
        default: break;
      }
      // signal response
      OnMetarDataEvent( response );
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
        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( latLon, range );
            break;
          default: break;
        }
        if (response.Count > 0 && response.Valid)
          break; // We have found an entry.. Return
      }
      // signal response
      OnMetarDataEvent( response );
    }


    /// <summary>
    /// Retrieve most current data for a Location and Destination
    /// Try a number of ranges to retrieve data to not overlaod the server
    /// </summary>
    private async Task GetData( LatLon latLon, LatLon destLatLon )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;
      if (destLatLon.IsEmpty) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {
        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( latLon, destLatLon, range );
            break;
          default: break;
        }
        if (response.Count > 0 && response.Valid)
          break; // We have found an entry.. Return
      }
      // signal response
      OnMetarDataEvent( response );
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

        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( latLon, dest, range );
            break;
          default: break;
        }
        if (response.Count > 0 && response.Valid)
          break; // We have found an entry.. Return
      }
      // signal response
      OnMetarDataEvent( response );
    }

    #endregion

  }
}
