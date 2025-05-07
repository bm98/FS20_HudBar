using System;

using DbgLib;

namespace MapLib.Sources.Providers
{
  internal class Bing_Imagery : MapProviderBase
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static Bing_Imagery Instance => lazy.Value;
    private static readonly Lazy<Bing_Imagery> lazy = new Lazy<Bing_Imagery>( ( ) => new Bing_Imagery( ) );
    private Bing_Imagery( )
      : base( MapProvider.BING_Imagery )
    {
     // var im = BingManager.GetImMetaData( _imagery ); // trigger loading of MetaData
      Copyright = BingManager.DefaultCopyright;
      Name = ProviderIni.ProviderName( MapProvider );
      LOG.Info( "MAP-CONFIG", RefererUrl );
    }

    /// <summary>
    /// Update once the Bing manager obtained the properties
    /// </summary>
    internal void UpdateFromIM( )
    {
      var im = BingManager.GetImMetaData( _imagery ); // trigger loading of MetaData
      if (im != null) {
        Copyright = im.Copyright;
        MinZoom = im.MinZoom;
        MaxZoom = im.MaxZoom;
        _imLoadingDone = true;
      }
    }

    // Type of map to retrieve
    private const BingMapsRESTToolkit.ImageryType _imagery = BingMapsRESTToolkit.ImageryType.Aerial;

    private bool _imLoadingDone = false;

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F313F31" );

    public override string Name { get; } = "Bing Imagery";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      if (!_imLoadingDone) {
        UpdateFromIM( );
        return null;
      }


      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = BingManager.MakeBingTileImageUrl( _imagery, mapImageID.TileXY, z );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}