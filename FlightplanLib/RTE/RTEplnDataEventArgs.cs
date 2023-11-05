using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.RTE
{
  /// <summary>
  /// Event fired when new RTE Plan data arrives
  /// 
  /// </summary>
  public class RTEplnDataEventArgs
  {
    /// <summary>
    /// RTE PLAN Data as string (XML string)
    /// </summary>
    public string RTEplnData { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public RTEplnDataEventArgs( string data)
    {
      RTEplnData = data;
    }

  }
}
