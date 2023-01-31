using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSFlt
{
  /// <summary>
  /// Event fired when new MSFS FLT data arrives
  /// 
  /// </summary>
  public class MSFSFltDataEventArgs
  {
    /// <summary>
    /// MSFS FLT Data as string
    /// </summary>
    public string MSFSFltData { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public MSFSFltDataEventArgs( string data )
    {
      MSFSFltData = data;
    }

  }
}
