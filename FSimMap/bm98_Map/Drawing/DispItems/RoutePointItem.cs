using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using bm98_Map.Drawing.DispItems.Routes;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// To show a Route Point
  /// </summary>
  internal class RoutePointItem : DisplayItem
  {
    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    /// <summary>
    /// Track of the Outbound path
    /// </summary>
    public int OutboundTrack_deg { get; set; } = 0;
    /// <summary>
    /// Coord of the Outbound path
    /// </summary>
    public LatLon OutboundLatLon { get; set; } = new LatLon( 0, 0 );
    /// <summary>
    /// Waypoint Label Rectangle
    /// </summary>
    public Rectangle WypLabelRectangle { get; set; }
    /// <summary>
    /// The Route Segment Manager to use
    /// </summary>
    public RouteSegmentMgr SegmentMgr { get; set; }

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public RoutePointItem( Bitmap spriteRef )
    {
      _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g,IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn
      if (String.StartsWith( "MIVEK")) {
        ;
      }
      var save = g.BeginContainer( );
      {
        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        var omp = vpRef.MapToCanvasPixel( OutboundLatLon );

        if (g.VisibleClipBounds.Contains( mp ) || g.VisibleClipBounds.Contains( omp )) {
          var rect = new Rectangle( new Point( 0, 0 ), _imageRef.Size );
          var textRect = new Rectangle( mp, new Size( 300, 64 ) );//96

          // draw to if possible
          if (!OutboundLatLon.IsEmpty) {
            if (SegmentMgr != null) {
              SegmentMgr.AddPoint( Pen, mp );
              SegmentMgr.AddPoint( Pen, omp );
            }
            else {
              g.DrawLine( Pen, mp, omp );
            }
          }
          var rotSave = g.BeginContainer( );
          g.TranslateTransform( -rect.Width / 2, -rect.Height / 2, MatrixOrder.Append );
          g.RotateTransform( (float)OutboundTrack_deg, MatrixOrder.Append );
          g.TranslateTransform( mp.X, mp.Y, MatrixOrder.Append );

          //g.TranslateTransform( rect.Width / 2, rect.Height / 2, MatrixOrder.Append );
          g.DrawImage( _imageRef, rect );
          g.EndContainer( rotSave );

          // 
          g.TranslateTransform( -textRect.Width / 2, rect.Height / 2, MatrixOrder.Append );// below
          g.DrawString( String, Font, TextBrush, textRect, StringFormat );
        }
        else {
          // only the track is visible
          // draw to if possible
          if (!OutboundLatLon.IsEmpty) {
            if (SegmentMgr != null) {
              SegmentMgr.AddPoint( Pen, mp );
              SegmentMgr.AddPoint( Pen, omp );
            }
            else {
              g.DrawLine( Pen, mp, omp );
            }
          }
        }
      }
      g.EndContainer( save );
    }
  }
}


