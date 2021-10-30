using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Distance indicator with a directional arrow up to 999.9 shown
  /// </summary>
  class V_DmeDist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_DmeDist( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "nm";
      m_default = DefaultString( "___._ " );
      Text = UnitString( m_default );
    }

    private string c_from = "↓";
    private string c_to = "↑";
    private string c_flat = " ";

    /// <summary>
    /// Set the value of the Control
    ///  pos. values indicate dist towards the target
    ///  neg. values indicate dist from the target
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = UnitString( m_default );
        }
        else if ( Math.Abs( (float)value ) >= 1000.0f ) {
          this.Text = UnitString( "> 999 " );
        }
        else {
          if ( value > 0 ) {
            this.Text = UnitString( $"{value,5:##0.0}{c_to}" );
          }
          else if ( value < 0 ) {
            this.Text = UnitString( $"{-value,5:##0.0}{c_from}" );
          }
          else {
            this.Text = UnitString( $"{value,5:##0.0}{c_flat}" );
          }
        }
      }
    }

    #region STATIC DME Dist Sign

    /// <summary>
    /// Returns a signed distance for the DME readout Control V_DistDme 
    /// flag==1 => To   + signed
    /// flag==2 => From - signed
    /// flag==0 => Off  NaN
    /// 
    /// </summary>
    /// <param name="absValue"></param>
    /// <param name="fromToFlag"></param>
    /// <returns></returns>
    public static float DmeDistance( float absValue, int fromToFlag )
    {
      return fromToFlag == 0 ? float.NaN : fromToFlag == 1 ? absValue : -absValue;
    }
    #endregion


  }
}
