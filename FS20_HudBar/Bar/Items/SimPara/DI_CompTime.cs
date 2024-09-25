using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_CompTime : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.CTIME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "C-CLK";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Time of day (Computer)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_CompTime( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.CTIME;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Clock( value2Proto ) { ItemForeColor = cTxDim };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      // just need a ping to update - not taking data from the Module
      AddObserver( Short, 3, OnDataArrival ); // 3/sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = (int)DateTime.Now.TimeOfDay.TotalSeconds;
      }
    }

  }
}
