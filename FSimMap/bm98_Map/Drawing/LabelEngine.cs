using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Label Placement Engine
  /// 
  /// Based on a grid with 128x64 sized label fields
  /// 
  /// Register the Marks with a xy position
  /// Then compute the labels
  /// Then retrieve label drawing rectangles 
  /// </summary>
  internal class LabelEngine
  {
    // fixed label size for the engine
    private readonly Size c_labelSize = new Size( 96, 64 );
    private readonly Rectangle c_leftTop;
    private readonly Rectangle c_rightTop;
    private readonly Rectangle c_leftBottom;
    private readonly Rectangle c_rightBottom;

    // max index (not count....)
    private readonly int c_maxX = 0;
    private readonly int c_maxY = 0;

    // map to cover
    private Size _mapSize;

    // one managed Label 
    private class XLabel
    {
      /// <summary>
      /// Markers and their position on the Map
      /// </summary>
      public Dictionary<string, Point> Markers = new Dictionary<string, Point>( );
      /// <summary>
      /// True if occupied by either a Mark or a Label
      /// </summary>
      public bool Occupied = false;
      /// <summary>
      /// True if it is a Marker position
      /// </summary>
      public bool HasMark => Markers.Count > 0;
      /// <summary>
      /// The left top Label position of THIS label 
      /// </summary>
      public Point LabelPosition;

      // ctor: left top position
      public XLabel( int x, int y )
      {
        LabelPosition = new Point( x, y );
      }
    }
    // array of managed Rectangles
    private XLabel[,] _xLabels;

    // file of Markers with their Label Position (after Layout)
    private Dictionary<string, Point> _markers = new Dictionary<string, Point>( );

    /// <summary>
    /// cTor: create a LabelEngine for the provided Map
    /// </summary>
    /// <param name="mapSize"></param>
    public LabelEngine( Size mapSize )
    {
      _mapSize = mapSize;
      int width = _mapSize.Width / c_labelSize.Width;
      int height = _mapSize.Height / c_labelSize.Height;

      _xLabels = new XLabel[width, height];
      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          _xLabels[x, y] = new XLabel( x * c_labelSize.Width, y * c_labelSize.Height );
        }
      }
      // setup quadrant detectors
      c_leftTop = new Rectangle( new Point( 0, 0 ), c_labelSize.Multiply( 0.5f ) );
      c_rightTop = new Rectangle( new Point( c_labelSize.Width / 2, 0 ), c_labelSize.Multiply( 0.5f ) );
      c_leftBottom = new Rectangle( new Point( 0, c_labelSize.Height / 2 ), c_labelSize.Multiply( 0.5f ) );
      c_rightBottom = new Rectangle( new Point( c_labelSize.Width / 2, c_labelSize.Height / 2 ), c_labelSize.Multiply( 0.5f ) );
      // set max index
      c_maxX = width - 1;
      c_maxY = height - 1;
    }

    // Return the XLabel Grid Position from the Map position
    private Point GridPos( Point mapPos )
    {
      return new Point( mapPos.X / c_labelSize.Width, mapPos.Y / c_labelSize.Height );
    }

    // Return the XLabel left,top Map Position from the Grid position
    private Point MapPos( Point mapPos )
    {
      return new Point( mapPos.X * c_labelSize.Width, mapPos.Y * c_labelSize.Height );
    }

    // mark the XLabel pos for mapPos as occupied
    // returns false if not currently within the map
    private bool ReserveXPos( string key, Point mapPos )
    {
      var gp = GridPos( mapPos );
      // if not in the grid, ignore it...
      try {
        _xLabels[gp.X, gp.Y].Occupied = true;
        _xLabels[gp.X, gp.Y].Markers.Add( key, mapPos );
        return true;
      }
      catch {
        Debug.WriteLine( $"NOT ON MAP: {key}  {mapPos}" );
      }
      return false;
    }

    // return the point left of this point
    private Point LeftPt( Point point )
    {
      if (point.X > 0) return new Point( point.X - 1, point.Y ); // left is available
      return point; // same as input as we cannot go there
    }
    // return the point above of this point
    private Point UpPt( Point point )
    {
      if (point.Y > 0) return new Point( point.X, point.Y - 1 ); // above is available
      return point; // same as input as we cannot go there
    }
    // return the point right of this point
    private Point RightPt( Point point )
    {
      if (point.X < c_maxX) return new Point( point.X + 1, point.Y ); // right is available
      return point; // same as input as we cannot go there
    }
    // return the point below of this point
    private Point DownPt( Point point )
    {
      if (point.Y < c_maxY) return new Point( point.X, point.Y + 1 ); // below is available
      return point; // same as input as we cannot go there
    }

    // Get the label position for a Mark
    // Returns a unoccupied position depending on the Markers position
    // checks 4 points to be unoccupied before returning a position without further checking
    private Point GetLabelPos( string key, XLabel xLabel )
    {
      // map pos of the Marker
      var markerMapPos = xLabel.Markers[key];
      var origPoint = GridPos( xLabel.LabelPosition ); // start with the Marker Field and march to find a free one
      var labelPoint = origPoint;
      labelPoint = origPoint;
      labelPoint = UpPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      labelPoint = origPoint;
      labelPoint = DownPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      labelPoint = origPoint;
      labelPoint = LeftPt( labelPoint );
      labelPoint = UpPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      labelPoint = origPoint;
      labelPoint = RightPt( labelPoint );
      labelPoint = DownPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      labelPoint = origPoint;
      labelPoint = LeftPt( labelPoint );
      labelPoint = DownPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      labelPoint = origPoint;
      labelPoint = RightPt( labelPoint );
      labelPoint = UpPt( labelPoint );
      if (!_xLabels[labelPoint.X, labelPoint.Y].Occupied) return labelPoint;

      // did not found any free location, return the one below (if there is...)
      return DownPt( GridPos( xLabel.LabelPosition ) );
    }

    /// <summary>
    /// The engine will layout all Labels for the Markers
    /// and rewrite the _marker Dict with new Points for each Key
    /// </summary>
    public void Layout( )
    {
      // find all Marks and assign labels, checks line by line from top to bottom
      for (int x = 0; x < _xLabels.GetLength( 0 ); x++) {
        for (int y = 0; y < _xLabels.GetLength( 1 ); y++) {
          if (_xLabels[x, y].HasMark) {
            foreach (var key in _xLabels[x, y].Markers.Keys) {
              // returns a LabelPos within our own grid
              var labelPos = GetLabelPos( key, _xLabels[x, y] );
              _markers[key] = MapPos( labelPos );
              _xLabels[labelPos.X, labelPos.Y].Occupied = true;
            }
          }
        }
      }
    }

    /// <summary>
    /// Register a Marker at a position
    /// </summary>
    /// <param name="key">Key to retrieve the Label properties</param>
    /// <param name="position">The xy position of the marker on the map</param>
    public void RegisterMarker( string key, Point position )
    {
      if (_markers.ContainsKey( key )) throw new ArgumentException( $"Key {key} already exists" );

      // mark the Label Position as occupied by a Mark if it is within the map
      if (ReserveXPos( key, position )) {
        _markers.Add( key, position );
      }
    }

    /// <summary>
    /// Retrieve Label Rectangle for a Marker
    /// </summary>
    /// <param name="key">A Marker Key</param>
    /// <returns>A rectangle</returns>
    public Rectangle LabelPos( string key )
    {
      if (_markers.ContainsKey( key )) {
        // Get the Marker and it's label position
        return new Rectangle( _markers[key], c_labelSize );
      }
      // if not found, it was probably not within the map range..
      return new Rectangle( );
    }

  }
}
