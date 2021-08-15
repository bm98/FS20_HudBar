using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FS20_HudBar.GUI;

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
    /// <param name="ctrl">The control to add (must implement IValue)</param>
    public void AddLbl( VItem item, Control ctrl )
    {
      this.Add( item, new ValueItem( ctrl ) );
    }

  }


}
