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

namespace bm98_Map.Drawing.DispItems
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
    public bool IsHoldType = false;

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
      if (spriteRef == null)
        _imageRef = null;
      else
        _imageRef = new Bitmap( spriteRef );
    }

    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected override void PaintThis( Graphics g, IVPortPaint vpRef )
    {
      if (!Active) return; // shall not be drawn

      var save = g.BeginContainer( );
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        var omp = vpRef.MapToCanvasPixel( OutboundLatLon );

        if (g.VisibleClipBounds.Contains( mp ) || g.VisibleClipBounds.Contains( omp )) {

          // draw to if possible and requested
          if (!OutboundLatLon.IsEmpty && ShowOutboundTrack) {
            g.DrawLine( Pen, mp, omp );
          }

          // draw image and deco only if set
          if (_imageRef != null) {
            var boxRect = new Rectangle( mp, _imageRef.Size );
            var spriteRect = new Rectangle( mp, _imageRef.Size.Multiply( 0.7f ) ); // make the sprites a bit smaller

            // level x-axis
            TurnAroundPoint( g, mp, vpRef.MapHeading );

            g.TranslateTransform( -spriteRect.Width / 2, -spriteRect.Height / 2 );
            g.DrawImage( _imageRef, spriteRect );
            g.TranslateTransform( spriteRect.Width / 2, spriteRect.Height / 2 );

            var textRect = new Rectangle( mp, new Size( 108, 64 ) );

            // Navaid Location = 0/0
            if (IsHoldType) {
              g.TranslateTransform( textRect.Width / 2 * 0.3f, -boxRect.Height * 0.6f );// above/ right
            }
            else if (IsNdbType) {
              g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f );// above
            }
            else if (IsWypType) {
              // use the outbound leg CompasPoint to place the Wyp label
              switch (CompassPoint) {
                case "N":
                  g.TranslateTransform( -textRect.Width * 1.3f, -boxRect.Height / 2 );//  left
                  break;
                case "E":
                  g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f );//  above
                  break;
                case "S":
                  g.TranslateTransform( textRect.Width / 2 * 0.3f, -boxRect.Height / 2 );//  right
                  break;
                case "W":
                  g.TranslateTransform( -textRect.Width / 2, boxRect.Height / 2 );// below
                  break;
                default:
                  g.TranslateTransform( -textRect.Width / 2, -boxRect.Height * 1.3f );//  above
                  break;
              }
            }
            else {
              g.TranslateTransform( -textRect.Width / 2, boxRect.Height / 2 );// below
            }
            g.DrawString( String, Font, TextBrush, textRect, StringFormat );
          }
        }
      }
      g.EndContainer( save );


    }


  }
}
