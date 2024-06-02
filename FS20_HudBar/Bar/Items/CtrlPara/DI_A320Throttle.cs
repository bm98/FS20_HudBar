using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_A320Throttle : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.A320THR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "A320T";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "A320 Throttle";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    // pad right to avoid col juggling
    private const string c_tIDLE = "IDLE   ";
    private const string c_tATRH = "A/THR  ";
    private const string c_tCL = "CL     ";
    private const string c_tMCL = "FLX/MCT";
    private const string c_tTOGA = "TOGA   ";
    private const string c_tREV = "REV    ";

    public DI_A320Throttle( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.E1_A320THR;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.E2_A320THR;
      _value2 = new V_Text( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, (int)(DataArrival_perSecond / 5), OnDataArrival );
    }

    private void TSetValue( V_Text value, float tPerc )
    {
      if (tPerc > 99f) {
        // nominal 100%
        value.Text = c_tTOGA;
        value.ItemForeColor = cTxInfo;
      }
      else if (tPerc > 93f) {
        // nominal 95%
        value.Text = c_tMCL;
        value.ItemForeColor = cTxInfo;
      }
      else if (tPerc > 85f) {
        // nominal 89%
        value.Text = c_tCL;
        value.ItemForeColor = cTxInfo;
      }
      else if (tPerc > 1) {
        // nominal > 0%
        value.Text = c_tATRH;
        value.ItemForeColor = cTxInfo;
      }
      else if (tPerc > -1) {
        // nominal 0%
        value.Text = c_tIDLE;
        value.ItemForeColor = cTxInfo;
      }
      else {
        // nominal < 0% .. -20%
        value.Text = c_tREV;
        value.ItemForeColor = cTxWarn;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        TSetValue( _value1 as V_Text, SV.Get<float>( SItem.fG_Thr_Lever1_prct ) );
        TSetValue( _value2 as V_Text, SV.Get<float>( SItem.fG_Thr_Lever2_prct ) );
      }
    }

  }
}

