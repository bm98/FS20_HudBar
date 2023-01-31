using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSPln
{
  /// <summary>
  /// Event fired when new MSFS PLN data arrives
  /// 
  /// </summary>
  public class MSFSPlnDataEventArgs
  {
    /// <summary>
    /// MSFS PLN Data as string
    /// </summary>
    public string MSFSPlnData { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public MSFSPlnDataEventArgs( string data)
    {
      MSFSPlnData = data;
    }

  }
}
