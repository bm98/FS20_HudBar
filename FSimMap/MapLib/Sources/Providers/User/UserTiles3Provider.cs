using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement the 3rd User TileServer
  /// </summary>
  internal sealed class UserTiles3Provider : UserTileBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static UserTiles3Provider Instance => lazy.Value;
    private static readonly Lazy<UserTiles3Provider> lazy = new Lazy<UserTiles3Provider>( ( ) => new UserTiles3Provider( ) );

    private UserTiles3Provider( ) :
      base( MapProvider.USER_TILES_3 )
    {
      // set only distict items here - the rest is done in the base class
      Copyright = string.Format( "User defined Tile Server 3" );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F33" );

    public override string Name { get; } = "User Tile Server 3";

    #endregion

  }
}

