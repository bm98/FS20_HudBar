using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FSimClientIF;

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

    // align size with ABRK to make it look pleasant.. (m_alignWidth chars, same as other AP values)
    private const string c_active = "ACTIVE   ";
    private readonly string c_activeM = $"ACTIVE  {GUI_Fonts.ManagedTag}";
    private const string c_armed = "ARMED    ";
    private const string c_off = "OFF      ";
    private const string c_toga = "  toga   ";


    public DI_Ap_AThrottle( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Auto Throttle / TOGA\nClick to toggle";

      LabelID = LItem;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.AP_ATHR_armed;
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxLabel, ItemBackColor = cValBG, Text = c_off };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.Click += _value1_Click;
      _value1.Cursor = Cursors.Hand;

      item = VItem.AP_ATHR_toga;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxLabel, ItemBackColor = cValBG, Text = c_toga };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );
      _value2.Click += _value2_Click;
      _value2.Cursor = Cursors.Hand;

      AddObserver( Short, 5, OnDataArrival );
    }

    private void _value1_Click( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.S_Ap_ATHR_toggle, true ); // toggles
    }

    private void _value2_Click( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.bGS_Ap_TOGA_active, true ); // toggles
    }


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        var atr = SV.Get<AutoThrottleMode>( SItem.athG_Ap_ATHR_mode );
        if (atr == AutoThrottleMode.Active) {
          _value1.ItemForeColor = cTxActive;
          _value1.Text = c_active;
        }
        else if (atr == AutoThrottleMode.Armed) {
          _value1.ItemForeColor = cTxSet;
          _value1.Text = c_armed;
        }
        else {
          _value1.ItemForeColor = cTxDim;
          _value1.Text = c_off;
        }

        // TOGA
        _value2.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_TOGA_active ) ? cTxActive : cTxDim;
        _value2.Text = SV.Get<bool>( SItem.bGS_Ap_TOGA_active ) ? c_toga.ToUpperInvariant( ) : c_toga;

      }
    }

  }
}
