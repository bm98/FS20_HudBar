﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.MSFSFlt.FLTDEC;
using CoordLib;
using System.IO;

namespace FlightplanLib.MSFSFlt
{
  /// <summary>
  /// Get and decode MSFS FLT files
  ///  will take care of decorated B21 Soaring Plans
  /// </summary>
  public class MSFSFlt
  {
    /// <summary>
    /// Returns the generic FlighPlan from a FLT File
    /// </summary>
    /// <param name="msfsPlan">A FLT plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( FLT msfsPlan )
    {
      // create gen doc items
      var plan = new FlightPlan {
        Source = SourceOfFlightPlan.MS_Pln,
        Title = msfsPlan.Main.Title,
        CruisingAlt_ft = msfsPlan.ATC_Aircraft.CruisingAltitude,
        FlightPlanType = msfsPlan.ATC_Flightplan.FlightPlanType,
        RouteType = msfsPlan.ATC_Flightplan.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };
      // create Origin (for the runway assuming the First Waypoint is the Airport)
      var loc = new Location {
        Icao_Ident = new IcaoRec { ICAO = msfsPlan.ATC_Flightplan.DepartureICAO },
        Iata_Ident = "", // not available
        Name = msfsPlan.ATC_Flightplan.DepartureName,
        LatLonAlt_ft = msfsPlan.ATC_Flightplan.DEP_LatLon,
        RunwayNumber_S = msfsPlan.ATC_Aircraft.Waypoint( msfsPlan.ATC_Aircraft.Waypoints.First( ).Key ).RunwayNumber_S,
        RunwayDesignation = msfsPlan.ATC_Aircraft.Waypoint( msfsPlan.ATC_Aircraft.Waypoints.First( ).Key ).RunwayDesignation,
      };
      plan.Origin = loc;
      // create Destination (for the runway assuming the Last Waypoint is the Airport)
      loc = new Location( ) {
        Icao_Ident = new IcaoRec { ICAO = msfsPlan.ATC_Flightplan.DestinationICAO },
        Iata_Ident = "", // not available
        Name = msfsPlan.ATC_Flightplan.DestinationName,
        LatLonAlt_ft = msfsPlan.ATC_Flightplan.DST_LatLon,
        RunwayNumber_S = msfsPlan.ATC_Aircraft.Waypoint( msfsPlan.ATC_Aircraft.Waypoints.Last( ).Key ).RunwayNumber_S,
        RunwayDesignation = msfsPlan.ATC_Aircraft.Waypoint( msfsPlan.ATC_Aircraft.Waypoints.Last( ).Key ).RunwayDesignation,
      };
      plan.Destination = loc;
      // create waypoints
      var wypList = new List<Waypoint>( );
      foreach (var fixKey in msfsPlan.ATC_Aircraft.Waypoints.Keys) {
        var fix = msfsPlan.ATC_Aircraft.Waypoint( fixKey );
        if (!fix.IsValid) continue; // there are FLT Waypoints inserted by MS which are not decoded and return an empty Wyp
        // create Waypoint
        var wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          ID = fix.ID,
          Name = fix.Ident,
          LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.Altitude_ft ),
          Airway_Ident = fix.Airway_Ident,
          Frequency = "", // PLN has no Frequ
          Icao_Ident = new IcaoRec( ) { ICAO = fix.Ident, Region = fix.Region, AirportRef = fix.Airport, },
          InboundTrueTrk = -1, // need to calculate this
          OutboundTrueTrk = -1, // need to calculate this
          Distance_nm = -1, // need to calculate this
          SID_Ident = fix.SID_Ident,
          STAR_Ident = fix.STAR_Ident,
          ApproachType = fix.ApproachType,
          ApproachSuffix = fix.Approach_Suffix,
          RunwayNumber_S = fix.RunwayNumber_S,
          RunwayDesignation = fix.RunwayDesignation,
          Stage = "", // TODO not avail, need to calculate this
        };
        wypList.Add( wyp );
      }
      plan.Waypoints = wypList;
      // create Plan Doc HTML
      //  NA
      // create Download Images
      //  NA
      // create Download Documents
      //  NA

      // calculate missing items
      plan.RecalcWaypoints( );
      return plan;
      
    }


    /// <summary>
    /// Event Handler for MSFS PLN data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void MSFSFltDataEventHandler( object sender, MSFSFltDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS PLN data arrival
    /// </summary>
    public event MSFSFltDataEventHandler MSFSFltDataEvent;

    // Signal the user that and what data has arrived
    private void OnMSFSFltDataEvent( string data )
    {
      MSFSFltDataEvent?.Invoke( this, new MSFSFltDataEventArgs( data ) );
    }

    /// <summary>
    /// Returns the path for the CustomFlight.pln file in the current installation
    ///   or an empty string if not found
    /// </summary>
    public static string CustomFlightPlan_filename => MS.MsFolders.GetCustomFlight_Plan( );

    /// <summary>
    /// Post a SimBrief request for a user ID
    /// The caller received an SimBrief Event when finished
    /// </summary>
    /// <param name="fileName">The fully qualified filename</param>
    public void PostDocument_Request( string fileName )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( fileName )) return;
      if (!File.Exists( fileName )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( fileName );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    #region Asynch Request methods

    // Retrieve most current data
    private async Task GetData( string fileName )
    {
      string response = await Provider.FltRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        // signal response
        OnMSFSFltDataEvent( response );
      }
    }

    #endregion

  }
}
