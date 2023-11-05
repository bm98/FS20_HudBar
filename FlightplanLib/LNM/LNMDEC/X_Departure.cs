using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// Start Type in Departure
  /// </summary>
  public enum StartTypeE
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    Airport,
    Runway,
    Parking,
    Helipad
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// LNM Departure
  /// </summary>
  [XmlRoot( ElementName = "Departure", IsNullable = false )]
  public class X_Departure
  {
    /*
        <xs:element name="Departure">
        <xs:complexType>
        <xs:sequence>
        <xs:element minOccurs="0" maxOccurs="1" ref="Pos"/>
        <xs:element minOccurs="0" maxOccurs="1" name="Start" type="xs:string"/>
        <xs:element minOccurs="0" maxOccurs="1" name="Type" type="StartType"/>
        <xs:element minOccurs="0" maxOccurs="1" name="Heading" type="xs:decimal"/>
        </xs:sequence>
        </xs:complexType>
        </xs:element>     


          <xs:simpleType name="StartType">
          <xs:restriction base="xs:string">
          <xs:enumeration value="None"/>
          <xs:enumeration value="Airport"/>
          <xs:enumeration value="Runway"/>
          <xs:enumeration value="Parking"/>
          <xs:enumeration value="Helipad"/>
          </xs:restriction>
          </xs:simpleType>
     */

    // Elements
    /// <summary>
    /// The Pos element
    /// </summary>
    [XmlElement( ElementName = "Pos" )]
    public X_Coord Pos { get; set; } = new X_Coord( );

    /// <summary>
    /// The Start element
    /// e.g. "GATE A 2"
    /// </summary>
    [XmlElement( ElementName = "Start" )]
    public string Start { get; set; } = "";
    /// <summary>
    /// The StartType element as string
    /// e.g. "Parking"
    /// </summary>
    [XmlElement( ElementName = "StartType" )]
    public string StartTypeS { get; set; } = "";

    /// <summary>
    /// The Heading element
    /// </summary>
    [XmlElement( ElementName = "Heading" )]
    public float Heading { get; set; }

    // Non XML

    /// <summary>
    /// The StartType element
    /// e.g. "Parking"
    /// </summary>
    public StartTypeE StartType {
      get {
        switch (StartTypeS) {
          case "Airport": return StartTypeE.Airport;
          case "Runway": return StartTypeE.Runway;
          case "Parking": return StartTypeE.Parking;
          case "Helipad": return StartTypeE.Helipad;
          default: return StartTypeE.None;
        }
      }
    }
  }
}
