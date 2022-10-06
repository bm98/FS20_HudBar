using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A Trigger Band supporting float
  /// </summary>
  class TriggerBandF
  {

    /// <summary>
    /// cTor: empty
    /// </summary>
    public TriggerBandF( ) { }

    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="offset">The band offset</param>
    public TriggerBandF(float level, float offset )
    {
      Level = level;
      Offset = offset;
    }

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public float Level { get; set; } = 0f;

    /// <summary>
    /// A Band symmetrically around the Level +- Offset where 
    /// the trigger is detected once it enters the band and 
    /// not retriggered until it has left the band
    /// NOTE: it is not recommended to set this to 0f as it 
    /// would require a match of float numbers which leads to an unpredictable behavior
    /// </summary>
    public float Offset { get; set; } = 0.1f;

    /// <summary>
    /// Returns the Maximum Band Value
    /// </summary>
    public float Max => Level + Offset;
    /// <summary>
    /// Returns the Minimum Band Value
    /// </summary>
    public float Min => Level - Offset;

    /// <summary>
    /// Returns true if the given value is within the band
    /// </summary>
    /// <param name="level">The level value</param>
    /// <returns>True if within the band</returns>
    public bool InBand( float level )
    {
      if ( level > Max ) return false;
      if ( level < Min ) return false;
      return true;
    }

  }


  /// <summary>
  /// A Trigger Band supporting int
  /// </summary>
  class TriggerBandI
  {
    /// <summary>
    /// cTor: empty
    /// </summary>
    public TriggerBandI( ) { }

    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="offset">The band offset</param>
    public TriggerBandI( int level, int offset )
    {
      Level = level;
      Offset = offset;
    }

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public int Level { get; set; } = 0;

    /// <summary>
    /// A Band symmetrically around the Level +- Offset where 
    /// the trigger is detected once it enters the band and 
    /// not retriggered until it has left the band
    /// </summary>
    public int Offset { get; set; } = 1;

    /// <summary>
    /// Returns the Maximum Band Value
    /// </summary>
    public int Max => Level + Offset;

    /// <summary>
    /// Returns the Minimum Band Value
    /// </summary>
    public int Min => Level - Offset;

    /// <summary>
    /// Returns true if the given value is within the band
    /// </summary>
    /// <param name="level">The level value</param>
    /// <returns>True if within the band</returns>
    public bool InBand( int level )
    {
      if ( level > Max ) return false;
      if ( level < Min ) return false;
      return true;
    }

  }

}
