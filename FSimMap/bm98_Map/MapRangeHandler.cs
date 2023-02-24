using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map
{
  /// <summary>
  /// The Zoom Level of the native Map 
  /// </summary>
  public enum MapRange
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    XFar = 7,  // and smaller
    FarFar = 9,
    Far = 10,
    Mid = 12,
    Near = 13,
    Close = 15,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Manages MapRange aka Zoom Levels
  /// </summary>
  internal class MapRangeHandler
  {
    // zoom limits
    private const ushort c_minZoom = 5;
    private const ushort c_maxZoom = 15;

    // master ZoomLevel
    private ushort _zoomLevel = 12;
    // ZoomLevel change handler (takes the ZoomLevel as argument)
    private Action<ushort> _setZoomLevel;


    /// <summary>
    /// The MapRange
    /// </summary>
    public MapRange MapRange => RangeFromZoom( _zoomLevel );

    /// <summary>
    /// The Zoom Level
    /// </summary>
    public ushort ZoomLevel => _zoomLevel;

    public bool IsXFar => RangeFromZoom( _zoomLevel ) < MapRange.FarFar;
    public bool IsFarFar => (RangeFromZoom( _zoomLevel ) < MapRange.Far) && (RangeFromZoom( _zoomLevel ) >= MapRange.FarFar);
    public bool IsFar => (RangeFromZoom( _zoomLevel ) < MapRange.Mid) && (RangeFromZoom( _zoomLevel ) >= MapRange.Far);
    public bool IsMid => (RangeFromZoom( _zoomLevel ) < MapRange.Near) && (RangeFromZoom( _zoomLevel ) >= MapRange.Mid);
    public bool IsNear => (RangeFromZoom( _zoomLevel ) < MapRange.Close) && (RangeFromZoom( _zoomLevel ) >= MapRange.Near);
    public bool IsClose => RangeFromZoom( _zoomLevel ) >= MapRange.Close;

    /// <summary>
    /// Returns the zoomLevel from the MapRange
    /// </summary>
    /// <param name="mapRange">A MapRange</param>
    /// <returns>The ZoomLevel</returns>
    public static ushort ZoomFromRange( MapRange mapRange ) => (ushort)((int)mapRange % 100);// used if MapRange gets explicite levels +100

    /// <summary>
    /// Returns the designated MapRange from the current ZoomLevel
    /// </summary>
    /// <param name="zoomLevel">The zoomLevel</param>
    /// <returns>A MapRange</returns>
    public static MapRange RangeFromZoom( ushort zoomLevel )
    {
      if (zoomLevel < (ushort)MapRange.FarFar) return MapRange.XFar;
      if ((zoomLevel < (ushort)MapRange.Far) && (zoomLevel >= (ushort)MapRange.FarFar)) return MapRange.FarFar;
      if ((zoomLevel < (ushort)MapRange.Mid) && (zoomLevel >= (ushort)MapRange.Far)) return MapRange.Far;
      if ((zoomLevel < (ushort)MapRange.Near) && (zoomLevel >= (ushort)MapRange.Mid)) return MapRange.Mid;
      if ((zoomLevel < (ushort)MapRange.Close) && (zoomLevel >= (ushort)MapRange.Near)) return MapRange.Near;
      return MapRange.Close;
    }


    /// <summary>
    /// cTor: init with a range
    /// </summary>
    /// <param name="mapRange">The mapRange</param>
    /// <param name="SetZoomLevel">An Action to set the zoomLevel if the MapRange changes</param>
    public MapRangeHandler( MapRange mapRange, Action<ushort> SetZoomLevel )
    {
      _setZoomLevel = SetZoomLevel;
      _zoomLevel = ZoomFromRange( mapRange ); // init without calling the action
    }

    /// <summary>
    /// Set a MapRange
    /// </summary>
    /// <param name="mapRange">The MapRange</param>
    public void SetMapRange( MapRange mapRange )
    {
      var zl = ZoomFromRange( mapRange );

      if (zl != _zoomLevel) {
        // if changed
        _zoomLevel = zl;
        _setZoomLevel( _zoomLevel );
      }
    }

    /// <summary>
    /// Set a ZoomLevel
    /// </summary>
    /// <param name="zoomLevel">The zoomLevel</param>
    /// <returns>True if the level changed</returns>
    public bool SetZoomLevel( ushort zoomLevel )
    {
      if (zoomLevel != _zoomLevel) {
        if (zoomLevel < c_minZoom) return false;
        if (zoomLevel > c_maxZoom) return false;

        // if changed
        _zoomLevel = zoomLevel;
        _setZoomLevel( _zoomLevel );
        return true;
      }
      return false;
    }

    /// <summary>
    /// Increment the ZoomLevel
    /// </summary>
    /// <returns>True if the level changed</returns>
    public bool IncZoomLevel( )
    {
     return SetZoomLevel( (ushort)(_zoomLevel + 1) );
    }

    /// <summary>
    /// Decrement the ZoomLevel
    /// </summary>
    /// <returns>True if the level changed</returns>
    public bool DecZoomLevel( )
    {
      return SetZoomLevel( (ushort)(_zoomLevel - 1) );
    }

    /// <summary>
    /// True if another zoom in is possible
    /// </summary>
    public bool CanIncZoomLevel => _zoomLevel < c_maxZoom;

    /// <summary>
    /// True if another zoom out is possible
    /// </summary>
    public bool CanDecZoomLevel => _zoomLevel > c_minZoom;

  }

  // extensions
  internal static class MRH_extensions
  {
    public static bool IsLess( this ushort _zl, MapRange mapRange ) => _zl < MapRangeHandler.ZoomFromRange( mapRange );
    public static bool IsLessOrEqual( this ushort _zl, MapRange mapRange ) => _zl <= MapRangeHandler.ZoomFromRange( mapRange );
    public static bool IsGreater( this ushort _zl, MapRange mapRange ) => _zl > MapRangeHandler.ZoomFromRange( mapRange );
    public static bool IsGreaterOrEqual( this ushort _zl, MapRange mapRange ) => _zl >= MapRangeHandler.ZoomFromRange( mapRange );
  }

}
