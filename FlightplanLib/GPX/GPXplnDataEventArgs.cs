using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.GPX
{
  /// <summary>
  /// Event fired when new GPX Plan data arrives
  /// 
  /// </summary>
  public class GPXplnDataEventArgs
  {
    /// <summary>
    /// GPX PLAN Data as string (XML string)
    /// </summary>
    public string GPXplnData { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public GPXplnDataEventArgs( string data)
    {
      GPXplnData = data;
    }

  }
}
