using DbgLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace PingLib
{
  /// <summary>
  /// Synth supporting Audio Output 
  /// 
  /// Reference: Some content is copied or derived from MS Online Doc about AudioGraphs
  /// </summary>
  internal class SynthProc : IDisposable
  {

    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // files we streamed to play - such files are put into the users Temp folder and remain there until someone cleans them up..
    private static List<DeviceInformation> _installedOutputDevices = new List<DeviceInformation>( );

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
        LOG.LogError( "LoadInstalledOutputDevices", $"Status {ret.Status}" );
      }
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

    // Wave Source
    private AudioFrameInputNode _frameInputNode = null;

    // AudioGraph
    private AudioGraph _audioGraph = null;
    private AudioDeviceOutputNode _deviceOutputNode = null;
    private DeviceInformation _outputDevice = null;

    private SynthWave _sWave = null;
    private bool _canPlay = true; // true when all infrastructure is OK

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
        AudioRenderCategory.SoundEffects,
        AudioRenderCategory.GameEffects,
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
        LOG.Log( "FindRenderCategory", $"About to test AudioGraph with RenderCategory: {_renderCat}" );
        // We await here the execution without providing an async method ...
        var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
        resultAG.Wait( );
        if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
          LOG.LogError( $"FindRenderCategory", $"AudioGraph test error: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
            + $"\nExtError: {resultAG.Result.ExtendedError}" );

          // try next category if there is one left
          if (renderSequence.Count > 0) {
            _renderCat = renderSequence.Dequeue( );
          }
          else {
            // sanity - should never happen
            LOG.LogError( $"FindRenderCategory", $"Program error - Queue overrun" );
            _renderCat = _renderNone;
            return;
          }
        }
        else {
          resultAG.Result.Graph?.Dispose( ); // not used after tryout
          LOG.Log( $"FindRenderCategory", $"Success with RenderCategory: {_renderCat}" );
          return; // _renderCat contains a successful one
        }
      } while (_renderCat != _renderNone);

      LOG.LogError( $"FindRenderCategory", $"Failed to find a working RenderCategory - cannot speak" );
      _canPlay = false;
      return; // could not resolve - left with _renderNone
    }

    #endregion

    // Init the AudioGraph
    //  despite the Aync methods - this will exec synchronously to get the InitPhase  only get done when all is available 
    private void InitAudioGraph( )
    {
      LOG.Log( "InitAudioGraph", "Begin" );
      // MUST WAIT UNTIL all items are created, else one may call Play too early...
      // cleanup existing items
      if (_deviceOutputNode != null) { _deviceOutputNode.Dispose( ); _deviceOutputNode = null; }
      if (_audioGraph != null) {
        _audioGraph.UnrecoverableErrorOccurred -= _audioGraph_UnrecoverableErrorOccurred;
        _audioGraph.Dispose( );
        _audioGraph = null;
      }
      _frameInputNode = null; // disposed when the graph is disposed

      // Create an AudioGraph
      AudioGraphSettings settings = new AudioGraphSettings( _renderCat ) {
        PrimaryRenderDevice = _outputDevice,  // If PrimaryRenderDevice is null, the default playback device will be used.
        MaxPlaybackSpeedFactor = 1, // should preserve some memory
        // set Mono Audio (default)
        EncodingProperties = new AudioEncodingProperties( ) { ChannelCount = 1, SampleRate = 44100, BitsPerSample = 32, Bitrate = 1 * 44100 * 32, Subtype = "float" },
      };
      // We await here the execution without providing an async method ...
      var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
      resultAG.Wait( );
      if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
        LOG.LogError( "InitAudioGraph", $"Failed to create AudioGraph with RenderCategory: {_renderCat}" );
        LOG.LogError( "InitAudioGraph", $"AudioGraph creation: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
                        + $"\nExtError: {resultAG.Result.ExtendedError}" );
        _canPlay = false;
        return; // ERROR EXIT
      }
      _audioGraph = resultAG.Result.Graph;
      LOG.Log( "InitAudioGraph", $"AudioGraph: [{_audioGraph.EncodingProperties}]" );
      _audioGraph.UnrecoverableErrorOccurred += _audioGraph_UnrecoverableErrorOccurred;

      // Create a device output node
      // The output node uses the PrimaryRenderDevice of the audio graph.
      // We await here the execution without providing an async method ...
      var resultDO = WindowsRuntimeSystemExtensions.AsTask( _audioGraph.CreateDeviceOutputNodeAsync( ) );
      resultDO.Wait( );
      if (resultDO.Result.Status != AudioDeviceNodeCreationStatus.Success) {
        // Cannot create device output node
        LOG.LogError( "InitAudioGraph", $"DeviceOutputNode creation: {resultDO.Result.Status}, TaskStatus: {resultDO.Status}"
                        + $"\nExtError: {resultDO.Result.ExtendedError}" );
        _canPlay = false;
        return; // ERROR EXIT
      }
      _deviceOutputNode = resultDO.Result.DeviceOutputNode;
      _canPlay = true; // finally

      if (CreateInputNode( )) {
        _frameInputNode.Start( );
        _audioGraph.Start( );
      }

      // log outcome
      var devName = (_deviceOutputNode.Device == null) ? "Standard Output Device" : _deviceOutputNode.Device.Name;
      LOG.Log( "InitAudioGraph", $"DeviceOutputNode: [{devName}]" );
      LOG.Log( "InitAudioGraph", $"InitAudioGraph-END" );
    }

    private void _audioGraph_UnrecoverableErrorOccurred( AudioGraph sender, AudioGraphUnrecoverableErrorOccurredEventArgs args )
    {
      LOG.LogError( "_audioGraph_UnrecoverableErrorOccurred", $"{args.Error}" );
      Console.WriteLine( args.Error );
    }

    // create the Audio Input Node
    private bool CreateInputNode( )
    {
      // Create the FrameInputNode at the same format as the graph
      AudioEncodingProperties nodeEncodingProperties = _audioGraph.EncodingProperties;
      _frameInputNode = _audioGraph.CreateFrameInputNode( nodeEncodingProperties );
      if (_frameInputNode == null) {
        LOG.LogError( "CreateInputNode", $"CreateFrameInputNode returned null" );
        //        await EndOfSound( );
        return false;
      }
      LOG.Log( "CreateInputNode", $"FrameInputNode: CREATED, emitter is {_frameInputNode.Emitter}" );
      // Initialize the Frame Input Node in the stopped state
      _frameInputNode.Stop( );
      // Hook up an event handler so we can start generating samples when needed
      // This event is triggered when the node is required to provide data
      _frameInputNode.QuantumStarted += _frameInputNode_QuantumStarted;
      _frameInputNode.AudioFrameCompleted += _frameInputNode_AudioFrameCompleted;
      _frameInputNode.AddOutgoingConnection( _deviceOutputNode );
      return true;
    }

    // called to supply a number of samples (Quantum)
    private void _frameInputNode_QuantumStarted( AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args )
    {
      // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
      // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
      // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
      uint numSamplesNeeded = (uint)args.RequiredSamples;

      if (numSamplesNeeded > 0) {
        AudioFrame audioData = GenerateAudioData( numSamplesNeeded );
        _frameInputNode.AddFrame( audioData );
      }
    }

    // will be called when an AudioFrame was rendered
    private void _frameInputNode_AudioFrameCompleted( AudioFrameInputNode sender, AudioFrameCompletedEventArgs args )
    {
      // may be not needed for any purpose ...
    }

    // from MS Documentation
    // create the Audio Samples for the next Quantum
    unsafe private AudioFrame GenerateAudioData( uint samples )
    {
      // Buffer size is (number of samples) * (size of each sample)
      // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
      uint bufferSize = samples * sizeof( float );
      AudioFrame frame = new AudioFrame( bufferSize );

      using (AudioBuffer buffer = frame.LockBuffer( AudioBufferAccessMode.Write ))
      using (IMemoryBufferReference reference = buffer.CreateReference( )) {
        byte* dataInBytes;
        uint capacityInBytes;
        float* dataInFloat;

        // Get the buffer from the AudioFrame
        ((IMemoryBufferByteAccess)reference).GetBuffer( out dataInBytes, out capacityInBytes );

        // Cast to float since the data we are generating is float
        dataInFloat = (float*)dataInBytes;

        int sampleRate = (int)_audioGraph.EncodingProperties.SampleRate;
        for (int i = 0; i < samples; i++) {
          double sample = _sWave.WaveSample( sampleRate ); ;
          dataInFloat[i] = (float)sample;
        }
      }
      return frame;
    }


    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public SynthProc( SynthWave synthWave )
    {
      // Init - will be flaged during init if something goes wrong
      _canPlay = false; // not yet
      _sWave = synthWave;
      /* AudioGraph*/
      // find a render cat (see notes above)
      FindRenderCategory( );
      InitAudioGraph( );
    }

    /// <summary>
    /// Selects a specific output device by name.
    /// </summary>
    /// <param name="displayName">The name of the output device to select</param>
    public void SelectOutputDevice( string displayName )
    {
      LOG.Log( "SelectOutputDevice", $"Is {displayName}" );
      _canPlay = false; // not yet

      // select the one requested
      var oDevs = GetInstalledOutputDevices( );
      _outputDevice = oDevs.FirstOrDefault( x => x.Name == displayName ); // null will select the defaultOutput
                                                                          // find a render cat (see notes above)
      FindRenderCategory( );
      InitAudioGraph( ); // restart the Graph
    }

    /// <summary>
    /// Reset the speakers concurrency check
    ///  don't know if the end event is fired in any case - this is to recover
    /// </summary>
    public void Reset( )
    {
    }

    /// <summary>
    /// Mute or Unmute the player
    /// </summary>
    /// <param name="muted"></param>
    public void Mute( bool muted )
    {
      if (!_canPlay) return; // Cannot

      if (_deviceOutputNode == null) {
        LOG.LogError( "Mute", $"Cannot Mute, DeviceOutput was null ??" );
        return;
      }
      _deviceOutputNode.ConsumeInput = !muted;
    }

    // To populate an AudioFrame with audio data, you must get access to the underlying memory buffer of the audio frame.
    // To do this you must initialize the IMemoryBufferByteAccess COM interface by adding the following code within your namespace.
    [ComImport]
    [Guid( "5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    unsafe interface IMemoryBufferByteAccess
    {
      void GetBuffer( out byte* buffer, out uint capacity );
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
          _audioGraph?.Stop( );

          _frameInputNode?.Dispose( );
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
