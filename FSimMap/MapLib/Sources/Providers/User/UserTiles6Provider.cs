using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 6th User TileServer
  /// </summary>
  internal sealed class UserTiles6Provider : UserTileBase
  {
    // Singleton Pattern
    public static UserTiles6Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles6Provider> lazy = new Lazy<UserTiles6Provider>( ( ) => new UserTiles6Provider( ) );

    private UserTiles6Provider( ) :
      base( MapProvider.USER_TILES_6 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 6" );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F36" );

    public override string Name { get; } = "User Tile Server 6";

    #endregion

  }
}
