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
  /// A Base Class to implement User Tile Servers
  /// </summary>
  internal abstract class UserTileBase : MapProviderBase
  {

    public UserTileBase( MapProvider mapProvider )
      : base( mapProvider )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "User defined tileserver" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F30" );

    public override string Name { get; } = "User Tile Server";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

    private readonly string UrlFormat = "http://userTiles/{z}/{x}/{y}.png";

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
