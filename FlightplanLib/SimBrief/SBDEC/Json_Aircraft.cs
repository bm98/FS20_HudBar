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
  public class Json_Aircraft
  {
    /// <summary>
    /// Aircraft Type ICAO Code
    /// </summary>
    [DataMember( Name = "icaocode", IsRequired = true )]
    public string TypeCode_ICAO { get; set; } = "";

    /// <summary>
    /// Aircraft Type Name
    /// </summary>
    [DataMember( Name = "name", IsRequired = true )]
    public string TypeName { get; set; } = "";

    /// <summary>
    /// Aircraft registration ID
    /// </summary>
    [DataMember( Name = "reg", IsRequired = true )]
    public string RegID { get; set; } = "";


    // non JSON

    /// <summary>
    /// Aircraft registration ID without dashes
    /// </summary>
    public string RegID_plain => RegID.Replace( "-", "" );

  }
}
