
namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Outside Interface for LevelDetector
  /// </summary>
  internal interface ILevelDetectorAPI
  {
    /// <summary>
    /// Wether the trigger resets when outside of the band
    /// </summary>
    bool AutoReset { get; set; }

    /// <summary>
    /// Set the trigger memory
    /// </summary>
    void SetTrigger( );

    /// <summary>
    /// Reset the trigger memory
    /// </summary>
    void ResetTrigger( );

  }
}
