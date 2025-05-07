using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// FAA/ArcGIS Imagery
  /// 
  /// https://faa.maps.arcgis.com/home/item.html?id=6ab79dc5de5743adb3e3b6e3c803aa59
  /// 
  /// https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/VFR_Sectional/MapServer/WMTS/tile/1.0.0/VFR_Sectional/default/default028mm/{z}/{y}/{x}
  /// ZOOM 8..12
  /// </summary>
  internal sealed class FAA_VFR_Sectional : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static FAA_VFR_Sectional Instance => lazy.Value;
    private static readonly Lazy<FAA_VFR_Sectional> lazy = new Lazy<FAA_VFR_Sectional>( ( ) => new FAA_VFR_Sectional( ) );
    private FAA_VFR_Sectional( )
      : base( MapProvider.FAA_VFR_Sectional )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Federal Aviation Administration, Aeronautical Information Services" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      MinZoom = 8;
      MaxZoom = 12;
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/VFR_Sectional/MapServer/WMTS/tile/1.0.0/VFR_Sectional/default/default028mm/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F42" );

    public override string Name { get; } = "FAA VFR Sectional";

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
