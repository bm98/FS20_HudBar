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
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ap_ApproachMode : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_APR_INFO;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "AP.APR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP/GPS Approach Info";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Ap_ApproachMode( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.AP_APR_INFO;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

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
        ColorType fCol; // default color
        // The Approach Type 
        string info = $"{SV.Get<APRtype>( SItem.aptG_Gps_APR_type )}"; // None, GPS, RNAV, ILS, VOR .... see enum
        if (SV.Get<APRtype>( SItem.aptG_Gps_APR_type ) == APRtype.GPS) fCol = cTxGps;
        else if (SV.Get<APRtype>( SItem.aptG_Gps_APR_type ) == APRtype.RNAV) fCol = cTxGps;
        else fCol = cTxNav;
        // The Approach Phase
        if (SV.Get<bool>( SItem.bGS_Ap_APRhold_on )
             && (SV.Get<APRmode>( SItem.apmG_Gps_APR_mode ) != APRmode.None)) {
          info += $" - {SV.Get<APRmode>( SItem.apmG_Gps_APR_mode )}"; // Transition, Final, Missed
        }
        _value1.Text = info;
        _value1.ItemForeColor = fCol;
      }
    }

  }
}

