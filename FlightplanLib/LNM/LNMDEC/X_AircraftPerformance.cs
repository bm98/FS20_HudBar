using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Departure
  /// </summary>
  [XmlRoot( ElementName = "AircraftPerformance", IsNullable = false )]
  public class X_AircraftPerformance
  {
    /*
    <AircraftPerformance>
      <FilePath>FlightFX HJET HA420.lnmperf</FilePath>
      <Type>HA420</Type>
      <Name>HJET</Name>
    </AircraftPerformance>

     */

    // Elements
    /// <summary>
    /// The Aircraft Type
    /// e.g. "HA420"
    /// </summary>
    [XmlElement( ElementName = "Type" )]
    public string AircraftType_ICAO { get; set; } = "";
    /// <summary>
    /// The Aircraft Type Name
    /// e.g. "HJET"
    /// </summary>
    [XmlElement( ElementName = "Name" )]
    public string AircraftType_Name { get; set; } = "";

  }
}
