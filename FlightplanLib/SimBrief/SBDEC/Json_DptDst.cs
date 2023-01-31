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
    [DataMember( Name = "icao_code", IsRequired = true )]
    public string AptICAO { get; set; } = "";

    [DataMember( Name = "iata_code", IsRequired = false )]
    public string AptIATA { get; set; } = "";

    [DataMember( Name = "name", IsRequired = false )]
    public string Name { get; set; } = "";

    [DataMember( Name = "plan_rwy", IsRequired = false )]
    public string PlannedRunway { get; set; } = "";

    [DataMember( Name = "pos_lat", IsRequired = true )]
    public string Latitude { get; set; } = ""; // dec degree e.g. 35.786228
    [DataMember( Name = "pos_long", IsRequired = true )]
    public string Longitude { get; set; } = ""; // dec degree e.g. 14.503772

    [DataMember( Name = "elevation", IsRequired = false )]
    public string Elevation { get; set; } = ""; // a number ft

    // Non JSON

    // converted datatypes
    public LatLon LatLon => new LatLon( Lat, Lon, Elevation_ft );
    public double Lat => Formatter.GetValue( Latitude );
    public double Lon => Formatter.GetValue( Longitude );
    public float Elevation_ft => (float)Formatter.GetValue( Elevation );

  }
}
