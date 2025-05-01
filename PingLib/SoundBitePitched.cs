using System;

namespace PingLib
{
  /// <summary>
  /// A SoundBite with Pitch Control
  /// </summary>
  public class SoundBitePitched : SoundBite
  {
    // control the pitch via the SpeedFact of the playback
    /// <summary>
    /// SpeedFactor Range-Min
    /// </summary>
    protected float _minSpeed = 1.0f;
    /// <summary>
    /// SpeedFactor Range-Max
    /// </summary>
    protected float _maxSpeed = 1.0f;
    // the used value to scale the SpeedFact with
    /// <summary>
    /// Value Range-Min
    /// </summary>
    protected float _minValue = 0.0f;
    /// <summary>
    /// Value Range-Max
    /// </summary>
    protected float _maxValue = 1.0f;

    private float _scale = 1.0f;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public SoundBitePitched( ) { }

    /// <summary>
    /// cTor: create from a number of values
    /// </summary>
    /// <param name="melody">Melody to play from</param>
    /// <param name="note">The Note to play</param>
    /// <param name="loops">Loops to add (max 5)</param>
    /// <param name="volume">Volume to play with (default = 1.0)</param>
    /// <param name="speed">Speed Factor (default = 1.0)</param>
    public SoundBitePitched( Melody melody, uint note, uint loops, float volume = 1.0f, float speed = 1.0f )
      : base( melody, note, loops, volume, speed )
    {
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
    public SoundBitePitched( Melody melody, uint note, float duration_sec, uint loops, float volume = 1.0f, float speed = 1.0f )
      : base( melody, note, duration_sec, loops, volume, speed )
    {
    }

    /// <summary>
    /// cTor: Copy
    /// </summary>
    /// <param name="other">SoundBite to copy from</param>
    public SoundBitePitched( SoundBitePitched other )
    : base( other )
    {
      _minSpeed = other._minSpeed;
      _maxSpeed = other._maxSpeed;
      _minValue = other._minValue;
      _maxValue = other._maxValue;
    }

    /// <summary>
    /// Setup the Pitch Range 
    /// </summary>
    /// <param name="minValue">The minimum input value for the lowest pitch</param>
    /// <param name="maxValue">The maximum input value for the highest pitch</param>
    /// <param name="minSpeed">The minimum SpeedFact for the lowest pitch</param>
    /// <param name="maxSpeed">The maximum SpeedFact for the highest pitch</param>
    public void SetPitchRange( float minValue, float maxValue, float minSpeed, float maxSpeed )
    {
      _minValue = minValue;
      _maxValue = maxValue;
      // sanity check Div0 protection
      if (_maxValue == _minValue) {
        _minValue = 0; _maxValue = 1;
      }
      _minSpeed = minSpeed;
      _maxSpeed = maxSpeed;
      _scale = (_maxSpeed - _minSpeed) / (_maxValue - _minValue);
    }

    /// <summary>
    /// Set the pitch from the input value 
    /// Scaled by the setup range
    /// </summary>
    /// <param name="value">An input value</param>
    public void SetPitch( float value )
    {
      SpeedFact = _scale * (value - _minValue) + _minSpeed;
      SpeedFact = Math.Max( _minSpeed, Math.Min( _maxSpeed, SpeedFact ) ); // crop at the limits
    }


  }
}
