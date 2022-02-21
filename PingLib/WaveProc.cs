using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Storage;
using System.IO;

namespace PingLib
{
  /// <summary>
  /// Implements the AudioGraph using Sound Files provided by the Library (Resources)
  /// 
  /// </summary>
  internal class WaveProc : IDisposable
  {

    #region STATIC

    private static List<SoundInfo> _installedSounds = new List<SoundInfo>();

    // Define Library Resources
    private static void LoadAvailableSounds( )
    {
      _installedSounds.Clear( );
      // add known Sounds manually (ID is the Property Name)
      // Usually 61 Tones where 0=Silence, C6=1,.. 5 Octaves to Tone 60
      _installedSounds.Add( new SoundInfo( "Silence", "One Second of Silence", "Silence_VBR50Q48kHz", Melody.Silence, SoundType.wma, 1, 1f, 61 ) ); // 1 sec Silence per Tone
      _installedSounds.Add( new SoundInfo( "NylonPic_1", "Nylon String Pic 1", "Nylon1_VBR50Q48kHz", Melody.Nylon_1, SoundType.wma, 1, 0.6f, 61 ) ); // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "NylonPic_2", "Nylon String Pic 2", "Nylon2_VBR50Q48kHz", Melody.Nylon_2, SoundType.wma, 1, 0.35f, 61 ) ); // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Synth_1", "Syth Tone 1", "Synth1_VBR50Q48kHz", Melody.Synth_1, SoundType.wma, 1, 0.3f, 61 ) );            // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Synth_2", "Syth Tone 2", "Synth2_VBR50Q48kHz", Melody.Synth_2, SoundType.wma, 1, 0.25f, 61 ) );           // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Synth_3", "Syth Tone 3 Smooth In", "Synth3_VBR50Q48kHz", Melody.Synth_3, SoundType.wma, 1, 0.3f, 61 ) );  // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Woodblocks_1", "Woodblock Sound 1", "Wood1_VBR50Q48kHz", Melody.Wood_1, SoundType.wma, 1, 0.4f, 61 ) );   // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Bell_1", "Bell Sound 1", "Bell1_VBR50Q48kHz", Melody.Bell_1, SoundType.wma, 3, 2.95f, 61 ) );             // Tone 0 = Silence
    }

    /// <summary>
    ///  Returns all of the installed sounds.
    /// </summary>
    /// <returns>Returns a read-only collection of the sounds currently available from the library</returns>
    public static IReadOnlyCollection<SoundInfo> GetInstalledSounds( )
    {
      LoadAvailableSounds( ); // reload (for now one cannot add own Sound Files)
      return _installedSounds;
    }

    #endregion

    // Wave Source
    private AudioFileInputNode _fileInputNode;
    private SoundInfo _sound;
    private SoundInfo _soundInUse;

    // AudioGraph
    private AudioGraph _audioGraph;
    private AudioDeviceOutputNode _deviceOutputNode;

    private AutoResetEvent _playingWaitHandle; // will report the end of play

    private bool _playing = false; // true while playing

    private bool _canPlay = false; // true when all infrastructure is OK


    // Init the AudioGraph
    //  despite the Aync methods - this will exec synchronously to get the InitPhase  only get done when all is available 
    private void InitAudioGraph( )
    {
      _canPlay = false;
      // MUST WAIT UNTIL all items are created, else one may call Play too early...
      Console.WriteLine( "PingLib-WaveProc: InitAudioGraph" );
      // cleanup existing items
      if ( _deviceOutputNode != null ) { _deviceOutputNode.Dispose( ); _deviceOutputNode = null; }
      if ( _audioGraph != null ) { _audioGraph.Dispose( ); _audioGraph = null; }

      // Create an AudioGraph
      AudioGraphSettings settings = new AudioGraphSettings ( Windows.Media.Render.AudioRenderCategory.SoundEffects) {
        PrimaryRenderDevice = null, // If PrimaryRenderDevice is null, the default playback device will be used.
        MaxPlaybackSpeedFactor = 2, // should preserve some memory
      };
      // We await here the execution without providing an async method ...
      var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync(settings));
      resultAG.Wait( );
      if ( resultAG.Result.Status != AudioGraphCreationStatus.Success ) {
        Console.WriteLine( $"PingLib-WaveProc: ERROR - AudioGraph creation error: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
          + $"\nExtError: {resultAG.Result.ExtendedError}" );
        return;
      }
      _audioGraph = resultAG.Result.Graph;
      Console.WriteLine( $"PingLib-WaveProc: AudioGraph: [{_audioGraph.EncodingProperties}]" );

      // Create a device output node
      // The output node uses the PrimaryRenderDevice of the audio graph.
      // We await here the execution without providing an async method ...
      var resultDO = WindowsRuntimeSystemExtensions.AsTask( _audioGraph.CreateDeviceOutputNodeAsync());
      resultDO.Wait( );
      if ( resultDO.Result.Status != AudioDeviceNodeCreationStatus.Success ) {
        // Cannot create device output node
        Console.WriteLine( $"PingLib-WaveProc: ERROR - DeviceOutputNode creation error: {resultDO.Result.Status}, TaskStatus: {resultDO.Status}"
          + $"\nExtError: {resultDO.Result.ExtendedError}" );
        return;
      }
      _deviceOutputNode = resultDO.Result.DeviceOutputNode;
      Console.WriteLine( $"PingLib-WaveProc: DeviceOutputNode: [{_deviceOutputNode.Device}]" );

      Console.WriteLine( "PingLib-WaveProc: InitAudioGraph-END" );
      _canPlay = true;
    }


    /// <summary>
    /// Common end of a sound call
    /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task EndOfSound( )
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      _playingWaitHandle?.Set( ); // signal end of the Speak phase
      _playing = false;
    }




#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async void StreamedFileWriter( StreamedFileDataRequest request )
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      try {
        using ( var outStream = request.AsStreamForWrite( ) ) {
          var res = Properties.Resources.ResourceManager.GetObject( _sound.Id );
          if ( res is byte[] ) {
            using ( var streamWriter = new BinaryWriter( outStream ) ) {
              streamWriter.Write( (byte[])Properties.Resources.ResourceManager.GetObject( _sound.Id ) );
            }
          }
          else {
            using ( var inStream = Properties.Resources.ResourceManager.GetStream( _sound.Id ) ) {
              await inStream.CopyToAsync( outStream );
            }
          }
        }
        request.Dispose( );
      }
      catch {
        request.FailAndClose( StreamedFileFailureMode.Incomplete );
      }
    }


    // Async Speak output of a text
    private async Task PlayAsyncLow( SoundBite soundBite )
    {
      _playing = true; // locks additional calls for Speak until finished talking this bit

      if (!_canPlay
            || _audioGraph == null || _deviceOutputNode == null ) {
        Console.WriteLine( $"PingLib-WaveProc: ERROR - AudioGraph does not exist: cannot play..\n [{_audioGraph}] [{_deviceOutputNode}]" );
        await EndOfSound( );
        return;
      }

      // don't reload if the sound is already in use
      if ( soundBite.Melody != _soundInUse?.Melody ) {
        // if a prev. Node exists, remove it
        if ( _fileInputNode != null ) {
          _audioGraph.Stop( );

          _fileInputNode.FileCompleted -= _fileInputNode_FileCompleted;
          _fileInputNode.RemoveOutgoingConnection( _deviceOutputNode );
          _fileInputNode.Dispose( );
          _fileInputNode = null;
        }
        // set new sound
        _sound = _installedSounds.Where( x => x.Melody == soundBite.Melody ).FirstOrDefault( );
        if ( _sound == null ) {
          Console.WriteLine( $"PingLib-WaveProc: ERROR - Melody has no Audiofile: {soundBite.Melody} - cannot play" );
          await EndOfSound( );
          return;
        }
        StorageFile file = await StorageFile.CreateStreamedFileAsync($"{_sound.Id}.{_sound.SType}", StreamedFileWriter, null);
        // create the InputNode
        var resultAF = await _audioGraph.CreateFileInputNodeAsync(file);
        if ( resultAF.Status != AudioFileNodeCreationStatus.Success ) {
          Console.WriteLine( $"PingLib-WaveProc: ERROR - AudioFileNodeCreationStatus creation error: {resultAF.Status}" 
            + $"\nExtError: {resultAF.ExtendedError}");
          await EndOfSound( );
          return;
        }
        _fileInputNode = resultAF.FileInputNode;
        _fileInputNode.FileCompleted += _fileInputNode_FileCompleted;
        _fileInputNode.AddOutgoingConnection( _deviceOutputNode );

        _audioGraph.Start( );

        _soundInUse = _sound.AsCopy( );
      }

      // we capture problems through Exceptions here - the settings and restrictions seem not complete in the Docs
      try {
        // Play it
        // cannot start after prev end - so set it null and seek to start of file
        _fileInputNode.StartTime = null;
        _fileInputNode.EndTime = null; // cannot start after prev end - so set it null
        _fileInputNode.Seek( new TimeSpan( 0 ) ); // have to seek to Start, we cannot assign a StartTime before the current Position
                                                  // only now we can set any new start and end... (that is not in the documents...)
        _fileInputNode.StartTime = TimeSpan.FromSeconds( soundBite.Tone * _soundInUse.ToneStep_sec );
        _fileInputNode.EndTime = _fileInputNode.StartTime + TimeSpan.FromSeconds( ( soundBite.Duration < 0 ) ? _soundInUse.ToneDuration_sec : soundBite.Duration );
        _fileInputNode.OutgoingGain = soundBite.Volume;
        _fileInputNode.LoopCount = (int)soundBite.Loops; // counts down in the Completed Callback - track completeness there (not in docu...)
        _fileInputNode.PlaybackSpeedFactor = soundBite.SpeedFact;
        // Plays in the current Thread - cannot be waited for in the same thread
        _fileInputNode.Start( );
        //_audioGraph.Start( );
      }
      catch ( Exception e ) {
        Console.WriteLine( $"PingLib-WaveProc: ERROR - Sample Setup caused an Exception\n{e.Message}" );
        await EndOfSound( );
      }
    }

    // will be called when speaking has finished
    private async void _fileInputNode_FileCompleted( AudioFileInputNode sender, object args )
    {
      if ( _fileInputNode.LoopCount > 0 ) return; // track the loop count down and wait until the end -murks MS will get here twice with 0  when the loopCount is used

      _fileInputNode.Stop( );
      await EndOfSound( );
    }


    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public WaveProc( AutoResetEvent resetEvent, bool loolPlayer )
    {
      _playingWaitHandle = resetEvent;
      _playingWaitHandle.Reset( );

      /* AudioGraph*/
      LoadAvailableSounds( );
      InitAudioGraph( );
    }

    /// <summary>
    /// Asynchronously plays a sound.
    ///  Waiting for this will not wait for when the Play has ended
    ///  but rather when the trigger for Output was given
    ///  There is currently no event available to detect end of Play
    /// </summary>
    /// <param name="soundBite">The SoundBite to play</param>
    public async Task PlayAsync( SoundBite soundBite )
    {
      if ( !_canPlay ) return; // Cannot
      if ( _playing ) return; // no concurrent playing

      await PlayAsyncLow( soundBite );
    }

    /// <summary>
    /// Reset the speakers concurrency check
    ///  don't know if the end event is fired in any case - this is to recover
    /// </summary>
    public void Reset( )
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      EndOfSound( );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Mute or Unmute the player
    /// </summary>
    /// <param name="muted"></param>
    public void Mute( bool muted )
    {
      if ( !_canPlay ) return; // Cannot

      if ( _deviceOutputNode == null ) {
        Console.WriteLine( "PingLib-WaveProc: ERROR - Cannot Mute, DeviceOutput was null ??" );
        return;
      }
      _deviceOutputNode.ConsumeInput = !muted;
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
          _fileInputNode?.Dispose( );
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
