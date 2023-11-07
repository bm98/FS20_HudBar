using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dNetBm98.IniLib;


namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// FLT Main Section
  /// </summary>
  public class Ini_Main
  {
    /// <summary>
    /// Title field
    /// </summary>
    [IniFileKey( "Title" )]
    public string Title { get; internal set; } = "";
    /// <summary>
    /// Description field
    /// </summary>
    [IniFileKey( "Description" )]
    public string Description { get; internal set; } = "";

  }
}
