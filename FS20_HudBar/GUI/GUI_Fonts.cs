using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

using DbgLib;

namespace FS20_HudBar.GUI
{
  // Mapped into the Namespace 

  /// <summary>
  /// HUD GUI FontSize
  /// </summary>
  public enum FontSize
  {
    Regular = 0,
    Plus_2,
    Plus_4,
    Plus_6,
    Plus_8,
    Plus_10,
    Minus_2,
    Minus_4,
    Plus_12,
    Plus_14,
    // added 20220212
    Plus_18,
    Plus_20,
    Plus_24,
    Plus_28,
    // added 20220304
    Plus_32,
  }

  /// <summary>
  /// Placement of the Bar
  /// </summary>
  public enum Placement
  {
    Bottom = 0,
    Left,
    Right,
    Top,
    //
    TopStack, // 20240222 break each item at the Label
  }

  /// <summary>
  /// Display Kind of the Bar
  /// </summary>
  public enum Kind
  {
    Bar = 0,
    Tile,
    Window, //20210718
    WindowBL, //20211022 borderless window
  }

  /// <summary>
  /// Flow Break types
  /// </summary>
  public enum BreakType //202220107
  {
    None = 0,
    FlowBreak = 1,
    DivBreak1 = 2, // Color Line Type 1
    DivBreak2 = 3, // Color Line Type 2
  }

  /// <summary>
  /// Display Transparency of the Bar
  /// </summary>
  public enum Transparent  //20211003
  {
    // Note: this is set to be the enum*10 => % Values and /10 => Opacity setting for the Main Form (lazy..)
    // 100% does not work with WinForms and the current implementation
    T0 = 0,
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
  /// Font provider, maintains default and user fonts
  ///  User Fonts are copied from default if no specific user font is provided and then returned as ConfigString
  ///  i.e. Config is never empty except for the first time (read from Settings as empty string)
  /// </summary>
  class GUI_Fonts : IDisposable
  {
    /*
      User Fonts are copied from default if no specific user font is provided and then returned as ConfigString
      i.e. Config is never empty except for the first time (read from Settings as empty string)
    */

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
    public enum FKinds
    {
      Lbl = 0,
      Value,
      Value2,
      Sign,
    }


    #region STATIC
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // figure space - i.e. wide space char for Labels (flex spaced fonts)
    // as it uses a larger height box make sure to check the appearance 
    // May be use 1 c_space added all the time to have a consistent layout
    internal static char c_space = '\x2007';

    // Store embedded Fonts here
    private static PrivateFontCollection s_privateFonts = new PrivateFontCollection( );

    /// <summary>
    /// cTor: Static, load the memory font(s)
    /// </summary>
    static GUI_Fonts( )
    {
      try {
        //  Font embedding Ref: https://web.archive.org/web/20141224204810/http://bobpowell.net/embedfonts.aspx
        // Load a rather condensed embedded font 
        byte[] fontdata;
        using (Stream fontStream = System.Reflection.Assembly.GetExecutingAssembly( ).GetManifestResourceStream( @"FS20_HudBar.Fonts.ShareTechMono-Regular.ttf" )) {

          fontdata = new byte[fontStream.Length];
          fontStream.Read( fontdata, 0, (int)fontStream.Length );
        }

        unsafe {
          fixed (byte* pFontData = fontdata) {
            s_privateFonts.AddMemoryFont( (IntPtr)pFontData, fontdata.Length );
          }
        }
      }
      catch (Exception e) {
        LOG.Error( $"static cTor GUI_Fonts: Cannot create Memory Font\n{e.Message}" );
      }
    }

    // returns a new Family either from Win or Private
    private static FontFamily GetFontFamily( string ffName )
    {
      FontFamily ret;
      // try Windows first
      if (FontFamily.Families.Where( x => x.Name == ffName ).Count( ) > 0) {
        ret = new FontFamily( ffName );
      }
      // then our private store
      else if (s_privateFonts.Families.Where( x => x.Name == ffName ).Count( ) > 0) {
        ret = new FontFamily( ffName, s_privateFonts );
      }
      // cannot find it anywhere
      else {
        // get a generic font
        ret = FontFamily.GenericSansSerif;
        LOG.Error( $"GetFontFamily: Cannot load: {ffName} - using a generic font" );
      }
      return ret;
    }

    /// <summary>
    /// Char for 'Managed' indication
    /// </summary>
    public static string ManagedTag = "♦";

    #endregion

    #region FontDescriptor (private)

    /// <summary>
    /// Local Font repository
    /// </summary>
    private class FontDescriptor : IDisposable
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
      private bool disposedValue;

      /// <summary>
      /// The current font to use
      /// </summary>
      public Font Font { get => m_font; set { m_font?.Dispose( ); m_font = value; } }  // maintain memory and dispose prev one

      /// <summary>
      /// cTor: empty
      /// </summary>
      public FontDescriptor( ) { }

      /// <summary>
      /// cTor: Copy
      /// </summary>
      /// <param name="other"></param>
      public FontDescriptor( FontDescriptor other )
      {
        // regular 
        this.RegFamily = GetFontFamily( other.RegFamily.Name );
        this.RegSize = other.RegSize;
        this.RegStyle = other.RegStyle;
        // condensed
        this.CondFamily = GetFontFamily( other.CondFamily.Name );
        this.CondSize = other.CondSize;
        this.CondStyle = other.CondStyle;
      }


      /// <summary>
      /// Create the applicable font 
      /// </summary>
      /// <param name="fontSize">The Size to create</param>
      /// <param name="condensed">Condensed Fonts or Regular ones</param>
      public void CreateFont( FontSize fontSize, bool condensed )
      {
        if (condensed) {
          Font = new Font( CondFamily, CondSize + FontIncrement( fontSize ), CondStyle );
        }
        else {
          Font = new Font( RegFamily, RegSize + FontIncrement( fontSize ), RegStyle );
        }
      }

      #region FontDescriptor Dispose

      protected virtual void Dispose( bool disposing )
      {
        if (!disposedValue) {
          if (disposing) {
            // TODO: dispose managed state (managed objects)
            m_font?.Dispose( );
            RegFamily?.Dispose( );
            CondFamily?.Dispose( );
          }
          disposedValue = true;
        }
      }
      public void Dispose( )
      {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
      }
      #endregion
    }

    #endregion


    // defaults flag
    private bool _usingDefaults = false;


    // Default Font repo - initialize on create and only used to maintain the original 
    private Dictionary<FKinds, FontDescriptor> m_fontStorageDefault = new Dictionary<FKinds, FontDescriptor>( ) {
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

    // User Font repo - initialize on create as a copy from Default, then loaded from AppDefault (if there are)
    private Dictionary<FKinds, FontDescriptor> m_fontStorageUser = new Dictionary<FKinds, FontDescriptor>( );

    // saved currents
    private FontSize m_fontSize = FontSize.Regular;
    private bool m_condensed = false;

    /// <summary>
    /// Returns the current Fontsize
    /// </summary>
    public FontSize Fontsize => m_fontSize;
    /// <summary>
    /// Returns the current Condensed Flag
    /// </summary>
    public bool Condensed => m_condensed;

    /// <summary>
    /// True when using default fonts
    /// </summary>
    public bool IsUsingDefaults => _usingDefaults;

    private static char c_NSpace = Convert.ToChar( 0x2007 );  // Number size Space

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
      switch (fontSize) {
        case FontSize.Regular: return 0;
        case FontSize.Plus_2: return 2;
        case FontSize.Plus_4: return 4;
        case FontSize.Plus_6: return 6;
        case FontSize.Plus_8: return 8;
        case FontSize.Plus_10: return 10;
        case FontSize.Plus_12: return 12;
        case FontSize.Plus_14: return 14;
        // added 20220212
        case FontSize.Plus_18: return 18;
        case FontSize.Plus_20: return 20;
        case FontSize.Plus_24: return 24;
        case FontSize.Plus_28: return 28;
        // added 20220304
        case FontSize.Plus_32: return 32;

        case FontSize.Minus_2: return -2;
        case FontSize.Minus_4: return -4;
        default: return 0;
      }
    }


    /// <summary>
    /// The Label Font
    /// </summary>
    public Font LabelFont => m_fontStorageUser[FKinds.Lbl].Font;
    /// <summary>
    /// The Value Item Font
    /// </summary>
    public Font ValueFont => m_fontStorageUser[FKinds.Value].Font;
    /// <summary>
    /// A smaller Value Item Font
    /// </summary>
    public Font Value2Font => m_fontStorageUser[FKinds.Value2].Font;
    /// <summary>
    /// The Sign Font
    /// </summary>
    public Font SignFont => m_fontStorageUser[FKinds.Sign].Font;


    /// <summary>
    /// cTor: Init default fonts from builtins
    /// </summary>
    public GUI_Fonts( bool condensed )
    {
      try {
        m_fontStorageDefault[FKinds.Value].CondFamily?.Dispose( );
        m_fontStorageDefault[FKinds.Value].CondFamily = s_privateFonts.Families[(int)EFonts.ShareTechMono];

        m_fontStorageDefault[FKinds.Value2].CondFamily?.Dispose( );
        m_fontStorageDefault[FKinds.Value2].CondFamily = s_privateFonts.Families[(int)EFonts.ShareTechMono];
      }
      catch {
        ; // DEBUG STOP ONLY
      }

      m_condensed = condensed;
      // get the user fonts from the defaults     
      ResetUserFonts( );
      // Get user fonts from AppDefaults

      // loading actual fonts for the first time
      SetFontsize( m_fontSize );
    }

    /// <summary>
    /// cTor: create a copy 
    /// </summary>
    /// <param name="other"></param>
    public GUI_Fonts( GUI_Fonts other )
    {
      try {
        m_fontStorageDefault[FKinds.Value].CondFamily?.Dispose( );
        m_fontStorageDefault[FKinds.Value].CondFamily = s_privateFonts.Families[(int)EFonts.ShareTechMono];

        m_fontStorageDefault[FKinds.Value2].CondFamily?.Dispose( );
        m_fontStorageDefault[FKinds.Value2].CondFamily = s_privateFonts.Families[(int)EFonts.ShareTechMono];
      }
      catch {
        ; // DEBUG STOP ONLY
      }

      m_condensed = other.m_condensed;
      m_fontSize = other.m_fontSize;

      _usingDefaults = true;
      // copy all from the user
      foreach (var fd in other.m_fontStorageUser) {
        m_fontStorageUser.Add( fd.Key, new FontDescriptor( fd.Value ) ); // create a copy (not a ref)
        _usingDefaults = false;
      };

      // loading actual fonts for the first time
      SetFontsize( m_fontSize );
    }


    /// <summary>
    /// Set the Condensed mode 
    /// </summary>
    /// <param name="condensed">True for condensed fonts</param>
    public void SetFontCondensed( bool condensed )
    {
      m_condensed = condensed;
      SetFontsize( m_fontSize ); // reload
    }

    /// <summary>
    /// Set the FontSize Served
    /// </summary>
    /// <param name="fontSize"></param>
    public void SetFontsize( FontSize fontSize )
    {
      m_fontSize = fontSize;
      // alloc each font only once and use it as ref later on
      foreach (var fd in m_fontStorageUser) {
        fd.Value.CreateFont( m_fontSize, m_condensed );
      }
    }

    /// <summary>
    /// Set a new User Font
    /// </summary>
    /// <param name="fontKind">Kind to set</param>
    /// <param name="font">The font</param>
    /// <param name="condensed">True to set the condensed font</param>
    public void SetUserFont( FKinds fontKind, Font font, FontSize fontSize, bool condensed )
    {
      if (fontKind == FKinds.Sign) return; // Sign is not user definable

      if (condensed) {
        m_fontStorageUser[fontKind].CondFamily?.Dispose( );
        m_fontStorageUser[fontKind].CondFamily = GetFontFamily( font.FontFamily.Name );
        m_fontStorageUser[fontKind].CondSize = font.Size - FontIncrement( fontSize ); // normalize
        if (fontKind == FKinds.Value2) m_fontStorageUser[fontKind].RegSize -= 1f; // Value2 fonts are smaller
        m_fontStorageUser[fontKind].CondStyle = font.Style;
      }
      else {
        m_fontStorageUser[fontKind].RegFamily?.Dispose( );
        m_fontStorageUser[fontKind].RegFamily = GetFontFamily( font.FontFamily.Name );
        m_fontStorageUser[fontKind].RegSize = font.Size - FontIncrement( fontSize ); // normalize
        if (fontKind == FKinds.Value2) m_fontStorageUser[fontKind].RegSize -= 2f; // Value2 fonts are smaller
        m_fontStorageUser[fontKind].RegStyle = font.Style;
      }
      SetFontsize( m_fontSize );
      _usingDefaults = false;
    }

    /// <summary>
    /// Set all user fonts to defaults
    /// </summary>
    public void ResetUserFonts( )
    {
      // remove old user Catalog
      foreach (var fd in m_fontStorageUser) {
        fd.Value?.Dispose( );
      }
      m_fontStorageUser.Clear( );

      // copy all from the default
      foreach (var fd in m_fontStorageDefault) {
        m_fontStorageUser.Add( fd.Key, new FontDescriptor( fd.Value ) ); // create a copy (not a ref)
      };
      SetFontsize( m_fontSize );
      _usingDefaults = true;
    }

    /// <summary>
    /// Load the serialized version into User Config
    /// </summary>
    public void FromConfigString( string cString )
    {
      // sanity
      if (string.IsNullOrEmpty( cString )) {
        ResetUserFonts( );
        return;
      }

      // remove old user Catalog
      foreach (var fd in m_fontStorageUser) {
        fd.Value?.Dispose( );
      }
      m_fontStorageUser.Clear( );
      // start loading
      string[] items = cString.Split( new char[] { '¦' }, StringSplitOptions.RemoveEmptyEntries );
      for (int i = 0; i < items.Length; i++) {
        string[] e = items[i].Split( new char[] { '¬' } );
        // decode - we need all 5 elements
        if (e.Length >= 7) {
          var newFd = new FontDescriptor( );
          if (Enum.TryParse( e[0], out FKinds kind )) {
            // regular
            if (float.TryParse( e[2], out float rsize )) {
              if (Enum.TryParse( e[3], out FontStyle rstyle )) {
                var rfFam = e[1];
                newFd.RegFamily = GetFontFamily( rfFam );
                newFd.RegSize = rsize;
                newFd.RegStyle = rstyle;
              }
            }
            // condensed
            if (float.TryParse( e[5], out float csize )) {
              if (Enum.TryParse( e[6], out FontStyle cstyle )) {
                var cfFam = e[4];
                newFd.CondFamily = GetFontFamily( cfFam );
                newFd.CondSize = csize;
                newFd.CondStyle = cstyle;
              }
            }
          }
          m_fontStorageUser.Add( kind, newFd );
        }
      }
      // check..
      if (m_fontStorageUser.Count == 3) {
        // add the sign from the defaults
        m_fontStorageUser.Add( FKinds.Sign, new FontDescriptor( m_fontStorageDefault[FKinds.Sign] ) ); // create a copy (not a ref)
        SetFontsize( m_fontSize );
        _usingDefaults = false;
      }
      else {
        // or first start
        LOG.Info( $"Read Font Config: did not found all entries - resetting user fonts to defaults" );
        ResetUserFonts( );
      }
    }

    /// <summary>
    /// Save User Fonts as Config String
    /// </summary>
    /// <returns>An AppSettings String</returns>
    public string AsConfigString( )
    {
      /*
       * Format:
       * "{FKinds¬FamilyName¬size¬style¬FamilyName¬size¬style¦}3"
       */
      var s = "";
      foreach (var fd in m_fontStorageUser) {
        if (fd.Key == FKinds.Sign) continue; // not sign fonts
        s += $"{fd.Key}¬";
        s += $"{fd.Value.RegFamily.Name}¬{fd.Value.RegSize:#0.00}¬{fd.Value.RegStyle}¬";
        s += $"{fd.Value.CondFamily.Name}¬{fd.Value.CondSize:#0.00}¬{fd.Value.CondStyle}¦";
      }
      return s;
    }


    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          // remove old user Catalog
          foreach (var fd in m_fontStorageUser) {
            fd.Value.Dispose( );
          }
          m_fontStorageUser.Clear( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }
    #endregion

  }
}

