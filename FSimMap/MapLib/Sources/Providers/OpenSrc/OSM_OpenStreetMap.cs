using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// OSM OpenStreetMap
  /// 
  ///  	https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png
  /// </summary>
  internal sealed class OSM_OpenStreetMap : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static OSM_OpenStreetMap Instance => lazy.Value;
    private static readonly Lazy<OSM_OpenStreetMap> lazy = new Lazy<OSM_OpenStreetMap>( ( ) => new OSM_OpenStreetMap( ) );
    private OSM_OpenStreetMap( )
      : base( MapProvider.OSM_OpenStreetMap )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Map tiles © OpenStreetMap contributors" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    // default URL
    private const string DefaultUrl = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    //https://{s}.tile.openstreetmap.de/{z}/{x}/{y}.png   de for german, rather than localized Names

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6DD6F319F34" );

    public override string Name { get; } = "OSM OpenStreetMap";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      // supports server in the URL
      char letter = "abc"[Tools.GetServerNum( mapImageID.TileXY, 3 )];
      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, Convert.ToString( letter ), "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}
