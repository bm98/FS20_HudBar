using System;
using System.Collections.Generic;
using System.IO;

namespace MSiniLib
{
  /// <summary>
  /// A catalog of Items 
  /// </summary>
  internal class ItemCat
  {
    // The cat
    private Dictionary<string, IniItem> _catalog = new Dictionary<string, IniItem>();

    /// <summary>
    /// The number of items in the catalog
    /// </summary>
    public int Count => _catalog.Count;

    /// <summary>
    /// Clear all items from this catalog
    /// </summary>
    public void Clear( ) => _catalog.Clear( );

    /// <summary>
    /// Add an Item
    /// </summary>
    /// <param name="item">The Item</param>
    /// <returns>True if added</returns>
    public bool Add( IniItem item )
    {
      if ( !item.Valid ) return false; // cannot add invalid items

      string itemName = IniItem.CatalogName( item.Name );
      if ( !_catalog.ContainsKey( itemName ) ) {
        _catalog.Add( IniItem.CatalogName( item.Name ), item );
        return true;
      }
      else {
        return false; // Duplicate Name
      }
    }

    /// <summary>
    /// Returns am Item of this name or null if not found
    ///  case insensitive search
    /// </summary>
    /// <param name="itemName">The Item name</param>
    /// <returns>The Section or Null if not found</returns>
    public IniItem GetItem( string itemName )
    {
      if ( string.IsNullOrWhiteSpace( itemName ) ) throw new ArgumentException( "The Item name cannot be empty" );

      itemName = IniItem.CatalogName( itemName );
      if ( _catalog.ContainsKey( itemName ) ) {
        return _catalog[itemName];
      }
      return null;
    }

    /// <summary>
    /// Remove an item with itemName
    /// </summary>
    /// <param name="itemName">The Item name</param>
    public void Remove( string itemName )
    {
      // subst the section name with our Lookup Name
      itemName = IniItem.CatalogName( itemName );

      _catalog.Remove( itemName );
    }

    /// <summary>
    /// Returns all items of this catalog as a string list
    ///  Item=Value ; Comment
    /// </summary>
    /// <returns>A List of strings</returns>
    public List<string> ItemList( )
    {
      var li = new List<string>();
      foreach ( var item in _catalog ) {
        if ( item.Value.Valid )
          li.Add( item.Value.ToString( ) );
      }
      return li;
    }

    /// <summary>
    /// Write all items
    /// </summary>
    /// <param name="streamWriter">A prepared streamwriter</param>
    public void WriteAll( StreamWriter streamWriter )
    {
      foreach ( var item in _catalog.Values ) {
        item.Write( streamWriter );
      }
    }

  }
}
