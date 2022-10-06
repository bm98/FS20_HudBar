using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  internal class Bing_ImageryLabels : MapProviderBase
  {
    // Singleton Pattern
    public static Bing_ImageryLabels Instance => lazy.Value;
    private static readonly Lazy<Bing_ImageryLabels> lazy = new Lazy<Bing_ImageryLabels>( ( ) => new Bing_ImageryLabels( ) );
    private Bing_ImageryLabels( )
      : base( MapProvider.BING_ImageryLabels )
    {
      var im = BingManager.GetImMetaData( _imagery ); // trigger loading of MetaData
    }

    #region ProviderBase Members

    public override string ContentID => UrlFormat; // use the URL as ID

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F313F32" );

    public override string Name { get; } = "Bing Imagery with Labels";

    public override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, "" ); // TODO RE ADD LANGUAGE
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

    private BingMapsRESTToolkit.ImageryType _imagery = BingMapsRESTToolkit.ImageryType.AerialWithLabels;

    private readonly string UrlFormat = "";

    string MakeTileImageUrl( TileXY tileXY, ushort zoom, string language )
    {
      var imMeta = BingManager.GetImMetaData( _imagery );
      if (imMeta == null) return ""; // cannot...

      string url = imMeta.GetTileUrl( tileXY, zoom, language );
      return url;
    }

  }
}

