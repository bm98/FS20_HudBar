using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// A Meter Class for Checkpoints
  ///  meters time and distance from trigger point&time
  /// </summary>
  class CPointMeter
  {
    private const int c_secPerDay = 24*60*60;

    /// <summary>
    /// True if the Meter has ever started
    /// </summary>
    public bool Started { get; private set; } = false;

    private double m_startLat=0;
    private double m_startLon=0;
    private int m_startSec;

    private double m_lapseLat=0;
    private double m_lapseLon=0;
    private int m_lapseSec;

    /// <summary>
    /// Start the Meter with parameters
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Start( double lat, double lon, int sec )
    {
      m_startLat = lat;
      m_startLon = lon;
      m_startSec = sec;
      Started = true;
    }

    /// <summary>
    /// Lapse the Meter for readout
    ///  takes care of midnight change over but not further days
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="sec">SimSeconds since Zulu 00:00</param>
    public void Lapse( double lat, double lon, int sec )
    {
      m_lapseLat = lat;
      m_lapseLon = lon;
      m_lapseSec = sec;
      // could be crossing midnight .. but we don't cover another day...
      if ( m_lapseSec < m_startSec )
        m_lapseSec += c_secPerDay;
    }

    /// <summary>
    /// The elapsed distance in nm
    /// </summary>
    public double Distance => ( Started ) ? DistanceToNm( m_startLat, m_startLon, m_lapseLat, m_lapseLon ) : 0;

    /// <summary>
    /// The elapsed seconds
    /// </summary>
    public int Duration => ( Started ) ? m_lapseSec - m_startSec : 0;


    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Latitude/longitude spherical geodesy tools                         (c) Chris Veness 2002-2017  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/latlong.html                                                    */
    /* www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-spherical.html                       */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /// <summary>
    /// Const Km per Nm
    /// </summary>
    private const double KmPerNm = 1.852;
    /// <summary>
    /// Const Earth radius in M
    /// </summary>
    private const double EarthRadiusM = 6371.0E3;
    /// <summary>
    /// Const Earth radius in km
    /// </summary>
    private const double EarthRadiusKm = EarthRadiusM / 1000;
    /// <summary>
    /// Const Earth radius in Nm
    /// </summary>
    private const double EarthRadiusNm = EarthRadiusKm / KmPerNm;

    /// <summary>
    /// Returns the distance from ‘this’ point to destination point (using haversine formula).
    ///      * @example
    ///      *     var p1 = new LatLon( 52.205, 0.119 );
    ///      *     var p2 = new LatLon( 48.857, 2.351 );
    ///      *     var d = p1.distanceTo( p2 ); // 404.3 km
    /// </summary>
    /// <param name="point">{LatLon} point - Latitude/longitude of destination point</param>
    /// <param name="radius">{number} [radius=6371e3] - (Mean) radius of earth (defaults to radius in metres).</param>
    /// <returns>{number} Distance between this point and destination point, in same units as radius.</returns>
    public double DistanceToNm(
      double start_lat, double start_lon,
      double point_lat, double point_lon,
      double radius = EarthRadiusNm )
    {
      // a = sin²(Δφ/2) + cos(φ1)⋅cos(φ2)⋅sin²(Δλ/2)
      // tanδ = √(a) / √(1−a)
      // see mathforum.org/library/drmath/view/51879.html for derivation
      var R = radius;
      double φ1 = start_lat.ToRadians( );
      double λ1 = start_lon.ToRadians( );
      double φ2 = point_lat.ToRadians( );
      double λ2 = point_lon.ToRadians( );
      double Δφ = φ2 - φ1;
      double Δλ = λ2 - λ1;

      var a = Math.Sin( Δφ / 2 ) * Math.Sin( Δφ / 2 )
            + Math.Cos( φ1 ) * Math.Cos( φ2 )
            * Math.Sin( Δλ / 2 ) * Math.Sin( Δλ / 2 );
      var c = 2 * Math.Atan2( Math.Sqrt( a ), Math.Sqrt( 1 - a ) );
      var d = R * c;

      return d;
    }

  }

  #region Extension

  /// <summary>
  /// Extension ToRadians / ToDegrees for double type
  /// </summary>
  internal static class Foo
  {
    public static double ToRadians( this double angleInDegree )
    {
      return ( angleInDegree * Math.PI ) / 180.0;
    }

    public static double ToDegrees( this double angleInRadians )
    {
      return angleInRadians * ( 180.0 / Math.PI );
    }

  }
  #endregion

}
