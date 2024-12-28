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
  class DI_FuelGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "F-CLR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fuel Graph C-LR";

    private readonly V_Base _label;
    private readonly A_Scale _scaleC;
    private readonly A_TwinScale _scaleLR;

    public DI_FuelGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.GraphX2;
      var item = VItem.FUEL_ANI_C;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scaleC = new A_Scale( ) { Minimum = 0, Maximum = 60000, AlertValue = 1, ItemForeColor_Alert = cWarn }; // limits are set while updating
      this.AddItem( _scaleC ); vCat.AddLbl( item, _scaleC );

      item = VItem.FUEL_ANI_LR;
      _scaleLR = new A_TwinScale( ) { Minimum = 0, Maximum = 60000, AlertValue = 1, ItemForeColor_Alert = cWarn, BorderStyle = BorderStyle.FixedSingle };
      this.AddItem( _scaleLR ); vCat.AddLbl( item, _scaleLR );

      AddObserver( Desc, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // we need to set the visibility and limits each time as we cannot guess when a new plane is loaded..
        _scaleC.Visible = (SV.Get<float>( SItem.fG_Fuel_Capacity_center_gal ) > 0);
        _scaleLR.Visible = ((SV.Get<float>( SItem.fG_Fuel_Capacity_left_gal ) > 0)
                           || (SV.Get<float>( SItem.fG_Fuel_Capacity_right_gal ) > 0));

        // Value update
        // Check Center Tank
        if (_scaleC.Visible) {
          _scaleC.AlertValue = _scaleC.Minimum; // reset before setting Max to avoid Argument exceptions when setting max
          _scaleC.Maximum = SV.Get<float>( SItem.fG_Fuel_Capacity_center_gal );
          _scaleC.AlertValue = SV.Get<float>( SItem.fG_Fuel_Capacity_center_gal ) / 4;
          _scaleC.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_center_gal );
        }

        // Handle LR Tanks with one Gauge
        if (_scaleLR.Visible) {
          var cap = Math.Max( SV.Get<float>( SItem.fG_Fuel_Capacity_left_gal ),
                                SV.Get<float>( SItem.fG_Fuel_Capacity_right_gal ) );

          _scaleLR.AlertValue = _scaleLR.Minimum; // reset before setting Max to avoid Argument exceptions when setting max
          _scaleLR.Maximum = cap;
          _scaleLR.AlertValue = cap / 4;
          _scaleLR.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_left_gal );
          _scaleLR.ValueLScale = SV.Get<float>( SItem.fG_Fuel_Quantity_right_gal );

          // Color when there is a substantial unbalance
          if (Calculator.HasFuelImbalance) {
            _scaleLR.ItemForeColor = cAlert;
            _scaleLR.ItemForeColor_LScale = cAlert;
          }
          else {
            _scaleLR.ItemForeColor = cOK;
            _scaleLR.ItemForeColor_LScale = cOK;
          }
        }

      }
    }

  }
}
