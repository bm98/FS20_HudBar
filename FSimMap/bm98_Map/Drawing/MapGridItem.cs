using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using MapLib.Tiles;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// To draw the Map Grid
  /// </summary>
  internal class MapGridItem : DisplayItem
  {

    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    public StringFormat StringFormatLat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };
    public StringFormat StringFormatLon { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };


    /// <summary>
    /// The Tile matrix in use
    /// </summary>
    public TileMatrix TileMatrixRef { get; set; }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public MapGridItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public MapGridItem( MapGridItem other )
      : base( other )
    {
      this.StringFormatLon = other.StringFormatLon.Clone( ) as StringFormat;
      this.StringFormatLat = other.StringFormatLat.Clone( ) as StringFormat;
    }

    // Round by 0.5 increments into the visible range of the map
    private float RoundToVisible( double num, bool inc )
    {
      if (num == 0) return 0; // easy way out

      // inc-> round 'upwards' in a Map sense (given the bottom Latitude or left Longitude)
      //   - for negative numbers towards Zero
      //   - for positive numbers away from Zero
      // dec-> round 'downwards' in a Map sense (given the top Latitude or right Longitude)
      //   - for negative numbers away from Zero
      //   - for positive numbers towards Zero

      var s = Math.Sign( num );
      double b = Math.Abs( num ) * 2.0; // extend

      if (inc) {
        //   - for negative numbers towards Zero  - for positive numbers away from Zero
        b = (s < 0) ? Math.Floor( b ) : Math.Ceiling( b );
      }
      else {//dec
            //   - for negative numbers away from Zero - for positive numbers towards Zero
        b = (s < 0) ? Math.Ceiling( b ) : Math.Floor( b );
      }
      return s * (float)b / 2.0f; // contract to get 0.5 increments
    }

    /// <summary>
    /// Draw a Map Scale
    /// </summary>
    protected override void PaintThis( Graphics g, Func<CoordLib.LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      // rounded towards a 0.5 deg number within the map
      LatLon lt = new LatLon( RoundToVisible( TileMatrixRef.LeftTop_coord.Lat, false ), RoundToVisible( TileMatrixRef.LeftTop_coord.Lon, true ) );
      LatLon lb = new LatLon( RoundToVisible( TileMatrixRef.LeftBottom_coord.Lat, true ), RoundToVisible( TileMatrixRef.LeftBottom_coord.Lon, true ) );
      LatLon rt = new LatLon( RoundToVisible( TileMatrixRef.RightTop_coord.Lat, false ), RoundToVisible( TileMatrixRef.RightTop_coord.Lon, false ) );
      // LatLon rb = new LatLon( RoundToVisible( TileMatrixRef.RightBottom_coord.Lat, true ), RoundToVisible( TileMatrixRef.RightBottom_coord.Lon, false ) );

      bool txLonDone = false;
      for (double yLat = lt.Lat; yLat >= lb.Lat; yLat -= 0.5) {
        Point p = new Point( 0, 0 );
        // when wrapping around 180° we need to split the Lon drawing in 2 sections
        if (rt.Lon > lt.Lon) {
          // case normal right.Lon > left.Lon
          for (double xLon = rt.Lon; xLon >= lt.Lon; xLon -= 0.5) {
            p = MapToPixel( new LatLon( yLat, xLon ) );
            g.DrawLine( Pen, p.X, 0, p.X, g.VisibleClipBounds.Height );
            if (!txLonDone) {
              g.DrawString( (xLon < 0) ? $"W{-xLon}°" : $"E{xLon}°", Font, TextBrush, p.X, p.Y + 5, StringFormatLon );
            }
          }
        }
        else {
          // case wrap at 180° right.Lon < left.Lon
          // West part, does NOT include 180°
          for (double xLon = rt.Lon; xLon > -180.0; xLon -= 0.5) {
            p = MapToPixel( new LatLon( yLat, xLon ) );
            g.DrawLine( Pen, p.X, 0, p.X, g.VisibleClipBounds.Height );
            if (!txLonDone) {
              g.DrawString( (xLon < 0) ? $"W{-xLon}°" : $"E{xLon}°", Font, TextBrush, p.X, p.Y + 5, StringFormatLon );
            }
          }
          // East part, does include 180°
          for (double xLon = 180.0; xLon >= lt.Lon; xLon -= 0.5) {
            p = MapToPixel( new LatLon( yLat, xLon ) );
            g.DrawLine( Pen, p.X, 0, p.X, g.VisibleClipBounds.Height );
            if (!txLonDone) {
              g.DrawString( (xLon < 0) ? $"W{-xLon}°" : $"E{xLon}°", Font, TextBrush, p.X, p.Y + 5, StringFormatLon );
            }
          }
        }
        txLonDone = true;
        g.DrawLine( Pen, 0, p.Y, g.VisibleClipBounds.Width, p.Y );
        g.DrawString( (yLat < 0) ? $"S{-yLat}°" : $"N{yLat}°", Font, TextBrush, p.X, p.Y, StringFormatLat );
      }
    }

  }
}