using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 5th User TileServer
  /// </summary>
  internal sealed class UserTiles5Provider : UserTileBase
  {
    // Singleton Pattern
    public static UserTiles5Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles5Provider> lazy = new Lazy<UserTiles5Provider>( ( ) => new UserTiles5Provider( ) );

    private UserTiles5Provider( ) :
      base( MapProvider.USER_TILES_5 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 5" );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F35" );

    public override string Name { get; } = "User Tile Server 5";

    #endregion

  }
}
