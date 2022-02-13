using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechLib
{
  /// <summary>
  /// Implements an Background thread which does the voice output
  /// so the calling entity is able to proceed with working
  ///  Words ( < 10sec chunks assumed..) will be queued and serialized voice output is generated
  ///  - Provides a list of installed voices
  ///  - Accepts to set the Voice from the Installed ones
  ///  - InitSpeaker() before start adding Words
  /// </summary>
  internal class SpeechWorker : BackgroundWorker, IDisposable
  {
    private const int _maxWordLength = 200;  // allow a maximum of 200 chars per Word added
    private const int c_timeout = 1000; // timout to check for cancellation
    private const int s_timeout = 10_000; // timout of speech

    private AutoResetEvent _workerWaitHandle;
    private AutoResetEvent _speakerWaitHandle;
    private int progressCount = 0;

    // The queue to speak
    private Queue<string> _words = new Queue<string>();

    private VoiceSynth _speaker;


    /// <summary>
    /// cTor: 
    /// </summary>
    public SpeechWorker( )
    {
      _workerWaitHandle = new AutoResetEvent( false );
      _speakerWaitHandle = new AutoResetEvent( false );
      _speaker = new VoiceSynth(_speakerWaitHandle);

      this.WorkerSupportsCancellation = true;
      this.WorkerReportsProgress = true;

      this.DoWork += SpeechWorker_DoWork;
    }

    
    /// <summary>
    /// Start the Speaker processing
    /// </summary>
    /// <param name="parameter">A parameter obj to use</param>
    public void InitSpeaker( object parameter )
    {
      if ( !this.IsBusy ) {
        // restart
        _speaker?.Dispose( );
        _workerWaitHandle.Dispose( );
        _speakerWaitHandle.Dispose( );

        _workerWaitHandle = new AutoResetEvent( false );
        _speakerWaitHandle = new AutoResetEvent( false );
        _speaker = new VoiceSynth( _speakerWaitHandle );

        ClearWords( );
        progressCount = 0;
        this.RunWorkerAsync( parameter );
      }
      else {
        throw new InvalidOperationException( "Cannot InitSpeaker while Busy. Must Cancel first!" );
      }
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
    /// Add a word to speak (can be more than word but don't get terse here)
    /// </summary>
    /// <param name="text">The string to speak (Max 200 chars supported)</param>
    public void AddWord( string text )
    {
      if ( !this.IsBusy ) {
        throw new InvalidOperationException( "Cannot add Words when not initialized" );
      }
      if ( text.Length> _maxWordLength ) {
        throw new InvalidOperationException( $"Text is too long: {text.Length} chars ({_maxWordLength} chars max supported)" );
      }

      lock ( _words ) {
        _words.Enqueue( text );
        if ( _words.Count == 1 )
          _workerWaitHandle.Set( );
      }
    }

    /// <summary>
    /// Clear the current Word queue
    /// </summary>
    public void ClearWords( )
    {
      lock ( _words ) {
        _words.Clear( );
      }
    }



    /// <summary>
    /// Processes the word queue one by one
    ///   the BGWorker Main loop
    /// </summary>
    private void SpeechWorker_DoWork( object sender, DoWorkEventArgs e )
    {
      bool doWork = !this.CancellationPending;
      string word = "";

      // Loop
      while ( true ) {
        if ( _workerWaitHandle.WaitOne( c_timeout ) ) {
          // signalled
          lock ( _words ) {
            if ( _words.Count > 0 ) {
              word = _words.Dequeue( );
            }
          }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          _speakerWaitHandle.Reset( );
          _speaker.SpeakAsync( word );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

          // wait until spoken
          if ( _speakerWaitHandle.WaitOne( s_timeout ) ) {
            // report a word said
            this.ReportProgress( ++progressCount );
          }
          else {
            // run into a speech time out - either the word was tool long or something else prevented
            // the Speaker to signal end of speech
            // anyway try to recover and continue
            _speaker.Reset( );
          }
        }

        // check for cancellation requested
        doWork &= ( !this.CancellationPending );
        if ( !doWork ) break;

        // retrigger of there are words left
        if ( _words.Count > 0 )
          _workerWaitHandle.Set( ); 
      }

      // finishing here
      _words.Clear( );
      _workerWaitHandle.Reset( );
      _speakerWaitHandle.Reset( );
      _speaker?.Dispose( );
      _speaker = null;
      // requires a new Init to proceed with Speaking
    }


  }
}
