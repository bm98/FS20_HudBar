using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile.Drawing
{
  /// <summary>
  /// The list of items to paint
  /// </summary>
//  internal class DisplayList : List<IDrawing>
  internal class DisplayList : Dictionary<int, IDrawing>
  {

    /// <summary>
    /// Add a DisplayItem to the DisplayList
    /// </summary>
    /// <param name="item"></param>
    public void AddItem( DisplayItem item )
    {
      this.Add( item.Key, item );
    }

    /// <summary>
    /// Remove a DisplayItem with key
    /// </summary>
    /// <param name="key">The key</param>
    public void RemoveItem( int key )
    {
      if ( this.ContainsKey( key ) )
        this.Remove( key );
    }

    /// <summary>
    /// Returns the DisplayItem with Key (first found)
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>A DisplayItem or null</returns>
    public DisplayItem DispItem( int key )
    {
      // fast main collection return
      if ( this.ContainsKey( key ) ) return (DisplayItem)this[key];
      // else track subitems
      foreach ( var di in this.Values ) {
        if ( di is DisplayItem ) {
          var dItem = di as DisplayItem;
          // try the subitems
          dItem = dItem.SubItemList.DispItem( key );
          if ( dItem != null ) return dItem;
        }
      }
      return null;
    }

    /// <summary>
    /// Deactivates all DisplayItems
    /// </summary>
    public void DeactivateAllDisplayItems( )
    {
      foreach ( var di in this.Values ) {
        if ( di is DisplayItem ) {
          var dItem = di as DisplayItem;
          dItem.Pressed = false;
          dItem.SubItemList.DeactivateAllDisplayItems( );
        }
      }
    }

    /// <summary>
    /// Does all paints 
    /// </summary>
    /// <param name="g">Graphics context</param>
    public void Paint( Graphics g )
    {
      foreach ( var i in this.Values ) {
        i.Paint( g );
      }
    }

    public void AddSubitems( DisplayList otherList )
    {
      foreach ( var di in otherList.Values ) {
        if ( di is DisplayItem ) this.AddItem( di as DisplayItem );
      }
    }

  }
}
