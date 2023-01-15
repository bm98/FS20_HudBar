using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FS20_HudBar.GUI;
using static FS20_HudBar.Conversions;
using static FS20_HudBar.GUI.GUI_Colors;
using PingLib;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Various Calculations implemented as static methods
  /// </summary>
  internal static class Calculator
  {
    // Timing infrastructure
    private static double _LastTick = 0;
    private static double _tick = 0;
    private static double _deltaT_s = 0;

    /// <summary>
    /// Pacer to calculate some averages from the running data
    ///  Should be called whenever the Sim Updates the data
    /// </summary>
    public static void PaceCalculator( )
    {
      double newTick = SC.SimConnectClient.Instance.HudBarModule.SimTime_zulu_Dsec;
      if (newTick < (_LastTick - 1.0)) {
        // new Tick is in the past (change in SimTime?? or midnight ZULU)
        _LastTick = newTick; // try to recover next round
      }
      else if (newTick > _LastTick) {
        // only when time passed...
        _tick = newTick;
        _deltaT_s = _tick - _LastTick;
        _LastTick = _tick;

        // list all methods which need to constantly readout SimData here
        FuelFlowTotalSampler( );
        // TE_RateSampler( );

        // Update Estimate Calculation with Acf data
        UpdateValues(
          SC.SimConnectClient.Instance.HudBarModule.Groundspeed_kt,
          SC.SimConnectClient.Instance.HudBarModule.AltMsl_ft,
          SC.SimConnectClient.Instance.HudBarModule.VS_ftPmin
        );

      }
    }

    #region AVG Modules

    /// <summary>
    /// Class to support Average calculations
    ///  Calculates 'real' average of the samples provided
    ///  Take care: Long chains may have performance penalties..
    /// </summary>
    private class AvgModule
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
    private class AvgModule_Rolling
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

    #endregion

    #region FUEL Avg Use and Reach calculation

    /// <summary>
    /// True if the fuel imbalance between L and R is more than a limit
    /// </summary>
    public static bool HasFuelImbalance {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return false; // cannot calculate anything

        var imbalanceGal = Math.Abs( SC.SimConnectClient.Instance.HudBarModule.FuelQuantityLeft_gal
                                      - SC.SimConnectClient.Instance.HudBarModule.FuelQuantityRight_gal );
        var min = Math.Min( SC.SimConnectClient.Instance.HudBarModule.FuelQuantityLeft_gal, SC.SimConnectClient.Instance.HudBarModule.FuelQuantityRight_gal );
        if (imbalanceGal > (min * 0.15)) {
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
        if (!SC.SimConnectClient.Instance.IsConnected) return false; // cannot calculate anything

        return FuelReach_sec( ) < 3600f; // warn <1h, alert <1/2h
      }
    }

    /// <summary>
    /// True if the fuel last less than the alert time
    /// </summary>
    public static bool FuelReachAlert {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return false; // cannot calculate anything

        return FuelReach_sec( ) < 1800f; // warn <1h, alert <1/2h
      }
    }


    // Fuel Flow
    private static AvgModule m_avgFuelFlowModule = new AvgModule( 5 ); // use N samples to average

    /// <summary>
    /// Sample the total fuel flow in  lb/hour and feed the AvgModule
    /// </summary>
    /// <returns></returns>
    private static void FuelFlowTotalSampler( )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return; // cannot calculate anything

      var eModule = SC.SimConnectClient.Instance.HudBarModule;
      float ff = eModule.Engine1_FuelFlow_lbPh;
      if (eModule.NumEngines > 1) ff += eModule.Engine2_FuelFlow_lbPh;
      if (eModule.NumEngines > 2) ff += eModule.Engine3_FuelFlow_lbPh;
      if (eModule.NumEngines > 3) ff += eModule.Engine4_FuelFlow_lbPh;

      m_avgFuelFlowModule.Sample( ff );
    }

    /// <summary>
    /// Returns a running average FuelFlow lb / hour
    /// </summary>
    /// <returns>Avg Fuel Flow [lb/h]</returns>
    public static float AvgFuelFlowTotal_lbPh( )
    {
      return m_avgFuelFlowModule.Avg;
    }

    /// <summary>
    /// Calculate how long the the fuel lasts with the current average and the current quantity
    /// </summary>
    /// <returns>The fuel reach in seconds</returns>
    public static float FuelReach_sec( )
    {
      if (AvgFuelFlowTotal_lbPh( ) <= 0) return float.NaN;

      return (SC.SimConnectClient.Instance.HudBarModule.FuelQuantityTotal_lb / m_avgFuelFlowModule.Avg) * 3600f;
    }

    #endregion

    #region WYP ESTIMATES

    // storage
    private static float m_gs = 0;
    private static float m_alt = 0;
    private static float m_vs = 0;

    private static float m_dampFactor = 9; // proportion of current and new value
    private static float m_divider = m_dampFactor + 1; // we don't recalculate this one each time

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
      m_gs = (m_gs * m_dampFactor + gs) / m_divider;
      m_alt = (m_alt * m_dampFactor + alt) / m_divider;
      m_vs = (m_vs * m_dampFactor + vs) / m_divider;
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
      if (tgtDist <= 0.0f) return 0; // avoid Div0 and cannot calc backwards 
      if (m_gs <= 0.0f) return 0;      // avoid Div0 and cannot calc with GS <=0

      float dFt = tgtAlt - m_alt;
      float minToTgt = tgtDist / NmPerMin( m_gs );
      return (int)Math.Round( (dFt / minToTgt) / 100f ) * 100;
    }

    /// <summary>
    /// The Altitude at Target with current GS and VS
    /// </summary>
    /// <param name="tgtDist">Target Distance [nm]</param>
    /// <returns>The altitude at target with current GS and VS from current Alt</returns>
    public static float AltitudeAtTgt( float tgtDist )
    {
      if (tgtDist <= 0.0f) return m_alt; // cannot calc backwards aiming
      if (m_gs <= 1f) return m_alt;      // should not calc with GS <=1

      float minToTgt = tgtDist / NmPerMin( m_gs );
      float dAlt = m_vs * minToTgt;
      return (int)Math.Round( (m_alt + dAlt) / 100f ) * 100; // fix at 100 steps
    }

    #endregion

    #region ICING Evaluation
    /// <summary>
    /// Truen when Icing conditions are present
    /// </summary>
    public static bool IcingCondition {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return false; // cannot calculate anything

        return SC.SimConnectClient.Instance.HudBarModule.OutsideTemperature_degC < 4;
      }
    }
    #endregion

    #region NAV ID Evaluation


    /// <summary>
    /// Returns the NAV1 ID for the tuned Station
    /// </summary>
    public static string NAV1_ID {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return "  "; // cannot calculate anything

        string gsi = (SC.SimConnectClient.Instance.NavModule.GS1_flag ? " ◊"        // GS received
          : SC.SimConnectClient.Instance.NavModule.GS1_available ? " ‡"  // GS available
          : " ");
        string id = SC.SimConnectClient.Instance.NavModule.Nav1_Ident + gsi;

        return id;
      }
    }
    /// <summary>
    /// Returns the NAV2 ID for the tuned Station
    /// </summary>
    public static string NAV2_ID {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return "  "; // cannot calculate anything

        string gsi = (SC.SimConnectClient.Instance.NavModule.GS2_flag ? " ◊"        // GS received
          : SC.SimConnectClient.Instance.NavModule.GS2_available ? " ‡"  // GS available
          : " ");
        string id = SC.SimConnectClient.Instance.NavModule.Nav2_Ident + gsi;

        return id;
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
      if (!SC.SimConnectClient.Instance.IsConnected) return 0; // cannot calculate anything

      var r = SC.SimConnectClient.Instance.HudBarModule.SimRate_rate;
      int steps = 0;
      // (0.25, 0.5, 1, 2, 4, 8, ..) only a float may not represent the numbers exactly 
      // so we add some tolerance for the resolution here (shifting all to Integers then rounding would be a solution too... e.g. *8)
      if (r > 1.01) {
        // should get us down to 1.00 
        while (r > 1.01) {
          steps--;
          r /= 2.0f;
        }
      }
      else if (r < 0.99) {
        // should get us up to 1.00
        while (r < 0.99) {
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
    public static float LoadPrct( float trq_ftlb, float erpm, float maxHP )
    {
      return (trq_ftlb * (erpm / 5252.0f)) / maxHP;
    }

    /// <summary>
    /// Returns a calculated MaxHP from current torque and rpm at 100% Load
    /// </summary>
    /// <param name="trq_ftlb">Torque in ft Lb</param>
    /// <param name="erpm">Engine RPM</param>
    /// <returns>The calculated MaxHP</returns>
    public static float MaxHPCalibration( float trq_ftlb, float erpm )
    {
      return (trq_ftlb * (erpm / 5252.0f));
    }
    #endregion

    #region Variometer Sounds

    /* NEW - only hi,lo and direction change */

    public enum EVolume
    {
      V_Silent = 0,
      // audible ones
      V_Plus,
      V_PlusMinus,
      // Can use the audible levels above
      V_LAST,
    }

    // Const built for TSynth2 Sound
    private const uint n_silence = 0;
    private const uint n_positive = 1;
    private const uint n_negative = 2;
    private const uint n_negative2 = 3; // 1/2 oct down from n_negative
    private const uint n_negative3 = 4; // 1 oct down from n_negative

    /// <summary>
    /// Set the value dependent Note in the soundBite
    /// </summary>
    /// <param name="volume">Enum for the Volume</param>
    /// <param name="value">the vario rate im m/sec</param>
    /// <param name="soundBite">The Sound to play</param>
    /// <returns>Returns true if the note has changed</returns>
    public static bool ModNote( EVolume volume, float value, SoundBitePitched soundBite )
    {
      bool changed = false;
      if (volume != EVolume.V_Silent) {
        // ping enabled
        if (value >= 0.5) {
          // starts at or above 0.5
          if (soundBite.Tone != n_positive) {
            // change if needed
            soundBite.Tone = n_positive;
            soundBite.SetPitchRange( 0.5f, 5, 0.9f, 1.99f );
          }
          soundBite.SetPitch( value );
          changed = true; // as the pitch changes mostly every cycle, we just assume it changed...
        }
        else if ((value <= -0.7) && (volume == EVolume.V_PlusMinus)) {
          // starts at or below -0.5 and if minus is pinged
          if (soundBite.Tone != n_negative3) {
            // change if needed
            soundBite.Tone = n_negative3;
            soundBite.SetPitchRange( -3f, -0.7f, 0.7f, 1f );
          }
          soundBite.SetPitch( value );
          changed = true; // as the pitch changes mostly every cycle, we just assume it changed...
        }
        else {
          // around 0 or below and Plus only -> silence
          if (soundBite.Tone != n_silence) {
            // change if needed
            soundBite.Tone = n_silence;
            changed = true;
          }
        }
      }
      else {
        // ping disabled
        if (soundBite.Tone != n_silence) {
          // change if needed
          soundBite.Tone = n_silence;
          changed = true;
        }
      }

      return changed;
    }

    #endregion

    #region IAS limits


    #endregion
  }
}
