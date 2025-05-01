using System.Drawing;

using CoordLib;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// VPort Interface for Painting
  /// </summary>
  internal interface IVPortPaint
  {

    /// <summary>
    /// Get the Heading the map North UP 
    /// </summary>
    float MapHeading { get; }

    /// <summary>
    /// Returns the LatLon coordinate point which is currently at the center of the Viewport
    /// </summary>
    LatLon ViewCenterLatLon { get; }

    /// <summary>
    /// Size of the drawing canvas in pixel
    /// </summary>
    Size CanvasSize { get; }
    /// <summary>
    /// Size of the output View (control client size)
    /// </summary>
    Size ViewportSize { get; }

    /// <summary>
    /// The rectangle mapped from the drawing canvas into the Viewport View [pixel]
    ///  where the Location is the top,left point where the mappin starts
    ///  where the Size is the Viewport Output window size
    /// </summary>
    Rectangle ViewPortView { get; }

    /// <summary>
    /// Function submitted for Drawing
    /// 
    /// Transform a MapCoordinate into Drawing Canvase Coordinates
    /// </summary>
    /// <param name="coordPoint">A Map coordinate</param>
    /// <returns>A Drawing Canvas Point</returns>
    Point MapToCanvasPixel( LatLon coordPoint );

    /// <summary>
    /// Map a Canvas (matrix) point to the VPort Pixel Point
    /// </summary>
    /// <param name="matrixPoint">A Canvas pixel</param>
    /// <returns>A ViewPort point</returns>
    Point MatrixPixelToVPixel( Point matrixPoint );

    /// <summary>
    /// Map a VPort Pixel Point to the Canvas (matrix) point
    /// </summary>
    /// <param name="vPoint">A ViewPort pointl</param>
    /// <returns>A Canvas pixel</returns>
    Point VPixelToMatrixPixel( Point vPoint );

  }
}
