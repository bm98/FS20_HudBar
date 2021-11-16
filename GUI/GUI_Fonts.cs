using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  // Mapped into the Namespace 

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
    Window, //20210718
    WindowBL, //20211022 borderless window
  }

  /// <summary>
  /// Display Transparency of the Bar
  /// </summary>
  public enum Transparent  //20211003
  {
    // Note: this is set to be the enum*10 => % Values and /10 => Opacity setting for the Main Form (lazy..)
    // 100% does not work with WinForms and the current implementation
    T0=0,
    T10,
    T20,
    T30,
    T40,
    T50,
    T60,
    T70,
    T80,
    T90,
  }

  /// <summary>
  /// Font provider
  /// </summary>
  class GUI_Fonts
  {
    /// <summary>
    /// Embedded Fonts
    /// </summary>
    private enum EFonts
    {
      ShareTechMono = 0,
    }

    /// <summary>
    /// The kind of Fonts we maintain here
    /// </summary>
    private enum FKinds
    {
      Lbl=0,
      Value,
      Value2,
      Sign,
    }

    /// <summary>
    /// Local Font repository
    /// </summary>
    private class FontDescriptor
    {
      // Regular Descriptor
      public FontFamily RegFamily { get; set; }
      public float RegSize { get; set; } // Base Size
      public FontStyle RegStyle { get; set; }

      // Condensed Descriptor
      public FontFamily CondFamily { get; set; }
      public float CondSize { get; set; } // Base Size
      public FontStyle CondStyle { get; set; }

      private Font m_font = null;
      /// <summary>
      /// The current font to use
      /// </summary>
      public Font Font { get => m_font; set { m_font?.Dispose( ); m_font = value; } }  // maintain memory and dispose prev one

      /// <summary>
      /// Create the applicable font 
      /// </summary>
      /// <param name="fontSize">The Size to create</param>
      /// <param name="condensed">Condensed Fonts or Regular ones</param>
      public void CreateFont( FontSize fontSize, bool condensed )
      {
        if ( condensed ) { 
          Font = new Font( CondFamily, CondSize + FontIncrement( fontSize ), CondStyle );
        }
        else { 
          Font = new Font( RegFamily, RegSize + FontIncrement( fontSize ), RegStyle );
        }
      }
    }


    // Font repo - initialize on create
    private Dictionary<FKinds, FontDescriptor> m_fontStorage = new Dictionary<FKinds, FontDescriptor>() {
      { FKinds.Lbl, new FontDescriptor()
        {
          RegFamily = new FontFamily("Bahnschrift"), RegSize = 9.75f, RegStyle = FontStyle.Regular,
          CondFamily = new FontFamily("Bahnschrift SemiBold Condensed"), CondSize = 9.75f, CondStyle = FontStyle.Regular,
        } },
      { FKinds.Value, new FontDescriptor()
        {
          RegFamily = new FontFamily("Lucida Console"), RegSize = 14.25f, RegStyle = FontStyle.Regular,
          CondFamily = new FontFamily("Lucida Console"), CondSize = 12f, CondStyle = FontStyle.Regular, // later rewritten for the embedded one
      } },
      { FKinds.Value2, new FontDescriptor()
        {
          RegFamily = new FontFamily("Lucida Console"), RegSize = 12f, RegStyle = FontStyle.Regular,
          CondFamily = new FontFamily("Lucida Console"), CondSize = 11.25f, CondStyle = FontStyle.Regular, // later rewritten for the embedded one
      } },
      { FKinds.Sign, new FontDescriptor()
        {
          RegFamily = new FontFamily("Wingdings"), RegSize = 15.76f, RegStyle = FontStyle.Regular,
          CondFamily = new FontFamily("Wingdings"), CondSize = 15.76f, CondStyle = FontStyle.Regular, // same as regular
      } },
    };

    // saved currents
    private FontSize m_fontSize = FontSize.Regular;
    private bool m_condensed = false;


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
    private static float FontIncrement( FontSize fontSize )
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

    // Store embedded Fonts here
    PrivateFontCollection m_privateFonts = new PrivateFontCollection( );

    /// <summary>
    /// The Label Font
    /// </summary>
    public Font LabelFont => m_fontStorage[FKinds.Lbl].Font;
    /// <summary>
    /// The Value Item Font
    /// </summary>
    public Font ValueFont => m_fontStorage[FKinds.Value].Font;
    /// <summary>
    /// A smaller Value Item Font
    /// </summary>
    public Font Value2Font => m_fontStorage[FKinds.Value2].Font;
    /// <summary>
    /// The Sign Font
    /// </summary>
    public Font SignFont => m_fontStorage[FKinds.Sign].Font;


    /// <summary>
    /// cTor: Init default fonts from builtins
    /// </summary>
    public GUI_Fonts( bool condensed )
    {
      m_condensed = condensed;
      try {
        //  Font embedding Ref: https://web.archive.org/web/20141224204810/http://bobpowell.net/embedfonts.aspx
        // Load a rather condensed embedded font 
        Stream fontStream = this.GetType().Assembly.GetManifestResourceStream(@"FS20_HudBar.Fonts.ShareTechMono-Regular.ttf");
        byte[] fontdata = new byte[fontStream.Length];
        fontStream.Read( fontdata, 0, (int)fontStream.Length );
        fontStream.Close( );
        unsafe {
          fixed ( byte* pFontData = fontdata ) {
            m_privateFonts.AddMemoryFont( (IntPtr)pFontData, fontdata.Length );
          }
        }
        // set new condensed if we were successful
        m_fontStorage[FKinds.Value].CondFamily?.Dispose( );
        m_fontStorage[FKinds.Value].CondFamily = m_privateFonts.Families[(int)EFonts.ShareTechMono];

        m_fontStorage[FKinds.Value2].CondFamily?.Dispose( );
        m_fontStorage[FKinds.Value2].CondFamily = m_privateFonts.Families[(int)EFonts.ShareTechMono];
      }
      catch {
        ; // DEBUG STOP ONLY
      }

      // loading actual fonts for the first time
      SetFontsize( m_fontSize );
    }

    /// <summary>
    /// Set the FontSize Served
    /// </summary>
    /// <param name="fontSize"></param>
    public void SetFontsize( FontSize fontSize )
    {
      m_fontSize = fontSize;
      // alloc each font only once and use it as ref later on
      foreach ( var fd in m_fontStorage ) {
        fd.Value.CreateFont( m_fontSize, m_condensed );
      }
    }

  }
}
