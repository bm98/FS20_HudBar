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
  /// Radio Altitude with audible output
  /// </summary>
  class V_RAaudio : V_Base
  {
    private Triggers.T_RAcallout m_raCallout;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_RAaudio( Label proto, bool showUnit, GUI_Speech gUI_SpeechRef )
    : base( proto, showUnit )
    {
      m_unit = "ft";
      m_default = DefaultString( "+__'___ " );
      Text = UnitString( m_default );
      m_raCallout = new Triggers.T_RAcallout( gUI_SpeechRef ) { Enabled = false }; // will be enabled once we get a value to report
      m_raCallout.RegisterObserver( ); // this one is not in the HUDVoice List - so call it here
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
          this.Text = UnitString( $"{value,7:##,##0} " ); // sign 5 digits, 1000 separator, add a blank to aling better
          // The RA callout will talk...
          m_raCallout.Enabled = true;
        }
      }
    }

  }
}