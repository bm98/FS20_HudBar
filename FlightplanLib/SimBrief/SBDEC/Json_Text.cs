using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Text Data Record
  /// </summary>
  [DataContract]
  public class Json_Text
  {
    [DataMember( Name = "plan_html", IsRequired = false )]
    public string Plan { get; set; } = ""; // as HTML

  }
}
