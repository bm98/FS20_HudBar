using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// A button that shows the label as %
  /// </summary>
  class B_Prct : B_Base
  {
    /// <summary>
    /// cTor: Create a UserControl..
    /// </summary>
    /// <param name="item">The GITem ID of this one</param>
    /// <param name="proto">A label Prototype to derive from</param>
    public B_Prct( VItem item, Label proto )
      :base(item, proto)
    {
      m_unit = "";
      m_default = DefaultString( "+___%" );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = UnitString( m_default );
        }
        else {
          this.Text = UnitString( $"{value,4:#0%}" );  // sign 2 digits %
        }
      }
    }


  }
}
