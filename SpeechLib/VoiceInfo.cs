namespace SpeechLib
{
  /// <summary>
  ///    Defines the values for the gender of a synthesized voice.
  ///  - derived from .Net Speech Library
  /// </summary>
  public enum VoiceGender
  {
    /// <summary>
    /// A male voice
    /// </summary>
    Male = 0,
    /// <summary>
    /// A female voice
    /// </summary>
    Female = 1,
  }

  /// <summary>
  /// Descriptor of an installed voice
  ///  - derived from .Net Speech Library
  /// </summary>
  public class VoiceInfo
  {
    /// <summary>
    /// Voice Gender
    /// </summary>
    public VoiceGender Gender { get; }
    /// <summary>
    /// Display Name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Language code
    /// </summary>
    public string Language { get; }
    /// <summary>
    /// Voice description
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// Voice ID
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="name">Voice Display Name</param>
    /// <param name="lang">Language Code</param>
    /// <param name="desc">Voice Description</param>
    /// <param name="id">Voice ID</param>
    /// <param name="gender"></param>
    public VoiceInfo( string name, string lang, string desc, string id, VoiceGender gender )
    {
      Gender = gender;
      Name = name;
      Language = lang;
      Description = desc;
      Id = id;
    }

    /// <summary>
    /// String representation of this object
    /// </summary>
    public override string ToString( ) => Description;
  }

}
