using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 4th User TileServer
  /// </summary>
  internal sealed class UserTiles4Provider : UserTileBase
  {
    // Singleton Pattern
    public static UserTiles4Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles4Provider> lazy = new Lazy<UserTiles4Provider>( ( ) => new UserTiles4Provider( ) );

    private UserTiles4Provider( ) :
      base( MapProvider.USER_TILES_4 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 4" );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F34" );

    public override string Name { get; } = "User Tile Server 4";

    #endregion

  }
}
