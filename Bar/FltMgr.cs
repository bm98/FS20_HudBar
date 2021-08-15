using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FS20_FltLib;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Handles the FLT File to get some more information other than SimConnect
  /// As we can only interface to one FLT file this is build as static entity
  /// </summary>
  internal static class FltMgr
  {
    private static bool m_ready = false;
    private static readonly TimeSpan c_callInterval = new TimeSpan(0,0,30); // 
    private static DateTime m_lastCall;

    private static readonly string c_fltDir = "";
    private static readonly string c_fltName = "MostCurrent.FLT";
    private static FS20FltFile m_fltFile = null;
    private static int m_currentHash = 0;

    private static FileSystemWatcher m_fsWatch = null;

    private static object m_lock = new object();


    /// <summary>
    /// Event triggered when a new FlightPlan is available
    /// </summary>
    public static event EventHandler<EventArgs> NewFlightPlan;
    private static void OnNewFlightPlan( )
    {
      NewFlightPlan?.Invoke( null, new EventArgs( ) );
    }

    /// <summary>
    /// The most current FlightPlan (or null)
    /// </summary>
    public static FlightPlan FlightPlan {
      get {
        lock ( m_lock ) {
          return m_fltFile?.FlightPlan;
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
      m_lastCall = DateTime.Now - c_callInterval; // make sure we call the first time

      // we shall not fail
      try {
        var tmpDir = Directory.CreateDirectory( Path.Combine(Path.GetTempPath( ), "HudBar" ) );
        c_fltDir = tmpDir.FullName;
      }
      catch { }

    }

    // triggered by the FS Watcher
    private static void M_fsWatch_Changed( object sender, FileSystemEventArgs e )
    {
      // created AND changed
      if ( e.ChangeType == WatcherChangeTypes.Changed ) {
        lock ( m_lock ) {
          m_fltFile = new FS20FltFile( e.FullPath ); // recreate
        }
        // decide if something has changed for this update
        HasChanged = ( m_currentHash != m_fltFile.FlightPlan.Hash );
        m_currentHash = m_fltFile.FlightPlan.Hash;
        // call the owner
        if ( HasChanged )
          OnNewFlightPlan( );
      }
    }

    /// <summary>
    /// Enable the FLT processing
    /// </summary>
    public static void Enable( )
    {
      if ( Directory.Exists( c_fltDir ) ) {
        m_fsWatch = new FileSystemWatcher( c_fltDir, c_fltName ) { NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite };
        m_fsWatch.Changed += M_fsWatch_Changed;
        // reset state
        m_currentHash = 0;
        HasChanged = false;
        m_ready = true;
        // start watching
        m_fsWatch.EnableRaisingEvents = true;
      }
    }


    /// <summary>
    /// Disable the FLT processing
    /// </summary>
    public static void Disable( )
    {
      m_ready = false;

      if ( m_fsWatch != null ) {
        m_fsWatch.EnableRaisingEvents = false;
        m_fsWatch.Changed -= M_fsWatch_Changed;
        m_fsWatch.Dispose( );
        m_fsWatch = null;
        lock ( m_lock ) {
          m_fltFile = null;
        }
      }
      HasChanged = false;
    }

    /// <summary>
    /// Pings the Handler to may be update from the FLT file
    /// </summary>
    /// <returns>True if a FLT retrieval was triggered</returns>
    public static bool Ping( )
    {
      if ( !m_ready ) return false; // not enabled or otherwise broken
      if ( ( m_lastCall + c_callInterval ) > DateTime.Now ) return false; // not yet time

      SC.SimConnectClient.Instance.SaveFlight( Path.Combine( c_fltDir, c_fltName ) ); // trigger a FLT file
      m_lastCall = DateTime.Now;

      return true;
      // the filewatcher should report the file created
    }


  }
}
