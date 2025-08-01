﻿using System;
using System.Collections.Generic;

using DbgLib;

using Windows.Devices.Enumeration;

namespace PingLib
{
  /// <summary>
  /// An Audio Loop Player Object using the Win10 TTS API 
  /// Wrapper to be used to output loops of short tunes
  /// 
  /// Cannot be inherited
  /// </summary>
  public sealed class Loops : IDisposable
  {
    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType);

    // Static cTor: Just report the Library to Dbg
    static Loops( )
    {
      LOG.Info( $"Init Module" );
    }
    /*
    /// <summary>
    /// Deletes the temp sound files deployed by this module (Streamed files)
    /// </summary>
    public static void RemoveTempSounds( )
    {
      WaveProc.RemoveTempSounds( );
    }
    */

    /// <summary>
    ///  Returns all of the installed sounds.
    /// </summary>
    /// <returns>Returns a read-only collection of the sounds currently available from the library</returns>
    public static IReadOnlyCollection<SoundInfo> InstalledSounds => WaveProc.GetInstalledSounds( );

    /// <summary>
    /// Returns all installed output devices
    /// </summary>
    /// <returns>Returns a read-only collection of the output devices currently installed on the system.</returns>
    public static IReadOnlyCollection<DeviceInformation> InstalledOutputDevices => WaveProc.GetInstalledOutputDevices( );

    #endregion

    // Background worker, Audio Output
    private LoopWorker _player;

    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public Loops( )
    {
      _player = new LoopWorker( );
      _player.InitPlayer( null );
      _player.ProgressChanged += _player_ProgressChanged;
    }

    private DateTime _prev = DateTime.Now;
    private void _player_ProgressChanged( object sender, System.ComponentModel.ProgressChangedEventArgs e )
    {
      /* DEBUG only
      var now = DateTime.Now;
      Console.WriteLine( $"{e.ProgressPercentage,5:###0}  - {now: O}  {(now-_prev).TotalMilliseconds,8:##,##0}" );
      _prev = now;
      */
    }

    /// <summary>
    /// Selects a specific Output device by name.
    /// </summary>
    /// <param name="displayName">The name of the device to select</param>
    public void SelectOutputDevice( string displayName )
    {
      _player?.SelectOutputDevice( displayName );
    }

    /// <summary>
    /// Asynchronously plays loops of sound bites.
    /// Audio output is the default output device
    ///  This is not an awaitable method
    /// </summary>
    /// <param name="soundBite">The SoundBite to play (max 10sec supported)</param>
    public void PlayAsync( SoundBite soundBite )
    {
      _player.AddSoundBite( soundBite );
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
