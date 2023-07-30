using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_SurfacesGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SURF_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RE-A";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Control Surfaces Graph";

    private readonly V_Base _label;
    private readonly A_Surfaces _surf1;

    public DI_SurfacesGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.SURF_ANI;
      _surf1 = new A_Surfaces( ) { };
      this.AddItem( _surf1 ); vCat.AddLbl( item, _surf1 );

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
        _surf1.Rudder_prct = SV.Get<float>( SItem.fG_Steer_Rudder_prct100 ); // +-1..0
        _surf1.Elevator_prct = SV.Get<float>( SItem.fG_Steer_Elevator_prct100 ); // +-1..0
        _surf1.Aileron_prct = SV.Get<float>( SItem.fG_Steer_Aileron_prct100 ); // +-1..0
      }
    }

  }
}
