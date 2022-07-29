using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace bm98_Checklist
{
  /// <summary>
  /// Single instance of an interval timer to do repeats for clicks
  ///  This single instance works as we have only one mouse holding one button down at anytime
  ///  So we don't need to create a timer for any button placed in an application
  /// </summary>
  internal sealed class Timer
  {
    // Singleton var
    private static readonly Lazy<Timer> lazy = new Lazy<Timer>(() => new Timer());

    /// <summary>
    /// The singleton instance of this class
    /// </summary>
    public static Timer Instance { get { return lazy.Value; } }


    // *** CLASS IMPLEMENTATION ***
    private DispatcherTimer m_timer = new DispatcherTimer();
    private EventHandler<EventArgs> m_handler;
    private object m_sender;

    /// <summary>
    /// the start interval for the timer 
    /// </summary>
    public uint StartInterval_ms { get; set; } = 1_000; // 1sec

    /// <summary>
    /// the base interval for the timer 
    /// i.e. the shortest pace for any purpose
    /// </summary>
    public uint BaseInterval_ms { get; set; } = 150;

    /// <summary>
    /// cTor:
    /// </summary>
    public Timer( )
    {
      m_timer = new DispatcherTimer( );
      m_timer.Tick += M_timer_Tick;
    }

    /// <summary>
    /// Timer Callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void M_timer_Tick( object sender, EventArgs e )
    {
      m_timer.Interval = new TimeSpan( 0, 0, 0, 0, (int)BaseInterval_ms ); // switch to short interval
      m_handler?.Invoke( m_sender, new EventArgs() ); // invoke the attached handlers
    }

    /// <summary>
    /// Start the timer submitting a handler to invoke and event args
    /// </summary>
    /// <param name="handler">A handler to invoke</param>
    /// <param name="sender">The sender object caused the timing events</param>
    public void StartTimer( EventHandler<EventArgs> handler, object sender )
    {
      m_handler = handler;
      m_sender = sender;
      m_timer.Interval = new TimeSpan( 0, 0, 0, 0, (int)StartInterval_ms ); // wait before repeat
      m_timer.Start( );
    }

    /// <summary>
    /// Stop the timer 
    /// </summary>
    public void StopTimer( )
    {
      m_handler = null; // unhook
      m_timer.Stop( );
    }

  }
}
