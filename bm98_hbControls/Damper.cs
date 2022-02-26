using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_hbControls
{
  /// <summary>
  /// Needle Damper
  /// </summary>
  internal class Damper
  {
    // full resolution numbers
    private double m_currentValue = 0;
    private double m_prevValue = 0 ;

    private ushort m_nSamples = 1; // lenght of accumulation
    private ushort m_precision = 3;

    private double m_scaleCurrent = 1;  // precalc: the weight of the current value
    private double m_scaleNew = 1;      // precalc: the weight of the to be added value (sum of scales should be 1.0)

    /// <summary>
    /// cTor: init with the sample size
    /// </summary>
    /// <param name="numSamples">Length of the number chain to average (default=5)</param>
    /// <param name="precision">Outgoing number of Digits (default=3)</param>
    public Damper( float initValue, ushort numSamples = 5, ushort precision = 3 )
    {
      m_prevValue = initValue;
      m_currentValue = initValue;
      m_nSamples = numSamples;
      m_precision = precision;
      // precalc scales
      m_scaleNew = 1.0 / m_nSamples;
      m_scaleCurrent = 1.0 - m_scaleNew;
    }

    /// <summary>
    /// Get; The set Dampening Factor (numSamples while creating the obj)
    /// </summary>
    public ushort Factor { get => m_nSamples; }

    /// <summary>
    /// Add one sample
    /// </summary>
    /// <param name="value">A sample</param>
    public void Sample( float value )
    {
      m_prevValue = m_currentValue;
      m_currentValue = m_scaleCurrent * m_currentValue + m_scaleNew * value;
    }

    /// <summary>
    /// Returns the Average
    /// </summary>
    public float Avg => (float)Math.Round( m_currentValue, m_precision );
    /// <summary>
    /// Returns the Previous Average
    /// </summary>
    public float AvgPrev => (float)Math.Round( m_prevValue, m_precision );

    /// <summary>
    /// Returns the Direction from Prev to Current Value (1: going up; -1 going down; 0: stay)
    /// </summary>
    public int Direction => ( Avg > AvgPrev ) ? 1 : ( Avg < AvgPrev ) ? -1 : 0;
  }
}
