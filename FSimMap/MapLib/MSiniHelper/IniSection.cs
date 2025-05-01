using System.IO;

namespace MSiniLib
{
  /// <summary>
  /// Represents a Section from an INI file
  /// [SECTION]
  /// ITEM=[VALUE][  ; COMMENT]
  ///   where VALUE is String | "quoted string"
  ///   and ITEM is a literal
  /// 
  /// </summary>
  internal class IniSection
  {
    #region STATIC Section Tools

    // Skip to Section and return it
    // returns the line read that contains a section name ([section])or an empty string
    private static string NextSection( TextReader reader )
    {
      string line;
      do {
        if ( reader.Peek( ) == -1 ) return ""; // EOF

        // read and use a line (cannot be null, checked above)
        line = reader.ReadLine( );
        if ( string.IsNullOrWhiteSpace( line ) ) {
          ; // skip empty lines
        }
        else if ( line.TrimStart( ).StartsWith( ";" ) ) {
          ; // skip line comments
        }
        else if ( line.TrimStart( ).StartsWith( "[" ) ) {
          return line;
        }
        else {
          ; // skip ?? should not really happen
        }
      } while ( line != null );

      return ""; // no item found
    }

    // read and add any further section
    // returns true if a section was found and added
    internal static bool GetNextSection( SectionCat sections, TextReader reader )
    {
      var line = NextSection(reader);
      if ( !string.IsNullOrEmpty( line ) ) {
        IniSection section = new IniSection( line.Replace("[", "" ).Replace("]", "" )); //  section name is stripped from []
        sections.Add( section ); // add also empty sections as they are valid and may be used

        // Get the section items - TODO get Comments read too
        line = IniItem.NextItem( reader );
        while ( !string.IsNullOrEmpty( line ) ) {
          IniItem item = new IniItem(line);
          if ( item.Valid ) {
            section.Items.Add( item );
          }
          //
          line = IniItem.NextItem( reader );
        }
        return true;
      }
      else {
        return false; // no further section found
      }
    }

    // read the main section (i.e. the one without [section] before
    internal static void GetMainSection( SectionCat sections, TextReader reader )
    {
      IniSection section = new IniSection(""); //  Main section name is empty

      // Get the section items - TODO get Comments read too
      var line = IniItem.NextItem( reader );
      while ( !string.IsNullOrEmpty( line ) ) {
        IniItem item = new IniItem(line);
        if ( item.Valid ) {
          section.Items.Add( item );
        }
        //
        line = IniItem.NextItem( reader );
      }
      // add section if not empty
      if ( section.Items.Count > 0 ) {
        sections.Add( section );
      }
    }

    /// <summary>
    /// Returns the Lookup Name for a section
    /// </summary>
    /// <param name="sectionName">A plain sectionname (can be empty for the Main section)</param>
    /// <returns>A Lookup Name</returns>
    internal static string CatalogName( string sectionName )
    {
      return string.IsNullOrEmpty( sectionName ) ? MainSection : sectionName.ToUpperInvariant( );
    }

    #endregion


    //***** CLASS
    // internally we use "__$$M_A_I_N$$__" as Main section name (hopefully nobody uses it...)
    // The Key is stored in UCase form as the search is case invariant

    /// <summary>
    /// The Libraries internal Section name of the Main section
    /// </summary>
    private const string MainSection = "__$$M_A_I_N$$__";

    // the items of this section
    private ItemCat _items = new ItemCat();

    /// <summary>
    /// The Section Name
    /// </summary>
    public string Name { get; private set; } = "";

    /// <summary>
    /// cTor: create an empty section with a name (Unique in a file !!)
    /// </summary>
    /// <param name="sectionName">The Name of the section</param>
    public IniSection( string sectionName )
    {
      if ( string.IsNullOrEmpty( sectionName ) )
        sectionName = MainSection;

      Name = sectionName;
    }

    /// <summary>
    /// Get; Items of this section
    /// </summary>
    public ItemCat Items => _items;


    /// <summary>
    /// Write this section to the stream
    /// </summary>
    /// <param name="streamWriter">A writable stream</param>
    public void Write( StreamWriter streamWriter )
    {
      streamWriter.WriteLine( ); // prepend with an empty line
      // Write the Section Name only if not MAIN
      if ( Name != MainSection )
        streamWriter.WriteLine( $"[{Name}]" );

      // dump all items
      _items.WriteAll( streamWriter );
    }

  }
}
