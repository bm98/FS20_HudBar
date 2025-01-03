﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_Nav2_Active : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV2;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-2 Id BRG DME";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Nav2_Active( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.NAV2_ID;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( valueProto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.NAV2_BRG;
      _value2 = new V_Deg( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.NAV2_DST;
      _value3 = new V_DmeDist( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      AddObserver( Desc, 2, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (!SV.Get<bool>( SItem.bG_Nav_2_available )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          _value2.Text = "";
          _value3.Text = "";
          return;
        }

        // Has NAV2
        this.Label.Text = SV.Get<bool>( SItem.bG_Nav_2_hasLOC ) ? "LOC 2" : "NAV 2";
        if (SV.Get<string>( SItem.sG_Nav_2_Ident ) != "") {
          _value1.ItemForeColor = SV.Get<bool>( SItem.bG_Nav_Source_NAV2 ) ? cTxNav : cTxInfo;
          _value1.Text = Calculator.NAV2_ID;

          var brg = (float)Geo.Wrap360( SV.Get<float>( SItem.fG_Nav_2_Radial_degm ) - 180 ); // direction towards the station 
          brg = SV.Get<bool>( SItem.bG_Nav_2_hasLOC ) ? SV.Get<float>( SItem.fG_Nav_2_CRS ) : brg; // for LOC use the LOC heading as direction

          if (SV.Get<bool>( SItem.bG_Nav_2_hasSignal ) && SV.Get<FromToFlag>( SItem.ftfG_Nav_2_fromToFlag ) != FromToFlag.OFF) {
            _value2.Value = brg;
          }
          else {
            _value2.Value = null;
          }

          if (SV.Get<bool>( SItem.bG_Nav_2_hasDME )) {
            var diversion = Geo.Wrap180( SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm ) - brg ); // delta < +-90 -> going towards the Station, outside going away from it
            var toFromFlag = (Math.Abs( diversion ) <= 90) ? 1 : 2; // same values as the Sim ToFrom Flag (1->To, 2-> From, 0->Off)
            toFromFlag = SV.Get<bool>( SItem.bG_Nav_2_hasSignal ) ? toFromFlag : 0; // no signal -> direction Off

            _value3.Value =
                  Conversions.DmeDistance( SV.Get<float>( SItem.fG_Nav_2_DMEdist_nm ), toFromFlag );
          }
          else {
            _value3.Value = null;
          }
        }
        else {
          _value1.ItemForeColor = cTxInfo;
          _value1.Text = $"{SV.Get<int>( SItem.iG_Nav_2_active_hz ) / 1_000_000f:000.00}";
          _value2.Value = null;
          _value3.Value = null;
        }
      }
    }

  }
}

