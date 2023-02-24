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
      Copyright = (im != null) ? im.Copyright : BingManager.DefaultCopyright;
      Name = ProviderIni.ProviderName( MapProvider );
    }

    // Type of map to retrieve
    private const BingMapsRESTToolkit.ImageryType _imagery = BingMapsRESTToolkit.ImageryType.AerialWithLabels;

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F313F32" );

    public override string Name { get; } = "Bing Imagery with Labels";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      string url = BingManager.MakeBingTileImageUrl( _imagery, mapImageID.TileXY, z );// TODO RE ADD LANGUAGE
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}

