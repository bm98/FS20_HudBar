using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 3rd User TileServer
  /// </summary>
  internal sealed class UserTiles1Provider : UserTileBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static UserTiles1Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles1Provider> lazy = new Lazy<UserTiles1Provider>( ( ) => new UserTiles1Provider( ) );

    private UserTiles1Provider( ) :
      base( MapProvider.USER_TILES_1 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 1" );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F31" );

    public override string Name { get; } = "User Tile Server 1";

    #endregion

  }
}
