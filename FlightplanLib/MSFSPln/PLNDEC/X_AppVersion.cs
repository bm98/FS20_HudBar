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
  /// An MSFS PLN AppVersion Element
  /// </summary>
  [XmlRootAttribute( "AppVersion", Namespace = "", IsNullable = false )]
  public class X_AppVersion
  {
    // Elements
    [XmlElement( ElementName = "AppVersionMajor" )]
    public int AppVersionMajor { get; set; }
    // Elements
    [XmlElement( ElementName = "AppVersionBuild" )]
    public int AppVersionBuild { get; set; }

  }
}
