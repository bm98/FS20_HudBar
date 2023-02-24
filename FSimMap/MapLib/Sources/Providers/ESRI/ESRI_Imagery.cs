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
  /// ESRI/ArcGIS Imagery
  /// 
  /// https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}
  /// </summary>
  internal sealed class ESRI_Imagery : MapProviderBase
  {
    // Singleton Pattern
    public static ESRI_Imagery Instance => lazy.Value;
    private static readonly Lazy<ESRI_Imagery> lazy = new Lazy<ESRI_Imagery>( ( ) => new ESRI_Imagery( ) );
    private ESRI_Imagery( )
      : base( MapProvider.ESRI_Imagery )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Esri, Maxar, Earthstar Geographics, and the GIS User Community" );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
    }

    // default URL
    private const string DefaultUrl = "https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F31" );

    public override string Name { get; } = "ESRI World Imagery";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}
