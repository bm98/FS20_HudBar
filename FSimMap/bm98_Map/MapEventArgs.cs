using System;

using CoordLib;

namespace bm98_Map
{
  /// <summary>
  /// UC_Map  EventArgs
  /// </summary>
  public class MapEventArgs : EventArgs
  {
    /// <summary>
    /// Center Coordinate of the drawn map
    /// </summary>
    public LatLon CenterCoordinate {get;set;}
    /// <summary>
    /// Range of the base map
    /// </summary>
    public MapRange MapRange { get; set; }

    /// <summary>
    /// ZoomLevel of the base map
    /// </summary>
    public ushort ZoomLevel { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public MapEventArgs( LatLon center, MapRange mapRange, ushort zoomLevel )
    {
      CenterCoordinate = center;
      MapRange = mapRange;
      ZoomLevel = zoomLevel;
    }

  }
}
