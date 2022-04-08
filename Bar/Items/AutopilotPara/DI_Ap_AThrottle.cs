using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ap_AThrottle : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_ATHR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ATHR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP ATHR, TOGA";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    // align size with ABRK to make it look pleasant.. (8 chars for now)
    private const string c_active = "active  ";
    private const string c_armed  = "armed   ";
    private const string c_off    = " off    ";
    private const string c_toga   = "  toga  ";


    public DI_Ap_AThrottle( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Auto Throttle / TOGA\nClick to toggle";

      LabelID = LItem;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.AP_ATHR_armed;
      _value1 = new V_Text( value2Proto ) { ItemBackColor = cValBG, Text = c_off };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.Click += _value1_Click;
      _value1.Cursor = Cursors.Hand;

      item = VItem.AP_ATHR_toga;
      _value2 = new V_Text( value2Proto ) { ItemBackColor = cValBG, Text = c_toga };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );
      _value2.Click += _value2_Click;
      _value2.Cursor = Cursors.Hand;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void _value1_Click( object sender, EventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      if ( SC.SimConnectClient.Instance.AP_G1000Module.ATHR_active || SC.SimConnectClient.Instance.AP_G1000Module.ATHRmanaged_active ) {
        SC.SimConnectClient.Instance.AP_G1000Module.ATHR_disconnect( );
      }
      else {
        SC.SimConnectClient.Instance.AP_G1000Module.ATHR_armed_toggle( );
      }
    }

    private void _value2_Click( object sender, EventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      SC.SimConnectClient.Instance.AP_G1000Module.TOGA_toggle( );
    }


    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.AP_G1000Module.ATHR_active ) {
          _value1.ItemForeColor = cNav;
          _value1.Text = c_active;  
        }
        else if ( SC.SimConnectClient.Instance.AP_G1000Module.ATHRmanaged_active ) {
          _value1.ItemForeColor = cNav;
          _value1.Text = c_active;
        }
        else if ( SC.SimConnectClient.Instance.AP_G1000Module.ATHR_armed ) {
          _value1.ItemForeColor = cSet;
          _value1.Text = c_armed;
        }
        else {
          _value1.ItemForeColor = cLabel;
          _value1.Text = c_off;
        }

        if ( SC.SimConnectClient.Instance.AP_G1000Module.TOGA_active ) {
          _value2.ItemForeColor = cNav;
          _value2.Text = c_toga.ToUpperInvariant();
        }
        else {
          _value2.ItemForeColor = cLabel;
          _value2.Text = c_toga;
        }


      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.AP_G1000Module.RemoveObserver( m_observerID );
    }

  }
}
