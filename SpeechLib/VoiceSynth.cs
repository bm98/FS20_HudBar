using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;

namespace SpeechLib
{
  /// <summary>
  /// Implements the voice synthesizer i.e. the speaker
  /// </summary>
  internal class VoiceSynth : IDisposable
  {

    #region STATIC

    private static List<InstalledVoice> _installedVoices = new List<InstalledVoice>();

    // load from system
    private static void LoadInstalledVoices( )
    {
      var voices = SpeechSynthesizer.AllVoices;
      // build our internal catalog
      _installedVoices.Clear( );
      foreach ( var v in voices ) {
        var vInfo = new VoiceInfo(v.DisplayName, v.Language, v.Description, v.Id,
            (v.Gender== Windows.Media.SpeechSynthesis.VoiceGender.Male)? VoiceGender.Male: VoiceGender.Female);
        var insVoice = new InstalledVoice(vInfo){ Enabled=true };
        _installedVoices.Add( insVoice );
      }
    }

    /// <summary>
    ///  Returns all of the installed speech synthesis (text-to-speech) voices.
    /// </summary>
    /// <returns>Returns a read-only collection of the voices currently installed on the system.</returns>
    public static IReadOnlyCollection<InstalledVoice> GetInstalledVoices( )
    {
      LoadInstalledVoices( ); // reload in case the user has added some
      return _installedVoices;
    }

    #endregion

    // Speech
    private SpeechSynthesizer _synthesizer;
    private SpeechSynthesisStream _stream;
    private VoiceInformation _voice;

    // AudioGraph
    private AudioGraph _audioGraph;
    private AudioDeviceOutputNode _deviceOutputNode;
    private MediaSourceAudioInputNode _mediaSourceInputNode;
    private MediaSource _mediaSource;

    private AutoResetEvent _speakerWaitHandle; // will report the end of speak

    private bool _voiceAvailable = false;
    private bool _speaking = false; // true while speaking


    private bool _canSpeak = false; // true when all infrastructure is OK

    // Init Synth and get the default Voice
    private void InitVoice( )
    {
      // Speech
      if ( _synthesizer != null ) {
        // cleanup existing items
        _synthesizer?.Dispose( );
        _synthesizer = null;
      }

      _synthesizer = new SpeechSynthesizer( );
      _synthesizer.Options.AppendedSilence = SpeechAppendedSilence.Min;

      var voices = SpeechSynthesizer.AllVoices;
      if ( voices.Count <= 0 ) {
        Console.WriteLine( "SpeechLib: No Voices installed" );
        return; // NOPE..
      }

      // default: first english in the catalog
      _voice = voices.Where( x => x.Language.StartsWith( "en" ) ).FirstOrDefault( );
      if ( _voice == null ) {
        _voice = voices.FirstOrDefault( ); // no english.. any first will do
      }
      _synthesizer.Voice = _voice;
      _voiceAvailable = true;
    }

    // Init the AudioGraph
    //  despite the Aync methods - this will exec synchronously to get the InitPhase  only get done when all is available 
    private void InitAudioGraph( )
    {
      _canSpeak = false;
      // MUST WAIT UNTIL all items are created, else one may call Speak too early...
      Console.WriteLine( "SpeechLib-VoiceSynth: InitAudioGraph" );
      // cleanup existing items
      if ( _mediaSourceInputNode != null ) {
        if ( _deviceOutputNode != null ) _mediaSourceInputNode.RemoveOutgoingConnection( _deviceOutputNode );
        _mediaSourceInputNode.Dispose( );
        _mediaSourceInputNode = null;
      }
      if ( _deviceOutputNode != null ) { _deviceOutputNode.Dispose( ); _deviceOutputNode = null; }
      if ( _audioGraph != null ) { _audioGraph.Dispose( ); _audioGraph = null; }

      // Create an AudioGraph
      AudioGraphSettings settings = new AudioGraphSettings( Windows.Media.Render.AudioRenderCategory.Speech ) {
        PrimaryRenderDevice = null, // If PrimaryRenderDevice is null, the default playback device will be used.
        MaxPlaybackSpeedFactor = 1, // should preserve some memory
      };
      // We await here the execution without providing an async method ...
      var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync(settings));
      resultAG.Wait( );
      if ( resultAG.Result.Status != AudioGraphCreationStatus.Success ) {
        Console.WriteLine( $"SpeechLib-VoiceSynth: ERROR - AudioGraph creation error: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
          + $"\nExtError: {resultAG.Result.ExtendedError}");
        return;
      }
      _audioGraph = resultAG.Result.Graph;
      Console.WriteLine( $"SpeechLib-VoiceSynth: AudioGraph: [{_audioGraph.EncodingProperties}]" );

      // Create a device output node
      // The output node uses the PrimaryRenderDevice of the audio graph.
      // We await here the execution without providing an async method ...
      var resultDO = WindowsRuntimeSystemExtensions.AsTask( _audioGraph.CreateDeviceOutputNodeAsync());
      resultDO.Wait( );
      if ( resultDO.Result.Status != AudioDeviceNodeCreationStatus.Success ) {
        // Cannot create device output node
        Console.WriteLine( $"SpeechLib-VoiceSynth: ERROR - DeviceOutputNode creation error: {resultDO.Result.Status}, TaskStatus: {resultDO.Status}"
          + $"\nExtError: {resultDO.Result.ExtendedError}");
        return;
      }
      _deviceOutputNode = resultDO.Result.DeviceOutputNode;
      Console.WriteLine( $"SpeechLib-VoiceSynth: DeviceOutputNode: [{_deviceOutputNode.Device}]" );

      Console.WriteLine( $"SpeechLib-VoiceSynth: InitAudioGraph-END" );
      _canSpeak = true;
    }


    /// <summary>
    /// Common end of a speaking call
    /// </summary>
    private void EndOfSpeak( )
    {
      _speakerWaitHandle?.Set( ); // signal end of the Speak phase
      _speaking = false;
    }

    // Async Speak output of a text
    private async Task SpeakAsyncLow( string text )
    {
      _speaking = true; // locks additional calls for Speak until finished talking this bit

      // Speech
      if ( !_canSpeak 
            || _synthesizer == null || _audioGraph == null || _deviceOutputNode == null ) {
        Console.WriteLine( $"SpeechLib-Speak: ERROR - Synthesizer or AudioGraph does not exist: cannot speak..\n[{_synthesizer}] [{_audioGraph}] [{_deviceOutputNode}]" );
        EndOfSpeak( );
        return;
      }

      // Generate a new, independent audio stream from plain text.
      _stream = await _synthesizer.SynthesizeTextToStreamAsync( text );

      // Must create a MediaSource obj to derive a Stream Consumer InputNode
      _mediaSource?.Dispose( ); // clean old
      _mediaSource = MediaSource.CreateFromStream( _stream, _stream.ContentType );

      if ( _mediaSourceInputNode != null ) {
        // clean old nodes
        _mediaSourceInputNode.MediaSourceCompleted -= MediaSourceInputNode_MediaSourceCompleted; // detach handler
        _mediaSourceInputNode.Dispose( );
      }
      // create the InputNode
      var resultMS = await _audioGraph.CreateMediaSourceAudioInputNodeAsync( _mediaSource );
      if ( resultMS.Status != MediaSourceAudioInputNodeCreationStatus.Success ) {
        // Cannot create input node
        Console.WriteLine( $"SpeechLib-Speak: MediaSourceAudioInputNode creation error: {resultMS.Status}\nExtError: {resultMS.ExtendedError}" );
        EndOfSpeak( );
        return; // cannot speak
      }
      _mediaSourceInputNode = resultMS.Node;
      // add a handler to stop and signale when finished
      _mediaSourceInputNode.MediaSourceCompleted += MediaSourceInputNode_MediaSourceCompleted; 
      _mediaSourceInputNode.AddOutgoingConnection( _deviceOutputNode ); // connect the graph
      // Speak it
      _audioGraph.Start( ); // Speaks in the current Thread - cannot be waited for in the same thread
    }

    // will be called when speaking has finished
    private void MediaSourceInputNode_MediaSourceCompleted( MediaSourceAudioInputNode sender, object args )
    {
      _audioGraph?.Stop( );
      _deviceOutputNode?.Start( ); // restart output - needed ??

      EndOfSpeak( );
    }


    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public VoiceSynth( AutoResetEvent resetEvent )
    {
      _speakerWaitHandle = resetEvent;
      _speakerWaitHandle.Reset( );

      /* AudioGraph*/
      LoadInstalledVoices( );
      InitVoice( );
      if ( _voiceAvailable ) {
        InitAudioGraph( );
      }
    }

    /// <summary>
    /// Selects a specific voice by name.
    /// </summary>
    /// <param name="displayName">The name of the voice to select</param>
    public void SelectVoice( string displayName )
    {
      if ( !_voiceAvailable ) return; // Cannot
      // select the one requested
      var voices = SpeechSynthesizer.AllVoices;
      _voice = voices.Where( x => x.DisplayName == displayName ).FirstOrDefault( );
      if ( _voice == null ) {
        _voice = voices.FirstOrDefault( ); // none found - grab the first available
      }
      _synthesizer.Voice = _voice;
    }


    /// <summary>
    /// Asynchronously speaks the contents of a string.
    ///  Waiting for this will not wait for when the Speech has ended
    ///  but rather when the trigger for Output was given
    ///  There is currently no event available to detect end of Speech
    /// </summary>
    /// <param name="text">The text to speak</param>
    public async Task SpeakAsync( string text )
    {
      if ( !_voiceAvailable ) return; // Cannot
      if ( !_canSpeak ) return; // Cannot
      if ( _speaking ) return; // no concurrent talking

      await SpeakAsyncLow( text );
    }

    /// <summary>
    /// Reset the speakers concurrency check
    ///  don't know if the end event is fired in any case - this is to recover
    /// </summary>
    public void Reset( )
    {
      EndOfSpeak( );
    }



    #region DISPOSE

    private bool disposedValue;

    /// <summary>
    /// Overridable Dispose
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Disposing flag</param>
    protected virtual void Dispose( bool disposing )
    {
      if ( !disposedValue ) {
        if ( disposing ) {
          // dispose managed state (managed objects)
          // cleanup existing items
          _synthesizer?.Dispose( );
          _mediaSourceInputNode?.RemoveOutgoingConnection( _deviceOutputNode );
          _mediaSourceInputNode?.Dispose( );
          _deviceOutputNode?.Dispose( );
          _audioGraph?.Dispose( );
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
