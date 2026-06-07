using System;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Flaps Index or Degree with audible output
  /// </summary>
  internal class V_FlapsAudio : V_Base
  {
    public const float FlapsUpValue = -1000f;

    private Triggers.T_FlapsDeg _flapsCallout;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_FlapsAudio( Label proto, GUI_Speech gUI_SpeechRef )
    : base( proto )
    {
      m_unit = "°"; // starts with degree
      m_default = DefaultString( "+__ " + " " ); // nn + blank
      Text = UnitString( m_default );
      _flapsCallout = new Triggers.T_FlapsDeg( gUI_SpeechRef ) { Enabled = false }; // will be enabled once we get a value to report
      _flapsCallout.RegisterObserver( ); // this one is not in the HUDVoice List - so call it here
    }

    /// <summary>
    /// Must be unregistered !!
    /// </summary>
    public void UnregisterDataSource( )
    {
      _flapsCallout.UnRegisterObserver( );
    }

    /// <summary>
    /// Set the Flaps Handle Index Mode (else it's degree)
    /// </summary>
    /// <param name="indexMode">True for index mode, false for degree</param>
    public void SetIndexMode( bool indexMode )
    {
      _flapsCallout.SetIndexMode( indexMode );
      m_unit = indexMode ? " " : "°";
    }

    /// <summary>
    /// Set the value of the Control - formatted as +N0
    ///  SET 'FlapsUpValue' for UP
    /// </summary>
    override public float? Value {
      set {
        if (value == null) {
          this.Text = UnitString( RightAlign( m_default ) );
          _flapsCallout.Enabled = false;
        }
        else if (float.IsNaN( (float)value )) {
          this.Text = UnitString( RightAlign( m_default ) );
          _flapsCallout.Enabled = false;
        }
        else {
          // The Flaps callout will talk...
          _flapsCallout.Enabled = true;
          if (value <= FlapsUpValue) {
            this.Text = UnitString( $"UP {_cManaged}" ); // 2 chars: add a blank to aling better with ° values
          }
          else {
            this.Text = UnitString( $"{value,3:#0} {_cManaged}" ); // 2 chars: 2 digits , add a blank to aling better with ° values
          }
        }
      }
    }

  }
}