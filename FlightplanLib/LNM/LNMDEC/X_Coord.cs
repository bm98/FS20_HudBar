using CoordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Coordinate
  /// </summary>
  [XmlRoot( ElementName = "Pos", IsNullable = false )]
  public class X_Coord
  {
    /*
        <xs:complexType>
        <xs:attribute name="Alt" type="xs:decimal"/>
        <xs:attribute name="Lat" use="required" type="xs:decimal"/>
        <xs:attribute name="Lon" use="required" type="xs:decimal"/>
        </xs:complexType>
        </xs:element>
     */

    // Attributes
    /// <summary>
    /// The Longitude
    /// </summary>
    [XmlAttribute( AttributeName = "Alt" )]
    public double Alt { get; set; } = double.NaN;

    /// <summary>
    /// The Latitude
    /// </summary>
    [XmlAttribute( AttributeName = "Lat" )]
    public double Lat { get; set; } = double.NaN;

    /// <summary>
    /// The Longitude
    /// </summary>
    [XmlAttribute( AttributeName = "Lon" )]
    public double Lon { get; set; } = double.NaN;


    // Elements
    // none

    // non XML

    /// <summary>
    /// Coordinate Lat,Lon, Elevation_ft
    /// </summary>
    public LatLon Coord => new LatLon(Lat,Lon,Alt);
  }
}
