using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MS
{
  /// <summary>
  /// Simple INI file reader/writer
  /// ini files are files composed from:
  /// sections as:
  /// [SECTION]
  /// lines as:
  /// ITEM=[CONTENT] [;COMMENT]  ([] items are optional)
  /// Content can be quoted "content bla" anything and will be returned as quoted part only
  /// Comments are lead by a ; (semicolon)
  /// lines may apear before, or within a section
  /// The SECTION name for the lines before any section starts use an empty section "" qualifier
  /// 
  /// </summary>
  internal class MSiniFile
  {
    /// <summary>
    /// Supported Encodings for INI files
    /// </summary>
    public enum IniEncoding
    {
      UTF8,
      Unicode,
      ANSI,
      iso_8859_1,
    }

    // this filename
    private string _fileName = "";
    // sections of this file
    private SectionCat _sections = new SectionCat( );
    // encoding used for read/write of files
    private Encoding _encoding = Encoding.UTF8;

    private bool _valid = false;
    private bool _unquote = false;


    // Read the Inifile from the stream
    private void LoadStreamLow( Stream stream )
    {
      _valid = false;
      using (var reader = new StreamReader( stream )) {
        // this shall never throw - we use IsValid to evaluate the outcome
        try {
          IniSection.GetMainSection( _sections, reader );
          bool result = IniSection.GetNextSection( _sections, reader );
          while (result) {
            result = IniSection.GetNextSection( _sections, reader );
          }
          //
          _valid = true; // only here we decide
        }
        catch (Exception ex) {
          Console.WriteLine( "MSiniFile: Cannot Read " + _fileName );
          Console.WriteLine( ex.Message );
          return; // ERROR - remains NOT Valid
        }
      }
    }

    /// <summary>
    /// Get the quoted part of this string
    /// </summary>
    /// <param name="inp">Input string where a quoted part must be extracted</param>
    /// <param name="quoteChar">The quote character, defaults to " (double quote)</param>
    /// <returns>The unqoted string</returns>
    private static string FromQuoted( string inp, char quoteChar = '"' )
    {
      var sx = inp.ToArray( )
        .SkipWhile( c => c != quoteChar )
        .Skip( 1 )
        .TakeWhile( c => c != quoteChar );
      var v = new string( sx.ToArray( ) );
      return v;
    }


    // Set the File Encoding to the ISO-8859-1 codepage (Western European ISO)
    //  this is preferred for MSFS (it seems they are not on UTF8 for at least the FLT files)
    private void SetEncodingISO_8859_1( )
    {
      _encoding = Encoding.GetEncoding( "iso-8859-1" );
    }

    // Set the File Encoding to UTF8
    private void SetEncodingUTF8( )
    {
      _encoding = Encoding.UTF8;
    }

    // Set the File Encoding to UNICODE (UTF16 little endian byte order)
    private void SetEncodingUNICODE( )
    {
      _encoding = Encoding.Unicode;
    }

    // Set the File Encoding to ANSI 
    private void SetEncodingANSI( )
    {
     // DISABLED IN FlightBag Implementation
     //  _encoding = CodePagesEncodingProvider.Instance.GetEncoding( 1252 );
    }

    /// <summary>
    /// Returns true if the file is valid
    /// </summary>
    public bool IsValid => _valid;


    /// <summary>
    /// Get; The Sections of this file
    /// </summary>
    public SectionCat SectionCatalog => _sections;


    /// <summary>
    /// cTor: empty
    /// </summary>
    public MSiniFile( ) { }

    /// <summary>
    /// cTor: Create an Ini File from a filename
    ///  Note: default encoding is UTF8 
    ///    if you need another encoding:
    ///    - create an empty object
    ///    - Load(file, encoding) 
    ///    
    ///   You may set an other encoding before Writing
    ///   
    /// </summary>
    /// <param name="fileName">Fully qualified path and name</param>
    public MSiniFile( string fileName )
    {
      this.Load( fileName );
    }

    /// <summary>
    /// cTor: Create an Ini File from a stream
    ///  Note: default encoding is UTF8 
    ///    if you need another encoding:
    ///    - create an empty object
    ///    - Load(stream, encoding) 
    ///    
    ///   You may set an other encoding before Writing
    ///   
    /// </summary>
    /// <param name="stream">An open stream</param>
    public MSiniFile( Stream stream )
    {
      this.Load( stream );
    }

    /// <summary>
    /// Set the File Encoding for writing
    /// </summary>
    public void SetEncoding( IniEncoding encoding )
    {
      switch (encoding) {
        case IniEncoding.UTF8: SetEncodingUTF8( ); break;
        case IniEncoding.Unicode: SetEncodingUNICODE( ); break;
        case IniEncoding.ANSI: SetEncodingANSI( ); break;
        case IniEncoding.iso_8859_1: SetEncodingISO_8859_1( ); break;
        default: SetEncodingUTF8( ); break;
      }
    }

    /// <summary>
    /// Set whether we read and Unquote values or not
    /// </summary>
    /// <param name="unqoting">True to unquote, else false</param>
    public void SetUnqoteValues( bool unqoting )
    {
      _unquote = unqoting;
    }



    /// <summary>
    /// Set a new filename for this INI file for writing
    /// </summary>
    /// <param name="fileName">A filename</param>
    public void SetFilename( string fileName )
    {
      _fileName = fileName;
    }


    /// <summary>
    /// Load from a stream (encoding is UTF8)
    /// </summary>
    /// <param name="stream">An open stream</param>
    public void Load( Stream stream )
    {
      _valid = false;
      _sections.Clear( );
      _fileName = "";
      if (stream.Length < 1) return; // ERROR too short...

      _fileName = "$$$STREAM$$$";
      LoadStreamLow( stream ); // get all
    }

    /// <summary>
    /// Load a new file
    /// 
    ///  Note: default encoding is iso-8859-1 (MSFS FLT encoding) 
    /// </summary>
    /// <param name="fileName">Fully qualified path and name</param>
    public void Load( string fileName, IniEncoding encoding = IniEncoding.iso_8859_1 )
    {
      _valid = false;
      _sections.Clear( );
      _fileName = "";
      if (!File.Exists( fileName )) return; // ERROR file does not exist..

      _fileName = fileName;
      SetEncoding( encoding );
      byte[] byt;
      try {
        using (var ts = File.Open( _fileName, FileMode.Open, FileAccess.Read, FileShare.Read )) {
          // convert the file content
          byt = new byte[ts.Length];
          ts.Read( byt, 0, byt.Length );
          var iString = Encoding.UTF8.GetString( Encoding.Convert( _encoding, Encoding.UTF8, byt ) );
          using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( iString ) )) {
            LoadStreamLow( ms );
          }
        }
      }
      catch { }
      finally {
        byt = new byte[0]; // help the GC
      }
    }

    /// <summary>
    /// Write the INI file to the set filename
    ///  Note: default encoding is UTF8 
    ///    if you need another encoding:
    ///    - Set another encoding
    ///    - Write()
    /// </summary>
    public void Write( )
    {
      if (!string.IsNullOrEmpty( Path.GetDirectoryName( _fileName ) ))
        if (!Directory.Exists( Path.GetDirectoryName( _fileName ) )) return; // ERROR no dir for this file

      // this shall never throw
      try {
        using (var sw = new StreamWriter( _fileName, false, _encoding )) {
          _sections.WriteAll( sw );
        }

      }
      catch {
        ; // DEBUG
      }
    }

    /// <summary>
    /// Set the Value for an item
    ///   will create section and item if needed, else it overwrites
    /// </summary>
    /// <param name="sectionName">A section name (can be empty for the main section)</param>
    /// <param name="itemName">The item name (cannot be empty)</param>
    /// <param name="value">A value to set (can be empty)</param>
    public bool SetValue( string sectionName, string itemName, string value )
    {
      if (string.IsNullOrEmpty( itemName )) return false;

      var section = _sections.GetSection( sectionName );
      if (section == null) {
        section = new IniSection( sectionName );
        _sections.Add( section );
      }

      var item = section.Items.GetItem( itemName );
      if (item == null) {
        item = new IniItem( itemName, value );
        section.Items.Add( item );
      }
      else {
        item.SetValue( value ); // overwrite existing value
      }

      return true;
    }

    /// <summary>
    /// Returns all lines of a single section [section]
    ///  Item=Value ; Comment
    /// Note: the match is case insensitive
    /// </summary>
    /// <param name="sectionName">The section name</param>
    /// <returns>The list of lines belonging to the section (can be an empty list)</returns>
    public List<string> GetSection( string sectionName )
    {
      var section = _sections.GetSection( sectionName );
      if (section == null) return new List<string>( ); // empty one

      return section.Items.ItemList( );
    }

    /// <summary>
    /// Returns the value of an item 
    /// Note: the match is case insensitive
    /// </summary>
    /// <param name="sectionName">The section name</param>
    /// <param name="item">The item sought</param>
    /// <returns>The item value (can be empty string)</returns>
    public string ItemValue( string sectionName, string item )
    {
      var section = _sections.GetSection( sectionName );
      if (section == null) return ""; // empty one

      var it = section.Items.GetItem( item );
      if (it == null) return ""; // empty one

      return _unquote ? FromQuoted( it.Value ) : it.Value;
    }

    /// <summary>
    /// Returns a value as number for a key in a section
    ///  use the "" to access the ones not contained in a section
    /// Note: the match is case insensitive
    /// </summary>
    /// <param name="sectionName">The section name</param>
    /// <param name="item">The item sought</param>
    /// <returns>A number or float.MinValue if not found or not a number</returns>
    public float ItemNumber( string sectionName, string item )
    {
      string value = ItemValue( sectionName, item );
      if (string.IsNullOrWhiteSpace( value )) return float.MinValue;
      if (float.TryParse( value, out float num )) {
        return num;
      }
      return float.MinValue;
    }

  }
}
