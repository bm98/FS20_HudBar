using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Calculate some estimates from current
  /// </summary>
  class Estimates
  {

    /// <summary>
    /// Calculates the distance per minute based on the current GS
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    /// <returns>Dist per minute [nm]</returns>
    public static float NmPerMin( float gs )
    {
      return gs / 60.0f;
    }

    /// <summary>
    /// VS required to go from current Altitude to Set Altitude [fpm]
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    /// <param name="curAlt">Current Altitude [ft]</param>
    /// <param name="tgtAlt">Target Altitude [ft]</param>
    /// <param name="tgtDist">Target Distance [nm]</param>
    /// <returns>Required VS to get to target at altitude</returns>
    public static float VSToTgt_AtAltitude( float gs, float curAlt, float tgtAlt, float tgtDist )
    {
      if ( tgtDist <= 0.0f ) return 0; // avoid Div0 and cannot calc backwards 
      if ( gs <= 0.0f ) return 0;      // avoid Div0 and cannot calc with GS <=0

      float dFt = tgtAlt - curAlt;
      float minToTgt = tgtDist / (gs/60.0f);
      return dFt / minToTgt;
    }

    /// <summary>
    /// The Altitude at Target with current GS and VS
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    /// <param name="curAlt">Current Altitude [ft]</param>
    /// <param name="vs">Vert Speed [fpm]</param>
    /// <param name="tgtDist">Target Distance [nm]</param>
    /// <returns>The altitude at target with current GS and VS from current Alt</returns>
    public static float AltitudeAtTgt( float gs, float curAlt, float vs, float tgtDist )
    {
      if ( tgtDist <= 0.0f ) return 0; // avoid Div0 and cannot calc backwards 
      if ( gs <= 0.0f ) return 0;      // avoid Div0 and cannot calc with GS <=0

      float minToTgt = tgtDist / (gs/60.0f);
      float dAlt = vs * minToTgt;
      return curAlt + dAlt;
    }


  }
}
