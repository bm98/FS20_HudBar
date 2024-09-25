using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Meters Sim time 
  /// </summary>
  class TimeMeter
  {
    private const int c_secPerDay = 24 * 60 * 60;
    /// <summary>
    /// True if the Meter has ever started
    /// </summary>
    public bool Started { get; private set; } = false;
    private double m_startSec;
    private double m_lapseSec;

    /// <summary>
    /// Start the Meter with parameters
    /// </summary>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Start( double sec )
    {
      m_startSec = sec;
      m_lapseSec=sec;
      Started = true;
    }

    /// <summary>
    /// Lapse the Meter for readout
    ///  takes care of midnight change over but not further days
    /// </summary>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Lapse(  double sec )
    {
      // sanity
      if (!Started) throw new InvalidOperationException( "Meter is not started" ); // don't lapse if not started

      m_lapseSec = sec;
      // could be crossing midnight .. but we don't cover another day...
      if (m_lapseSec < m_startSec)
        m_lapseSec += c_secPerDay;
    }

    /// <summary>
    /// Stop the Meter
    /// </summary>
    public void Stop( )
    {
      Started = false;
    }

    /// <summary>
    /// The elapsed seconds
    /// </summary>
    public int Duration => (int)(Started ? m_lapseSec - m_startSec : 0);


  }
}
