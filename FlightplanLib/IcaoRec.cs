using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib
{
  /// <summary>
  /// An ICAO record
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// </summary>
  public class IcaoRec
  {
    /// <summary>
    /// True if this is a valid item
    /// </summary>
    public bool IsValid => !string.IsNullOrEmpty( ICAO );

    /// <summary>
    /// The ICAO ID
    /// </summary>
    public string ICAO { get; internal set; } = "";
    /// <summary>
    /// Optional:
    /// An ICAO Region Code
    /// </summary>
    public string Region { get; internal set; } = "";
    /// <summary>
    /// Optional:
    /// An AirportRef (ICAO) if applicable
    /// </summary>
    public string AirportRef { get; internal set; } = "";

    /// <summary>
    /// ICAO value to be reported as String property of the object
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString( ) => ICAO;

  }
}
