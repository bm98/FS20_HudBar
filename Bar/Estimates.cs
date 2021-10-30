using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Calculate some estimates from current
  /// </summary>
  class Estimates
  {
    // storage
    private static float m_gs = 0;
    private static float m_alt = 0;
    private static float m_vs = 0;

    private static float m_dampFactor = 9; // proportion of current and new value
    private static float m_divider = m_dampFactor+1; // we don't recalculate this one each time

    /// <summary>
    /// Update the aircraft values
    ///   Dampens the input to stabilize the readouts
    /// </summary>
    /// <param name="gs">Groundspeed [kt]</param>
    /// <param name="alt">Current Altitude [ft]</param>
    /// <param name="vs">Vert Speed [fpm]</param>
    public static void UpdateValues( float gs, float alt, float vs )
    {
      // for now all values are dampened with the same proportion
      m_gs = ( m_gs * m_dampFactor + gs ) / m_divider;
      m_alt = ( m_alt * m_dampFactor + alt ) / m_divider;
      m_vs = ( m_vs * m_dampFactor + vs ) / m_divider;
    }

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
    /// <param name="tgtAlt">Target Altitude [ft]</param>
    /// <param name="tgtDist">Target Distance [nm]</param>
    /// <returns>Required VS to get to target at altitude</returns>
    public static float VSToTgt_AtAltitude( float tgtAlt, float tgtDist )
    {
      if ( tgtDist <= 0.0f ) return 0; // avoid Div0 and cannot calc backwards 
      if ( m_gs <= 0.0f ) return 0;      // avoid Div0 and cannot calc with GS <=0

      float dFt = tgtAlt - m_alt;
      float minToTgt = tgtDist / NmPerMin( m_gs );
      return (int)Math.Round( ( dFt / minToTgt ) / 100f ) * 100;
    }

    /// <summary>
    /// The Altitude at Target with current GS and VS
    /// </summary>
    /// <param name="tgtDist">Target Distance [nm]</param>
    /// <returns>The altitude at target with current GS and VS from current Alt</returns>
    public static float AltitudeAtTgt( float tgtDist )
    {
      if ( tgtDist <= 0.0f ) return 0; // avoid Div0 and cannot calc backwards 
      if ( m_gs <= 0.0f ) return 0;      // avoid Div0 and cannot calc with GS <=0

      float minToTgt = tgtDist / NmPerMin( m_gs );
      float dAlt = m_vs * minToTgt;
      return (int)Math.Round( ( m_alt + dAlt ) / 100f ) * 100;
    }


  }
}
