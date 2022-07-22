using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Checklist
{
  /// <summary>
  /// Handles the Font chores for the Checklist
  /// Register Fonts used
  /// Get Fonts by Descriptor
  /// 
  /// Load Fonts
  /// </summary>
  internal class FontProvider : IDisposable
  {
    // Font Base used 
    private float _baseSize = 9;
    private string _baseDescriptor = "";

    // A helper class to manage all fonts
    private class FontSpec
    {
      // mark as NOT in use
      private const float c_NilSize = -1;

      // Will hold the original Font registered on App Start
      public string Name = "Arial";
      public float Size = 9;
      public GraphicsUnit FontSizeUnit = GraphicsUnit.Point;
      public FontStyle FontStyle;

      // The base size to scale user fonts with
      public float BaseSize = 9;

      // A User Font if the user has choosen one
      public string UserName = "";
      public float UserSize = c_NilSize;
      public GraphicsUnit UserFontSizeUnit = GraphicsUnit.Point;
      public FontStyle UserFontStyle;

      // cTor: empty
      public FontSpec( ) { }
      // cTor: from Font and BaseSize
      public FontSpec( Font font, float baseSize )
      {
        Name = font.Name;
        Size = font.Size;
        FontSizeUnit = font.Unit;
        FontStyle = font.Style;
        BaseSize = baseSize;
      }

      // Returns the font (User if defined, else the Standard)
      public Font Font {
        get {
          if (UserSize > 1) {
            return UserFont ?? StdFont; // return UserFont only if valid
          }
          return StdFont;
        }
      }

      // returns the Standard Font (alloc when needed)
      public Font StdFont {
        get {
          if (_stdFont == null) {
            _stdFont = new Font( Name, Size, FontStyle, FontSizeUnit );
          }
          return _stdFont;
        }
      }

      // returns the UserFont (alloc when needed)
      public Font UserFont {
        get {
          if (_userFont == null) {
            try {
              _userFont = new Font( UserName, UserSize, UserFontStyle, UserFontSizeUnit );
            }
            catch {
              // reset as we cannot create this one anyway
              UserName = ""; UserSize = c_NilSize; UserFontSizeUnit = GraphicsUnit.Point; UserFontStyle = FontStyle.Regular;
              _userFont = null;
            }
          }
          return _userFont;
        }
      }
      private Font _stdFont = null;
      private Font _userFont = null;


      // Set the user Font for this item from a prototype
      public void SetUserFont( Font font )
      {
        _userFont?.Dispose( );
        _userFont = null;

        UserName = font.Name;
        UserSize = font.Size;
        UserFontSizeUnit = font.Unit;
        UserFontStyle = font.Style;
      }

      // Get the user font spec as config string
      // returns the "name;size;unit"
      public string GetUserFontSpec( )
      {
        return $"{UserName}¦{UserSize}¦{UserFontSizeUnit}¦{UserFontStyle}";
      }

      // Set a user font and allocate items
      public void SetUserFontSpec( string userFontSpec )
      {
        // reset
        _userFont?.Dispose( );
        _userFont = null;
        UserName = ""; UserSize = c_NilSize; UserFontSizeUnit = GraphicsUnit.Point; UserFontStyle = FontStyle.Regular;
        // ---
        if (string.IsNullOrWhiteSpace( userFontSpec )) return;

        // "Name¦Size¦SizeUnit¦Style" (see above for the string formatter)
        string[] e = userFontSpec.Split( new char[] { '¦' }, StringSplitOptions.RemoveEmptyEntries );
        if (e.Length != 4) return;
        // brute force...
        try {
          UserFontStyle = (FontStyle)Enum.Parse( typeof( FontStyle ), e[3] );
          UserFontSizeUnit = (GraphicsUnit)Enum.Parse( typeof( GraphicsUnit ), e[2] );
          UserSize = float.Parse( e[1] ) * (Size / BaseSize); // scale if needed;
          UserName = e[0];
        }
        catch (Exception ex) {
          Console.WriteLine( $"FontProvider.SetUserFontSpec - failed \n{ex}" );
          return;
        }
      }
      // dispose the allocated fonts
      public void DisposeFont( )
      {
        _stdFont?.Dispose( );
        _userFont?.Dispose( );
      }
    }// class

    // local store for the Font registered
    private Dictionary<string, FontSpec> _fontSpecs = new Dictionary<string, FontSpec>( );

    // a default font
    private readonly Font _defaultFont;

    /// <summary>
    /// cTor:
    /// </summary>
    public FontProvider( )
    {
      _defaultFont = new Font( "Arial", 9, FontStyle.Regular, GraphicsUnit.Point );
    }

    /// <summary>
    /// Returns the default font
    /// </summary>
    public Font DefaultFont => _defaultFont;

    /// <summary>
    /// Register the Fonts used by the App
    /// The first registered font is used as BaseFont and also provides the Config String
    /// </summary>
    /// <param name="fontDescriptor">A unique descriptor used by the App</param>
    /// <param name="font">A font</param>
    public void RegisterFont( string fontDescriptor, Font font )
    {
      if (_fontSpecs.Count == 0) {
        // the first one - set base values for further reference
        _baseDescriptor = fontDescriptor;
        _baseSize = font.Size;
      }
      var fs = new FontSpec( font, _baseSize );
      _fontSpecs.Add( fontDescriptor, fs );
    }

    /// <summary>
    /// Update the FontSpec with a base font from the user
    /// Sizes are updated proportionally
    /// </summary>
    /// <param name="font">A user provided font</param>
    public void SetUserFont( Font font )
    {
      foreach (var fs in _fontSpecs.Values) {
        fs.SetUserFont( font );
      }
    }


    /// <summary>
    /// Get a Font for a registered Descriptor
    /// </summary>
    /// <param name="fontDescriptor">A unique descriptor used by the App</param>
    /// <returns>A Font - BaseFont or finally DefaultFont if the sought is not found</returns>
    public Font GetFont( string fontDescriptor )
    {
      if (_fontSpecs.ContainsKey( fontDescriptor )) {
        return _fontSpecs[fontDescriptor].Font;
      }
      else if( _fontSpecs.ContainsKey( _baseDescriptor ) ) {
        return _fontSpecs[_baseDescriptor].Font;
      }
      return _defaultFont;
    }

    /// <summary>
    /// Returns a Config String for the User Font
    /// </summary>
    /// <returns>A Config String (can be empty)</returns>
    public string GetUserConfigString( )
    {
      if (_fontSpecs.ContainsKey( _baseDescriptor )) {
        return _fontSpecs[_baseDescriptor].GetUserFontSpec( );
      }
      return "";
    }

    /// <summary>
    /// Alloc User Fonts from Config String
    /// </summary>
    /// <param name="configString">The Config String</param>
    public void SetUserConfigString( string configString )
    {
      foreach (var fs in _fontSpecs.Values) {
        fs.SetUserFontSpec( configString );
      }
    }


    #region Dispose

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          foreach (var fs in _fontSpecs.Values) {
            fs.DisposeFont( );
          }
        }

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
