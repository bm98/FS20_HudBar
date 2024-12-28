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
using static FS20_HudBar.GUI.GUI_Fonts;
using static FSimClientIF.Sim;
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_HTrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.H_TRIM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "H-Trim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Heli Trim (lat,lon)";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_HTrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Heli Trim value\nClick to reset to 0 %";

      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      // All ERA-Trim label get a button to activate the 0 Trim action
      var item = VItem.HTRIM_LON;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct_999( value2Proto ) { ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.HTRIM_LAT;
      _value2 = new V_Prct_999( value2Proto ) { ItemBackColor = cValBG };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;

      _value1.MouseClick += _value1_MouseClick;
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.NoMoveVert;

      _value2.MouseClick += _value2_MouseClick;
      _value2.MouseWheel += _value2_MouseWheel;
      _value2.Scrollable = true;
      _value2.Cursor = Cursors.NoMoveVert;

      AddObserver( Desc, 5, OnDataArrival);
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      float current = SV.Get<float>( SItem.fGS_Trm_RotorLongitudinalTrim_prct100 );
      if (e.Delta > 0) {
        // Wheel Up - Dec Long Trim - 'nose down'
        SV.Set( SItem.cmS_Trm_RotorLongitudinalTrim, CmdMode.Dec );
      }
      else if (e.Delta < 0) {
        // Wheel Down- Inc Long Trim - 'nose up'
        SV.Set( SItem.cmS_Trm_RotorLongitudinalTrim, CmdMode.Inc );
      }
    }

    private void _value2_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      float current = SV.Get<float>( SItem.fGS_Trm_RotorLateralTrim_prct100 );
      if (e.Delta > 0) {
        // Wheel Up - Dec Lat Trim - 'left '
        SV.Set( SItem.cmS_Trm_RotorLateralTrim, CmdMode.Dec );
      }
      else if (e.Delta < 0) {
        // Wheel Down- Inc Lat Trim - 'right'
        SV.Set( SItem.cmS_Trm_RotorLateralTrim, CmdMode.Inc );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.bS_Trm_RotorReset, true ); // Reset both
    }

    private void _value1_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.fGS_Trm_RotorLongitudinalTrim_prct100, 0f ); // Set 0
    }

    private void _value2_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.fGS_Trm_RotorLateralTrim_prct100, 0f ); // Set 0
    }


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        float trimLon = SV.Get<float>( SItem.fGS_Trm_RotorLongitudinalTrim_prct100 );
        float trimLat = SV.Get<float>( SItem.fGS_Trm_RotorLateralTrim_prct100 );
        _value1.Value = trimLon * 100f;
        _value2.Value = trimLat * 100f;
        // warn if beyond 90%
        _value1.ItemBackColor = (Math.Abs( trimLon ) > 0.9) ? cWarnBG : cValBG;
        _value2.ItemBackColor = (Math.Abs( trimLat ) > 0.9) ? cWarnBG : cValBG;
      }
    }

  }
}

