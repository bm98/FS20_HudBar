using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  internal class Bing_Imagery : MapProviderBase
  {
    // Singleton Pattern
    public static Bing_Imagery Instance => lazy.Value;
    private static readonly Lazy<Bing_Imagery> lazy = new Lazy<Bing_Imagery>( ( ) => new Bing_Imagery( ) );
    private Bing_Imagery( )
      : base( MapProvider.BING_Imagery )
    {
      var im = BingManager.GetImMetaData( _imagery ); // trigger loading of MetaData
    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F313F31" );

    public override string Name { get; } = "Bing Imagery";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, string.Empty );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    public override string Copyright {
      get {
        var im = BingManager.GetImMetaData( _imagery );
        if (im != null) return im.Copyright;
        return BingManager.DefaultCopyright;

      }
    }

    #endregion

    private BingMapsRESTToolkit.ImageryType _imagery = BingMapsRESTToolkit.ImageryType.Aerial;

    private readonly string UrlFormat = "";

    string MakeTileImageUrl( TileXY tileXY, ushort zoom, string language )
    {
      var imMeta = BingManager.GetImMetaData( _imagery );
      if (imMeta == null) return ""; // cannot...
      // have to have the MetaData available 

      string url = imMeta.GetTileUrl( tileXY, zoom, language );
      return url;
    }

  }
}