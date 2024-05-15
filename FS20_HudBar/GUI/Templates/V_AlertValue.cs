using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Type of alert
  /// </summary>
  public enum AlertType
  {
    /// <summary>
    /// Alert OFF
    /// </summary>
    OFF = 0,
    /// <summary>
    /// Altitude  [ft or m]
    /// </summary>
    ALT,
    /// <summary>
    /// AOG Altitude  [ft or m]
    /// </summary>
    AOG,
    /// <summary>
    /// Vertical Rate  [fpm or mps]
    /// </summary>
    VS,
    /// <summary>
    /// Airspeed  [kt or kmh]
    /// </summary>
    SPD,
    /// <summary>
    /// Distance  [nm or km]
    /// </summary>
    DIST,
    /// <summary>
    /// Time  [Min]
    /// </summary>
    TIME,
  }

  class V_AlertValue : V_Base
  {
    // type of alert
    private AlertType _alertType = AlertType.ALT;

    private const string c_up = "↑";
    private const string c_do = "↓";
    private const string c_flat = " ";

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_AlertValue( Label proto )
    : base( proto )
    {
      m_unit = "ft";
      _width = 8;
      m_default = DefaultString( "______" ); // -nn,nnn + blank
      Text = UnitString( m_default );
      SetDistance_Metric( ); // update if needed
    }

    /// <summary>
    /// Set the type of Alert Value 
    /// </summary>
    public AlertType AlertValueType {
      get => _alertType;
      set {
        _alertType = value;
        SetDistance_Metric( ); // update if needed
      }
    }

    protected override void SetDistance_Metric( )
    {
      switch (_alertType) {
        case AlertType.ALT:
        case AlertType.AOG:
          m_unit = _altitude_metric ? "m" : "ft";
          break;
        case AlertType.VS:
          m_unit = _altitude_metric ? "m/s" : "fpm";
          break;
        case AlertType.SPD:
          m_unit = _distance_metric ? "km/h" : "kt";
          break;
        case AlertType.DIST:
          m_unit = _distance_metric ? "km" : "nm";
          break;
        case AlertType.TIME:
          m_unit = "Min";
          break;

        default:
          m_unit = "_";
          break;
      }
    }

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if (value == null) {
          this.Text = RightAlign( m_default );
        }
        else if (float.IsNaN( (float)value )) {
          this.Text = RightAlign( m_default );
        }
        else {
          float uValue = 0;
          switch (_alertType) {

            case AlertType.ALT:
            case AlertType.AOG:
              uValue = _altitude_metric ? (float)M_From_Ft( (float)value ) : (float)value;
              this.Text = UnitString( RightAlign( $"{uValue,7:##,##0} " ) );
              break;

            case AlertType.VS:
              uValue = _altitude_metric ? (float)Mps_From_Ftpm( (float)value ) : (float)value;
              if (value <= -5) {
                this.Text = UnitString( RightAlign( $"{-uValue,4:###0}{c_do}" ) );
              }
              else if (value >= 5) {
                this.Text = UnitString( RightAlign( $"{uValue,4:###0}{c_up}" ) );
              }
              else {
                this.Text = UnitString( RightAlign( $"{0,4:###0}{c_flat}" ) );
              }
              break;

            case AlertType.SPD:
              uValue = _distance_metric ? (float)Kmh_From_Kt( (float)value ) : (float)value;
              this.Text = UnitString( RightAlign( $"{uValue,4:###0} " ) );
              break;

            case AlertType.DIST:
              uValue = _distance_metric ? (float)Km_From_Nm( (float)value ) : (float)value;
              this.Text = UnitString( RightAlign( $"{uValue,6:###0.0} " ) );
              break;

            case AlertType.TIME:
              uValue = (float)value;
              this.Text = UnitString( RightAlign( $"{Math.Truncate( uValue ),4:###0}:{(uValue - Math.Truncate( uValue )) * 60f:00} " ) );
              //this.Text = UnitString( RightAlign( $"{uValue,6:###0.0} " ) );
              break;

            default:
              this.Text = RightAlign( m_default ); // OFF
              break;
          }
        }
      }
    }

  }
}
