﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Est_VS : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.EST_VS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "WP-VS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Estimate VS to WYP@ALT";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Est_VS( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.EST_VS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_VSpeedPerMin( valueProto ) { ItemForeColor = cTxEst };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Gps_FP_active )) {
          float tgtAlt = SV.Get<float>( SItem.fG_Gps_WYP_alt_ft );
          // Estimates use WYP ALT if >0 (there is no distinction if a WYP ALT is given - it is 0 if not)
          ColorType estCol = cTxEst;
          if (tgtAlt == 0) {
            // use Set Alt if WYP ALT is zero (see comment above)
            tgtAlt = SV.Get<float>( SItem.fGS_Ap_ALT_setting_ft );
            estCol = cTxSet;
          }
          _value1.Value = Calculator.VSToTgt_AtAltitude( tgtAlt, SV.Get<float>( SItem.fG_Gps_WYP_dist_nm ) );
          _value1.ItemForeColor = estCol;
        }
        else {
          _value1.Value = null; // cannot if we don't have a WYP to aim at
        }
      }
    }

  }
}
