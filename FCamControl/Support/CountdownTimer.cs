using System;

using Timer = System.Timers.Timer;

namespace FCamControl
{
  /// <summary>
  /// A count down timer with reporting
  /// Resolution is 100ms
  /// </summary>
  internal class CountdownTimer
  {
    private const int c_intervall_ms = 100;
    private double _duration_ms = 0;
    private int _countDown_ms = 0;

    private Timer _timer;


    /// <summary>
    /// Timer Duration in ms
    /// Set - resolution is 100ms
    /// </summary>
    public double Duration_ms { get => _duration_ms; set => _duration_ms = value; }

    /// <summary>
    /// Remaining time until elapsed
    /// </summary>
    public int Remaining_ms => _countDown_ms;

    /// <summary>
    /// Reports progress every 100 ms
    /// </summary>
    public event EventHandler<EventArgs> Progress;
    private void OnProgress( ) => Progress?.Invoke( this, EventArgs.Empty );

    public CountdownTimer( )
    {
      _timer = new Timer( );
      _timer.Stop( );
      _timer.Elapsed += _timer_Elapsed;
    }

    private void _timer_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
    {
      _countDown_ms -= c_intervall_ms;
      // finished 
      if (_countDown_ms <= 0) {
        _timer.AutoReset = false;
        _timer.Stop( );
        _countDown_ms = 0;
      }
      OnProgress( );
    }

    /// <summary>
    /// Start the Timer
    /// </summary>
    public void Start( )
    {
      _timer.AutoReset = true;
      _timer.Interval = c_intervall_ms;
      _countDown_ms = (int)_duration_ms;
      _timer.Start( );
    }

    /// <summary>
    /// Stop the Timer
    /// </summary>
    public void Stop( )
    {
      _timer.Stop( );
      _countDown_ms = 0;
    }


  }
}
