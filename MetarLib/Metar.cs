﻿using System.Collections.Generic;
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
    /// If there is no record received it will try the lat, lon provided
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="station">The ICAO Station Name</param>
    /// <param name="lat">Latitude -90 .. +90 </param>
    /// <param name="lon">Longitude -180 .. +180</param>
    public void PostMETAR_Request( string station, double lat = double.NaN, double lon = double.NaN )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( station )) return;
      if (!double.IsNaN( lat ) && ((lat < -90.0) || (lat > 90.0))) return;
      if (!double.IsNaN( lat ) && ((lon < -180.0) || (lon > 180.0))) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( station, lat, lon );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a METAR request for a station (ICAO code)
    /// If there is no record received it will try the lat, lon provided
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="station">The ICAO Station Name</param>
    /// <param name="latLon">A LatLon location</param>
    public void PostMETAR_Request( string station, LatLon latLon )
    {
      if (latLon.IsEmpty)
        PostMETAR_Request( station );
      else
        PostMETAR_Request( station, latLon.Lat, latLon.Lon );
    }


    /// <summary>
    /// Post a METAR request for a Position (lat/lon) with range (Statute Miles)
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="lat">Latitude -90 .. +90 </param>
    /// <param name="lon">Longitude -180 .. +180</param>
    public void PostMETAR_Request( double lat, double lon )
    {
      // Sanity checks
      if (!double.IsNaN( lat ) && ((lat < -90.0) || (lat > 90.0))) return;
      if (!double.IsNaN( lat ) && ((lon < -180.0) || (lon > 180.0))) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( lat, lon );
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

      PostMETAR_Request( latLon.Lat, latLon.Lon );
    }


    /// <summary>
    /// Post a METAR request for a position and bearing with range (Statute Miles)
    /// The caller received an METAR Event when finished
    /// </summary>
    /// <param name="lat">Latitude -90 .. +90 </param>
    /// <param name="lon">Longitude -180 .. +180</param>
    /// <param name="bearing">The bearing to fly to</param>
    public void PostMETAR_Request( double lat, double lon, float bearing )
    {
      // Sanity checks
      if (!double.IsNaN( lat ) && ((lat < -90.0) || (lat > 90.0))) return;
      if (!double.IsNaN( lat ) && ((lon < -180.0) || (lon > 180.0))) return;
      if (!float.IsNaN( bearing ) && ((bearing < 0.0) || (bearing > 360.0))) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( lat, lon, bearing );
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

      PostMETAR_Request( latLon.Lat, latLon.Lon, bearing );
    }


    /// <summary>
    /// Post a METAR request for a position and destination
    /// The caller received an METAR Event when finished
    /// NOTE: This gets all stations found along the full path - can be many !!
    /// </summary>
    /// <param name="lat">Latitude -90 .. +90 </param>
    /// <param name="lon">Longitude -180 .. +180</param>
    /// <param name="destICAO">Destination ICAO station ID</param>
    public void PostMETAR_Request( double lat, double lon, string destICAO )
    {
      // Sanity checks
      if (!double.IsNaN( lat ) && ((lat < -90.0) || (lat > 90.0))) return;
      if (!double.IsNaN( lat ) && ((lon < -180.0) || (lon > 180.0))) return;
      if (string.IsNullOrWhiteSpace( destICAO )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( lat, lon, destICAO );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a METAR request for a position and destination
    /// The caller received an METAR Event when finished
    /// NOTE: This gets all stations found along the full path - can be many !!
    /// </summary>
    /// <param name="latLon">A LatLon location</param>
    /// <param name="destICAO">Destination ICAO station ID</param>
    public void PostMETAR_Request( LatLon latLon, string destICAO )
    {
      // Sanity checks
      if (latLon.IsEmpty) return;

      PostMETAR_Request( latLon.Lat, latLon.Lon, destICAO );
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
    /// Retrieve most current data for a Station
    /// If provided use the lat lon location if the station cannot return an answer
    /// Not provided means either of lat, lon is a NaN
    /// </summary>
    private async Task GetData( string station, double lat, double lon )
    {
      var response = new MetarTafDataList( );
      switch (MetarProvider) {
        case Providers.AviationWeatherDotGov:
          response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( station );
          break;
        default: break;
      }
      if (response.Count > 0 && response.Valid) {
        // signal response and end
        OnMetarDataEvent( response );
        return;
      }
      else {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        GetData( lat, lon ); // try the location
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      }
    }


    /// <summary>
    /// Retrieve most current data for a Location
    /// Try a number of ranges to retrieve data to not overlaod the server
    /// </summary>
    private async Task GetData( double lat, double lon )
    {
      // Sanity checks
      if (double.IsNaN( lat )) return;
      if (double.IsNaN( lon )) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {
        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( lat, lon, range );
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
    private async Task GetData( double lat, double lon, string dest )
    {
      // Sanity checks
      if (double.IsNaN( lat )) return;
      if (double.IsNaN( lon )) return;
      if (string.IsNullOrWhiteSpace( dest )) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {
        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( lat, lon, dest, range );
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
    private async Task GetData( double lat, double lon, float bearing )
    {
      // Sanity checks
      if (double.IsNaN( lat )) return;
      if (double.IsNaN( lon )) return;
      if (float.IsNaN( bearing )) return;

      var response = new MetarTafDataList( );
      foreach (var range in c_MaxRangeSM) {

        var pos = new LatLon( lat, lon );
        var dest = pos.DestinationPoint( range * 2, bearing, ConvConsts.EarthRadiusSM ); // pt at end of range*2

        switch (MetarProvider) {
          case Providers.AviationWeatherDotGov:
            response = await Provider.AviationWeatherDotGov.MetarRequest.GetMetar( lat, lon, dest.Lat, dest.Lon, range );
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
