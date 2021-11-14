using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Various Calculations implemented as static methods
  /// </summary>
  internal static class Calculator
  {
    /// <summary>
    /// Pacer to calculate some averages from the running data
    ///  Should be called whenever the Sim Updates the data
    /// </summary>
    public static void PaceCalculator( )
    {
      // list all methods which need to constantly readout SimData here
      FuelFlowTotalSampler( );
    }


    #region AVG Module

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


    #endregion

    #region FUEL Avg Use and Reach calculation

    /// <summary>
    /// True if the fuel imbalance between L and R is more than a limit
    /// </summary>
    public static bool HasFuelImbalance {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return false; // cannot calculate anything

        var imbalanceGal
          =  Math.Abs( SC.SimConnectClient.Instance.AircraftModule.FuelQuantityLeft_gal -SC.SimConnectClient.Instance.AircraftModule.FuelQuantityRight_gal);
        if ( imbalanceGal > ( SC.SimConnectClient.Instance.AircraftModule.FuelCapacityTotal_gal * 0.15 ) ) {
          //Imbalance > 15% Total Fuel
          return true;
        }
        return false;
      }
    }

    /// <summary>
    /// True if the fuel last less than the warning time
    /// </summary>
    public static bool FuelReachWarn {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return false; // cannot calculate anything

        return FuelReach_sec( ) < 3600f; // warn <1h, alert <1/2h
      }
    }

    /// <summary>
    /// True if the fuel last less than the alert time
    /// </summary>
    public static bool FuelReachAlert {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return false; // cannot calculate anything

        return FuelReach_sec( ) < 1800f; // warn <1h, alert <1/2h
      }
    }


    // Fuel Flow
    private static AvgModule m_avgFuelFlowModule = new AvgModule( 5 ); // use 10 samples to average

    /// <summary>
    /// Sample the total fuel flow in  gal/hour and feed the AvgModule
    /// </summary>
    /// <returns></returns>
    private static void FuelFlowTotalSampler( )
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
    public static float AvgFuelFlowTotal_galPh( )
    {
      //return SC.SimConnectClient.Instance.AircraftModule.EstimatedCruiseFFlow_gph;
      return m_avgFuelFlowModule.Avg;
    }

    /// <summary>
    /// Calculate how long the the fuel lasts with the current average and the current quantity
    /// </summary>
    /// <returns>The fuel reach in seconds</returns>
    public static float FuelReach_sec( )
    {
      if ( AvgFuelFlowTotal_galPh() <= 0 ) return float.NaN;

      return ( SC.SimConnectClient.Instance.AircraftModule.FuelQuantityTotal_gal / m_avgFuelFlowModule.Avg ) * 3600f;
    }

    #endregion

    #region WYP ESTIMATES

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

    #endregion

    #region ICING Evaluation
    /// <summary>
    /// Truen when Icing conditions are present
    /// </summary>
    public static bool IcingCondition {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return false; // cannot calculate anything

        return SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC < 4;
      }
    }
    #endregion

    #region NAV ID Evaluation
    /// <summary>
    /// Returns the NAV1 ID for the tuned Station
    /// </summary>
    public static string NAV1_ID {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return ""; // cannot calculate anything

        return
          ( SC.SimConnectClient.Instance.NavModule.GS1_flag ? "‡◊"        // GS received
          : SC.SimConnectClient.Instance.NavModule.GS1_available ? "‡ "  // GS available
          : SC.SimConnectClient.Instance.NavModule.Nav1_hasLOC ? "† "    // LOC availbe
          : "  " ) + SC.SimConnectClient.Instance.NavModule.Nav1_Ident;
      }
    }
    /// <summary>
    /// Returns the NAV2 ID for the tuned Station
    /// </summary>
    public static string NAV2_ID {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return ""; // cannot calculate anything

        return
          ( SC.SimConnectClient.Instance.NavModule.GS2_flag ? "‡◊"        // GS received
          : SC.SimConnectClient.Instance.NavModule.GS2_available ? "‡ "  // GS available
          : SC.SimConnectClient.Instance.NavModule.Nav2_hasLOC ? "† "    // LOC availbe
          : "  " ) + SC.SimConnectClient.Instance.NavModule.Nav2_Ident;
      }
    }
    #endregion

    #region SimRate Calc

    /// <summary>
    /// Calculate the Inc,Dec Steps needed to get back to Normal
    ///   Assumes that the Rate is set by 1.0   *2 or /2 (0.25, 0.5, 1, 2, 4, 8, ..)
    ///   As of Nov.2021 ...
    /// </summary>
    /// <returns>The steps needed (pos=> increase, neg=> decrease  rate)</returns>
    public static int SimRateStepsToNormal( )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return 0; // cannot calculate anything

      var r = SC.SimConnectClient.Instance.AircraftModule.SimRate_rate;
      int steps = 0;
      // (0.25, 0.5, 1, 2, 4, 8, ..) only a float may not represent the numbers exactly 
      // so we add some tolerance for the resolution here (shifting all to Integers then rounding would be a solution too... e.g. *8)
      if ( r > 1.01 ) {
        // should get us down to 1.00 
        while ( r > 1.01 ) {
          steps--;
          r /= 2.0f;
        }
      }
      else if ( r < 0.99 ) {
        // should get us up to 1.00
        while ( r < 0.99 ) {
          steps++;
          r *= 2.0f;
        }
      }

      return steps;
    }

    #endregion

    #region Load calculations
    /// <summary>
    /// Returns the Load % from current torque and rpm (0...1)
    /// </summary>
    /// <param name="trq_ftlb">Torque in ft Lb</param>
    /// <param name="erpm">Engine RPM</param>
    /// <param name="maxHP">Max rated HP</param>
    /// <returns>The % Load</returns>
    public static float LoadPrct(float trq_ftlb, float erpm, float maxHP )
    {
      return ( trq_ftlb * (erpm/5252.0f)) / maxHP;
    }

    /// <summary>
    /// Returns a calculated MaxHP from current torque and rpm at 100% Load
    /// </summary>
    /// <param name="trq_ftlb">Torque in ft Lb</param>
    /// <param name="erpm">Engine RPM</param>
    /// <returns>The calculated MaxHP</returns>
    public static float MaxHPCalibration( float trq_ftlb, float erpm )
    {
      return ( trq_ftlb * ( erpm / 5252.0f ) );
    }
    #endregion

  }
}
