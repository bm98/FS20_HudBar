using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FlightplanLib.MSFSPln.PLNDEC
{

  /// <summary>
  /// An MSFS PLN Flight Plan
  /// </summary>
  [XmlRootAttribute( "SimBase.Document", Namespace = "", IsNullable = false )]
  public class PLN
  {
    // Attributes
    [XmlAttribute( AttributeName = "Type" )]
    public string DType { get; set; } = ""; // AceXML

    [XmlAttribute( AttributeName = "version" )]
    public string DVersion { get; set; } // "1,0" have not seen others

    // Elements
    [XmlElement( ElementName = "Descr" )]
    public string Descr { get; set; } = ""; // "AceXML Document"


    /// <summary>
    /// The Flight Plan
    /// </summary>
    [XmlElement( ElementName = "FlightPlan.FlightPlan", Type = typeof( X_FlightPlan ) )]
    public X_FlightPlan FlightPlan { get; set; } = null;

    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    public bool IsValid => FlightPlan != null;

  }
}
