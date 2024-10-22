using System;

namespace FShelf.FPlans
{
  /// <summary>
  /// Supplied when a Flightplan was captured after Requesting a plan to be loaded
  ///  will also reply when not successfull
  /// </summary>
  internal class FlightPlanArrivedEventArgs : EventArgs
  {
    /// <summary>
    /// Will be true if successful
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Source of the new plan
    /// </summary>
    public FSimFlightPlans.SourceOfFlightPlan Source { get; set; }

    /// <summary>
    /// The message related to the capture event
    /// </summary>
    public string LoadMessage { get; set; }
    /// <summary>
    /// cTor:
    /// </summary>
    public FlightPlanArrivedEventArgs( bool success, FSimFlightPlans.SourceOfFlightPlan source, string loadMessage )
    {
      Success = success;
      Source = source;
      LoadMessage = loadMessage;
    }

  }

  /// <summary>
  /// Supplied when the next Wayppint has changed
  /// </summary>
  internal class WaypointNextChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Ident of the new next Waypoint
    ///  can be an empty string if there is no next anymore
    /// </summary>
    public string NextWaypoint_ID { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public WaypointNextChangedEventArgs( string nextWypID )
    {
      NextWaypoint_ID = nextWypID;
    }


  }

}
