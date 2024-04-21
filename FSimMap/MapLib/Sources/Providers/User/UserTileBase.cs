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
  /// A Base Class to implement User Tile Servers
  /// </summary>
  internal abstract class UserTileBase : MapProviderBase
  {

    public UserTileBase( MapProvider mapProvider )
      : base( mapProvider )
    {
      RefererUrl = "";
      // check for Overrides - MUST else it will not get anyting
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Copyright = string.Format( "User defined tileserver" );
      Name = ProviderIni.ProviderName( MapProvider );
    }

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319F30" );

    public override string Name { get; } = "User Tile Server";

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
