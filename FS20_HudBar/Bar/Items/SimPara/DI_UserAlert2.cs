using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using static FS20_HudBar.GUI.GUI_Colors.ColorType;
using static FSimClientIF.Sim;
using FS20_HudBar.GUI.Templates;

namespace FS20_HudBar.Bar.Items
{
  class DI_UserAlert2 : DI_UserAlert
  {

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static new readonly LItem LItem = LItem.USR_ALERT_2;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static new readonly string Short = "ALRT 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static new readonly string Desc = "Alert 2";

    public DI_UserAlert2( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Click to switch the Alert type";

      LabelID = LItem;
      var item = VItem.USR_ALERT_2;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      _value1 = new V_AlertValue( value2Proto ) { ItemForeColor = cTxSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _alert = (_value1 as V_AlertValue);
      _alert.AlertValueType = AlertType.OFF; // default

      item = VItem.USR_ALED_2;
      _led = new A_LEDbar( ) { ItemForeColor = cInfo }; // =>OFF
      this.AddItem( _led ); vCat.AddLbl( item, _led );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.Cursor = Cursors.Hand;

      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      _led.Click += _led_Click;
      _led.Cursor = Cursors.Hand;

      m_observerID = SV.AddObserver( Short, (int)DataArrival_perSecond / 2, OnDataArrival ); // twice per sec

    }

    // need a distinct method (using the generic will not register more than once)
    protected new void OnDataArrival( string dataRefName )
    {
      base.OnDataArrival( dataRefName );
    }

  }
}
