using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 3rd User TileServer
  /// </summary>
  internal sealed class UserTiles1Provider : UserTileBase
  {
    // Singleton Pattern
    public static UserTiles1Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles1Provider> lazy = new Lazy<UserTiles1Provider>( ( ) => new UserTiles1Provider( ) );

    private UserTiles1Provider( ) :
      base( MapProvider.USER_TILES_1 )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "User defined Tile Server 1" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F31" );

    public override string Name { get; } = "User Tile Server 1";

    #endregion

    private readonly string UrlFormat = "http://userTiles1/{z}/{x}/{y}.png";

  }
}
