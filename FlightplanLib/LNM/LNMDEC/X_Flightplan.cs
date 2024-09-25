using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Flightplan
  /// </summary>
  [XmlRoot( ElementName = "Flightplan", IsNullable = false )]
  public class X_Flightplan
  {
    /*
        <xs:element name="Flightplan">
        <xs:complexType>
        <xs:sequence>
        <xs:element minOccurs="1" maxOccurs="1" ref="Header"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="SimData"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="NavData"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="AircraftPerformance"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Departure"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Procedures"/>
        <xs:element minOccurs="0" maxOccurs="unbounded" ref="Alternates"/>
        <xs:element minOccurs="1" maxOccurs="unbounded" ref="Waypoints"/>
        </xs:sequence>
        </xs:complexType>
        </xs:element>
     */

    // Elements
    /// <summary>
    /// Header of the plan
    /// </summary>
    [XmlElement( ElementName = "Header" )]
    public X_Header Header { get; set; } = null;
    /// <summary>
    /// Some Sim relevant data
    /// </summary>
    [XmlElement( ElementName = "SimData" )]
    public X_SimNavData SimData { get; set; } = null;
    /// <summary>
    /// Some Nav Database used relevant data
    /// </summary>
    [XmlElement( ElementName = "NavData" )]
    public X_SimNavData NavData { get; set; } = null;

    /// <summary>
    /// Some Aircraft related data
    /// </summary>
    [XmlElement( ElementName = "AircraftPerformance", IsNullable = true )]
    public X_AircraftPerformance AircraftPerformance = new X_AircraftPerformance();

    /// <summary>
    /// The Departure (OPTIONAL)
    /// </summary>
    [XmlElement( ElementName = "Departure", IsNullable = true )]
    public X_Departure Departure { get; set; } = null;
    /// <summary>
    /// The SID,STAR procedures
    /// </summary>
    [XmlElement( ElementName = "Procedures" )]
    public X_Procedures Procedures { get; set; } = new X_Procedures( );
    /// <summary>
    /// The Waypoint Catalog
    /// </summary>
    [XmlElement( ElementName = "Waypoints" )]
    public X_WaypointsCat WaypointCat { get; set; } = new X_WaypointsCat( );
  }


  /// <summary>
  /// Catalog of Waypoints
  /// </summary>
  [XmlRoot( ElementName = "Waypoints", IsNullable = false )]
  public class X_WaypointsCat
  {
    // Elements
    /// <summary>
    /// The List of Waypoints
    /// </summary>
    [XmlElement( ElementName = "Waypoint" )]
    public List<X_Waypoint> Waypoints { get; set; } = new List<X_Waypoint>( );
  }

}
