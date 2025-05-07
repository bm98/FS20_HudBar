using System;

using DbgLib;

namespace MapLib.Sources.Providers
{

  /// <summary>
  /// By Stadia Maps, Stamen Terrain by Stamen Design, under CC BY 3.0. Data by OpenStreetMap, under ODbL.
  /// 
  /// https://stamen.com/
  /// 
  /// As of Mid 2023
  ///  served by  Stadia Maps (https://docs.stadiamaps.com/)
  /// </summary>
  internal sealed class StamenTerrainProvider : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static StamenTerrainProvider Instance => lazy.Value;
    private static readonly Lazy<StamenTerrainProvider> lazy = new Lazy<StamenTerrainProvider>( ( ) => new StamenTerrainProvider( ) );
    private StamenTerrainProvider( )
      : base( MapProvider.Stamen_Terrain )
    {

      RefererUrl = DefaultUrl + $"?api_key={StadiaStamenKey}"; // add key;
      Copyright = string.Format( "By Stadia Maps, Stamen Design, OpenMapTiles contributors under CC BY 3.0. Data by OpenStreetMap, under ODbL.", DateTime.Today.Year );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }
    // the key
    private static string StadiaStamenKey => MapProviderBase.ProviderIni.StadiaStamenKey;

    /*
     Stadia Maps:
       terrain 	https://tiles.stadiamaps.com/tiles/stamen_terrain/{z}/{x}/{y}{r}.png?api_key=YOUR-API-KEY 	
     */
    // default URL (May 2025 Update parameter {r} required = @1x)
    private const string DefaultUrl = "https://tiles.stadiamaps.com/tiles/stamen_terrain/{z}/{x}/{y}@1x.png";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F419F76" );

    public override string Name { get; } = "Stamen Terrain Map";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      // supports server in the URL
      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}

