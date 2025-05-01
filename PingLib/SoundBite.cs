using System.Linq;

using DbgLib;

namespace PingLib
{

  /// <summary>
  /// A sound bite to play
  /// </summary>
  public class SoundBite
  {
    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #endregion

    /// <summary>
    /// A Melody from the available ones
    /// </summary>
    public Melody Melody { get; set; } = Melody.Silence;

    /// <summary>
    /// Tone to play
    ///  Notes are samples within the Melody 0... 
    /// </summary>
    public uint Tone { get; set; } = 0;
    /// <summary>
    /// The Volume 0 .. 1.0 relative to the App Volume
    /// </summary>
    public float Volume { get; set; } = 1;
    /// <summary>
    /// The Duration the sample should play (-1 defaults to the defined duration)
    /// </summary>
    public float Duration { get; set; } = -1;
    /// <summary>
    /// Number of Loops to add to the one trigger
    /// </summary>
    public uint Loops { get; set; } = 0;
    /// <summary>
    /// Speed factor to play the tone with (nominal = 1.0)
    /// </summary>
    public float SpeedFact { get; set; } = 1;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public SoundBite( ) { }

    /// <summary>
    /// cTor: create from a number of values
    /// </summary>
    /// <param name="melody">Melody to play from</param>
    /// <param name="note">The Note to play</param>
    /// <param name="loops">Loops to add (max 5)</param>
    /// <param name="volume">Volume to play with (default = 1.0)</param>
    /// <param name="speed">Speed Factor (default = 1.0)</param>
    public SoundBite( Melody melody, uint note, uint loops, float volume = 1.0f, float speed = 1.0f )
    {
      Melody = melody;
      var tune = WaveProc.GetInstalledSounds( ).Where( x => x.Melody == melody ).FirstOrDefault( );
      if (tune == null) {
        // sould not happen .....
        LOG.Error( "SoundBite", $"Cannot find the Melody: {melody}" );
        Melody = Melody.Silence;
        return;
      }

      Tone = (note < tune.NumTones) ? note : note % tune.NumTones; // safeguard Note
      Volume = (volume >= 0) ? volume : 0; // must be >= 0.0
      Loops = (loops <= 5) ? loops : 5; // must be <= 5 
      SpeedFact = (speed > 0) ? speed : 1; // must be > 0.0
    }
    /// <summary>
    /// cTor: create from a number of values
    /// </summary>
    /// <param name="melody">Melody to play from</param>
    /// <param name="note">The Note to play</param>
    /// <param name="duration_sec">Play Duration in sec</param>
    /// <param name="loops">Loops to add (max 5)</param>
    /// <param name="volume">Volume to play with (default = 1.0)</param>
    /// <param name="speed">Speed Factor (default = 1.0)</param>
    public SoundBite( Melody melody, uint note, float duration_sec, uint loops, float volume = 1.0f, float speed = 1.0f )
    {
      Melody = melody;
      var tune = WaveProc.GetInstalledSounds( ).Where( x => x.Melody == melody ).FirstOrDefault( );
      if (tune == null) {
        // sould not happen .....
        LOG.Error( "SoundBite", $"Cannot find the Melody: {melody}" );
        Melody = Melody.Silence;
        return;
      }

      Tone = (note < tune.NumTones) ? note : note % tune.NumTones; // safeguard Note
      Volume = (volume >= 0) ? volume : 0; // must be >= 0.0
      Duration = (duration_sec < tune.ToneDuration_sec) ? duration_sec : -1; // must be < default of Tune
      Loops = (loops <= 5) ? loops : 5; // must be <= 5 
      SpeedFact = (speed > 0) ? speed : 1; // must be > 0.0
    }


    /// <summary>
    /// cTor: Copy constructor
    /// </summary>
    /// <param name="other"></param>
    public SoundBite( SoundBite other )
    {
      this.Melody = other.Melody;
      this.Tone = other.Tone;
      this.Duration = other.Duration;
      this.Loops = other.Loops;
      this.Volume = other.Volume;
      this.SpeedFact = other.SpeedFact;
    }


  }
}
