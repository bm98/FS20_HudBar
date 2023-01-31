using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Navlog Data Record
  /// </summary>
  [DataContract]
  public class Json_Navlog
  {
    [DataMember( Name = "fix", IsRequired = true )]
    public List<Json_Fix> Waypoints { get; set; } = new List<Json_Fix>( );

  }


}
