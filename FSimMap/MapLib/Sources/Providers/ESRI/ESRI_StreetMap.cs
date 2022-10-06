using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

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
    // Singleton Pattern
    public static ESRI_StreetMap Instance => lazy.Value;
    private static readonly Lazy<ESRI_StreetMap> lazy = new Lazy<ESRI_StreetMap>( ( ) => new ESRI_StreetMap( ) );
    private ESRI_StreetMap( )
      : base( MapProvider.ESRI_StreetMap )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "Sources: Esri, HERE, Garmin, USGS, Intermap, INCREMENT P, NRCAN, Esri Japan, METI,\n"
                               + "Esri China( Hong Kong ), NOSTRA, © OpenStreetMap contributors, and the GIS User Community" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }


    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F34" );

    public override string Name { get; } = "ESRI World StreetMap";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "https://services.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}";

    string MakeTileImageUrl( TileXY tileXY, uint zoom, string language )
    {
      string url = UrlFormat;

      url = url.Replace( "{z}", "{0}" );
      url = url.Replace( "{x}", "{1}" );
      url = url.Replace( "{y}", "{2}" );

      return string.Format( url, zoom, tileXY.X, tileXY.Y );

    }
  }
}
