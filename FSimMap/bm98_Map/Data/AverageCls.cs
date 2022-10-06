using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Data
{
  /// <summary>
  /// Class to support Average calculations
  ///  Calculates 'real' average of the samples provided
  ///  Take care: Long chains may have performance penalties..
  /// </summary>
  internal class AvgModule
  {
    // full resolution numbers
    private double m_currentValue = 0;
    private double m_prevValue = 0;

    private ushort m_nSamples = 1;
    private ushort m_precision = 3;
    private Queue<double> m_samples;

    /// <summary>
    /// cTor: init with the sample size
    /// </summary>
    /// <param name="numSamples">Length of the number chain to average (default=5)</param>
    /// <param name="precision">Outgoing number of Digits (default=3)</param>
    public AvgModule( ushort numSamples = 5, ushort precision = 3 )
    {
      m_nSamples = numSamples;
      m_precision = precision;
      m_samples = new Queue<double>( m_nSamples + 1 );
    }
    /// <summary>
    /// Add one sample
    /// </summary>
    /// <param name="value">A sample</param>
    public void Sample( float value )
    {
      if (float.IsNaN( value )) return; // simply ignore NaNs

      m_prevValue = m_currentValue;
      m_samples.Enqueue( value / m_nSamples ); // store scaled, so we only use the Sum for returning the value
      while (m_samples.Count > m_nSamples) {
        m_samples.Dequeue( );
      }
      m_currentValue = (m_samples.Count <= 0) ? 0 : m_samples.Sum( );
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
    public int Direction => (Avg > AvgPrev) ? 1 : (Avg < AvgPrev) ? -1 : 0;
  }


  /// <summary>
  /// Class to support Rolling Average calculations
  ///  Adds a new value with 1/length weight to calculate the new value
  /// </summary>
  internal class AvgModule_Rolling
  {
    // full resolution numbers
    private double m_currentValue = 0;
    private double m_prevValue = 0;

    private ushort m_nSamples = 1; // lenght of accumulation
    private ushort m_precision = 3;

    private double m_scaleCurrent = 1;  // precalc: the weight of the current value
    private double m_scaleNew = 1;      // precalc: the weight of the to be added value (sum of scales should be 1.0)

    /// <summary>
    /// cTor: init with the sample size
    /// </summary>
    /// <param name="numSamples">Length of the number chain to average (default=5)</param>
    /// <param name="precision">Outgoing number of Digits (default=3)</param>
    public AvgModule_Rolling( ushort numSamples = 5, ushort precision = 3 )
    {
      m_nSamples = numSamples;
      m_precision = precision;
      // precalc scales
      m_scaleNew = 1.0 / m_nSamples;
      m_scaleCurrent = 1.0 - m_scaleNew;
    }
    /// <summary>
    /// Add one sample
    /// </summary>
    /// <param name="value">A sample</param>
    public void Sample( float value )
    {
      if (float.IsNaN( value )) return; // simply ignore NaNs

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
    public int Direction => (Avg > AvgPrev) ? 1 : (Avg < AvgPrev) ? -1 : 0;

  }

}