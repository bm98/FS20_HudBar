using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Sim and NavData Element
  /// </summary>
  public class X_SimNavData
  {
    /*
        <xs:element name="SimData">
        <xs:complexType>
        <xs:simpleContent>
        <xs:extension base="xs:string">
        <xs:attribute name="Cycle" type="xs:integer"/>
        </xs:extension>
        </xs:simpleContent>
        </xs:complexType>
        </xs:element>

        <xs:element name="NavData">
        <xs:complexType>
        <xs:simpleContent>
        <xs:extension base="xs:string">
        <xs:attribute name="Cycle" type="xs:integer"/>
        </xs:extension>
        </xs:simpleContent>
        </xs:complexType>
        </xs:element>     
     */

    // Attributes
    /// <summary>
    /// The Name element
    /// </summary>
    [XmlAttribute( AttributeName = "Cycle" )]
    public int Cycle { get; set; } = 0;

    // Elements
    /// <summary>
    /// The Content element
    /// </summary>
    [XmlText()]
    public string Item { get; set; } = "";
  }
}
