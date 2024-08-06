using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  /// <summary>
  /// Implements a TextItem (button label)
  /// paints the text and if Active and a Fill is given onto a background
  /// </summary>
  internal class TextItem : DisplayItem
  {
    /// <summary>
    /// The text drawing format such as alignment
    /// </summary>
    public StringFormat StringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

    /// <summary>
    /// cTor: empty
    /// </summary>
    public TextItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public TextItem( TextItem other )
      : base( other )
    {
      this.StringFormat = other.StringFormat.Clone( ) as StringFormat;
    }

    /// <summary>
    /// Get a clone of this TextItem
    /// </summary>
    /// <returns></returns>
    public virtual TextItem Clone( )
    {
      return new TextItem( this );
    }

    protected override void PaintThis( Graphics g )
    {
      // draw label background if active and there is a FillBrush
      if ( Active == ActiveState.Alert && BackgBrushAlarm != null ) {
        g.FillRectangle( BackgBrushAlarm, Rectangle );
        if ( !string.IsNullOrEmpty( String ) ) {
          g.DrawString( String, Font, TextBrushAlert, Rectangle, StringFormat );
        }
      }
      else if ( Active == ActiveState.Warn && BackgBrushWarn != null ) {
        g.FillRectangle( BackgBrushWarn, Rectangle );
        if ( !string.IsNullOrEmpty( String ) ) {
          g.DrawString( String, Font, TextBrushWarn, Rectangle, StringFormat );
        }
      }
      // draw label if string is not empty
      else if ( !string.IsNullOrEmpty( String ) ) {
        if ( (TextBrushActive != null) && Active == ActiveState.Engaged ) {
          g.DrawString( String, Font, TextBrushActive, Rectangle, StringFormat );
        }
        else if ( (TextBrushArmed != null) && Active == ActiveState.Armed ) {
          g.DrawString( String, Font, TextBrushArmed, Rectangle, StringFormat );
        }
      }
    }
  }
}
