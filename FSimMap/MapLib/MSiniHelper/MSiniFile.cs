using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MSiniLib
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
    // this filename
    private string _fileName = "";
    // sections of this file
    private SectionCat _sections = new SectionCat();
    // encoding used for read/write
    private Encoding _encoding = Encoding.UTF8;

    // Load the file content
    private void LoadFileLow( )
    {
      if ( !File.Exists( _fileName ) ) return; // ERROR file does not exist..
      using ( var tr = new StreamReader( _fileName , _encoding) ) {
        IniSection.GetMainSection( _sections, tr );
        bool result = IniSection.GetNextSection(_sections, tr);
        while ( result ) {
          result = IniSection.GetNextSection( _sections, tr );
        }
      }
    }

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
    ///    - Set another encoding
    ///    - Load(file) 
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
    /// Set the File Endoding to the ISO-8859-1 codepage (Western European ISO)
    ///  this is preferred for MSFS (it seems they are not on UTF8 for at least the FLT files)
    /// </summary>
    public void SetEncodingANSI( )
    {
      _encoding = Encoding.GetEncoding( "iso-8859-1" );
    }

    /// <summary>
    /// Set the File Endoding to UTF8
    /// </summary>
    public void SetEncodingUTF8( )
    {
      _encoding = Encoding.UTF8;
    }

    /// <summary>
    /// Set the File Endoding to UNICODE (UTF16 little endian byte order)
    /// </summary>
    public void SetEncodingUNICODE( )
    {
      _encoding = Encoding.Unicode;
    }

    /// <summary>
    /// Set a new filename for this INI file
    /// </summary>
    /// <param name="fileName">A filename</param>
    public void SetFilename( string fileName )
    {
      _fileName = fileName;
    }


    /// <summary>
    /// Load a new file
    /// 
    ///  Note: default encoding is UTF8 
    ///    if you need another encoding:
    ///    - Set another encoding
    ///    - Load(file) 
    /// </summary>
    /// <param name="fileName">Fully qualified path and name</param>
    public void Load( string fileName )
    {
      _sections.Clear( );
      _fileName = "";
      if ( !File.Exists( fileName ) ) return; // ERROR file does not exist..

      _fileName = fileName;
      LoadFileLow( ); // get all
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
      if ( !string.IsNullOrEmpty( Path.GetDirectoryName( _fileName ) ) )
        if ( !Directory.Exists( Path.GetDirectoryName( _fileName ) ) ) return; // ERROR no dir for this file

      // this shall never throw
      try {
        using ( var sw = new StreamWriter( _fileName, false, _encoding ) ) {
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
    public bool SetValue(string sectionName, string itemName, string value )
    {
      if ( string.IsNullOrEmpty( itemName ) ) return false;

      var section = _sections.GetSection(sectionName);
      if ( section == null ) {
        section = new IniSection( sectionName );
        _sections.Add( section );
      }

      var item = section.Items.GetItem(itemName);
      if (item == null ) {
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
      var section = _sections.GetSection(sectionName);
      if ( section == null ) return new List<string>( ); // empty one

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
      var section = _sections.GetSection(sectionName);
      if ( section == null ) return ""; // empty one

      var it = section.Items.GetItem(item);
      if ( it == null ) return ""; // empty one

      return it.Value;
    }


  }
}
