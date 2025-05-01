namespace SpeechLib
{
  /// <summary>
  /// Descriptor of an installed voice
  ///  - derived from .Net Speech Library
  /// </summary>
  public class InstalledVoice
  {
    /// <summary>
    /// Get; Set;  Voice is enabled
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// Get; the VoiceInfo Item
    /// </summary>
    public VoiceInfo VoiceInfo { get; }

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="voiceInfo">The VoiceInfo item</param>
    public InstalledVoice(VoiceInfo voiceInfo )
    {
      VoiceInfo = voiceInfo;
    }

    /// <summary>
    /// String representation of this object
    /// </summary>
    public override string ToString( ) => VoiceInfo.ToString( );
  }


}

