using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Fetch Data Record
  /// </summary>
  [DataContract( Name = "fetch" )]
  public class Json_Fetch
  {
    /// <summary>
    /// The userid field
    /// </summary>
    [DataMember( Name = "userid", IsRequired = true )]
    public string UserID { get; set; } = "";

    /// <summary>
    /// The status field
    /// </summary>
    [DataMember( Name = "status", IsRequired = true )]
    public string Status { get; set; } = "Error";

  // non JSON

    /// <summary>
    /// True when the returned status was success
    /// </summary>
  public bool IsSuccess => Status.ToLowerInvariant( ) == "success";

  }
}
