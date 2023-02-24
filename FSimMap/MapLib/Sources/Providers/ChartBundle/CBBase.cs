using CoordLib.MercatorTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// A Base Class to implement ChartBundle Servers
  /// see http://www.chartbundle.com/charts/
  /// </summary>
  internal abstract class CBBase : MapProviderBase
  {

    public CBBase( MapProvider mapProvider )
      : base( mapProvider )
    {
      RefererUrl = "";
      Copyright = string.Format( "Chartbundle.com and its data providers" );
      Name = ProviderIni.ProviderName( MapProvider );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D90" );

    public override string Name { get; } = "Chartbundle Tile Server";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion

  }
}
