using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// A Catalog of DispItems
  /// KEY: The Label Enum (LItem)
  /// VALUE: the DispItem
  /// </summary>
  internal class DispItemCat : Dictionary<LItem, DispItem>
  {
    /// <summary>
    /// Create a DispItem add it to the collection and return it 
    /// </summary>
    /// <param name="item">The Label Item Enum (key)</param>
    /// <returns>The newly created dispItem</returns>
    public DispItem CreateDisp( LItem item )
    {
      var di = new DispItem();
      this.Add( item, di );
      return di;
    }

    // add a DispItem to the collection
    public void AddDisp( LItem item, DispItem disp )
    {
      this.Add( item, disp );
    }

  }

}
