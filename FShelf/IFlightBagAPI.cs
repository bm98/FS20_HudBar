using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FShelf
{
  /// <summary>
  /// Available API Methods of the FlightBag
  /// </summary>
  public interface IFlightBagAPI
  {
    /// <summary>
    /// True in standalone mode
    /// </summary>
    bool Standalone { get; }

    /// <summary>
    /// Fired when the user loaded a valid flightplan
    /// </summary>
    event EventHandler<EventArgs> FlightPlanLoadedByUser;

    /// <summary>
    /// Ref to the currently loaded Flightplan, can be empty one (check IsValid)
    /// NOTE: don't mess with it and update when the load event is fired... 
    /// TODO: provide a tamper safe interface...
    /// </summary>
    FSimFlightPlans.FlightPlan FlightPlanRef { get; }

    /// <summary>
    /// Departure Airport ICAO ID
    /// </summary>
    string DEP_Airport { get; set; }

    /// <summary>
    /// Arrival Airport ICAO ID
    /// </summary>
    string ARR_Airport { get; set; }

    /// <summary>
    /// The active SimBrief Pilot ID
    /// </summary>
    string SimBriefID { get; }

    /// <summary>
    /// Load the active FP from SimBrief - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    bool LoadFromSimBrief( );

    /// <summary>
    /// Open a Dialog to load a FLightPlan from File - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    bool LoadFlightPlanFile( );

    /// <summary>
    /// Loads the default PLN file - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    bool LoadDefaultPLN( );

    /// <summary>
    /// Load a Route String - reports via FlightPlanLoadedByUser Event
    /// </summary>
    /// <returns>True if loading</returns>
    bool LoadRouteString( string routeString );

  }
}
