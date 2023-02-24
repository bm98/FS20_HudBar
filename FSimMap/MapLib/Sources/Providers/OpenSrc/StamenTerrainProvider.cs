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
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Map tiles by Stamen Design, under CC BY 3.0. Data by OpenStreetMap, under ODbL.", DateTime.Today.Year );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
    }

    /*
      Many applications and libraries understand the notion of map URL templates. These are ours:
          https://stamen-tiles.a.ssl.fastly.net/toner/{z}/{x}/{y}.png
          https://stamen-tiles.a.ssl.fastly.net/terrain/{z}/{x}/{y}.jpg
          https://stamen-tiles.a.ssl.fastly.net/watercolor/{z}/{x}/{y}.jpg
      Multiple subdomains can be also be used: https://stamen-tiles-{S}.a.ssl.fastly.net
      JavaScript can be loaded from https://stamen-maps.a.ssl.fastly.net/js/tile.stamen.js.
      If you need protocol-agnostic URLs, use //stamen-tiles-{s}.a.ssl.fastly.net/, as that endpoint will work for both SSL and non-SSL connections.     
     */
    // default URL
    private const string DefaultUrl = "https://stamen-tiles-{s}.a.ssl.fastly.net/terrain/{z}/{x}/{y}.jpg";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F419F76" );

    public override string Name { get; } = "Stamen Terrain Map";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      // supports server in the URL
      char letter = "abcd"[Tools.GetServerNum( mapImageID.TileXY, 4 )];
      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, Convert.ToString( letter ), "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}

