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
  /// General Data Record
  /// </summary>
  [DataContract( Name = "general" )]
  public class Json_General
  {
    [DataMember( Name = "initial_altitude", IsRequired = true )]
    public string CruiseAlt_S { get; set; } = "";

    [DataMember( Name = "stepclimb_string", IsRequired = true )]
    public string StepProfile { get; set; } = "";

    // non JSON

    public float CruiseAlt_ft => (float)Formatter.GetValue( CruiseAlt_S );

  }
}
