using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Params Data Record
  /// </summary>
  [DataContract]
  public class Json_Params
  {
    /// <summary>
    /// The ofp_layout field
    /// </summary>
    [DataMember( Name = "ofp_layout", IsRequired = true )]
    public string OFP_Layout { get; set; } = "";

    /// <summary>
    /// The units field
    /// </summary>
    [DataMember( Name = "units", IsRequired = true )]
    public string MassUnit { get; set; } = "";

  }
}
