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
  class DI_Gforce_MM : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GFORCE_MM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "G-MM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "G Force Min, Max";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Gforce_MM( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "G Force min/max\nClick to reset";

      LabelID = LItem;
      var item = VItem.GFORCE_Min;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Gforce_MM_ButtonClicked;
      _value1 = new V_GForce( value2Proto ) { ItemForeColor = cInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.GFORCE_Max;
      _value2 = new V_GForce( value2Proto ) { ItemForeColor = cLabel };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    private void DI_Gforce_MM_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      SC.SimConnectClient.Instance.HudBarModule.ResetGForceIndicator( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.GForceMin_g;
        _value2.Value = SC.SimConnectClient.Instance.HudBarModule.GForceMax_g;
      }
    }

  }
}

