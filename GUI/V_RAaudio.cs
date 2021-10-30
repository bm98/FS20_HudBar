using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Radio Altitude with audible output
  /// </summary>
  class V_RAaudio : V_Base
  {

    private GUI_Speech m_gUI_SpeechRef = null;
    private int m_prevVal = -1000;
    private List<int> m_said = new List<int>();

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
      m_gUI_SpeechRef = gUI_SpeechRef;
      m_raCallout = new Triggers.T_RAcallout( gUI_SpeechRef ) { Enabled = true }; // must be enabled
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
          // The RA callout should talk...
          m_raCallout.UpdateState( SimConnectClient.SimConnectClient.Instance.AircraftModule );

          //Say( (float)value );
        }
      }
    }


    // say a value and take care to not repeat it
    private void SayOnce( int value )
    {
      if ( m_said.Contains( value ) ) return; // already said
      m_gUI_SpeechRef.Say( value );
      m_said.Add( value ); // store
    }

    // Decides on the speech output 
    private void Say( float value )
    {
      if ( m_gUI_SpeechRef == null ) return; // Nope
      int iVal = (int)value;

      // GPSs say 500 so we start at 400 to not interfere
      // Logic - output  400, 300, 200, 100, 50, 40, 30, 20, 10
      // should only talk while going down.. and start at 5..3 above the altitude, else it's too late
      // one neeed to be above 405 to restart the down ladder

      // above audible RA 
      if ( iVal > 405 ) {
        // clear said RAs and return
        if ( m_said.Count > 0 )
          m_said.Clear( );
        m_prevVal = 406; // next checkIn RA
        return;
      }

      // not yet at next CheckIn RA - go around
      if ( iVal >= m_prevVal ) {
        return;
      }

      // going down now
      if ( iVal <= 405 && iVal > 400 ) {
        SayOnce( 400 );
        m_prevVal = 306; // next checkIn RA
      }
      else if ( iVal <= 305 && iVal > 300 ) {
        SayOnce( 300 );
        m_prevVal = 206; // next checkIn RA
      }
      else if ( iVal <= 205 && iVal > 200 ) {
        SayOnce( 200 );
        m_prevVal = 106; // next checkIn RA
      }
      else if ( iVal <= 105 && iVal > 100 ) {
        SayOnce( 100 );
        m_prevVal = 54; // next checkIn RA
      }
      else if ( iVal <= 53 && iVal > 50 ) {
        SayOnce( 50 );
        m_prevVal = 44; // next checkIn RA
      }
      else if ( iVal <= 43 && iVal > 40 ) {
        SayOnce( 40 );
        m_prevVal = 34; // next checkIn RA
      }
      else if ( iVal <= 33 && iVal > 30 ) {
        SayOnce( 30 );
        m_prevVal = 24; // next checkIn RA
      }
      else if ( iVal <= 23 && iVal > 20 ) {
        SayOnce( 20 );
        m_prevVal = 14; // next checkIn RA
      }
      else if ( iVal <= 13 && iVal > 10 ) {
        SayOnce( 10 );
        m_prevVal = 4; // next checkIn RA
      }
      else {
        m_prevVal = iVal; // skipped the ladder for any reason -set current as new checkIn RA
      }

    }

  }
}