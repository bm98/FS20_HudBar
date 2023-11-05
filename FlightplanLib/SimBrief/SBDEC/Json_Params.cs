using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    /// The user_id field
    /// </summary>
    [DataMember( Name = "user_id", IsRequired = false )]
    public string UserID { get; set; } = "";
    /// <summary>
    /// The time_generated field
    /// </summary>
    [DataMember( Name = "time_generated", IsRequired = false )]
    public string TimeStampUX { get; set; } = "";
    /* 		"time_generated": "1698334195",      */

    /// <summary>
    /// The units field
    /// </summary>
    [DataMember( Name = "units", IsRequired = true )]
    public string MassUnit { get; set; } = "";


    /// <summary>
    /// Time Stamp generated
    /// </summary>
    [IgnoreDataMember]
    public DateTime Timestamp => new DateTime( 1970, 1, 1, 0, 0, 0 ).Add( TimeSpan.FromSeconds( long.Parse( TimeStampUX ) ) );

  }
}
