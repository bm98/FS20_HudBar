using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// ESRI/ArcGIS StreetMap
  /// 
  /// https://services.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}
  /// 
  /// V2 (not used so far)
  /// https://basemaps.arcgis.com/arcgis/rest/services/World_Basemap_v2/VectorTileServer
  /// Sources: Esri, HERE, Garmin, FAO, NOAA, USGS, © OpenStreetMap contributors, and the GIS User Community
  /// </summary>
  internal sealed class ESRI_StreetMap : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static ESRI_StreetMap Instance => lazy.Value;
    private static readonly Lazy<ESRI_StreetMap> lazy = new Lazy<ESRI_StreetMap>( ( ) => new ESRI_StreetMap( ) );
    private ESRI_StreetMap( )
      : base( MapProvider.ESRI_StreetMap )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Sources: Esri, HERE, Garmin, USGS, Intermap, INCREMENT P, NRCAN, Esri Japan, METI,\n"
                               + "Esri China( Hong Kong ), NOSTRA, © OpenStreetMap contributors, and the GIS User Community" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "https://services.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F34" );

    public override string Name { get; } = "ESRI World StreetMap";

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
