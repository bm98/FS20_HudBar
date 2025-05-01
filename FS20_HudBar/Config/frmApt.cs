using System;
using System.Windows.Forms;

using FS20_HudBar.Bar;

using FSimFlightPlans.Routes;
using FSimFlightPlans.RTE.RTEDEC;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// Form to enter Departure and Arrival Airport ICAOs
  /// and to Load Plans 
  /// 
  /// If the user loads a plan it will be used
  /// If the user clears the fields and closes, the plan is used if valid
  /// If the user enters the fields and closes, clear plan ??
  /// 
  /// </summary>
  public partial class frmApt : Form
  {
    // flag for a loaded plan
    private bool _planLoaded = false;

    /// <summary>
    /// Departure Apt ICAO ID
    /// </summary>
    public string DepAptICAO { get; set; } = "";
    /// <summary>
    /// Destination Apt ICAO ID
    /// </summary>
    public string ArrAptICAO { get; set; } = "";

    // load data from a valid source
    private void LoadFromPlan( )
    {
      // load default
      if (HudBar.FlightPlanRef.IsValid) {
        lblLoading.Text = $"using a plan\n ({HudBar.FlightPlanRef.Source})";
        // dest from FPLan
        DepAptICAO = HudBar.FlightPlanRef.Origin.Icao_Ident;
        ArrAptICAO = HudBar.FlightPlanRef.Destination.Icao_Ident;
        // get the last route back if there is any
        if (HudBar.FlightPlanRef.HasRoute) {
          txRoute.Text = HudBar.FlightPlanRef.RouteString;
        }
      }
      else {
        lblLoading.Text = "no plan loaded";
        // no Flightplan
        if (AirportMgr.IsDepAvailable) {
          // departure from Mgr (prev entry)
          DepAptICAO = AirportMgr.DepAirportICAO;
        }
        else {
          // no preset
          DepAptICAO = "";
        }

        if (AirportMgr.IsArrAvailable) {
          // destination from Mgr (prev entry)
          ArrAptICAO = AirportMgr.ArrAirportICAO;
        }
        else {
          // no preset
          ArrAptICAO = "";
        }
      }
      txDep.Text = DepAptICAO;
      txArr.Text = ArrAptICAO;
    }


    // test for valid route string
    private string _tmpRouteString = "";
    private string _tmpRouteHelp = "";

    private bool TestRouteString( string routeString )
    {
      _tmpRouteString = routeString;
      _tmpRouteHelp = "";

      Route route = RteDecoder.FromString( routeString );
      if (route == null) {
        // should not happen...
        _tmpRouteHelp = "Route cannot be decoded - please verify for correctness";
        return false;
      }


      // missing DEP ??
      if (!route.HasDeparture) {
        var tmpRte = RteDecoder.FromString( txDep.Text + " " + txArr.Text );
        if (!tmpRte.IsValid) {
          _tmpRouteHelp += "Missing a known DEP and/or ARR airport, ";
        }
        else {
          // ok now
          route = tmpRte;
          _tmpRouteString = route.RouteString;
        }
      }

      if (!route.IsValid) {
        _tmpRouteHelp += "Route decode failed - please verify";
      }

      return string.IsNullOrEmpty( _tmpRouteHelp ); // ok when no help is provided
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public frmApt( )
    {
      InitializeComponent( );

      // must be empty to start with
      txDep.Text = "";
      txArr.Text = "";
      txRoute.Text = "";

      lblLoading.Text = "ready";

      HudBar.FlightBagRef.FlightPlanLoadedByUser += FlightBagRef_FlightPlanLoadedByUser;
    }

    // form load event
    private void frmApt_Load( object sender, EventArgs e )
    {
      LoadFromPlan( );

      lblSBPilotID.Text = HudBar.FlightBagRef.SimBriefID;
    }

    private void frmApt_Activated( object sender, EventArgs e )
    {
      lblDataSource.Text = bm98_hbFolders.Folders.FS2024Used ? "MSFS 2024" : "MSFS 2020";
    }

    private void frmApt_FormClosing( object sender, FormClosingEventArgs e )
    {
      HudBar.FlightBagRef.FlightPlanLoadedByUser -= FlightBagRef_FlightPlanLoadedByUser;
      this.Hide( );
    }

    // Close
    private void btClose_Click( object sender, EventArgs e )
    {
      // clean entry
      var s = txDep.Text.TrimStart( );
      if (s.Length > 4) s = s.Substring( 0, 4 ); // limit for sanity
      DepAptICAO = s.TrimEnd( ).ToUpperInvariant( );

      s = txArr.Text.TrimStart( );
      if (s.Length > 4) s = s.Substring( 0, 4 ); // limit for sanity
      ArrAptICAO = s.TrimEnd( ).ToUpperInvariant( );

      // Update DEP
      if (string.IsNullOrWhiteSpace( DepAptICAO )) {
        // empty entry to clear
        if (HudBar.FlightPlanRef.IsValid) {
          // update with FP origin
          AirportMgr.UpdateDep( HudBar.FlightPlanRef.Origin.Icao_Ident );
        }
        else {
          // clear with N.A. airport
          AirportMgr.UpdateDep( AirportMgr.AirportNA_Icao );
        }
      }
      else {
        // user or loaded entry - will be checked in the Mgr
        AirportMgr.UpdateDep( DepAptICAO );
      }

      // Update ARR
      if (string.IsNullOrWhiteSpace( ArrAptICAO )) {
        // empty entry to clear
        if (HudBar.FlightPlanRef.IsValid) {
          // update with FP destination
          AirportMgr.UpdateArr( HudBar.FlightPlanRef.Destination.Icao_Ident );
        }
        else {
          // clear with N.A. airport
          AirportMgr.UpdateArr( AirportMgr.AirportNA_Icao );
        }
      }
      else {
        // user or loaded entry - will be checked in the Mgr
        AirportMgr.UpdateArr( ArrAptICAO );
      }

      this.Hide( );
    }



    // Clear fields
    private void btClear_Click( object sender, EventArgs e )
    {
      txDep.Text = "";
      txArr.Text = "";
      txRoute.Text = "";
    }

    // load the SB plan
    private void btLoadSBPlan_Click( object sender, EventArgs e )
    {
      _planLoaded = false;
      lblLoading.Text = "requested";
      lblLoading.Text = HudBar.FlightBagRef.LoadFromSimBrief( ) ? "loading" : "load failed";
    }

    // load the MSFS default PLN
    private void btLoadDefaultPLN_Click( object sender, EventArgs e )
    {
      _planLoaded = false;
      lblLoading.Text = "requested";
      lblLoading.Text = HudBar.FlightBagRef.LoadDefaultPLN( ) ? "loading" : "load failed ";
    }

    // load a plan from file
    private void btLoadPlanFile_Click( object sender, EventArgs e )
    {
      _planLoaded = false;
      lblLoading.Text = "requested";
      lblLoading.Text = HudBar.FlightBagRef.LoadFlightPlanFile( ) ? "loading" : "load failed";
    }

    // load from route input
    private void btRouteString_Click( object sender, EventArgs e )
    {
      lblRouteStatus.Text = "";
      lblLoading.Text = "testing";
      if (TestRouteString( txRoute.Text )) {
        txRoute.Text = _tmpRouteString; // update (may have added the airports)
        lblLoading.Text = HudBar.FlightBagRef.LoadRouteString( txRoute.Text ) ? "loading" : "load failed";
      }
      else {
        lblLoading.Text = "test failed";
        lblRouteStatus.Text = "ERR: " + _tmpRouteHelp;
      }
    }

    // Load event
    private void FlightBagRef_FlightPlanLoadedByUser( object sender, EventArgs e )
    {
      var dd = new dNetBm98.Win.WinFormInvoker( this );
      dd.HandleEvent( ( ) => {
        lblLoading.Text = "loaded";
        LoadFromPlan( );
        _planLoaded = HudBar.FlightPlanRef.IsValid;
      } );
    }

  }
}
