using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.LNM
{
  /// <summary>
  /// Event fired when new LNM Plan data arrives
  /// 
  /// </summary>
  public class LNMplnDataEventArgs
  {
    /// <summary>
    /// LNM PLAN Data as string (XML string)
    /// </summary>
    public string LNMplnData { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public LNMplnDataEventArgs( string data )
    {
      LNMplnData = data;
    }

  }
}
