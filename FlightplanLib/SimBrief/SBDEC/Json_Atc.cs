using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using static FSimFacilityIF.Extensions;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// ATC Data Record
  /// </summary>
  [DataContract]
  public class Json_Atc
  {
    /// <summary>
    /// The route field
    /// Route with SpeedAlt but without Apts/RW and Ozeanic Wyps
    /// </summary>
    [DataMember( Name = "route", IsRequired = true )]
    public string RouteS { get; set; } = "";

  }
}
