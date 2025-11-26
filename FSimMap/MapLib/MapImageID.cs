using System;

using CoordLib.MercatorTiles;

namespace MapLib
{
  /// <summary>
  /// The MapImage discrimination
  /// </summary>
  public struct MapImageID : IEquatable<MapImageID>, IComparable<MapImageID>
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
    public MapImageID( TileXY tileXY, ushort zoom, MapProvider mapProvider )
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
    /// True for a valid ID
    /// </summary>
    public bool IsValid => !string.IsNullOrEmpty( FullKey );


    /// <inheritdoc/>
    public int CompareTo( MapImageID other )
    {
      if (other == default) return 1;
      if (this == default) return -1;
      if (this.Equals( other )) return 0;

      if (this.ZoomLevel > other.ZoomLevel) return -1;
      if (this.ZoomLevel < other.ZoomLevel) return 1;

      if (this.TileXY.X > other.TileXY.X) return 1;
      if (this.TileXY.X < other.TileXY.X) return -1;
      if (this.TileXY.Y > other.TileXY.Y) return 1;
      if (this.TileXY.Y < other.TileXY.Y) return -1;

      return 0;
    }

    /// <inheritdoc/>
    public bool Equals( MapImageID other )
    {
      return (this.TileXY == other.TileXY)
         && (this.ZoomLevel == other.ZoomLevel)
         && (this.MapProvider == other.MapProvider);
    }

    /// <inheritdoc/>
    public static bool operator ==( MapImageID iD, MapImageID other )
    {
      return iD.Equals( other );
    }
    /// <inheritdoc/>
    public static bool operator !=( MapImageID iD, MapImageID other )
    {
      return !iD.Equals( other );
    }

    /// <inheritdoc/>
    public override bool Equals( object obj )
    {
      if (!(obj is MapImageID)) return false;

      return this.Equals( obj );
    }

    /// <inheritdoc/>
    public override int GetHashCode( )
    {
      return TileXY.GetHashCode( ) ^ ZoomLevel.GetHashCode( ) ^ MapProvider.GetHashCode( ) ^ 32167;
    }

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
