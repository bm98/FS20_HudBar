using System;

namespace FShelf.FPlans
{
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
