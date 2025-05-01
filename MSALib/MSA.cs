using CoordLib;

namespace MSALib
{
  /// <summary>
  /// A trivial MSA (Minimum Save Altitude) Lib
  /// derived from: 
  /// https://www.usgs.gov/coastal-changes-and-impacts/gmted2010
  /// https://www.ngdc.noaa.gov/mgg/topo/globe.html
  /// 
  /// GMTED2010 max30_grd, 30 arcsec patched with GLOBE elevation data and wrapped with Max value to 1° / 0.5° raster 
  /// 
  /// To allow for 'safe' altitudes the elevations are extended by 1000 ft rsp 2000 ft in areas above 1500 m and rounded up to 100ft quantities
  /// water bodies (in fact all NoData areas) return 0 ft
  /// 
  /// </summary>
  public static class MSA
  {
    private static MSAdata0_5Deg _msaData0_5 = new MSAdata0_5Deg( );
    private static MSAdata1Deg _msaData1 = new MSAdata1Deg( );

    /// <summary>
    /// The MSA value of a coord point
    /// </summary>
    /// <param name="lat">A Lat</param>
    /// <param name="lon">A Lon</param>
    /// <param name="res0_5">True for 0.5 deg grid values - default = false</param>
    /// <returns>An altitude in ft</returns>
    public static int Msa_ft( double lat, double lon, bool res0_5 = false ) => res0_5 ? _msaData0_5.MSAof_ft( lat, lon ) : _msaData1.MSAof_ft( lat, lon );

    /// <summary>
    /// The MSA value of a coord point
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <param name="res0_5">True for 0.5 deg grid values - default = false</param>
    /// <returns>An altitude in ft</returns>
    public static int Msa_ft( LatLon latLon, bool res0_5 = false ) => res0_5 ? _msaData0_5.MSAof_ft( latLon ) : _msaData1.MSAof_ft( latLon );


  }
}
