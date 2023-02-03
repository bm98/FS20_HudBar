using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// An MSFS PLN ICAO Element
  /// </summary>
  [XmlRootAttribute( "ICAO", Namespace = "", IsNullable = false )]
  public class X_Icao
  {
    // Elements
    /// <summary>
    /// The  field
    /// </summary>
    [XmlElement( ElementName = "ICAOIdent" )]
    public string ICAO_Ident { get; set; } = "";

    /// <summary>
    /// The  field
    /// </summary>
    [XmlElement( ElementName = "ICAORegion", IsNullable = false )]
    public string Region { get; set; } = "";

    /// <summary>
    /// The  field
    /// </summary>
    [XmlElement( ElementName = "ICAOAirport", IsNullable = false )]
    public string AirportCode { get; set; } = "";

    // non XML

    /// <summary>
    /// Return the ICAO as String representation of this object
    /// </summary>
    /// <returns></returns>
    public override string ToString( ) => ICAO_Ident;

    /// <summary>
    /// True if there is an ICAO content available
    /// </summary>
    public bool IsValid => !string.IsNullOrWhiteSpace( ICAO_Ident );

  }
}
