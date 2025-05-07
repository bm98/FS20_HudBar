using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// CARTO dark no labels
  /// 
  /// http://{s}.basemaps.cartocdn.com/dark_nolabels/{z}/{x}/{y}.png
  /// 
  /// </summary>
  internal sealed class CARTO_DarkNL : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static CARTO_DarkNL Instance => lazy.Value;
    private static readonly Lazy<CARTO_DarkNL> lazy = new Lazy<CARTO_DarkNL>( ( ) => new CARTO_DarkNL( ) );
    private CARTO_DarkNL( )
      : base( MapProvider.CARTO_DarkNL )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "(c) CARTO (dark no labels)" );
      MaxZoom = 13;
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "http://{s}.basemaps.cartocdn.com/dark_nolabels/{z}/{x}/{y}.png";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6CC6F319F52" );

    public override string Name { get; } = "CARTO Dark no Labels";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      char letter = "abc"[Tools.GetServerNum( mapImageID.TileXY, 3 )];
      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z!=mapImageID.ZoomLevel) {
        return null; 
      } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, Convert.ToString( letter ), "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}
