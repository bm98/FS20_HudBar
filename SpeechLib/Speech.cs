using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

using DbgLib;

namespace SpeechLib
{
  /// <summary>
  /// A Speech Object using the Win10 TTS API 
  /// Wrapper to be used to output short texts given as strings
  /// Select a voice from the Installed Voices
  /// Voice output is the default output device
  /// 
  /// Cannot be inherited
  /// </summary>
  public sealed class Speech : IDisposable
  {
    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Static cTor: Just report the Library to Dbg
    static Speech( )
    {
      LOG.Log( $"Init Module" );
    }

    /// <summary>
    ///  Returns all of the installed speech synthesis (text-to-speech) voices.
    /// </summary>
    /// <returns>Returns a read-only collection of the voices currently installed on the system.</returns>
    public static IReadOnlyCollection<InstalledVoice> InstalledVoices => VoiceSynth.GetInstalledVoices( );

    /// <summary>
    /// Returns all installed output devices
    /// </summary>
    /// <returns>Returns a read-only collection of the output devices currently installed on the system.</returns>
    public static IReadOnlyCollection<DeviceInformation> InstalledOutputDevices => VoiceSynth.GetInstalledOutputDevices( );

    #endregion

    // Background worker, Voice Output
    private SpeechWorker _speaker;

    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public Speech( )
    {
      _speaker = new SpeechWorker( );
      _speaker.InitSpeaker( null );
      _speaker.ProgressChanged += _speaker_ProgressChanged;
    }

    private void _speaker_ProgressChanged( object sender, System.ComponentModel.ProgressChangedEventArgs e )
    {
      // handle synch later
    }


    /// <summary>
    /// Selects a specific voice by name.
    /// </summary>
    /// <param name="displayName">The name of the voice to select</param>
    public void SelectVoice( string displayName )
    {
      _speaker?.SelectVoice( displayName );
    }

    /// <summary>
    /// Selects a specific Output device by name.
    /// </summary>
    /// <param name="displayName">The name of the device to select</param>
    public void SelectOutputDevice( string displayName )
    {
      _speaker?.SelectOutputDevice( displayName );
    }


    /// <summary>
    /// Asynchronously speaks the contents of a string.
    /// Voice output is the default output device
    ///  This is not an awaitable method
    /// </summary>
    /// <param name="text">The string to speak (Max 200 chars supported)</param>
    public void SpeakAsync( string text )
    {
      _speaker.AddWord( text );
    }

    /// <summary>
    /// Synchronously speaks the contents of a string.
    /// Voice output is the default output device
    /// NOT YET IMPLEMENTED behaves the same as SpeakAsync
    /// </summary>
    /// <param name="text">The string to speak (Max 200 chars supported)</param>
    public void Speak( string text )
    {
      _speaker.AddWord( text );
      // TODO Synch.. with _speaking properly...
    }

    #region DISPOSE

    private bool disposedValue;

    /// <summary>
    /// Overridable Dispose
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Disposing flag</param>
    private void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // dispose managed state (managed objects)
          _speaker.CancelAsync( );
          _speaker.Dispose( );
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
