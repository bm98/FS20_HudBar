using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using dNetBm98;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// LNM Header
  /// </summary>
  [XmlRoot( ElementName = "Header", IsNullable = false )]
  public class X_Header
  {
    /*
      <xs:element name="Header">
      <xs:complexType>
      <xs:sequence>
      <xs:element minOccurs="0" maxOccurs="1" ref="FlightplanType"/>
      <xs:element minOccurs="1" maxOccurs="1" ref="CruisingAlt"/>
      <xs:element minOccurs="1" maxOccurs="1" ref="CruisingAltF"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="Comment"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="CreationDate"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="FileVersion"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="ProgramName"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="ProgramVersion"/>
      <xs:element minOccurs="0" maxOccurs="1" ref="Documentation"/>
      </xs:sequence>
      </xs:complexType>
     */

    // Elements
    /// <summary>
    /// The FlightplanType element
    /// </summary>
    [XmlElement( ElementName = "FlightplanType" )]
    public string FlightplanTypeS { get; set; } = "";

    /// <summary>
    /// The CruisingAlt element
    /// </summary>
    [XmlElement( ElementName = "CruisingAlt" )]
    public float CruisingAlt_ft { get; set; } = float.NaN;
    /// <summary>
    /// The CruisingAltF element (is this Ft ??)
    ///  not always present (old SW version ? )
    /// </summary>
    [XmlElement( ElementName = "CruisingAltF" )]
    public float CruisingAltNum_ft { get; set; } = float.NaN;

    /// <summary>
    /// The CreationDate element
    /// </summary>
    [XmlElement( ElementName = "CreationDate" )]
    public DateTime CreationDate { get; set; } = DateTime.MinValue;

    /// <summary>
    /// The FileVersion element
    /// </summary>
    [XmlElement( ElementName = "FileVersion" )]
    public string FileVersion { get; set; } = "";
    /// <summary>
    /// The ProgramName element
    /// </summary>
    [XmlElement( ElementName = "ProgramName" )]
    public string ProgramName { get; set; } = "";
    /// <summary>
    /// The ProgramVersion element
    /// </summary>
    [XmlElement( ElementName = "ProgramVersion" )]
    public string ProgramVersion { get; set; } = "";
    /// <summary>
    /// The Documentation Link element
    /// </summary>
    [XmlElement( ElementName = "Documentation" )]
    public string DocumentationLink { get; set; } = "";

    // Non XML

    
    /// <summary>
    /// Returns the CruiseAlt as int rounded to 100 ft
    /// </summary>
    public int CruiseAlt_ft {
      get {
        if (!float.IsNaN( CruisingAltNum_ft ))
          return XMath.AsRoundInt( CruisingAltNum_ft, 100 );
        if (!float.IsNaN( CruisingAlt_ft ))
          return XMath.AsRoundInt( CruisingAlt_ft, 100 );
        return 0; // nothing found
      }
    }

    /// <summary>
    /// Type of the Flightplan
    /// </summary>
    public TypeOfFlightplan FlightplanType {
      get {
        switch (FlightplanTypeS) {
          case "IFR": return TypeOfFlightplan.IFR;
          case "VFR": return TypeOfFlightplan.VFR;
          default: return TypeOfFlightplan.VOR;
        }
      }
    }
  }
}
