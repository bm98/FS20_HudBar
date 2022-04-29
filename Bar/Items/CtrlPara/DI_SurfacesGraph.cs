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
      _label = new L_Text(lblProto) { Text = Short }; this.AddItem(_label);

      var item = VItem.SURF_ANI;
      _surf1 = new A_Surfaces() { };
      this.AddItem(_surf1); vCat.AddLbl(item, _surf1);

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver(Short, OnDataArrival);
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible)
      {
        _surf1.Rudder_prct = SC.SimConnectClient.Instance.HudBarModule.Rudder_prct; // +-1..0
        _surf1.Elevator_prct = SC.SimConnectClient.Instance.HudBarModule.Elevator_prct; // +-1..0
        _surf1.Aileron_prct = SC.SimConnectClient.Instance.HudBarModule.Aileron_prct; // +-1..0
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver(m_observerID);
    }

  }
}
