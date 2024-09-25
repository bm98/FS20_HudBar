using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;
using bm98_hbFolders;

using FlightplanLib.Flightplan;
using FlightplanLib.GPX;
using FlightplanLib.LNM;
using FlightplanLib.MSFSFlt;
using FlightplanLib.MSFSPln;
using FlightplanLib.RTE;
using FlightplanLib.SimBrief;

namespace Z_TEST_Flightplan
{
  public partial class Form1 : Form
  {
    private const string c_destLocation = @".\TESTRESULTS";
    private string _destPath = "";
    private string _loadFilename = "";
    private bool _dbMissing = false;

    #region Copied from FShelf.FPlans.FPWrapper

    /*
      Copied 1:1 from Wrapper
       changed public to private
     */


    // holds the currently loaded plan 
    private FlightPlan _fPlan = new FlightPlan( );
    // SB Helper
    private SbDocLoader _sbDocLoader = new SbDocLoader( );

    /// <summary>
    /// True if the SimBrief Plan is in use
    /// </summary>
    private bool IsSbPlan => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.SimBrief);

    /// <summary>
    /// True if the MSFS GPX Plan is in use
    /// </summary>
    private bool IsMsFsPLN => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Pln);

    /// <summary>
    /// True if the MSFS FLT Plan is in use
    /// </summary>
    private bool IsMsFsFLT => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Flt);

    /// <summary>
    /// True if the LNM GPX Export Plan is in use
    /// </summary>
    private bool IsLnmGPX => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.LNM_Gpx);
    /// <summary>
    /// True if the LNM RTE Export Plan is in use
    /// </summary>
    private bool IsLnmRTE => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.GEN_Rte);
    /// <summary>
    /// True if the LNMpln Export Plan is in use
    /// </summary>
    private bool IsLnmPLN => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.LNM_Pln);

    /// <summary>
    /// Get: the Plan
    /// </summary>
    private FlightPlan FlightPlan => _fPlan;


    /// <summary>
    /// Decode and Load the SimBrief Plan
    /// </summary>
    /// <param name="jsonString">The received JSON Plan as string</param>
    private void LoadSbPlan( string jsonString )
    {
      var sb = FlightplanLib.SimBrief.SBDEC.JsonDecoder.FromString( jsonString );
      if (sb.IsValid) {
        _fPlan = SimBrief.AsFlightPlan( sb );
      }
    }

    /// <summary>
    /// Decode and Load the MSFS GPX Plan
    /// </summary>
    /// <param name="plnXmlString">The received MSFS Plan as string</param>
    private void LoadMsFsPLN( string plnXmlString )
    {
      var pln = FlightplanLib.MSFSPln.PLNDEC.PlnDecoder.FromString( plnXmlString );
      if (pln.IsValid) {
        _fPlan = MSFSPln.AsFlightPlan( pln );
      }
    }

    /// <summary>
    /// Decode and Load the MSFS FLT Plan
    /// </summary>
    /// <param name="fltIniString">The received MSFS Plan as string</param>
    private void LoadMsFsFLT( string fltIniString )
    {
      var flt = FlightplanLib.MSFSFlt.FLTDEC.FltDecoder.FromString( fltIniString );
      if (flt.IsValid) {
        _fPlan = MSFSFlt.AsFlightPlan( flt );
      }
    }

    /// <summary>
    /// Decode and Load the LNM GPX Exported Plan
    /// </summary>
    /// <param name="plnXmlString">The received LNM GPX Exported Plan as string</param>
    private void LoadLnmGPX( string plnXmlString )
    {
      var pln = FlightplanLib.GPX.GPXDEC.GpxDecoder.FromString( plnXmlString );
      if (pln.IsValid) {
        _fPlan = GPXpln.AsFlightPlan( pln );
      }
    }

    /// <summary>
    /// Decode and Load the LNM Route String
    /// </summary>
    /// <param name="rteString">The received LNM Route String</param>
    private void LoadLnmRTE( string rteString )
    {
      var pln = FlightplanLib.RTE.RTEDEC.PlnDecoder.FromString( rteString );
      if (pln.IsValid) {
        _fPlan = RTEpln.AsFlightPlan( pln );
      }
    }

    /// <summary>
    /// Decode and Load the LNM PLN String
    /// </summary>
    /// <param name="rteString">The received LNM PLN String</param>
    private void LoadLnmPLN( string rteString )
    {
      var pln = FlightplanLib.LNM.LNMDEC.LnmPlnDecoder.FromString( rteString );
      if (pln.IsValid) {
        _fPlan = LNMpln.AsFlightPlan( pln );
      }
    }


    /// <summary>
    /// Load the available Docs and saves them into the destination
    /// (acts on the valid plan)
    /// </summary>
    /// <param name="destLocation">The destination Folder</param>
    /// <param name="asPDF">True to save as PDF else as Image</param>
    /// <returns>An empty string or an error string when not successfull</returns>
    private string GetAndSaveDocuments( string destLocation, bool asPDF )
    {
      var sb = new StringBuilder( );
      if (!Directory.Exists( destLocation )) {
        sb.AppendLine( "ERROR: Destination folder does not exist" ); // should not happen or the user removed the Bag Folder
      }
      else {
        if (IsSbPlan) {
          // Load Shelf Docs
          if (!_sbDocLoader.LoadDocuments( FlightPlan, destLocation )) {
            sb.AppendLine( "FP document is currently in use, could not change it" );
          }
        }
        else if (IsMsFsPLN) {
          ; // nothing
        }
        else if (IsMsFsFLT) {
          ; // nothing
        }
        else if (IsLnmGPX) {
          ; // nothing
        }
        else if (IsLnmRTE) {
          ; // nothing
        }
        else if (IsLnmPLN) {
          ; // nothing
        }
        // the plan as table "@.FlightTable.png"
        var fpTable = new FlightPlanTable( );
        if (!fpTable.SaveDocument( FlightPlan, destLocation )) {
          sb.AppendLine( "FP table is currently in use, could not change it" );
        }
      }

      return sb.ToString( );
    }

    #endregion

    public Form1( )
    {
      InitializeComponent( );

      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "FShelfAppSettings.json" );
      _destPath = Path.GetFullPath( c_destLocation );
      if (!Directory.Exists( _destPath )) {
        Directory.CreateDirectory( _destPath );
      }

    }

    private void Form1_Load( object sender, EventArgs e )
    {
      _dbMissing = !File.Exists( Folders.GenAptDBFile ); // facilities DB missing
      if (_dbMissing) {
        RTB.Text = "Facility DB missing - setup path";
      }
      else {
        RTB.Text = "Facility DB found";
      }
    }


    private bool FPLoader( )
    {
      string buffer = "";

      try {
        using (StreamReader sr = File.OpenText( _loadFilename )) {
          buffer = sr.ReadToEnd( );
        }
      }
      catch (Exception ex) {
        RTB.Text += $"ERROR: FPLoader.ReadFile \n{ex}\n";
      }

      string fpExt = Path.GetExtension( _loadFilename );
      try {
        switch (fpExt.ToLowerInvariant( )) {
          case ".pln":
            LoadMsFsPLN( buffer );
            return true;
          case ".flt":
            LoadMsFsFLT( buffer );
            return true;
          case ".gpx":
            LoadLnmGPX( buffer );
            return true;
          case ".lnmpln":
            LoadLnmPLN( buffer );
            return true;
          case ".json":
            LoadSbPlan( buffer );
            return true;
          case ".rte":
            LoadLnmRTE( buffer );
            return true;
          default:
            RTB.Text += $"ERROR: Unknown Plan Format for plan file:\n{_loadFilename}\n";
            break;
        }
      }
      catch (Exception ex) {
        RTB.Text += $"ERROR: FPLoader.LoadPlan \n{ex}\n";
      }
      return false;
    }

    private void btLoadFP_Click( object sender, EventArgs e )
    {
      if (OFD.ShowDialog( this ) == DialogResult.OK) {
        _loadFilename = OFD.FileName;
        RTB.Text = $"Loading file: {_loadFilename}\n";
        _fPlan = null;
        bool loadResult = FPLoader( );
        RTB.Text += $"Load result: {loadResult}\n";
        if (loadResult) {
          RTB.Text += $"Saving in: {c_destLocation}\n";
          string saveResult = GetAndSaveDocuments( _destPath, true );
          RTB.Text += $"Save result: {saveResult}\n";
          Thread.Sleep( 5000 );
          File.Move( Path.Combine( _destPath, "@.FlightTable.pdf" ), Path.Combine( _destPath, $"FT_{DateTime.Now.ToString( "s" ).Replace( ":", "_" )}.pdf" ) );

          RTB.Text += $"\n++++ A valid Flightplan is available\n";
          txLat.Text = _fPlan.Origin.LatLonAlt_ft.Lat.ToString( );
          txLon.Text = _fPlan.Origin.LatLonAlt_ft.Lon.ToString( );
        }
        else {
          RTB.Text += $"\n---- NO valid Flightplan is available\n";
        }

      }
    }

    private void btNextWYP_Click( object sender, EventArgs e )
    {
      _fPlan.TrackAircraft( new LatLon( double.Parse( txLat.Text ), double.Parse( txLon.Text ) ) );

      RTB.Text += $"RouteTracking: {_fPlan.PrevRoutePoint.Icao_Ident} -> {_fPlan.NextRoutePoint.Icao_Ident} along {_fPlan.DistTraveled_nm:0.00} nm\n";
    }


  }
}
