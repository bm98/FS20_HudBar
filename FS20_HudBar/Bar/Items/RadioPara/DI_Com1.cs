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
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Com1 : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.COM1;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "COM 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "COM-1 Stdby Active";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Com1( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.COM1_SWAP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Click += _label_Click;

      item = VItem.COM1_STDBY;
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Cursor = Cursors.SizeNS;

      item = VItem.COM1_ACTIVE;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxNav };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.ComModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.ComModule ); // use the generic one
    }

    // Inc/Dec Standby Frequ
    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      _value1.ActivateForm( e );

      // 1/2 - 1/2  dectection for Digits
      var whole = e.Location.X < ( _value1.Width / 2 );

      if ( e.Delta > 0 ) {
        SC.SimConnectClient.Instance.ComModule.COM1set( whole ? FSimClientIF.CmdMode.Inc : FSimClientIF.CmdMode.Inc_Fract );
      }
      else if ( e.Delta < 0 ) {
        SC.SimConnectClient.Instance.ComModule.COM1set( whole ? FSimClientIF.CmdMode.Dec : FSimClientIF.CmdMode.Dec_Fract );
      }
    }

    // Swap COM Active - Standby
    private void _label_Click( object sender, EventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      SC.SimConnectClient.Instance.ComModule.COM1set( FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Text = $"{SC.SimConnectClient.Instance.ComModule.COM1_stdby_hz / 1_000_000f:000.000}";
        _value2.Text = $"{SC.SimConnectClient.Instance.ComModule.COM1_active_hz / 1_000_000f:000.000}";
      }
    }

  }
}

