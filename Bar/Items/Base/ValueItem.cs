using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items.Base
{
  /// <summary>
  /// A Value Item, carries the IValue Interface and the associated Control 
  /// </summary>
  internal class ValueItem
  {
    private object m_ctrl = null;

    /// <summary>
    /// The IValue Interface
    /// </summary>
    public IValue Value => (IValue)m_ctrl;

    /// <summary>
    /// The IColorType Interface
    /// </summary>
    public IColorType ColorType => (IColorType)m_ctrl;

    /// <summary>
    /// The associated Control
    /// </summary>
    public Control Ctrl => (Control)m_ctrl;


    /// <summary>
    /// cTor: Create a ValueItem exposing the Interface or the underlying Control
    /// </summary>
    /// <param name="ctrl">An object derived from Control, implementing an IValue interface</param>
    public ValueItem( Control ctrl )
    {
      m_ctrl = ctrl;
    }

  }
}
