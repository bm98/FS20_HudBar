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
  /// Implements an Background thread which does the sound output
  /// so the calling entity is able to proceed with working
  ///  Tunes ( less than 10sec chunks assumed..) will be queued and serialized sound output is generated
  ///  - InitSpeaker() before start adding SoundBites
  /// </summary>
  internal class SoundWorker : BackgroundWorker, IDisposable
  {
    private const int c_timeout = 1000; // timout to check for cancellation
    private const int s_timeout = 10_000; // timout of speech

    private AutoResetEvent _workerWaitHandle;
    private AutoResetEvent _playerWaitHandle;
    private int _progressCount = 0;

    // The queue to speak
    private Queue<SoundBite> _soundBites = new Queue<SoundBite>();

    private WaveProc _player;

    private bool _muted;

    /// <summary>
    /// cTor: creates a Player Task (BG Worker)
    /// </summary>
    public SoundWorker( )
    {
      _workerWaitHandle = new AutoResetEvent( false );
      _playerWaitHandle = new AutoResetEvent( false );
      _player = new WaveProc( _playerWaitHandle, false );

      this.WorkerSupportsCancellation = true;
      this.WorkerReportsProgress = true;

      this.DoWork += SoundWorker_DoWork;
    }


    /// <summary>
    /// Start the Speaker processing
    /// </summary>
    /// <param name="parameter">A parameter obj to use (NOT USED)</param>
    public void InitPlayer( object parameter )
    {
      if ( !this.IsBusy ) {
        // restart
        _player?.Dispose( );
        _workerWaitHandle.Dispose( );
        _playerWaitHandle.Dispose( );

        _workerWaitHandle = new AutoResetEvent( false );
        _playerWaitHandle = new AutoResetEvent( false );
        _player = new WaveProc( _playerWaitHandle, false );

        ClearSoundBites( );
        _progressCount = 0;
        this.RunWorkerAsync( parameter );
      }
      else {
        throw new InvalidOperationException( "PingLib-Sound: ERROR Cannot InitPlayer while Busy. Must Cancel first!" );
      }
    }

    /// <summary>
    /// Add a sound to play
    /// </summary>
    /// <param name="soundBite">The SoundBite to play</param>
    public void AddSoundBite( SoundBite soundBite )
    {
      if ( !this.IsBusy ) {
        throw new InvalidOperationException( "PingLib-Sound: ERROR Cannot add SoundBites when not initialized" );
      }

      lock ( _soundBites ) {
        _soundBites.Enqueue( soundBite );
        // restart if needed
        if ( _soundBites.Count == 1 )
          _workerWaitHandle.Set( );
      }
    }

    /// <summary>
    /// Clear the current Word queue
    /// </summary>
    public void ClearSoundBites( )
    {
      lock ( _soundBites ) {
        _soundBites.Clear( );
      }
      _workerWaitHandle.Reset( );
    }

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
    private void SoundWorker_DoWork( object sender, DoWorkEventArgs e )
    {
      bool doWork = !this.CancellationPending;
      SoundBite soundbite = new SoundBite();

      // Loop
      while ( true ) {
        // wait for incoming play orders
        if ( _workerWaitHandle.WaitOne( c_timeout ) ) {
          // signalled
          lock ( _soundBites ) {
            if ( _soundBites.Count > 0 ) {
              soundbite = _soundBites.Dequeue( );
            }
          }

          _playerWaitHandle.Reset( );
          // Cannot await - the BGWorker is a workaround to provide a .Net framework library
          // Waiting for completion is done via the _playerHandle Sema, will be set on termination of the play by the _player
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          _player.PlayAsync( soundbite );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

          // wait until finished playing
          if ( _playerWaitHandle.WaitOne( s_timeout ) ) {
            // report a tune played
            this.ReportProgress( ++_progressCount );
          }
          else {
            // run into a play time out - either the tune was tool long or something else prevented
            // the Player to signal end of play
            // anyway try to recover and continue
            _player.Reset( );
          }
        } // Look for sound to play loop
          // times out eventually if there is nothing to do, checking for Abort signal

        // check for cancellation requested
        doWork &= ( !this.CancellationPending );
        if ( !doWork ) break;

        // retrigger if there are soundBites left
        if ( _soundBites.Count > 0 ) {
          _workerWaitHandle.Set( );
        }

      } // TASK LOOP

      // finishing here
      _soundBites.Clear( );
      _workerWaitHandle.Reset( );
      _playerWaitHandle.Reset( );
      _player?.Dispose( );
      _player = null;
      // requires a new Init to proceed
    }


  }
}
