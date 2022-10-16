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
  class DI_MsFS : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.MSFS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "MSFS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "MSFS Status";

    private readonly B_Base _label;

    public DI_MsFS( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Click to change the text appearance\nSteps through Bright, Dim, Dark ";

      LabelID = LItem;
      var item = VItem.Ad;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      _label.ButtonClicked += _label_ButtonClicked;
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      NextColorset( ); // MSFS, rotate colorset
                       // save as setting
      AppSettings.Instance.Appearance = (int)Colorset;
      AppSettings.Instance.Save( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( )
    {
      // NOT USED
    }

  }
}
