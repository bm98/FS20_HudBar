using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// File Name/Link Pair Data Record
  /// </summary>
  [DataContract]
  public class Json_NameLinkPair
  {
    /// <summary>
    /// The name field
    /// </summary>
    [DataMember( Name = "name", IsRequired = false )]
    public string Name { get; set; } = "";

    /// <summary>
    /// The link field
    /// </summary>
    [DataMember( Name = "link", IsRequired = false )]
    public string Link { get; set; } = ""; // a document file name for getting file downloads

  }
}
