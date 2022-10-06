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
  /// OSM OpenStreetMap
  /// 
  ///  	https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png
  /// </summary>
  internal sealed class OSM_OpenStreetMap : MapProviderBase
  {    // Singleton Pattern
    public static OSM_OpenStreetMap Instance => lazy.Value;
    private static readonly Lazy<OSM_OpenStreetMap> lazy = new Lazy<OSM_OpenStreetMap>( ( ) => new OSM_OpenStreetMap( ) );
    private OSM_OpenStreetMap( )
      : base( MapProvider.OSM_OpenStreetMap )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "Map tiles © OpenStreetMap contributors" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }

    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6DD6F319F34" );

    public override string Name { get; } = "OSM OpenStreetMap";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    //https://{s}.tile.openstreetmap.de/{z}/{x}/{y}.png   de for german, rather than localized Names

    string MakeTileImageUrl( TileXY tileXY, ushort zoom, string language )
    {
      string url = UrlFormat;
      char letter = "abc"[Tools.GetServerNum( tileXY, 3 )];

      url = url.Replace( "{z}", "{0}" );
      url = url.Replace( "{x}", "{1}" );
      url = url.Replace( "{y}", "{2}" );
      url = url.Replace( "{s}", "{3}" );

      return string.Format( url, zoom, tileXY.X, tileXY.Y, letter );

    }
  }
}
