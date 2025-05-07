using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 2nd User TileServer
  /// </summary>
  internal sealed class UserTiles2Provider : UserTileBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static UserTiles2Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles2Provider> lazy = new Lazy<UserTiles2Provider>( ( ) => new UserTiles2Provider( ) );

    private UserTiles2Provider( ) :
      base( MapProvider.USER_TILES_2 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 2" );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F32" );

    public override string Name { get; } = "User Tile Server 2";

    #endregion

  }
}
