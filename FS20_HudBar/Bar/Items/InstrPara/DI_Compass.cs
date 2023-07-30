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
  class DI_Compass : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.COMPASS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "COMP";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Compass degm";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly A_WindArrow _comp;

    public DI_Compass( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // Wind Direction, Speed
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.COMPASS;
      _value1 = new V_Deg( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.COMPASS_ANI;
      _comp = new A_WindArrow( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true, ItemForeColor = cInfo };
      this.AddItem( _comp ); vCat.AddLbl( item, _comp );

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
        _value1.Value = SV.Get<float>( SItem.fG_Nav_Compass_degm );
        _comp.DirectionFrom = (int)SV.Get<float>( SItem.fG_Nav_Compass_degm );
        _comp.Heading = 180;
      }
    }

  }
}
