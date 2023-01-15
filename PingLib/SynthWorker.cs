using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PingLib
{
  /// <summary>
  /// Implements a Synthetic Sound Machine
  ///  - Define Base Frequ and Shape
  ///  - Mute if needed
  /// </summary>
  internal class SynthWorker : BackgroundWorker, IDisposable
  {
    private const int c_timeout = 1000; // timout to check for cancellation

    private SynthWave _sWave= null;
    private SynthProc _player;

    private bool _muted;

    /// <summary>
    /// cTor: creates a Player Task (BG Worker)
    /// </summary>
    public SynthWorker( )
    {
      _sWave = new SynthWave();
      _player = new SynthProc( _sWave );

      this.WorkerSupportsCancellation = true;
      this.WorkerReportsProgress = true;

      this.DoWork += SynthWorker_DoWork;
    }

    /// <summary>
    /// Selects a specific output device by name.
    /// </summary>
    /// <param name="displayName">The name of the output to select</param>
    public void SelectOutputDevice( string displayName )
    {
      _player?.SelectOutputDevice( displayName );
    }

    /// <summary>
    /// Start the Speaker processing
    /// </summary>
    /// <param name="parameter">A parameter obj to use (NOT USED)</param>
    public void InitPlayer( object parameter )
    {
      if (!this.IsBusy) {
        // restart
        _player?.Dispose( );

        _player = new SynthProc( _sWave );

        this.RunWorkerAsync( parameter );
      }
      else {
        throw new InvalidOperationException( "PingLib-Synth: ERROR Cannot InitPlayer while Busy. Must Cancel first!" );
      }
    }

    /// <summary>
    /// Returns the used waveform
    /// </summary>
    public SynthWave WaveForm { get { return _sWave; } }

    /// <summary>
    /// Mute the player 
    /// </summary>
    public bool Mute {
      get => _muted;
      set {
        _muted = value;
        _player?.Mute( _muted );
      }
    }


    /// <summary>
    /// Processes the word queue one by one
    ///   the BGWorker Main loop
    /// </summary>
    private void SynthWorker_DoWork( object sender, DoWorkEventArgs e )
    {
      bool doWork = !this.CancellationPending;

      // Loop
      while (true) {
        Thread.Sleep( c_timeout );
        // check for cancellation requested
        doWork &= (!this.CancellationPending);
        if (!doWork) break;

      } // TASK LOOP

      // finishing here
      _player?.Dispose( );
      _player = null;
      // requires a new Init to proceed
    }


  }
}
