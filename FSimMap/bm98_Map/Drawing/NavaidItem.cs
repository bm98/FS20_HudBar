using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using static dNetBm98.XSize;
using CoordLib;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// To show a Navaid
  /// </summary>
  internal class NavaidItem : DisplayItem
  {
    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    // text location
    public bool IsNdbType = false;
    public bool IsWypType = false;

    /// <summary>
    /// The RunwayIdent  'nnd' (if any)
    /// </summary>
    public string RunwayIdent { get; set; } = "";
    /// <summary>
    /// The Approach  'RNAV' (if any)
    /// </summary>
    public string RunwayApproachIdent { get; set; } = "";
    /// <summary>
    /// The outbound Compass Direction (NSWE) for Approaches
    /// </summary>
    public string CompassPoint { get; set; } = "";

    /// <summary>
    /// Coord of the Outbound path for waypoints
    /// </summary>
    public LatLon OutboundLatLon { get; set; } = LatLon.Empty;
    /// <summary>
    /// True if the outbound track should be drawn
    /// </summary>
    public bool ShowOutboundTrack { get; set; } = false;

    /// <summary>
    /// Waypoint Label Rectangle
    /// </summary>
    public Rectangle WypLabelRectangle { get; set; }

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public NavaidItem( Bitmap spriteRef )
    {
      _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="MapToPixel">Map to Pixel Mapping function</param>
    protected override void PaintThis( Graphics g, Func<LatLon, Point> MapToPixel )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = MapToPixel( CoordPoint );
        var omp = MapToPixel( OutboundLatLon );

        if (g.VisibleClipBounds.Contains( mp ) || g.VisibleClipBounds.Contains( omp )) {
          var boxRect = new Rectangle( mp, _imageRef.Size );
          var spriteRect = new Rectangle( mp, _imageRef.Size.Multiply( 0.7f ) ); // make the sprites a bit smaller
          var textRect = new Rectangle( mp, new Size( 108, 64 ) );

          // draw to if possible and requested
          if (!OutboundLatLon.IsEmpty && ShowOutboundTrack) {
            g.DrawLine( Pen, mp, omp );
          }

          g.TranslateTransform( -spriteRect.Width / 2, -spriteRect.Height / 2 );
          g.DrawImage( _imageRef, spriteRect );
          g.TranslateTransform( spriteRect.Width / 2, spriteRect.Height / 2, MatrixOrder.Append );
          // Navaid Location = 0/0
          if (IsNdbType) {
            g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f, MatrixOrder.Append );// above
          }
          else if (IsWypType) {
            // use the outbound leg CompasPoint to place the Wyp label
            switch (CompassPoint) {
              case "N":
                g.TranslateTransform( -textRect.Width * 1.3f, -boxRect.Height / 2, MatrixOrder.Append );//  left
                break;
              case "E":
                g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f, MatrixOrder.Append );//  above
                break;
              case "S":
                g.TranslateTransform( textRect.Width / 2 * 0.3f, -boxRect.Height / 2, MatrixOrder.Append );//  right
                break;
              case "W":
                g.TranslateTransform( -textRect.Width / 2, boxRect.Height / 2, MatrixOrder.Append );// below
                break;
              default:
                g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f, MatrixOrder.Append );//  above
                break;
            }
          }
          else {
            g.TranslateTransform( -textRect.Width / 2, boxRect.Height / 2, MatrixOrder.Append );// below
          }
          g.DrawString( String, Font, TextBrush, textRect, StringFormat );
        }
      }
      g.EndContainer( save );


    }


  }
}
