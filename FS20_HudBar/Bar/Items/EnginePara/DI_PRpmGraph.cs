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
  class DI_PRpmGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.PRPM_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "P-RPM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Propeller RPM Graph";

    private readonly V_Base _label;
    private readonly A_Scale _scale1;
    private readonly A_TwinScale _scale2;
    private readonly A_Scale _scale3;
    private readonly A_TwinScale _scale4;

    public DI_PRpmGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.P1_RPM_ANI;
      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      item = VItem.P2_RPM_ANI;
      _scale2 = new A_TwinScale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK, ItemForeColor_LScale = cOK };
      this.AddItem( _scale2 ); vCat.AddLbl( item, _scale2 );

      // add 2 more values
      //this.TwoRows = true;
      item = VItem.P3_RPM_ANI;
      _scale3 = new A_Scale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK };
      this.AddItem( _scale3 ); vCat.AddLbl( item, _scale3 );

      item = VItem.P4_RPM_ANI;
      _scale4 = new A_TwinScale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK, ItemForeColor_LScale = cOK };
      this.AddItem( _scale4 ); vCat.AddLbl( item, _scale4 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _scale1.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines == 1 ); // Single
        _scale2.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines > 1 );  // Twin Left
        _scale3.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines == 3 ); // Twin Left + Single Right
        _scale4.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines > 3 );  // Twin Left + Twin Right

        if ( _scale1.Visible ) {
          _scale1.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct; // 0..100
          _scale1.ItemBackColor = ( SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct > 102 ) ? cWarnBG : cBG;
        }

        if ( _scale2.Visible && !( _scale3.Visible || _scale4.Visible ) ) {
          _scale2.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct; // 0..100
          _scale2.ValueLScale = SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct; // 0..100
          _scale2.ItemBackColor = ( ( SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct > 102 ) ||
                                    ( SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct > 102 ) ) ? cWarnBG : cBG;
        }

        if ( _scale3.Visible ) {
          // reorder for 3 Engines
          _scale2.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct; // 0..100
          _scale2.ValueLScale = SC.SimConnectClient.Instance.HudBarModule.Propeller3_rpm_prct; // 0..100
          _scale2.ItemBackColor = ( ( SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct > 102 ) ||
                                    ( SC.SimConnectClient.Instance.HudBarModule.Propeller3_rpm_prct > 102 ) ) ? cWarnBG : cBG;

          _scale3.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct; // 0..100
          _scale3.ItemBackColor = ( SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct > 102 ) ? cWarnBG : cBG;
        }

        if ( _scale4.Visible ) {
          // reorder for 4 Engines
          _scale2.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct; // 0..100
          _scale2.ValueLScale = SC.SimConnectClient.Instance.HudBarModule.Propeller3_rpm_prct; // 0..100
          _scale2.ItemBackColor = ( ( SC.SimConnectClient.Instance.HudBarModule.Propeller1_rpm_prct > 102 ) ||
                                    ( SC.SimConnectClient.Instance.HudBarModule.Propeller3_rpm_prct > 102 ) ) ? cWarnBG : cBG;

          _scale4.Value = SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct; // 0..100
          _scale4.ValueLScale = SC.SimConnectClient.Instance.HudBarModule.Propeller4_rpm_prct; // 0..100
          _scale4.ItemBackColor = ( ( SC.SimConnectClient.Instance.HudBarModule.Propeller2_rpm_prct > 102 ) ||
                                    ( SC.SimConnectClient.Instance.HudBarModule.Propeller4_rpm_prct > 102 ) ) ? cWarnBG : cBG;
        }
      }
    }

  }
}



