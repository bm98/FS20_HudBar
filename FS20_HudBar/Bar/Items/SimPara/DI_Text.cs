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
  class DI_Text : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.TXT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "TXT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Free Text";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Text( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.TXT;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.Click += _label_Click;

    }

     // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      ; // not a Sim driven item
    }

   /// <summary>
    /// Enter the FreeText
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _label_Click( object sender, EventArgs e )
    {
      var TTX = new Config.frmText();
      TTX.ShowDialog( this );
      // Update in any case
      OnDataArrival( AppSettings.Instance.FreeText );
    }

    /// <summary>
    /// Update from FreeText Update
    /// </summary>
    private void OnDataArrival( string data )
    {
      if ( this.Visible ) {
        _value1.Text = data;
      }
    }

  }
}
