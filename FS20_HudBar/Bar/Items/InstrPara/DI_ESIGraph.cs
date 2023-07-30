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
  class DI_ESIGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ESI_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ESI";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ESI Panel";

    private readonly V_Base _label;
    private readonly A_AHor _scale1;
    private readonly A_FPA _scale2;

    public DI_ESIGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.ATT_ANI;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scale1 = new A_AHor( ) { Minimum = -10, Maximum = 10, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      item = VItem.FPA_ANI;
      _scale2 = new A_FPA( ) { MinimumVer = -6, MaximumVer = 6, MinimumHor = -6 * 2, MaximumHor = 6 * 2, ItemForeColor = cOK };
      this.AddItem( _scale2 ); vCat.AddLbl( item, _scale2 );

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
        _scale1.PitchAngle = SV.Get<float>( SItem.fG_Acft_Pitch_deg );
        _scale1.BankAngle = SV.Get<float>( SItem.fG_Acft_Bank_deg );

        _scale2.VerticalAngle = SV.Get<float>( SItem.fG_Acft_FlightPathAngle_deg );
        _scale2.HorizontalAngle = (float)CoordLib.Geo.Wrap180(
            SV.Get<float>( SItem.fG_Gps_GTRK_true_deg ) - SV.Get<float>( SItem.fG_Gps_GHDG_true_deg )
        ); // +-180°
      }
    }

  }
}
