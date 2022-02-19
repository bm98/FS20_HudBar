using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingLib
{

  /// <summary>
  /// An Audio Player Object using the Win10 TTS API 
  /// Wrapper to be used to output short tunes
  /// 
  /// Cannot be inherited
  /// </summary>
  public sealed class Sounds : IDisposable
  {
    #region STATIC

    /// <summary>
    ///  Returns all of the installed sounds.
    /// </summary>
    /// <returns>Returns a read-only collection of the sounds currently available from the library</returns>
    public static IReadOnlyCollection<SoundInfo> InstalledSounds => WaveProc.GetInstalledSounds( );


    #endregion

    // Background worker, Audio Output
    private SoundWorker _player;

    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public Sounds( )
    {
      _player = new SoundWorker( );
      _player.InitPlayer( null );
      _player.ProgressChanged += _player_ProgressChanged;
    }

    private void _player_ProgressChanged( object sender, System.ComponentModel.ProgressChangedEventArgs e )
    {
      // handle synch later
    }


    /// <summary>
    /// Asynchronously plays soundBites
    /// Audio output is the default output device
    ///  This is not an awaitable method
    /// </summary>
    /// <param name="soundBite">The SoundBite to play (max 10sec supported)</param>
    public void PlayAsync( SoundBite soundBite )
    {
      _player.AddSoundBite( soundBite );
    }

    /// <summary>
    /// Synchronously plays soundBites
    /// Audio output is the default output device
    /// NOT YET IMPLEMENTED behaves the same as PlayAsync
    /// </summary>
    /// <param name="soundBite">The SoundBite to play</param>

    public void Play( SoundBite soundBite )
    {
      _player.AddSoundBite( soundBite );
      // TODO 
    }

    /// <summary>
    /// Get; Set; Mute / Unmute the player 
    /// </summary>
    public bool Mute { get => _player.Mute; set => _player.Mute = value; }

    #region DISPOSE

    private bool disposedValue;

    /// <summary>
    /// Overridable Dispose
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Disposing flag</param>
    private void Dispose( bool disposing )
    {
      if ( !disposedValue ) {
        if ( disposing ) {
          // dispose managed state (managed objects)
          _player.CancelAsync( );
          _player.Dispose( );
        }

        disposedValue = true;
      }
    }

    /// <summary>
    /// Final Dispose of the class
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }


}
