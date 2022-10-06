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
  /// ESRI/ArcGIS Imagery
  /// 
  /// https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}
  /// </summary>
  internal sealed class ESRI_Imagery : MapProviderBase
  {
    // Singleton Pattern
    public static ESRI_Imagery Instance => lazy.Value;
    private static readonly Lazy<ESRI_Imagery> lazy = new Lazy<ESRI_Imagery>( ( ) => new ESRI_Imagery( ) );
    private ESRI_Imagery( )
      : base( MapProvider.ESRI_Imagery )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "Esri, Maxar, Earthstar Geographics, and the GIS User Community" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }

    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F31" );

    public override string Name { get; } = "ESRI World Imagery";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}";

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
