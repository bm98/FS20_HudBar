using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF.Flightplan;
using FSimClientIF;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Handles the FLT File to get some more information other than SimConnect
  /// As we can only interface to one FLT file this is build as static entity
  /// 
  /// DON'T Use the SimConnectClient FlightPlanModule outside, use the Manager...
  /// 
  /// </summary>
  internal static class AtcFltPlanMgr
  {
    private static int m_currentHash = 0;

    private static object m_lock = new object( );

    private static FSimClientIF.Flightplan.FlightPlan m_FP; // The managed one
    private static FSimClientIF.Flightplan.FlightPlan m_dummyFP = new FSimClientIF.Flightplan.FlightPlan( ); // an empty one to return when needed

    /// <summary>
    /// Event triggered when a new ATC FlightPlan is available
    /// </summary>
    public static event EventHandler<EventArgs> NewAtcFlightPlan;
    private static void OnNewAtcFlightPlan( )
    {
      NewAtcFlightPlan?.Invoke( null, new EventArgs( ) );
    }

    /// <summary>
    /// Get;Set: True if the FPlan Module is enabled and returns events
    /// </summary>
    public static bool Enabled {
      get => SC.SimConnectClient.Instance.FlightPlanModule.Enabled;
      set {
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = value;
        if (value) {
          // enable - reset state
          m_currentHash = AtcFlightPlan.Hash;
          HasChanged = true;
        }
        else {
          // disable
          HasChanged = false;
          m_currentHash = 0;
          lock (m_lock) {
            m_FP = null; // Note: The FlightPlan property returns an empty one now
          }
        }

      }
    }

    /// <summary>
    /// Get; Set: The current Flightplan Mode
    /// </summary>
    public static FSimClientIF.FlightPlanMode FlightPlanMode {
      get => SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode;
      set => SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode = value;
    }

    /// <summary>
    /// The most current ATC FlightPlan (can be an empty one)
    /// </summary>
    public static FSimClientIF.Flightplan.FlightPlan AtcFlightPlan {
      get {
        lock (m_lock) {
          if (m_FP != null)
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
      return AtcFlightPlan;
    }


    /// <summary>
    /// cTor:
    /// </summary>
    static AtcFltPlanMgr( )
    {
      // receive FLT updates
      //      SC.SimConnectClient.Instance.FltSave += Instance_FltSave;
      SC.SimConnectClient.Instance.FlightPlanModule.NewDataAvailable += FlightPlanModule_NewDataAvailable;
    }

    // Reports on valid and changed FPs only (module must be enabled, and mode is capture ATC or Backup)
    private static void FlightPlanModule_NewDataAvailable( object sender, EventArgs e )
    {
      OnNewAtcFlightPlan( ); // inform the Bar
    }


    // Handle new FLT files comming in, this will essentially capture all files reported from SimConnect
    private static void Instance_FltSave( object sender, FltSaveEventArgs e )
    {
      // sanity
      if (!e.FileValid) return; // invalid file
      if (!e.FlightPlanAvailable) return; // no (new or old) FP was stored

      lock (m_lock) {
        m_FP = SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan.Clone( ); // we have a copy
        HasChanged = (m_currentHash != m_FP.Hash);
        m_currentHash = m_FP.Hash;
      }
      // call the owner
      if (HasChanged)
        OnNewAtcFlightPlan( ); // inform the Bar
    }

  }
}
