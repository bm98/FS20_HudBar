using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  internal class Bing_OSMap : MapProviderBase
  {
    // Singleton Pattern
    public static Bing_OSMap Instance => lazy.Value;
    private static readonly Lazy<Bing_OSMap> lazy = new Lazy<Bing_OSMap>( ( ) => new Bing_OSMap( ) );
    private Bing_OSMap( )
      : base( MapProvider.BING_OStreetMap )
    {
      var im = BingManager.GetImMetaData( _imagery ); // trigger loading of MetaData
      Copyright = (im != null) ? im.Copyright : BingManager.DefaultCopyright;
      Name = ProviderIni.ProviderName( MapProvider );
    }

    // Type of map to retrieve
    private const BingMapsRESTToolkit.ImageryType _imagery = BingMapsRESTToolkit.ImageryType.Road;

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F313F33" );

    public override string Name { get; } = "Bing Roadmap";

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

