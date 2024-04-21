using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// FAA/ArcGIS Imagery
  /// 
  /// https://faa.maps.arcgis.com/home/item.html?id=ba711890a6f24720bf63a77c5aeb1e64
  /// 
  /// https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/VFR_Terminal/MapServer/tile/{z}/{y}/{x}?cacheKey=85a564cdecfa1f12
  /// ZOOM 10..12
  /// </summary>
  internal sealed class FAA_VFR_Terminal : MapProviderBase
  {
    // Singleton Pattern
    public static FAA_VFR_Terminal Instance => lazy.Value;
    private static readonly Lazy<FAA_VFR_Terminal> lazy = new Lazy<FAA_VFR_Terminal>( ( ) => new FAA_VFR_Terminal( ) );
    private FAA_VFR_Terminal( )
      : base( MapProvider.FAA_VFR_Terminal )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Federal Aviation Administration, Aeronautical Information Services" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
      MinZoom = 10;
      MaxZoom = 12;
    }

    // default URL
    private const string DefaultUrl = "https://tiles.arcgis.com/tiles/ssFJjBXIUyZDrSYZ/arcgis/rest/services/VFR_Terminal/MapServer/tile/{z}/{y}/{x}?cacheKey=85a564cdecfa1f12";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F41" );

    public override string Name { get; } = "FAA VFR Terminal";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      ushort z = ZoomCheck( mapImageID.ZoomLevel );
      if (z != mapImageID.ZoomLevel) { return null; } // not allowed 

      string url = MakeTileImageUrl( mapImageID.TileXY, z, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}
