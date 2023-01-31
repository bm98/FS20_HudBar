using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.MS;


namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// FLT Main Section
  /// </summary>
  public class Ini_Main
  {
    [IniFileKey( "Title" )]
    public string Title { get; internal set; } = "";

    [IniFileKey( "Description" )]
    public string Description { get; internal set; } = "";

  }
}
