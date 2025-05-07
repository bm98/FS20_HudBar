using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// FAA/ArcGIS Imagery
  /// 
  /// https://faa.maps.arcgis.com/home/item.html?id=97ab03c55bb442b897daeba53a5cc85d
  /// 
  /// https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/IFR_AreaLow/MapServer/WMTS/tile/1.0.0/IFR_AreaLow/default/default028mm/{z}/{y}/{x}
  /// ZOOM 7..12
  /// </summary>
  internal sealed class FAA_IFR_AreaLow : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static FAA_IFR_AreaLow Instance => lazy.Value;
    private static readonly Lazy<FAA_IFR_AreaLow> lazy = new Lazy<FAA_IFR_AreaLow>( ( ) => new FAA_IFR_AreaLow( ) );
    private FAA_IFR_AreaLow( )
      : base( MapProvider.FAA_IFR_AreaLow )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Federal Aviation Administration, Aeronautical Information Services" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      MinZoom = 7;
      MaxZoom = 12;
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/IFR_AreaLow/MapServer/WMTS/tile/1.0.0/IFR_AreaLow/default/default028mm/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F43" );

    public override string Name { get; } = "FAA IFR Area Low";

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
