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
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        ColorType fCol = cInfo; // default color
        // The Approach Type 
        string info = $"{SC.SimConnectClient.Instance.GpsModule.GpsAPR_Type}"; // None, GPS, RNAV, ILS, VOR .... see enum
        if ( SC.SimConnectClient.Instance.GpsModule.GpsAPR_Type == FSimClientIF.APRtype.GPS ) fCol = cGps;
        else if ( SC.SimConnectClient.Instance.GpsModule.GpsAPR_Type == FSimClientIF.APRtype.RNAV ) fCol = cGps;
        else fCol = cNav;
        // The Approach Phase
        if ( SC.SimConnectClient.Instance.GpsModule.IsGpsAPR_active
             && ( SC.SimConnectClient.Instance.GpsModule.GpsAPR_Mode != FSimClientIF.APRmode.None ) ) {
          info += $" - {SC.SimConnectClient.Instance.GpsModule.GpsAPR_Mode}"; // Transition, Final, Missed
        }
        _value1.Text = info;
        _value1.ItemForeColor = fCol;
      }
    }

  }
}

