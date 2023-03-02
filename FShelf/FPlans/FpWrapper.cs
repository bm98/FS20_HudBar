using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib;
using FlightplanLib.MSFSPln;
using FlightplanLib.MSFSFlt;
using FlightplanLib.SimBrief;

namespace FShelf.FPlans
{
  /// <summary>
  /// A Wrapper for the different plans
  /// manages the exclusion of the others if one type is used
  /// </summary>
  internal class FpWrapper
  {
    // holds the currently loaded plan 
    private FlightPlan _fPlan = new FlightPlan( );

    // note: those shall never get null!! (assign an empty plan if not in use)
    //    private OFP _sbPlan = new OFP( );
    //    private PLN _msfsPlan = new PLN( );

    // SB Helper
    private SbDocLoader _sbDocLoader = new SbDocLoader( );

    /// <summary>
    /// True if the SimBrief Plan is in use
    /// </summary>
    public bool IsSbPlan => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.SimBrief);

    /// <summary>
    /// True if the MSFS PLN Plan is in use
    /// </summary>
    public bool IsMsFsPLN => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Pln);

    /// <summary>
    /// True if the MSFS FLT Plan is in use
    /// </summary>
    public bool IsMsFsFLT => _fPlan.IsValid && (_fPlan.Source == SourceOfFlightPlan.MS_Flt);


    /// <summary>
    /// Get: the Plan
    /// </summary>
    public FlightPlan FlightPlan => _fPlan;

    /// <summary>
    /// Decode and Load the SimBrief Plan
    /// </summary>
    /// <param name="jsonString">The received JSON Plan as string</param>
    public void LoadSbPlan( string jsonString )
    {
      var sb = FlightplanLib.SimBrief.SBDEC.JsonDecoder.FromString( jsonString );
      if (sb.IsValid) {
        _fPlan = SimBrief.AsFlightPlan( sb );
      }
    }

    /// <summary>
    /// Decode and Load the MSFS PLN Plan
    /// </summary>
    /// <param name="plnXmlString">The received MSFS Plan as string</param>
    public void LoadMsFsPLN( string plnXmlString )
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
    public void LoadMsFsFLT( string fltIniString )
    {
      var flt = FlightplanLib.MSFSFlt.FLTDEC.FltDecoder.FromString( fltIniString );
      if (flt.IsValid) {
        _fPlan = MSFSFlt.AsFlightPlan( flt );
      }
    }

    /// <summary>
    /// Load the available Docs and saves them into the destination
    /// (acts on the valid plan)
    /// </summary>
    /// <param name="destLocation">The destination Folder</param>
    /// <returns>An empty string or an error string when not successfull</returns>
    public string GetAndSaveDocuments( string destLocation )
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
        // the plan as table 
        var fpTable = new FlightPlanTable( );
        if (!fpTable.SaveDocument( FlightPlan, destLocation )) {
          sb.AppendLine( "FP document is currently in use, could not change it" );
        }


      }

      return sb.ToString( );
    }


  }
}
