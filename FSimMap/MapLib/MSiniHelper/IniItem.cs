using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MSiniLib
{

  /// <summary>
  /// Represents an Item in a Section
  /// ITEM=[VALUE][  ; COMMENT]
  ///   where VALUE is String | "quoted string"
  ///   and ITEM is a literal
  /// </summary>
  internal class IniItem
  {
    #region STATIC ITEM TOOLS

    #region Split INI Lines

    // Regex to match:  item  = content  (an item must start with a literal, content can by anyting)
    //  succeeds only when an Item = is found
    private readonly static Regex c_Item = new Regex(@"^(?<item>[^=]+)(\s*=)(?<cont>.*)?", RegexOptions.Compiled);

    /// <summary>
    /// Returns the content of an item (item=content)
    /// Note: the content is returned with any trailing comments if there are
    /// </summary>
    /// <param name="line">An INI line</param>
    /// <param name="itemName">Out: the items name</param>
    /// <param name="itemContent">Out: the item content</param>
    /// <returns>True if the format matched</returns>
    private static bool SplitItem( string line, out string itemName, out string itemContent )
    {
      itemName = ""; itemContent = ""; // defaults
      line = line.TrimStart( );
      if ( string.IsNullOrEmpty( line ) ) return false; // shortcut

      Match match = c_Item.Match( line );
      if ( match.Success ) {
        itemName = match.Groups["item"].Value.TrimEnd();
        itemContent = match.Groups["cont"].Success ? match.Groups["cont"].Value : "";
        return true;
      }
      return false;
    }


    // Derive if the string starts with a " and has a terminating " somewhere 
    private readonly static Regex c_dQuoted = new Regex(@"^("")(?<cont>.*)("")(?<rest>.*)?", RegexOptions.Compiled);

    // Catches the first ; and reports it as comment the leading part is cont
    // cont OR comment may be empty (comment is undef if there is no semi)
    private readonly static Regex c_semi    = new Regex(@"^(?<cont>[^;]*)(?<comment>;.*)?", RegexOptions.Compiled);

    private static bool SplitContent( string itemContent, out string value, out string comment )
    {
      /*
       content:
        |any content
        |any content ; comment
        |"quoted content" ignored content
        |"quoted ; content" ; comment
       */
      value = ""; comment = ""; // defaults
      if ( string.IsNullOrEmpty( itemContent ) ) return true; // shortcut

      Match match = c_dQuoted.Match(itemContent); // starting " needed
      if ( match.Success ) {
        value = "\"" + match.Groups["cont"].Value + "\""; // can be empty between quotes
        var rest = match.Groups["rest"].Success ? match.Groups["rest"].Value : ""; // can be empty
        if ( !string.IsNullOrEmpty( rest ) ) {
          match = c_semi.Match( rest );
          if ( match.Success ) {
            comment = match.Groups["comment"].Success ? match.Groups["cont"].Value : ""; // can be empty
            // remove the starting ;
            if ( comment.Length > 1 ) comment = comment.Remove( 0, 1 );
            else if ( comment.Length > 0 ) comment = "";
          }
        }
        return true;
      }

      // not quoted
      match = c_semi.Match( itemContent ); // may or may not have a semi
      if ( match.Success ) {
        value = match.Groups["cont"].Success ? match.Groups["cont"].Value : ""; // can be empty
        comment = match.Groups["comment"].Success ? match.Groups["comment"].Value : ""; // can be empty
        return true;
      }

      return false;
    }


    /// <summary>
    /// Split an INI line into pieces
    /// </summary>
    /// <param name="line">An INI line</param>
    /// <param name="itemName">The Item Name</param>
    /// <param name="value">The Item Value</param>
    /// <param name="comment">The Item Comment</param>
    /// <returns></returns>
    internal static bool SplitLine( string line, out string itemName, out string value, out string comment )
    {
      itemName = ""; value = ""; comment = ""; // defaults

      // Split from A=B
      if ( SplitItem( line, out string iName, out string iCont ) ) {
        itemName = iName;
        // Split B part
        if ( SplitContent( iCont, out string iValue, out string iComment ) ) {
          value = iValue;
          comment = iComment;
          return true;
        }
      }
      return false;
    }

    #endregion

    // Skip to Item and return it
    // returns the line read that contains an item or an empty string
    internal static string NextItem( TextReader reader )
    {
      string line;
      do {
        if ( reader.Peek( ) == -1 ) return ""; // EOF
        if ( Convert.ToChar( reader.Peek( ) ) == '[' ) {
          return ""; // just the start of a new section 
        }
        // read and use a line (cannot be null, checked above)
        line = reader.ReadLine( );
        if ( string.IsNullOrWhiteSpace( line ) ) {
          ; // skip empty lines
        }
        else if ( line.TrimStart( ).StartsWith( ";" ) ) {
          ; // skip line comments
        }
        else {
          return line;
        }
      } while ( line != null );

      return ""; // no item found
    }

    /// <summary>
    /// Returns the Lookup Name for an Item
    /// </summary>
    /// <param name="itemName">A plain Item name</param>
    /// <returns>A Lookup Name</returns>
    internal static string CatalogName( string itemName )
    {
      return itemName.ToUpperInvariant( );
    }

    #endregion


    //***** CLASS


    /// <summary>
    /// The Item Name
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The item value as string
    /// </summary>
    public string Value { get; private set; }
    /// <summary>
    /// The item comment as string
    /// </summary>
    public string Comment { get; private set; }

    /// <summary>
    /// True if the item is valid
    /// </summary>
    public bool Valid { get; private set; } = false;



    /// <summary>
    /// cTor: create a item from a line
    /// will be stripped from blanks
    /// 
    /// Throws: ArgumentException if the item name is empty or whitespace only
    /// </summary>
    /// <param name="line">The INI Line</param>
    public IniItem( string line )
    {
      if ( SplitLine( line, out string iName, out string iValue, out string iComment ) ) {
        Name = iName.TrimEnd();
        Value = iValue.TrimEnd( );
        Comment = iComment.TrimEnd( );
        Valid = true;
      }
    }


    /// <summary>
    /// cTor: create a item with a name (shall not be an empty name)
    ///        and a value string
    /// the item name will be stripped from blanks
    /// 
    /// Throws: ArgumentException if the item name is empty or whitespace only
    /// </summary>
    /// <param name="itemName">The item Name</param>
    /// <param name="value">The item value string(can be empty)</param>
    /// <param name="comment">An optional comment (can be empty)</param>
    public IniItem( string itemName, string value, string comment = "" )
    {
      if ( string.IsNullOrWhiteSpace( itemName ) ) throw new ArgumentException( "The Item name cannot be empty" );

      Name = itemName.Trim( );
      Value = value;
      Comment = comment;
      Valid = true;
    }

    /// <summary>
    /// Add or replace the current value
    /// </summary>
    /// <param name="value">A value (can be empty)</param>
    public void SetValue( string value )
    {
      Value = value;
    }


    /// <summary>
    /// Add or replace the current comment
    /// </summary>
    /// <param name="comment">A comment (can be empty)</param>
    public void SetComment( string comment )
    {
      Comment = comment;
    }

    /// <summary>
    /// Write this item to the stream
    /// </summary>
    /// <param name="streamWriter">A writable stream</param>
    public void Write( StreamWriter streamWriter )
    {
      if ( !Valid ) return; // cannot write invalid items

      streamWriter.WriteLine( this.ToString() );
    }

    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString( )
    {
      if ( !Valid ) return ""; 

      string line = $"{Name}={Value}";
      if ( !string.IsNullOrEmpty( Comment ) ) {
        line += $"  ; {Comment}";
      }
      return line;
    }

  }
}
