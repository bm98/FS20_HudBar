using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;
using CoordLib.Extensions;
using CoordLib.MercatorTiles;

using bm98_Map;
using bm98_Map.Data;
using FSimFacilityIF;
using System.IO;

using MapLib;
using FlightplanLib;

namespace TEST_Map
{
  public partial class Form1 : Form
  {
    // HudBar file locations in MyDocuments
    private const string c_HudBarFolder = @"MSFS_HudBarSave";
    private readonly string c_HudBarDbFolder = Path.Combine( c_HudBarFolder, "db" );
    private readonly string c_HudBarCacheFolder = Path.Combine( c_HudBarFolder, "cache" );
    private readonly string c_HudBarSettingsFolder = Path.Combine( c_HudBarFolder, "settings" );
    // MyDocuments folder
    private readonly string c_MyDocuments = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

    private const string c_GenAptDBNameDblite = @"fs2020genApt.dblite";
    private string _genAptDBPath = "";


    public Form1( )
    {
      MapManager.Instance.InitMapLib( Path.Combine( c_MyDocuments, c_HudBarFolder ) ); // Init before anything else
      /* DEBUG SETTINGS
      MapManager.Instance.SetMemCacheStatus( false );  // disable Memory Cache here
      MapManager.Instance.SetDiskCacheStatus( false );  // disable Disk Cache here
      MapManager.Instance.SetProviderSource( true );  // disable Providers here
      */
      //
      InitializeComponent( );

      tlp.Dock = DockStyle.Fill;
      uC_Map1.Dock = DockStyle.Fill;

      uC_Map1.MapCenterChanged += UC_Map1_MapCenterChanged;
      uC_Map1.MapRangeChanged += UC_Map1_MapRangeChanged;

      _genAptDBPath = Path.Combine( c_MyDocuments, c_HudBarDbFolder ); // MyDocuments

      txAirport.Text = "KORD";
    }

    private void UC_Map1_MapRangeChanged( object sender, MapEventArgs e )
    {
      lblEvent.Text = $"R:{DateTime.Now.ToShortTimeString( )}";
      // Console.WriteLine( $"GGG  UC_Map1_MapRangeChanged:  {e.CenterCoordinate} - {e.MapRange}" );
    }

    private void UC_Map1_MapCenterChanged( object sender, MapEventArgs e )
    {
      lblEvent.Text = $"C:{DateTime.Now.ToShortTimeString( )}";
      // Console.WriteLine( $"GGG  UC_Map1_MapCenterChanged: {e.CenterCoordinate} - {e.MapRange}" );

    }

    private IAirport airport = null;

    private void button1_Click( object sender, EventArgs e )
    {
      uC_Map1.MapCreator.Reset( );
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = false }) {
        if (!_db.Open( Path.Combine( _genAptDBPath, c_GenAptDBNameDblite ) ))
          return; // no db available

        // airport = _db.DbReader.GetAirport( "NZSP" ); // very south cannot map
        //airport = _db.DbReader.GetAirport( "NZPG" ); // very south 
        //airport = _db.DbReader.GetAirport( "CYLT" ); // very north
        //airport = _db.DbReader.GetAirport( "NGTA" ); // at the Dateline & Aequator (almost)
        //airport = _db.DbReader.GetAirport( "NGFU" ); // at the Dateline
        //airport = _db.DbReader.GetAirport( "EGLL" ); // at the 0 Meridian
        //airport = _db.DbReader.GetAirport( "EGJJ" ); 
        //airport = _db.DbReader.GetAirport( "LSZH" ); // well known
        //airport = _db.DbReader.GetAirport( "KORD" ); // most runways
        //airport = _db.DbReader.GetAirport( "KSFO" );
        //airport = _db.DbReader.GetAirport( "LOWI" );
        //airport = _db.DbReader.GetAirport( "LSZU" ); // single runway
        //airport = _db.DbReader.GetAirport( "KOSH" );

        airport = _db.DbReader.GetAirport( txAirport.Text.ToUpperInvariant( ) );
      }
      if (airport != null) {
        uC_Map1.MapCreator.SetAirport( airport );
        _trackPt = airport.Coordinate;
        uC_Map1.MapCreator.Commit( );
      }
    }

    private void btZoomOut_Click( object sender, EventArgs e )
    {
      uC_Map1.ZoomOut( );
    }

    private void btZoomIn_Click( object sender, EventArgs e )
    {
      uC_Map1.ZoomIn( );
    }

    private void btRangeFar_Click( object sender, EventArgs e )
    {
      uC_Map1.MapRange = MapRange.Mid;
    }

    private void btRangeMid_Click( object sender, EventArgs e )
    {
      uC_Map1.MapRange = MapRange.Near;
    }

    private void btRangeNear_Click( object sender, EventArgs e )
    {
      uC_Map1.MapRange = MapRange.Close;
    }


    private TrackedAircraftCls trackedAircraft = new TrackedAircraftCls( ) {
      Position = new LatLon( 0, 0 ),
     TrueHeading_deg = 45,
      Altitude_ft = 0000,
      RadioAlt_ft = 2000,
      Ias_kt = 200,
      Vs_fpm = -500,
      Gs_kt = 220,
      Trk_degm = 46,
      TrueTrk_deg = 42,
      WindDirection_deg = -12,
      WindSpeed_kt = 0,
      ShowAircraft = true,
      ShowAircraftRange = true,
      ShowAircraftWind = true,
      ShowAircraftTrack = true,
    };

    private LatLon _trackPt = new LatLon( );

    private void btTrackAcft_Click( object sender, EventArgs e )
    {
      _trackPt.Lat += 0.001;
      _trackPt.Lon += 0.001;

      trackedAircraft.Position = _trackPt;
      uC_Map1.UpdateAircraft( trackedAircraft );
      // next click
      trackedAircraft.TrueHeading_deg += 5;
      trackedAircraft.Altitude_ft += 300;
      trackedAircraft.RadioAlt_ft -= 100;
      trackedAircraft.Ias_kt += 36;
      trackedAircraft.Gs_kt += 36;
      trackedAircraft.Trk_degm += 5;
      trackedAircraft.TrueTrk_deg += 5;
      trackedAircraft.Vs_fpm += 100;
      trackedAircraft.WindDirection_deg += 12;
      trackedAircraft.WindSpeed_kt += 5;
      trackedAircraft.ShowAircraftRange = true;
      trackedAircraft.ShowAircraftWind = true;
      trackedAircraft.ShowAircraftTrack = true;
    }

    private void btTogGrid_Click( object sender, EventArgs e )
    {
      uC_Map1.ShowMapGrid = !uC_Map1.ShowMapGrid;
    }


    private List<INavaid> NList( LatLon latLon )
    {
      var nList = new List<INavaid>( );
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = false }) {
        if (!_db.Open( Path.Combine( _genAptDBPath, c_GenAptDBNameDblite ) ))
          return nList; // no db available

        // get the the Quads around
        var qs = Quad.Around49EX( uC_Map1.MapCenter( ).AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        nList = _db.DbReader.Navaids_ByQuadList( qs ).ToList( );
      }
      // APT IFR Waypoints
      if (airport != null) {
        nList.AddRange( airport.Navaids.Where( x => x.IsWaypoint ) );
      }

      return nList;
    }

    private List<IAirportDesc> AList( LatLon latLon )
    {
      var aList = new List<IAirportDesc>( );
      using (var _db = new FSimFacilityDataLib.AirportDB.DbConnection( ) { ReadOnly = true, SharedAccess = false }) {
        if (!_db.Open( Path.Combine( _genAptDBPath, c_GenAptDBNameDblite ) ))
          return aList; // no db available

        // get the the Quads around
        var qs = Quad.Around49EX( uC_Map1.MapCenter( ).AsQuadMax( ).AtZoom( (int)MapRange.FarFar ) ); // FF level
        aList = _db.DbReader.AirportDescs_ByQuadList( qs ).ToList( );
      }
      return aList;
    }

    private void btNavaid_Click( object sender, EventArgs e )
    {
      uC_Map1.SetNavaidList( NList( airport.Coordinate ) );
      uC_Map1.SetAltAirportList( AList( airport.Coordinate ) );
    }

    private void btSetRoute_Click( object sender, EventArgs e )
    {
      Route route = new Route( );
      route.AddRoutePoint( new RoutePoint( "LMML", new LatLon( 35.857542, 14.477439, 297 ), TypeOfWaypoint.Airport, 0, 0, false ) ); // origin
      route.AddRoutePoint( new RoutePoint( "D130B", new LatLon( 35.833300, 14.507458, 5600 ), TypeOfWaypoint.Waypoint, 134, 0, true ) );
      route.AddRoutePoint( new RoutePoint( "D161D", new LatLon( 35.786228, 14.503772, 10300 ), TypeOfWaypoint.Waypoint, 183, 0, true ) );
      route.AddRoutePoint( new RoutePoint( "TOC", new LatLon( 35.698334, 15.172231, 25000 ), TypeOfWaypoint.Waypoint, 99, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "GODAK", new LatLon( 35.637778, 15.616389, 25000 ), TypeOfWaypoint.Waypoint, 99, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "RUDOG", new LatLon( 35.433333, 17.291944, 25000 ), TypeOfWaypoint.Waypoint, 98, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "INBIN", new LatLon( 35.268611, 18.500000, 25000 ), TypeOfWaypoint.Waypoint, 99, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "VANIX", new LatLon( 34.827500, 21.390833, 25000 ), TypeOfWaypoint.Waypoint, 99, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "ARLOS", new LatLon( 34.625278, 23.000000, 25000 ), TypeOfWaypoint.Waypoint, 98, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "OTREX", new LatLon( 35.154444, 24.938889, 25000 ), TypeOfWaypoint.Waypoint, 71, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "D223J", new LatLon( 35.227633, 25.035419, 25000 ), TypeOfWaypoint.Waypoint, 47, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "TOD", new LatLon( 35.250273, 25.065335, 25000 ), TypeOfWaypoint.User, 47, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "IRA", new LatLon( 35.340744, 25.185144, 20300 ), TypeOfWaypoint.VOR, 47, 0, false ) );
      route.AddRoutePoint( new RoutePoint( "D040Q", new LatLon( 35.546111, 25.425000, 10500 ), TypeOfWaypoint.Waypoint, 43, 0, true ) );
      route.AddRoutePoint( new RoutePoint( "GONSO", new LatLon( 35.449722, 25.390833, 7000 ), TypeOfWaypoint.Waypoint, 196, 0, true ) );
      route.AddRoutePoint( new RoutePoint( "LGIR", new LatLon( 35.339722, 25.180278, 115 ), TypeOfWaypoint.Airport, 237, 0, false ) );

      uC_Map1.SetRoute( route );
    }


  }
}
