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
  class DI_Ias : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.IAS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "IAS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft IAS kt";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Ias( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.IAS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Speed( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

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


    // Retuns a ColorType for the IAS based on DesignSpeeds
    private ColorType IAScolor( )
    {
      if ( SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround ) return cInfo;

      if ( SC.SimConnectClient.Instance.HudBarModule.Overspeed_warn ) return cAlert;
      if ( SC.SimConnectClient.Instance.HudBarModule.Stall_warn ) return cAlert;

      var flapsSpeed = (SC.SimConnectClient.Instance.HudBarModule.FlapsDeployment_prct> 0.8 )
                        ? SC.SimConnectClient.Instance.HudBarModule.DesingSpeedVS0_kt
                        : SC.SimConnectClient.Instance.HudBarModule.DesingSpeedVS1_kt;
      if ( SC.SimConnectClient.Instance.HudBarModule.IAS_kt <= flapsSpeed ) return cAlert;
      if ( SC.SimConnectClient.Instance.HudBarModule.IAS_kt <= ( flapsSpeed + 5 ) ) return cWarn; // within 5kts of Flaps speed

      return cInfo;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.IAS_kt;
        _value1.ItemForeColor = IAScolor( );
      }
    }

  }
}
