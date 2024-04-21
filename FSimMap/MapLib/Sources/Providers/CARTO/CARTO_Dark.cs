using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// CARTO dark  'Dark Matter'
  /// 
  /// http://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png
  /// 
  /// </summary>
  internal sealed class CARTO_Dark : MapProviderBase
  {
    // Singleton Pattern
    public static CARTO_Dark Instance => lazy.Value;
    private static readonly Lazy<CARTO_Dark> lazy = new Lazy<CARTO_Dark>( ( ) => new CARTO_Dark( ) );
    private CARTO_Dark( )
      : base( MapProvider.CARTO_Dark )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "(c) CARTO (Dark Matter)" );
      MaxZoom = 13;
    }

    // default URL
    private const string DefaultUrl = "http://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6CC6F319F51" );

    public override string Name { get; } = "CARTO Dark Matter";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      char letter = "abc"[Tools.GetServerNum( mapImageID.TileXY, 3 )];
      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, Convert.ToString( letter ), "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}
