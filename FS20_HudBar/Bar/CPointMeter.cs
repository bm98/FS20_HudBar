using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// A Meter Class for Checkpoints
  ///  meters time and distance from trigger point&time
  /// </summary>
  class CPointMeter
  {
    private const int c_secPerDay = 24 * 60 * 60;

    /// <summary>
    /// True if the Meter has ever started
    /// </summary>
    public bool Started { get; private set; } = false;

    private LatLon m_startLatLon = new LatLon( );
    private double m_startSec;

    private LatLon m_lapseLatLon = new LatLon( );
    private double m_lapseSec;

    /// <summary>
    /// Start the Meter with parameters
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Start( LatLon latLon, double sec )
    {
      m_startLatLon = latLon;
      m_startSec = sec;
      Started = true;
    }

    /// <summary>
    /// Lapse the Meter for readout
    ///  takes care of midnight change over but not further days
    /// </summary>
    /// <param name="latLon">Latitude, Longitude</param>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Lapse( LatLon latLon, double sec )
    {
      m_lapseLatLon = latLon;
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
    /// The elapsed distance in nm
    /// </summary>
    public double Distance => (Started) ? m_startLatLon.DistanceTo( m_lapseLatLon, ConvConsts.EarthRadiusNm ) : 0;

    /// <summary>
    /// The elapsed seconds
    /// </summary>
    public int Duration => (int)((Started) ? m_lapseSec - m_startSec : 0);

  }

}
