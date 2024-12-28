using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_ATrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ATRIM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "A-Trim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aileron Trim";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    private const float c_incPerWheel = 0.002f; // Get 0.2% per mouse inc

    public DI_ATrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Aileron Trim value\nClick to reset to 0 %";

      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      // Aileron
      // All ERA-Trim label get a button to activate the 0 Trim action
      var item = VItem.ATRIM;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct_999( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.MouseWheel += _label_MouseWheel;
      _label.Scrollable = true;
      _label.Cursor = Cursors.SizeNS;

      AddObserver( Desc, 5, OnDataArrival );
    }

    private void _label_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      float current = SV.Get<float>( SItem.fGS_Trm_AileronTrim_prct100 );
      if (e.Delta > 0) {
        // Wheel Up - roll Right (same as mouse in Sim)
        SV.Set( SItem.fGS_Trm_AileronTrim_prct100, current + c_incPerWheel );
      }
      else if (e.Delta < 0) {
        // Wheel Down - roll Left (same as mouse in Sim)
        SV.Set( SItem.fGS_Trm_AileronTrim_prct100, current - c_incPerWheel );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        SV.Set( SItem.fGS_Trm_AileronTrim_prct100, 0f ); // Set 0
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        float trim = SV.Get<float>( SItem.fGS_Trm_AileronTrim_prct100 );
        _value1.Value = trim * 100f;
        // warn if beyond 90%
        _value1.ItemBackColor = (Math.Abs( trim ) > 0.9) ? GUI_Colors.ColorType.cWarnBG : GUI_Colors.ColorType.cBG;
      }
    }

  }
}

