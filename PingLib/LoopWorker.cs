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
  ///  Tunes ( less than 10sec chunks assumed..) will be played as loop
  ///  - InitSpeaker() before start adding SoundBites
  ///  - Change the Tune while playing
  ///  - Cancel if Done
  ///  - Mute if needed
  /// </summary>
  internal class LoopWorker : BackgroundWorker, IDisposable
  {
    private const int s_timeout = 10_000; // timout of play per tune

    private AutoResetEvent _playerWaitHandle;
    private int _progressCount = 0;

    // The item to play from
    private object _soundBiteLock = new object();
    private SoundBite _soundBite = new SoundBite();

    private WaveProc _player;

    private bool _muted;

    /// <summary>
    /// cTor: creates a Player Task (BG Worker)
    /// </summary>
    public LoopWorker( )
    {
      _playerWaitHandle = new AutoResetEvent( false );
      _player = new WaveProc( _playerWaitHandle, true );

      this.WorkerSupportsCancellation = true;
      this.WorkerReportsProgress = true;

      this.DoWork += SoundWorker_DoWork;
    }


    /// <summary>
    /// Start the Sound processing
    /// </summary>
    /// <param name="parameter">A parameter obj to use (NOT USED)</param>
    public void InitPlayer( object parameter )
    {
      if ( !this.IsBusy ) {
        // restart
        _player?.Dispose( );
        _playerWaitHandle.Dispose( );

        _playerWaitHandle = new AutoResetEvent( false );
        _player = new WaveProc( _playerWaitHandle, true );

        ClearSoundBites( );
        _progressCount = 0;
        this.RunWorkerAsync( parameter );
      }
      else {
        throw new InvalidOperationException( "PingLib-Loop: ERROR - Cannot InitPlayer while Busy. Must Cancel first!" );
      }
    }

    /// <summary>
    /// Change the sound to play
    /// </summary>
    /// <param name="soundBite">The SoundBite to play from now on</param>
    public void AddSoundBite( SoundBite soundBite )
    {
      if ( !this.IsBusy ) {
        throw new InvalidOperationException( "PingLib-Loop: ERROR - Cannot add SoundBites when not initialized" );
      }

      lock ( _soundBiteLock ) {
        _soundBite = new SoundBite( soundBite );
      }
    }

    /// <summary>
    /// Clear the current Word queue
    /// </summary>
    public void ClearSoundBites( )
    {
      lock ( _soundBiteLock ) {
        _soundBite = new SoundBite( ); // Load a default (which is Silence in 1sec duration tones)
      }
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
    /// Plays the SoundBite in a Loop until cancelled
    ///   the BGWorker Main loop
    /// </summary>
    private void SoundWorker_DoWork( object sender, DoWorkEventArgs e )
    {
      bool doWork = !this.CancellationPending;
      SoundBite soundbite = new SoundBite();

      // Loop
      while ( true ) {
        // endless loop to play tones, default will take the Tones from the Silcence Melody
        _playerWaitHandle.Reset( );
        lock ( _soundBiteLock ) {
          // Cannot await - the BGWorker is a workaround to provide a .Net framework library
          // Waiting for completion is done via the _playerHandle Sema, will be set on termination of the play by the _player
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          _player.PlayAsync( _soundBite );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

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

        // check for cancellation requested
        doWork &= ( !this.CancellationPending );
        if ( !doWork ) break;
      } // TASK LOOP

      // finishing here
      _playerWaitHandle.Reset( );
      _player?.Dispose( );
      _player = null;
      // requires a new Init to proceed
    }


  }
}

