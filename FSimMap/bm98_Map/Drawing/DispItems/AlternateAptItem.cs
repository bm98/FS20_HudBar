using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// To show an alternate Airport
  /// </summary>
  internal class AlternateAptItem : DisplayItem
  {
    private Bitmap _imageRef;
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

    /// <summary>
    /// The RunwayIdent  'nnd' (if any)
    /// </summary>
    public string RunwayIdent = "";
    /// <summary>
    /// Waypoint Label Rectangle
    /// </summary>
    public Rectangle WypLabelRectangle;

    /// <summary>
    /// cTor: create sprite, submit the image (will not be managed or disposed here)
    /// </summary>
    public AlternateAptItem( Bitmap spriteRef )
    {
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
        // Set world transform of graphics object to translate.
        var mp = vpRef.MapToCanvasPixel( CoordPoint );
        if (g.VisibleClipBounds.Contains( mp )) {
          var rect = new Rectangle( mp, _imageRef.Size );
          // level x-axis
          TurnAroundPoint( g, mp, vpRef.MapHeading );

          g.TranslateTransform( -rect.Width / 2, -rect.Height / 2 );
          g.DrawImage( _imageRef, rect );
          g.TranslateTransform( rect.Width / 2, rect.Height / 2 );
          // Navaid Location = 0/0

          var textRect = new Rectangle( mp, new Size( 96, 64 ) );
          g.TranslateTransform( -textRect.Width / 2, rect.Height / 2 );// below
          g.DrawString( String, Font, TextBrush, textRect, StringFormat );
        }
      }
      g.EndContainer( save );
    }
  }
}

