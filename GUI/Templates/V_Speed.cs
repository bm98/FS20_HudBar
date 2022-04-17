using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Speed Formatter
  /// </summary>
  class V_Speed : V_Base
  {

    private const string c_ias = "";
    private const string c_mach ="";
    private bool _machMode = false;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Speed( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "kt";
      _machMode = false;
      m_default = DefaultString( "____ " ); // NNNN 
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Wether or not we are in MACH mode
    /// </summary>
    public bool MachMode {
      get => _machMode;
      set {
        _machMode = value;
        m_unit = _machMode ? "M " : "kt";
      }
    }
    /// <summary>
    /// Set the value of the Control
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
          if ( _machMode ) {
            this.Text = UnitString( $"{value,4:0.00} " ); // 1.2 digits, add a blank for alignment
          }
          else {
            // kts Mode
            this.Text = UnitString( $"{value,4:###0} " ); // positive only 4 digits, add a blank for alignment
          }
        }
      }
    }

  }
}

