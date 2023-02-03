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
    /// <summary>
    /// The fetch field
    /// </summary>
    [DataMember( Name = "fetch", IsRequired = true )]
    public Json_Fetch Fetch { get; set; } = new Json_Fetch( );

    /// <summary>
    /// The params field
    /// </summary>
    [DataMember( Name = "params", IsRequired = true )]
    public Json_Params Params { get; set; } = new Json_Params( );

    /// <summary>
    /// The general field
    /// </summary>
    [DataMember( Name = "general", IsRequired = true )]
    public Json_General General { get; set; } = new Json_General( );

    /// <summary>
    /// The origin field
    /// </summary>
    [DataMember( Name = "origin", IsRequired = true )]
    public Json_DptDst Departure { get; set; } = new Json_DptDst( );

    /// <summary>
    /// The destination field
    /// </summary>
    [DataMember( Name = "destination", IsRequired = true )]
    public Json_DptDst Destination { get; set; } = new Json_DptDst( );

    /// <summary>
    /// The navlog field
    /// </summary>
    [DataMember( Name = "navlog", IsRequired = true )]
    public Json_Navlog NavLog { get; set; } = new Json_Navlog( );

    /// <summary>
    /// The text field
    /// </summary>
    [DataMember( Name = "text", IsRequired = true )]
    public Json_Text Text { get; set; } = new Json_Text( );

    /// <summary>
    /// The files field
    /// </summary>
    [DataMember( Name = "files", IsRequired = false )]
    public Json_Files Plan_Files { get; set; } = new Json_Files( );

    /// <summary>
    /// The images field
    /// </summary>
    [DataMember( Name = "images", IsRequired = false )]
    public Json_Images Image_Files { get; set; } = new Json_Images( );

    // non JSON

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    public bool IsValid => Fetch.IsSuccess;

  }
}
