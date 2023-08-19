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
using FSimClientIF;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ap_ABrake : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_ABRK;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ABRK";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP Auto Brake";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    // align size with ABRK to make it look pleasant.. (m_alignWidth chars, same as other AP values)

    private const string c_aSkid = " a-skid  ";


    public DI_Ap_ABrake( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Auto Brake\nClick to toggle";

      LabelID = LItem;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.AP_ABRK_armed;
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxDim, ItemBackColor = cValBG, Text = AutoBrakeLevel.OFF.ToString( ).PadRight( m_alignWidth ) };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.Click += _value1_Click;
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      item = VItem.AP_ASKID;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxDim, Text = c_aSkid };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    private void _value1_Click( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (SV.Get<bool>( SItem.bG_Ap_ABRK_active )) {
        SV.Set( SItem.cmS_Ap_ABRK_set, CmdMode.Off );
      }
    }

    // Inc/Dec Standby Frequ
    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (e.Delta > 0 && SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ) < AutoBrakeLevel.HIGH_4) {
        SV.Set( SItem.cmS_Ap_ABRK_set, CmdMode.Inc );
      }
      else if (e.Delta < 0 && SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ) > AutoBrakeLevel.RTO) {
        SV.Set( SItem.cmS_Ap_ABRK_set, CmdMode.Dec );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Ap_ABRK_active )) {
          _value1.ItemForeColor = cTxActive;
          _value1.Text = SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ).ToString( ).PadRight( m_alignWidth );
        }
        else {
          if (SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ) == AutoBrakeLevel.OFF) {
            _value1.ItemForeColor = cTxDim;
            _value1.Text = SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ).ToString( ).PadRight( m_alignWidth );
          }
          else {
            _value1.ItemForeColor = cTxSet;
            _value1.Text = SV.Get<AutoBrakeLevel>( SItem.ablGS_Ap_ABRK_level ).ToString( ).PadRight( m_alignWidth );
          }
        }

        // Anti Skid (A320 only it seems)
        _value2.Text = SV.Get<bool>( SItem.bG_Ap_ASKID_active ) ? c_aSkid.ToUpperInvariant( ) : c_aSkid;
        _value2.ItemForeColor = SV.Get<bool>( SItem.bG_Ap_ASKID_active ) ? cTxActive : cTxDim;
      }
    }

  }
}

