using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MS
{
  /// <summary>
  /// The Catalog of INI Sections
  ///  NOTE the main section of an ini file as no section name
  /// </summary>
  internal class SectionCat
  {
    // the cat
    private Dictionary<string, IniSection> _catalog = new Dictionary<string, IniSection>();

    /// <summary>
    /// The number of items in the catalog
    /// </summary>
    public int Count => _catalog.Count;

    /// <summary>
    /// Clear all items from this catalog
    /// </summary>
    public void Clear( ) => _catalog.Clear( );

    /// <summary>
    /// The items of this Catalog as Enumerable
    /// </summary>
    public IEnumerable<IniSection> Sections => _catalog.Values;
    /// <summary>
    /// Add a section
    /// </summary>
    /// <param name="section">The Section</param>
    /// <returns>True if added</returns>
    public bool Add( IniSection section )
    {
      // subst the section name with our Lookup Name
      string sectionName = IniSection.CatalogName( section.Name );

      if ( !_catalog.ContainsKey( sectionName ) ) {
        _catalog.Add( sectionName, section );
        return true;
      }
      else {
        return false; // duplicate section name..
      }
    }

    /// <summary>
    /// Returns a Section of this name or null if not found
    /// </summary>
    /// <param name="sectionName">The Section name</param>
    /// <returns>The Section or Null if not found</returns>
    public IniSection GetSection( string sectionName )
    {
      sectionName = IniSection.CatalogName( sectionName ); // make a searchable name
      if ( _catalog.ContainsKey( sectionName ) ) {
        return _catalog[sectionName];
      }
      return null;
    }

    /// <summary>
    /// Remove a section with sectionName
    /// </summary>
    /// <param name="sectionName">The Section name</param>
    public void Remove( string sectionName )
    {
      // subst the section name with our Lookup Name
      sectionName = IniSection.CatalogName( sectionName );

      _catalog.Remove( sectionName );
    }

    /// <summary>
    /// Write all sections of this Catalog
    /// </summary>
    /// <param name="streamWriter">A prepared streamwriter</param>
    public void WriteAll( StreamWriter streamWriter )
    {
      foreach ( var section in _catalog.Values ) {
        section.Write( streamWriter );
      }
    }

  }
}
