using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.GPX.GPXDEC
{

  /// <summary>
  /// An LNM GPX Exported Flight Plan
  /// </summary>
  [XmlRoot( ElementName = "gpx", Namespace = @"http://www.topografix.com/GPX/1/1", IsNullable = false )]
  public class GPX
  {
    // Attributes
    /// <summary>
    /// The gpx attributes
    /// </summary>
    [XmlAnyAttribute]
    public XmlAttribute[] Attributes { get; set; }


    // Elements
    /// <summary>
    /// The Flight Plan
    /// </summary>
    [XmlElement( ElementName = "rte" )]
    public X_Rte Route { get; set; } = null;

    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    [XmlIgnore]
    public bool IsValid {
      get {
        // weird - debugger fails to resolve this - and then breaks with Null exception
        // hence the try catch ??!!
        try {
          return (Route != null) && Route.IsValid;
        }
        catch {
          return false;
        }
      }
    }
  }
}
