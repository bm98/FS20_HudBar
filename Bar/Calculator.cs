using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Various Calculations
  /// </summary>
  class Calculator
  {

    /// <summary>
    /// Class to support Average calculations
    /// </summary>
    private class AvgModule
    {
      private int m_nSamples = 1;
      private Queue<float> m_samples;

      /// <summary>
      /// cTor: init with the sample size
      /// </summary>
      /// <param name="numSamples"></param>
      public AvgModule( int numSamples = 5 )
      {
        m_nSamples = numSamples;
        m_samples = new Queue<float>( m_nSamples + 1 );
      }
      /// <summary>
      /// Add one sample
      /// </summary>
      /// <param name="value">A sample</param>
      public void Sample( float value )
      {
        m_samples.Enqueue( value );
        while ( m_samples.Count > m_nSamples ) {
          m_samples.Dequeue( );
        }
      }

        /// <summary>
      /// Returns the Average
      /// </summary>
      public float Avg => ( m_samples.Count <= 0 ) ? 0 : m_samples.Average( );

    }


    /// <summary>
    /// Pacer to calculate some averages from the running data
    ///  Should be called whenever the Sim Updates the data
    /// </summary>
    static public void PaceCalculator( )
    {
      // list all methods which need to constantly readout SimData here
      FuelFlowTotalSampler( );
    }


    #region FUEL Avg Use and Reach calculation

    // Fuel Flow
    private static AvgModule m_avgFuelFlowModule = new AvgModule( 5 ); // use 10 samples to average

    /// <summary>
    /// Sample the total fuel flow in  gal/hour and feed the AvgModule
    /// </summary>
    /// <returns></returns>
    static private void FuelFlowTotalSampler( )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // cannot calculate anything

      var eModule = SC.SimConnectClient.Instance.EngineModule;
      float ff = eModule.Engine1_FuelFlow_galPh;
      if ( eModule.NumEngines > 1 ) ff += SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_galPh;
      if ( eModule.NumEngines > 2 ) ff += SC.SimConnectClient.Instance.EngineModule.Engine3_FuelFlow_galPh;
      if ( eModule.NumEngines > 3 ) ff += SC.SimConnectClient.Instance.EngineModule.Engine4_FuelFlow_galPh;

      m_avgFuelFlowModule.Sample( ff );
    }

    /// <summary>
    /// Returns a running average FuelFlow gal / hour
    /// </summary>
    /// <returns>Avg Fuel Flow [gal/h]</returns>
    static public float AvgFuelFlowTotal_galPh( )
    {
      return m_avgFuelFlowModule.Avg;
    }

    /// <summary>
    /// Calculate how long the the fuel lasts with the current average and the current quantity
    /// </summary>
    /// <returns>The fuel reach in seconds</returns>
    static public float FuelReach_sec( )
    {
      if ( m_avgFuelFlowModule.Avg <= 0 ) return float.NaN;

      return ( SC.SimConnectClient.Instance.AircraftModule.FuelQuantityTotal_gal / m_avgFuelFlowModule.Avg ) * 3600f;
    }

    #endregion


  }
}
