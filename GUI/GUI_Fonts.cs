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

    private Font m_labelFont;
    private Font m_signFont;
    private Font m_valueFont;
    private Font m_value2Font;

    private const float m_plus2Inc = 2;  // add this to the regular fontsize
    private const float m_plus4Inc = 4; // add this to the regular fontsize
    private const float m_plus6Inc = 6; // add this to the regular fontsize
    private const float m_plus8Inc = 8; // add this to the regular fontsize
    private const float m_plus10Inc = 10; // add this to the regular fontsize
    private const float m_minus2Inc = -2;  // add this to the regular fontsize
    private const float m_minus4Inc = -4;  // add this to the regular fontsize

    /// <summary>
    /// Returns a Font Increment for a FontSize Enum 
    ///   add this to the regular fontsize
    /// </summary>
    /// <param name="fontSize">The FontSize Enum</param>
    /// <returns>An increment </returns>
    public static float FontIncrement(FontSize fontSize )
    {
      switch ( fontSize ) {
        case FontSize.Regular:return 0;
        case FontSize.Plus_2:return 2;
        case FontSize.Plus_4:return 4;
        case FontSize.Plus_6: return 6;
        case FontSize.Plus_8: return 8;
        case FontSize.Plus_10:return 10;
        case FontSize.Minus_2: return -2;
        case FontSize.Minus_4:return -4;
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
