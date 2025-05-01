using System.Collections.Generic;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Catalog of ValueItems
  /// KEY: Value Item Enum
  /// VALUE: ValueItem
  /// </summary>
  internal class ValueItemCat : Dictionary<VItem, ValueItem>
  {

    /// <summary>
    /// Add a control to the Catalog
    /// </summary>
    /// <param name="item">The VItem enum</param>
    /// <param name="ctrl">The control to add</param>
    public void AddLbl( VItem item, object ctrl )
    {
      this.Add( item, new ValueItem( ctrl ) );
    }

  }


}
