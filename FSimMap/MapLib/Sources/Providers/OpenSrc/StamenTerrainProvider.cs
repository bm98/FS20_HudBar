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
  /// Stamen Terrain by Stamen Design, under CC BY 3.0. Data by OpenStreetMap, under ODbL.
  /// 
  /// https://stamen.com/
  /// </summary>
  internal sealed class StamenTerrainProvider : MapProviderBase
  {
    // Singleton Pattern
    public static StamenTerrainProvider Instance => lazy.Value;
    private static readonly Lazy<StamenTerrainProvider> lazy = new Lazy<StamenTerrainProvider>( ( ) => new StamenTerrainProvider( ) );
    private StamenTerrainProvider( )
      : base( MapProvider.Stamen_Terrain )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "Map tiles by Stamen Design, under CC BY 3.0. Data by OpenStreetMap, under ODbL.", DateTime.Today.Year );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F419F76" );

    public override string Name { get; } = "Stamen Terrain Map";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "https://stamen-tiles.a.ssl.fastly.net/terrain/{z}/{x}/{y}.jpg";

    string MakeTileImageUrl( TileXY tileXY, ushort zoom, string language )
    {
      string url = UrlFormat;

      url = url.Replace( "{z}", "{0}" );
      url = url.Replace( "{x}", "{1}" );
      url = url.Replace( "{y}", "{2}" );

      return string.Format( url, zoom, tileXY.X, tileXY.Y );

    }
  }
}

