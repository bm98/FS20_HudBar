using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib
{
  /// <summary>
  /// The MapImage discrimination
  /// </summary>
  public struct MapImageID
  {
    /// <summary>
    /// A Tile coordinate
    /// </summary>
    public TileXY TileXY;
    /// <summary>
    /// A Map Zoom Level
    /// </summary>
    public ushort ZoomLevel;
    /// <summary>
    /// A Map Provider (enum)
    /// </summary>
    public MapProvider MapProvider;

    /// <summary>
    /// cTor: to create from arguments
    /// </summary>
    /// <param name="tileXY">A TileXY</param>
    /// <param name="zoom">A Zoom level</param>
    /// <param name="mapProvider">A MapProvider Enum</param>
    public MapImageID( TileXY tileXY, ushort zoom, MapProvider mapProvider)
    {
      TileXY = tileXY;
      ZoomLevel = zoom;
      MapProvider = mapProvider;
    }

    //public string QuadKey => Tools.ToQuadKey( this.TileXY );

    /// <summary>
    /// Returns the Tile coordinate Zxy Key (Znn¦Xnnn¦Ynnnn)
    /// </summary>
    public string ZxyKey => Tools.ToZxyKey( this );
    /// <summary>
    /// Returns the Full Image Key (Provider¦Znn¦Xnnn¦Ynnnn)
    /// </summary>
    public string FullKey => Tools.ToFullKey( this );

    /// <summary>
    /// A string representation of this Object
    /// </summary>
    /// <returns></returns>
    public override string ToString( )
    {
      return FullKey;
    }

  }
}
