﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using System.IO;

using DbgLib;
using Windows.Devices.Enumeration;

namespace PingLib
{
  /// <summary>
  /// Implements the AudioGraph using Sound Files provided by the Library (Resources)
  /// 
  /// </summary>
  internal class WaveProc : IDisposable
  {

    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // flag to not deploy multiple times
    private static bool _soundsDeployed = false;

    // files we streamed to play - such files are put into the users Temp folder and remain there until someone cleans them up..
    private static List<SoundInfo> _installedSounds = new List<SoundInfo>( );
    private static List<string> _streamedSoundFiles = new List<string>( );
    private static List<DeviceInformation> _installedOutputDevices = new List<DeviceInformation>( );

    // Define Library Resources
    private static void LoadAvailableSounds( )
    {
      _installedSounds.Clear( );
      // add known Sounds manually (ID is the Property Name)
      // Usually 61 Tones where 0=Silence, C6=1,.. 5 Octaves to Tone 60
      _installedSounds.Add( new SoundInfo( "Silence", "One Second of Silence", "Silence_VBR50Q48kHz", Melody.Silence, SoundType.wma, 1, 1f, 61 ) ); // 1 sec Silence per Tone
      //  _installedSounds.Add( new SoundInfo( "NylonPic_1", "Nylon String Pic 1", "Nylon1_VBR50Q48kHz", Melody.Nylon_1, SoundType.wma, 1, 0.6f, 61 ) ); // Tone 0 = Silence
      //  _installedSounds.Add( new SoundInfo( "NylonPic_2", "Nylon String Pic 2", "Nylon2_VBR50Q48kHz", Melody.Nylon_2, SoundType.wma, 1, 0.35f, 61 ) ); // Tone 0 = Silence
      //  _installedSounds.Add( new SoundInfo( "Synth_1", "Syth Tone 1", "Synth1_VBR50Q48kHz", Melody.Synth_1, SoundType.wma, 1, 0.3f, 61 ) );            // Tone 0 = Silence
      //  _installedSounds.Add( new SoundInfo( "Synth_2", "Syth Tone 2", "Synth2_VBR50Q48kHz", Melody.Synth_2, SoundType.wma, 1, 0.25f, 61 ) );           // Tone 0 = Silence
      //  _installedSounds.Add( new SoundInfo( "Synth_3", "Syth Tone 3 Smooth In", "Synth3_VBR50Q48kHz", Melody.Synth_3, SoundType.wma, 1, 0.3f, 61 ) );  // Tone 0 = Silence
      //  _installedSounds.Add( new SoundInfo( "Woodblocks_1", "Woodblock Sound 1", "Wood1_VBR50Q48kHz", Melody.Wood_1, SoundType.wma, 1, 0.4f, 61 ) );   // Tone 0 = Silence
      _installedSounds.Add( new SoundInfo( "Bell_1", "Bell Sound 1", "Bell1_VBR50Q48kHz", Melody.Bell_1, SoundType.wma, 3, 2.95f, 61 ) );             // Tone 0 = Silence
      //_installedSounds.Add( new SoundInfo( "TSynth", "Triangle  Wave", "TSynth_VBR50Q48kHz", Melody.TSynth, SoundType.wma, 0.45f, 0.3f, 6 ) );         // Tone 0 = Silence
      //_installedSounds.Add( new SoundInfo( "TSynth_2", "Triangle 2  Wave", "TSynth2_VBR50Q48kHz", Melody.TSynth2, SoundType.wma, 2.0f, 1.9f, 4 ) );   // Tone 0 = Silence, 2sec 
      _installedSounds.Add( new SoundInfo( "TSynth_2", "Clarinet Wave", "TSynth3_VBR50Q48kHz", Melody.TSynth3, SoundType.wma, 2.0f, 1.9f, 5 ) );   // Tone 0 = Silence, 2sec 

      DeploySoundFiles( );
    }

    // deploy sound files into the designated folder to pick them up later for streaming
    private static bool DeploySoundFiles( )
    {
      if (_soundsDeployed) return true; // already done

      foreach (var soundInfo in _installedSounds) {
        try {
          using (var outStream = File.Create( SoundFileFrom( soundInfo ), 4096 )) {
            var res = Properties.Resources.ResourceManager.GetObject( soundInfo.Id );
            if (res is byte[]) {
              using (var streamWriter = new BinaryWriter( outStream )) {
                streamWriter.Write( (byte[])Properties.Resources.ResourceManager.GetObject( soundInfo.Id ) );
              }
            }
            else {
              using (var inStream = Properties.Resources.ResourceManager.GetStream( soundInfo.Id )) {
                inStream.CopyTo( outStream );
              }
            }
          }
        }
        catch (Exception ex) {
          // could not write soundfile - log and ignore
          LOG.Error( "DeploySoundFiles", ex, $"Deploy failed for sound <{soundInfo.Id}>" );
        }
      }
      // finishing
      _soundsDeployed = true;
      return true;
    }


    // returns a soundfile path and name from SoundInfo
    private static string SoundFileFrom( SoundInfo soundInfo ) => Path.Combine( SoundFolder( ), $"{soundInfo.Id}.{soundInfo.SType}" );

    // returns the (hudbar temp) folder name for sounds
    // use only this method and not the variable !!
    private static string SoundFolder( )
    {
      if (string.IsNullOrEmpty( _soundFolder )) {
        // initialize if needed
        _soundFolder = bm98_hbFolders.Folders.UserTempPath;
      }
      return _soundFolder;
    }
    private static string _soundFolder = "";


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
        LOG.Error( "LoadInstalledOutputDevices", $"Status {ret.Status}" );
      }
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

    /// <summary>
    /// Deletes the temp sound files deployed by this module (Streamed files land in the users Temp folder)
    /// 
    /// TODO no longer needed when deploying sounds to the HudBar TempLocation - ev. remove this
    /// 
    /// </summary>
    public static void RemoveTempSounds( )
    {
      var usrTemp = Path.GetTempPath( );
      // list of filenames without extension
      foreach (var sFileName in _streamedSoundFiles) {
        var sFiles = Directory.EnumerateFiles( usrTemp, sFileName + "*.wma" ); // get all (there might be FILE (n).wma)
        foreach (var sFile in sFiles) {
          // never fail the Delete
          try {
            File.Delete( sFile );
          }
          catch { }
        }
      }
      _streamedSoundFiles.Clear( );
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
    private AudioFileInputNode _fileInputNode = null;
    private SoundInfo _sound = null;
    private SoundInfo _soundInUse = null;

    // AudioGraph
    private AudioGraph _audioGraph = null;
    private AudioDeviceOutputNode _deviceOutputNode = null;
    private DeviceInformation _outputDevice = null;

    private AutoResetEvent _playingWaitHandle = null; // will report the end of play

    private bool _playing = false; // true while playing

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
        LOG.Info( "FindRenderCategory", $"About to test AudioGraph with RenderCategory: {_renderCat}" );
        // We await here the execution without providing an async method ...
        var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
        resultAG.Wait( );
        if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
          LOG.Error( "FindRenderCategory", $"AudioGraph test error: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
            + $"\nExtError: {resultAG.Result.ExtendedError}" );

          // try next category if there is one left
          if (renderSequence.Count > 0) {
            _renderCat = renderSequence.Dequeue( );
          }
          else {
            // sanity - should never happen
            LOG.Error( "FindRenderCategory", $"Program error - Queue overrun" );
            _renderCat = _renderNone;
            return;
          }
        }
        else {
          resultAG.Result.Graph?.Dispose( ); // not used after tryout
          LOG.Info( "FindRenderCategory", $"Success with RenderCategory: {_renderCat}" );
          return; // _renderCat contains a successful one
        }
      } while (_renderCat != _renderNone);

      LOG.Error( "FindRenderCategory", $"Failed to find a working RenderCategory - cannot speak" );
      _canPlay = false;
      return; // could not resolve - left with _renderNone
    }

    #endregion

    // Init the AudioGraph
    //  despite the Aync methods - this will exec synchronously to get the InitPhase  only get done when all is available 
    private void InitAudioGraph( )
    {
      LOG.Info( "InitAudioGraph", "Begin" );
      // MUST WAIT UNTIL all items are created, else one may call Play too early...
      // cleanup existing items
      if (_deviceOutputNode != null) { _deviceOutputNode.Dispose( ); _deviceOutputNode = null; }
      if (_audioGraph != null) {
        _audioGraph.UnrecoverableErrorOccurred -= _audioGraph_UnrecoverableErrorOccurred;
        _audioGraph.Dispose( );
        _audioGraph = null;
      }
      _fileInputNode = null; // disposed when the graph is disposed
      _soundInUse = null;

      // Create an AudioGraph
      AudioGraphSettings settings = new AudioGraphSettings( _renderCat ) {
        PrimaryRenderDevice = _outputDevice,  // If PrimaryRenderDevice is null, the default playback device will be used.
        MaxPlaybackSpeedFactor = 2, // should preserve some memory
      };
      // We await here the execution without providing an async method ...
      var resultAG = WindowsRuntimeSystemExtensions.AsTask( AudioGraph.CreateAsync( settings ) );
      resultAG.Wait( );
      if (resultAG.Result.Status != AudioGraphCreationStatus.Success) {
        LOG.Error( "InitAudioGraph", $"Failed to create AudioGraph with RenderCategory: {_renderCat}" );
        LOG.Error( "InitAudioGraph", $"AudioGraph creation: {resultAG.Result.Status}, TaskStatus: {resultAG.Status}"
                        + $"\nExtError: {resultAG.Result.ExtendedError}" );
        _canPlay = false;
        return; // ERROR EXIT
      }
      _audioGraph = resultAG.Result.Graph;
      LOG.Info( "InitAudioGraph", $"AudioGraph: [{_audioGraph.EncodingProperties}]" );
      _audioGraph.UnrecoverableErrorOccurred += _audioGraph_UnrecoverableErrorOccurred;

      // Create a device output node
      // The output node uses the PrimaryRenderDevice of the audio graph.
      // We await here the execution without providing an async method ...
      var resultDO = WindowsRuntimeSystemExtensions.AsTask( _audioGraph.CreateDeviceOutputNodeAsync( ) );
      resultDO.Wait( );
      if (resultDO.Result.Status != AudioDeviceNodeCreationStatus.Success) {
        // Cannot create device output node
        LOG.Error( "InitAudioGraph", $"DeviceOutputNode creation: {resultDO.Result.Status}, TaskStatus: {resultDO.Status}"
                        + $"\nExtError: {resultDO.Result.ExtendedError}" );
        _canPlay = false;
        return; // ERROR EXIT
      }
      _deviceOutputNode = resultDO.Result.DeviceOutputNode;
      _canPlay = true; // finally
      // log outcome
      var devName = (_deviceOutputNode.Device == null) ? "Standard Output Device" : _deviceOutputNode.Device.Name;
      LOG.Info( "InitAudioGraph", $"DeviceOutputNode: [{devName}]" );
      LOG.Info( "InitAudioGraph", $"InitAudioGraph-END" );
    }

    private void _audioGraph_UnrecoverableErrorOccurred( AudioGraph sender, AudioGraphUnrecoverableErrorOccurredEventArgs args )
    {
      LOG.Error( "_audioGraph_UnrecoverableErrorOccurred", $"Error: {args.Error}" );
      Console.WriteLine( args.Error );
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

    // TODO Using deployed files to read for streaming - ev. remove this utility for dynamic streaming
    private async void StreamedFileWriter( StreamedFileDataRequest request )
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      try {
        using (var outStream = request.AsStreamForWrite( )) {
          var res = Properties.Resources.ResourceManager.GetObject( _sound.Id );
          if (res is byte[]) {
            using (var streamWriter = new BinaryWriter( outStream )) {
              streamWriter.Write( (byte[])Properties.Resources.ResourceManager.GetObject( _sound.Id ) );
            }
          }
          else {
            using (var inStream = Properties.Resources.ResourceManager.GetStream( _sound.Id )) {
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


    // Async Play a melody from a file (wma)
    private async Task PlayAsyncLow( SoundBite soundBite )
    {
      _playing = true; // locks additional calls for Play until finished playing this bit

      if (!_canPlay || _audioGraph == null || _deviceOutputNode == null) {
        LOG.Error( "PlayAsyncLow", $"Some items do not exist: cannot play..\n [{_audioGraph}] [{_deviceOutputNode}]" );
        await EndOfSound( );
        return;
      }

      // don't reload if the sound is already in use
      if (soundBite.Melody != _soundInUse?.Melody) {
        LOG.Info( "PlayAsyncLow", $"RELOAD soundbite: {soundBite.Melody}" );
        // if a prev. Node exists, remove it
        if (_fileInputNode != null) {
          _audioGraph.Stop( );

          _fileInputNode.Stop( );
          _fileInputNode.FileCompleted -= _fileInputNode_FileCompleted;
          _fileInputNode.RemoveOutgoingConnection( _deviceOutputNode );
          _fileInputNode.Dispose( );
          _fileInputNode = null;
        }
        // set new sound
        _sound = _installedSounds.Where( x => x.Melody == soundBite.Melody ).FirstOrDefault( );
        if (_sound == null) {
          LOG.Error( "PlayAsyncLow", $"Melody has no Audiofile: {soundBite.Melody} - cannot play" );
          await EndOfSound( );
          return;
        }
        //TODO ev. Remove this
        // StorageFile file = await StorageFile.CreateStreamedFileAsync( $"{_sound.Id}.{_sound.SType}", StreamedFileWriter, null ); // using temp files

        // using deployed files for streaming
        StorageFile file = await StorageFile.GetFileFromPathAsync( SoundFileFrom( _sound ) );
        _streamedSoundFiles.Add( file.DisplayName ); // add to clean up list
        // create the InputNode
        var resultAF = await _audioGraph.CreateFileInputNodeAsync( file );
        if (resultAF.Status != AudioFileNodeCreationStatus.Success) {
          LOG.Error( "PlayAsyncLow", $"AudioFileNodeCreationStatus creation: {resultAF.Status}"
                                  + $"\nExtError: {resultAF.ExtendedError}" );
          await EndOfSound( );
          return;
        }
        LOG.Info( "PlayAsyncLow", $"FileInputNode: CREATED, srcFile is {resultAF.FileInputNode.SourceFile?.Name}" );
        _fileInputNode = resultAF.FileInputNode;
        _fileInputNode.FileCompleted += _fileInputNode_FileCompleted;
        _fileInputNode.AddOutgoingConnection( _deviceOutputNode );

        _audioGraph.Start( );

        _soundInUse = _sound.AsCopy( );
      }

      // we capture problems through Exceptions here - the settings and restrictions seem not complete in the Docs
      try {
        // Play it
        // LOG.Log( $"Play {_fileInputNode.ConsumeInput}" ); // extensive logging when enabled (play silence...)

        // cannot start after prev end - so set it null and seek to start of file
        _fileInputNode.StartTime = null;
        _fileInputNode.EndTime = null; // cannot start after prev end - so set it null
        _fileInputNode.Seek( new TimeSpan( 0 ) ); // have to seek to Start, we cannot assign a StartTime before the current Position
                                                  // only now we can set any new start and end... (that is not in the documents...)
        _fileInputNode.StartTime = TimeSpan.FromSeconds( soundBite.Tone * _soundInUse.ToneStep_sec );
        _fileInputNode.EndTime = _fileInputNode.StartTime + TimeSpan.FromSeconds( (soundBite.Duration < 0) ? _soundInUse.ToneDuration_sec : soundBite.Duration );
        _fileInputNode.OutgoingGain = soundBite.Volume;
        _fileInputNode.LoopCount = (int)soundBite.Loops; // counts down in the Completed Callback - track completeness there (not in docu...)
        _fileInputNode.PlaybackSpeedFactor = soundBite.SpeedFact;
        // Plays in the current Thread - cannot be waited for in the same thread
        _fileInputNode.Start( );

        //_audioGraph.Start( );
      }
      catch (Exception ex) {
        LOG.Error( "PlayAsyncLow", ex, "Sample Setup caused an Exception" );
        await EndOfSound( );
      }
    }

    // will be called when sound has finished
    private async void _fileInputNode_FileCompleted( AudioFileInputNode sender, object args )
    {
      if (_fileInputNode == null) {
        LOG.Info( "_fileInputNode_FileCompleted", "_fileInputNode was already disposed (==null)" );
        return;
      }

      try {
        // track the loop count down and wait until the end -murks MS will get here twice with 0  when the loopCount is used
        if (_fileInputNode.LoopCount > 0) return;
        _fileInputNode.Stop( );
        await EndOfSound( );
      }
      catch (Exception ex) {
        _fileInputNode = null;
        LOG.Error( "_fileInputNode_FileCompleted", ex, "caused an Exception" );
      }
    }


    /// <summary>
    /// cTor: Init facility
    /// </summary>
    public WaveProc( AutoResetEvent resetEvent, bool loolPlayer )
    {
      // Init - will be flaged during init if something goes wrong
      _canPlay = false; // not yet

      LoadAvailableSounds( );

      _playingWaitHandle = resetEvent;
      _playingWaitHandle.Reset( );

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
      LOG.Info( "SelectOutputDevice", $"Is {displayName}" );
      _canPlay = false; // not yet

      // select the one requested
      var oDevs = GetInstalledOutputDevices( );
      _outputDevice = oDevs.FirstOrDefault( x => x.Name == displayName ); // null will select the defaultOutput
      // find a render cat (see notes above)
      FindRenderCategory( );
      InitAudioGraph( ); // restart the Graph
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
      if (!_canPlay) return; // Cannot
      if (_playing) return; // no concurrent playing

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
      if (!_canPlay) return; // Cannot

      if (_deviceOutputNode == null) {
        LOG.Error( "Mute", "Cannot Mute, _deviceOutputNode already disposed (==null)" );
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
      if (!disposedValue) {
        if (disposing) {
          _canPlay = false;
          // dispose managed state (managed objects)
          // cleanup existing items
          _fileInputNode?.Dispose( );
          _fileInputNode = null;
          _deviceOutputNode?.Dispose( );
          _deviceOutputNode = null;
          _audioGraph?.Dispose( );
          _audioGraph = null;
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
