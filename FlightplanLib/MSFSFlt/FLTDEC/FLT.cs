﻿using FlightplanLib.MSFSPln.PLNDEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using FlightplanLib.MS;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// An MSFS FLT Flight Plan
  /// </summary>
  public class FLT
  {
    // Main Keys

    // ... NONE

    /// <summary>
    /// Sect [Main] 
    /// </summary>
    [IniFileSection( "Main" )]
    public Ini_Main Main { get; internal set; } = new Ini_Main( );

    // Sect [Options] - not used

    // Secct [Sim.0] - not used

    /// <summary>
    /// Sect [Departure] 
    /// </summary>
    [IniFileSection( "Departure" )]
    public Ini_DepArr Departure { get; internal set; } = new Ini_DepArr( );

    /// <summary>
    /// Sect [Arrival]
    /// </summary>
    [IniFileSection( "Arrival" )]
    public Ini_DepArr Arrival { get; internal set; } = new Ini_DepArr( );

    /// <summary>
    /// Sect [ATC_Aircraft.0] active Flight gets changes while in flight and saved
    /// </summary>
    [IniFileSection( "ATC_Aircraft.0" )]
    public Ini_ATC_Aircraft ATC_Aircraft { get; internal set; } = null;

    /// <summary>
    /// Sect [ATC_ActiveFlightPlan.0] seems to be an initial or current FP, never contains ATC intermediate waypoints
    /// </summary>
    [IniFileSection( "ATC_ActiveFlightPlan.0" )]
    public Ini_ATC_ActiveFlightPlan ATC_Flightplan { get; internal set; } = null;


    // Sect [ATC_RequestedFlightPlan.0]   if [ATC_Aircraft.0] RequestedFlightPlan=True
    // Sect [OriginalFlightPlan]   (Mission Files)

    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    public bool IsValid => ATC_Aircraft != null;

  }
}
