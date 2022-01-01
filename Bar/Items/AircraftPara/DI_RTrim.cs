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
  class DI_RTrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.RTRIM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "R-Trim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Rudder Trim";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    private const float c_incPerWheel = 0.002f; // Get 0.2% per mouse inc

    public DI_RTrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Rudder Trim value\nClick to reset to 0 %";

      LabelID = LItem;
      // Rudder
      // All ERA-Trim label get a button to activate the 0 Trim action
      var item = VItem.RTRIM;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct_999( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.MouseWheel += _label_MouseWheel;
      _label.Cursor = Cursors.SizeNS;

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    private void _label_MouseWheel( object sender, MouseEventArgs e )
    {
      if ( e.Delta > 0 ) {
        // Wheel Up - yaw Right (same as mouse in Sim)
        SC.SimConnectClient.Instance.HudBarModule.RudderTrim_prct += c_incPerWheel;
      }
      else if ( e.Delta < 0 ) {
        // Wheel Down- yaw Left (same as mouse in Sim)
        SC.SimConnectClient.Instance.HudBarModule.RudderTrim_prct -= c_incPerWheel;
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.HudBarModule.RudderTrim_prct = 0; // Set 0
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.RudderTrim_prct;
      }
    }

  }
}

