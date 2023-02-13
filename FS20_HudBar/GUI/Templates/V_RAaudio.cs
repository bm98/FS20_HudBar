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
  /// Radio Altitude with audible output
  /// </summary>
  class V_RAaudio : V_Base
  {
    private Triggers.T_RAcallout m_raCallout;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_RAaudio( Label proto, GUI_Speech gUI_SpeechRef )
    : base( proto )
    {
      m_unit = "ft";
      m_default = DefaultString( "+__'___ " + " " ); // -nn,nnn + blank
      Text = UnitString( m_default );
      m_raCallout = new Triggers.T_RAcallout( gUI_SpeechRef ) { Enabled = false }; // will be enabled once we get a value to report
      m_raCallout.RegisterObserver( ); // this one is not in the HUDVoice List - so call it here
    }

    /// <summary>
    /// Must be unregistered !!
    /// </summary>
    public void UnregisterDataSource( )
    {
      m_raCallout.UnRegisterObserver( );
    }


    protected override void SetAltitude_Metric( )
    {
      m_unit = _altitude_metric ? "m" : "ft";
      m_raCallout.SetMetric( _altitude_metric ); // must change the Callout Obj as well
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if (value == null) {
          this.Text = UnitString( RightAlign( m_default ) );
          m_raCallout.Enabled = false;
        }
        else if (float.IsNaN( (float)value )) {
          this.Text = UnitString( RightAlign( m_default ) );
          m_raCallout.Enabled = false;
        }
        else {
          // The RA callout will talk...
          m_raCallout.Enabled = true;

          float uValue = _altitude_metric ? (float)M_From_Ft( (float)value ) : (float)value;
          this.Text = UnitString( RightAlign( $"{uValue,7:##,##0} {_cManaged}" ) ); // 9 chars: sign + 5 digits + 1000 separator, add a blank to aling better with ° values
        }
      }
    }

  }
}