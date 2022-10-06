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
  class DI_Xpdr : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.XPDR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "XPDR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Transponder Code/Status";

    private readonly V_Base _label;
    private readonly V_Base _value1; // Code
    private readonly V_Base _value2; // Status

    public DI_Xpdr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.XPDR_CODE;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.XPDR_STAT;
      _value2 = new V_Text( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.ComModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.ComModule.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        var stat = SC.SimConnectClient.Instance.ComModule.Transponder_status;
        if ( SC.SimConnectClient.Instance.ComModule.Transponder_Available ) {
          _value1.Text = $"{SC.SimConnectClient.Instance.ComModule.Transponder_code:0000}";
          _value2.Text = $"{stat}";
          if ( stat == FSimClientIF.TransponderStatus.ALT ) {
            _value1.ItemForeColor = cNav;
            _value2.ItemForeColor = cNav;
          }
          else {
            _value1.ItemForeColor = cInfo;
            _value2.ItemForeColor = cInfo;
          }

        }
        else {
          _value1.Text = null;
          _value2.Text = null;
          _value1.ItemForeColor = cInfo;
          _value2.ItemForeColor = cInfo;
        }
      }
    }

  }
}

