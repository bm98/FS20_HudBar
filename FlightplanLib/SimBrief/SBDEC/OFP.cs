using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// The SimBrief OFP (Decoding only)
  /// 
  /// only relevant data is decoded
  /// </summary>
  [DataContract]
  public class OFP
  {
    [DataMember( Name = "fetch", IsRequired = true )]
    public Json_Fetch Fetch { get; set; } = new Json_Fetch( );

    [DataMember( Name = "params", IsRequired = true )]
    public Json_Params Params { get; set; } = new Json_Params( );

    [DataMember( Name = "general", IsRequired = true )]
    public Json_General General { get; set; } = new Json_General( );

    [DataMember( Name = "origin", IsRequired = true )]
    public Json_DptDst Departure { get; set; } = new Json_DptDst( );

    [DataMember( Name = "destination", IsRequired = true )]
    public Json_DptDst Destination { get; set; } = new Json_DptDst( );

    [DataMember( Name = "navlog", IsRequired = true )]
    public Json_Navlog NavLog { get; set; } = new Json_Navlog( );

    [DataMember( Name = "text", IsRequired = true )]
    public Json_Text Text { get; set; } = new Json_Text( );

    [DataMember( Name = "files", IsRequired = false )]
    public Json_Files Plan_Files { get; set; } = new Json_Files( );

    [DataMember( Name = "images", IsRequired = false )]
    public Json_Images Image_Files { get; set; } = new Json_Images( );

    // non JSON

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    public bool IsValid => Fetch.IsSuccess;

  }
}
