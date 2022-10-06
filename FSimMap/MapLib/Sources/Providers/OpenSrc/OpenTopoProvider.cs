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
  /// OpenTopoMap (CC-BY-SA)
  /// 
  /// https://tile.opentopomap.org/{z}/{x}/{y}.png
  /// </summary>
  internal sealed class OpenTopoProvider : MapProviderBase
  {
    // Singleton Pattern
    public static OpenTopoProvider Instance => lazy.Value;
    private static readonly Lazy<OpenTopoProvider> lazy = new Lazy<OpenTopoProvider>( ( ) => new OpenTopoProvider( ) );
    private OpenTopoProvider( )
      : base( MapProvider.OpenTopo )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "Kartendaten: © OpenStreetMap-Mitwirkende, SRTM | Kartendarstellung: ©{0} OpenTopoMap (CC-BY-SA)", DateTime.Today.Year );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }


    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F419F66" );

    public override string Name { get; } = "Open Topo Map";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl(mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "https://tile.opentopomap.org/{z}/{x}/{y}.png";

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
