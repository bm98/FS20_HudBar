using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Waypoint Tracker (static module)
  /// </summary>
  class WPTracker
  {
    private static bool m_started = false;   // true if the flight has started

    private static int m_simTime = -1;        // start of flight
    private static int m_timeElapsed = 0;    // sec since start of flight

    private static string m_prevWP = "";     // the previous WP name
    private static string m_nextWP = "";     // the next WP name
    private static int m_wpSimTime = -1;      // start of WP
    private static int m_wpTimeElapsed = 0;  // sec since start of WP

    /// <summary>
    /// The time elapsed from the last WP [sim seconds]
    /// </summary>
    public static int WPTimeEnroute_sec => m_wpTimeElapsed;
    /// <summary>
    /// The time elapsed since the flight started [sim seconds]
    /// </summary>
    public static int TimeEnroute_sec => m_timeElapsed;

    /// <summary>
    /// The used Previous WP
    /// </summary>
    public static string PrevWP => m_prevWP;
    /// <summary>
    /// The used Next WP
    /// </summary>
    public static string NextWP => m_nextWP;

    /// <summary>
    /// Init a new Flight - reset Vars
    /// </summary>
    public static void InitFlight( )
    {
      m_started = false;
      m_nextWP = "";
      m_prevWP = "";
      m_simTime = -1;
      m_timeElapsed = 0;
      m_wpSimTime = -1;
      m_wpTimeElapsed = 0;
    }

    /// <summary>
    /// True if the Waypoint has changed 
    /// (Use Read() to commit)
    /// </summary>
    public static bool HasChanged { get; private set; } = false;

    /// <summary>
    /// Reset the Manager to be read again
    /// </summary>
    public static void Reset( )
    {
      HasChanged = true;
    }

    /// <summary>
    /// Commit reading the new Waypoint (NEXT)
    /// </summary>
    /// <returns>The current NEXT Waypoint</returns>
    public static string Read( )
    {
      HasChanged = false;
      return m_nextWP;
    }


    /// <summary>
    /// Tracks the enroute time
    /// </summary>
    /// <param name="prev">Prev WP ID</param>
    /// <param name="next">Nect WP ID</param>
    /// <param name="simSeconds">SimTime</param>
    /// <param name="onGround">OnGround Flag</param>
    public static void Track( string prev, string next, int simSeconds, bool onGround )
    {
      if ( next != m_nextWP ) {
        m_nextWP = next;
        m_prevWP = prev;
        // WP has changed 
        HasChanged = true;
      }

      // check if the flight has started
      if ( onGround && !m_started ) return; // not yet

      // we have started.. keep timers
      m_started = true;
      if ( m_simTime < 0 ) m_simTime = simSeconds;
      if ( m_wpSimTime < 0 ) m_wpSimTime = simSeconds;

      if ( HasChanged ) {
        // WP has changed 
        m_wpSimTime = simSeconds; // new WP reference time
        m_wpTimeElapsed = 0;
      }
      else {
        // same WP next as before - cal time Enroute
        m_timeElapsed = simSeconds - m_simTime;
        m_wpTimeElapsed = simSeconds - m_wpSimTime;
      }
    }



  }
}
