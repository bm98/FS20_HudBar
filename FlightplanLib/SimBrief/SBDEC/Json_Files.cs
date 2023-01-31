using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Files Data Record
  /// </summary>
  [DataContract]
  public class Json_Files
  {
    [DataMember( Name = "directory", IsRequired = true )]
    public string Directory { get; set; } = ""; // a base URL for getting file downloads

    [DataMember( Name = "pdf", IsRequired = false )]
    public Json_NameLinkPair Pdf_File { get; set; } = new Json_NameLinkPair( ); // single entry

    [DataMember( Name = "file", IsRequired = false )]
    public List<Json_NameLinkPair> Files { get; set; } = new List<Json_NameLinkPair>( ); // list of files


  }


}
