using FSimFacilityIF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Procedures Element
  /// </summary>
  [XmlRoot( ElementName = "Procedures", IsNullable = false )]
  public class X_Procedures
  {
    /*
        <xs:complexType>
        <xs:sequence>
        <xs:element minOccurs="0" maxOccurs="1" ref="SID"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="STAR"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Approach"/>
        </xs:sequence>
        </xs:complexType>
        </xs:element>     
     
     */

    // Elements
    /// <summary>
    /// The SID element (optional - NULL)
    /// </summary>
    [XmlElement( ElementName = "SID" )]
    public X_SID SID { get; set; } = null;

    /// <summary>
    /// The STAR element (optional - NULL)
    /// </summary>
    [XmlElement( ElementName = "STAR" )]
    public X_STAR STAR { get; set; } = null;

    /// <summary>
    /// The Approach element (optional - NULL)
    /// </summary>
    [XmlElement( ElementName = "Approach" )]
    public X_Approach Approach { get; set; } = null;

  }

  /// <summary>
  /// The SID Element
  /// </summary>
  public class X_SID
  {
    /*
        <xs:element name="SID">
        <xs:complexType>
        <xs:sequence>
        <xs:element ref="Name"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Runway"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Transition"/>
        </xs:sequence>
        </xs:complexType>
        </xs:element>     
     */

    // Elements
    /// <summary>
    /// The Name element
    /// </summary>
    [XmlElement( ElementName = "Name" )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The Runway element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Runway" )]
    public string Runway { get; set; } = "";

    /// <summary>
    /// The Transition element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Transition" )]
    public string Transition { get; set; } = "";

    /// <summary>
    /// The Type element (optional - empty)
    /// not in XSD... can be "CUSTOMDEPART"
    /// </summary>
    [XmlElement( ElementName = "Type" )]
    public string SidType { get; set; } = "";

    /// <summary>
    /// The CustomDistance element (optional - NaN)
    /// not in XSD... used with type "CUSTOMDEPART"
    /// </summary>
    [XmlElement( ElementName = "CustomDistance" )]
    public float CustomDistance { get; set; } = float.NaN;

    // non XML

    /// <summary>
    /// True if a Transition is given
    /// </summary>
    [XmlIgnore]
    public bool HasTransition => !string.IsNullOrEmpty( Transition );

    /// <summary>
    /// True if the SID is artificial and refers to a Custom Departure
    /// </summary>
    [XmlIgnore]
    public bool IsCustom => SidType == "CUSTOMDEPART";

  }

  /// <summary>
  /// The STAR Element
  /// </summary>
  public class X_STAR
  {
    /*
      <xs:element name="STAR">
      <xs:complexType>
      <xs:sequence>
      <xs:element ref="Name"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="Runway"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="Transition"/>
      </xs:sequence>
      </xs:complexType>
      </xs:element>
     */

    // Elements
    /// <summary>
    /// The Name element
    /// </summary>
    [XmlElement( ElementName = "Name" )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The Runway element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Runway" )]
    public string Runway { get; set; } = "";

    /// <summary>
    /// The Transition element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Transition" )]
    public string Transition { get; set; } = "";

    // non XML

    /// <summary>
    /// True if a Transition is given
    /// </summary>
    [XmlIgnore]
    public bool HasTransition => !string.IsNullOrEmpty( Transition );

  }

  /// <summary>
  /// The Approach Element
  /// </summary>
  public class X_Approach
  {
    /*
        <xs:element name="Approach">
        <xs:complexType>
        <xs:sequence>
        <xs:element minOccurs="0" maxOccurs="1" ref="Name"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="ARINC"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Runway"/>
        <xs:element minOccurs="0" maxOccurs="1" name="Type" type="xs:string"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Suffix"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="Transition"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="TransitionType"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="CustomDistance"/>
        <xs:element minOccurs="0" maxOccurs="1" ref="CustomAltitude"/>
        </xs:sequence>
        </xs:complexType>
        </xs:element>
     */

    // Elements
    /// <summary>
    /// The Name element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Name" )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The ARINC element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "ARINC" )]
    public string ARINC { get; set; } = "";

    /// <summary>
    /// The Runway element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Runway" )]
    public string Runway { get; set; } = "";

    /// <summary>
    /// The Type element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Type" )]
    public string ApproachTypeS { get; set; } = "";

    /// <summary>
    /// The Suffix element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Suffix" )]
    public string Suffix { get; set; } = "";

    /// <summary>
    /// The Transition element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "Transition" )]
    public string Transition { get; set; } = "";

    /// <summary>
    /// The TransitionType element (optional - empty)
    /// </summary>
    [XmlElement( ElementName = "TransitionType" )]
    public string TransitionType { get; set; } = "";

    /// <summary>
    /// The CustomDistance element (optional - NaN)
    /// </summary>
    [XmlElement( ElementName = "CustomDistance" )]
    public float CustomDistance { get; set; } = float.NaN;

    /// <summary>
    /// The CustomAltitude element (optional - NaN)
    /// </summary>
    [XmlElement( ElementName = "CustomAltitude" )]
    public float CustomAltitude { get; set; } = float.NaN;

    // Non XML
    /// <summary>
    /// True if a Suffix is given
    /// </summary>
    [XmlIgnore]
    public bool HasSuffix => !string.IsNullOrEmpty( Suffix );

    /// <summary>
    /// Approach Navigation Type available in LNM
    /// </summary>
    public NavTyp Proc {
      get {
        switch (ApproachTypeS) {
          case "ILS": return NavTyp.ILS;
          case "VOR": return NavTyp.VOR;
          case "VORDME": return NavTyp.VORDME;
          case "NDB": return NavTyp.NDB;
          case "NDBDME": return NavTyp.NDBDME;
          case "RNAV": return NavTyp.RNAV;
          case "LOC": return NavTyp.LOC;
          case "LOC_BC": return NavTyp.LOC_BC;
          case "LDA": return NavTyp.LDA;
          case "GPS": return NavTyp.GPS;
          case "SDF": return NavTyp.SDF;
          default:
            return NavTyp.None;
        }
      }
    }
    /// <summary>
    /// Returns a complete Approach Type with Suffix
    /// e.g. RNAV Z
    /// </summary>
    public string ProcRef => AsProcRef( Proc, Suffix );


  }


}
