using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief
{
  /// <summary>
  /// The Format of the data
  /// </summary>
  public enum SimBriefDataFormat
  {
    /// <summary>
    /// XML formatted data
    /// </summary>
    XML = 0,
    /// <summary>
    /// JSON formatted data
    /// </summary>
    JSON,
  }

  /// <summary>
  /// Event fired when new METAR data arrives
  /// 
  /// </summary>
  public class SimBriefDataEventArgs
  {
    /// <summary>
    /// True if successful
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// SimBrief Data as string
    /// </summary>
    public string SimBriefData { get; set; }
    /// <summary>
    /// Data Format of the contained data
    /// </summary>
    public SimBriefDataFormat DataFormat { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public SimBriefDataEventArgs(bool success, string data, SimBriefDataFormat dataFormat )
    {
      Success = success;
      SimBriefData = data;
      DataFormat = dataFormat;
    }

  }
}
