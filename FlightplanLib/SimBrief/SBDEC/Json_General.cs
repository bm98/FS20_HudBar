using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    /// The Airline ICAO name
    /// </summary>
    [DataMember( Name = "icao_airline", IsRequired = true )]
    public string Airline_ICAO { get; set; } = "";

    /// <summary>
    /// The Flightnumber
    /// </summary>
    [DataMember( Name = "flight_number", IsRequired = true )]
    public string FlightNumber { get; set; } = "";

    /// <summary>
    /// The stepclimb_string field
    /// </summary>
    [DataMember( Name = "stepclimb_string", IsRequired = true )]
    public string StepProfile { get; set; } = "";
    /// <summary>
    /// The route field
    /// Route without SpeedAlt, Apts/RW and Ozeanic Wyps 
    /// </summary>
    [DataMember( Name = "route", IsRequired = true )]
    public string RouteS { get; set; } = "";

    /// <summary>
    /// The route_navigraph field
    /// Route with Ozeanic Wyps (56N020W) without SpeedAlt, Apts/RW
    /// </summary>
    [DataMember( Name = "route_navigraph", IsRequired = true )]
    public string RouteNGS { get; set; } = "";

    // non JSON

    /// <summary>
    /// The initial_altitude field as number
    /// </summary>
    public float CruiseAlt_ft => (float)Formatter.GetValue( CruiseAlt_S );

  }
}
