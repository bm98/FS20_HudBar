using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  internal class TapeItem : DisplayItem
  {
    private int _valScale = 10;         // the Scale Increment for large units
    private readonly int _scale = 5;    // the scale number steps in large Units
    private float _offset = 65;         // offset of the Scale items (optical tuning to match scale and numbers)

    public int ValScale {
      get => _valScale;
      set {
        // sanity
        if (value < 10) return;

        if (_valScale == value) return; // already

        _valScale = value;
      }
    }

    /// <summary>
    /// cTor: empty
    /// </summary>
    public TapeItem( ) { }

    /// <summary>
    /// cTor: define Value Scale
    /// </summary>
    public TapeItem( int valScale )
    {
      _valScale = valScale;
    }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public TapeItem( TapeItem other )
      : base( other )
    {
      this.Value = other.Value;
      this.AlignRight = other.AlignRight;
    }

    /// <summary>
    /// Indicated Value
    /// </summary>
    public float Value { get; set; } = 0;

    /// <summary>
    /// Tape alignment within bounding box
    /// </summary>
    public bool AlignRight { get; set; } = false; // align right
    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    private StringFormat StringFormatLeft = new StringFormat( ) { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
    private StringFormat StringFormatRight = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

    // local only - draw the scale of the tape indicator
    private void DrawScale( Graphics g, Pen pen, float scaledValue )
    {
      var part = scaledValue % _scale;
      var YC = Rectangle.Height / 2;

      var ctxSave = g.Save( );

      g.TranslateTransform( 0, _offset / _scale * part ); // set origin to pos of AS indicator

      pen.Width = 1;
      // dashes to the right, drawing from the vert center to aling nicely
      for (int i = -7; i < 9; i++) {
        if (AlignRight)
          g.DrawLine( pen, Rectangle.Right - 8, YC + (i * _offset / 2), Rectangle.Right, YC + (i * _offset / 2) );
        else
          g.DrawLine( pen, 0, YC + (i * _offset / 2), 10, YC + (i * _offset / 2) );
      }
      // larger ones for the set indicator
      pen.Width = 2;
      for (int i = -5; i < 6; i++) {
        if (AlignRight)
          g.DrawLine( pen, Rectangle.Right - 12, YC + (i * _offset), Rectangle.Right, YC + (i * _offset) );
        else
          g.DrawLine( pen, 0, YC + (i * _offset), 20, YC + (i * _offset) );
      }

      g.Restore( ctxSave );
    }


    /// <summary>
    /// Draw a sprite image (if Active = Engaged)
    /// </summary>
    /// <param name="g">Graphics context</param>
    protected override void PaintThis( Graphics g )
    {
      float value = Value; // TEMP
      float scaledValue = (value * _scale) / _valScale;

      var XC = Rectangle.Width / 2;
      var YC = Rectangle.Height / 2;
      _offset = (int)(Rectangle.Height / _scale); // 5.9

      if (Active == ActiveState.Engaged) {

        var pen = new Pen( Pen.Color, 1 ); // used pen
        var ctxSave = g.Save( );

        // set the origin to pos of AS indicator and assume a relative drawing 
        g.TranslateTransform( Rectangle.Location.X, Rectangle.Location.Y );

        // for the rest clip the usable area
        var dRect = new Rectangle( 0, 0, Rectangle.Width-1, Rectangle.Height );
        g.SetClip( dRect, CombineMode.Replace );

        // makes the scale indicators
        DrawScale( g, pen, scaledValue );

        // draw scale numbers
        // calc the in-betweens..
        var part = scaledValue % _scale;
        var scaleValue = ((value < 0) ? Math.Ceiling( value / _valScale ) : Math.Floor( value / _valScale )) * _valScale;

        // the numbers of the band
        for (int i = -5; i < 6; i++) {
          // vert pos of the number is centerY - Offset*Step +  Offset/Scale*part
          var dispValue = scaleValue + i * _valScale;
          if (dispValue >= 0) {
            if (AlignRight)
              g.DrawString( $"{scaleValue + i * _valScale:## ##0}", Font, TextBrushArmed,
                            new RectangleF( 0, YC - i * _offset + _offset / _scale * part - 15, dRect.Width, 30 ), StringFormatRight );
            else
              g.DrawString( $"{scaleValue + i * _valScale:## ##0}", Font, TextBrushArmed,
                            new RectangleF( 0, YC - i * _offset + _offset / _scale * part - 15, dRect.Width, 30 ), StringFormatLeft );
          }
        }

        g.Restore( ctxSave );
        pen.Dispose( );
      }
    }


  }
}
