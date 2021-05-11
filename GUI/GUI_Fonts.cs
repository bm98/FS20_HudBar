using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  public enum FontSize
  {
    Regular=0,
    Larger,
    Largest,
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

    private float m_largerInc = 2;  // add this to the regular fontsize
    private float m_largestInc = 4; // add this to the regular fontsize

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
      switch ( fontSize ) {
        case FontSize.Largest:
          LabelFont = new Font( m_labelFont.FontFamily, m_labelFont.Size + m_largestInc );
          SignFont = new Font( m_signFont.FontFamily, m_signFont.Size + m_largestInc );
          ValueFont = new Font( m_valueFont.FontFamily, m_valueFont.Size + m_largestInc );
          Value2Font = new Font( m_value2Font.FontFamily, m_value2Font.Size + m_largestInc );
          break;
        case FontSize.Larger:
          LabelFont = new Font( m_labelFont.FontFamily, m_labelFont.Size + m_largerInc );
          SignFont = new Font( m_signFont.FontFamily, m_signFont.Size + m_largerInc );
          ValueFont = new Font( m_valueFont.FontFamily, m_valueFont.Size + m_largerInc );
          Value2Font = new Font( m_value2Font.FontFamily, m_value2Font.Size + m_largerInc );
          break;
        case FontSize.Regular:
          LabelFont = new Font( m_labelFont.FontFamily, m_labelFont.Size );
          SignFont = new Font( m_signFont.FontFamily, m_signFont.Size );
          ValueFont = new Font( m_valueFont.FontFamily, m_valueFont.Size );
          Value2Font = new Font( m_value2Font.FontFamily, m_value2Font.Size );
          break;

        default:
          LabelFont = new Font( m_labelFont.FontFamily, m_labelFont.Size );
          SignFont = new Font( m_signFont.FontFamily, m_signFont.Size );
          ValueFont = new Font( m_valueFont.FontFamily, m_valueFont.Size );
          Value2Font = new Font( m_value2Font.FontFamily, m_value2Font.Size );
          break;
      }
    }

  }
}
