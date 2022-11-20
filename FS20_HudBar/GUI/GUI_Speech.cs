using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeechLib; // instead of the above .Net lib (provides all installed voices)

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// A Class supporting Speach Output
  /// </summary>
  class GUI_Speech
  {

    #region STATIC 

    private static List<string> m_voiceNames = new List<string>( );
    private static List<string> m_outputDeviceNames = new List<string>( );

    /// <summary>
    /// static cTor
    ///   get voices etc resources only once for the application
    /// </summary>
    static GUI_Speech( )
    {
      // collect the available voices, add Disabled as first item
      var voices = Speech.InstalledVoices.ToList( );
      m_voiceNames.Add( "Voice out disabled" );
      foreach (var v in voices) {
        if (v.Enabled)
          m_voiceNames.Add( v.VoiceInfo.Name );
      }
      // collect the available output devices, add Standard Output as first item
      var oDevices = Speech.InstalledOutputDevices.ToList( );
      m_outputDeviceNames.Add( "Standard Output Device" );
      foreach (var oDevice in oDevices) {
        m_outputDeviceNames.Add( oDevice.Name );
      }
    }

    /// <summary>
    /// Returns a list of available Voice Names
    /// </summary>
    public static IList<string> AvailableVoices => m_voiceNames;

    /// <summary>
    /// Returns a list of available Output Devices
    /// </summary>
    public static IList<string> AvailableOutputDevices => m_outputDeviceNames;

    #endregion // STATIC

    // ******** CLASS 

    private Speech m_synthesizer = new Speech( );

    // true if the pack is available - i.e. a Voice is set and supported
    private bool m_available = false;
    // true if Enabled 
    private bool m_enabled = false;

    /// <summary>
    /// cTor:  Init with a valid VoiceName from the List
    /// </summary>
    /// <param name="voiceName">The Voicename</param>
    public GUI_Speech( )
    {
      m_available = false;
      m_enabled = false;
    }

    /// <summary>
    /// Enable or disable the Voice Out
    ///  Default DISABLED
    /// </summary>
    public bool Enabled {
      get => m_enabled;
      set => m_enabled = value;
    }

    /// <summary>
    /// Set the Voice for Speach output
    /// </summary>
    /// <param name="displayName">The VoiceName</param>
    /// <returns>True if successfull</returns>
    public bool SetVoice( string displayName )
    {
      // reset
      m_available = false;

      // get the voice
      if (displayName == m_voiceNames[0]) {
        // Voice disabled
        m_available = false;
      }
      else if (m_voiceNames.Contains( displayName )) {
        // this shall never fail
        try {
          m_synthesizer.SelectVoice( displayName );
          m_available = true;
        }
        catch {
#if DEBUG
          throw new ArgumentOutOfRangeException( $"Cannot Select Voice {displayName}" );
#endif
        }
      }
      return m_available;
    }


    /// <summary>
    /// Set the OutputDevice for Speach output
    /// </summary>
    /// <param name="displayName">The DeviceName</param>
    /// <returns>True if successfull</returns>
    public void SetOutputDevice( string displayName )
    {
      if (m_outputDeviceNames.Contains( displayName )) {
        // this shall never fail
        try {
          m_synthesizer.SelectOutputDevice( displayName );
        }
        catch {
          m_synthesizer.SelectOutputDevice( "" ); // trigger the default device
#if DEBUG
          throw new ArgumentOutOfRangeException( $"Cannot Select Device {displayName}" );
#endif
        }
      }
    }


    /// <summary>
    /// Speaks a Number but only returns after end of speach
    /// Avoid concurrency issues in Config
    /// </summary>
    /// <param name="number">The number to speak</param>
    public void SaySynched( int number )
    {
      if (m_available && m_enabled) m_synthesizer.Speak( number.ToString( ) );
    }

    /// <summary>
    /// Speaks a Number
    /// </summary>
    /// <param name="number">The number to speak</param>
    public void Say( int number )
    {
      if (m_available && m_enabled) m_synthesizer.SpeakAsync( number.ToString( ) );
    }

    /// <summary>
    /// Speaks a Text
    /// </summary>
    /// <param name="text">The text to speak</param>
    public void Say( string text )
    {
      if (m_available && m_enabled) m_synthesizer.SpeakAsync( text );
    }

  }
}
