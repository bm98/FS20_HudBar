using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// HUD GUI FontSize
  /// </summary>
  public enum FontSize
  {
    Regular=0,
    Plus_2,
    Plus_4,
    Plus_6,
    Plus_8,
    Plus_10,
    Minus_2,
    Minus_4,
    Plus_12,
    Plus_14,
  }

  /// <summary>
  /// Placement of the Bar
  /// </summary>
  public enum Placement
  {
    Bottom=0,
    Left,
    Right,
    Top,
  }

  /// <summary>
  /// Display Kind of the Bar
  /// </summary>
  public enum Kind
  {
    Bar=0,
    Tile,
  }

  /// <summary>
  /// Font provider
  /// </summary>
  class GUI_Fonts
  {
    // Regular Fonts
    private Font m_lblRegular =  new Font( "Bahnschrift", 9.75f, FontStyle.Bold);
    private Font m_valRegular =  new Font( "Lucida Console", 14.25f, FontStyle.Bold);
    private Font m_valRegular2 =  new Font( "Lucida Console", 12f, FontStyle.Bold);
    // Condensed Fonts (Lucida Console seems to be one of the least line height creeping fonts, so we just use a smaller one for condensed)
    // Option would be to use the same as for the labels - gets then really condensed..)
    private Font m_lblCondensed =  new Font( "Bahnschrift SemiBold Condensed", 9.75f, FontStyle.Bold);
    //    private Font m_valCondensed =  new Font( "Lucida Console", 12f, FontStyle.Bold);
    //    private Font m_valCondensed2 =  new Font( "Lucida Console", 11.25f, FontStyle.Bold);
    private Font m_valCondensed =  new Font( "Bahnschrift SemiCondensed", 14.25f, FontStyle.Bold);
    private Font m_valCondensed2 =  new Font( "Bahnschrift SemiCondensed", 12f, FontStyle.Bold);
    // Sign font
    private Font m_sign =  new Font( "Wingdings", 15.76f, FontStyle.Bold);

    // Assigned and sized fonts
    private Font m_labelFont = null;
    private Font m_signFont = null;
    private Font m_valueFont = null;
    private Font m_value2Font = null;

    private static char c_NSpace =Convert.ToChar(0x2007);  // Number size Space

    /// <summary>
    /// Pad a string on the right side with NSpaces up to fieldSize
    /// </summary>
    /// <param name="label">The Input string</param>
    /// <param name="fieldSize">The fieldSize</param>
    /// <returns></returns>
    public static string PadRight( string label, int fieldSize )
    {
      return label.PadRight( fieldSize, c_NSpace );
    }


    /// <summary>
    /// Returns a Font Increment for a FontSize Enum 
    ///   add this to the regular fontsize
    /// </summary>
    /// <param name="fontSize">The FontSize Enum</param>
    /// <returns>An increment </returns>
    public static float FontIncrement( FontSize fontSize )
    {
      switch ( fontSize ) {
        case FontSize.Regular: return 0;
        case FontSize.Plus_2: return 2;
        case FontSize.Plus_4: return 4;
        case FontSize.Plus_6: return 6;
        case FontSize.Plus_8: return 8;
        case FontSize.Plus_10: return 10;
        case FontSize.Plus_12: return 12;
        case FontSize.Plus_14: return 14;
        case FontSize.Minus_2: return -2;
        case FontSize.Minus_4: return -4;
        default: return 0;
      }
    }

    public Font LabelFont { get; private set; }
    public Font SignFont { get; private set; }
    public Font ValueFont { get; private set; }
    public Font Value2Font { get; private set; }

    /// <summary>
    /// cTor: Init default fonts
    /// </summary>
    public GUI_Fonts( Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      m_labelFont = lblProto.Font;
      m_signFont = signProto.Font;
      m_valueFont = valueProto.Font;
      m_value2Font = value2Proto.Font;
    }
    /// <summary>
    /// cTor: Init default fonts from builtins
    /// </summary>
    public GUI_Fonts( bool condensed )
    {
      m_labelFont?.Dispose( ); 
      m_signFont?.Dispose( );
      m_valueFont?.Dispose( );
      m_value2Font?.Dispose( );

      if ( condensed ) {
        m_labelFont = m_lblCondensed;
        m_signFont = m_sign;
        m_valueFont = m_valCondensed;
        m_value2Font = m_valCondensed2;
      }
      else {
        m_labelFont = m_lblRegular;
        m_signFont = m_sign;
        m_valueFont = m_valRegular;
        m_value2Font = m_valRegular2;
      }
    }

    /// <summary>
    /// Set the FontSize Served
    /// </summary>
    /// <param name="fontSize"></param>
    public void SetFontsize( FontSize fontSize )
    {
      // maintain memory 
      if ( m_labelFont != null ) m_labelFont.Dispose( );
      if ( m_signFont != null ) m_signFont.Dispose( );
      if ( m_valueFont != null ) m_valueFont.Dispose( );
      if ( m_value2Font != null ) m_value2Font.Dispose( );

      // alloc each font only once and use it as ref
      float fontInc = FontIncrement(fontSize);
      LabelFont = new Font( m_labelFont.FontFamily, m_labelFont.Size + fontInc );
      SignFont = new Font( m_signFont.FontFamily, m_signFont.Size + fontInc );
      ValueFont = new Font( m_valueFont.FontFamily, m_valueFont.Size + fontInc );
      Value2Font = new Font( m_value2Font.FontFamily, m_value2Font.Size + fontInc );
    }

  }
}
