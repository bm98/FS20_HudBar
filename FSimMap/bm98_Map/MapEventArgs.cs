using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Range or ZoomLevel of the base map
    /// </summary>
    public MapRange MapRange { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public MapEventArgs( LatLon center, MapRange mapRange )
    {
      CenterCoordinate = center;
      MapRange = mapRange;
    }
  }
}
