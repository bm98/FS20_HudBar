using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using FSimFacilityIF;

namespace FlightplanLib.LNM.LNMDEC
{

  /// <summary>
  /// LNM SimpleWaypoint Type
  /// </summary>
  public enum WaypointTypeE
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    UNKNOWN = 0,
    AIRPORT,
    WAYPOINT,
    VOR,
    NDB,
    USER
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// LNM Waypoint Element
  /// </summary>
  [XmlRoot( ElementName = "Waypoint", IsNullable = false )]
  public class X_Waypoint
  {
    /*
        <xs:element name="Waypoint">
        <xs:complexType>
        <xs:sequence>
        <xs:element minOccurs="0" maxOccurs="1" ref="Name"/>
        <xs:element ref="Ident"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Region"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Airway"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Track"/>
        <xs:element ref="Type"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Comment"/>
        <xs:element ref="Pos"/>  // SimpleWaypointType
        </xs:sequence>
        </xs:complexType>
        </xs:element>

        <xs:simpleType name="SimpleWaypointType">
        <xs:restriction base="xs:string">
        <xs:enumeration value="AIRPORT"/>
        <xs:enumeration value="UNKNOWN"/>
        <xs:enumeration value="WAYPOINT"/>
        <xs:enumeration value="VOR"/>
        <xs:enumeration value="NDB"/>
        <xs:enumeration value="USER"/>
        </xs:restriction>
        </xs:simpleType>
    */

    // Elements
    /// <summary>
    /// The Name element
    /// </summary>
    [XmlElement( ElementName = "Name" )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The Ident element
    /// </summary>
    [XmlElement( ElementName = "Ident" )]
    public string Ident { get; set; } = "";

    /// <summary>
    /// The Region element
    /// </summary>
    [XmlElement( ElementName = "Region" )]
    public string Region { get; set; } = "";

    /// <summary>
    /// The Airway element
    /// </summary>
    [XmlElement( ElementName = "Airway" )]
    public string Airway { get; set; } = "";

    /// <summary>
    /// The Track element
    /// </summary>
    [XmlElement( ElementName = "Track" )]
    public string Track { get; set; } = "";

    /// <summary>
    /// The Type element as string
    /// </summary>
    [XmlElement( ElementName = "Type" )]
    public string WaypointTypeS { get; set; } = "";

    /// <summary>
    /// The Comment element
    /// </summary>
    [XmlElement( ElementName = "Comment" )]
    public string Comment { get; set; } = "";

    /// <summary>
    /// The Pos element
    /// </summary>
    [XmlElement( ElementName = "Pos" )]
    public X_Coord Pos { get; set; } = new X_Coord( );

    // Non XML

    /// <summary>
    /// True when an Airway is given
    /// </summary>
    [XmlIgnore]
    public bool HasAirway=> !string.IsNullOrEmpty( Airway );

    /// <summary>
    /// The WaypointType element
    /// </summary>
    [XmlIgnore]
    public WaypointTyp WaypointType {
      get {
        switch (WaypointTypeS) {
          case "AIRPORT": return WaypointTyp.APT;
          case "NDB": return WaypointTyp.NDB;
          case "VOR": return WaypointTyp.VOR;
          case "WAYPOINT": return WaypointTyp.WYP;
          case "USER": return WaypointTyp.USR;
          default: return WaypointTyp.OTH;
        }
      }
    }

  }
}
