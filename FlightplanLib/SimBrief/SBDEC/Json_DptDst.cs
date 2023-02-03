using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CoordLib;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Origin / Destination Data Record
  /// </summary>
  [DataContract]
  public class Json_DptDst
  {
    /// <summary>
    /// The icao_code field
    /// </summary>
    [DataMember( Name = "icao_code", IsRequired = true )]
    public string AptICAO { get; set; } = "";

    /// <summary>
    /// The iata_code field
    /// </summary>
    [DataMember( Name = "iata_code", IsRequired = false )]
    public string AptIATA { get; set; } = "";

    /// <summary>
    /// The name field
    /// </summary>
    [DataMember( Name = "name", IsRequired = false )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The plan_rwy field
    /// </summary>
    [DataMember( Name = "plan_rwy", IsRequired = false )]
    public string PlannedRunway { get; set; } = "";

    /// <summary>
    /// The pos_lat field
    /// </summary>
    [DataMember( Name = "pos_lat", IsRequired = true )]
    public string Latitude { get; set; } = ""; // dec degree e.g. 35.786228
    /// <summary>
    /// The pos_long field
    /// </summary>
    [DataMember( Name = "pos_long", IsRequired = true )]
    public string Longitude { get; set; } = ""; // dec degree e.g. 14.503772

    /// <summary>
    /// The elevation field
    /// </summary>
    [DataMember( Name = "elevation", IsRequired = false )]
    public string Elevation { get; set; } = ""; // a number ft

    // Non JSON

    // converted datatypes
    /// <summary>
    /// LatLonAlt (derived field)
    /// </summary>
    public LatLon LatLon => new LatLon( Lat, Lon, Elevation_ft );
    /// <summary>
    /// Latitude (derived field)
    /// </summary>
    public double Lat => Formatter.GetValue( Latitude );
    /// <summary>
    /// Longitude (derived field)
    /// </summary>
    public double Lon => Formatter.GetValue( Longitude );
    /// <summary>
    /// Elevation ft (derived field)
    /// </summary>
    public float Elevation_ft => (float)Formatter.GetValue( Elevation );

  }
}
