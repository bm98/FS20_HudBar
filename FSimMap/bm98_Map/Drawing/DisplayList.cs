using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bm98_Map.Drawing.DispItems;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// The list of items to paint
  /// </summary>
  internal class DisplayList : ConcurrentDictionary<int, IDrawing>
  {

    /// <summary>
    /// Add a DisplayItem to the DisplayList
    /// </summary>
    /// <param name="item">A Display Item</param>
    public void AddItem( DisplayItem item )
    {
      if (this.TryAdd( item.Key, item )) return;

      ; // DEBUG STOP
    }

    /// <summary>
    /// Remove a DisplayItem with key
    ///   TODO - may be use a concurrent List - else we may need to lock while using
    /// </summary>
    /// <param name="key">The key</param>
    public void RemoveItem( int key )
    {
      if (this.TryRemove( key, out IDrawing _ )) return;

      ; // DEBUG STOP
    }

    /// <summary>
    /// Returns the DisplayItem with Key (first found)
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>A DisplayItem or null</returns>
    public DisplayItem DispItem( int key )
    {
      // fast main collection return
      if (this.ContainsKey( key )) return (DisplayItem)this[key];
      // else track subitems
      foreach (var di in this.Values) {
        if (di is DisplayItem) {
          var dItem = di as DisplayItem;
          // try the subitems
          dItem = dItem.SubItemList.DispItem( key );
          if (dItem != null) return dItem;
        }
      }
      return null;
    }

    /// <summary>
    /// Does all paints 
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    public void Paint( Graphics g, IVPortPaint vpRef )
    {
      // the concurrent dict does not return the entries in the added order when enumerating the dict... but line segments prefer to be ordered...
      foreach (var i in this.Values.OrderBy( k => k.Key )) {
        i.Paint( g, vpRef );
      }
    }

    /// <summary>
    /// Add an Item to this DispItem as Sub List
    /// (note those are drawn on top of this Item)
    /// </summary>
    /// <param name="otherDisplayList">A DisplayList</param>
    public void AddSubitems( DisplayList otherDisplayList )
    {
      foreach (var di in otherDisplayList.Values) {
        if (di is DisplayItem) this.AddItem( di as DisplayItem );
      }
    }

  }
}
