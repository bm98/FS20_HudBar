using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// FAA/ArcGIS Imagery
  /// 
  /// https://faa.maps.arcgis.com/home/item.html?id=0cb4491ce486481e9d81de2234b13332
  /// 
  /// https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/IFR_High/MapServer/WMTS/tile/1.0.0/IFR_High/default/default028mm/{z}/{y}/{x}
  /// ZOOM 5..9
  /// </summary>
  internal sealed class FAA_IFR_AreaHigh : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static FAA_IFR_AreaHigh Instance => lazy.Value;
    private static readonly Lazy<FAA_IFR_AreaHigh> lazy = new Lazy<FAA_IFR_AreaHigh>( ( ) => new FAA_IFR_AreaHigh( ) );
    private FAA_IFR_AreaHigh( )
      : base( MapProvider.FAA_IFR_AreaHigh )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Federal Aviation Administration, Aeronautical Information Services" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      MinZoom = 5;
      MaxZoom = 9;
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/IFR_High/MapServer/WMTS/tile/1.0.0/IFR_High/default/default028mm/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F44" );

    public override string Name { get; } = "FAA IFR Area High";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}
