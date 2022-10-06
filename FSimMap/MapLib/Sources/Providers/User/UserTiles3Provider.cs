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
  internal sealed class UserTiles3Provider : UserTileBase
  {
    // Singleton Pattern
    public static UserTiles3Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles3Provider> lazy = new Lazy<UserTiles3Provider>( ( ) => new UserTiles3Provider( ) );

    private UserTiles3Provider( ) :
      base( MapProvider.USER_TILES_3 )
    {
      RefererUrl = UrlFormat;
      Copyright = string.Format( "User defined Tile Server 3" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        UrlFormat = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F33" );

    public override string Name { get; } = "User Tile Server 3";

    #endregion

    private readonly string UrlFormat = "http://userTiles1/{z}/{x}/{y}.png";

  }
}

