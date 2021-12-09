using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF.Flightplan;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Handles the FLT File to get some more information other than SimConnect
  /// As we can only interface to one FLT file this is build as static entity
  /// </summary>
  internal static class FltMgr
  {
//    private static FS20FltFile m_fltFile = null;
    private static int m_currentHash = 0;

    private static object m_lock = new object();

    private static FlightPlan m_FP; // The managed one
    private static FlightPlan m_dummyFP = new FlightPlan(); // an empty one to return when needed

    /// <summary>
    /// Event triggered when a new FlightPlan is available
    /// </summary>
    public static event EventHandler<EventArgs> NewFlightPlan;
    private static void OnNewFlightPlan( )
    {
      NewFlightPlan?.Invoke( null, new EventArgs( ) );
    }


    /// <summary>
    /// The most current FlightPlan (can be an empty one)
    /// </summary>
    public static FlightPlan FlightPlan {
      get {
        lock ( m_lock ) {
          if ( m_FP != null )
            return m_FP;
          else
            return m_dummyFP; // the empty one
        }
      }
    }

    /// <summary>
    /// True if the FlightPlan has changed 
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
    /// Commit reading the new FlightPlan
    /// </summary>
    /// <returns>The current FlightPlan</returns>
    public static FlightPlan Read( )
    {
      HasChanged = false;
      return FlightPlan;
    }


    /// <summary>
    /// cTor:
    /// </summary>
    static FltMgr( )
    {
      // receive FLT updates
      SC.SimConnectClient.Instance.FltSave += Instance_FltSave;
    }

    // Handle new FLT files comming in
    private static void Instance_FltSave( object sender, SC.State.FltSaveEventArgs e )
    {
      lock ( m_lock ) {
        m_FP = SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Copy(); // we have a copy
        HasChanged = ( m_currentHash != m_FP.Hash );
        m_currentHash = m_FP.Hash;
      }
      // call the owner
      if ( HasChanged )
        OnNewFlightPlan( ); // inform the Bar
    }


    /// <summary>
    /// Enable the FLT processing
    /// </summary>
    public static void Enable( )
    {
      SC.SimConnectClient.Instance.FltSaveModule.Enabled = true;
      // reset state
      m_currentHash = FlightPlan.Hash;
      HasChanged = true;
    }


    /// <summary>
    /// Disable the FLT processing
    /// </summary>
    public static void Disable( )
    {
      SC.SimConnectClient.Instance.FltSaveModule.Enabled = false;
      HasChanged = false;
      m_currentHash = 0;
      lock ( m_lock ) {
        m_FP = null; // Note: The FlightPlan property returns an empty one now
      }
    }


  }
}
