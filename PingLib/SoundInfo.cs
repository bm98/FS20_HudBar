using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingLib
{
  /// <summary>
  ///    Defines the values for the gender of a synthesized voice.
  ///  - derived from .Net Speech Library
  /// </summary>
  public enum Melody
  {
    /// <summary>
    /// One second of Silence..
    /// </summary>
    Silence = 0,
    /*
    /// <summary>
    /// A Nylon String pic  (Tone 0 -> Silence)
    /// </summary>
    Nylon_1,
    /// <summary>
    /// A Nylon String pic  (Tone 0 -> Silence)
    /// </summary>
    Nylon_2,
    /// <summary>
    /// A Short Synthesizer Attack (Tone 0 -> Silence)
    /// </summary>
    Synth_1,
    /// <summary>
    /// A Shorter Synthesizer Attack (Tone 0 -> Silence)
    /// </summary>
    Synth_2,
    /// <summary>
    /// A Shorter Synthesizer Smooth In  (Tone 0 -> Silence)
    /// </summary>
    Synth_3,
    /// <summary>
    /// Woodblock (Tone 0 -> Silence)
    /// </summary>
    Wood_1,
    */
    /// <summary>
    /// Bell - about 3 sec per Tone (Tone 0 -> Silence)
    /// </summary>
    Bell_1,
    /// <summary>
    /// Triangle Wave with glide 0.3 sec segments
    ///  Notes: 
    ///  0: Silence, 
    ///  1: Low
    ///  2: Low to High
    ///  3: High
    ///  4: High to Low
    ///  5: Low 1 oct down
    /// </summary>
    TSynth,
    /// <summary>
    /// Triangle Wave 2 sec segments
    ///  Notes: 
    ///  0: Silence, 
    ///  1: High .35s tone length
    ///  2: Med  .47s tone length
    ///  3: Low  .47s tone length
    /// </summary>
    TSynth2,
    /// <summary>
    /// Triangle Wave 2 sec segments
    ///  Notes: 
    ///  0: Silence, 
    ///  1: High .45s tone length
    ///  2: Med  .75s tone length
    ///  3: Low  .75s tone length
    ///  4: Lower  .75s tone length
    /// </summary>
    TSynth3,
  }

  /// <summary>
  /// Type of Soundfile Resource
  /// </summary>
  public enum SoundType
  {
    /// <summary>
    /// MP3 File
    /// </summary>
    mp3 = 0,
    /// <summary>
    /// WAV File
    /// </summary>
    wav,
    /// <summary>
    /// WMA File
    /// </summary>
    wma,
  }

  /// <summary>
  /// Descriptor of an installed voice
  ///  - derived from .Net Speech Library
  /// </summary>
  public class SoundInfo
  {
    /// <summary>
    /// The Meldody
    /// </summary>
    public Melody Melody { get; } = Melody.Silence;
    /// <summary>
    /// Display Name
    /// </summary>
    public string Name { get; } = "";
    /// <summary>
    /// Melody description
    /// </summary>
    public string Description { get; } = "";
    /// <summary>
    /// Melody ID
    /// </summary>
    public string Id { get; } = "";
    /// <summary>
    /// Type of File
    /// </summary>
    public SoundType SType { get; } = SoundType.mp3;
    /// <summary>
    /// Second Step per Tone in the file
    /// </summary>
    public float ToneStep_sec { get; } = 1;
    /// <summary>
    /// Full Tone Sample Duration [sec]
    /// </summary>
    public float ToneDuration_sec { get; } = 0.5f;
    /// <summary>
    /// Number of Tones in the Sound File
    /// </summary>
    public uint NumTones { get; } = 1;


    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="name">Melody Display Name</param>
    /// <param name="desc">Melody Description</param>
    /// <param name="id">Melody ID</param>
    /// <param name="melody">Melody item</param>
    /// <param name="type">Type of Soundfile Resource</param>
    /// <param name="ToneStep">The Step per Tone in the file [sec]</param>
    /// <param name="ToneDuration">The Duration of a Tone in the file [sec]</param>
    /// <param name="numTones">Number of Tones in the file</param>
    public SoundInfo( string name, string desc, string id, Melody melody, SoundType type, float ToneStep, float ToneDuration, uint numTones )
    {
      Melody = melody;
      SType = type;
      Name = name;
      Description = desc;
      Id = id;
      ToneStep_sec = (ToneStep >= 0.2) ? ToneStep : 0.2f; // must be at least 0.2 sec per Tone
      ToneDuration_sec = (ToneDuration >= 0.05f) ? ToneDuration : 0.05f;  // must be at least 0.05 sec per tune
      NumTones = (numTones > 0) ? numTones : 1; // must be >1
    }

    /// <summary>
    /// Return a copy of This
    /// </summary>
    /// <returns>A SoundInfo</returns>
    public SoundInfo AsCopy( )
    {
      return new SoundInfo( this.Name, this.Description, this.Id, this.Melody, this.SType, this.ToneStep_sec, this.ToneDuration_sec, this.NumTones );
    }

    /// <summary>
    /// String representation of this object
    /// </summary>
    public override string ToString( ) => Description;



  }

}
