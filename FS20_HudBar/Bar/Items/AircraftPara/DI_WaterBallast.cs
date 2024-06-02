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
  internal class DI_WaterBallast : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.WBALLAST_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "W-BAL";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Water Ballast lb, gph";

    private readonly V_Base _label;
    private readonly A_Scale _scaleQuan;
    private readonly A_Scale _scaleFlow;

    public DI_WaterBallast( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.WBALLAST_ANI;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scaleQuan = new A_Scale( ) { Minimum = 0, Maximum = 60000, AlertValue = 0, ItemForeColor_Alert = cWarn }; // limits are set while updating
      this.AddItem( _scaleQuan ); vCat.AddLbl( item, _scaleQuan );

      item = VItem.WFLOW_ANI;
      _scaleFlow = new A_Scale( ) { Minimum = 0, Maximum = 500, AlertValue = 0, ItemForeColor_Alert = cWarn, BorderStyle = BorderStyle.FixedSingle };
      this.AddItem( _scaleFlow ); vCat.AddLbl( item, _scaleFlow );

      AddObserver( Short, (int)(DataArrival_perSecond / 1), OnDataArrival); // once per sec
    }

    // flow has strange units should be gallons/hour but is very small ~0.00013 for 0.055 gal/sec
    // so we try to establish the max from the seen flow
    private const float c_flowFact = 100_000f; // this may change if ASOBO fixes the rate calc
    private float _maxFlow = 0.0001f * c_flowFact;
    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        var ds = SV;
        // we need to set the visibility and limits each time as we cannot guess when a new plane is loaded..
        _scaleQuan.Visible = SV.Get<int>( SItem.iG_Acft_NumWaterBallastTanks_num ) > 0;
        _scaleFlow.Visible = _scaleQuan.Visible;

        // Value update
        // Quantity
        if (_scaleQuan.Visible) {
          _scaleQuan.Maximum = SV.Get<float>( SItem.fG_Acft_WaterBallast_capacity_pound );
          _scaleQuan.Value = SV.Get<float>( SItem.fG_Acft_WaterBallast_quantity_pound );
        }

        // Flow
        if (_scaleFlow.Visible) {
          var flow = SV.Get<float>( SItem.fG_Acft_WaterBallast_flowrate_gph ) * c_flowFact; // scale up for the graph use
          _maxFlow = (flow > _maxFlow) ? flow : _maxFlow;
          _scaleFlow.Maximum = _maxFlow;
          _scaleFlow.Value = flow;

          // Color when there is a flow out of water
          if (flow > 1) {
            _scaleQuan.ItemForeColor = cWarn;
            _scaleFlow.ItemForeColor = cWarn;
          }
          else {
            _scaleQuan.ItemForeColor = cOK;
            _scaleFlow.ItemForeColor = cOK;
          }
        }

      }
    }

  }
}