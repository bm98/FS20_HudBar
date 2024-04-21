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
  /// OpenTopoMap (CC-BY-SA)
  /// 
  /// https://a.tile.opentopomap.org/{z}/{x}/{y}.png
  /// </summary>
  internal sealed class OpenTopoProvider : MapProviderBase
  {
    // Singleton Pattern
    public static OpenTopoProvider Instance => lazy.Value;
    private static readonly Lazy<OpenTopoProvider> lazy = new Lazy<OpenTopoProvider>( ( ) => new OpenTopoProvider( ) );
    private OpenTopoProvider( )
      : base( MapProvider.OpenTopo )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Kartendaten: © OpenStreetMap-Mitwirkende, SRTM | Kartendarstellung: ©{0} OpenTopoMap (CC-BY-SA)", DateTime.Today.Year );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
    }

    /*
        Die Kacheln können unter folgendem Pfad abgerufen werden:
        https://{a|b|c}.tile.opentopomap.org/{z}/{x}/{y}.png
    */
    // default URL
    private const string DefaultUrl = "https://{s}.tile.opentopomap.org/{z}/{x}/{y}.png";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F419F66" );

    public override string Name { get; } = "Open Topo Map";

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
