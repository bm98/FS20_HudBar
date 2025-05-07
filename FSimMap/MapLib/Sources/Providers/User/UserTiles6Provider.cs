using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 6th User TileServer
  /// </summary>
  internal sealed class UserTiles6Provider : UserTileBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static UserTiles6Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles6Provider> lazy = new Lazy<UserTiles6Provider>( ( ) => new UserTiles6Provider( ) );

    private UserTiles6Provider( ) :
      base( MapProvider.USER_TILES_6 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 6" );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F36" );

    public override string Name { get; } = "User Tile Server 6";

    #endregion

  }
}
