using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Lights;

namespace PingLib
{
  /// <summary>
  /// The Waveform to generate
  /// </summary>
  public enum WaveForm
  {
    /// <summary>
    /// Will output silence
    /// </summary>
    Silence = 0,
    /// <summary>
    /// Sine Wave
    /// </summary>
    SinWave,
    /// <summary>
    /// Triangle Wave
    /// </summary>
    TriangleWave,
    /// <summary>
    /// Square Wave
    /// </summary>
    SquareWave,
    // others..
  }

  /// <summary>
  /// Defines a Synth Wave as Input
  /// F Range is 100..6000 Hz
  /// </summary>
  public class SynthWave
  {
    private const double c_fMin = 100f;
    private const double c_fMax = 6000f;
    private const double c_PiHalf = Math.PI / 2.0;
    private const double c_Pi2 = Math.PI * 2.0;

    /// <summary>
    /// The WaveForm
    /// </summary>
    public WaveForm Shape {
      get => _shape;
      set => _shape = value;
    }
    private WaveForm _shape = WaveForm.Silence;

    /// <summary>
    /// The Main Frequency (100 .. 6'000 Hz)
    /// </summary>
    public double Frequency {
      get => _frequency;
      set {
        _frequency = (value < c_fMin) ? c_fMin : (value > c_fMax) ? c_fMax : value;
        _omega = _frequency * c_Pi2;
      }
    }
    private double _frequency = 0;
    private double _omega = 0;

    /// <summary>
    /// The Volume/Amplitude (0..1)
    /// </summary>
    public double Volume {
      get => _volume;
      set {
        _volume = (value < 0) ? 0 : (value > 1) ? 1 : value;
      }
    }
    private double _volume = 0;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public SynthWave( ) { }

    /// <summary>
    /// cTor: with arguments
    /// </summary>
    /// <param name="waveform">The waveform</param>
    /// <param name="freq">The frequ.</param>
    /// <param name="volume">The volume (defaults to 1.0)</param>
    public SynthWave( WaveForm waveform, double freq, double volume = 1 )
    {
      Shape = waveform;
      Frequency = freq;
      Volume = volume;
    }

    // stores the current point in time to maintain the phase of the output
    private double _phaseAngle = 0;

    /// <summary>
    /// Create a Wave sample for this item
    /// </summary>
    /// <returns></returns>
    internal double WaveSample( int samplingRate )
    {
      double angleIncrement = _omega / samplingRate;
      // sin wave is the base wave
      var sine = Math.Sin( _phaseAngle );

      double sOut = 0;
      switch (_shape) {
        case WaveForm.SinWave:
          sOut = sine * _volume;
          break;
        case WaveForm.TriangleWave:
          sOut = Math.Acos( sine ) / c_PiHalf * _volume;
          break;
        case WaveForm.SquareWave:
          if (sine >= 0)
            sOut = _volume;
          else
            sOut = -_volume;
          break;
        case WaveForm.Silence: break; // ampl. is 0 here..
        default: break; // should never happen ...
      }
      _phaseAngle += angleIncrement;
      return sOut;
    }

  }
}
