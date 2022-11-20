using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Media.Core;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Media.SpeechSynthesis;

using DbgLib;
using System.IO;
using Windows.Devices.Enumeration;

namespace SpeechLib
{
  /// <summary>
  /// Implements the voice synthesizer i.e. the speaker
  /// </summary>
  internal class VoiceSynth : IDisposable
  {

    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // List of installed voices
    private static List<InstalledVoice> _installedVoices = new List<InstalledVoice>( );
    private static List<DeviceInformation> _installedOutputDevices = new List<DeviceInformation>( );

    // load from system
    private static void LoadInstalledVoices( )
    {
      var voices = SpeechSynthesizer.AllVoices;
      // build our internal catalog
      _installedVoices.Clear( );
      foreach (var v in voices) {
        var vInfo = new VoiceInfo( v.DisplayName, v.Language, v.Description, v.Id,
            (v.Gender == Windows.Media.SpeechSynthesis.VoiceGender.Male) ? VoiceGender.Male : VoiceGender.Female );
        var insVoice = new InstalledVoice( vInfo ) { Enabled = true };
        _installedVoices.Add( insVoice );
      }
    }

    // Collect Audio Render Device(s) to see if any is available
    private static void LoadInstalledOutputDevices( )
    {
      _installedOutputDevices.Clear( );
      var ret = DeviceInformation.FindAllAsync( DeviceClass.AudioRender ).AsTask( );
      ret.Wait( );
      if (ret.Status == TaskStatus.RanToCompletion) {
        foreach (DeviceInformation deviceInterface in ret.Result) {
          _installedOutputDevices.Add( deviceInterface );
        }
      }
      else {
        LOG.LogError( $"LoadInstalledOutputDevices: Status {ret.Status}" );
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

    /// <summary>
    /// Returns all installed output devices
    /// </summary>
    /// <returns>Returns a read-only collection of the output devices currently installed on the system.</returns>
    public static IReadOnlyCollection<DeviceInformation> GetInstalledOutputDevices( )
    {
      LoadInstalledOutputDevices( );
      return _installedOutputDevices;
    }

    #endregion

    // Speech
    private SpeechSynthesizer _synthesizer = null;
    private SpeechSynthesisStream _stream = null;
    private VoiceInformation _voice = null;

    // AudioGraph
    private AudioGraph _audioGraph = null;
    private AudioDeviceOutputNode _deviceOutputNode = null;
    private MediaSourceAudioInputNode _mediaSourceInputNode = null;
    private MediaSource _mediaSource = null;
    private DeviceInformation _outputDevice = null;

    private AutoResetEvent _speakerWaitHandle = null; // will report the end of speak

    private bool _voiceAvailable = true; // true if any voice is available
    private bool _speaking = false; // true while speaking
    private bool _canSpeak = true; // true when all infrastructure is OK

    #region Render Category

    // Mean to find a render category that works, sometimes the devices are not available for certain categories
    // Found that Speech may reply 0x8889000A (Device in Use) but not other categories ?? No MS Doc for this

    private const AudioRenderCategory _renderNone = AudioRenderCategory.Alerts; // Alerts is interpreted as NA...
    private AudioRenderCategory _renderCat = _renderNone;

    // Find a valid AudioGraph RenderCategory
    // this should leave _renderCat with a valid one or _renderNone
    private void FindRenderCategory( )
    {
      // A list of tryouts for the output rendering
      Queue<AudioRenderCategory> renderSequence = new Queue<AudioRenderCategory>( new[]{
        AudioRenderCategory.Speech,
        AudioRenderCategory.GameChat,
        AudioRenderCategory.GameEffects,
        AudioRenderCategory.SoundEffects,
        AudioRenderCategory.Media,
        AudioRenderCategory.Other,
        // Finally the Not Available Cat
        _renderNone,
      } );
      _renderCat = renderSequence.Dequeue( );

      // Try a cat that works
      do {
        // Create an AudioGraph
        AudioGraphSettings settings = new AudioGraphSettings( _renderCat ) {
          PrimaryRenderDevice = _outputDevice, // If PrimaryRenderDevice is null, the default playback device will be used.
        };
        LOG.Log( $"FindRenderCategory: About to test AudioGraph with RenderCategory: {_renderCat}" );
        // We await here the execution without providing an async method ...
        var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
        resultAG.Wait( );
        if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
          LOG.LogError( $"FindRenderCategory: AudioGraph test error: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
            + $"\nExtError: {resultAG.Result.ExtendedError}" );

          // try next category if there is one left
          if (renderSequence.Count > 0) {
            _renderCat = renderSequence.Dequeue( );
          }
          else {
            // sanity - should never happen
            LOG.LogError( $"FindRenderCategory: Program error - Queue overrun" );
            _renderCat = _renderNone;
            return;
          }
        }
        else {
          resultAG.Result.Graph?.Dispose( ); // not used after tryout
          LOG.Log( $"FindRenderCategory: Success with RenderCategory: {_renderCat}" );
          return; // _renderCat contains a successful one
        }
      } while (_renderCat != _renderNone);

      LOG.LogError( $"FindRenderCategory: Failed to find a working RenderCategory - cannot speak" );
      _canSpeak = false;
      return; // could not resolve - left with _renderNone
    }

    #endregion

    // Init Synth and get the default Voice
    private void InitVoice( )
    {
      // Speech
      if (_synthesizer != null) {
        // cleanup existing items
        _synthesizer?.Dispose( );
        _synthesizer = null;
      }

      _synthesizer = new SpeechSynthesizer( );
      _synthesizer.Options.AppendedSilence = SpeechAppendedSilence.Min;

      var voices = SpeechSynthesizer.AllVoices;
      if (voices.Count <= 0) {
        LOG.Log( "InitVoice: No Voices installed or found" );
        _voiceAvailable = false;
        return; // NOPE..
      }

      // default: first english in the catalog
      _voice = voices.Where( x => x.Language.StartsWith( "en" ) ).FirstOrDefault( );
      if (_voice == null) {
        _voice = voices.FirstOrDefault( ); // no english.. any first will do
      }
      _synthesizer.Voice = _voice;
    }


    // Init the AudioGraph
    //  despite the Aync methods - this will exec synchronously to get the InitPhase  only get done when all is available 
    private void InitAudioGraph( )
    {
      LOG.Log( "InitAudioGraph: Begin" );
      // find a render cat (see notes above)
      FindRenderCategory( );

      // MUST WAIT UNTIL all items are created, else one may call Speak too early...
      // cleanup existing items
      if (_mediaSourceInputNode != null) {
        if (_deviceOutputNode != null) _mediaSourceInputNode.RemoveOutgoingConnection( _deviceOutputNode );
        _mediaSourceInputNode.Dispose( );
        _mediaSourceInputNode = null;
      }
      if (_deviceOutputNode != null) { _deviceOutputNode.Dispose( ); _deviceOutputNode = null; }
      if (_audioGraph != null) { _audioGraph.Dispose( ); _audioGraph = null; }

      // Create an AudioGraph
      AudioGraphSettings settings = new AudioGraphSettings( _renderCat ) {
        PrimaryRenderDevice = _outputDevice,  // If PrimaryRenderDevice is null, the default playback device will be used.
      };
      // We await here the execution without providing an async method ...
      var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
      resultAG.Wait( );
      if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
        LOG.LogError( $"InitAudioGraph: Failed to create AudioGraph with RenderCategory: {_renderCat}" );
        LOG.LogError( $"InitAudioGraph: AudioGraph creation: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
                        + $"\nExtError: {resultAG.Result.ExtendedError}" );
        _canSpeak = false;
        return; // ERROR EXIT
      }
      _audioGraph = resultAG.Result.Graph;
      LOG.Log( $"InitAudioGraph: AudioGraph: [{_audioGraph.EncodingProperties}]" );

      // Create a device output node
      // The output node uses the PrimaryRenderDevice of the audio graph.
      // We await here the execution without providing an async method ...
      var resultDO = WindowsRuntimeSystemExtensions.AsTask( _audioGraph.CreateDeviceOutputNodeAsync( ) );
      resultDO.Wait( );
      if (resultDO.Result.Status != AudioDeviceNodeCreationStatus.Success) {
        // Cannot create device output node
        LOG.LogError( $"InitAudioGraph: DeviceOutputNode creation: {resultDO.Result.Status}, TaskStatus: {resultDO.Status}"
                        + $"\nExtError: {resultDO.Result.ExtendedError}" );
        _canSpeak = false;
        return; // ERROR EXIT
      }
      _deviceOutputNode = resultDO.Result.DeviceOutputNode;
      _canSpeak = true; // finally

      var devName = (_deviceOutputNode.Device == null) ? "Standard Output Device" : _deviceOutputNode.Device.Name;
      LOG.Log( $"InitAudioGraph: DeviceOutputNode: [{devName}]" );
      LOG.Log( $"InitAudioGraph: InitAudioGraph-END" );
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
      if (!_canSpeak || _synthesizer == null || _audioGraph == null || _deviceOutputNode == null) {
        LOG.LogError( $"SpeakAsyncLow: Some item do not exist: cannot speak..\n[{_synthesizer}] [{_audioGraph}] [{_deviceOutputNode}]" );
        EndOfSpeak( );
        return;
      }

      // Generate a new, independent audio stream from plain text.
      _stream = await _synthesizer.SynthesizeTextToStreamAsync( text );

      // Must create a MediaSource obj to derive a Stream Consumer InputNode
      _mediaSource?.Dispose( ); // clean old
      _mediaSource = MediaSource.CreateFromStream( _stream, _stream.ContentType );

      if (_mediaSourceInputNode != null) {
        // clean old nodes
        _mediaSourceInputNode.MediaSourceCompleted -= MediaSourceInputNode_MediaSourceCompleted; // detach handler
        _mediaSourceInputNode.Dispose( );
      }
      // create the InputNode
      var resultMS = await _audioGraph.CreateMediaSourceAudioInputNodeAsync( _mediaSource );
      if (resultMS.Status != MediaSourceAudioInputNodeCreationStatus.Success) {
        // Cannot create input node
        LOG.LogError( $"SpeakAsyncLow: MediaSourceAudioInputNode creation: {resultMS.Status}\nExtError: {resultMS.ExtendedError}" );
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
      // Init - will be flaged during init if something goes wrong
      _voiceAvailable = true;
      _canSpeak = false; // not yet

      _speakerWaitHandle = resetEvent;
      _speakerWaitHandle.Reset( );

      LoadInstalledVoices( );
      InitVoice( );

      /* AudioGraph*/
      if (_voiceAvailable) {
        InitAudioGraph( );
      }
    }

    /// <summary>
    /// Selects a specific voice by name.
    /// </summary>
    /// <param name="displayName">The name of the voice to select</param>
    public void SelectVoice( string displayName )
    {
      LOG.Log( $"SelectVoice: {displayName}" );
      if (!_voiceAvailable) return; // Cannot, no voices installed
      // select the one requested
      var voices = SpeechSynthesizer.AllVoices;
      _voice = voices.FirstOrDefault( x => x.DisplayName == displayName );
      if (_voice == null) {
        _voice = voices.FirstOrDefault( ); // none found - grab the first available
      }
      _synthesizer.Voice = _voice;
    }

    /// <summary>
    /// Selects a specific output device by name.
    /// </summary>
    /// <param name="displayName">The name of the output device to select</param>
    public void SelectOutputDevice( string displayName )
    {
      LOG.Log( $"SelectOutputDevice: {displayName}" );
      if (!_voiceAvailable) return; // Cannot, no voices installed

      _canSpeak = false; // not yet
      // select the one requested
      var oDevs = GetInstalledOutputDevices( );
      _outputDevice = oDevs.FirstOrDefault( x => x.Name == displayName ); // null will select the defaultOutput
      InitAudioGraph( ); // restart the Graph
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
      if (!_voiceAvailable) return; // Cannot
      if (!_canSpeak) return; // Cannot
      if (_speaking) return; // no concurrent talking

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
      if (!disposedValue) {
        if (disposing) {
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
