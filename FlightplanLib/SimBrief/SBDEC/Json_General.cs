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
    /// <summary>
    /// The initial_altitude field
    /// </summary>
    [DataMember( Name = "initial_altitude", IsRequired = true )]
    public string CruiseAlt_S { get; set; } = "";

    /// <summary>
    /// The stepclimb_string field
    /// </summary>
    [DataMember( Name = "stepclimb_string", IsRequired = true )]
    public string StepProfile { get; set; } = "";

    // non JSON

    /// <summary>
    /// The initial_altitude field as number
    /// </summary>
    public float CruiseAlt_ft => (float)Formatter.GetValue( CruiseAlt_S );

  }
}
